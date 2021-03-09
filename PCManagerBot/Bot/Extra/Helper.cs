using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace PCManagerBot.Bot.Extra
{
    class Helper
    {
        // processing screenshot images
        private Bitmap ResizeImg(Bitmap bmp, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);

            using (Graphics g = Graphics.FromImage(result as Image))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, 0, 0, nWidth, nHeight);
            }

            return result;
        }

        public Bitmap ResizeImg(Bitmap bmp, int xN) => ResizeImg(bmp, bmp.Width / xN, bmp.Height / xN);

        private Bitmap GetPrintScreen()
        {
            Bitmap screen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(screen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, screen.Size);
            return ResizeImg(screen, 2);
        }

        public void HttpUploadScreen(string uri, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            // creat an http request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            Stream stream = request.GetRequestStream();

            string formdata = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                string formItem = String.Format(formdata, key, nvc[key]);
                byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);
                stream.Write(formItemBytes, 0, formItemBytes.Length);
            }
            stream.Write(boundaryBytes, 0, boundaryBytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = String.Format(headerTemplate, paramName, file, contentType);
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            stream.Write(headerBytes, 0, headerBytes.Length);

            Bitmap screen = GetPrintScreen();
            MemoryStream ms = new MemoryStream();
            screen.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;

            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            ms.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            stream.Write(trailer, 0, trailer.Length);
            stream.Close();

            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                Stream s = response.GetResponseStream();
                StreamReader reader = new StreamReader(s);
                Logs.WriteLog("File " + file + " is sent to server, server's response: " + reader.ReadToEnd());
            }
            catch (Exception ex)
            {
                Logs.WriteLog("Error occured while sending the file: " + ex.Message);
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
            finally
            {
                request = null;
            }
        }

        // size suffix, write kb, mb or gb according to value
        public string SizeSuffix(long value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        // processing applications (app names, installation paths and so on)
        private string ExistsInSubKey(RegistryKey root, string subKeyName, string attributeName, string nameOfAppToFind)
        {
            RegistryKey subkey;
            string displayName;

            using (RegistryKey key = root.OpenSubKey(subKeyName))
            {
                if (key != null)
                {
                    foreach (string kn in key.GetSubKeyNames())
                    {
                        using (subkey = key.OpenSubKey(kn))
                        {
                            displayName = subkey.GetValue(attributeName) as string;
                            if (!String.IsNullOrEmpty(displayName))
                            {
                                displayName = displayName.ToLower();
                                if (displayName.Contains(nameOfAppToFind))
                                {
                                    return subkey.GetValue("InstallLocation") as string;
                                }
                            }
                        }
                    }
                }
            }

            return "Can't open it";
        }

        public string GetApplictionInstallPath(string nameOfAppToFind)
        {
            string installedPath;
            string keyName;

            // search in: LocalMachine_32
            keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            installedPath = ExistsInSubKey(Registry.LocalMachine, keyName, "DisplayName", nameOfAppToFind);
            if (!string.IsNullOrEmpty(installedPath))
            {
                return installedPath;
            }

            return String.Empty;
        }

        // get the names of installed apps
        public string GetInstalledPrograms()
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();

            result.AddRange(GetInstalledProgramsFromRegistry(RegistryView.Registry32));
            result.AddRange(GetInstalledProgramsFromRegistry(RegistryView.Registry64));
            
            foreach (string s in result)
            {
                sb.AppendLine(s);
            }

            return sb.ToString();
        }

        private IEnumerable<string> GetInstalledProgramsFromRegistry(RegistryView registryView)
        {
            const string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            List<string> result = new List<string>();

            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (IsProgramVisible(subkey))
                        {
                            result.Add((string)subkey.GetValue("DisplayName"));
                        }
                    }
                }
            }

            return result;
        }

        private bool IsProgramVisible(RegistryKey subkey)
        {
            var name = (string)subkey.GetValue("DisplayName");
            var releaseType = (string)subkey.GetValue("ReleaseType");
            //var unistallString = (string)subkey.GetValue("UninstallString");
            var systemComponent = subkey.GetValue("SystemComponent");
            var parentName = (string)subkey.GetValue("ParentDisplayName");

            return
                !string.IsNullOrEmpty(name)
                && string.IsNullOrEmpty(releaseType)
                && string.IsNullOrEmpty(parentName)
                && (systemComponent == null);
        }

        public string GetAppPath(string productName)
        {
            const string foldersPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders";
            var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            var subKey = baseKey.OpenSubKey(foldersPath);
            if (subKey == null)
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                subKey = baseKey.OpenSubKey(foldersPath);
            }
            return subKey != null ? subKey.GetValueNames().FirstOrDefault(kv => kv.Contains(productName)) : "ERROR";
        }
    }
}

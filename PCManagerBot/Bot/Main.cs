using System;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Management;
using PCManagerBot.Bot.Extra;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace PCManagerBot.Bot
{
    public enum BotState { Wait, KillProc, StartProc, SendMsg, SearchWeb }

    class Main
    {
        private int id;
        private string token;
        private string baseUrl = "https://api.telegram.org/bot";
        private long lastUpdateId = 0;
        private WebClient wc;
        private BotState botState = BotState.Wait;
        private Helper helper;
        private const string infoText =
@"Welcome, to PC Manager Bot. You can find all the useful commnds below
/start -> to start working with bot    
/help -> to get a help
/log -> returns last 10 logs
/screenshot -> takes a screenshot from your PC
/process -> gets a list of currently running processes
/process_start -> starts a brand new process
/process_kill -> kills one of the currently running processes
/apps -> gets a list of all apps installed on your machine
/sysinfo -> gets a system information of your machine
/sendmsg -> this command is just for fun
/search_web -> search for something in the web!
/shutdown -> shutdowns the machine";

        public bool HasData { get; set; } = false;

        public Main()
        {
            Init();
        }

        private void Init()
        {
            InsertInitialData();

            wc = new WebClient();
            helper = new Helper();
            Logs.WriteLog("Authorization...");
        }

        private void InsertInitialData()
        {
            string[] lines = new string[2];
            if (File.Exists("userinfo.txt"))
            {
                using (StreamReader sr = new StreamReader("userinfo.txt"))
                {
                    int i = 0;
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines[i] = line;
                        i++;
                    }
                }
                id = Int32.Parse(lines[0]);
                token = lines[1];
                HasData = true;
            }
        }

        public void Update()
        {
            string s = wc.DownloadString(baseUrl + token + "/getUpdates?offset=" + (lastUpdateId + 1));
            TelegramMessage tgMessages = JsonConvert.DeserializeObject<TelegramMessage>(s);

            HandleResults(tgMessages);
        }

        private void HandleResults(TelegramMessage tgMessages)
        {
            if (!tgMessages.ok || tgMessages.result.Length == 0) return;

            foreach (Result result in tgMessages.result)
            {
                lastUpdateId = result.update_id;
                Logs.WriteLog(result.message.from.first_name + ": " + result.message.text);

                if (result.message.chat.id != id)
                {
                    SendMessage(result.message.chat.id, $"You wrote me {result.message.text}, but I do not know what to do");
                    return;
                }

                switch (botState)
                {
                    case BotState.KillProc:
                        if (ProcessIsClosed(result.message.text))
                        {
                            SendMessage(result.message.chat.id, "Process terminated");
                            Logs.WriteLog("Process terminated: " + result.message.text);
                        }
                        else
                        {
                            SendMessage(result.message.chat.id, "No such process");
                        }
                        break;
                    case BotState.StartProc:
                        if (ProcessIsStarted(result.message.text))
                        {
                            SendMessage(result.message.chat.id, "App started");
                            Logs.WriteLog("Launching app: " + result.message.text);
                        }
                        else
                        {
                            SendMessage(result.message.chat.id, "No such app");
                        }
                        break;
                    case BotState.SendMsg:
                        SendMsg(result.message.text);
                        break;
                    case BotState.SearchWeb:
                        SearchWeb(result.message.text);
                        break;
                    /*default:
                        SendMessage(result.message.chat.id, result.message.text);
                        break;*/
                }

                SendAnswer(result.message.chat.id, result.message.text);
            }
        }

        private void SendMessage(long chatId, string text)
        {
            string address = baseUrl + token + "/sendMessage";
            NameValueCollection collection = new NameValueCollection();
            collection.Add("chat_id", chatId.ToString());
            collection.Add("text", text);
            wc.UploadValues(address, collection);
        }

        private void SendAnswer(long chatId, string text)
        {
            string answer = "";

            switch (text.ToLower())
            {
                case "/start": answer = "Hello, I am a PC Manager Bot. Do you know what I can do? /help"; break;
                case "/help": answer = infoText; break;
                case "/log": answer = Logs.ReturnLastTenLogs(); break;
                case "/screenshot": SendPrintScreen(chatId); return;
                case "/process": answer = GetMyProcess(); break;
                case "/process_start": answer = "Which one?"; botState = BotState.StartProc; break;
                case "/process_kill": answer = GetMyProcess() + "\r\nWhich one?"; botState = BotState.KillProc; break;
                case "/apps": answer = helper.GetInstalledPrograms(); break;
                case "/sysinfo": answer = GetSystemInfo(); break;
                case "/search_web": answer = "Google it! Type something..."; botState = BotState.SearchWeb; break;
                case "/sendmsg": answer = "Write something..."; botState = BotState.SendMsg; break;
                case "/shutdown": ShutDown(); break;
                default: throw new ArgumentException("Unknown command");
            }

            SendMessage(chatId, answer);
        }

        private void ShutDown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                     mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                               mboShutdownParams, null);
            }
        }

        private void SendMsg(string text)
        {
            botState = BotState.Wait;
            MessageBox.Show(text, "");
        }

        private void SendPrintScreen(long chatId)
        {
            string address = baseUrl + token + "/sendPhoto";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chatId.ToString());
            helper.HttpUploadScreen(address, "MyScreen.jpg", "photo", "image/jpg", nvc);
        }

        private string GetMyProcess()
        {
            Process[] processes = Process.GetProcesses();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Processes' list");
            sb.AppendLine(new String('=', 25));
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                    sb.AppendLine(p.StartTime + ": " + p.ProcessName + " - " + p.MainWindowTitle);
            }
            sb.AppendLine(new String('=', 25));
            return sb.ToString();
        }

        private bool ProcessIsClosed(string procName)
        {
            botState = BotState.Wait;
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName == procName)
                {
                    Process.GetProcessesByName(procName)[0].Kill();
                    return true;
                }
            }

            return false;
        }

        private bool ProcessIsStarted(string appName)
        {
            string exeFolder = helper.GetApplictionInstallPath(appName);
            botState = BotState.Wait;

            if (!String.IsNullOrEmpty(exeFolder))
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(exeFolder);
                    FileInfo[] files = d.GetFiles("*.exe");
                    string exeFile = "", exePath = "";
                    long fileSize = files[0].Length;
                    for (int i = 1; i < files.Length; i++)
                    {
                        if (fileSize < files[i].Length)
                            fileSize = files[i].Length;
                    }
                    foreach (FileInfo file in files)
                    {
                        if (file.Length == fileSize)
                            exeFile = file.Name;
                    }
                    exePath = Path.Combine(exeFolder, exeFile);
                    if (File.Exists(exePath))
                    {
                        Process.Start(exePath);
                        return true;
                    }
                }
                catch
                {
                    MessageBox.Show("Cannot launch an app", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return false;
        }

        private string GetSystemInfo()
        {
            StringBuilder sb = new StringBuilder();
            //OS Info
            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            sb.AppendLine("=====OS Info=====");
            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                sb.AppendLine("Caption: " + obj["Caption"]);
                sb.AppendLine("Version: " + obj["Version"]);
            }

            //CPU Info
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
            sb.AppendLine("=====CPU Info=====");
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                sb.AppendLine("Name: " + obj["Name"]);
                sb.AppendLine("Number of cores: " + obj["NumberOfCores"]);
                sb.AppendLine("Caption: " + obj["Caption"]);
            }

            //RAM Info
            ManagementObjectSearcher myRamObject = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            sb.AppendLine("====RAM Info=====");
            foreach (ManagementObject obj in myRamObject.Get())
            {
                sb.AppendLine("Total visible memory: " + helper.SizeSuffix((long)Convert.ToDouble(obj["TotalVisibleMemorySize"]) * 1024));
            }

            //Drives Info
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            sb.AppendLine("====Drive Info=====");
            foreach (DriveInfo d in allDrives)
            {
                sb.AppendLine("Drive " + d.Name);
                if (d.IsReady == true)
                {
                    sb.AppendLine("Total available space: " + helper.SizeSuffix(d.TotalFreeSpace));
                    sb.AppendLine("Total size of drive: " + helper.SizeSuffix(d.TotalSize));
                }
            }

            //GPU Info
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            sb.AppendLine("====GPU Info=====");
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                sb.AppendLine("Name: " + obj["Name"]);
                sb.AppendLine("Caption: " + obj["Caption"]);
                sb.AppendLine("AdapterRAM: " + helper.SizeSuffix((long)Convert.ToDouble(obj["AdapterRAM"])));
            }

            return sb.ToString();
        }

        private string SearchWeb(string query)
        {
            botState = BotState.Wait;
            Process.Start($"microsoft-edge:https://www.google.com/search?q={query}");
            return "started";
        }
    }
}

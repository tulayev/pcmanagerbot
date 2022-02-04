using PCManagerBot.Bot;
using PCManagerBot.Bot.Extra;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCManagerBot
{
    public partial class MainForm : Form
    {
        private Main main;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            main = new Main();
            timer.Enabled = true;

            if (!File.Exists("userinfo.txt"))
                panel.Visible = true;
        }  

        private void timer_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => main.Update());
        }

        private void btnGetlogs_Click(object sender, EventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(Logs.GetLog, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = String.Empty;
                    while ((line = sr.ReadLine()) != null)
                        tBText.Text += line + "\r\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e) =>
            this.Close();

        private void okBtn_Click(object sender, EventArgs e)
        {
            string tgId = tBTgId.Text.Trim();
            string tgToken = tBTgToken.Text.Trim();

            if (!String.IsNullOrEmpty(tBTgId.Text) && !String.IsNullOrEmpty(tBTgToken.Text))
            {
                using (StreamWriter sw = new StreamWriter("userinfo.txt", false))
                {
                    sw.WriteLine(tgId);
                    sw.WriteLine(tgToken);
                }
                main = new Main();
                panel.Visible = false;
            }
            else
            {
                tGIdLabel.Text = tGTokenLabel.Text = "This field must be filled!";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel.Visible = true;
        }

        /* Set app to autorun
        private void SetAutoRun()
        {
            string exePath = System.Windows.Forms.Application.ExecutablePath;
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            reg.SetValue("TelegramBot", exePath);
            //reg.DeleteValue("TelegramBot"); 
        }*/
    }
}

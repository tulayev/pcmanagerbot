using System;
using System.IO;

namespace PCManagerBot.Bot.Extra
{
    static class Logs
    {
        private const string botLog = "BotLog.log";

        // get botlog file path
        public static string GetLog
        {
            get { return botLog; }
        }

        // log writer method
        public static void WriteLog(string text)
        {
            text = DateTime.Now + " " + text + Environment.NewLine;
            File.AppendAllText(botLog, text);
        }

        public static string ReturnLastTenLogs()
        {
            string answer = "";
            int lastTenLog;
            if (File.Exists(botLog))
            {
                string[] logs = File.ReadAllLines(botLog);
                lastTenLog = logs.Length > 10 ? logs.Length - 10 : 0;
                for (int i = lastTenLog; i < logs.Length; i++)
                {
                    answer += logs[i] + Environment.NewLine;
                }
            }
            return answer;
        }

    }
}

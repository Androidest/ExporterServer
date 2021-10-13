using System;
using System.Collections.Generic;

namespace Utils
{
    public class Logger : Singleton<Logger>
    {
        List<string> Logs = new List<string>();
        int LogPointer = 0;

        public void AddLog(string msg)
        {
            lock (Logs)
            {
                Console.WriteLine(msg);
                Logs.Add(msg);
            }
        }

        public string GetLogs()
        {
            string unread = "";
            lock (Logs)
            {
                for (int i = LogPointer; i < Logs.Count; ++i)
                {
                    unread += Logs[i] + "\n";
                }
                if (Logs.Count > 0)
                {
                    LogPointer = Logs.Count;
                }
            }
            return unread;
        }
    }
}


using System;
using System.Collections.Generic;

namespace Utils
{
    public class Logger : Singleton<Logger>
    {
        string[] Logs = new string[10000];
        int curPointer = 0;
        int LogPointer = 0;

        private int ToIndex(int pointer)
        {
            return pointer % Logs.Length;
        }

        public void AddLog(string msg)
        {
            lock (Logs)
            {
                Console.WriteLine(msg);
                Logs[ToIndex(curPointer++)] = msg;
            }
        }

        public string GetLogs()
        {
            string unread = "";
            lock (Logs)
            {
                for (int i = LogPointer; i < curPointer; ++i)
                {
                    unread += Logs[ToIndex(i)] + "\n";
                }
                LogPointer = curPointer;
            }
            return unread;
        }
    }
}


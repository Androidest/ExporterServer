using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExporterShared.Models
{
    public class Command
    {
        public delegate void ProgressCallback(int execTime, string output);
        public ProgressCallback OnProgress = null;
        private ProcessStartInfo CommandInfo = null;

        public string App
        {
            get => CommandInfo.FileName;
            set
            {
                CommandInfo.FileName = value;
            }
        }
        public string Args
        {
            get => CommandInfo.Arguments;
            set
            {
                CommandInfo.Arguments = value;
            }
        }

        public Command()
        {
            CommandInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        public Command(string app, string args):this()
        {
            App = app;
            Args = args;
        }

        public async Task Execute()
        {
            System.Timers.Timer Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            int execTime = 0;

            try
            {
                Process proc = Process.Start(CommandInfo);
                Timer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    string output = "";
                    OnProgress?.Invoke(++execTime, output);
                });

                Timer.Start();
                await Task.Run(() => proc.WaitForExit());
                Timer.Close();
                if (0 != proc.ExitCode)
                {
                    throw new Exception(string.Format("命令返回值：{0}", proc.ExitCode));
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}

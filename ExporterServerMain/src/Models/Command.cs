using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExporterShared.Models
{
    public class Command
    {
        public delegate void ProgressCallback(int execTime);
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
            int execTime = 0;
            System.Timers.Timer Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
            {
                OnProgress?.Invoke(++execTime);
            });
            
            Timer.Start();
            try
            {
                Process proc = Process.Start(CommandInfo);
                await Task.Run(() => proc.WaitForExit());
            }
            catch(Exception e)
            {
                throw e;
            }
            Timer.Close();
        }
    }
}

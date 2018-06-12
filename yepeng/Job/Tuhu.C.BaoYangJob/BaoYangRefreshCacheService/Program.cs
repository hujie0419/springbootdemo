extern alias old;
using System;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;
using Topshelf;
using Tuhu.ZooKeeper;

namespace BaoYangRefreshCacheService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            RefreshService service = new RefreshService();
            HostFactory.Run(x =>
            {
                x.Service<RefreshService>(s =>
                {
                    s.ConstructUsing(name => service);
                    s.WhenStarted(tc => tc.StartIt());
                    s.WhenStopped(tc => tc.StopIt());
                    s.WhenPaused(tc => tc.PauseIt());
                    s.WhenContinued(tc => tc.ContinueIt());
                });
                x.RunAsLocalSystem();
                x.EnablePauseAndContinue();             
            });        
        }
    }
}

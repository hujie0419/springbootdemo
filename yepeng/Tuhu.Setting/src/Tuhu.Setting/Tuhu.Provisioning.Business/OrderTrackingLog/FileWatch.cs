using System;
using System.IO;

namespace Tuhu.Provisioning.Business.OrderTrackingLog
{
    public static class FileWatch
    {
        public static Action OnConfigChanged { get; set; }
        static FileWatch()
        {
            var fileWatch = new FileSystemWatcher(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "XML"))
            {
                Filter = "OrderTrackingLogMapping.xml",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
            };
            fileWatch.Changed += new FileSystemEventHandler(fsw_Changed);
            fileWatch.EnableRaisingEvents = true;
        }

        private static void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            OnConfigChanged();
        }
    }
}
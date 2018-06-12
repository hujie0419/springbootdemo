using Common.Logging;
using CrystalQuartz.Application;
using CrystalQuartz.Owin;
using Microsoft.Owin.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Topshelf;
using Tuhu.ZooKeeper;

namespace Tuhu.C.SyncProductPriceJob
{
    public class QuartzService : MasterSlaveService, ServiceControl, ServiceSuspend
    {
        private IDisposable _webApp;

        protected override ILog Logger { get; }

        public QuartzService()
        {
            Logger = LogManager.GetLogger<QuartzService>();
        }

        //从工厂中获取一个调度器实例化
        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public override Task OnStartService()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            var uri = ConfigurationManager.AppSettings["CrystalQuartzUrl"] ?? "http://*:5555";
            var index = uri.IndexOf('/', 10);

            _webApp = WebApp.Start(index > 0 ? uri.Substring(0, index) : uri, app =>
            {
                app.UseCrystalQuartz(_scheduler, new CrystalQuartzOptions
                {
                    Path = index > 0 ? uri.Substring(index) : ""
                });
            });

            if (_webApp != null)
            {
                Logger.Info($"WebApp 启动成功：{uri}");
            }

            _scheduler.Start();

            return Task.CompletedTask;
        }

        public override Task OnStopService()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }
            _scheduler.Shutdown(false);
            return Task.CompletedTask;
        }

        protected override bool IsServiceStoped() => _scheduler == null || !_scheduler.IsStarted && !_scheduler.IsShutdown;

        protected override string GetMessage(string status)
        {
            var clientInfo = Client.ClientInfo;

            return clientInfo.Ip + "的同步产品价格服务" + status;
        }

        public bool Start(HostControl hostControl)
        {
            OnStart(null);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            OnStop();
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            OnPause();
            _scheduler.PauseAll();
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            OnContinue();
            _scheduler.ResumeAll();
            return true;
        }

#if DEBUG

        public void StartIt()
        {
            OnStart(null);
        }

        public void StopIt()
        {
            OnStop();
        }

#endif
    }
}

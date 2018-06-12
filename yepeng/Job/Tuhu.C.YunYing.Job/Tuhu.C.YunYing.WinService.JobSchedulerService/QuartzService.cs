using System;
using System.Configuration;
using Common.Logging;
using CrystalQuartz.Application;
using CrystalQuartz.Owin;
using Microsoft.Owin.Hosting;
using Quartz;
using Quartz.Impl;
using Tuhu.ZooKeeper;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService
{
    public partial class QuartzService : MasterSlaveService
    {
        public override ILog Logger { get; }
        public override bool StandAlone => ConfigurationManager.AppSettings["StandAlone"] == "true";

        private IScheduler _scheduler;
        private IDisposable _webApp;

        public QuartzService()
        {
            InitializeComponent();

            Logger = LogManager.GetLogger(this.GetType());
        }


        public override void StartService()
        {
            StopService();
            //var env = ConfigurationManager.AppSettings["Env"];

            if (ConfigurationManager.AppSettings["RunJob"] == "true")
            {
                _scheduler = new StdSchedulerFactory().GetScheduler();

                var uri = ConfigurationManager.AppSettings["CrystalQuartzUrl"] ?? "http://*:5555";
                var index = uri.IndexOf('/', 10);

                _webApp = WebApp.Start(index > 0 ? uri.Substring(0, index) : uri, app =>
                {
                    app.UseCrystalQuartz(_scheduler, new CrystalQuartzOptions
                    {
                        Path = index > 0 ? uri.Substring(index) : ""
                    });
                });

                _scheduler.Start();
            }
        }

        public override void StopService()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            if (_scheduler != null && _scheduler.IsStarted && !_scheduler.IsShutdown)
                _scheduler.Shutdown(true);
        }

        protected override string GetMessage(string status)
        {
            var clientInfo = Client.ClientInfo;

            return clientInfo.Ip + "的运营作业调度服务" + status;
        }

        protected override void OnPause()
        {
            _scheduler.PauseAll();
        }

        protected override void OnContinue()
        {
            _scheduler.ResumeAll();
        }

#if DEBUG
        public void StartIt()
        {
            this.OnStart(null);
        }
        public void StopIt()
        {
            this.OnStop();
        }
        public void PauseIt()
        {
            this.OnPause();
        }
        public void ContinueIt()
        {
            this.OnContinue();
        }
#endif
    }
}

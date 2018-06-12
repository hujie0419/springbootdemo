using Common.Logging;
using CrystalQuartz.Application;
using CrystalQuartz.Owin;
using Microsoft.Owin.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Tuhu.ZooKeeper;

namespace Tuhu.C.Job
{
    public partial class QuartzService : MasterSlaveService
    {
        protected override ILog Logger { get; }
        public override bool StandAlone => ConfigurationManager.AppSettings["StandAlone"] == "true";

        private IScheduler _scheduler;
        private IDisposable _webApp;

        public QuartzService()
        {
            InitializeComponent();

            Logger = LogManager.GetLogger(this.GetType());
        }

        public override async Task OnStartService()
        {
           await StopService();
            if (ConfigurationManager.AppSettings["RunJob"] == "true")
                // 线下的job不跑，如果要跑，把developonly中RunJob设置成true，但不要提交
            {
                _scheduler = new StdSchedulerFactory().GetScheduler();

                var uri = ConfigurationManager.AppSettings["CrystalQuartzUrl"] ?? "http://*:5559";
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

        public override Task OnStopService()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            if (_scheduler != null && _scheduler.IsStarted && !_scheduler.IsShutdown)
                _scheduler.Shutdown(false);

            return Task.CompletedTask;
        }

        protected override string GetMessage(string status)
        {
            var clientInfo = Client.ClientInfo;

            return clientInfo.Ip + "的作业调度服务" + status;
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

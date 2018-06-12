using Common.Logging;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using Quartz.Impl.Matchers;
using Tuhu.ZooKeeper;
using System;
using Owin;
using CrystalQuartz.Owin;
using Microsoft.Owin.Hosting;

namespace BaoYangRefreshCacheService
{
    public partial class RefreshService : MasterSlaveService
    {
        protected override ILog Logger => LogManager.GetLogger(this.GetType());

        private IScheduler _scheduler;
        private IDisposable _webApp;

        public RefreshService()
        {
            InitializeComponent();
        }

        public override void OnStartService()
        {
            StopService();

            _scheduler = new StdSchedulerFactory().GetScheduler();
            _scheduler.Start();

            var port = ConfigurationManager.AppSettings["webport"];

            Action<IAppBuilder> startup = app =>
            {
                app.UseCrystalQuartz(_scheduler);
            };

            _webApp = WebApp.Start($"http://*:{port}/", startup);
        }

        public override void OnStopService()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            if (_scheduler != null && _scheduler.IsStarted && !_scheduler.IsShutdown)
                _scheduler.Shutdown(false);
        }

        protected override string GetMessage(string status)
        {
            var clientInfo = Client.ClientInfo;

            return clientInfo.Ip + "的保养Windows Service" + status;
        }

        protected override void OnPause()
        {
            _scheduler.PauseAll();
        }

        protected override void OnContinue()
        {
            _scheduler.ResumeAll();
        }

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.ZooKeeper;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using Owin;
using CrystalQuartz.Owin;
using Microsoft.Owin.Hosting;

namespace Tuhu.CarArchiveService
{
    public class MainService : MasterSlaveService
    {
        public override ILog Logger { get; }
        public override bool StandAlone => ConfigurationManager.AppSettings["StandAlone"] == "true";

        private IScheduler _scheduler;
        private IDisposable _webApp;

        public MainService()
        {
            Logger = LogManager.GetLogger(this.GetType());
        }

        public override void StartService()
        {
            StopService();

            if (!IsTestEnvironment())
            {
                _scheduler = new StdSchedulerFactory().GetScheduler();
                _scheduler.Start();

                var port = ConfigurationManager.AppSettings["webport"];

                Action<IAppBuilder> startup = app =>
                {
                    app.UseCrystalQuartz(_scheduler);
                };

                _webApp = WebApp.Start($"http://*:{port}/", startup);
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
                _scheduler.Shutdown(false);
        }

        protected override string GetMessage(string status)
        {
            var clientInfo = Client.ClientInfo;

            return clientInfo.Ip + "的汽车电子档案服务：" + status;
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

        private bool IsTestEnvironment()
        {
            string environment = ConfigurationManager.AppSettings["environment"].ToString();

#if DEBUG
            return false;
#endif
            return string.Equals(environment, "test", StringComparison.OrdinalIgnoreCase);
        }
    }
}

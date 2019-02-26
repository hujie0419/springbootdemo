using System;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.BLL;


namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class BaoYangAutoAssociationJob : BaseJob
    {
        public BaoYangAutoAssociationJob()
        {
            logger = LogManager.GetLogger(typeof(BaoYangAutoAssociationJob));
        }


        protected override string JobName
        {
            get { return nameof(BaoYangAutoAssociationJob); }
        }

        public override void Exec()
        {
            try
            {

                BaoYangAutoAssociationBLL bll = new BaoYangAutoAssociationBLL(logger);
                var startTime = bll.GetJobLastRunTime();
                var tids = bll.SelectNewVehicleTids(startTime);
                var pids = bll.SelectPids();
                bll.AsociateVehicles(tids, pids);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}

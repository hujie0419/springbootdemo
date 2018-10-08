using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.C.Job.ThirdPart.Model;
using Tuhu.C.Job.ThirdPart.Model.Enum;
using Tuhu.C.Job.ThirdPart.Model.GFCard;

namespace Tuhu.C.Job.ThirdPart.Manager.GFCard
{
    public class GFSmsTaskExecutor
    {
        private static readonly int _smsTemmplateId = 271;
        public IEnumerable<GFBankSmsTask> _task;
        private GFDal _gfDal = GFDal.GetInstance();
        public GFSmsTaskExecutor(IEnumerable<GFBankSmsTask> task)
        {
            this._task = task;
        }
        /// <summary>
        /// 执行发短信任务
        /// </summary>
        /// <param name="sendTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankSmsTask>> ExecuteTasks(DateTime sendTime, DateTime startTime)
        {
            if (_task == null || !_task.Any())
                return new List<GFBankSmsTask>();
            var mobiles = _task.Select(s => s.Mobile).Distinct();

            var sendSmsResult = await Proxy.UtilityServiceProxy.SendSmsAsync(mobiles, _smsTemmplateId, new string[0], null, null, null, sendTime);
            if (sendSmsResult)
            {
                await _gfDal.BatchUpdateGFBankSmsTasksStatus(_task, nameof(GFTaskStatus.Complated), startTime);
                return new List<GFBankSmsTask>();
            }
            else
            {
                await _gfDal.BatchUpdateGFBankSmsTasksStatus(_task, nameof(GFTaskStatus.SmsFailed), startTime);
                return _task;
            }
        }
        /// <summary>
        /// 执行发短信失败的任务
        /// </summary>
        /// <param name="sendTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankSmsTask>> ExecuteSmsFailedTasks(DateTime sendTime, DateTime startTime)
        {
            if (_task == null || !_task.Any())
                return new List<GFBankSmsTask>();
            var mobiles = _task.Select(s => s.Mobile).Distinct();

            var sendSmsResult = await Proxy.UtilityServiceProxy.SendSmsAsync(mobiles, _smsTemmplateId, new string[0], null, null, null, sendTime);
            if (sendSmsResult)
            {
                await _gfDal.BatchUpdateGFBankSmsTasksStatus(_task, nameof(GFTaskStatus.Complated), startTime);
                return new List<GFBankSmsTask>();
            }
            else
            {
                await _gfDal.BatchUpdateGFBankSmsTasksStatus(_task, nameof(GFTaskStatus.RetrySmsFailed), startTime);
                return _task;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;

namespace Tuhu.C.Job.UserAccountCombine.Job
{
    public class CleanDupYewuUserJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CleanDupYewuUserJob));
        private const string runtimeSwitch = "CleanDupYewuUserJob";
        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("CleanDupYewuUser开始执行：" + DateTime.Now.ToString());
            try
            {
                //获取YewuUser表里uid\3rdId\channel都一样的数据
                var stepOne_GetDupYewuUsers = UACManager.GetDupYewuUsers();
                if (null == stepOne_GetDupYewuUsers)
                {
                    _logger.Info("CleanDupYewuUser没有需要合并的记录：" + DateTime.Now.ToString());
                }
                else
                {
                    //针对每组相同UserId+ThirdpartyId+Channel,选择pkid最小的，其余记录下来，然后删除
                    var stepTwo_CollectNeedDeleteList = UACManager.CollectNeedDeleteDupYewuUserList(stepOne_GetDupYewuUsers);

                    if (UACManager.CheckSwitch(runtimeSwitch)) //如果开关开着，跑一条，如果开关关了跑全部
                    {
                        stepTwo_CollectNeedDeleteList = stepTwo_CollectNeedDeleteList.Take(1).ToList();
                    }

                    //删除stepTwo数据，并记录数据
                    var stepThree_DeleteAndRecord = UACManager.DeleteAndRecordDupYewuUsers(stepTwo_CollectNeedDeleteList);
                    if (!stepThree_DeleteAndRecord)
                        _logger.Info("CleanDupYewuUser清理重复三方业务表数据失败");
                }
                _logger.Info("CleanDupYewuUser执行结束");
            }
            catch (Exception ex)
            {
                _logger.Info($"CleanDupYewuUser：运行异常=》{ex}");
            }
            _logger.Info("CleanDupYewuUser执行结束");
        }
    }
}

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
    public class UserCombineAndRecordLogJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(UserCombineAndRecordLogJob));
        private const string runtimeSwitch = "UserCombineAndRecordLogJob";
        public void Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();

            //_logger.Info("UserCombineAndRecordLog开始执行：" + DateTime.Now.ToString());
            //if (UACManager.CheckSwitch(runtimeSwitch))
            //{
            //    try
            //    {                    
            //        //获取UAC_NeedCombineUserId表里IsOperateSuccess=0(即操作失败，或者还没执行)的数据
            //        var stepOne_GetNeedCombineUserIdList = UACManager.GetNeedCombineUserIdList();
            //        if (null == stepOne_GetNeedCombineUserIdList)
            //        {
            //            _logger.Info("UserCombineAndRecordLog没有需要合并的记录：" + DateTime.Now.ToString());
            //        }
            //        else
            //        {
            //            //循环遍历，对于每条记录，
            //            //如果所有表都更新成功，更新IsOperateSuccess=1，记录所有表操作日志
            //            //否者，记录报错，回滚操作，记录失败  
            //            //stepTwo_CombineAndRecordAction计数了需要完成\成功\失败的行数

            //            int onceRunCount = Convert.ToInt32(ConfigurationManager.AppSettings["UserCombineOnceRunCount"]);
            //            if (onceRunCount != 0 && stepOne_GetNeedCombineUserIdList.Count > onceRunCount)
            //            {
            //                stepOne_GetNeedCombineUserIdList = stepOne_GetNeedCombineUserIdList.Take(onceRunCount).ToList();
            //            }

            //            var stepTwo_CombineAndRecordAction = UACManager.CombineAndRecordAction(stepOne_GetNeedCombineUserIdList);
            //            _logger.Info("UserCombineAndRecordLog：" + stepTwo_CombineAndRecordAction);
            //        }                    
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.Info($"UserCombineAndRecordLog：运行异常=》{ex}");
            //    }
            //}
            //_logger.Info("UserCombineAndRecordLog执行结束");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;


namespace Tuhu.C.Job.UserAccountCombine.Job
{
    public class CollectNeedCombineUserIdJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CollectNeedCombineUserIdJob));
        public void Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();

            //try
            //{
            //    _logger.Info("CollectNeedCombineUserId开始执行：" + DateTime.Now.ToString());
            //    //获取YewuThirdpartyInfo表里所有同3rdID+channel的用户
            //    var stepOne_GetNeedCombineUserList = UACManager.GetNeedCombineYewuUsers();
            //    if (null == stepOne_GetNeedCombineUserList)
            //    {
            //        _logger.Info("CollectNeedCombineUserId已经没有需要合并UserId：" + DateTime.Now.ToString());
            //    }
            //    else
            //    {
            //        //针对每组3rdID+channel，选出一个主UserId，其他全是需要更新的UserId
            //        var stepTwo_CombineResultList = UACManager.FilterPrimaryUserIds(stepOne_GetNeedCombineUserList);
            //        //把筛选出的结果集上传到数据库
            //        var stepThree_InsertNeedCombineUserIds = UACManager.InsertNeedCombineUserIds(stepTwo_CombineResultList);
            //        if (!stepThree_InsertNeedCombineUserIds)
            //            _logger.Info("CollectNeedCombineUserId插入结果集失败");
            //    }
            //    _logger.Info("CollectNeedCombineUserId执行结束");
            //}
            //catch(Exception ex)
            //{
            //    _logger.Info($"CollectNeedCombineUserId：运行异常=》{ex}");
            //}
        }
    }
}

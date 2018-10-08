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
    public class CollectNeedCombineInUserTableJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CollectNeedCombineInUserTableJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info("CollectNeedCombineInUserTable开始执行：" + DateTime.Now.ToString());
                
                //获取手机号不为空 and 用户状态Isactive=true 的情况下手机号重复的用户
                var stepOne_GetNeedCombineInUserTable = UACManager.GetNeedCombineInUserTable();
                if(null == stepOne_GetNeedCombineInUserTable)
                {
                    _logger.Info("CollectNeedCombineInUserTable已经没有需要合并UserId：" + DateTime.Now.ToString());
                }
                else
                {
                    _logger.Info("CollectNeedCombineInUserTable已经得到所有元数据,一共有" + stepOne_GetNeedCombineInUserTable.Count() + "条重复数据需要合并," + DateTime.Now.ToString());
                    //针对每个mobile，选出一个主UserId，其他全是需要更新的UserId
                    var stepTwo_CombineResultList = UACManager.FilterPrimaryUserIdsViaPhone(stepOne_GetNeedCombineInUserTable);
                    _logger.Info("CollectNeedCombineInUserTable已经完成筛选主UserId的逻辑" + DateTime.Now.ToString());

                    _logger.Info("CollectNeedCombineInUserTable开始执行需要合并账号插入数据库的操作." + DateTime.Now.ToString());
                    //把筛选出的结果集上传到数据库
                    var stepThree_InsertNeedCombineUserIdViaPhone = UACManager.InsertNeedCombineUserIdsViaPhone(stepTwo_CombineResultList);

                    if (!stepThree_InsertNeedCombineUserIdViaPhone)
                        _logger.Info("CollectNeedCombineInUserTable插入结果集失败");
                }
                _logger.Info("CollectNeedCombineInUserTable执行结束");
            }
            catch(Exception ex)
            {
                _logger.Info($"CollectNeedCombineInUserTable：运行异常=》{ex}");
            }
        }
    }
}

using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.UserShareJob.Dal;

namespace Tuhu.C.Job.UserShareJob.Job
{
    /// <summary>
    /// 废弃
    /// </summary>
    [DisallowConcurrentExecution]
    public class SendCodeForUserGroupJob : IJob
    {
        private static ILog SendCodeForUserGroupLogger = LogManager.GetLogger<SendCodeForUserGroupJob>();
        public void Execute(IJobExecutionContext context)
        {
            SendCodeForUserGroupLogger.Info($"推荐有礼特殊人群配置同步开始：每两小时同步一次数据");
            //从Tuhu_bi库同步到Configuration 
            var needData = DalShareUser.GetNeedExportData().ToList();
            if (!needData.Any())
            {
                SendCodeForUserGroupLogger.Error($"推荐有礼特殊人群配置同步:没有需要同步的数据");
                return;
            }
            foreach (var groupData in needData.GroupBy(n => n.GroupId))
            {
                var groupIsExist = DalShareUser.GetGroupIdIsExist(groupData.Key);
                var tempData = groupData.ToList();
                if (!groupIsExist)
                {
                    if (tempData.Count > 1000)
                    {
                        int index = 0, pageSize = 500;
                        var temptempData = tempData.Skip(index * pageSize).Take(pageSize).ToList();
                        while (temptempData != null & temptempData.Count > 0)
                        {
                            if (!DalShareUser.CreateSendCodeForUserGroupData(temptempData))
                                SendCodeForUserGroupLogger.Error($"推荐有礼特殊人群配置同步失败:GroupId={groupData.Key}");
                            SendCodeForUserGroupLogger.Info($"推荐有礼特殊人群配置同步成功:GroupId={groupData.Key}");
                            index++;
                            temptempData = tempData.Skip(index * pageSize).Take(pageSize).ToList();
                        }
                    }
                    else
                    {
                        if (!DalShareUser.CreateSendCodeForUserGroupData(tempData))
                            SendCodeForUserGroupLogger.Error($"推荐有礼特殊人群配置同步失败:GroupId={groupData.Key}");
                        SendCodeForUserGroupLogger.Info($"推荐有礼特殊人群配置同步成功:GroupId={groupData.Key}");
                    }
                }
                else
                {
                    SendCodeForUserGroupLogger.Info($"推荐有礼特殊人群配置同步异常:群体GroupId已经存在：GroupId={groupData.Key}");
                }
            }
            SendCodeForUserGroupLogger.Info($"推荐有礼特殊人群配置同步结束");
        }
    }
}

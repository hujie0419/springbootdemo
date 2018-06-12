using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.UserShareJob.Dal;
using System.Threading;
using Tuhu.C.Job.UserShareJob.Model;

namespace Tuhu.C.Job.UserShareJob.Job
{
    [DisallowConcurrentExecution]
    class UserShareRankingJob : IJob
    {
        private static ILog UserShareRankingLogger = LogManager.GetLogger<UserShareRankingJob>();
        public void Execute(IJobExecutionContext context)
        {
            UserShareRankingLogger.Info($"分享排行榜：每天晚上23:30更新{DateTime.Now}");
            //获取参入推荐有礼用户
            int pageIndex = 1, pageSize = 100;
            while (true)
            {
                try
                {
                    var allUsers = DalShareUser.GetAllShareUsers(pageIndex, pageSize).ToList();
                    if (allUsers == null || !allUsers.Any())
                    {
                        UserShareRankingLogger.Info($"没有获取到用户数据");
                        break;
                    }
                    var shareUserData = new List<UserShareAwardmodel>();
                    shareUserData.AddRange(DalShareUser.GetShareUserDatas_Old(allUsers));
                    shareUserData.AddRange(DalShareUser.GetShareUserDatas_New(allUsers));
                    if (shareUserData == null || !shareUserData.Any())
                    { pageIndex++; continue; }

                    List<UserShareRankingModel> InsertOrUpdateList = new List<Model.UserShareRankingModel>();

                    #region 奖励金额计算
                    var temp = shareUserData.ToArray();

                    #endregion

                    temp.GroupBy(x => x.UserId).ForEach(f =>
                    {
                        var InsertOrUpdateModel = InsertOrUpdateList.Where(w => w.UserId == f.Key).FirstOrDefault();
                        if (InsertOrUpdateModel == null)
                        {
                            InsertOrUpdateModel = new UserShareRankingModel() { UserId = f.Key, TotalReward = f.Sum(e => (e.IsFromOldTalbe ? (e.Award == 0 ? 20 : e.Award / 100) : e.Award)) };
                            InsertOrUpdateList.Add(InsertOrUpdateModel);
                        }

                    });

                    #region 获取用户昵称头像信息
                    DalShareUser.GetUserNickNameAndHeadImg(InsertOrUpdateList);
                    #endregion

                    if (!DalShareUser.CreateUserShareRankingData(InsertOrUpdateList))
                        UserShareRankingLogger.Error($"分享排行榜：第{pageIndex}页数据更新失败,每页{pageSize}条");
                    UserShareRankingLogger.Info($"分享排行榜：第{pageIndex}页数据更新成功,每页{pageSize}条");
                }
                catch (Exception ex)
                {
                    UserShareRankingLogger.Error(ex);
                    break;
                }
                pageIndex++;

            }
            UserShareRankingLogger.Info($"分享排行榜：更新完成{DateTime.Now}");
        }
    }
}

using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount;
using Tuhu.C.Job.UserAuthJob.DAL;
using Tuhu.C.Job.UserAuthJob.Model;

namespace Tuhu.C.Job.UserAuthJob
{
    [DisallowConcurrentExecution]
    public class UserProfileInitialization : IJob
    {
        static readonly ILog Logger = LogManager.GetLogger<UserProfileInitialization>();
        const int pageSize = 800;
        static int JobBatch = 1;
        private static readonly string OrderQTYKey = "CreatedOrderQTY";

        public void Execute(IJobExecutionContext context)
        {
            JobBatch++;
            Logger.Info($"{JobBatch}=>开始执行");
            try
            {
                 AsyncHelper.RunSync(RefreshUserWithDate);
            }
            catch (Exception ex)
            {
                Logger.Error($"{JobBatch}=>执行异常=>{ex.ToString()}");
            }
            Logger.Info($"{JobBatch}=>执行完毕");
        }

        private async Task RefreshUserWithDate()
        {
            const int pageSize = 1000;
            var nowDate = DateTime.Now;
            var beginDate = nowDate.AddDays(-5);
            decimal num = await UserProfileInitializationDal.SelectUnInitUserWithDateCount(beginDate, nowDate);
            var pageNum = Convert.ToInt32(Math.Ceiling(num / pageSize));
            Logger.Info($"{JobBatch}=>刷新用户属性job开始.一共{pageNum}批次.一共{num}个用户.");

            var errorList = new List<UserProfileWithValue>();
            var client = new UserProfileClient();
            for (var i = 1; i < pageNum + 1; i++)
            {
                Thread.Sleep(500);
                Logger.Info($"{JobBatch}=>第{i}批次开始刷新");
                var resultList = await UserProfileInitializationDal.SelectUserProfileWithDateQTY(pageSize, errorList.Count + pageSize * (i - 1), beginDate, nowDate);
                Logger.Info($"{JobBatch}=>第{i}批次开始刷新=>{resultList.Count()}个用户");

                var batchErrorList = new List<UserProfileWithValue>();
                resultList.ForEach(p =>
                {
                    Thread.Sleep(10);
                    var resultOne = client.Set(p.UserId, OrderQTYKey, p.Value);
                    if (!resultOne.Success)
                    {
                        Thread.Sleep(500);
                        client = new UserProfileClient();
                        resultOne = client.Set(p.UserId, OrderQTYKey, p.Value);
                    }
                    if (!resultOne.Success || !resultOne.Result)
                    {
                        batchErrorList.Add(new UserProfileWithValue() { UserId = p.UserId, Value = p.Value });
                        if (!resultOne.Success)
                            client = new UserProfileClient();
                    }
                });
                Logger.Info($"{JobBatch}=>第{i}批次刷新完毕=>{batchErrorList.Count}个失败");
                errorList.AddRange(batchErrorList);

            }
            Logger.Info($"{JobBatch}=>全部刷新完毕=>用户总量{num};{errorList.Count}个失败");
        }

        private async Task RefreshUserprofileQTY()
        {
            const int pageSize = 1000;
            decimal num = await UserProfileInitializationDal.SelectUnInitUserCount(OrderQTYKey);
            var pageNum = Convert.ToInt32(Math.Ceiling(num / pageSize));

            Logger.Info($"{JobBatch}=>刷新用户属性job开始.一共{pageNum}批次.一共{num}个用户.");

            var errorList = new List<UserProfileWithValue>();
            var client = new UserProfileClient();
            for (var i = 1; i < pageNum + 1; i++)
            {
                Thread.Sleep(500);
                Logger.Info($"{JobBatch}=>第{i}批次开始刷新");
                var resultList = await UserProfileInitializationDal.SelectUserProfileWithPageOrderQTY(pageSize, errorList.Count);
                Logger.Info($"{JobBatch}=>第{i}批次开始刷新=>{resultList.Count()}个用户");

                var batchErrorList = new List<UserProfileWithValue>();
                resultList.ForEach(p =>
                {
                    Thread.Sleep(10);
                    var resultOne = client.Set(p.UserId, OrderQTYKey, p.Value);
                    if (!resultOne.Success)
                    {
                        Thread.Sleep(500);
                        client = new UserProfileClient();
                        resultOne = client.Set(p.UserId, OrderQTYKey, p.Value);
                    }
                    if (!resultOne.Success || !resultOne.Result)
                    {
                        batchErrorList.Add(new UserProfileWithValue() { UserId = p.UserId, Value = p.Value });
                        if (!resultOne.Success)
                            client = new UserProfileClient();
                    }
                });
                Logger.Info($"{JobBatch}=>第{i}批次刷新完毕=>{batchErrorList.Count}个失败");
                errorList.AddRange(batchErrorList);

            }
            Logger.Info($"{JobBatch}=>全部刷新完毕=>用户总量{num};{errorList.Count}个失败");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Config;
using Tuhu.Service.Member;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using CacheClient = Tuhu.Nosql.CacheClient;

namespace Tuhu.C.Job.Job
{
    /// <summary>
    ///     活动-用户-答题-清算JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public class ActivityUserQuestionAnswerClearJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ActivityUserQuestionAnswerClearJob>();

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("活动-用户-答题-清算JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"活动-用户-答题-清算JOB 结束执行,用时{stopwatch.Elapsed.Seconds}秒");
        }


        #region

        private void Run()
        {
            using (var acitivtyClient = new ActivityClient())
            {
                #region 世界杯活动清算

                //获取世界杯活动
                var activity = acitivtyClient.GetWorldCup2018Activity();

                if (activity.Success && activity.Result != null)
                {
                    //获取今天发布'结果'的题目  列表
                    var questions = DalActivity.SearchTodayReleaseQuestionAnswerList(activity.Result.QuestionnaireID)?.ToList();

                    if (questions != null)
                    {
                        if (questions.Count > 0)
                        {
                            //刷新缓存
                            RefreshCache(activity?.Result?.PKID ?? 0);
                        }

                        //循环问题
                        questions.ForEach(question =>
                        {
                            UserAnswerClear(question, activity.Result.PKID);
                        });
                    }

                }

                #endregion
            }
        }

        /// <summary>
        ///     清算用户数据
        /// </summary>
        /// <param name="questionModel"></param>
        /// <param name="activityId"></param>
        private void UserAnswerClear(QuestionModel questionModel, long activityId)
        {
            var questionId = questionModel.PKID;
            //获取题目的选项和子项
            var options = DalActivity.SearchQuestionOption(questionId)?.ToList();
            if (options?.Count == 0)
                return;
            //正确选项
            var rightOption = options.FirstOrDefault(p => p.QuestionParentID == 0 && p.IsRightValue == 1);
            if (rightOption == null)
                return;
            var i = 1;
            using (var db = DbHelper.CreateDbHelper())
            using (var configClient = new ConfigClient())
            using (var userClient = new UserClient())
            using (var activityClient = new ActivityClient())
            using (var templatePushClient = new TemplatePushClient())
            using (var cacheClient = CacheHelper.CreateCacheClient("ActivityUserQuestionAnswerClear"))
            {
                //推送的开关设置
                var pushResult = configClient.GetOrSetRuntimeSwitch("activityquestionclearpush")?.Result;
                //推送开关
                var pushFlag = pushResult?.Value ?? false;
                //推送的模板ID
                var pushIdstr = pushResult?.Description;
                var pushId = 0;
                int.TryParse(pushIdstr, out pushId);
                while (true)
                {
                    var ts = new Stopwatch();
                    ts.Start();
                    var clearCount = 1000;
                    //获取用户答题尚未清算数据
                    var userAnswerNotClearDatas = DalActivity.SearchUserAnswerNotClear(questionId, clearCount)?.ToList();
                    if (userAnswerNotClearDatas?.Count == 0)
                        break;

                    //循环用户数据
                    userAnswerNotClearDatas.AsParallel().ForAll(userData =>
                    {
                        try
                        {
                            //用户选择的选项
                            var userOption = options.FirstOrDefault(p => p.PKID == userData.AnswerOptionID);
                            if (userOption != null)
                            {

                                Logger.Info($"开始清算用户数据：{userData.UserID} {userData.AnswerOptionID}");

                                //找用户选项的根选项
                                var rootOption = GetRootOption(userOption, options);
                                //判断是否正确选项
                                //正确则赠送兑换券 否则不赠送兑换券
                                if (rootOption.PKID == rightOption.PKID)
                                {
                                    //更新回答结果
                                    var resultAnswer = activityClient.ModifyQuestionUserAnswerResult(
                                        new ModifyQuestionUserAnswerResultRequest()
                                        {
                                            AnswerResultStatus = 1,
                                            ResultId = userData.PKID,
                                            WinCouponCount = userData.WinCouponCount
                                        });

                                    if (resultAnswer?.Result?.IsOk==true)
                                    {
                                        //更新兑换券 和 日志
                                        var result = activityClient.ModifyActivityCoupon(userData.UserID,
                                            activityId,
                                            userOption.WinCouponCount ?? 0
                                            , "答题清算服务"
                                            , userData.AnswerDate);
                                        if (!result.Success && result.Result <= 0)
                                        {
                                            Logger.Warn($"请求 ModifyActivityCoupon 失败 {result.ErrorMessage}");
                                            //回滚
                                            DalActivity.UpdateUserAnswerResult(db, userData.PKID,
                                                userData.WinCouponCount, 0);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    //更新用户答题结果
                                    var resultAnswer = activityClient.ModifyQuestionUserAnswerResult(
                                        new ModifyQuestionUserAnswerResultRequest()
                                        {
                                            AnswerResultStatus = 2,
                                            ResultId = userData.PKID,
                                            WinCouponCount = 0
                                        });
                                }
                                Logger.Info($"用户 {userData.UserID} ");
                                //推送消息 判断推送开关是否开启
                                if (pushFlag && pushId != 0)
                                {
                                    PushMessage(pushId, cacheClient, userClient, templatePushClient, userData.UserID,userData.CreateDatetime);

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error($" {nameof(ActivityUserQuestionAnswerClearJob)} 失败 ", e);

                        }
                    });
                    ts.Stop();
                    Logger.Info($" {nameof(UserAnswerClear)} 清算 {clearCount} 条数据 执行时间 {ts.Elapsed.TotalSeconds} 秒 ");
                    i++;
                }
            }
        }


        /// <summary>
        ///     找根选项 （递归）
        /// </summary>
        /// <param name="questionOptionModel"></param>
        /// <param name="questionOptionModels"></param>
        /// <returns></returns>
        private QuestionOptionModel GetRootOption(QuestionOptionModel questionOptionModel,
            List<QuestionOptionModel> questionOptionModels)
        {
            var parentOptionId = questionOptionModel.QuestionParentID;
            if (questionOptionModel.QuestionParentID == 0)
                return questionOptionModel;
            var finderOption = questionOptionModels.FirstOrDefault(p => p.PKID == parentOptionId);
            if (finderOption != null && finderOption.QuestionParentID == 0)
                return finderOption;

            return GetRootOption(finderOption, questionOptionModels);
        }


        /// <summary>
        ///     推送消息
        /// </summary>
        /// <param name="cacheClient"></param>
        /// <param name="templatePushClient"></param>
        /// <param name="userId"></param>
        /// <param name="userClient"></param>
        private void PushMessage(
            int pushId
            , CacheClient cacheClient
            , UserClient userClient
            , TemplatePushClient templatePushClient
            , Guid userId
            , DateTime playtime)

        {
            var cacheKey = "/ActivityUserQuestionAnswerClear/" + userId.ToString("N");
            //判断redis里面今天是否已经推送过了
            var existsResult = cacheClient.Exists(cacheKey);
            //如果不存在
            if (!existsResult.Success)
            {
                var dateDiff = (DateTime.Now.Date.AddDays(1) - DateTime.Now).Add(TimeSpan.FromMinutes(5));
                //保存到cache中
                //今天不再推送
                var setResult = cacheClient.Set(cacheKey, 1, dateDiff);

                if (setResult.Success)
                {
                    //获取用户数据
                    var userInfo = userClient.FetchUserByUserId(userId.ToString());
                    if (userInfo != null && userInfo.Success && userInfo.Result != null)
                        templatePushClient.PushByUserIDAndBatchID(new List<string> { userId.ToString() }, pushId,
                            new PushTemplateLog
                            {
                                Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>
                                {
                                    {"{{nickname}}", userInfo.Result.Nickname},
                                    {"{{playtime}}", playtime.ToString("yyyy-MM-dd")},
                                })
                            });
                }

            }
        }

        /// <summary>
        ///     刷新缓存
        /// </summary>
        private void RefreshCache(long activityId)
        {
            try
            {
                using (var activityClient = new ActivityClient())
                {
                    //刷新问题缓存
                    activityClient.RefreshActivityQuestionCache(activityId);
                    //刷新兑换物缓存
                    activityClient.RefreshActivityPrizeCache(activityId);
                }

            }
            catch (Exception e)
            {
                Logger.Info($" {nameof(ActivityUserQuestionAnswerClearJob)} 刷新缓存失败");
            }
            
        }

        #endregion
    }
}

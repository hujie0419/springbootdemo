using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>
    /// 已过期
    /// </summary>
    [DisallowConcurrentExecution]
    public class TireLunGuFTQuan0401Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TireLunGuFTQuan0401Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 4, 5, 2, 0, 0) || DateTime.Now > new DateTime(2017, 5, 5, 0, 0, 0))
                {
                    Logger.Info("2017-4-5~2017-5-5发放买轮胎送轮毂优惠券");
                    return;

                    #region 买轮胎送轮毂优惠券需求

                    //  查找条件：
                    //·渠道：自有平台（app、官网、微信）
                    //·订单类别：轮胎订单
                    //·订单状态：已提交
                    //上线时间：4月5号 零点
                    //下线时间：5月5号
                    //要求：轮胎订单提交后，半小时内发放指定轮毂优惠券到顾客账户，需要短信通知！
                    //短信文案：【途虎养车】买轮胎送福利啦！途虎送您的1800元FT系列轮毂券已到账！超炫酷超实惠，立即查看：https://l.tuhu.cn/aLAI。
                    //                说明：1.轮胎订单取消，轮毂券可以不取消；
                    //         2.同一个账户，同一个月内只推送一次。
                    //发券要求：
                    //优惠券规则编号 优惠券规则Guid   名称 说明  面额 使用条件    优惠有效期 电销可发放   支持用户范围 发行量 已领取 操作历史    操作
                    //3196    94afd44d - 24f2 - 4431 - b677 - f81173dde305    FT系列轮毂优惠券 买轮胎送轮毂优惠券   700.00  3999.00 自领取后15天 否   全部      0   查看操作历史 编辑| 复制
                    //3195    cee136c9 - df60 - 44d7 - 8df5 - 06f8601f2238    FT系列轮毂优惠券 买轮胎送轮毂优惠券   500.00  2999.00 自领取后15天 否   全部      0   查看操作历史 编辑| 复制
                    //3194    7de2771e - c793 - 4edb - 88a8 - 2fe02e5c14c8 FT系列轮毂优惠券   买轮胎送轮毂优惠券   350.00  1999.00 自领取后15天 否   全部      0   查看操作历史 编辑| 复制
                    //3193    8fc00885 - 57d2 - 4940 - 8c36 - 4699b6d37831 FT系列轮毂优惠券   买轮胎送轮毂优惠券   250.00  1399.00 自领取后15天 否   全部      0   查看操作历史 编辑| 复制

                    #endregion 买轮胎送轮毂优惠券需求
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务买轮胎送轮毂优惠券" + id);
                var tupleList = DalCoupon.Get0401TireOrders();
                if (tupleList == null || !tupleList.Any())
                {
                    Logger.Info("不存在符合要求的订单");
                    return;
                }
                Logger.Info(id + "查出订单数量为" + tupleList.Count);
                var tupleListDistinct = tupleList.Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + tupleListDistinct.Count);
                foreach (var item in tupleListDistinct)
                {
                    Logger.Info(id + $"塞券订单{item.Item2}");
                    //查看用户本月是否已获取过
                    if (!DalCoupon.GetHasGetPromotionCodeFor0401(item.Item1))
                    {
                        if (CreateOrYzPromotion(item.Item1.ToString(), item.Item2.ToString()))
                        {
                            string userTel = DalCoupon.GetUserPhoneByUserId(item.Item1);
                            //BLL.Business.SendMarketingSms(userTel, "买轮胎送福利啦！", "【途虎养车】买轮胎送福利啦！途虎送您的1800元FT系列轮毂券已到账！超炫酷超实惠，立即查看：https://l.tuhu.cn/aLAI 。");
                        }
                        else
                        {
                            Logger.Error(id + $"塞券订单{item.Item2}用户{item.Item1}塞券失败");
                        }
                    }
                    else
                    {
                        Logger.Info(id + $"塞券订单{item.Item2}用户{item.Item1}本月已经获取该劵，故不发放");
                    }
                }
                Logger.Info("结束任务" + id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public static bool CreateOrYzPromotion(string userId, string orderID)
        {
            try
            {
                var result = true;
                List<Tuple<int, int, int>> Coupons = new List<Tuple<int, int, int>>()
                {
                   Tuple.Create(3196, 700, 3999),
                    Tuple.Create(3195, 500, 2999),
                     Tuple.Create(3194, 350, 1999),
                     Tuple.Create(3193, 250, 1399)
                    };
                using (var db = DbHelper.CreateDbHelper())
                {
                    db.BeginTransaction();
                    try
                    {
                        Coupons.ForEach(f =>
                        {
                            if (result)
                            {
                                var promotionCodeModel = new PromotionCodeModel();
                                promotionCodeModel.OrderId = 0;
                                promotionCodeModel.Status = 0;
                                promotionCodeModel.Description = "买轮胎送轮毂优惠券";
                                promotionCodeModel.Discount = f.Item2;
                                promotionCodeModel.MinMoney = f.Item3;
                                promotionCodeModel.EndTime = DateTime.Now.AddDays(15).Date;
                                promotionCodeModel.StartTime = DateTime.Now.Date;
                                promotionCodeModel.UserId = new Guid(userId);
                                promotionCodeModel.CodeChannel = "FT系列轮毂优惠券";
                                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                                promotionCodeModel.RuleId = 29378;
                                promotionCodeModel.GetRuleId = f.Item1;
                                promotionCodeModel.Number = 1;
                                var CreateResult = DalCoupon.CreatePromotionCode(promotionCodeModel, db, "TireLunGuFTQuan0401Job");
                                if (CreateResult < 0)
                                {
                                    db.Rollback();
                                    Logger.Error($"优惠券创建失败：用户{userId},订单{orderID},导致异常的优惠券信息，优惠券规则Id：{f.Item1},ErrorCode={CreateResult}");
                                    result = false;
                                    return;
                                }
                            }
                        });
                        if (result)
                        {
                            db.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        db.Rollback();
                        Logger.Error(ex.Message, ex);
                        return false;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
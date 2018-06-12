using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>618福利2000元神胎券 huhengxing 2016-6-13 已过期</summary>
    [DisallowConcurrentExecution]
    public class GodTireQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GodTireQuanJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 6, 14))
                {
                    Logger.Info("2016-6-14开始发放618福利日轮胎返2000元神胎券");
                    return;
                }
                if (DateTime.Now > new DateTime(2016, 7, 15))
                {
                    Logger.Info("2016-7-15后结束发放618福利日轮胎返2000元神胎券");
                    return;
                }
                Logger.Info("启动任务");
                using (var cmd = new SqlCommand(@"[Gungnir].[dbo].[ProMotionCode_GodTireQuan]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            int result = CreateOrYzPromotion(i.UserID, i.OrderID);
                            if (result > 0)
                            {
                                BLL.Business.SendMarketingSms(i.UserTel, "途虎618福利日轮胎返2000元券", 2000, "返券");
                            }
                        }
                    }
                }

                Logger.Info("结束任务");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public static int CreateOrYzPromotion(string userId, string orderID)
        {
            try
            {
                List<PromotionCodeModel> modelList = new List<PromotionCodeModel>();
                //神胎券包括 现金券，电瓶，内饰清洗，胜牌，小保养，车品，3M，底盘装甲，大保养，智能车机，贴膜，轮毂 总计12种
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 20, 0, userId, orderID, 562, 5));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 100, 0, userId, orderID, 119, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 100, 0, userId, orderID, 36, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 100, 0, userId, orderID, 1020, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 100, 199, userId, orderID, 1312, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 100, 199, userId, orderID, 30, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 100, 199, userId, orderID, 161, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 200, 699, userId, orderID, 1256, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 200, 699, userId, orderID, 1316, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 200, 699, userId, orderID, 1022, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 300, 599, userId, orderID, 1647, 1));
                modelList.Add(GetPromotionCodeModel("618轮胎活动返券", 400, 799, userId, orderID, 1137, 1));

                if (modelList.Count() < 1)
                {
                    return -1;
                }
                using (var dbHelper = new Tuhu.WebSite.Component.SystemFramework.SqlDbHelper())//将多个数据插入数据库，需用事务保持一致
                {
                    dbHelper.BeginTransaction();
                    foreach (var mod in modelList)
                    {
                        var result = CreateMultiplePromotionCode(dbHelper, mod);
                        if (result < 1)
                        {
                            dbHelper.Rollback();
                            return result;
                        }
                    }
                    dbHelper.Commit();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }

        public static int CreateMultiplePromotionCode(Tuhu.WebSite.Component.SystemFramework.BaseDbHelper dbHelper, PromotionCodeModel promotionCodeModel)
        {
            try
            {
                using (var cmd = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StartTime", promotionCodeModel.StartTime.ToString());
                    cmd.Parameters.AddWithValue("@EndDateTime", promotionCodeModel.EndTime.ToString());
                    cmd.Parameters.AddWithValue("@Type", promotionCodeModel.Type);
                    cmd.Parameters.AddWithValue("@Description", promotionCodeModel.Description);
                    cmd.Parameters.AddWithValue("@Discount", promotionCodeModel.Discount);
                    cmd.Parameters.AddWithValue("@MinMoney", promotionCodeModel.MinMoney);
                    cmd.Parameters.AddWithValue("@Number", promotionCodeModel.Number);
                    cmd.Parameters.AddWithValue("@CodeChannel", promotionCodeModel.CodeChannel);
                    cmd.Parameters.AddWithValue("@UserID", promotionCodeModel.UserId);
                    cmd.Parameters.AddWithValue("@BatchID", promotionCodeModel.BatchId == null ? 0 : promotionCodeModel.BatchId.Value);
                    cmd.Parameters.AddWithValue("@RuleID", promotionCodeModel.RuleId);
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Results",
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Output,
                        Value = 0
                    });

                    dbHelper.ExecuteNonQuery(cmd);
                    var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }

        public static PromotionCodeModel GetPromotionCodeModel(string description, int discount, decimal minMoney, string userId, string orderID, int ruleId, int num)
        {
            PromotionCodeModel model = new PromotionCodeModel();
            model.OrderId = 0;
            model.Status = 0;
            model.Description = description;
            model.Discount = discount;
            model.MinMoney = minMoney;
            model.EndTime = DateTime.Now.AddDays(180).Date;
            model.StartTime = DateTime.Now.Date;
            model.UserId = new Guid(userId);
            model.CodeChannel = "618福利日2000元神胎券活动";
            model.BatchId = Convert.ToInt32(orderID);
            model.RuleId = ruleId;
            model.Number = num;
            return model;
        }
    }
}
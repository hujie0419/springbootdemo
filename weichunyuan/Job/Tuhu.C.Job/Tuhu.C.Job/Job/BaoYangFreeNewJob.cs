using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Service.Utility;

namespace Tuhu.C.Job.Job
{
    public class BaoYangFreeNewJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeNewJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            using (var cmd = new SqlCommand(@"SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.RuleID IN ( 1157, 1160, 1163, 1166 )
		AND O.Status = N'3Installed'
		AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.RuleID = 23
								AND PC.BatchID = O.PKID
								AND PC.CodeChannel = N'途虎免单券' );"))
            {
                var dt = DbHelper.ExecuteQuery(true, cmd, _ => _);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                foreach (var i in dic)
                {
                    if (!string.IsNullOrWhiteSpace(i.SumMoney))
                    {
                        var m = Convert.ToDecimal(i.SumMoney);
                        switch (i.RuleID)
                        {
                            case "1157":
                                if (m > 100)
                                    m = 100;
                                break;

                            case "1160":
                                if (m > 200)
                                    m = 200;
                                break;

                            case "1163":
                                if (m > 300)
                                    m = 300;
                                break;

                            case "1166":
                                if (m > 400)
                                    m = 400;
                                break;
                        }
                        CreateOrYzPromotion(i.UserID, m, int.Parse(i.OrderID));
                        //感谢您参与买轮胎送保养免单券活动，{0}元的保养现金券已经发至您的账户，可下载手机APP查看 http://t.cn/Ryy1xkt
                        using (var client = new SmsClient())
                        {
                            client.SendSms(i.UserTel, 30, m.ToString());
                        }
                    }
                }
            }

            Logger.Info("结束任务");
        }

        public static void CreateOrYzPromotion(string userId, decimal money, int OrderID)
        {
            if (money > 1000)
                money = 1000;

            var promotionCodeModel = new PromotionCodeModel();
            promotionCodeModel.OrderId = 0;
            promotionCodeModel.Status = 0;
            promotionCodeModel.Description = "满" + (money + 1) + "元可用，仅限保养产品使用";
            promotionCodeModel.Discount = (int)Math.Ceiling(money);
            promotionCodeModel.MinMoney = (money + 1);
            promotionCodeModel.EndTime = DateTime.Now.AddMonths(6).Date;
            promotionCodeModel.StartTime = DateTime.Now.Date;
            promotionCodeModel.UserId = new Guid(userId);
            promotionCodeModel.CodeChannel = "途虎免单券";
            promotionCodeModel.BatchId = OrderID;
            promotionCodeModel.RuleId = 23;
            var result = CreateMultiplePromotionCode(promotionCodeModel, 1);
        }

        public static int CreateMultiplePromotionCode(PromotionCodeModel promotionCodeModel, int num)
        {
            if (num == -1)
                num = promotionCodeModel.Number;

            return CreateMultiplePromotionCode2(promotionCodeModel, num);
        }

        public static int CreateMultiplePromotionCode2(PromotionCodeModel promotionCodeModel, int num)
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
                    cmd.Parameters.AddWithValue("@Number", num);
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

                    DbHelper.ExecuteNonQuery(cmd);
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
    }
}

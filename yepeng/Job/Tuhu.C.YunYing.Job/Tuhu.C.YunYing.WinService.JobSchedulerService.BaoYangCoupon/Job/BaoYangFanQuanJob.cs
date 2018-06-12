using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.WebSite.Component.SystemFramework.Models;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>保养免单活动返券 huhengxing 2016-4-13 已过期</summary>
    [DisallowConcurrentExecution]
    public class BaoYangFanQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFanQuanJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now > new DateTime(2016, 5, 21))
                {
                    Logger.Info("2016-5-20后结束发放到店保养券");
                    return;
                }
                Logger.Info("启动任务");
                using (var cmd = new SqlCommand(@"[Gungnir].[dbo].[ProMotionCode_FanQuan]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            var items = SelectOrderListByOrderId(i.OrderID);
                            var oil = items.Where(x => x.Pid.LastIndexOf("OL-") == 0).ToList();
                            var other = items.Where(x => x.Pid.LastIndexOf("OL-") != 0).ToList();
                            var oilMoney = oil.Select(x => x.TotalPrice).AsQueryable().Sum();
                            if (oilMoney > 300)
                                oilMoney = 300;

                            var otherMoney = other.Select(x => x.TotalPrice).AsQueryable().Sum();
                            var m = Convert.ToInt32((otherMoney + oilMoney).ToString("0"));
                            if (m > 500)
                                m = 500;

                            int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                            if (result > 0)
                            {
                                BLL.Business.SendMarketingSms(i.UserTel, "途虎保养免单", m, "保养现金券");
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

        public static IEnumerable<OrderItem> SelectOrderListByOrderId(string orderId)
        {
            using (var cmd = new SqlCommand(@"SELECT
                                               PKID,OrderID,OrderNo,PID,Poid,Category,Name,
                                                Size,Remark,CarCode,CarName,Num,MarkedPrice,Discount,Price,TotalDiscount,
                                                TotalPrice,LastUpdateTime,PurchasePrice,Cost,InstallFee,Vendor,IsPaid,PaidVia,
                                                InstockDate,PaidDate,PurchaseStatus,IsInstallFeePaid,InstallFeePaidDate,Deleted,
                                                CreateDate,Commission,HCNum,OrigProdId,ProductType,ParentID,WeekYear,RefID,
                                                FUPID,PromotionCode,PromotionMoney,MatchedProducts,ExtCol,IsDaiFa,NodeNo,
                                                TotalManualDiscount,ListPrice,PayPrice,TuhuCost
                                              FROM Gungnir..tbl_OrderList WITH ( NOLOCK ) WHERE OrderID=@OrderID AND Deleted = 0 "))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                if (money > 500)
                    money = 500;

                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;

                promotionCodeModel.Description = "保养免单活动返券，限到店保养使用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = (money + 1);
                promotionCodeModel.EndTime = DateTime.Now.AddDays(90).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "到店保养券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 23;
                var result = CreateMultiplePromotionCode(promotionCodeModel, 1);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
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
                    //cmd.Parameters.AddWithValue("@PromtionName", promotionCodeModel.CodeChannel);//PromotionCodeModel无PromtionName
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

    public class OrderItem : BaseModel
    {
        public OrderItem() : base()
        {
        }

        public OrderItem(DataRow dr) : base(dr)
        {
        }

        public string Pid { set; get; }
        public int Num { set; get; }
        public decimal Price { set; get; }
        public decimal TotalPrice { set; get; }
    }

    public class PromotionCodeModel : BaseModel
    {
        public PromotionCodeModel()
        {
            this.Number = 1;
        }

        public PromotionCodeModel(DataRow row)
        {
            base.Parse(row);

            if (this.EndTime != null)
                this.EndDateTime = this.EndTime.ToString("yyyy-MM-dd");
            if (this.IsGift == null)
                this.IsGift = 0;
        }

        public int Number { set; get; }
        public string EndDateTime { set; get; }

        /// <summary>
        /// 主键编号
        /// </summary>
        public int Pkid { get; set; }
        /// <summary>
        /// 券名称
        /// </summary>
        public string PromtionName { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 优惠券券号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 创建优惠券时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 优惠券面值
        /// </summary>
        public decimal? MinMoney { get; set; }

        /// <summary>
        /// 优惠券有效期（开始时间）
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 优惠券有效期（结束时间）
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 优惠券使用状态 0:未使用  1:已使用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 优惠券类别
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 优惠券使用时间
        /// </summary>
        public DateTime? UsedTime { get; set; }

        public int Discount { get; set; }
        public int OrderId { get; set; }
        public int? Type { get; set; }

        //是否分享过
        public int? IsGift { get; set; }

        public string UserPhone { get; set; }

        /// <summary>优惠券生成渠道 </summary>
        public string CodeChannel { get; set; }

        /// <summary>优惠券批次号 </summary>
        public int? BatchId { get; set; }

        public int? RuleId { get; set; }
        public int? GetRuleId { get; set; }
    }

    public class OrderList : BaseModel
    {
        public OrderList() : base()
        {
        }

        public OrderList(System.Data.DataRow dr) : base(dr)
        {
        }

        public Guid UserId { set; get; }
        public int Num { set; get; }
        public string Status { set; get; }
        public string InstallStatus { set; get; }
        public string InstallType { set; get; }
        public int InstallShopId { set; get; }
        public string DeliveryStatus { set; get; }
    }
}

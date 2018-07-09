using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Tuhu.Models;
using Tuhu.Service.Utility;

namespace Tuhu.C.Job.Job
{
    /// <summary>Sql代理</summary>
    [DisallowConcurrentExecution]
    public class AppNewUserGiftJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AppNewUserGiftJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            using (var cmd = new SqlCommand(@"SELECT	u_user_id,
															u_mobile_number
													FROM	Tuhu_profiles..UserObject AS UO WITH (NOLOCK)
													WHERE	dt_date_first_time_login_app IS NOT NULL
															AND DATEDIFF(DAY, dt_date_first_time_login_app, GETDATE()) = 1
															AND u_user_id NOT IN (SELECT	UserId
																				  FROM		Gungnir..tbl_PromotionCode AS PC WITH (NOLOCK)
																				  WHERE		CodeChannel = N'途虎新手大礼包')"))
            {
                var dt = DbHelper.ExecuteQuery(cmd, _ => _);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["u_user_id"].ToString(), UserPhone = x["u_mobile_number"].ToString() }).ToList();
                foreach (var i in dic)
                {
                    CreateOrYzPromotion(i.UserID);
                    //作为途虎最重视的用户，特赠送您途虎新手大礼包：{0}元代金券组合。换轮胎做保养买车品可用。快打开APP查看，及时使用，避免浪费。
                    using (var client = new SmsClient())
                    {
                        client.SendSms(i.UserPhone, 29, "50");
                    }
                }
            }

            Logger.Info("结束任务");
        }

        public static void CreateOrYzPromotion(string userId)
        {
            var lPro = new List<PromotionCodeModel>();
            for (var i = 0; i < 6; i++)
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                switch (i)
                {
                    case 0://轮胎券
                        promotionCodeModel.Type = 1;
                        promotionCodeModel.Description = "满599元可用，仅限轮胎产品使用";
                        promotionCodeModel.Discount = 10;
                        promotionCodeModel.MinMoney = 599;
                        promotionCodeModel.EndTime = DateTime.Now.AddDays(14);
                        promotionCodeModel.RuleId = 25;
                        break;

                    case 1://保养券
                        promotionCodeModel.Type = 2;
                        promotionCodeModel.Description = "满299元可用，仅限保养产品使用";
                        promotionCodeModel.Discount = 20;
                        promotionCodeModel.MinMoney = 299;
                        promotionCodeModel.EndTime = DateTime.Now.AddDays(14);
                        promotionCodeModel.RuleId = 23;
                        break;

                    case 2://车品券
                        promotionCodeModel.Type = 8;
                        promotionCodeModel.Description = "满79元可用，仅限车品产品使用";
                        promotionCodeModel.Discount = 5;
                        promotionCodeModel.MinMoney = 79;
                        promotionCodeModel.EndTime = DateTime.Now.AddDays(14);
                        promotionCodeModel.RuleId = 30;
                        break;

                    case 3://美容券
                        promotionCodeModel.Type = 5;
                        promotionCodeModel.Description = "可用于洗车、打蜡产品使用";
                        promotionCodeModel.Discount = 5;
                        promotionCodeModel.MinMoney = 5;
                        promotionCodeModel.EndTime = DateTime.Now.AddDays(14);
                        promotionCodeModel.RuleId = 32;
                        break;

                    case 4://保养券
                        promotionCodeModel.Type = 2;
                        promotionCodeModel.Description = "满199元可用，仅限保养产品使用";
                        promotionCodeModel.Discount = 10;
                        promotionCodeModel.MinMoney = 199;
                        promotionCodeModel.EndTime = DateTime.Now.AddDays(14);
                        promotionCodeModel.RuleId = 23;
                        break;

                    case 5://洗车券
                        promotionCodeModel.Type = 3;
                        promotionCodeModel.Description = "可用于标准洗车5座";
                        promotionCodeModel.Discount = 0;
                        promotionCodeModel.MinMoney = 0;
                        promotionCodeModel.EndTime = DateTime.Now.AddDays(14);
                        promotionCodeModel.RuleId = 42;
                        break;
                }
                promotionCodeModel.StartTime = DateTime.Now;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "途虎新手大礼包";
                promotionCodeModel.BatchId = -1;

                lPro.Add(promotionCodeModel);
            }
            using (var helper = DbHelper.CreateDbHelper())
            {
                helper.BeginTransaction();
                for (var i = 0; i < lPro.Count; i++)
                {
                    var promotionCodeModel = new PromotionCodeModel();
                    promotionCodeModel = lPro[i];

                    #region 创建优惠券

                    var result = 0;
                    try
                    {
                        result = CreateMultiplePromotionCode(promotionCodeModel, 1);
                        if (result > 0)
                        {
                        }
                        else
                        {
                            helper.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        helper.Rollback();
                    }

                    #endregion 创建优惠券
                }
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

    public class PromotionCodeModel : BaseModel
    {
        public PromotionCodeModel()
        {
            this.Number = 1;
        }

        protected override void Parse(DataRow row, PropertyInfo[] properties)
        {
            base.Parse(row, properties);

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
    }
}

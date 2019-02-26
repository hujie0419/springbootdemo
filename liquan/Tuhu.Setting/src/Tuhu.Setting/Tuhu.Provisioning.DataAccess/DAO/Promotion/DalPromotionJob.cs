using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalPromotionJob
    {
        public static List<BizPromotionCode> SelectPromotionCodesByUserId(string userId)
        {
            var sqlparamters = new[]
            {
                new SqlParameter("@UserId", userId)
            };
            return DbHelper.ExecuteDataTable("User_Promotion_SelectUserPromotionByUserId", CommandType.StoredProcedure
                , sqlparamters).ConvertTo<BizPromotionCode>().ToList();
        }

        /// <summary>
        /// 根据订单号获取优惠券
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public static List<BizPromotionCode> SelectPromotionByOrderId(int OrderId)
        {
            var sqlparamters = new[]
            {
                new SqlParameter("@OrderId", OrderId)
            };
            return DbHelper.ExecuteDataTable(
                @"SELECT	PC.Code,--优惠码
				            PC.Type,--优惠券类型
				            PC.Description,--优惠描述
				            PC.MinMoney,--满足多少钱才能使用
				            PC.Discount,--优惠价格
				            PC.StartTime,--开始时间
				            PC.EndTime,--结束时间
				            PC.UsedTime,--使用时间
				            PC.OrderId,--使用订单号
				            PC.Status,--优惠券状态
				            pc.PKID AS PkId
	                FROM		dbo.tbl_PromotionCode AS PC WITH(NOLOCK)
	                WHERE		PC.OrderId = @OrderId
	                ORDER BY pc.Type",
                CommandType.Text
                , sqlparamters
            ).ConvertTo<BizPromotionCode>().ToList();
        }


        /// <summary>
        /// 获得一个订单已经使用的优惠券
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static int GetOrderUsedPromtionCodeNumByOrderId(SqlConnection sqlConnection, int orderId)
        {
            var sqlparamters = new[]
            {
                new SqlParameter("@OrderId", orderId)
            };
            int num = 0;
            var obj = SqlHelper.ExecuteScalar(sqlConnection, CommandType.StoredProcedure, "GetOrderUsedPromtionCodeNumByOrderId", sqlparamters);
            var tryParse = int.TryParse(obj == null ? "0" : obj.ToString(), out num);
            return num;
        }

        /// <summary>
        /// 根据优惠券PKID获取优惠券信息
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="promotionCode">优惠券PKID</param>
        /// <returns></returns>
        public static BizPromotionCode FetchPromotionCodeByPromotionCode(SqlConnection connection, int promotionCode)
        {
            const string sql = @"  SELECT   TOP 1 *
									FROM    dbo.tbl_PromotionCode AS PC WITH ( NOLOCK )
									WHERE   PKID = @PromotionCode;";

            var sqlparamters = new[]
            {
                new SqlParameter("@PromotionCode", promotionCode)
            };
            return SqlHelper.ExecuteDataTable2(connection, CommandType.Text, sql, sqlparamters).ConvertTo<BizPromotionCode>()?.FirstOrDefault();
        }

        /// <summary>
        /// 统计所有发布时间、发布渠道、已兑换
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public static DataTable SelectExchangeCodeDetailByPage(int PageNumber, int PageSize, out int TotalCount)
        {
            TotalCount = 0;
            using (var cmd = new SqlCommand("[Activity].[dbo].[SelectPromotionCode_FenYe]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                DataTable dt = DbHelper.ExecuteDataTable(cmd);
                TotalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dt;
            }
        }


        public static DataTable SelectGiftBag(int PageNumber, int PageSize, out int TotalCount)
        {
            TotalCount = 0;
            using (var cmd = new SqlCommand("[Activity].[dbo].[GiftBag_tbl_ExchangeCodeDetailFenYe]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                DataTable dt = DbHelper.ExecuteDataTable(cmd);
                TotalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dt;
            }
        }

        public static DataTable SelectGiftBagByPKID(int pkid)
        {
            var cmd = string.Format(@"SELECT ECD.PKID,CodeType,Name,CodeChannel,CONVERT(VARCHAR(30),CodeStartTime,102)AS CodeStartTime,CONVERT(VARCHAR(30),CodeEndTime,102)AS CodeEndTime,Money,MinMoney,Number,Validity,ECD.IsActive FROM Activity..tbl_ExchangeCodeDetail AS ECD 
              WHERE ECD.ParentID={0} order by PKID desc", pkid);
            return DbHelper.ExecuteDataTable(cmd);

        }

        public static DataTable SelectGiftByDonwLoad(int pkid)
        {
            var cmd = string.Format(@" SELECT ECD.PKID,ECD.IsActive ,(SELECT COUNT(1) FROM Activity..tbl_ExchangeCodeDetail WHERE ParentID={0}) AS T  FROM Activity..tbl_ExchangeCodeDetail AS ECD 
              WHERE ECD.PKID={0}", pkid);
            return DbHelper.ExecuteDataTable(cmd);
        }

        public static int SelectDownloadByPKID(int pkid)
        {
            var cmd = string.Format(
                @"SELECT Count(1) FROM Activity..tbl_ExchangeCode AS EC
                WHERE EC.DetailsID={0}", pkid);
            return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
        }

        public static DataTable GetEdit(int pkid)
        {
            var cmd = string.Format(@"SELECT ECD.PKID,ecd.ParentID,Name,CodeChannel,Validity,ecd.CodeType,ecd.Money,ecd.MinMoney,ecd.Number,CONVERT(VARCHAR(30),ECD.CodeStartTime,102) AS CodeStartTime
,CONVERT(VARCHAR(30),ECD.CodeEndTime,102) AS CodeEndTime,ECD.IsActive,ecd.Validity FROM Activity..tbl_ExchangeCodeDetail AS ECD 
WHERE ECD.PKID={0}", pkid);
            return DbHelper.ExecuteDataTable(cmd);
        }

        public static int UpdateGift(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Activity..tbl_ExchangeCodeDetail
                                            SET     Name = @Name ,
                                                    CodeChannel = @CodeChannel ,
                                                    CodeType = @CodeType ,
                                                    Money = @Money ,
                                                    MinMoney = @MinMoney ,
                                                    Number = @Number ,
                                                    CodeStartTime = @CodeStartTime ,
                                                    CodeEndTime = @CodeEndTime ,
                                                    IsActive = @IsActive,
                                                    Validity=@Validity
                                            WHERE   PKID = @PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", ecd.PKID);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@CodeType", ecd.CodeType);
                cmd.Parameters.AddWithValue("@Money", ecd.Money);
                cmd.Parameters.AddWithValue("@MinMoney", ecd.MinMoney);
                cmd.Parameters.AddWithValue("@CodeStartTime", ecd.CodeStartTime);
                cmd.Parameters.AddWithValue("@CodeEndTime", ecd.CodeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                cmd.Parameters.AddWithValue("@Validity", ecd.Validity);
                cmd.Parameters.AddWithValue("@Number", ecd.Number);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }

        public static int DoUpdateOEM(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Activity..tbl_ExchangeCodeDetail
                                            SET     Name = @Name ,
                                                    CodeChannel = @CodeChannel ,
                                                    ExChangeStartTime = @ExChangeStartTime ,
                                                    ExChangeEndTime = @ExChangeEndTime ,
                                                    IsActive = @IsActive 
                                            WHERE   PKID = @PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", ecd.PKID);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@ExChangeStartTime", ecd.ExChangeStartTime);
                cmd.Parameters.AddWithValue("@ExChangeEndTime", ecd.ExChangeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }


        public static int AddGift(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Activity..tbl_ExchangeCodeDetail(ParentID,Name,CodeChannel,CodeType,Money,MinMoney,Number,CodeStartTime,CodeEndTime,IsActive,Validity)
VALUES(@ParentID,@Name,@CodeChannel,@CodeType,@Money,@MinMoney,@Number,@CodeStartTime,@CodeEndTime,@IsActive,@Validity)"))
            {
                cmd.Parameters.AddWithValue("@ParentID", ecd.ParentID);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@CodeType", ecd.CodeType);
                cmd.Parameters.AddWithValue("@Money", ecd.Money);
                cmd.Parameters.AddWithValue("@MinMoney", ecd.MinMoney);
                cmd.Parameters.AddWithValue("@CodeStartTime", ecd.CodeStartTime);
                cmd.Parameters.AddWithValue("@CodeEndTime", ecd.CodeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                cmd.Parameters.AddWithValue("@Validity", ecd.Validity);
                cmd.Parameters.AddWithValue("@Number", ecd.Number);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static object SelectCodeChannelByAddGift(int id)
        {
            using (var cmd = new SqlCommand(@"  select CodeChannel from [Activity].[dbo].[tbl_ExchangeCodeDetail] where pkid=@PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", id);
                return DbHelper.ExecuteScalar(cmd);
            }
        }

        public static int AddOEM(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Activity..tbl_ExchangeCodeDetail(ParentID,Name,CodeChannel,ExChangeStartTime,ExChangeEndTime,IsActive)
VALUES(@ParentID,@Name,@CodeChannel,@ExChangeStartTime,@ExChangeEndTime,@IsActive)"))
            {
                cmd.Parameters.AddWithValue("@ParentID", 0);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@ExChangeStartTime", ecd.ExChangeStartTime);
                cmd.Parameters.AddWithValue("@ExChangeEndTime", ecd.ExChangeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static string GenerateCoupon(int Number, int DetailsID)
        {
            try
            {
                using (var cmd = new SqlCommand("[Gungnir].[dbo].[Promotion_CreateExchangeCode]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Number", Number);
                    cmd.Parameters.AddWithValue("@DetailsID", DetailsID);
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Results",
                        Direction = ParameterDirection.Output,
                        SqlDbType = SqlDbType.Int
                    });
                    DbHelper.ExecuteDataTable(cmd);
                    return cmd.Parameters["@Results"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string CreateOrUpdatePromotionTaskPromotionListNew(PromotionTaskCouponRule info)
        {
            #region sqlAdd
            const string sqlAdd = @"INSERT INTO dbo.tbl_PromotionTaskPromotionList
		        (CouponRulesId ,
		          PromotionDescription ,
		          StartTime ,
		          EndTime ,
		          MinMoney ,
		          DiscountMoney,
				  FinanceMarkName,
				  IntentionId,
                  DepartmentId,
                  Issuer,
				  IssueChannle,
				  IssueChannleId,
                  Creater,
				  IntentionName,
				  DepartmentName,
				  Number,
                  BusinessLineId,
                  BusinessLineName,
                  IsRemind,
                  IsPush,
                  PushSetting
		        )
		VALUES  ( @CouponRulesId , -- CouponRulesId - int
		          @PromotionDescription , -- PromotionDescription - nvarchar(200)
		          @StartTime , -- StartTime - datetime
		          @EndTime , -- EndTime - datetime
		          @MinMoney , -- MinMoney - float
		          @DiscountMoney,  -- DiscountMoney - float
				  @FinanceMarkName,
				  @IntentionId,
                  @DepartmentId,
                  @Issuer,
				  @IssueChannle,
				  @IssueChannleId,
                  @Creater,
				  @IntentionName,
				  @DepartmentName,
				  @Number,
                  @BusinessLineId,
                  @BusinessLineName,
                  @IsRemind,
                  @IsPush,
                  @PushSetting
		        );SELECT @@IDENTITY;";
            #endregion

            #region sqlUpdate
            const string sqlUpdate = @"UPDATE tbl_PromotionTaskPromotionList
		SET CouponRulesId = @CouponRulesId,
		PromotionDescription = @PromotionDescription,
		StartTime = @StartTime,
		EndTime = @EndTime,
		MinMoney = @MinMoney,
		DiscountMoney = @DiscountMoney,
		FinanceMarkName=@FinanceMarkName,
		IntentionId=@IntentionId,
        DepartmentId=@DepartmentId,
        Issuer=@Issuer,
		IssueChannle=@IssueChannle,
		IssueChannleId=@IssueChannleId,
		DepartmentName=@DepartmentName,
		IntentionName=@IntentionName,
		Number=@Number,
        BusinessLineId=@BusinessLineId,
        BusinessLineName=@BusinessLineName,
        IsRemind=@IsRemind,
        IsPush=@IsPush,
        PushSetting=@PushSetting,
		UpdateTime = GETDATE()
		WHERE TaskPromotionListId = @TaskPromotionListId";
            #endregion
            try
            {
                string sql = string.Empty;
                bool IsAdd = (info.TaskPromotionListId ?? 0) <= 0;
                if (IsAdd)//add
                {
                    sql = sqlAdd;
                }
                else
                {
                    sql = sqlUpdate;
                }
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@TaskPromotionListId", info.TaskPromotionListId);
                    cmd.Parameters.AddWithValue("@CouponRulesId", info.CouponRulesId);
                    cmd.Parameters.AddWithValue("@PromotionDescription", info.PromotionDescription);
                    cmd.Parameters.AddWithValue("@StartTime", info.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", info.EndTime);
                    cmd.Parameters.AddWithValue("@MinMoney", info.MinMoney);
                    cmd.Parameters.AddWithValue("@DiscountMoney", info.DiscountMoney);
                    cmd.Parameters.AddWithValue("@FinanceMarkName", info.FinanceMarkName);
                    cmd.Parameters.AddWithValue("@IntentionId", info.IntentionId);
                    cmd.Parameters.AddWithValue("@DepartmentId", info.DepartmentId);
                    cmd.Parameters.AddWithValue("@IntentionName", info.IntentionName);
                    cmd.Parameters.AddWithValue("@DepartmentName", info.DepartmentName);
                    cmd.Parameters.AddWithValue("@Issuer", info.Issuer);
                    cmd.Parameters.AddWithValue("@IssueChannle", "手动塞券");
                    cmd.Parameters.AddWithValue("@IssueChannleId", null);
                    cmd.Parameters.AddWithValue("@Creater", info.Creater);
                    cmd.Parameters.AddWithValue("@Number", info.Number);
                    cmd.Parameters.AddWithValue("@BusinessLineId", info.BusinessLineId);
                    cmd.Parameters.AddWithValue("@BusinessLineName", info.BusinessLineName);
                    cmd.Parameters.AddWithValue("@IsRemind", info.IsRemind);
                    cmd.Parameters.AddWithValue("@IsPush", info.IsPush);
                    cmd.Parameters.AddWithValue("@PushSetting", info.PushSetting);
                    if (IsAdd)//新增
                    {
                        return DbHelper.ExecuteScalar(cmd).ToString(); ;
                    }
                    DbHelper.ExecuteNonQuery(cmd);
                    return info.TaskPromotionListId?.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static DataTable UpdateOEM(int pkid)
        {
            using (var cmd = new SqlCommand(@"  SELECT  p.PKID,p.ParentID,CONVERT(VARCHAR(30),p.ExChangeStartTime,102) AS ExChangeStartTime,CONVERT(VARCHAR(30),p.ExChangeEndTime,102) AS ExChangeEndTime, isnull(Name,'') AS Name,CodeChannel,ISNULL(p.CodeType,'-1') AS CodeType,ISNULL(p.Money,'') AS Money,ISNULL(p.MinMoney,'') AS MinMoney,ISNULL(p.Number,'') AS Number,CONVERT(VARCHAR(30),p.CodeStartTime,102) AS CodeStartTime
,CONVERT(VARCHAR(30),p.CodeEndTime,102) AS CodeEndTime,ISNULL(p.IsActive,'') AS IsActive,ISNULL(p.Validity,'') AS Validity
    FROM    Activity..tbl_ExchangeCodeDetail AS P WITH ( NOLOCK )where PKID=@PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataTable CreateExcel(int pkid)
        {
            var cmd = new SqlCommand("SELECT EC.Code FROM Activity..tbl_ExchangeCode AS EC WHERE EC.DetailsID=@DetailsID and Status=0");
            cmd.Parameters.AddWithValue("@DetailsID", pkid);
            return DbHelper.ExecuteDataTable(cmd);
        }


        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int DeletePromoCode(int pkid)
        {
            var cmd = new SqlCommand(@"  DELETE FROM Activity..tbl_ExchangeCodeDetail WHERE PKID=@PKID");
            cmd.Parameters.AddWithValue("@PKID", pkid);
            return DbHelper.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 查询改礼包是否已生成券
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int SelectPromoCodeCount(int pkid)
        {
            var cmd = new SqlCommand(@" SELECT COUNT(1) FROM  [Activity].[dbo].[tbl_ExchangeCode] WHERE DetailsID=@PKID");
            cmd.Parameters.AddWithValue("@PKID", pkid);
            return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
        }

        /// <summary>
        /// 删除礼包
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int DeleteGift(int pkid)
        {
            var cmd = new SqlCommand(@"DELETE FROM [Activity].[dbo].[tbl_ExchangeCodeDetail] WHERE ParentID=@PKID or PKID=@PKID ");
            cmd.Parameters.AddWithValue("@PKID", pkid);
            return DbHelper.ExecuteNonQuery(cmd);
        }

        public static DataSet SelectByPhoneNum(string PhoneNum)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            using (var cmd = new SqlCommand(@"[Activity].[dbo].[SelectByPhoneNum]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Phone", PhoneNum);
                return dbHelper.ExecuteDataSet(cmd);
            }
        }
        /// <summary>
        /// 查询优惠券下拉列表
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectDropDownList()
        {
            using (var cmd = new SqlCommand(@"SELECT PKID,Name FROM Activity..tbl_CouponRules WITH (NOLOCK) WHERE ParentID=0"))
            {
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="ecd"></param>
        /// <returns></returns>
        public static int CreeatePromotion(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Activity..tbl_ExchangeCodeDetail(ParentID,Name,CodeChannel,RuleId,Money,MinMoney,Number,CodeStartTime,CodeEndTime,IsActive,Validity,CodeType)
VALUES(@ParentID,@Name,@CodeChannel,@RuleId,@Money,@MinMoney,@Number,@CodeStartTime,@CodeEndTime,@IsActive,@Validity,@CodeType)"))
            {
                cmd.Parameters.AddWithValue("@ParentID", ecd.ParentID);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@RuleId", ecd.RuleId);
                cmd.Parameters.AddWithValue("@CodeType", ecd.CodeType);
                cmd.Parameters.AddWithValue("@Money", ecd.Money);
                cmd.Parameters.AddWithValue("@MinMoney", ecd.MinMoney);
                cmd.Parameters.AddWithValue("@CodeStartTime", ecd.CodeStartTime);
                cmd.Parameters.AddWithValue("@CodeEndTime", ecd.CodeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                cmd.Parameters.AddWithValue("@Validity", ecd.Validity);
                cmd.Parameters.AddWithValue("@Number", ecd.Number);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 优惠券列表详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataTable SelectPromotionDetails(int pkid)
        {
            using (var cmd = new SqlCommand(@"SELECT  ECD.PKID ,
		        CodeType ,
                CR.Name AS QName ,
                ECD.Name ,
                CodeChannel ,
                CONVERT(VARCHAR(30), CodeStartTime, 102) AS CodeStartTime ,
                CONVERT(VARCHAR(30), CodeEndTime, 102) AS CodeEndTime ,
                Money ,
                MinMoney ,
                Number ,
                Validity ,
                ECD.IsActive
        FROM    Activity..tbl_ExchangeCodeDetail AS ECD
                LEFT JOIN Activity..tbl_CouponRules CR ON ECD.RuleId = CR.PKID
                WHERE ECD.ParentID=@ParentID order by PKID desc"))
            {
                cmd.Parameters.AddWithValue("@ParentID", pkid);
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 查询优惠券详情-->修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable SelectPromotionDetailsByEdit(int id)
        {
            using (var cmd = new SqlCommand(@"SELECT ECD.PKID,ecd.ParentID,Name,CodeChannel,Validity,ecd.RuleId,ecd.Money,ecd.MinMoney,ecd.Number,
                    CONVERT(VARCHAR(30),ECD.CodeStartTime,102) AS CodeStartTime
                    ,CONVERT(VARCHAR(30),ECD.CodeEndTime,102) AS CodeEndTime,ECD.IsActive,ecd.Validity 
                    FROM Activity..tbl_ExchangeCodeDetail AS ECD 
                    WHERE ECD.PKID=@id"))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 修改优惠券
        /// </summary>
        /// <param name="ecd"></param>
        /// <returns></returns>
        public static int UpdatePromotionDetailsByOK(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Activity..tbl_ExchangeCodeDetail
                                            SET     Name = @Name ,
                                                    CodeChannel = @CodeChannel ,
                                                    RuleId = @RuleId ,
													CodeType=@CodeType,
                                                    Money = @Money ,
                                                    MinMoney = @MinMoney ,
                                                    Number = @Number ,
                                                    CodeStartTime = @CodeStartTime ,
                                                    CodeEndTime = @CodeEndTime ,
                                                    IsActive = @IsActive,
                                                    Validity=@Validity
                                            WHERE   PKID = @PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", ecd.PKID);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@CodeType", ecd.CodeType);
                cmd.Parameters.AddWithValue("@RuleId", ecd.RuleId);
                cmd.Parameters.AddWithValue("@Money", ecd.Money);
                cmd.Parameters.AddWithValue("@MinMoney", ecd.MinMoney);
                cmd.Parameters.AddWithValue("@CodeStartTime", ecd.CodeStartTime);
                cmd.Parameters.AddWithValue("@CodeEndTime", ecd.CodeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                cmd.Parameters.AddWithValue("@Validity", ecd.Validity);
                cmd.Parameters.AddWithValue("@Number", ecd.Number);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }

        /// <summary>
        /// 创建优惠券执行任务
        /// </summary>
        /// <param name="promotionTask">优惠券任务对象</param>
        /// <param name="operateBy">操作者</param>
        /// <param name="cellPhones">需要发券的用户列表</param>
        /// <returns></returns>
        public static int CreateOrUpdatePromotionTask(PromotionTask promotionTask, string operateBy, List<string> TaskPromotionListIds = null,
            List<string> cellPhones = null)
        {
            try
            {

                //新增一条优惠券任务，同时添加到操作日志中
                using (var command = new SqlCommand("[Gungnir].[dbo].[Promotion_CreateOrUpdatePromotionTask]"))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    var taskPromotionListIdsTable = new DataTable();
                    taskPromotionListIdsTable.Columns.Add("TaskPromotionListId", Type.GetType("System.Int32"));
                    TaskPromotionListIds.ForEach(item =>
                    {
                        taskPromotionListIdsTable.Rows.Add(taskPromotionListIdsTable.NewRow()[0] = item);
                    });
                    var cellPhoneTable = new DataTable();
                    cellPhoneTable.Columns.Add("TaskId", Type.GetType("System.Int32"));
                    cellPhoneTable.Columns.Add("UserCellPhone", Type.GetType("System.String"));

                    foreach (var cellPhone in cellPhones)
                    {
                        var newRow = cellPhoneTable.NewRow();
                        newRow[0] = 0;
                        newRow[1] = cellPhone;
                        cellPhoneTable.Rows.Add(newRow);
                    }

                    var sqlParameters = new[]{
                        new SqlParameter("@PromotionTaskId",promotionTask.PromotionTaskId),
                        new SqlParameter("@TaskPromotionListIds",taskPromotionListIdsTable),
                        new SqlParameter("@CellPhones",cellPhoneTable),
                        new SqlParameter("@TaskName",promotionTask.TaskName),
                        new SqlParameter("@TaskType", promotionTask.TaskType),
                        new SqlParameter("@TaskStartTime", promotionTask.TaskStartTime),
                        new SqlParameter("@TaskEndTime", promotionTask.TaskEndTime),
                        new SqlParameter("@ExecPeriod", promotionTask.ExecPeriod),
                        new SqlParameter("@SelectUserType", promotionTask.SelectUserType),
                        new SqlParameter("@FilterStartTime", promotionTask.FilterStartTime),
                        new SqlParameter("@FilterEndTime", promotionTask.FilterEndTime),
                        new SqlParameter("@Brand", promotionTask.Brand),
                        new SqlParameter("@Category", promotionTask.Category),
                        new SqlParameter("@Pid", promotionTask.Pid),
                        new SqlParameter("@SpendMoney", promotionTask.SpendMoney),
                        new SqlParameter("@PurchaseNum", promotionTask.PurchaseNum),
                        new SqlParameter("@Area", promotionTask.Area),
                        new SqlParameter("@Channel", promotionTask.Channel),
                        new SqlParameter("@OrderType", promotionTask.OrderType),
                        new SqlParameter("@InstallType", promotionTask.InstallType),
                        new SqlParameter("@OrderStatus", promotionTask.OrderStatus),
                        new SqlParameter("@Seable", promotionTask.Seable),
                        new SqlParameter("@Vehicle", promotionTask.Vehicle),
                        new SqlParameter("@CreateBy",operateBy),
                        new SqlParameter("@AfterValue",string.Empty),
                        new SqlParameter("@IsLimitOnce",promotionTask.IsLimitOnce),
                        new SqlParameter("@SmsId",promotionTask.SmsId),
                        new SqlParameter("@SmsParam",promotionTask.SmsParam),
                        new SqlParameter("@PromotionTaskActivityId",promotionTask.PromotionTaskActivityId),
                        new SqlParameter("@IsImmediately",promotionTask.IsImmediately),
                        new SqlParameter("@ProductType",promotionTask.ProductType),
                        new SqlParameter() {
                            ParameterName = "@PromotionTaskId_Output",
                            Direction = ParameterDirection.Output,
                            SqlDbType = SqlDbType.Int
                        }
                        //new SqlParameter("@AfterValue",string.Format("优惠券RuleId:{0},优惠券描述:{1},优惠券任务类型:{2},任务开始时间:{3},任务结束时间:{4},使用金额:{5},优惠金额:{6},筛选用户数量:{7}",
                        //promotionTask.PromotionRuleId,promotionTask.PromotionDescription,promotionTask.TaskType,promotionTask.TaskStartTime,promotionTask.TaskEndTime,
                        //promotionTask.UseMoney,promotionTask.DiscountMoney,cellPhones==null ? 0 : cellPhones.Count))
                    };
                    command.Parameters.AddRange(sqlParameters);
                    DbHelper.ExecuteNonQuery(command);
                    return (int)command.Parameters["@PromotionTaskId_Output"].Value;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// 根据优惠券任务Id获得优惠券任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SearchPromotionByCondition GetPromotionTaskById(int id)
        {
            using (var command = new SqlCommand())
            {
                const string sql = "SELECT top 1 * FROM gungnir.dbo.tbl_PromotionTask WITH(NOLOCK) WHERE PromotionTaskId=@ID";
                var sqlParameter = new SqlParameter("@ID", id);
                command.CommandText = sql;
                command.Parameters.Add(sqlParameter);
                var promotionTask = DbHelper.ExecuteDataTable(command).ConvertTo<SearchPromotionByCondition>().FirstOrDefault();
                return promotionTask;
            }
        }

        /// <summary>
        /// 根据条件查询优惠券派送任务
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="taskNo">任务编号</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="taskStatus">任务状态</param>
        /// <param name="taskType">任务类型</param>
        /// <param name="promotionRuleId">优惠券类型编号</param>
        /// <param name="count">符合查询条件的数据数量</param>
        /// <returns></returns>
        public static List<SearchPromotionByCondition> SearchPromotionTaskByCondition(int pageIndex, int pageSize, int? promotionTaskId, string taskName,
            DateTime? createTime, int? taskStatus, int? taskType, int? promotionRuleId, out int count)
        {
            count = 0;
            DbParameter[] parameters =
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@PromotionTaskId", promotionTaskId),
                new SqlParameter("@TaskName", taskName),
                new SqlParameter("@CreateTime", createTime),
                new SqlParameter("@TaskStatus", taskStatus),
                new SqlParameter("@TaskType", taskType),
                new SqlParameter("@CouponRulesId", promotionRuleId),
                new SqlParameter("@Count", SqlDbType.Int) {Direction = ParameterDirection.Output}
            };
            var promotionList = DbHelper.ExecuteDataTable("Promotion_SearchPromotionByCondition", CommandType.StoredProcedure
                , parameters).ConvertTo<SearchPromotionByCondition>().ToList();
            var lastOrDefault = parameters.LastOrDefault();
            if (lastOrDefault != null)
            {
                int.TryParse(lastOrDefault.Value.ToString(), out count);
            }
            return promotionList;
        }

        /// <summary>
        /// 根据过滤条件获取筛选的用户
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DataTable FilterUserByCondition(PromotionTaskFilterInfo info)
        {
            DbParameter[] parameters =
            {
                new SqlParameter("@FilterStartTime", info.FilterStartTime),
                new SqlParameter("@FilterEndTime", info.FilterEndTime),
                new SqlParameter("@Brand", info.Brand),
                new SqlParameter("@Category", info.Category),
                new SqlParameter("@Pid", info.Pid),
                new SqlParameter("@SpendMoney", info.SpendMoney),
                new SqlParameter("@PurchaseNum", info.PurchaseNum),
                new SqlParameter("@Area", info.Area),
                new SqlParameter("@Channel", info.Channel),
                new SqlParameter("@InstallType", info.InstallType),
                new SqlParameter("@OrderStatus", info.OrderStatus),
                new SqlParameter("@Vehicle", info.Vehicle),
                new SqlParameter("@PromotionTaskId", null),
                new SqlParameter("@FilterOrderNo", null)
            };
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            using (var command = new SqlCommand())
            {
                command.CommandText = "Promotion_FilterUserByCondition";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);
                command.CommandTimeout = 900;
                return dbHelper.ExecuteDataTable(command);
            }
        }


        public static PromotionTask SelectPromotionTaskInfoByIdNew(int promotionTaskId)
        {
            #region sqlPromotionTask
            const string sqlPromotionTask = @"SELECT PromotionTaskId,
          TaskName,
          TaskType,
          TaskStartTime,
          TaskEndTime,
          ExecPeriod,
          TaskStatus,
          CreateTime,
          SelectUserType,
          FilterStartTime,
          FilterEndTime,
          Brand,
          Category,
          Pid,
          SpendMoney,
          PurchaseNum,
          Area,
          Channel,
          OrderType,
          InstallType,
          OrderStatus,
          Seable,
          Vehicle,
          UpdateTime,
          CouponRulesIds,
          IsLimitOnce,
          SmsId,
          SmsParam,
          PromotionTaskActivityId,
          ProductType
        FROM[dbo].[tbl_PromotionTask]  WITH(NOLOCK)
        WHERE PromotionTaskId = @PromotionTaskId";
            #endregion

            #region sqlTaskPromotionList
            const string sqlTaskPromotionList = @"SELECT TaskPromotionListId FROM dbo.tbl_PromotionTaskPromotionList  WITH(NOLOCK) WHERE PromotionTaskId =@PromotionTaskId";
            #endregion

            using (var cmd = new SqlCommand(sqlPromotionTask))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                var promotiontask = DbHelper.ExecuteDataTable(cmd).ConvertTo<PromotionTask>().SingleOrDefault();
                if (promotiontask != null)
                {
                    var taskPromotionListId = string.Empty;
                    cmd.CommandText = sqlTaskPromotionList;
                    var dt = DbHelper.ExecuteDataTable(cmd);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            taskPromotionListId = taskPromotionListId + "," + row[0].ToString();
                        }
                    }
                    promotiontask.PromotionListIds = taskPromotionListId.TrimStart(',');
                }
                return promotiontask;
            }
        }
        public static PromotionTaskCouponRule SelectPromotionTaskPromotionListByIdNew(int taskPromotionListId, int couponRulesId)
        {
            const string sqlPromotionTask = @"SELECT 
		PromotionTaskId ,
        CouponRulesId ,
		RuleName=(SELECT top 1 Name FROM Activity..tbl_CouponRules WITH(NOLOCK) WHERE  ParentID = 0  AND PKID=@CouponRulesId),
        PromotionDescription ,
        StartTime ,
        EndTime ,
        MinMoney ,
        DiscountMoney ,
        CreateTime ,
        UpdateTime,
		FinanceMarkName,
		[DepartmentName],
		[IntentionName],
        [BusinessLineName],
		Number
		Number,
        IsRemind,
        IsPush,
        PushSetting,
        CR.PromotionType
		FROM gungnir.dbo.tbl_PromotionTaskPromotionList  as ptpl WITH(NOLOCK)
        left join Activity..tbl_CouponRules AS CR WITH ( NOLOCK )  on ptpl.CouponRulesId = CR.PKID
		WHERE TaskPromotionListId = @TaskPromotionListId";
            using (var cmd = new SqlCommand(sqlPromotionTask))
            {
                cmd.Parameters.AddWithValue("@TaskPromotionListId", taskPromotionListId);
                cmd.Parameters.AddWithValue("@CouponRulesId", couponRulesId);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<PromotionTaskCouponRule>().SingleOrDefault();
            }
        }

        public static List<SelectPromotionCodeByUserCellPhonePager> SelectPromotionCodeByUserCellPhonePager(string userCellPhone, int? status, int pageIndex, int pageSize, out int count)
        {
            count = 0;
            DbParameter[] parameters =
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@UserCellPhone", userCellPhone),
                new SqlParameter("@Status", status),
                new SqlParameter("@Count", SqlDbType.Int) {Direction = ParameterDirection.Output}
            };
            var promotionList = DbHelper.ExecuteDataTable("Promotion_SelectPromotionCodeByUserCellPhonePager", CommandType.StoredProcedure
                , parameters).ConvertTo<SelectPromotionCodeByUserCellPhonePager>().ToList();
            var lastOrDefault = parameters.LastOrDefault();
            if (lastOrDefault != null)
            {
                int.TryParse(lastOrDefault.Value.ToString(), out count);
            }
            return promotionList;
        }

        /// <summary>
        /// 修改优惠券任务状态(审核、关闭)
        /// </summary>
        /// <param name="id">优惠券任务编号</param>
        /// <param name="taskStatus">任务状态</param>
        /// <returns></returns>
        public static int UpdatePromotionTaskStatus(int id, PromotionConsts.PromotionTaskStatusEnum taskStatus, string operateBy)
        {
            try
            {
                string sql = "";
                DbParameter[] parameters = null;
                if (taskStatus == PromotionConsts.PromotionTaskStatusEnum.Executed)
                {
                    sql = @"UPDATE  Gungnir..tbl_PromotionTask WITH ( ROWLOCK )
                SET     TaskStatus = @TaskStatus,AuditTime=GETDATE(),Auditor=@CreateBy
                WHERE   PromotionTaskId = @PromotionTaskId; ";
                    parameters = new DbParameter[]
                    {
                        new SqlParameter("@PromotionTaskId", id),
                        new SqlParameter("@TaskStatus", (int)taskStatus),
                        new SqlParameter("@CreateBy", operateBy)
                    };
                }
                else if (taskStatus == PromotionConsts.PromotionTaskStatusEnum.Closed)
                {
                    sql = @"UPDATE  dbo.tbl_PromotionTask WITH ( ROWLOCK )
                    SET     TaskStatus = @TaskStatus,CloseTime = GETDATE()
                    WHERE   PromotionTaskId = @PromotionTaskId;";
                    parameters = new DbParameter[]
                    {
                        new SqlParameter("@PromotionTaskId", id),
                        new SqlParameter("@TaskStatus", (int)taskStatus)
                    };
                }
                if (!string.IsNullOrEmpty(sql))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddRange(parameters);
                        return DbHelper.ExecuteNonQuery(cmd);
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static int UpdatePromotionTaskTaskStartTime(int id)
        {
            try
            {
                using (var cmd = new SqlCommand(@"UPDATE  Gungnir..tbl_PromotionTask WITH ( ROWLOCK )
                SET     TaskStartTime=GETDATE()
                WHERE   PromotionTaskId = @PromotionTaskId; "))
                {
                    DbParameter[] parameters =
                    {
                        new SqlParameter("@PromotionTaskId", id)
                    };
                    cmd.Parameters.AddRange(parameters);
                    return DbHelper.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static int ExecutePromotionTask(int id)
        {
            try
            {
                DbParameter[] parameters =
                {
                    new SqlParameter("@PromotionTaskId", id)
                };
                return DbHelper.ExecuteNonQuery("Gungnir..Promotion_SendPromotionToUserRepeatTaskJob",
                    CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DataTable GetTaskPromotionList(int promotionTaskId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandText = " SELECT * FROM Gungnir..tbl_PromotionTaskPromotionList WITH(NOLOCK) WHERE PromotionTaskId=@PromotionTaskId";
                command.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                return DbHelper.ExecuteDataTable(command);
            }
        }

        public static void DelPromotionTaskPromotionListById(int taskPromotionListId)
        {
            try
            {
                DbParameter[] parameters =
                {
                    new SqlParameter("@TaskPromotionListId", taskPromotionListId)
                };
                DbHelper.ExecuteNonQuery("Promotion_DelPromotionTaskPromotionListById", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 根据优惠券ID查询优惠券名称
        /// </summary>
        /// <param name="promotionRuleId"></param>
        /// <returns></returns>
        public static string GetPromotionRuleNameById(int promotionRuleId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandText = @"SELECT CR.Name FROM	
                                        Activity..tbl_CouponRules AS CR WITH ( NOLOCK ) 
                                        WHERE   CR.ParentID = 0  AND PKID=@PKID ";
                var sqlParameter = new SqlParameter("@PKID", promotionRuleId);
                command.Parameters.Add(sqlParameter);
                return (string)DbHelper.ExecuteScalar(command);
            }
        }
        public static List<Dictionary> GetAllOrderChannel()
        {
            string sql = "SELECT  DISTINCT ChannelType DicType,ChannelType DicKey,ChannelType DicValue FROM dbo.tbl_ChannelDictionaries WITH(NOLOCK) ";
            using (var command = new SqlCommand())
            {
                command.CommandText = sql;
                return DbHelper.ExecuteDataTable(command).ConvertTo<Dictionary>().ToList();
            }
        }

        public static List<Dictionary> GetOrderChannelChildren(string channelType)
        {
            string sql = @"SELECT   ChannelType DicType,ChannelKey DicKey,ChannelValue DicValue FROM 
            Gungnir.dbo.tbl_ChannelDictionaries WITH(NOLOCK) WHERE ChannelType = @ChannelType";
            using (var command = new SqlCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue("@ChannelType", channelType);
                return DbHelper.ExecuteDataTable(command).ConvertTo<Dictionary>().ToList();
            }
        }

        /// <summary>
        /// 获取产品所有的品牌
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllProductBrands()
        {
            using (var command = new SqlCommand())
            {
                command.CommandText = "SELECT PKID,BrandName FROM Tuhu_productcatalog..CarPAR_CatalogBrands WITH(NOLOCK)";
                return DbHelper.ExecuteDataTable(command);
            }
        }
        public static string GetEmailBody(SearchPromotionByCondition promotionTaskModel)
        {
            if (promotionTaskModel == null)
                return string.Empty;
            string ptList = @"SELECT   PromotionTaskId,CouponRulesId,PromotionDescription,StartTime,EndTime,MinMoney,DiscountMoney,CreateTime,UpdateTime,FinanceMarkName,
                                       Name=(select top 1 name from [Activity].[dbo].[tbl_CouponRules] with(nolock) where  PKID=CouponRulesId)
                              FROM gungnir.dbo.tbl_PromotionTaskPromotionList pt WITH(NOLOCK) 
                              WHERE PromotionTaskId=@PromotionTaskId";
            var cmd = new SqlCommand(ptList);
            cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskModel.PromotionTaskId);
            Func<DataTable, string> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    StringBuilder sbr = new StringBuilder(256);
                    sbr.AppendFormat($"任务编号：{promotionTaskModel.PromotionTaskId}；创建人：{promotionTaskModel.Creater}；审核人：{ThreadIdentity.Identifier?.Name}；优惠券信息↓：\r\n");
                    foreach (DataRow item in dt.Rows)
                    {
                        sbr.AppendFormat($"优惠券RuleId ={item.Field<int>("CouponRulesId")}，优惠券规则名称 ={item.Field<string>("Name")}，财务标识名称 ={item.Field<string>("FinanceMarkName")}，优惠券描述 ={item.Field<string>("PromotionDescription")}，使用开始时间 ={item.Field<DateTime?>("StartTime")}，使用结束时间 ={item.Field<DateTime?>("EndTime")}，使用金额 ={item.Field<object>("MinMoney").ToString()}，优惠金额 ={item.Field<object>("DiscountMoney").ToString()}\r\n");
                    }
                    return sbr.ToString();
                }
                return string.Empty;
            };
            return action(DbHelper.ExecuteDataTable(cmd));
        }
        public static DataTable GetDepartmentUseSetting()
        {
            var cmd = new SqlCommand(@"Select * from Configuration.[dbo].[CouponDepartmentUseSetting] with(nolock) where IsDel=0");
            return DbHelper.ExecuteDataTable(cmd);
        }

        public static DataTable GetAllBiActivity(IEnumerable<int> ContainIds = null)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                string sql =
                    @"SELECT PKID,ActivityName,Description,PromotionTaskId FROM Tuhu_bi..[tbl_PromotionTaskActivity] WITH(NOLOCK) WHERE (Status=0 AND PromotionTaskId IS NULL)";
                if (ContainIds != null && ContainIds.Any())
                {
                    sql += $" OR PromotionTaskId IN ({string.Join(",", ContainIds)})";
                }
                using (var cmd = new SqlCommand(sql))
                {

                    return dbHelper.ExecuteDataTable(cmd);
                }
            }

        }

        public static int SetPromotionTaskActivity(int promotionTaskActivityId, long promotionTaskId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    dbHelper.CreateDbCommand(
                        @"UPDATE Tuhu_bi..tbl_PromotionTaskActivity SET PromotionTaskId=@PromotionTaskId where PKID=@PKID AND Status=0 AND PromotionTaskId IS NULL")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }

        }

        public static int ResetPromotionTaskActivity(int promotionTaskActivityId, long promotionTaskId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    dbHelper.CreateDbCommand(
                        @"UPDATE Tuhu_bi..tbl_PromotionTaskActivity SET PromotionTaskId=NULL where PKID=@PKID AND PromotionTaskId=@PromotionTaskId AND Status=0")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static DataTable GetPromotionTaskActivity(int promotionTaskActivityId, long promotionTaskId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    dbHelper.CreateDbCommand(
                        @"SELECT PKID,ActivityName,Description,PromotionTaskId FROM Tuhu_bi..[tbl_PromotionTaskActivity] WITH(NOLOCK) WHERE PKID=@PKID AND PromotionTaskId=@PromotionTaskId")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    return dbHelper.ExecuteDataTable(cmd);
                }
            }
        }

        public static int GetActivityUsersCount(int promotionTaskActivityId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    dbHelper.CreateDbCommand(
                        @"SELECT COUNT(1) FROM Tuhu_bi..[tbl_PromotionTaskActivityUsers] WITH(NOLOCK) WHERE PromotionTaskActivityId=@PromotionTaskActivityId")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskActivityId", promotionTaskActivityId));
                    return (int)dbHelper.ExecuteScalar(cmd);
                }
            }
        }
        /// <summary>
        /// 少于200条并且立即发送的时候，需要查出来同步到临时发送的表里
        /// </summary>
        /// <param name="promotionTaskActivityId"></param>
        /// <param name="promotionTaskId"></param>
        /// <returns></returns>
        public static DataTable SelectAllActivityUsers(int promotionTaskActivityId, int promotionTaskId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    dbHelper.CreateDbCommand(
                        @"SELECT TOP 200 U.* FROM Tuhu_bi..tbl_PromotionTaskActivityUsers as U 
                        join Tuhu_bi..tbl_PromotionTaskActivity AS A ON U.PromotionTaskActivityId=A.PKID 
                        WHERE A.PKID=@PKID AND PromotionTaskId=@PromotionTaskId AND Status=0 ORDER BY U.PKID ASC")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    return dbHelper.ExecuteDataTable(cmd);
                }
            }
        }

        public static void MovePromotionTaskActivityUsers(int promotionTaskActivityId, int promotionTaskId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            DataTable data;
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    dbHelper.CreateDbCommand(
                        $@"SELECT U.UserTel AS UserCellPhone,{promotionTaskId} AS PromotionTaskId,GETDATE() AS CeateTime FROM Tuhu_bi..tbl_PromotionTaskActivityUsers as U 
                        join Tuhu_bi..tbl_PromotionTaskActivity AS A ON U.PromotionTaskActivityId=A.PKID 
                        WHERE A.PKID=@PKID AND PromotionTaskId=@PromotionTaskId AND Status=0 ORDER BY U.PKID ASC")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    data = dbHelper.ExecuteDataTable(cmd);
                }
            }

            if (data != null && data.Rows.Count > 0)
            {
                BulkCopy("tbl_PromotionSingleTaskUsers", data,
                    ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            }

        }

        public static DataTable SelectPromotionTaskHistoryUsers(
            int promotionTaskId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP 200 PromotionSingleTaskUsersHistoryId,PromotionTaskId,UserCellPhone,UserId FROM Gungnir..tbl_PromotionSingleTaskUsersHistory AS H WITH(NOLOCK) JOIN 
Tuhu_profiles..UserObject AS U ON H.UserCellPhone COLLATE Chinese_PRC_CI_AS =U.u_mobile_number COLLATE Chinese_PRC_CI_AS
 WHERE PromotionTaskId=@PromotionTaskId ORDER BY PromotionSingleTaskUsersHistoryId"
                ))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static bool ExistsPromotionTaskHistoryUsers(
            int promotionTaskId)
        {

            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP 1 PromotionSingleTaskUsersHistoryId FROM Gungnir..tbl_PromotionSingleTaskUsersHistory WITH(NOLOCK) WHERE PromotionTaskId=@PromotionTaskId ORDER BY PromotionSingleTaskUsersHistoryId ASC"
                ))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                var obj = DbHelper.ExecuteScalar(cmd);
                return obj != null;
            }
        }

        public static bool ExistsPromotionTaskUsers(
            int promotionTaskId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP 1 PromotionSingleTaskUsersId FROM Gungnir..tbl_PromotionSingleTaskUsers WITH(NOLOCK) WHERE PromotionTaskId=@PromotionTaskId ORDER BY PromotionSingleTaskUsersId ASC"
                ))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                var obj = DbHelper.ExecuteScalar(cmd);
                return obj != null;
            }
        }

        public static void MoveFilterOrderData(int promotionTaskId)
        {
            DataTable data;
            using (var cmd = new SqlCommand("Gungnir.dbo.Promotion_FilterUserByCondition") { CommandTimeout = 20 * 60 })
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FilterStartTime", null);
                cmd.Parameters.AddWithValue("@FilterEndTime", null);
                cmd.Parameters.AddWithValue("@Brand", null);
                cmd.Parameters.AddWithValue("@Category", null);
                cmd.Parameters.AddWithValue("@Pid", null);
                cmd.Parameters.AddWithValue("@SpendMoney", 0);
                cmd.Parameters.AddWithValue("@PurchaseNum", 0);
                cmd.Parameters.AddWithValue("@Area", null);
                cmd.Parameters.AddWithValue("@Channel", null);
                cmd.Parameters.AddWithValue("@InstallType", null);
                cmd.Parameters.AddWithValue("@OrderStatus", 0);
                cmd.Parameters.AddWithValue("@Vehicle", null);
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                cmd.Parameters.AddWithValue("@FilterOrderNo", null);
                data = DbHelper.ExecuteDataTable(cmd);
            }
            if (data != null && data.Rows.Count > 0)
            {
                data.Columns.Add(new DataColumn("PromotionTaskId", typeof(int)) { DefaultValue = promotionTaskId });
                data.Columns["MobileNum"].ColumnName = "UserCellPhone";
                data.Columns.Remove("UserID");
                data.Columns.Add(new DataColumn("CeateTime", typeof(DateTime)) { DefaultValue = DateTime.Now });
                BulkCopy("tbl_PromotionSingleTaskUsers", data,
                    ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            }
        }
        public static void BulkCopy(string toTableName, DataTable dt, string connStr)
        {
            var sqlConn = new SqlConnection(connStr);

            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn) { BulkCopyTimeout = 20 * 60 };
            bulkCopy.DestinationTableName = toTableName;
            foreach (DataColumn c in dt.Columns)
            {
                bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
            }
            bulkCopy.BatchSize = dt.Rows.Count;
            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
        }
    }
}
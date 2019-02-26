using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.SqlServer.Server;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Extensions;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALPromotion
    {
        private static readonly String Configuration_Readonly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        public static int InsertPromotionCode(SqlDbHelper dbhelper, PromotionCode PC)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[PromotionCode_CreatePromotionCode]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", PC.UserID);
                cmd.Parameters.AddWithValue("@StartTime", PC.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", PC.EndTime);
                cmd.Parameters.AddWithValue("@OrderId", PC.OrderID);
                cmd.Parameters.AddWithValue("@Status", PC.Status);
                cmd.Parameters.AddWithValue("@Type", PC.Type);
                cmd.Parameters.AddWithValue("@Description", PC.Description);
                cmd.Parameters.AddWithValue("@Discount", PC.Discount);
                cmd.Parameters.AddWithValue("@MinMoney", PC.MinMoney);
                cmd.Parameters.AddWithValue("@RuleID", PC.RuleID);
                cmd.Parameters.AddWithValue("@Code", PC.Code);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int CreatePromotionCodeNew(PromotionCode model)
        {
            using (var cmd = new SqlCommand(@"INSERT	INTO Gungnir..tbl_PromotionCode
            (
                Code,
                UserId,
                StartTime,
                EndTime,
                CreateTime,
                Status,
                Description,
                Discount,
                MinMoney,
                CodeChannel,
                RuleID,
                PromtionName,
                BatchId,
                Issuer,
                IssueChannle,
                IssueChannleId,
				Creater,
				DepartmentName,
				IntentionName
            )VALUES(
            RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12),
            @UserID,
            @StartDate,
            @EndDate,
            GETDATE(),
            0,
            @Description,
            @Discount,
            @Minmoney,
            @Channel,
            @RuleID,
            @PromtionName,
            @BatchId,
            @Issuer,
            @IssueChannle,
            @IssueChannleId,
			@Creater,
			@DepartmentName,
			@IntentionName);
            SELECT @@IDENTITY; "))
            {
                cmd.Parameters.AddWithValue("@UserID", model.UserID);
                cmd.Parameters.AddWithValue("@StartDate", model.StartTime);
                cmd.Parameters.AddWithValue("@EndDate", model.EndTime);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Discount", model.Discount);
                cmd.Parameters.AddWithValue("@Minmoney", model.MinMoney);
                cmd.Parameters.AddWithValue("@Channel", model.CodeChannel);
                cmd.Parameters.AddWithValue("@RuleID", model.RuleID);
                cmd.Parameters.AddWithValue("@PromtionName", model.PromotionName);
                cmd.Parameters.AddWithValue("@BatchId", model.BatchId);
                cmd.Parameters.AddWithValue("@Issuer", model.Issuer);
                cmd.Parameters.AddWithValue("@IssueChannle", model.IssueChannle);
                cmd.Parameters.AddWithValue("@IssueChannleId", model.IssueChannleId);
                cmd.Parameters.AddWithValue("@Creater", model.Creater);
                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                cmd.Parameters.AddWithValue("@IntentionName", model.IntentionName);
                var obj = DbHelper.ExecuteScalar(cmd);
                if (obj != null) return int.Parse(obj.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 获取所有类型的优惠券来进行展示
        /// </summary>
        /// <returns></returns>
        public static ListModel<PromotionModel> SelectAllPromotion(PromotionFilterConditionModel model)
        {
            var resultModel = new ListModel<PromotionModel>();
            string sql = @"SELECT *
                FROM    Activity..tbl_CouponRules AS CR WITH(NOLOCK)
            WHERE ParentID = 0 ";
            var whereSql = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.Remark))
            {
                whereSql.Append(" AND CR.Name LIKE '%' + @Remark + '%' ");
            }
            if (model.RuleID != null)
            {
                whereSql.Append(" AND PKID=@RuleID ");
            }
            if (model.OrderType != null)
            {
                whereSql.Append(" AND CR.InstallType = @OrderType ");
            }
            if (model.PromotionType != null)
            {
                whereSql.Append(" AND CR.PromotionType = @PromotionType ");
            }
            var getRuleWhereSql = new StringBuilder();
            if (model.GetRuleID != null)
            {
                getRuleWhereSql.Append(" AND GCR.PKID=@GetRuleID ");
            }
            if (model.GetRuleGUID != null)
            {
                getRuleWhereSql.Append(" AND GCR.GetRuleGUID = @GetRuleGUID ");
            }
            if (!string.IsNullOrWhiteSpace(model.PromotionName))
            {
                getRuleWhereSql.Append(" AND GCR.PromtionName LIKE '%' + @PromotionName + '%' ");
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                getRuleWhereSql.Append(" AND GCR.Description LIKE '%' + @Description + '%' ");
            }
            if (model.Minmoney_Min != null)
            {
                getRuleWhereSql.Append(" AND GCR.Minmoney >= @Minmoney_Min ");
            }
            if (model.Minmoney_Max != null)
            {
                getRuleWhereSql.Append(" AND GCR.Minmoney <= @Minmoney_Max ");
            }
            if (model.Discount_Min != null)
            {
                getRuleWhereSql.Append(" AND GCR.Discount >= @Discount_Min ");
            }
            if (model.Discount_Max != null)
            {
                getRuleWhereSql.Append(" AND GCR.Discount <= @Discount_Max ");
            }
            if (model.AllowChanel != null)
            {
                getRuleWhereSql.Append(" AND GCR.AllowChanel = @AllowChanel ");
            }
            if (model.SupportUserRange != null)
            {
                getRuleWhereSql.Append(" AND GCR.SupportUserRange = @SupportUserRange ");
            }

            if (getRuleWhereSql.Length > 0)
            {
                whereSql.Append($@" AND CR.PKID IN(
                SELECT GCR.RuleID
                    FROM      Activity.dbo.tbl_GetCouponRules AS GCR WITH(NOLOCK)
                WHERE 1=1 {getRuleWhereSql}) ");
            }

            string countSql = @"SELECT COUNT(1) FROM Activity..tbl_CouponRules AS CR WITH(NOLOCK) WHERE ParentID = 0 ";

            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                var cmd = new SqlCommand($@"{sql} {whereSql} ORDER BY CR.PKID ASC
                OFFSET(@PageSize * (@PageIndex - 1)) ROWS
                    FETCH NEXT @PageSize ROWS ONLY; ");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Remark", model.Remark);
                cmd.Parameters.AddWithValue("@RuleID", model.RuleID);
                cmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                cmd.Parameters.AddWithValue("@PromotionType", model.PromotionType);
                cmd.Parameters.AddWithValue("@GetRuleID", model.GetRuleID);
                cmd.Parameters.AddWithValue("@GetRuleGUID", model.GetRuleGUID);
                cmd.Parameters.AddWithValue("@PromotionName", model.PromotionName);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Minmoney_Min", model.Minmoney_Min);
                cmd.Parameters.AddWithValue("@Minmoney_Max", model.Minmoney_Max);
                cmd.Parameters.AddWithValue("@Discount_Min", model.Discount_Min);
                cmd.Parameters.AddWithValue("@Discount_Max", model.Discount_Max);
                cmd.Parameters.AddWithValue("@AllowChanel", model.AllowChanel);
                cmd.Parameters.AddWithValue("@SupportUserRange", model.SupportUserRange);
                cmd.Parameters.AddWithValue("@PageIndex", model.PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", model.PageSize);
                resultModel.Source = dbhelper.ExecuteDataTable(cmd).ConvertTo<PromotionModel>();

                cmd.CommandText = $"{countSql} {whereSql}";
                resultModel.Pager = new PagerModel
                {
                    CurrentPage = model.PageIndex,
                    PageSize = model.PageSize,
                    TotalItem = (int)dbhelper.ExecuteScalar(cmd)
                };
            }
            return resultModel;
        }
        /// <summary>
        /// 获取某种类型的优惠券来的详细信息
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectPromotionDetail(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"  SELECT	CR.*,
                                           			VS.CarparName AS ShopName
                                             FROM		Activity..tbl_CouponRules AS CR WITH ( NOLOCK )
                                             LEFT JOIN Gungnir.dbo.vw_Shop AS VS WITH ( NOLOCK )
                                           			ON CR.ShopID = VS.PKID
                                             WHERE		CR.ParentID = @ParentID;");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ParentID", id);
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        public static int SaveGetPCodeRule(GetPCodeModel model)
        {

            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand("INSERT INTO Activity..tbl_GetCouponRules (RuleID,[Description],PromtionName,Discount,Minmoney,AllowChanel,Term,ValiStartDate,ValiEndDate,Quantity,RemindQuantity,RemindEmails,GetQuantity,CreateDateTime,LastDateTime,SingleQuantity,DXStartDate,DXEndDate,SupportUserRange,DetailShowStartDate,DetailShowEndDate,Channel,DepartmentId,IntentionId,BusinessLineId,DepartmentName,IntentionName,BusinessLineName,Creater,DeadLineDate,IsPush,PushSetting) VALUES (@RuleID,@Description,@PromtionName,@Discount,@Minmoney,@AllowChanel,@Term,@ValiStartDate,@ValiEndDate,@Quantity,@RemindQuantity,@RemindEmails,0, GETDATE(), GETDATE(), @SingleQuantity,@DXStartDate,@DXEndDate,@SupportUserRange,@DetailShowStartDate,@DetailShowEndDate,@Channel,@DepartmentId,@IntentionId,@BusinessLineId,@DepartmentName,@IntentionName,@BusinessLineName,@Creater,@DeadLineDate,@IsPush,@PushSetting)   SELECT ISNULL(@@IDENTITY,0)");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@RuleID", model.RuleID);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@PromtionName", model.PromtionName);
                cmd.Parameters.AddWithValue("@Discount", model.Discount);
                cmd.Parameters.AddWithValue("@Minmoney", model.Minmoney);
                cmd.Parameters.AddWithValue("@AllowChanel", model.AllowChanel);
                cmd.Parameters.AddWithValue("@Term", model.Term);
                cmd.Parameters.AddWithValue("@DXStartDate", model.DXStartDate);
                cmd.Parameters.AddWithValue("@DXEndDate", model.DXEndDate);
                cmd.Parameters.AddWithValue("@ValiStartDate", model.ValiStartDate);
                cmd.Parameters.AddWithValue("@ValiEndDate", model.ValiEndDate);
                cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@RemindQuantity", model.RemindQuantity);
                cmd.Parameters.AddWithValue("@RemindEmails", model.RemindEmails);
                cmd.Parameters.AddWithValue("@SingleQuantity", model.SingleQuantity);
                cmd.Parameters.AddWithValue("@SupportUserRange", model.SupportUserRange);
                cmd.Parameters.AddWithValue("@DetailShowStartDate", model.DetailShowStartDate);
                cmd.Parameters.AddWithValue("@DetailShowEndDate", model.DetailShowEndDate);
                cmd.Parameters.AddWithValue("@Channel", model.Channel);
                cmd.Parameters.AddWithValue("@IntentionId", model.IntentionId);
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@BusinessLineId", model.BusinessLineId);
                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                cmd.Parameters.AddWithValue("@IntentionName", model.IntentionName);
                cmd.Parameters.AddWithValue("@BusinessLineName", model.BusinessLineName);
                cmd.Parameters.AddWithValue("@Creater", model.Creater);
                cmd.Parameters.AddWithValue("@DeadLineDate", model.DeadLineDate);
                cmd.Parameters.AddWithValue("@IsPush", model.IsPush);
                cmd.Parameters.AddWithValue("@PushSetting", model.PushSetting);
                return Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
            }
        }
        public static int UpdateGetPCodeRule(GetPCodeModel model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(
                    "UPDATE Activity..tbl_GetCouponRules WITH(ROWLOCK) SET [Description]=@Description,PromtionName=@PromtionName,Discount=@Discount,Minmoney=@Minmoney,AllowChanel=@AllowChanel,Term=@Term,ValiStartDate=@ValiStartDate,ValiEndDate=@ValiEndDate,Quantity=@Quantity,RemindQuantity=@RemindQuantity,RemindEmails=@RemindEmails,LastDateTime=GETDATE(),SingleQuantity=@SingleQuantity,DXStartDate=@DXStartDate,DXEndDate=@DXEndDate,SupportUserRange=@SupportUserRange,DetailShowStartDate=@DetailShowStartDate,DetailShowEndDate=@DetailShowEndDate,Channel=@Channel,DepartmentId=@DepartmentId,IntentionId=@IntentionId,BusinessLineId=@BusinessLineId,DepartmentName=@DepartmentName,IntentionName=@IntentionName,BusinessLineName=@BusinessLineName,DeadLineDate=@DeadLineDate,IsPush=@IsPush,PushSetting=@PushSetting  WHERE PKID=@PKID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", model.GETPKID);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@PromtionName", model.PromtionName);
                cmd.Parameters.AddWithValue("@Discount", model.Discount);
                cmd.Parameters.AddWithValue("@Minmoney", model.Minmoney);
                cmd.Parameters.AddWithValue("@AllowChanel", model.AllowChanel);
                cmd.Parameters.AddWithValue("@Term", model.Term);
                cmd.Parameters.AddWithValue("@DXStartDate", model.DXStartDate);
                cmd.Parameters.AddWithValue("@DXEndDate", model.DXEndDate);
                cmd.Parameters.AddWithValue("@ValiStartDate", model.ValiStartDate);
                cmd.Parameters.AddWithValue("@ValiEndDate", model.ValiEndDate);
                cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@RemindQuantity", model.RemindQuantity);
                cmd.Parameters.AddWithValue("@RemindEmails", model.RemindEmails);
                cmd.Parameters.AddWithValue("@SingleQuantity", model.SingleQuantity);
                cmd.Parameters.AddWithValue("@SupportUserRange", model.SupportUserRange);
                cmd.Parameters.AddWithValue("@DetailShowStartDate", model.DetailShowStartDate);
                cmd.Parameters.AddWithValue("@DetailShowEndDate", model.DetailShowEndDate);
                cmd.Parameters.AddWithValue("@Channel", model.Channel);
                cmd.Parameters.AddWithValue("@IntentionId", model.IntentionId);
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@BusinessLineId", model.BusinessLineId);
                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                cmd.Parameters.AddWithValue("@IntentionName", model.IntentionName);
                cmd.Parameters.AddWithValue("@BusinessLineName", model.BusinessLineName);
                cmd.Parameters.AddWithValue("@DeadLineDate", model.DeadLineDate);
                cmd.Parameters.AddWithValue("@IsPush", model.IsPush);
                cmd.Parameters.AddWithValue("@PushSetting", model.PushSetting);
                var result = DbHelper.ExecuteNonQuery(cmd);
                return result > 0 ? model.GETPKID : result;
            }
        }

        /// <summary>
        /// 获取某一个类型的优惠券来进行展示
        /// </summary>
        /// <returns></returns>
        public static DataRow FetchPromotionByPKID(string id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("SELECT	* FROM Activity..tbl_CouponRules AS CR WITH ( NOLOCK ) WHERE PKID=@PKID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", Convert.ToInt32(id));
                return dbhelper.ExecuteDataRow(cmd);
            }

        }
        /// <summary>
        /// 获取产品的所有的分类
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectProductCategory()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("SELECT  oid, ParaentOid, ParentOid,CategoryName,DisplayName, [Description],NodeNo FROM Tuhu_productcatalog..vw_ProductCategories WITH(NOLOCK)");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        /// <summary>
        /// 获取产品的所有的分类名字和展示名字
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectProductCategoryCategoryNameAndDisplayName()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(
@"SELECT  c.CategoryName ,
        c.DisplayName ,
        c.ParaentOid ,
        c.Oid
FROM    Tuhu_productcatalog..vw_ProductCategories AS c WITH ( NOLOCK );");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        /// <summary>
        /// 获取产品品牌
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectProductBrand(string type)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("[Tuhu_productcatalog]..[Promotion_SelectProductBrand]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);
                return dbhelper.ExecuteDataTable(cmd);
            }

        }
        public static int UpdateParent(SqlDbHelper dbhelper, PromotionModel model, int PKID)
        {
            using (var cmd = new SqlCommand("Update Activity..tbl_CouponRules set Name=@Name,IOSKey=@IOSKey,IOSValue=@IOSValue,androidKey=@androidKey,androidValue=@androidValue,HrefType=@HrefType,CustomSkipPage=@CustomSkipPage,WxSkipPage=@WxSkipPage,H5SkipPage=@H5SkipPage,PromotionType=@PromotionType,InstallType=@InstallType,IsActive=@IsActive,PIDType=@PIDType where ParentID=0 and PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@InstallType", model.InstallType);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@IOSKey", model.GetKeyValue(true, PKID).FirstOrDefault().Key);
                cmd.Parameters.AddWithValue("@IOSValue", model.GetKeyValue(true, PKID).FirstOrDefault().Value);
                cmd.Parameters.AddWithValue("@androidKey", model.GetKeyValue(false, PKID).FirstOrDefault().Key);
                cmd.Parameters.AddWithValue("@androidValue", model.GetKeyValue(false, PKID).FirstOrDefault().Value);
                cmd.Parameters.AddWithValue("@HrefType", model.HrefType);
                cmd.Parameters.AddWithValue("@CustomSkipPage", model.CustomSkipPage);
                cmd.Parameters.AddWithValue("@WxSkipPage", model.WxSkipPage);
                cmd.Parameters.AddWithValue("@H5SkipPage", model.H5SkipPage);
                cmd.Parameters.AddWithValue("@PromotionType", model.PromotionType);
                cmd.Parameters.AddWithValue("@PIDType", model.PIDType);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        public static int DeleteRecord(SqlDbHelper dbhelper, string type, int PKID)
        {
            using (var cmd = new SqlCommand("Delete from  Activity..tbl_CouponRules  where (@type=N'oneChild'and PKID=@PKID)OR @type=N'allChild' and ParentID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 记录优惠券
        /// </summary>
        /// <param name="model"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int SavePromotionInfo(SqlDbHelper dbhelper, PromotionModel model, string action, int? shopType, int? shopId)
        {
            if (string.IsNullOrWhiteSpace(action))
                return -1;
            if (String.Compare(action, "add", false) == 0 || String.Compare(action, "update", false) == 0)
            {
                using (var cmd = new SqlCommand(@"IF EXISTS ( SELECT	1
	                                            			  FROM		Activity.dbo.tbl_CouponRules
	                                            			  WHERE		PKID = @ParentID )
	                                              BEGIN 
	                                            		INSERT	INTO Activity.dbo.tbl_CouponRules
	                                            				(
	                                            				  Name,
	                                            				  Category,
	                                            				  ProductID,
	                                            				  Brand,
	                                            				  ParentID,
	                                            				  IsActive,
	                                            				  CreateDateTime,
	                                            				  LastDateTime,
	                                            				  InstallType,
	                                            				  IOSKey,
	                                            				  IOSValue,
	                                            				  androidKey,
	                                            				  androidValue,
	                                            				  HrefType,
                                                                  CustomSkipPage,
                                                                  WxSkipPage,
                                                                  H5SkipPage,
	                                            				  PIDType,
	                                            				  PromotionType,
                                                                  ShopType,
                                                                  ShopID)
	                                            		VALUES	(
	                                            				  @Name,
	                                            				  @Category,
	                                            				  @ProductID,
	                                            				  @Brand,
	                                            				  @ParentID,
	                                            				  1,
	                                            				  GETDATE(),
	                                            				  GETDATE(),
	                                            				  @InstallType,
	                                            				  @IOSKey,
	                                            				  @IOSValue,
	                                            				  @androidKey,
	                                            				  @androidValue,
	                                            				  @HrefType,
                                                                  @CustomSkipPage,
                                                                  @WxSkipPage,
                                                                  @H5SkipPage,
	                                            				  @PIDType,
	                                            				  @PromotionType,
                                                                  @ShopType,
                                                                  @ShopID)
	                                              END; "))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Brand", model.Brand);
                    cmd.Parameters.AddWithValue("@Category", model.Category);
                    cmd.Parameters.AddWithValue("@ProductID", model.ProductID);
                    //cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@PIDType", model.PIDType);
                    cmd.Parameters.AddWithValue("@InstallType", model.InstallType);
                    cmd.Parameters.AddWithValue("@ParentID", model.ParentID);
                    cmd.Parameters.AddWithValue("@IOSKey", model.GetKeyValue(true, model.ParentID).FirstOrDefault().Key);
                    cmd.Parameters.AddWithValue("@IOSValue", model.GetKeyValue(true, model.ParentID).FirstOrDefault().Value);
                    cmd.Parameters.AddWithValue("@androidKey", model.GetKeyValue(false, model.ParentID).FirstOrDefault().Key);
                    cmd.Parameters.AddWithValue("@androidValue", model.GetKeyValue(false, model.ParentID).FirstOrDefault().Value);
                    cmd.Parameters.AddWithValue("@HrefType", model.HrefType);
                    cmd.Parameters.AddWithValue("@CustomSkipPage", model.CustomSkipPage);
                    cmd.Parameters.AddWithValue("@WxSkipPage", model.WxSkipPage);
                    cmd.Parameters.AddWithValue("@H5SkipPage", model.H5SkipPage);
                    cmd.Parameters.AddWithValue("@PromotionType", model.PromotionType);
                    cmd.Parameters.AddWithValue("@ShopType", shopType);
                    cmd.Parameters.AddWithValue("@ShopID", shopId);
                    return dbhelper.ExecuteNonQuery(cmd);
                }
            }
            else
                return -1;
        }

        public static void isSuccessForCreateParent(SqlDbHelper dbhelper, PromotionModel model, out int result)
        {
            using (var cmd = new SqlCommand(@" INSERT	INTO Activity..tbl_CouponRules
                                               		(
                                               		  Name,
                                               		  Category,
                                               		  ProductID,
                                               		  Brand,
                                               		  ParentID,
                                               		  IsActive,
                                               		  CreateDateTime,
                                               		  LastDateTime,
                                               		  InstallType,
                                               		  IOSKey,
                                               		  IOSValue,
                                               		  androidKey,
                                               		  androidValue,
                                               		  HrefType,
                                                      CustomSkipPage,
                                                      WxSkipPage,
                                                      H5SkipPage,
                                                      PIDType,
                                               		  PromotionType )
                                                VALUES	(
                                               		  @Name,
                                               		  NULL,
                                               		  NULL,
                                               		  NULL,
                                               		  0,
                                               		  @IsActive,
                                               		  GETDATE(),
                                               		  GETDATE(),
                                               		  @InstallType,
                                               		  @IOSKey,
                                               		  @IOSValue,
                                               		  @androidKey,
                                               		  @androidValue,
                                               		  @HrefType,
                                                      @CustomSkipPage,
                                                      @WxSkipPage,
                                                      @H5SkipPage,
                                                      @PIDType,
                                               		  @PromotionType );
                                                SELECT	ISNULL(@@IDENTITY,0)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@InstallType", model.InstallType);
                cmd.Parameters.AddWithValue("@IOSKey", model.GetKeyValue(true, 0).FirstOrDefault().Key);
                cmd.Parameters.AddWithValue("@IOSValue", model.GetKeyValue(true, 0).FirstOrDefault().Value);
                cmd.Parameters.AddWithValue("@androidKey", model.GetKeyValue(false, 0).FirstOrDefault().Key);
                cmd.Parameters.AddWithValue("@androidValue", model.GetKeyValue(false, 0).FirstOrDefault().Value);
                cmd.Parameters.AddWithValue("@HrefType", model.HrefType);
                cmd.Parameters.AddWithValue("@CustomSkipPage", model.CustomSkipPage);
                cmd.Parameters.AddWithValue("@WxSkipPage", model.WxSkipPage);
                cmd.Parameters.AddWithValue("@H5SkipPage", model.H5SkipPage);
                cmd.Parameters.AddWithValue("@PromotionType", model.PromotionType);
                cmd.Parameters.AddWithValue("@PIDType", model.PIDType);

                result = Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
                if (result > 0)
                    UpdateIOSValueAndroidValue(dbhelper, result);
            }
        }

        public static void UpdateIOSValueAndroidValue(SqlDbHelper dbHelper, int pkid)
        {
            dbHelper.ExecuteNonQuery(@" UPDATE	Activity..tbl_CouponRules
                                        SET	IOSValue = REPLACE(IOSValue, 0, @PKID),
                                       		androidValue = REPLACE(androidValue, 0, @PKID)
                                        WHERE	PKID = @PKID", CommandType.Text, new SqlParameter("@PKID", pkid));
        }
        public static ListModel<GetPCodeModel> SelectGeCouponRulesByCondition(int id, PromotionFilterConditionModel model)
        {
            string sql = @"
            SELECT  CR.PKID ,
                    CR.Name ,
                    CR.InstallType ,
                    GCR.PKID AS GETPKID ,
                    GCR.GetRuleGUID ,
                    GCR.RuleID ,
                    GCR.SingleQuantity ,
                    GCR.PromtionName ,
                    GCR.Description ,
                    GCR.Discount ,
                    GCR.Minmoney ,
                    GCR.Term ,
                    GCR.Channel ,
                    GCR.ValiStartDate ,
                    GCR.ValiEndDate ,
                    GCR.AllowChanel ,
                    GCR.Quantity ,
                    GCR.GetQuantity ,
                    GCR.IsPush ,
                    GCR.PushSetting ,
                    DXStartDate ,
                    DXEndDate ,
                    SupportUserRange ,
                    DetailShowStartDate ,
                    DetailShowEndDate,
		            CR.PromotionType
            FROM    Activity..tbl_CouponRules AS CR WITH ( NOLOCK )
                    LEFT JOIN Activity..tbl_GetCouponRules AS GCR WITH ( NOLOCK ) ON CR.PKID = GCR.RuleID
            WHERE   CR.PKID = @PKID ";

            string countSql = @"SELECT  COUNT(1)  FROM    Activity..tbl_CouponRules AS CR WITH(NOLOCK)
            LEFT JOIN Activity..tbl_GetCouponRules AS GCR WITH(NOLOCK) ON CR.PKID = GCR.RuleID
            WHERE CR.PKID = @PKID ";

            var whereSql = new StringBuilder();
            if (model.GetRuleID != null)
            {
                whereSql.Append(" AND GCR.PKID = @GetRuleID ");
            }
            if (model.GetRuleGUID != null)
            {
                whereSql.Append(" AND GCR.GetRuleGUID = @GetRuleGUID ");
            }
            if (!string.IsNullOrWhiteSpace(model.PromotionName))
            {
                whereSql.Append(" AND GCR.PromtionName LIKE '%' + @PromotionName + '%' ");
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                whereSql.Append(" AND GCR.Description LIKE '%' + @Description + '%' ");
            }
            if (model.Minmoney_Min != null)
            {
                whereSql.Append(" AND GCR.Minmoney >= @Minmoney_Min ");
            }
            if (model.Minmoney_Max != null)
            {
                whereSql.Append(" AND GCR.Minmoney <= @Minmoney_Max ");
            }
            if (model.Discount_Min != null)
            {
                whereSql.Append(" AND GCR.Discount >= @Discount_Min ");
            }
            if (model.Discount_Max != null)
            {
                whereSql.Append(" AND GCR.Discount <= @Discount_Max ");
            }
            if (model.AllowChanel != null)
            {
                whereSql.Append(" AND GCR.AllowChanel = @AllowChanel ");
            }
            if (model.SupportUserRange != null)
            {
                whereSql.Append(" AND GCR.SupportUserRange = @SupportUserRange ");
            }
            if (model.DepartmentId != null)
            {
                whereSql.Append(" AND  GCR.DepartmentId = @DepartmentId ");
            }
            if (model.IntentionId != null)
            {
                whereSql.Append(" AND  GCR.IntentionId = @IntentionId ");
            }
            var resultModel = new ListModel<GetPCodeModel>();
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                var cmd = new SqlCommand($@"{sql} {whereSql} ORDER BY GCR.PKID DESC 
                                        OFFSET(@PageSize * (@PageIndex - 1)) ROWS
                                        FETCH NEXT @PageSize ROWS ONLY;");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", id);
                cmd.Parameters.AddWithValue("@GetRuleID", model.GetRuleID);
                cmd.Parameters.AddWithValue("@GetRuleGUID", model.GetRuleGUID);
                cmd.Parameters.AddWithValue("@PromotionName", model.PromotionName);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Minmoney_Min", model.Minmoney_Min);
                cmd.Parameters.AddWithValue("@Minmoney_Max", model.Minmoney_Max);
                cmd.Parameters.AddWithValue("@Discount_Min", model.Discount_Min);
                cmd.Parameters.AddWithValue("@Discount_Max", model.Discount_Max);
                cmd.Parameters.AddWithValue("@AllowChanel", model.AllowChanel);
                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@IntentionId", model.IntentionId);
                cmd.Parameters.AddWithValue("@SupportUserRange", model.SupportUserRange);
                cmd.Parameters.AddWithValue("@PageIndex", model.PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", model.PageSize);
                resultModel.Source = dbhelper.ExecuteDataTable(cmd).ConvertTo<GetPCodeModel>();

                cmd.CommandText = $@"{countSql} {whereSql}";
                resultModel.Pager = new PagerModel
                {
                    CurrentPage = model.PageIndex,
                    PageSize = model.PageSize,
                    TotalItem = (int)dbhelper.ExecuteScalar(cmd)
                };
            }
            return resultModel;
        }
        public static DataTable SelectGeCouponRulesByRuleID(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"
                    SELECT  CR.PKID,
                            CR.Name,
                            CR.InstallType,
                            CR.RuleDescription,
                            GCR.PKID AS GETPKID,
                            GCR.GetRuleGUID,
                            GCR.RuleID,
                            GCR.SingleQuantity,
                            GCR.PromtionName,
                            GCR.Description,
                            GCR.Discount,
                            GCR.Minmoney,
                            GCR.Term,
                            GCR.Channel,
                            GCR.ValiStartDate,
                            GCR.ValiEndDate,
                            GCR.AllowChanel,
                            GCR.Quantity,
                            GCR.RemindQuantity,
                            GCR.RemindEmails,
                            GCR.GetQuantity,
                            GCR.DepartmentId,
                            GCR.IntentionId,
                            GCR.BusinessLineId,
                            DXStartDate,
                            DXEndDate,
                            SupportUserRange,
                            DetailShowStartDate,
                            DetailShowEndDate,
                            DeadLineDate,
                            IsPush,
                            PushSetting,
                            CR.PromotionType
                    FROM    Activity..tbl_CouponRules AS CR WITH(NOLOCK)
                            LEFT JOIN Activity..tbl_GetCouponRules AS GCR WITH(NOLOCK) ON CR.PKID = GCR.RuleID
                    WHERE   CR.PKID = @PKID;");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", id);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
        /// <summary>
        /// 获取优惠券领取规则
        /// </summary>
        /// <param name="getRuleId"></param>
        /// <returns></returns>
        public static DataTable SelectGeCouponRulesByGetRuleID(int getRuleId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"
                    SELECT  CR.PKID,
                            CR.Name,
                            CR.InstallType,
                            CR.RuleDescription,
                            GCR.PKID AS GETPKID,
                            GCR.GetRuleGUID,
                            GCR.RuleID,
                            GCR.SingleQuantity,
                            GCR.PromtionName,
                            GCR.Description,
                            GCR.Discount,
                            GCR.Minmoney,
                            GCR.Term,
                            GCR.Channel,
                            GCR.ValiStartDate,
                            GCR.ValiEndDate,
                            GCR.AllowChanel,
                            GCR.Quantity,
                            GCR.RemindQuantity,
                            GCR.RemindEmails,
                            GCR.GetQuantity,
                            GCR.DepartmentId,
                            GCR.IntentionId,
                            GCR.BusinessLineId,
                            DXStartDate,
                            DXEndDate,
                            SupportUserRange,
                            DetailShowStartDate,
                            DetailShowEndDate,
                            DeadLineDate,
                            IsPush,
                            PushSetting,
                            CR.PromotionType
                    FROM    Activity..tbl_CouponRules AS CR WITH(NOLOCK)
                            LEFT JOIN Activity..tbl_GetCouponRules AS GCR WITH(NOLOCK) ON CR.PKID = GCR.RuleID
                    WHERE   GCR.PKID = @PKID;");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", getRuleId);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
        public static Dictionary<string, string> GetKeyValue(this PromotionModel model, bool isIOS, int ruleId)
        {
            //0：优惠券说明页
            //1：轮胎列表
            //2：保养列表
            //3：车品商城
            //4：商品列表
            //5：洗车
            //6：打蜡
            //7：内饰清洗
            var dic = new Dictionary<string, string>();
            if (isIOS)
            {
                switch (model.HrefType.ToString())
                {
                    case "0": dic.Add("TNCouponHelpVC", null); break;
                    case "1": dic.Add("TNTireListViewController", null); break;
                    case "2": dic.Add("TNMaintainVC", null); break;
                    case "3": dic.Add("TNOtheViewController", null); break;
                    case "4": dic.Add("Tuhu.THCouponGoodsListVC", "{\"ruleID\":" + ruleId + "}"); break;
                    case "5": dic.Add("StoreListVC", "{\"vcType\":1}"); break;
                    case "6": dic.Add("StoreListVC", "{\"vcType\":2}"); break;
                    case "7": dic.Add("StoreListVC", "{\"vcType\":3}"); break;
                    default: dic.Add("TNCouponHelpVC", null); break;
                }
            }
            else
            {
                switch (model.HrefType.ToString())
                {
                    case "0": dic.Add("cn.TuHu.Activity.QuanHelpActivity", null); break;
                    case "1": dic.Add("cn.TuHu.Activity.TirChoose.TireUI", null); break;
                    case "2": dic.Add("cn.TuHu.Activity.Maintenance.CarMaintenanceActivity", null); break;
                    case "3": dic.Add("cn.TuHu.Activity.CarItemActivity", null); break;
                    case "4": dic.Add("cn.TuHu.Activity.MyPersonCenter.YHQGoodsActivity", "[{'RuleID':'" + ruleId + "'} ]"); break;
                    case "5": dic.Add("cn.TuHu.Activity.WashShopUI", "[{'ShowType':'CarWashing'} ]"); break;
                    case "6": dic.Add("cn.TuHu.Activity.WashShopUI", "[{'ShowType':'CarMaintenance'} ]"); break;
                    case "7": dic.Add("cn.TuHu.Activity.WashShopUI", "[{'ShowType':'CarInteriorClean'}]"); break;
                    default: dic.Add("cn.TuHu.Activity.QuanHelpActivity", null); break;
                }
            }
            return dic;

        }
        public static int SavePromotionInfoRule(IEnumerable<PromotionModel> PromotionModel, string action, int parentID, string shoptypes, string shopids)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                if (string.Compare(action, "update", false) == 0)
                {
                    if (parentID > 0 && UpdateParent(dbhelper, PromotionModel.FirstOrDefault(), parentID) > 0)
                    {
                        if (DeleteRecord(dbhelper, "allChild", parentID) >= 0)
                        {
                            if (!string.IsNullOrWhiteSpace(shoptypes))
                            {
                                foreach (var item in shoptypes.Split('|'))
                                {
                                    var shoptype = Convert.ToInt32(item);
                                    foreach (var p in PromotionModel)
                                    {
                                        var r = SavePromotionInfo(dbhelper, p, action, shoptype, null);
                                        if (r <= 0)
                                        {
                                            dbhelper.Rollback();
                                            return -1;
                                        }
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(shopids))
                            {
                                foreach (var item in shopids.Split('|').Distinct())
                                {
                                    var shopid = Convert.ToInt32(item);
                                    foreach (var p in PromotionModel)
                                    {
                                        var r = SavePromotionInfo(dbhelper, p, action, null, shopid);
                                        if (r <= 0)
                                        {
                                            dbhelper.Rollback();
                                            return -1;
                                        }
                                    }
                                }
                            }
                            if (string.IsNullOrWhiteSpace(shopids) && string.IsNullOrWhiteSpace(shoptypes))
                            {
                                foreach (var p in PromotionModel)
                                {
                                    var r = SavePromotionInfo(dbhelper, p, action, null, null);
                                    if (r <= 0)
                                    {
                                        dbhelper.Rollback();
                                        return -1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dbhelper.Rollback();
                            return -1;
                        }
                    }
                    else
                    {
                        dbhelper.Rollback();
                        return -1;
                    }
                    dbhelper.Commit();
                    return parentID;
                }
                else if (string.Compare(action, "add", false) == 0)
                {
                    var result = 0;
                    DALPromotion.isSuccessForCreateParent(dbhelper, PromotionModel.FirstOrDefault(), out result);
                    if (result > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(shoptypes))
                        {
                            foreach (var item in shoptypes.Split('|'))
                            {
                                var shoptype = Convert.ToInt32(item);
                                foreach (var p in PromotionModel)
                                {
                                    p.ParentID = result;
                                    var r = SavePromotionInfo(dbhelper, p, action, shoptype, null);
                                    if (r <= 0)
                                    {
                                        dbhelper.Rollback();
                                        return -1;
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(shopids))
                        {
                            foreach (var item in shopids.Split('|').Distinct())
                            {
                                var shopid = Convert.ToInt32(item);
                                foreach (var p in PromotionModel)
                                {
                                    p.ParentID = result;
                                    var r = SavePromotionInfo(dbhelper, p, action, null, shopid);
                                    if (r <= 0)
                                    {
                                        dbhelper.Rollback();
                                        return -1;
                                    }
                                }
                            }
                        }
                        if (string.IsNullOrWhiteSpace(shopids) && string.IsNullOrWhiteSpace(shoptypes))
                        {
                            foreach (var p in PromotionModel)
                            {
                                p.ParentID = result;
                                var r = SavePromotionInfo(dbhelper, p, action, null, null);
                                if (r <= 0)
                                {
                                    dbhelper.Rollback();
                                    return -1;
                                }
                            }
                        }

                        dbhelper.Commit();
                        return result;
                    }
                    else
                    {
                        dbhelper.Rollback();
                        return -1;
                    }
                }
                else
                    return -1;
            }
        }

        public static string FetchShopNameByID(int shopID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var OBJ = dbHelper.ExecuteScalar("SELECT VS.CarparName FROM Gungnir.dbo.Shops AS VS WITH(NOLOCK) WHERE VS.PKID=@ShopID", CommandType.Text, new SqlParameter("@ShopId", shopID));
                return OBJ == null || OBJ == DBNull.Value ? null : OBJ.ToString();
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
        public static DataTable UpdateOEM(int pkid)
        {
            using (var cmd = new SqlCommand(@"  SELECT  p.PKID,p.ParentID,CONVERT(VARCHAR(30),p.ExChangeStartTime,102) AS ExChangeStartTime,CONVERT(VARCHAR(30),p.ExChangeEndTime,102) AS ExChangeEndTime, isnull(Name,'') AS Name,CodeChannel,ISNULL(p.CodeType,'-1') AS CodeType,ISNULL(p.Money,'') AS Money,ISNULL(p.MinMoney,'') AS MinMoney,ISNULL(p.Number,'') AS Number,CONVERT(VARCHAR(30),p.CodeStartTime,102) AS CodeStartTime
,CONVERT(VARCHAR(30),p.CodeEndTime,102) AS CodeEndTime,ISNULL(p.IsActive,'') AS IsActive,ISNULL(p.Validity,'') AS Validity,LimitNum,OnlyForNewUser
    FROM    Activity..tbl_ExchangeCodeDetail AS P WITH ( NOLOCK )where PKID=@PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return DbHelper.ExecuteDataTable(cmd);
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
        public static DataTable SelectGiftByDonwLoad(int pkid)
        {
            var cmd = string.Format(@" SELECT ECD.PKID,ECD.IsActive ,(SELECT COUNT(1) FROM Activity..tbl_ExchangeCodeDetail WHERE ParentID={0}) AS T  FROM Activity..tbl_ExchangeCodeDetail AS ECD 
              WHERE ECD.PKID={0}", pkid);
            return DbHelper.ExecuteDataTable(cmd);
        }

        #region 5.0 生成随机字符串 
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length)
        {
            // 批量 需要考虑 伪随机问题
            Random r = new Random();
            string str = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 批量生成 优惠券兑换码
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="DetailsID"></param>
        /// <returns></returns>
        public static string GenerateCoupon(int Number, int DetailsID)
        {
            try
            {

                //using (var cmd = new SqlCommand("[Gungnir].[dbo].[Promotion_CreateExchangeCode]"))
                //{
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Parameters.AddWithValue("@Number", Number);
                //    cmd.Parameters.AddWithValue("@DetailsID", DetailsID);
                //    cmd.Parameters.Add(new SqlParameter()
                //    {
                //        ParameterName = "@Results",
                //        Direction = ParameterDirection.Output,
                //        SqlDbType = SqlDbType.Int
                //    });
                //    DbHelper.ExecuteDataTable(cmd);
                //    return cmd.Parameters["@Results"].Value.ToString();
                //}


                //List<string> randomStringList = new List<string>();
                for (int i = 0; i < Number; i++)
                {
                    string randomString = GetRandomString(8);
                    while (CheckExchangeCodeExist(randomString))
                    {
                        randomString = GetRandomString(8);
                    }
                    int  result = InsertExchangeCode(DetailsID, randomString);
                    //randomStringList.Add(randomString);
                }
                return "1";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 验证优惠券兑换码是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckExchangeCodeExist(string code)
        {
            string sql = @"  SELECT  Count(1)  
                             FROM  Activity..tbl_ExchangeCode WITH (NOLOCK)  
                             WHERE code = @code";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@code", code);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd)) > 0 ? true : false;
            }
        }


        /// <summary>
        /// 新增优惠券兑换码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int InsertExchangeCode(int DetailsID,string code)
        {
            string sql = @"INSERT Activity..tbl_ExchangeCode  
                                 (DetailsID,  
                                  Code,  
                                  Status,  
                                  CreateDateTime,  
                                  ExchangeTime,  
                                  UsedUserID  
                                 )  
                               VALUES (@DetailsID,  
                                  @code,  
                                  0,  
                                  GETDATE(),  
                                  NULL,  
                                  NULL    
                                 )  ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@DetailsID", DetailsID);
                cmd.Parameters.AddWithValue("@code", code);
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

        public static DataTable SelectPromotionDetailsByEdit(int id)
        {
            using (var cmd = new SqlCommand(@"SELECT ECD.PKID,ecd.ParentID,Name,CodeChannel,Validity,ecd.RuleId,ecd.Money,ecd.MinMoney,ecd.Number,
                    CONVERT(VARCHAR(30),ECD.CodeStartTime,102) AS CodeStartTime
                    ,CONVERT(VARCHAR(30),ECD.CodeEndTime,102) AS CodeEndTime,ECD.IsActive,ecd.Validity,ECD.DepartmentId,ECD.IntentionId
                    FROM Activity..tbl_ExchangeCodeDetail AS ECD 
                    WHERE ECD.PKID=@id"))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return DbHelper.ExecuteDataTable(cmd);
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
        public static int AddOEM(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Activity..tbl_ExchangeCodeDetail(ParentID,Name,CodeChannel,ExChangeStartTime,ExChangeEndTime,IsActive,OnlyForNewUser,LimitNum)
VALUES(@ParentID,@Name,@CodeChannel,@ExChangeStartTime,@ExChangeEndTime,@IsActive,@OnlyForNewUser,@LimitNum)"))
            {
                cmd.Parameters.AddWithValue("@ParentID", 0);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@ExChangeStartTime", ecd.ExChangeStartTime);
                cmd.Parameters.AddWithValue("@ExChangeEndTime", ecd.ExChangeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                cmd.Parameters.AddWithValue("@OnlyForNewUser", ecd.OnlyForNewUser);
                cmd.Parameters.AddWithValue("@LimitNum", ecd.LimitNum);
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
                                                    IsActive = @IsActive,
                                                    OnlyForNewUser=@OnlyForNewUser,
                                                    LimitNum=@LimitNum
                                            WHERE   PKID = @PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", ecd.PKID);
                cmd.Parameters.AddWithValue("@Name", ecd.Name);
                cmd.Parameters.AddWithValue("@CodeChannel", ecd.CodeChannel);
                cmd.Parameters.AddWithValue("@ExChangeStartTime", ecd.ExChangeStartTime);
                cmd.Parameters.AddWithValue("@ExChangeEndTime", ecd.ExChangeEndTime);
                cmd.Parameters.AddWithValue("@IsActive", ecd.IsActive);
                cmd.Parameters.AddWithValue("@OnlyForNewUser", ecd.OnlyForNewUser);
                cmd.Parameters.AddWithValue("@LimitNum", ecd.LimitNum);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        public static int SelectDownloadByPKID(int pkid)
        {
            var cmd = string.Format(
                @"SELECT Count(1) FROM Activity..tbl_ExchangeCode AS EC
                WHERE EC.DetailsID={0}", pkid);
            return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
        }
        /// <summary>
        /// 查询改礼包是否已生成券
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int SelectPromoCodeCount(int pkid)
        {
            using (var cmd = new SqlCommand(@" SELECT COUNT(1) FROM  [Activity].[dbo].[tbl_ExchangeCode] WHERE DetailsID=@PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }

        }

        /// <summary>
        /// 删除礼包
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int DeleteGift(int pkid)
        {
            using (var cmd = new SqlCommand(@"DELETE FROM [Activity].[dbo].[tbl_ExchangeCodeDetail] WHERE ParentID=@PKID or PKID=@PKID "))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return DbHelper.ExecuteNonQuery(cmd);
            }

        }
        /// <summary>
        /// Excel导出
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static DataTable CreateExcel(int pkid)
        {
            using (var cmd = new SqlCommand("SELECT EC.Code FROM Activity..tbl_ExchangeCode AS EC WHERE EC.DetailsID=@DetailsID and Status=0"))
            {
                cmd.Parameters.AddWithValue("@DetailsID", pkid);
                return DbHelper.ExecuteDataTable(cmd);
            }


        }
        public static int CreeatePromotion(ExchangeCodeDetail ecd)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Activity..tbl_ExchangeCodeDetail(ParentID,Name,CodeChannel,RuleId,Money,MinMoney,Number,CodeStartTime,CodeEndTime,IsActive,Validity,CodeType,DepartmentId,IntentionId,Creater,Issuer)
VALUES(@ParentID,@Name,@CodeChannel,@RuleId,@Money,@MinMoney,@Number,@CodeStartTime,@CodeEndTime,@IsActive,@Validity,@CodeType,@DepartmentId,@IntentionId,@Creater,@Issuer)"))
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
                cmd.Parameters.AddWithValue("@DepartmentId", ecd.DepartmentId);
                cmd.Parameters.AddWithValue("@IntentionId", ecd.IntentionId);
                cmd.Parameters.AddWithValue("@Creater", ecd.Creater);
                cmd.Parameters.AddWithValue("@Issuer", ecd.Issuer);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
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
                                                    Validity=@Validity,
                                                    DepartmentId=@DepartmentId,
                                                    IntentionId=@IntentionId,
                                                    Issuer=@Issuer
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
                cmd.Parameters.AddWithValue("@DepartmentId", ecd.DepartmentId);
                cmd.Parameters.AddWithValue("@IntentionId", ecd.IntentionId);
                cmd.Parameters.AddWithValue("@Issuer", ecd.Issuer);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }
        public static int DeletePromoCode(int pkid)
        {
            using (var cmd = new SqlCommand(@"  DELETE FROM Activity..tbl_ExchangeCodeDetail WHERE PKID=@PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return DbHelper.ExecuteNonQuery(cmd);
            }


        }
        public static DataTable GetDepartmentUseSetting()
        {
            using (var cmd = new SqlCommand(@"Select * from Configuration.[dbo].[CouponDepartmentUseSetting] with(nolock) where IsDel=0", new SqlConnection(Configuration_Readonly)))
            {
                return DbHelper.ExecuteDataTable(cmd);
            }


        }
        public static DataTable GetDepartmentUseSettingNameBySettingId(List<int> ids)
        {
            string sql = @"Select settingId,displayName from Configuration.[dbo].[CouponDepartmentUseSetting] with(nolock) where IsDel=0 and SettingId in({0})";
            using (var cmd = new SqlCommand(string.Format(sql, string.Join(",", ids))))
            {
                return DbHelper.ExecuteDataTable(cmd);
            }


        }
        public static DataTable GetDepartmentUseSettingByParentId(int parentId)
        {
            using (var cmd = new SqlCommand(@"Select * from Configuration.[dbo].[CouponDepartmentUseSetting] with(nolock) where parentSettingId=@parentId and IsDel=0 order by CreateTime asc"))
            {
                cmd.Parameters.AddWithValue("@parentId", parentId);
                return DbHelper.ExecuteDataTable(cmd);
            }


        }
        public static DataTable GetDepartmentUseSettingNameBySettingId(int id)
        {
            string sql = @"Select * from Configuration.[dbo].[CouponDepartmentUseSetting] with(nolock) where IsDel=0 and SettingId=@SettingId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SettingId", id);
                return DbHelper.ExecuteDataTable(cmd);
            }


        }
        /// <summary>
        /// 获取部门和用途信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="settingId"></param>
        /// <returns></returns>
        public static DepartmentAndUseModel GetCouponDepartmentUseSettingBySettingId(SqlConnection conn, int settingId)
        {
            #region SQL
            string sql = @"
                            SELECT [PKID]
                                ,[SettingId]
                                ,[DisplayName]
                                ,[ParentSettingId]
                                ,[Type]
                                ,[IsDel]
                                ,[CreateTime]
                                ,[UpdateTime]
                            FROM [Configuration]..[CouponDepartmentUseSetting] WITH(NOLOCK) WHERE SettingId=@SettingId ";
            #endregion
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@SettingId", settingId);
            return conn.Query<DepartmentAndUseModel>(sql, dp).FirstOrDefault();
        }
        public static bool DeleteDepartmentUseSettingNameBySettingId(int id)
        {
            string sql = @"update  Configuration.[dbo].[CouponDepartmentUseSetting] set isDel=1 ,updateTime=getdate()  where  SettingId=@SettingId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SettingId", id);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }


        }
        public static bool InsertDepartmentUseSetting(DepartmentAndUseModel model)
        {
            string sql = @"insert into Configuration.[dbo].[CouponDepartmentUseSetting](settingid,DisplayName, ParentSettingId, Type, IsDel, CreateTime, UpdateTime) 
                            values(0,@DisplayName, @ParentSettingId, @Type, 0, getdate(), null);
                           update  Configuration.[dbo].[CouponDepartmentUseSetting] with(rowlock) set settingid=@@IDENTITY  where PKID=@@IDENTITY;
                           Select @@IDENTITY; ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@DisplayName", model.DisplayName);
                cmd.Parameters.AddWithValue("@ParentSettingId", model.ParentSettingId);
                cmd.Parameters.AddWithValue("@Type", model.Type);
                model.SettingId = Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
                return model.SettingId > 0;
            }
        }
        public static bool UpdateDepartmentUseSetting(DepartmentAndUseModel model)
        {
            string sql = @"update   Configuration.[dbo].[CouponDepartmentUseSetting] set  DisplayName=@DisplayName, UpdateTime=GetDate() where SettingId=@SettingId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SettingId", model.SettingId);
                cmd.Parameters.AddWithValue("@DisplayName", model.DisplayName);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }



        }

        public static DataTable GetAllBusinessLines()
        {
            string sql = "SELECT * FROM Configuration.[dbo].[PromotionBusinessLineConfig] WITH(NOLOCK) WHERE IsDel=0";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static DataTable GetAllBusinessLinesById(int businessLineId)
        {
            string sql = "SELECT * FROM Configuration.[dbo].[PromotionBusinessLineConfig] WITH(NOLOCK) WHERE IsDel=0 AND PKID=@BusinessLineId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@BusinessLineId", businessLineId);
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static int SaveBusinessLine(PromotionBusinessLineModel model)
        {
            string sql = "";
            SqlParameter[] parameters = null;
            if (model.PKID > 0)
            {
                sql = @"UPDATE Configuration.[dbo].[PromotionBusinessLineConfig] WITH(ROWLOCK) SET DisplayName=@DisplayName,LastUpdater=@Operater,UpdateTime=GETDATE() WHERE PKID=@PKID";
                parameters = new[]
                {
                    new SqlParameter("@DisplayName",model.DisplayName),
                    new SqlParameter("@Operater", model.Operater),
                    new SqlParameter("@PKID",model.PKID)
                };
            }
            else
            {
                sql = @"INSERT INTO Configuration.[dbo].[PromotionBusinessLineConfig](DisplayName,Creater,LastUpdater,IsDel,UpdateTime,CreateTime)
                        VALUES(@DisplayName,@Operater,@Operater,0,GETDATE(),GETDATE())";
                parameters = new[]
                {
                    new SqlParameter("@DisplayName",model.DisplayName),
                    new SqlParameter("@Operater", model.Operater)
                };
            }
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddRange(parameters);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static int DeleteBusinessLine(int pkid, string operater)
        {
            string sql = @"UPDATE Configuration.[dbo].[PromotionBusinessLineConfig] WITH(ROWLOCK) SET IsDel=1,LastUpdater=@Operater,UpdateTime=GETDATE() WHERE PKID=@PKID";
            SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@Operater", operater),
                    new SqlParameter("@PKID",pkid)
                };
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddRange(parameters);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 保存优惠券使用规则
        /// </summary>
        /// <param name="PromotionModel"></param>
        /// <param name="action"> "add" : "update"</param>
        /// <param name="parentID"></param>
        /// <param name="shoptypes"></param>
        /// <param name="shopids"></param>
        /// <returns></returns>
        public static int SaveCouponRuleInfo(PromotionModel PromotionModel, string[] shoptypes, string[] shopids, string[] categorys, string[] brands, string[] pids)
        {
            var result = 0;
            #region sql_InsertCouponRule
            const string sql_InsertCouponRule = @"INSERT	INTO Activity..tbl_CouponRules
                                               		(
                                               		  Name,
                                               		  Category,
                                               		  ProductID,
                                               		  Brand,
                                               		  ParentID,
                                               		  IsActive,
                                               		  CreateDateTime,
                                               		  LastDateTime,
                                               		  InstallType,
                                                      OrderPayMethod,
                                               		  IOSKey,
                                               		  IOSValue,
                                               		  androidKey,
                                               		  androidValue,
                                               		  HrefType,
                                                      CustomSkipPage,
                                                      WxSkipPage,
                                                      H5SkipPage,
                                               		  PromotionType,
                                                      ConfigType,
                                                      PIDType,
                                                      EnabledGroupBuy,
                                                      RuleDescription)
                                                VALUES	(
                                               		  @Name,
                                               		  NULL,
                                               		  NULL,
                                               		  NULL,
                                               		  0,
                                               		  0,
                                               		  GETDATE(),
                                               		  GETDATE(),
                                               		  @InstallType,
                                               		  @OrderPayMethod,
                                               		  @IOSKey,
                                               		  @IOSValue,
                                               		  @androidKey,
                                               		  @androidValue,
                                               		  @HrefType,
                                                      @CustomSkipPage,
                                                      @WxSkipPage,
                                                      @H5SkipPage,
                                               		  @PromotionType,
                                                      @ConfigType,
                                                      @PIDType,
                                                      @EnabledGroupBuy,
                                                      @RuleDescription);
                                                SELECT	ISNULL(@@IDENTITY,0)";
            #endregion
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    using (var cmd = new SqlCommand(sql_InsertCouponRule))
                    {
                        #region cmdPara
                        cmd.Parameters.AddWithValue("@Name", PromotionModel.Name);
                        cmd.Parameters.AddWithValue("@InstallType", PromotionModel.InstallType);
                        cmd.Parameters.AddWithValue("@OrderPayMethod", PromotionModel.OrderPayMethod);
                        cmd.Parameters.AddWithValue("@IOSKey", PromotionModel.GetKeyValue(true, 0).FirstOrDefault().Key);
                        cmd.Parameters.AddWithValue("@IOSValue", PromotionModel.GetKeyValue(true, 0).FirstOrDefault().Value);
                        cmd.Parameters.AddWithValue("@androidKey", PromotionModel.GetKeyValue(false, 0).FirstOrDefault().Key);
                        cmd.Parameters.AddWithValue("@androidValue", PromotionModel.GetKeyValue(false, 0).FirstOrDefault().Value);
                        cmd.Parameters.AddWithValue("@HrefType", PromotionModel.HrefType);
                        cmd.Parameters.AddWithValue("@CustomSkipPage", PromotionModel.CustomSkipPage);
                        cmd.Parameters.AddWithValue("@WxSkipPage", PromotionModel.WxSkipPage);
                        cmd.Parameters.AddWithValue("@H5SkipPage", PromotionModel.H5SkipPage);
                        cmd.Parameters.AddWithValue("@PromotionType", PromotionModel.PromotionType);
                        cmd.Parameters.AddWithValue("@ConfigType", PromotionModel.ConfigType);
                        cmd.Parameters.AddWithValue("@PIDType", PromotionModel.PIDType);
                        cmd.Parameters.AddWithValue("@EnabledGroupBuy", PromotionModel.EnabledGroupBuy);
                        cmd.Parameters.AddWithValue("@RuleDescription", PromotionModel.RuleDescription);
                        #endregion
                        var temp = Convert.ToInt32(db.ExecuteScalar(cmd));
                        if (temp > 0)
                        {


                            if (CreateCouponConfigs(temp, shoptypes, shopids, categorys, brands, pids, db))
                            {
                                db.Commit();
                                if (PromotionModel.CustomSkipPage != null
                                    && PromotionModel.CustomSkipPage.Contains("{{TempProductListRouteLink}}")
                                    || PromotionModel.CustomSkipPage.Contains("{{GroupBuyingProductLink}}"))
                                    UpdateCustomSkipPageLink(temp, PromotionModel.CustomSkipPage);
                                if (PromotionModel.WxSkipPage != null
                                    && PromotionModel.WxSkipPage.Contains("{{TempProductListRouteLink}}")
                                    || PromotionModel.WxSkipPage.Contains("{{GroupBuyingProductLink}}"))
                                    UpdateWxSkipPageLink(temp, PromotionModel.WxSkipPage);
                                if (PromotionModel.H5SkipPage != null
                                    && PromotionModel.H5SkipPage.Contains("{{TempProductListRouteLink}}")
                                    || PromotionModel.H5SkipPage.Contains("{{GroupBuyingProductLink}}"))
                                    UpdateH5SkipPageLink(temp, PromotionModel.H5SkipPage);
                                result = temp;
                            }
                            else
                            {
                                db.Rollback();
                            }
                        }
                        else
                        {
                            db.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.Rollback();
                }
            }
            return result;
        }
        /// <summary>
        /// 变更APP页面跳转链接
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="customSkipPage"></param>
        /// <returns></returns>
        private static int UpdateCustomSkipPageLink(int ruleId, string customSkipPage)
        {
            if (customSkipPage == "{{TempProductListRouteLink}}")
                customSkipPage = "/searchResult?ruleid=" + ruleId + "&s=";
            else if (customSkipPage == "{{GroupBuyingProductLink}}")
                customSkipPage = "/webView?url=https%3A%2F%2Fwx.tuhu.cn%2Fvue%2FGroupBuy%2Fpages%2Fsearch%2Fsearch%3FruleId%3D" + ruleId;
            using (var cmd = new SqlCommand(@"update Activity..tbl_CouponRules set CustomSkipPage=@CustomSkipPage where Pkid=@RuleID"))
            {
                cmd.Parameters.AddWithValue("@RuleID", ruleId);
                cmd.Parameters.AddWithValue("@CustomSkipPage", customSkipPage);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }
        /// <summary>
        /// 变更小程序页面跳转链接
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="wxSkipPage"></param>
        /// <returns></returns>
        private static int UpdateWxSkipPageLink(int ruleId, string wxSkipPage)
        {

            if (wxSkipPage == "{{TempProductListRouteLink}}")       //商品列表
                wxSkipPage = "/pages/search/search?flag=coupon&ruleId=" + ruleId;
            else if (wxSkipPage == "{{GroupBuyingProductLink}}")    //拼团商品列表
                wxSkipPage = "/pages/search/search?appid=wx25f9f129712845af&ruleId=" + ruleId;
            using (var cmd = new SqlCommand(@"update Activity..tbl_CouponRules set WxSkipPage=@WxSkipPage where Pkid=@RuleID"))
            {
                cmd.Parameters.AddWithValue("@RuleID", ruleId);
                cmd.Parameters.AddWithValue("@WxSkipPage", wxSkipPage);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }
        /// <summary>
        /// 变更H5面跳转链接
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="h5SkipPage"></param>
        /// <returns></returns>
        private static int UpdateH5SkipPageLink(int ruleId, string h5SkipPage)
        {
            if (h5SkipPage == "{{TempProductListRouteLink}}")
                h5SkipPage = "http://wx.tuhu.cn/ChePin/CpList?ruleId=" + ruleId;
            else if (h5SkipPage == "{{GroupBuyingProductLink}}")
                h5SkipPage = "https://wx.tuhu.cn/vue/GroupBuy/pages/search/search?ruleId=" + ruleId;
            using (var cmd = new SqlCommand(@"update Activity..tbl_CouponRules set H5SkipPage=@H5SkipPage where Pkid=@RuleID"))
            {
                cmd.Parameters.AddWithValue("@RuleID", ruleId);
                cmd.Parameters.AddWithValue("@H5SkipPage", h5SkipPage);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }
        private static bool CreateCouponConfigs(int ruleId, string[] shoptypes, string[] shopids, string[] categorys, string[] brands, string[] pids, BaseDbHelper db)
        {
            //1=Category;2=Brand;4=PID；8=门店类型;16=ShopId
            #region sql_InsertCouponConfig
            const string sql_InsertCouponConfig = @"Insert Into {0}
  Values(@RuleID
		 ,@Type
		 ,@ConfigValue
		 ,getdate()
		 ,getdate()
		 )";
            #endregion
            using (var cmd = new SqlCommand())
            {
                var InsertResult = true;

                #region 门店类型
                foreach (var item in shoptypes ?? new string[] { })
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = string.Format(sql_InsertCouponConfig, "[Activity].[dbo].[tbl_CouponRules_ConfigShop]");
                    cmd.Parameters.AddWithValue("@RuleID", ruleId);
                    cmd.Parameters.AddWithValue("@Type", 8);
                    cmd.Parameters.AddWithValue("@ConfigValue", item);
                    if (db.ExecuteNonQuery(cmd) <= 0)
                    {
                        InsertResult = false;
                        break;
                    }
                }
                if (!InsertResult)
                    return InsertResult;
                #endregion

                #region 门店id
                foreach (var item in shopids ?? new string[] { })
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = string.Format(sql_InsertCouponConfig, "[Activity].[dbo].[tbl_CouponRules_ConfigShop]");
                    cmd.Parameters.AddWithValue("@RuleID", ruleId);
                    cmd.Parameters.AddWithValue("@Type", 16);
                    cmd.Parameters.AddWithValue("@ConfigValue", item);
                    if (db.ExecuteNonQuery(cmd) <= 0)
                    {
                        InsertResult = false;
                        break;
                    }
                }
                if (!InsertResult)
                    return InsertResult;
                #endregion

                #region 产品类目
                foreach (var item in categorys)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = string.Format(sql_InsertCouponConfig, "[Activity].[dbo].[tbl_CouponRules_ConfigProduct]");
                    cmd.Parameters.AddWithValue("@RuleID", ruleId);
                    cmd.Parameters.AddWithValue("@Type", 1);
                    cmd.Parameters.AddWithValue("@ConfigValue", item);
                    if (db.ExecuteNonQuery(cmd) <= 0)
                    {
                        InsertResult = false;
                        break;
                    }
                }
                if (!InsertResult)
                    return InsertResult;
                #endregion
                #region 产品品牌
                foreach (var item in brands)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = string.Format(sql_InsertCouponConfig, "[Activity].[dbo].[tbl_CouponRules_ConfigProduct]");
                    cmd.Parameters.AddWithValue("@RuleID", ruleId);
                    cmd.Parameters.AddWithValue("@Type", 2);
                    cmd.Parameters.AddWithValue("@ConfigValue", item);
                    if (db.ExecuteNonQuery(cmd) <= 0)
                    {
                        InsertResult = false;
                        break;
                    }
                }
                if (!InsertResult)
                    return InsertResult;
                #endregion
                #region 产品PIDs
                foreach (var item in pids)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = string.Format(sql_InsertCouponConfig, "[Activity].[dbo].[tbl_CouponRules_ConfigProduct]");
                    cmd.Parameters.AddWithValue("@RuleID", ruleId);
                    cmd.Parameters.AddWithValue("@Type", 4);
                    cmd.Parameters.AddWithValue("@ConfigValue", item);
                    if (db.ExecuteNonQuery(cmd) <= 0)
                    {
                        InsertResult = false;
                        break;
                    }
                }
                if (!InsertResult)
                    return InsertResult;
                #endregion
                return InsertResult;
            }
        }

        public static int UpdateCouponRuleInfo(PromotionModel PromotionModel, string[] shoptypes, string[] shopids, string[] categorys, string[] brands, string[] pids)
        {
            var result = 0;
            const string sql_UpdateCouponRule = @"Update Activity..tbl_CouponRules with(rowlock) 
                                                        set Name=@Name,
                                                        IOSKey=@IOSKey,
                                                        IOSValue=@IOSValue,
                                                        androidKey=@androidKey,
                                                        androidValue=@androidValue,
                                                        HrefType=@HrefType,
                                                        CustomSkipPage=@CustomSkipPage,
                                                        WxSkipPage=@WxSkipPage,
                                                        H5SkipPage=@H5SkipPage,
                                                        PromotionType=@PromotionType,
                                                        InstallType=@InstallType,
                                                        OrderPayMethod=@OrderPayMethod,
                                                        ConfigType=@ConfigType,
                                                        PIDType=@PIDType,
                                                        EnabledGroupBuy=@EnabledGroupBuy,
                                                        RuleDescription=@RuleDescription
                                                        where  PKID=@PKID";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    using (var cmd = new SqlCommand(sql_UpdateCouponRule))
                    {
                        #region cmdPara
                        cmd.Parameters.AddWithValue("@Name", PromotionModel.Name);
                        cmd.Parameters.AddWithValue("@InstallType", PromotionModel.InstallType);
                        cmd.Parameters.AddWithValue("@OrderPayMethod", PromotionModel.OrderPayMethod);
                        cmd.Parameters.AddWithValue("@IOSKey", PromotionModel.GetKeyValue(true, 0).FirstOrDefault().Key);
                        cmd.Parameters.AddWithValue("@IOSValue", PromotionModel.GetKeyValue(true, 0).FirstOrDefault().Value);
                        cmd.Parameters.AddWithValue("@androidKey", PromotionModel.GetKeyValue(false, 0).FirstOrDefault().Key);
                        cmd.Parameters.AddWithValue("@androidValue", PromotionModel.GetKeyValue(false, 0).FirstOrDefault().Value);
                        cmd.Parameters.AddWithValue("@HrefType", PromotionModel.HrefType);
                        cmd.Parameters.AddWithValue("@CustomSkipPage", PromotionModel.CustomSkipPage);
                        cmd.Parameters.AddWithValue("@WxSkipPage", PromotionModel.WxSkipPage);
                        cmd.Parameters.AddWithValue("@H5SkipPage", PromotionModel.H5SkipPage);
                        cmd.Parameters.AddWithValue("@PromotionType", PromotionModel.PromotionType);
                        cmd.Parameters.AddWithValue("@ConfigType", PromotionModel.ConfigType);
                        cmd.Parameters.AddWithValue("@PIDType", PromotionModel.PIDType);
                        cmd.Parameters.AddWithValue("@EnabledGroupBuy", PromotionModel.EnabledGroupBuy);
                        cmd.Parameters.AddWithValue("@RuleDescription", PromotionModel.RuleDescription);
                        cmd.Parameters.AddWithValue("@PKID", PromotionModel.PKID);
                        #endregion
                        var temp = db.ExecuteNonQuery(cmd);
                        if (temp > 0)
                        {
                            if (DeleteCouponConfigs(PromotionModel.PKID, db) && CreateCouponConfigs(PromotionModel.PKID, shoptypes, shopids, categorys, brands, pids, db))
                            {
                                db.Commit();
                                result = PromotionModel.PKID;
                            }
                            else
                            {
                                db.Rollback();
                            }
                        }
                        else
                        {
                            db.Rollback();
                        }
                    }

                }
                catch (Exception ex)
                {
                    db.Rollback();
                }
            }
            return result;
        }

        private static bool DeleteCouponConfigs(int ruleId, BaseDbHelper db)
        {
            const string sql = @"Delete from [Activity].[dbo].[tbl_CouponRules_ConfigShop] where RuleId=@RuleId;Delete from [Activity].[dbo].[tbl_CouponRules_ConfigProduct] where RuleId=@RuleId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@RuleId", ruleId);
                try
                {
                    return db.ExecuteNonQuery(cmd) >= 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static DataTable GetPromotionDetail(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"  SELECT	    top 1 *
                                             FROM		Activity..tbl_CouponRules  WITH ( NOLOCK )
                                             WHERE		pkid= @pkid;");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@pkid", id);
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        public static DataTable GetCouponShopConfig(int ruleId)
        {
            const string sql = @"SELECT CCP.*,VS.CarparName as ShopName  FROM [Activity].[dbo].[tbl_CouponRules_ConfigShop] AS CCP WITH(NOLOCK) 
LEFT JOIN Gungnir.dbo.vw_Shop AS VS WITH(NOLOCK)  on CONVERT(int,CCP.ConfigValue)=VS.PKID
WHERE RuleId=@RuleId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@RuleId", ruleId);
                var result = DbHelper.ExecuteDataTable(cmd);
                return result;
            }
        }
        public static DataTable GetCouponProductConfig(int ruleId)
        {
            const string sql = @"SELECT PKID, RuleID, Type, ConfigValue, CreateDateTime, LastUpdateDateTime  FROM [Activity].[dbo].[tbl_CouponRules_ConfigProduct] WITH(NOLOCK) WHERE RuleId=@RuleId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@RuleId", ruleId);
                var result = DbHelper.ExecuteDataTable(cmd);
                return result;
            }
        }
        public static DataTable SelectProductNamesByPIDs(string[] pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var OBJ = dbHelper.ExecuteDataTable($"SELECT CP.ProductID + '|' + VariantID AS PID,CP.DisplayName,Exist=1 FROM Tuhu_productcatalog..[CarPAR_zh-CN] AS CP WITH(NOLOCK) WHERE CP.ProductID + '|' + VariantID COLLATE Chinese_PRC_CI_AS IN({"'" + String.Join("','", pids) + "'"})", CommandType.Text);
                return OBJ;
            }
        }
        public static DataTable SelectShopNamesByIDs(int[] shopIDs)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var OBJ = dbHelper.ExecuteDataTable($"SELECT VS.PKID,VS.CarparName,Exist=1 FROM Gungnir.dbo.Shops AS VS WITH(NOLOCK) WHERE VS.PKID IN({String.Join(",", shopIDs)})", CommandType.Text);
                return OBJ;
            }
        }
        public static IEnumerable<PromotionModel> SelectPromotionDetailNew(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            const string sqlRules = @"select [Name],[ConfigType] From [Activity].[dbo].[tbl_CouponRules] with(nolock) where pkid=@pkid";
            const string sqlShopConfig = @"Select  [Type],ConfigValue from [Activity].[dbo].[tbl_CouponRules_ConfigShop] with(nolock) where RuleId=@pkid";
            const string sqlProductConfig = @"Select  [Type],ConfigValue from [Activity].[dbo].[tbl_CouponRules_ConfigProduct] with(nolock) where RuleId=@pkid";
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand();
                cmd.CommandText = sqlRules;
                cmd.Parameters.AddWithValue("@pkid", id);
                var dt = dbhelper.ExecuteDataTable(cmd);
                if (dt == null || dt.Rows.Count <= 0)
                    return null;
                var result = new List<PromotionModel>();

                var couponRule = Tuple.Create(dt.Rows[0].GetValue<string>("Name"), dt.Rows[0].GetValue<int>("ConfigType"));

                cmd.CommandText = sqlShopConfig;
                var shopConfig = dbhelper.ExecuteDataTable(cmd);

                cmd.CommandText = sqlProductConfig;
                var productConfig = dbhelper.ExecuteDataTable(cmd);
                if (productConfig != null && productConfig.Rows.Count > 0)
                {
                    var productList = new List<PromotionModel>();
                    foreach (DataRow item in productConfig.Rows)
                    {
                        productList.Add(new PromotionModel
                        {
                            Name = couponRule.Item1,
                            Category = item.GetValue<int>("Type") == 2 ? item.GetValue<string>("ConfigValue").Split('|').FirstOrDefault() : (item.GetValue<int>("Type") == 1 ? item.GetValue<string>("ConfigValue") : "——"),
                            Brand = item.GetValue<int>("Type") == 2 ? item.GetValue<string>("ConfigValue").Split('|').LastOrDefault() : "——",
                            ProductID = item.GetValue<int>("Type") == 4 ? item.GetValue<string>("ConfigValue") : "——",
                        });
                    }
                    if (shopConfig != null && shopConfig.Rows.Count > 0)
                    {
                        foreach (DataRow item in shopConfig.Rows)
                        {
                            productList.ForEach(f =>
                            {
                                result.Add(new PromotionModel
                                {
                                    ShopType = item.GetValue<int>("Type") == 8 ? item.GetValue<int>("ConfigValue") : 0,
                                    ShopID = item.GetValue<int>("Type") == 16 ? item.GetValue<int>("ConfigValue") : 0,
                                    Name = f.Name,
                                    Category = f.Category,
                                    Brand = f.Brand,
                                    ProductID = f.ProductID
                                });
                            });
                        }
                    }
                    else
                    {
                        result = productList;
                    }
                }
                return result;
            }

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

        public static int GetUserCountByMobilels(IEnumerable<string> mobiles)
        {
            var cellPhoneTable = new DataTable();
            cellPhoneTable.Columns.Add("UserCellPhone", typeof(string));

            foreach (var cellPhone in mobiles)
            {
                var newRow = cellPhoneTable.NewRow();
                newRow[0] = cellPhone;
                cellPhoneTable.Rows.Add(newRow);
            }
            using (var cmd = new SqlCommand(@"SELECT COUNT(1) FROM Tuhu_profiles..UserObject AS U WITH(NOLOCK) JOIN @Mobiles AS M ON U.u_mobile_number=M.Cellphone 
                                            AND U.IsActive=1"))
            {
                cmd.Parameters.Add(
                    new SqlParameter("@Mobiles", cellPhoneTable) { SqlDbType = SqlDbType.Structured, TypeName = "Cellphone" });
                return (int)DbHelper.ExecuteScalar(cmd);
            }
        }

        public static DataTable GetPromotionLog(string objectId, string objectType)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                string sql =
                    @"SELECT * FROM Tuhu_log..PromotionConfigLog WITH(NOLOCK) WHERE ObjectId=@ObjectId AND ObjectType=@ObjectType ORDER BY PKID DESC";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ObjectId", objectId);
                    cmd.Parameters.AddWithValue("@ObjectType", objectType);
                    return dbHelper.ExecuteDataTable(cmd);
                }
            }
        }
    }
}

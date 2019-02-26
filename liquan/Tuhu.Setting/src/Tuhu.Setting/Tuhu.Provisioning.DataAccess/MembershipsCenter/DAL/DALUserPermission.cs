using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALUserPermission
    {
        #region 会员权益(特权)
        /// <summary>
        /// 获取会员权益列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<UserPermissionModel> QueryPageList(UserPermissionModel search, int pageIndex, int pageSize)
        {
            var strSql = new StringBuilder();
            var strCondition = new StringBuilder();
            var sqlParamerts = new List<SqlParameter>();
            #region 查询条件
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    strCondition.AppendFormat(" And Name like @Name");
                    sqlParamerts.Add(new SqlParameter("@Name", "%" + search.Name + "%"));
                }
                if (search.MembershipsGradeId > 0)
                {
                    strCondition.AppendFormat(" And MembershipsGradeId=@MembershipsGradeId");
                    sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", search.MembershipsGradeId));
                }
            }
            #endregion
            #region sql语句
            strSql.AppendFormat(@"
SELECT *
FROM   (   SELECT ROW_NUMBER() OVER ( ORDER BY Id desc ) AS rn ,
                  [Id] ,
                  [Name] ,
                  [LightImage] ,
                  [DarkImage] ,
                  [TopImage] ,
                  [Position] ,
                  [IsTopImage] ,
                  [UseUserLevel] ,
                  [Description] ,
                  [IsEnable] ,
                  [IsLight] ,
                  [FootTile] ,
                  [Version] ,
                  [DescriptionTitle] ,
                  [LightText] ,
                  [DarkText] ,
                  [MembershipsGradeId] ,
                  [PermissionType] ,
                  [EnabledVersion] ,
                  [EndVersion] ,
                  [CreateDatetime] ,
                  [CreateBy] ,
                  [LastUpdateDateTime] ,
                  [LastUpdateBy] ,
                  [IsDeleted] ,
                  [CheckCycle] ,
                  [IsLinkUrl] ,
                  [LightUrl] ,
                  [LightButtonUrl] ,
                  [CardImage] ,
                  [AndroidUrl] ,
                  [IOSUrl] ,
                  [DescriptionDetail] ,
                  [IsClickReceive] ,
                  [PromptTag]
           FROM   Gungnir.dbo.tbl_UserPermission WITH ( NOLOCK )
           WHERE   IsDeleted=0  {0}
) t WHERE  t.rn > @StartRow  AND t.rn <= @EndRow 
ORDER BY Id DESC
", strCondition.ToString());
            #endregion

            sqlParamerts.Add(new SqlParameter("@StartRow", pageSize * (pageIndex - 1)));
            sqlParamerts.Add(new SqlParameter("@EndRow", pageSize * pageIndex));

            using (var dataCmd = new SqlCommand(strSql.ToString()))
            {
                dataCmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteDataTable(dataCmd)?.ConvertTo<UserPermissionModel>().ToList();
            }
        }


        /// <summary>
        /// 获取会员权益数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public static int QueryPageCount(UserPermissionModel search)
        {
            var strCount = new StringBuilder();
            var strCondition = new StringBuilder();
            var sqlParamerts = new List<SqlParameter>();
            #region  获取数量
            strCondition.Append(@"
            SELECT ISNULL(COUNT(*),0) AS totalCount 
FROM   Gungnir.dbo.tbl_UserPermission WITH ( NOLOCK )
WHERE  IsDeleted = 0
            ");
            #endregion
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    strCondition.AppendFormat(" And Name like @Name");
                    sqlParamerts.Add(new SqlParameter("@Name", "%" + search.Name + "%"));
                }
                if (search.MembershipsGradeId > 0)
                {
                    strCondition.AppendFormat(" And MembershipsGradeId=@MembershipsGradeId");
                    sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", search.MembershipsGradeId));
                }
            }
            using (var countCmd = new SqlCommand(strCondition.ToString()))
            {
                countCmd.Parameters.AddRange(sqlParamerts.ToArray());
                var count = DbHelper.ExecuteScalar(countCmd);
                return Convert.ToInt32(count);
            }
        }

        /// <summary>
        /// 添加会员权益
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(UserPermissionModel model)
        {
            var sqlParamerts = new List<SqlParameter>();
            #region sql脚本
            var strSql = @"
INSERT INTO Gungnir..tbl_UserPermission (   [Name] ,
                                            [LightImage] ,
                                            [DarkImage] ,
                                            [Position] ,
                                            [Description] ,
                                            [IsEnable] ,
                                            [IsLight] ,
                                            [FootTile] ,
                                            [DescriptionTitle] ,
                                            [LightText] ,
                                            [DarkText] ,
                                            [MembershipsGradeId] ,
                                            [PermissionType] ,
                                            [EnabledVersion] ,
                                            [EndVersion] ,
                                            [CreateDatetime] ,
                                            [CreateBy] ,
                                            [LastUpdateDateTime] ,
                                            [LastUpdateBy] ,
                                            [IsDeleted] ,
                                            [CheckCycle] ,
                                            [IsLinkUrl] ,
                                            [LightUrl] ,
                                            [LightButtonUrl] ,
                                            [CardImage] ,
                                            [AndroidUrl] ,
                                            [IOSUrl] ,
                                            [DescriptionDetail] ,
                                            [IsClickReceive] ,
                                            [PromptTag]
                                        )
VALUES ( @Name ,
         @LightImage ,
         @DarkImage ,
         @Position ,
         @Description ,
         @IsEnable ,
         @IsLight ,
         @FootTile ,
         @DescriptionTitle ,
         @LightText ,
         @DarkText ,
         @MembershipsGradeId ,
         @PermissionType ,
         @EnabledVersion ,
         @EndVersion ,
         GETDATE(),
         @CreateBy ,
         GETDATE(),
         @LastUpdateBy ,
         0 ,
         @CheckCycle ,
         @IsLinkUrl ,
         @LightUrl ,
         @LightButtonUrl ,
         @CardImage ,
         @AndroidUrl ,
         @IOSUrl ,
         @DescriptionDetail ,
         @IsClickReceive ,
         @PromptTag
       );
";
            #endregion
            #region 参数化赋值
            sqlParamerts.Add(new SqlParameter("@Name", model.Name));
            sqlParamerts.Add(new SqlParameter("@LightImage", model.LightImage));
            sqlParamerts.Add(new SqlParameter("@DarkImage", model.DarkImage));
            sqlParamerts.Add(new SqlParameter("@Position", model.Position));
            sqlParamerts.Add(new SqlParameter("@Description", model.Description));
            sqlParamerts.Add(new SqlParameter("@IsEnable", model.IsEnable));
            sqlParamerts.Add(new SqlParameter("@IsLight", model.IsLight));
            sqlParamerts.Add(new SqlParameter("@FootTile", model.FootTile));
            sqlParamerts.Add(new SqlParameter("@DescriptionTitle", model.DescriptionTitle));
            sqlParamerts.Add(new SqlParameter("@LightText", model.LightText));
            sqlParamerts.Add(new SqlParameter("@DarkText", model.DarkText));
            sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", model.MembershipsGradeId));
            sqlParamerts.Add(new SqlParameter("@PermissionType", model.PermissionType));
            sqlParamerts.Add(new SqlParameter("@EnabledVersion", model.EnabledVersion));
            sqlParamerts.Add(new SqlParameter("@EndVersion", model.EndVersion));
            sqlParamerts.Add(new SqlParameter("@CreateBy", model.LastUpdateBy));
            sqlParamerts.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
            sqlParamerts.Add(new SqlParameter("@CheckCycle", model.CheckCycle));
            sqlParamerts.Add(new SqlParameter("@IsLinkUrl", model.IsLinkUrl));
            sqlParamerts.Add(new SqlParameter("@LightUrl", model.LightUrl));
            sqlParamerts.Add(new SqlParameter("@LightButtonUrl", model.LightButtonUrl));
            sqlParamerts.Add(new SqlParameter("@CardImage", model.CardImage));
            sqlParamerts.Add(new SqlParameter("@AndroidUrl", model.AndroidUrl));
            sqlParamerts.Add(new SqlParameter("@IOSUrl", model.IOSUrl));
            sqlParamerts.Add(new SqlParameter("@DescriptionDetail", model.DescriptionDetail));
            sqlParamerts.Add(new SqlParameter("@IsClickReceive", model.IsClickReceive));
            sqlParamerts.Add(new SqlParameter("@PromptTag", model.PromptTag));
            #endregion
            using (var cmd = new SqlCommand(strSql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 更新会员权益
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(UserPermissionModel model)
        {
            var sqlParamerts = new List<SqlParameter>();
            #region sql脚本
            var strSql = @"
UPDATE Gungnir..tbl_UserPermission WITH ( ROWLOCK )
SET    [Name] = @Name ,
       [LightImage] = @LightImage ,
       [DarkImage] = @DarkImage ,
       [Position] = @Position ,
       [Description] = @Description ,
       [IsEnable] = @IsEnable ,
       [IsLight] = @IsLight ,
       [FootTile] = @FootTile ,
       [DescriptionTitle] = @DescriptionTitle ,
       [LightText] = @LightText ,
       [DarkText] = @DarkText ,
       [MembershipsGradeId] = @MembershipsGradeId ,
       [PermissionType] = @PermissionType ,
       [EnabledVersion] = @EnabledVersion ,
       [EndVersion] = @EndVersion ,
       [LastUpdateDateTime] = @LastUpdateDateTime ,
       [LastUpdateBy] = @LastUpdateBy ,
       [CheckCycle] = @CheckCycle ,
       [IsLinkUrl] = @IsLinkUrl ,
       [LightUrl] = @LightUrl ,
       [LightButtonUrl] = @LightButtonUrl ,
       [AndroidUrl] = @AndroidUrl ,
       [IOSUrl] = @IOSUrl ,
       [DescriptionDetail] = @DescriptionDetail,
       [CardImage]=@CardImage,
       [IsClickReceive]=@IsClickReceive,
       [PromptTag]=@PromptTag
WHERE  Id = @Id
";
            #endregion
            #region 参数化赋值
            sqlParamerts.Add(new SqlParameter("@Name", model.Name));
            sqlParamerts.Add(new SqlParameter("@LightImage", model.LightImage));
            sqlParamerts.Add(new SqlParameter("@DarkImage", model.DarkImage));
            sqlParamerts.Add(new SqlParameter("@Position", model.Position));
            sqlParamerts.Add(new SqlParameter("@Description", model.Description));
            sqlParamerts.Add(new SqlParameter("@IsEnable", model.IsEnable));
            sqlParamerts.Add(new SqlParameter("@IsLight", model.IsLight));
            sqlParamerts.Add(new SqlParameter("@FootTile", model.FootTile));
            sqlParamerts.Add(new SqlParameter("@DescriptionTitle", model.DescriptionTitle));
            sqlParamerts.Add(new SqlParameter("@LightText", model.LightText));
            sqlParamerts.Add(new SqlParameter("@DarkText", model.DarkText));
            sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", model.MembershipsGradeId));
            sqlParamerts.Add(new SqlParameter("@PermissionType", model.PermissionType));
            sqlParamerts.Add(new SqlParameter("@EnabledVersion", model.EnabledVersion));
            sqlParamerts.Add(new SqlParameter("@EndVersion", model.EndVersion));
            sqlParamerts.Add(new SqlParameter("@LastUpdateDateTime", model.LastUpdateDateTime));
            sqlParamerts.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
            sqlParamerts.Add(new SqlParameter("@CheckCycle", model.CheckCycle));
            sqlParamerts.Add(new SqlParameter("@IsLinkUrl", model.IsLinkUrl));
            sqlParamerts.Add(new SqlParameter("@LightUrl", model.LightUrl));
            sqlParamerts.Add(new SqlParameter("@LightButtonUrl", model.LightButtonUrl));
            sqlParamerts.Add(new SqlParameter("@AndroidUrl", model.AndroidUrl));
            sqlParamerts.Add(new SqlParameter("@IOSUrl", model.IOSUrl));
            sqlParamerts.Add(new SqlParameter("@DescriptionDetail", model.DescriptionDetail));
            sqlParamerts.Add(new SqlParameter("@CardImage", model.@CardImage));
            sqlParamerts.Add(new SqlParameter("@IsClickReceive", model.@IsClickReceive));
            sqlParamerts.Add(new SqlParameter("@PromptTag", model.PromptTag));
            sqlParamerts.Add(new SqlParameter("@Id", model.Id));
            #endregion
            using (var cmd = new SqlCommand(strSql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 删除会员权益
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id)
        {
            string sql = "update Gungnir..tbl_UserPermission WITH(ROWLOCK) SET IsDeleted=1 where ID=@ID";
            using (var cmd = new SqlCommand(sql.ToString()))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static UserPermissionModel GetModelById(int id)
        {
            #region 获取数据脚本
            var strSql = @"
 SELECT top 1 
                  [Id] ,
                  [Name] ,
                  [LightImage] ,
                  [DarkImage] ,
                  [TopImage] ,
                  [Position] ,
                  [IsTopImage] ,
                  [UseUserLevel] ,
                  [Description] ,
                  [IsEnable] ,
                  [IsLight] ,
                  [FootTile] ,
                  [Version] ,
                  [DescriptionTitle] ,
                  [LightText] ,
                  [DarkText] ,
                  [MembershipsGradeId] ,
                  [PermissionType] ,
                  [EnabledVersion] ,
                  [EndVersion] ,
                  [CreateDatetime] ,
                  [CreateBy] ,
                  [LastUpdateDateTime] ,
                  [LastUpdateBy] ,
                  [IsDeleted] ,
                  [CheckCycle] ,
                  [IsLinkUrl] ,
                  [LightUrl] ,
                  [LightButtonUrl] ,
                  [CardImage] ,
                  [AndroidUrl] ,
                  [IOSUrl] ,
                  [DescriptionDetail] ,
                  [IsClickReceive] ,
                  [PromptTag]
           FROM   Gungnir.dbo.tbl_UserPermission WITH ( NOLOCK )
           WHERE  Id=@Id And IsDeleted=0  
            ";
            #endregion
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                var retsult= DbHelper.ExecuteDataTable(cmd)?.ConvertTo<UserPermissionModel>().ToList();
                if(retsult!=null && retsult.Count > 0)
                {
                    return retsult[0];
                }
                return null;
            }
        }

        #endregion


        #region 特价商品

        public static DataTable GetActivityProductList(string activityID)
        {
          
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                string sql = @" SELECT FSP.*,P.cy_list_price, FSP.ProductName as  DisplayName FROM Activity..tbl_FlashSaleProducts FSP LEFT JOIN Tuhu_productcatalog..vw_Products P ON FSP.PID=P.PID
                       WHERE FSP.ActivityID = @ActivityID   ";

                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                return dbhelper.ExecuteDataTable(cmd);
            }
         
        }

        public static bool AddByActivityProduct(UserPermissionActivityProduct model)
        {
            DeleteByActivityProductPID(model.ActivityID.ToString(), model.PID);

            string sql = @"INSERT INTO Activity.[dbo].[tbl_FlashSaleProducts]
           ([ActivityID]
             ,[PID]
           ,[Price]--促销价
           ,[TotalQuantity]--库存
           ,[MaxQuantity]--个人限购
           ,[CreateDateTime]
           ,[ProductName]--产品名称简称
           ,[IsUsePCode]--是否可以使用优惠券 true
           ,[Channel]--all  pc  app
            ,[FalseOriginalPrice]
          )
     VALUES
           (@ActivityID,@PID,@Price,@TotalQuantity,@MaxQuantity,getdate(),@ProductName,1,'all',@FalseOriginalPrice)";

            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                cmd.Parameters.AddWithValue("@PID", model.PID.Trim());
                cmd.Parameters.AddWithValue("@Price",model.Price);
                cmd.Parameters.AddWithValue("@TotalQuantity",model.TotalQuantity);
                cmd.Parameters.AddWithValue("@MaxQuantity",model.MaxQuantity);
                //  cmd.Parameters.AddWithValue("@SaleOutQuantity",model.SaleOutQuantity);
                cmd.Parameters.AddWithValue("@ProductName",model.DisplayName);
                cmd.Parameters.AddWithValue("@FalseOriginalPrice",model.FalseOriginalPrice);
                int i = dbhelper.ExecuteNonQuery(cmd);
                if (i > 0)
                    return true;
                else
                    return false;
            }
           
        }

        public static bool UpdateByActivityProduct(UserPermissionActivityProduct model)
        {
            string sql = @"update Activity.[dbo].[tbl_FlashSaleProducts] set [Price]=@Price , TotalQuantity=@TotalQuantity,MaxQuantity=@MaxQuantity
                                  ,ProductName=@ProductName,FalseOriginalPrice=@FalseOriginalPrice  where PKID=@PKID  ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@TotalQuantity",model.TotalQuantity);
                cmd.Parameters.AddWithValue("@MaxQuantity", model.MaxQuantity);
                cmd.Parameters.AddWithValue("@ProductName", model.DisplayName);
                cmd.Parameters.AddWithValue("@PKID",model.PKID);
                cmd.Parameters.AddWithValue("@FalseOriginalPrice",model.FalseOriginalPrice);
                int i = dbhelper.ExecuteNonQuery(cmd);
                if (i > 0)
                    return true;
                else
                    return false;
            }
        }

        private static bool DeleteByActivityProductPID(string activityID, string pid)
        {
            string[] array = { "8312FE9D-6DCE-4CD5-87D5-99CE064336A3", "650C6D05-51F8-46FB-A84F-F136D727A59B", "663543D4-5B62-4133-935D-F0A4C68751F8", "88C467DB-622E-4F02-89A5-F80405076599" };
            if (activityID != array[0] && activityID != array[1] && activityID != array[2] && activityID != array[3])
            {
                return false;
            }

            string sql = " DELETE FROM SystemLog.dbo.tbl_FlashSaleRecords WHERE ActivityID=@ActivityID AND PID=@PID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                cmd.Parameters.AddWithValue("@PID", pid);
                int i = db.ExecuteNonQuery(cmd);
                if (i > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 删除活动产品
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static bool DeleteByActivityProduct(string activityID,string pkid)
        {
            string[] array = { "8312FE9D-6DCE-4CD5-87D5-99CE064336A3", "650C6D05-51F8-46FB-A84F-F136D727A59B", "663543D4-5B62-4133-935D-F0A4C68751F8", "88C467DB-622E-4F02-89A5-F80405076599" };
            if (activityID != array[0] && activityID != array[1] && activityID != array[2] && activityID != array[3])
            {
                return false;
            }

            string sql = " DELETE FROM Activity..tbl_FlashSaleProducts WHERE ActivityID=@ActivityID AND PKID=@PKID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                int i = db.ExecuteNonQuery(cmd);
                if (i > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 获取活动商品信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static DataTable GetActivityProduct(string pkID)
        {
            string sql = @" SELECT FSP.*,P.cy_list_price,FSP.ProductName as  DisplayName FROM Activity..tbl_FlashSaleProducts FSP LEFT JOIN Tuhu_productcatalog..vw_Products P ON FSP.PID=P.PID
                       WHERE FSP.PKID = @pkID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@pkID", pkID);
                DataTable dt = db.ExecuteDataTable(cmd);
                return dt;
            }
        }

        #endregion


        #region 会员运费折扣

        public static bool AddUseTrans(tbl_UserTransportation model)
        {
            string sql = @"INSERT INTO  Gungnir.[dbo].[tbl_UserTransportation]
           ([Rank]
           ,[TransMoney]
           ,[Discount]
           ,[SaleMoney])
     VALUES
           (@Rank
           ,@TransMoney
           ,@Discount
           ,@SaleMoney)";

            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Rank", model.Rank);
                cmd.Parameters.AddWithValue("@TransMoney", model.TransMoney);
                cmd.Parameters.AddWithValue("@Discount", model.Discount);
                cmd.Parameters.AddWithValue("@SaleMoney",model.SaleMoney);
                return db.ExecuteNonQuery(cmd) > 0;
            }


               
        }


        public static bool UpdateTrans(tbl_UserTransportation model)
        {
            string sql = @" update  Gungnir.[dbo].[tbl_UserTransportation] set TransMoney=@TransMoney,Discount=@Discount,SaleMoney=@SaleMoney where [Rank]=@Rank ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Rank", model.Rank);
                cmd.Parameters.AddWithValue("@TransMoney", model.TransMoney);
                cmd.Parameters.AddWithValue("@Discount", model.Discount);
                cmd.Parameters.AddWithValue("@SaleMoney", model.SaleMoney);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }


        public static bool Exist(string rank)
        {
            string sql = "SELECT COUNT(1) FROM Gungnir.[dbo].[tbl_UserTransportation] WHERE [Rank]=@Rank ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Rank",rank);
                return (int)db.ExecuteScalar(cmd) > 0;
            }
        }


        public static DataTable GetUseTransMoney()
        {
            string sql = "SELECT * FROM Gungnir..tbl_UserTransportation WITH(NOLOCK) ORDER BY [RANK] ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var db = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return db.ExecuteDataTable(cmd);
            }
        }

        #endregion


        #region 升级任务

        /// <summary>
        /// 获取升级任务列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTaskList(string appType)
        {
            string sql = "SELECT * FROM Gungnir..tbl_UserTask WHERE APPType=@APPType ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@APPType", appType);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static bool AddTask(tbl_UserTask model)
        {
            string sql = "INSERT INTO  Gungnir..tbl_UserTask VALUES(@TaskName,@Description,@APPType,@APPHandler,@APPConnect)";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@TaskName", model.TaskName.Trim()??null);
                cmd.Parameters.AddWithValue("@Description", model.Description.Trim()??null);
                cmd.Parameters.AddWithValue("@APPType",model.APPType);
                cmd.Parameters.AddWithValue("@APPHandler",model.APPHandler.Trim()??null);
                cmd.Parameters.AddWithValue("@APPConnect",model.APPConnect.Trim()??null);
                return dbhelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool UpdateTask(tbl_UserTask model)
        {
            string sql = @"
UPDATE Gungnir..tbl_UserTask SET [Description]=@Description, APPType=@APPType,APPHandler=@APPHandler,APPConnect=@APPConnect
WHERE ID=@ID";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Description", model.Description.Trim()??null);
                cmd.Parameters.AddWithValue("@APPType", model.APPType);
                cmd.Parameters.AddWithValue("@APPHandler", model.APPHandler.Trim()??null);
                cmd.Parameters.AddWithValue("@APPConnect", model.APPConnect.Trim()??null);
                cmd.Parameters.AddWithValue("@ID",model.ID);
                return dbhelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static DataTable GetTask(string id)
        {
            string sql = "SELECT * FROM Gungnir..tbl_UserTask where ID=@ID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID",id);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static bool DeleteTask(string id)
        {
            string sql = "DELETE FROM  Gungnir..tbl_UserTask WHERE ID=@ID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return dbhelper.ExecuteNonQuery(cmd)>0;
            }
        }


        #endregion


        #region 会员优惠券
        public static DataTable GetPromotionList(string userRank)
        {
            string sql = "SELECT * FROM Gungnir..tbl_UserPromotioncode WHERE UserRank=@UserRank ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserRank", userRank);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static bool AddPromotion(tbl_UserPromotionCode model)
        {
            string sql = "INSERT INTO  Gungnir..tbl_UserPromotioncode VALUES(@SImage,@BImage,@RuleGuid,@UserRank,@CouponName,@CouponDescription)";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SImage", model.SImage);
                cmd.Parameters.AddWithValue("@BImage", model.BImage);
                cmd.Parameters.AddWithValue("@RuleGuid", model.RuleGuid);
                cmd.Parameters.AddWithValue("@UserRank", model.UserRank);
                cmd.Parameters.AddWithValue("@CouponName", model.CouponName);
                cmd.Parameters.AddWithValue("@CouponDescription",model.CouponDescription);
                return dbhelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool UpdatePromotion(tbl_UserPromotionCode model)
        {
            string sql = @"
UPDATE Gungnir..tbl_UserPromotioncode SET [SImage]=@SImage, BImage=@BImage,RuleGuid=@RuleGuid,UserRank=@UserRank,CouponName=@CouponName,CouponDescription=@CouponDescription
WHERE ID=@ID";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SImage", model.SImage);
                cmd.Parameters.AddWithValue("@BImage", model.BImage);
                cmd.Parameters.AddWithValue("@RuleGuid", model.RuleGuid);
                cmd.Parameters.AddWithValue("@UserRank", model.UserRank);
                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@CouponName",model.CouponName);
                cmd.Parameters.AddWithValue("@CouponDescription",model.CouponDescription);
                return dbhelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static DataTable GetPromotion(string id)
        {
            string sql = "SELECT * FROM Gungnir..tbl_UserPromotioncode where ID=@ID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static bool DeletePromotion(string id)
        {
            string sql = "DELETE FROM  Gungnir..tbl_UserPromotioncode WHERE ID=@ID ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return dbhelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        #endregion

    }
}

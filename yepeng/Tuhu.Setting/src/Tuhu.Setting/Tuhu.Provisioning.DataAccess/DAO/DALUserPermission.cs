using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALUserPermission
    {
        #region 会员特权
        public static DataTable SelectAllUserPermission()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("SELECT * FROM Gungnir..tbl_UserPermission AS UP  WITH(NOLOCK) ORDER BY  UP.Version DESC, id DESC ");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd);
            }
        }


        public static int GetRowCount()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("SELECT count(*) FROM Gungnir..tbl_UserPermission AS UP");
                cmd.CommandType = CommandType.Text;
                object obj = dbhelper.ExecuteScalar(cmd);
                int result = 0;
                int.TryParse(obj.ToString(), out result);
                return result;
            }
        }

        public static DataTable SelectUserPermissionByPage(int page, int pageSize)
        {
            string sql = @" select [Id]
      ,[Name]
      ,[LightImage]
      ,[DarkImage]
      ,[TopImage]
      ,[Position]
      ,[IsTopImage]
      ,[UseUserLevel]
      ,[Description]
      ,[IsEnable]
      ,[IsLight]
    ,[FootTile]
     from (
select ROW_NUMBER()over(order by [Version] DESC, id DESC ) as rownumber, * from tbl_UserPermission   ) p
where rownumber >(@pageSize*(@Page-1)) and rownumber <= (@pageSize*@Page) ORDER BY  [Version] DESC, id DESC  ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }


        public static int AddUserPermission(UserPermissionModel model)
        {
            string sql = @"insert into Gungnir..tbl_UserPermission([Name]
                           ,[LightImage]
                           ,[DarkImage]
                           ,[TopImage]
                           ,[Position]
                           ,[IsTopImage]
                           ,[UseUserLevel]
                           ,[Description],[IsEnable],[IsLight],[FootTile],[Version]) 
                         values (@Name,@LightImage,@DarkImage,@TopImage,@Position,@IsTopImage,@UseUserLevel,@Description,@IsEnable,@IsLight,@FootTile,@Version) ";

            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@LightImage", model.LightImage);
                cmd.Parameters.AddWithValue("@DarkImage", model.DarkImage);
                cmd.Parameters.AddWithValue("@TopImage", model.TopImage);
                cmd.Parameters.AddWithValue("@Position", model.Position);
                cmd.Parameters.AddWithValue("@IsTopImage", model.IsTopImage);
                cmd.Parameters.AddWithValue("@UseUserLevel", model.UseUserLevel);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@IsEnable",model.IsEnable);
                cmd.Parameters.AddWithValue("@IsLight",model.IsLight);
                cmd.Parameters.AddWithValue("@FootTile",model.FootTile);
                cmd.Parameters.AddWithValue("@Version",model.Version);
                return dbhelper.ExecuteNonQuery(cmd);
            }

        }


        public static int UpdateUserPermission(UserPermissionModel model)
        {
            string sql = @"update Gungnir..tbl_UserPermission set Name=@Name,LightImage=@LightImage,DarkImage=@DarkImage,
                        TopImage=@TopImage,Position=@Position,IsTopImage=@IsTopImage,UseUserLevel=@UseUserLevel,Description=@Description,IsEnable=@IsEnable,IsLight=@IsLight,FootTile=@FootTile,[Version]=@Version
                           where ID=@ID";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@LightImage", model.LightImage);
                cmd.Parameters.AddWithValue("@DarkImage", model.DarkImage);
                cmd.Parameters.AddWithValue("@TopImage", model.TopImage);
                cmd.Parameters.AddWithValue("@Position", model.Position);
                cmd.Parameters.AddWithValue("@IsTopImage", model.IsTopImage);
                cmd.Parameters.AddWithValue("@UseUserLevel", model.UseUserLevel);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@IsEnable", model.IsEnable);
                cmd.Parameters.AddWithValue("@IsLight", model.IsLight);
                cmd.Parameters.AddWithValue("@FootTile", model.FootTile);
                cmd.Parameters.AddWithValue("@ID", model.Id);
                cmd.Parameters.AddWithValue("@Version",model.Version);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }


        public static UserPermissionModel GetUserPermission(string id)
        {
            string sql = "select top 1 * from Gungnir..tbl_UserPermission where ID=@id";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);
                DataTable dt = dbhelper.ExecuteDataTable(cmd);
                if (dt != null && dt.Rows.Count > 0)
                    return new UserPermissionModel(dt.Rows[0]);
                else
                    return new UserPermissionModel();
            }

        }

        public static int Delete(string id)
        {
            string sql = "delete from Gungnir..tbl_UserPermission where ID=@ID";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return dbhelper.ExecuteNonQuery(cmd);
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

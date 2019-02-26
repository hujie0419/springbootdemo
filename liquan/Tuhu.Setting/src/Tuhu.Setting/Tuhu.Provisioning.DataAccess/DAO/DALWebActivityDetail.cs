using Microsoft.ApplicationBlocks.Data;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALWebActivityDetail
    {
        /// <summary>
        /// 获取活动基本信息
        /// </summary>
        /// <param name="ActiveID"></param>
        /// <returns></returns>
        public static DataTable FetchWebActivityDetail(string ActiveID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM	HuoDong..tbl_WebActivity AS WA LEFT JOIN HuoDong..tbl_WebAct_CommodityFloors AS WACF ON WA.ActiveID = WACF.ActiveID  where WA.ActiveID=@ActiveID ORDER BY WACF.FloorID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", ActiveID);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="ActiveID"></param>
        /// <returns></returns>
        public static int DeleteWebActivity(string ActiveID, int Type)
        {
            if (Type == 1)
            {
                var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new SqlDbHelper(conn))
                {
                    var cmd = new SqlCommand("[HuoDong].dbo.[DeleteWebActivityDetail]");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ActiveID", ActiveID);
                    return dbhelper.ExecuteNonQuery(cmd);
                }
            }
            else
            {
                IConnectionManager cm = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
                using (var conn = cm.OpenConnection())
                {
                    var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, "UPDATE Activity..tbl_FlashSale SET  WebBackground=NULL ,WebCornerMark=NULL,WebBanner=NULL WHERE ActivityID=@ActivityID", new SqlParameter("@ActivityID", ActiveID));
                    cm.CloseConnection();
                    return result;
                }
            }
        }

        /// <summary>
        /// 获取所有活动的基本信息
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllWebActivity()
        {
            var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM	HuoDong..tbl_WebActivity ORDER BY ActiveID ");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 获取每一楼层的产品信息
        /// </summary>
        /// <param name="ActiveID"></param>
        /// <param name="FloorID"></param>
        /// <returns></returns>
        public static DataTable FetchProductsForFloor(string ActiveID, int FloorID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"Select * From [HuoDong].[dbo].[tbl_WebAct_Products] with (nolock) where  ActiveID=@ActiveID AND FloorID=@FloorID ORDER BY FloorID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", ActiveID);
                cmd.Parameters.AddWithValue("@FloorID", FloorID);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
        /// <summary>
        /// 获取每次活动的其他部分链接的信息  PartID为1表示是底部链接
        /// </summary>
        /// <param name="ActiveID"></param>
        /// <returns></returns>
        public static DataTable FetchOtherPartForFloor(string ActiveID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"Select * From [HuoDong].[dbo].[tbl_WebAct_OtherPart] with (nolock) where  ActiveID=@ActiveID  ORDER BY orderID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", ActiveID);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
        /// <summary>
        /// 获取最新活动的期数
        /// </summary>
        /// <returns></returns>
        public static string GetLastActiveID()
        {
            var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT TOP(1) ActiveID FROM HuoDong..tbl_WebActivity AS WA WITH(NOLOCK) ORDER BY CONVERT(INT,WA.ActiveID) DESC ");
                cmd.CommandType = CommandType.Text;
                var result = dbhelper.ExecuteScalar(cmd);
                if (result == null)
                    result = "0";
                return result.ToString();
            }
        }

        /// <summary>
        /// 更新或新增之前判断该活动是否存在
        /// </summary>
        /// <returns></returns>
        public static int isExist(SqlDbHelper dbhelper, string ActiveID)
        {
            using (var cmd = new SqlCommand(@"SELECT *  FROM HuoDong..tbl_WebActivity AS WA WITH(NOLOCK) Where WA.ActiveID=@ActiveID "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", ActiveID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 新增活动基本信息
        /// </summary>
        /// <param name="wa"></param>
        /// <returns></returns>
        public static int InsertWebActive(SqlDbHelper dbhelper, WebActive wa)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO HuoDong..tbl_WebActivity
				( ActiveID,
				  ActiveName,
				  ActiveLink,
				  ActiveDescription,
				  Banner,
				  CornerMark,
				  backgroundColor,
				  StartDateTime,
				  EndDateTime,
				  CreateDateTime
				)
				VALUES	(@ActiveID, @ActiveName, @ActiveLink,@ActiveDescription ,@Banner, @CornerMark, @backgroundColor, @StartDateTime,@EndDateTime,GETDATE())"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", wa.ActiveID);
                cmd.Parameters.AddWithValue("@ActiveName", wa.ActiveName);
                cmd.Parameters.AddWithValue("@ActiveLink ", wa.ActiveLink);
                cmd.Parameters.AddWithValue("@ActiveDescription", wa.ActiveDescription);
                cmd.Parameters.AddWithValue("@Banner", wa.Banner);
                cmd.Parameters.AddWithValue("@CornerMark", wa.CornerMark);
                cmd.Parameters.AddWithValue("@backgroundColor", wa.backgroundColor);
                cmd.Parameters.AddWithValue("@StartDateTime", wa.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", wa.EndDateTime);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 新增楼层
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public static int InsertCommodifyFloor(SqlDbHelper dbhelper, CommodityFloor cf)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO HuoDong..tbl_WebAct_CommodityFloors
				( ActiveID,
				  FloorID,
				  FloorPicture,
				  FloorLink
				)
				VALUES	(@ActiveID, @FloorID, @FloorPicture ,  @FloorLink)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", cf.ActiveID);
                cmd.Parameters.AddWithValue("@FloorID", cf.FloorID);
                cmd.Parameters.AddWithValue("@FloorPicture ", cf.FloorPicture);
                cmd.Parameters.AddWithValue("@FloorLink", cf.FloorLink);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 新增其他部分链接
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int InsertOtherPart(SqlDbHelper dbhelper, OtherPart op)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO HuoDong..tbl_WebAct_OtherPart
				( ActiveID,
				  PartID,
				  Picture,
				  PartLink,
				  orderID
				)
				VALUES (@ActiveID, @PartID, @Picture, @PartLink,@orderID)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", op.ActiveID);
                cmd.Parameters.AddWithValue("@PartID", op.PartID);
                cmd.Parameters.AddWithValue("@Picture ", op.Picture);
                cmd.Parameters.AddWithValue("@PartLink", op.PartLink);
                cmd.Parameters.AddWithValue("@orderID", op.orderID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 新增楼层商品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>

        public static int InsertProducts(SqlDbHelper dbhelper, Products product)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO HuoDong..tbl_WebAct_Products
				( FloorID,
				  ActiveID,
				  ProductID,
				  VariantID,
				  orderID
				)
				VALUES (@FloorID, @ActiveID, @ProductID,@VariantID,@orderID)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActiveID", product.ActiveID);
                cmd.Parameters.AddWithValue("@FloorID", product.FloorID);
                cmd.Parameters.AddWithValue("@ProductID ", product.ProductID);
                cmd.Parameters.AddWithValue("@VariantID", product.VariantID);
                cmd.Parameters.AddWithValue("@orderID", product.orderID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 新增活动/或更新活动
        /// </summary>
        /// <param name="webact"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int InsertWebActivityDetail(WebActive webact, string action)
        {
            var result = 1;
            var conn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                if (action == "更新")
                {
                    result = DeleteWebActivity(webact.ActiveID, 1);
                }

                if (isExist(dbhelper, webact.ActiveID) <= 0 && result > 0)
                {
                    result = InsertWebActive(dbhelper, webact);
                    if (result > 0)
                    {
                        foreach (var cf in webact.CommodifyFloor)
                        {
                            result = InsertCommodifyFloor(dbhelper, cf);
                            if (result > 0)
                            {
                                foreach (var p in cf.Products)
                                {
                                    result = InsertProducts(dbhelper, p);
                                    if (result <= 0)
                                    {
                                        result = -97;//产品插入失败
                                        dbhelper.Rollback();
                                        return result;
                                    }
                                }
                            }
                            else
                            {
                                result = -98;//商品楼层插入失败
                                dbhelper.Rollback();
                                return result;
                            }
                        }
                        if (webact.OtherPart != null && webact.OtherPart.Count() > 0)
                        {
                            foreach (var op in webact.OtherPart)
                            {
                                result = InsertOtherPart(dbhelper, op);
                                if (result <= 0)
                                {
                                    result = -96;//底部链接插入失败
                                    dbhelper.Rollback();
                                    return result;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = -99;//活动基本信息插入失败
                        dbhelper.Rollback();
                        return result;
                    }
                }
                else
                {
                    result = -1;//更新失败
                    dbhelper.Rollback();
                    return result;
                }

                dbhelper.Commit();
                return result;
            }
        }
    }
}

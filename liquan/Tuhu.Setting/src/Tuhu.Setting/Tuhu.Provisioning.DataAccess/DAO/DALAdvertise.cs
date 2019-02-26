using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALAdvertise
    {
        #region Advertise
        /// <summary>
        /// 获取所有Advertise
        /// </summary>
        public static List<Advertise> GetAllAdvertise(SqlConnection connection)
        {
            var sql = "SELECT * FROM tbl_Advertise WITH (NOLOCK) ORDER BY State DESC, adcolumnid,(case when (GETDATE()>BeginDateTime and GETDATE()<DATEADD(day,1, EndDateTime)) then 0 else (case when GETDATE()<BeginDateTime then 1 else 2 end) end),EndDateTime DESC";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<Advertise>().ToList();
        }
        /// <summary>
        /// 删除Advertise
        /// </summary>
        /// <param name="id">PKID</param>
        public static void DeleteAdvertise(SqlConnection connection, int id)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",id)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM tbl_Advertise WHERE PKID=@PKID;DELETE FROM tbl_AdProduct WHERE AdvertiseID=@PKID", sqlParamters);
        }
        /// <summary>
        /// 添加Advertise
        /// </summary>
        /// <param name="advertise">Advertise对象</param>
        public static void AddAdvertise(SqlConnection connection, Advertise advertise)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@AdColumnID",advertise.AdColumnID),
                new SqlParameter("@Name",advertise.Name==null?"":advertise.Name),
                new SqlParameter("@Position",advertise.Position),
                new SqlParameter("@BeginDateTime",advertise.BeginDateTime.HasValue?advertise.BeginDateTime.Value:DateTime.Parse("1900-01-01")),
                new SqlParameter("@EndDateTime",advertise.EndDateTime.HasValue?advertise.EndDateTime.Value:DateTime.Parse("2020-01-01")),
                new SqlParameter("@Image",advertise.Image==null?"":advertise.Image),
                new SqlParameter("@Url",advertise.Url==null?"":advertise.Url),
                new SqlParameter("@ActivityID",advertise.ActivityID==null?"":advertise.ActivityID),
                new SqlParameter("@ShowType",advertise.ShowType),
                new SqlParameter("@State",advertise.State),
                new SqlParameter("@CreateDateTime",advertise.CreateDateTime),
                new SqlParameter("@LastUpdateDateTime",advertise.LastUpdateDateTime),//默认为0
                new SqlParameter("@Platform",advertise.Platform),//0或NULL：所有；1：PC；2：微信；4：Android；8：IOS
                new SqlParameter("@FunctionID",advertise.FunctionID==null?"":advertise.FunctionID),
                new SqlParameter("@TopPicture",advertise.TopPicture==null?"":advertise.TopPicture),
                new SqlParameter("@AdType",advertise.AdType==null?0:advertise.AdType),
                new SqlParameter("@ProductID",advertise.ProductID==null?"":advertise.ProductID),
                new SqlParameter("@IsSendStamps",advertise.IsSendStamps),
                new SqlParameter("@ActivityKey",advertise.ActivityKey==null?"":advertise.ActivityKey)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into tbl_Advertise(AdColumnID,Name,Position,BeginDateTime,EndDateTime,Image,Url,ActivityID,ShowType,State,CreateDateTime,LastUpdateDateTime,Platform,FunctionID,TopPicture,AdType,ProductID,IsSendStamps,ActivityKey) values
                 (@AdColumnID,@Name,@Position,@BeginDateTime,@EndDateTime,@Image,@Url,@ActivityID,@ShowType,@State,@CreateDateTime,@LastUpdateDateTime,@Platform,@FunctionID,@TopPicture,@AdType,@ProductID,@IsSendStamps,@ActivityKey)"
                , sqlParamters);
        }
        /// <summary>
        /// 修改Advertise
        /// </summary>
        /// <param name="advertise">Advertise对象</param>
        public static void UpdateAdvertise(SqlConnection connection, Advertise advertise)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",advertise.PKID),
                new SqlParameter("@Name",advertise.Name==null?"":advertise.Name),
                new SqlParameter("@Position",advertise.Position),
                new SqlParameter("@BeginDateTime",advertise.BeginDateTime),
                new SqlParameter("@EndDateTime",advertise.EndDateTime),
                new SqlParameter("@Image",advertise.Image==null?"":advertise.Image),
                new SqlParameter("@Url",advertise.Url==null?"":advertise.Url),
                new SqlParameter("@ActivityID",advertise.ActivityID==null?"":advertise.ActivityID),
                new SqlParameter("@ShowType",advertise.ShowType),
                new SqlParameter("@State",advertise.State),//默认为0
                new SqlParameter("@LastUpdateDateTime",advertise.LastUpdateDateTime),//默认为0
                new SqlParameter("@Platform",advertise.Platform),//0或NULL：所有；1：PC；2：微信；4：Android；8：IOS
                new SqlParameter("@FunctionID",advertise.FunctionID==null?"":advertise.FunctionID),
                new SqlParameter("@TopPicture",advertise.TopPicture==null?"":advertise.TopPicture),
                new SqlParameter("@AdType",advertise.AdType==null?0:advertise.AdType),
                new SqlParameter("@ProductID",advertise.ProductID==null?"":advertise.ProductID),
                new SqlParameter("@IsSendStamps",advertise.IsSendStamps),
                new SqlParameter("@ActivityKey",advertise.ActivityKey==null?"":advertise.ActivityKey)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"update tbl_Advertise set Name=@Name,Position=@Position,BeginDateTime=@BeginDateTime,EndDateTime=@EndDateTime
                    ,Image=@Image,Url=@Url,ActivityID=@ActivityID,ShowType=@ShowType,State=@State,LastUpdateDateTime=@LastUpdateDateTime,Platform=@Platform,FunctionID=@FunctionID,
                   TopPicture=@TopPicture,AdType=@AdType,ProductID=@ProductID,IsSendStamps=@IsSendStamps,ActivityKey=@ActivityKey where PKID=@PKID"
                , sqlParamters);
        }
        /// <summary>
        /// 根据id获取Advertise对象
        /// </summary>
        /// <param name="advertise">Advertise对象</param>
        public static Advertise GetAdvertiseByID(SqlConnection connection, int id)
        {
            Advertise _Advertise = null;
            var parameters = new[]
            {
                new SqlParameter("@PKID", id)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
                    PKID,  
                    AdColumnID,
		            Name,
		            Position,
		            BeginDateTime,
		            EndDateTime,
		            Image,
		            Url,
		            ShowType,
		            State,
		            CreateDateTime,
		            LastUpdateDateTime,
		            Platform,
		            FunctionID,
		            TopPicture,
		            AdType,
		            ProductID,
                    ActivityID,
		            ISNULL(IsSendStamps,0) AS IsSendStamps,
		            ActivityKey
            FROM Gungnir..tbl_Advertise WHERE PKID=@PKID", parameters))
            {
                if (_DR.Read())
                {
                    _Advertise = new Advertise();
                    _Advertise.PKID = _DR.GetTuhuValue<int>(0);
                    _Advertise.AdColumnID = _DR.GetTuhuString(1);
                    _Advertise.Name = _DR.GetTuhuString(2);
                    _Advertise.Position = _DR.GetTuhuValue<byte>(3);
                    _Advertise.BeginDateTime = _DR.GetTuhuValue<System.DateTime>(4);
                    _Advertise.EndDateTime = _DR.GetTuhuValue<System.DateTime>(5);
                    _Advertise.Image = _DR.GetTuhuString(6);
                    _Advertise.Url = _DR.GetTuhuString(7);
                    _Advertise.ShowType = _DR.GetTuhuValue<int>(8);
                    _Advertise.State = _DR.GetTuhuValue<byte>(9);
                    _Advertise.CreateDateTime = _DR.GetTuhuValue<System.DateTime>(10);
                    _Advertise.LastUpdateDateTime = _DR.GetTuhuValue<System.DateTime>(11);
                    _Advertise.Platform = _DR.GetTuhuValue<int>(12);
                    _Advertise.FunctionID = _DR.GetTuhuString(13);
                    _Advertise.TopPicture = _DR.GetTuhuString(14);
                    _Advertise.AdType = _DR.GetTuhuValue<int>(15);
                    _Advertise.ProductID = _DR.GetTuhuString(16);
                    _Advertise.ActivityID = _DR.GetTuhuString(17);
                    _Advertise.IsSendStamps = _DR.GetTuhuValue<bool>(18);
                    _Advertise.ActivityKey = _DR.GetTuhuString(19);
                }
            }
            return _Advertise;
        }
        public static bool IsExistsxAdColumnID(SqlConnection connection, string AdColumnID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@AdColumnID",AdColumnID),
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text,
                @"SELECT COUNT(1) FROM tbl_Advertise  WHERE AdColumnID=@AdColumnID"
                , sqlParamters)) <= 0;
        }
        #endregion
        #region AdProduct
        /// <summary>
        /// 获取模块下面的所有产品
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<AdProduct> GetAdProListByAdID(SqlConnection connection, int AdvertiseID)
        {
            var parameters = new[]
            {
                new SqlParameter("@AdvertiseID", AdvertiseID)
            };
            var sql = "SELECT * FROM tbl_AdProduct WITH (NOLOCK) where AdvertiseID=@AdvertiseID ORDER BY State desc,Position";
            //update by renyingqiang  (原因：默认的DbHelper 读取数据库为Gungnir ，但是本站点已经加密连接字符串，DbHelper没添加解密过程)
            //return DbHelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<AdProduct>().ToList();
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, parameters).ConvertTo<AdProduct>().ToList();
        }
        public static string GetCountByAdID(SqlConnection connection, int AdvertiseID)
        {
            var sqlParameter = new SqlParameter("@AdvertiseID", AdvertiseID);
            var _Count = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT COUNT(PID) FROM tbl_AdProduct where AdvertiseID=@AdvertiseID", sqlParameter);
            return _Count == null ? "0" : _Count.ToString();
        }

        public static void DeleteAdProduct(SqlConnection connection, int AdvertiseID, string PID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@AdvertiseID",AdvertiseID),
                new SqlParameter("@PID",PID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM tbl_AdProduct WHERE AdvertiseID=@AdvertiseID and PID=@PID", sqlParamters);
        }
        public static void ChangeState(SqlConnection connection, int AdvertiseID, string PID, byte State)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@AdvertiseID",AdvertiseID),
                new SqlParameter("@PID",PID),
                new SqlParameter("@State",State==0?1:0)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "update tbl_AdProduct set State=@State where AdvertiseID=@AdvertiseID and PID=@PID", sqlParamters);
        }
        public static string GetProductNameByPID(SqlConnection connection, string PID)
        {
            var sqlParameter = new SqlParameter("@PID", PID);
            var _PANME = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT DisplayName FROM Tuhu_productcatalog..[CarPAR_zh-CN] where PID =@PID", sqlParameter);
            return _PANME == null ? "" : _PANME.ToString();
        }

        public static Dictionary<string, string> GetProductNamesByPids(SqlConnection connection,
            IEnumerable<string> pids)
        {
            var sqlParameter = new SqlParameter("@PIDS", string.Join(",",pids));
            var queryResult = SqlHelper.ExecuteDataTable(connection, CommandType.Text, "SELECT PID,DisplayName FROM Tuhu_productcatalog..[CarPAR_zh-CN] where PID IN(SELECT Item FROM Tuhu_productcatalog..SplitString(@PIDS,',',1))", sqlParameter);
            var result = new Dictionary<string, string>();
            foreach (DataRow row in queryResult.Rows)
            {
                result[row["PID"].ToString()] = row["DisplayName"].ToString();
            }
            return result;
        }

        public static string GetCateNameByCateID(SqlConnection connection, string CateID)
        {
            var sqlParameter = new SqlParameter("@CateID", CateID);
            var _PANME = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT ZC.DisplayName FROM	Tuhu_productcatalog..[CarPAR_zh-CN] AS ZC WHERE ZC.CategoryName=@CateID", sqlParameter);
            return _PANME == null ? "" : _PANME.ToString();
        }

        /// <summary>
        /// 根据PID获取产品相关信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static DataTable GetProductInfoByPID(SqlConnection connection, string PID)
        {
            string sql = "select TOP 1 cy_list_price,displayname from Tuhu_productcatalog.[dbo].[CarPAR_zh-CN] WITH (NOLOCK) WHERE PID=@PID";
            var sqlParameter = new SqlParameter("@PID", PID);
            var resultData = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlParameter);
            if (resultData != null && resultData.Rows.Count > 0)
                return resultData;
            else
                return null;
        }
        public static void UpdateAdProduct(SqlConnection connection, int AdvertiseID, string PID, string NewPID, byte Position, decimal PromotionPrice, int PromotionNum)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@AdvertiseID",AdvertiseID),
                new SqlParameter("@PID",PID),
                new SqlParameter("@NewPID",NewPID),
                new SqlParameter("@Position",Position),
                new SqlParameter("@PromotionPrice",PromotionPrice),
                new SqlParameter("@PromotionNum",PromotionNum)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "UPDATE tbl_AdProduct SET PID=@NewPID,Position=@Position,PromotionPrice=@PromotionPrice,PromotionNum=@PromotionNum WHERE AdvertiseID=@AdvertiseID and PID=@PID", sqlParamters);
        }

        public static void AddAdProduct(SqlConnection connection, int AdvertiseID, string PID, byte Position, byte State, decimal PromotionPrice, int PromotionNum)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@AdvertiseID",AdvertiseID),
                new SqlParameter("@PID",PID),
                new SqlParameter("@Position",Position),
                new SqlParameter("@State",State),
                new SqlParameter("@PromotionPrice",PromotionPrice),
                new SqlParameter("@PromotionNum",PromotionNum)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into tbl_AdProduct(AdvertiseID,PID,Position,State,CreateDateTime,LastUpdateDateTime,PromotionPrice,PromotionNum) values
                 (@AdvertiseID,@PID,@Position,@State,GETDATE(),GETDATE(),@PromotionPrice,@PromotionNum)"
                , sqlParamters);
        }
        #endregion


        /// <summary>
        /// 通过产品id获取产品的信息 新版本
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static DataTable GetProductInfoByPIdNewVersion(SqlConnection connection, string PID)
        {
            string sql = @"select TOP 1 cy_list_price,displayname from Tuhu_productcatalog.[dbo].[CarPAR_zh-CN] WITH (NOLOCK) 
                            WHERE PID=@PID AND OnSale = 1 AND stockout = 0 AND i_ClassType IN (2,4)";
            var sqlParameter = new SqlParameter("@PID", PID);
            var resultData = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlParameter);
            if (resultData != null && resultData.Rows.Count > 0)
                return resultData;
            else
                return null;
        }
    }
}
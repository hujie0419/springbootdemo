using System;
using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Dapper;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALStarRatingStore
    {
        #region 工厂店投放

        /// <summary>
        /// 工厂店投放-获取某个时间段的工厂店列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static List<StarRatingStoreModel> GetStarRatingStoreList(out int recordCount, string startTime, string endTime, int pageSize = 10, int pageIndex = 1)
        {
            string sql = @"SELECT [PKID]
                  ,[UserName]
                  ,[Phone]
                  ,[StoreName]
                  ,[Duty]
                  ,[ProvinceID]
                  ,[CityID]
                  ,[DistrictID]
                  ,[ProvinceName]
                  ,[CityName]
                  ,[DistrictName]
                  ,[StoreAddress]
                  ,[Area]
                  ,[StoreArea]
                  ,[StoreNum]
                  ,[WorkPositionNum]
                  ,[MaintainQualification]
                  ,[Storefront]
                  ,[StorefrontDesc]
                  ,[StoreLocation]
                  ,[IsAgree]
                  ,[CreateDateTime]
                  ,[LastUpdateDateTime]
              FROM [Activity].[dbo].[StarRatingStore] WITH ( NOLOCK ) where 1=1";
            string sqlCount = @"SELECT COUNT(1) FROM [Activity].[dbo].[StarRatingStore] WITH(NOLOCK)  WHERE 1=1  ";
            if (string.IsNullOrWhiteSpace(startTime) && string.IsNullOrWhiteSpace(endTime))
            {
                startTime = DateTime.Now.ToString().Split(' ')[0];
                endTime = DateTime.Now.ToString().Split(' ')[0] + " 23:59:59";
            }
            else if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
            {
                startTime = startTime.Split(' ')[0];
                endTime = endTime.Split(' ')[0] + " 23:59:59";
            }
            sql += " and CreateDateTime between @startTime and @endTime order by CreateDateTime  desc OFFSET (@pageIndex -1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY";
            sqlCount += " and CreateDateTime between @startTime and @endTime";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PageSize", pageSize);
                dp.Add("@PageIndex", pageIndex);
                dp.Add("@startTime", startTime);
                dp.Add("@endTime", endTime);
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@startTime", startTime);
                parameters[1] = new SqlParameter("@endTime", endTime);
                recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, parameters);
                return conn.Query<StarRatingStoreModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 工厂店投放-查看详情
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static StarRatingStoreModel GetStarRatingStoreModel(int PKID)
        {
            string sql = @"SELECT [PKID]
                  ,[UserName]
                  ,[Phone]
                  ,[StoreName]
                  ,[Duty]
                  ,[ProvinceID]
                  ,[CityID]
                  ,[DistrictID]
                  ,[ProvinceName]
                  ,[CityName]
                  ,[DistrictName]
                  ,[StoreAddress]
                  ,[StoreArea]
                  ,[StoreNum]
                  ,[WorkPositionNum]
                  ,[MaintainQualification]
                  ,[Storefront]
                  ,[StorefrontDesc]
                  ,[StoreLocation]
                  ,[IsAgree]
                  ,[CreateDateTime]
                  ,[LastUpdateDateTime]
              FROM [Activity].[dbo].[StarRatingStore] WITH ( NOLOCK ) where PKID=@PKID";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PKID", PKID);
                return conn.Query<StarRatingStoreModel>(sql, dp).ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// 工厂店投放-Excel导出
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<StarRatingStoreModel> GetStarList(string startTime, string endTime)
        {
            string sql = @"SELECT 
                  [UserName]
                  ,[Phone]
                  ,[StoreName]
                  ,[Duty]
                  ,[ProvinceName]
                  ,[CityName]
                  ,[DistrictName]
                  ,[StoreAddress]
                  ,[Area]
                  ,[StoreArea]
                  ,[StoreNum]
                  ,[WorkPositionNum]
                  ,[MaintainQualification]
                  ,[Storefront]
                  ,[StorefrontDesc]
                  ,[StoreLocation]
                  ,[IsAgree]
                  ,[CreateDateTime]
                  ,[LastUpdateDateTime]
              FROM [Activity].[dbo].[StarRatingStore] WITH ( NOLOCK ) where CreateDateTime between @startTime and @endTime order by CreateDateTime  desc";
            if (string.IsNullOrWhiteSpace(startTime) && string.IsNullOrWhiteSpace(endTime))
            {
                startTime = DateTime.Now.ToString().Split(' ')[0];
                endTime = DateTime.Now.ToString().Split(' ')[0] + " 23:59:59";
            }
            else if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
            {
                startTime = startTime.Split(' ')[0];
                endTime = endTime.Split(' ')[0] + " 23:59:59";
            }
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@startTime", startTime);
                dp.Add("@endTime", endTime);
                return conn.Query<StarRatingStoreModel>(sql, dp).ToList();
            }
        }
        #endregion
    }
}

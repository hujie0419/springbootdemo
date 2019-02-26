using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalVendorProductPriorityConfig
    {
        /// <summary>
        /// 按地区查询品牌优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="districtId"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public Tuple<List<VendorProductPriorityConfigModel>, int> SearchVendorProductPriorityConfig
            (SqlConnection conn, string productType, string configType, List<int> regionIds, PagerModel pager)
        {
            #region sql
            const string sql = @"SELECT  @TotalCount = COUNT(1)
                                FROM    Configuration.dbo.VendorProductPriorityConfig AS ca WITH ( NOLOCK )
                                        INNER JOIN Configuration..SplitString(@RegionIds, ',', 1) AS r ON r.Item = ca.RegionId
                                WHERE   ca.ProductType = @ProductType
                                        AND ca.ConfigType = @ConfigType
                                        AND ca.IsDeleted = 0;
                                SELECT  ca.PKID ,
                                        ca.ProductType ,
                                        ca.ConfigType ,
                                        ca.VehicleId ,
                                        ca.RegionId ,
                                        ca.Priority ,
                                        ca.IsEnabled
                                FROM    Configuration.dbo.VendorProductPriorityConfig AS ca WITH ( NOLOCK )
                                        INNER JOIN Configuration..SplitString(@RegionIds, ',', 1) AS r ON r.Item = ca.RegionId
                                WHERE   ca.ProductType = @ProductType
                                        AND ca.ConfigType = @ConfigType
                                        AND ca.IsDeleted = 0
                                ORDER BY ca.RegionId
                                        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                        ONLY;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ProductType", productType),
                new SqlParameter("@ConfigType", configType),
                new SqlParameter("@PageIndex", pager.CurrentPage),
                new SqlParameter("@PageSize", pager.PageSize),
                new SqlParameter("@RegionIds", string.Join(",", regionIds)),
                new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var data = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VendorProductPriorityConfigModel>().ToList();
            var totalCount = Convert.ToInt32(parameters.Last().Value);
            var result = new Tuple<List<VendorProductPriorityConfigModel>, int>(data, totalCount);
            return result;
        }

        /// <summary>
        /// 编辑品牌优先级
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool EditVendorProductPriorityConfig(SqlConnection conn, VendorProductPriorityConfigModel model)
        {
            #region sql
            const string sql = @"UPDATE  Configuration.dbo.VendorProductPriorityConfig
                                SET     Priority = @Priority ,
                                        IsEnabled = @IsEnabled ,
                                        IsDeleted = 0 ,
                                        LastUpdateDateTime = GETDATE()
                                WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", model.PKID),
                new SqlParameter("@Priority", model.Priority),
                new SqlParameter("@IsEnabled", model.IsEnabled)
            };
            var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 添加品牌覆盖区域
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddVendorProductPriorityConfig(SqlConnection conn, VendorProductPriorityConfigModel model)
        {
            #region sql
            const string sql = @"INSERT  INTO Configuration..VendorProductPriorityConfig
                                        ( ProductType ,
                                          ConfigType ,
                                          VehicleId ,
                                          RegionId ,
                                          Priority ,
                                          IsEnabled
                                        )
                                OUTPUT  Inserted.PKID
                                VALUES  ( @ProductType ,
                                          @ConfigType ,
                                          @VehicleId ,
                                          @RegionId ,
                                          @Priority ,
                                          @IsEnabled
                                        );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@ConfigType", model.ConfigType),
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@Priority",model.Priority),
                new SqlParameter("@IsEnabled",model.IsEnabled)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 根据PKID获取品牌优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public VendorProductPriorityConfigModel GetVendorProductPriorityConfigByPKID(SqlConnection conn, long pkid)
        {
            const string sql = @"SELECT  ca.PKID ,
                                        ca.ProductType ,
                                        ca.ConfigType ,
                                        ca.VehicleId ,
                                        ca.RegionId ,
                                        ca.Priority ,
                                        ca.IsEnabled ,
                                        ca.IsDeleted ,
                                        ca.CreateDateTime ,
                                        ca.LastUpdateDateTime
                                FROM    Configuration.dbo.VendorProductPriorityConfig AS ca WITH ( NOLOCK )
                                WHERE   PKID = @PKID;";
            var paras = new[]
            {
                new SqlParameter("@PKID", pkid),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paras)
                .ConvertTo<VendorProductPriorityConfigModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取覆盖区域配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public VendorProductPriorityConfigModel GetVendorProductPriorityConfigByVehicleId
            (SqlConnection conn, VendorProductPriorityConfigModel model)
        {
            const string sql = @"SELECT  s.PKID ,
                                        s.ProductType ,
                                        s.ConfigType ,
                                        s.VehicleId ,
                                        s.RegionId ,
                                        s.Priority ,
                                        s.IsEnabled ,
                                        s.IsDeleted ,
                                        s.CreateDateTime ,
                                        s.LastUpdateDateTime
                                FROM    Configuration..VendorProductPriorityConfig AS s WITH ( NOLOCK )
                                WHERE   s.ProductType = @ProductType
                                        AND s.ConfigType = @ConfigType
                                        AND (@VehicleId IS NULL OR @VehicleId=N'' OR s.VehicleId = @VehicleId)
                                        AND (@RegionId IS NULL OR @RegionId=N'' OR s.RegionId = @RegionId);";
            var paras = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@ConfigType", model.ConfigType),
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@RegionId", model.RegionId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paras)
                .ConvertTo<VendorProductPriorityConfigModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 是否存在品牌优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistVendorProductPriorityConfigByVehicleId
            (SqlConnection conn, VendorProductPriorityConfigModel model)
        {
            const string sql = @"SELECT  COUNT(1)
                                FROM    Configuration..VendorProductPriorityConfig AS s WITH ( NOLOCK )
                                WHERE   s.ProductType = @ProductType
                                        AND s.ConfigType = @ConfigType
                                        AND (@VehicleId IS NULL OR @VehicleId=N'' OR s.VehicleId = @VehicleId)
                                        AND (@RegionId IS NULL OR @RegionId=N'' OR s.RegionId = @RegionId)
                                        AND s.IsDeleted = 0
                                        AND s.PKID <> @PKID;";
            var paras = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@ConfigType", model.ConfigType),
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@PKID", model.PKID)
            };
            var result = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, paras)) > 0;
            return result;
        }
    }
}

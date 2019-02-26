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
    public class DalVendorProductCoverArea
    {
        /// <summary>
        /// 查询覆盖区域--按品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="districtId"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public Tuple<List<VendorProductCoverAreaModel>, int> SearchVendorProductCoverArea
            (SqlConnection conn, string productType, string coverType, string brand, string pid,
             List<int> regionIds, PagerModel pager)
        {
            #region sql
            const string sql = @"SELECT  @TotalCount = COUNT(1)
                                FROM    Configuration.dbo.VendorProductCoverAreaConfig AS ca WITH ( NOLOCK )
                                INNER JOIN Configuration..SplitString(@RegionIds, ',', 1) AS r
                                    ON r.Item = ca.CoverRegionId
                                WHERE   (@Brand IS NULL OR @Brand=N'' OR ca.Brand = @Brand)
                                        AND (@Pid IS NULL OR @Pid=N'' OR ca.Pid = @Pid)
                                        AND ca.ProductType = @ProductType
                                        AND ca.CoverType = @CoverType
                                        AND ca.IsDeleted=0;
                                SELECT  ca.PKID ,
                                        ca.ProductType ,
                                        ca.CoverType ,
                                        ca.IsDeleted ,
                                        ca.CoverRegionId ,
                                        ca.Remark ,
                                        ca.IsEnabled ,
                                        ca.Brand,
                                        ca.Pid
                                FROM    Configuration.dbo.VendorProductCoverAreaConfig AS ca WITH ( NOLOCK )
                                INNER JOIN Configuration..SplitString(@RegionIds, ',', 1) AS r
                                    ON r.Item = ca.CoverRegionId
                                WHERE   (@Brand IS NULL OR @Brand=N'' OR ca.Brand = @Brand)
                                        AND (@Pid IS NULL OR @Pid=N'' OR ca.Pid = @Pid)
                                        AND ca.ProductType = @ProductType
                                        AND ca.CoverType = @CoverType
                                        AND ca.IsDeleted=0
                                ORDER BY ca.CoverRegionId
                                        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                        ONLY;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Brand", brand),
                new SqlParameter("@Pid", pid),
                new SqlParameter("@ProductType", productType),
                new SqlParameter("@CoverType", coverType),
                new SqlParameter("@PageIndex", pager.CurrentPage),
                new SqlParameter("@PageSize", pager.PageSize),
                new SqlParameter("@RegionIds", string.Join(",", regionIds)),
                new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var data = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VendorProductCoverAreaModel>().ToList();
            var totalCount = Convert.ToInt32(parameters.Last().Value);
            var result = new Tuple<List<VendorProductCoverAreaModel>, int>(data, totalCount);
            return result;
        }

        /// <summary>
        /// 编辑品牌覆盖区域
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool EditVendorProductCoverArea(SqlConnection conn, VendorProductCoverAreaModel model)
        {
            #region sql
            const string sql = @"UPDATE  Configuration.dbo.VendorProductCoverAreaConfig
                                        SET     Remark = @Remark ,
                                                IsEnabled = @IsEnabled ,
                                                LastUpdateDateTime = GETDATE()
                                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", model.PKID),
                new SqlParameter("@Remark", model.Remark),
                new SqlParameter("@IsEnabled", model.IsEnabled)
            };
            var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 删除覆盖区域
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteVendorProductCoverArea(SqlConnection conn, long pkid)
        {
            const string sql = @"UPDATE  Configuration.dbo.VendorProductCoverAreaConfig
                                SET     IsDeleted = 1 ,
                                        LastUpdateDateTime = GETDATE()
                                WHERE   PKID = @PKID;";
            SqlParameter parameter = new SqlParameter("@PKID", pkid);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        /// <summary>
        /// 添加品牌覆盖区域
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddVendorProductCoverArea(SqlConnection conn, VendorProductCoverAreaModel model)
        {
            #region sql
            const string sql = @"INSERT  INTO Configuration..VendorProductCoverAreaConfig
                                        ( ProductType ,
                                          CoverType ,
                                          Brand ,
                                          Pid ,
                                          CoverRegionId ,
                                          Remark ,
                                          IsEnabled 
                                        )
                                OUTPUT  Inserted.PKID
                                VALUES  ( @ProductType ,
                                          @CoverType ,
                                          @Brand ,
                                          @Pid ,
                                          @CoverRegionId ,
                                          @Remark ,
                                          @IsEnabled 
                                        );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@CoverType", model.CoverType),
                new SqlParameter("@Brand", model.Brand),
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@CoverRegionId", model.CoverRegionId),
                new SqlParameter("@Remark",model.Remark),
                new SqlParameter("@IsEnabled",model.IsEnabled)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 根据PKID获取覆盖区域配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public VendorProductCoverAreaModel GetVendorProductConverAreaByPKID(SqlConnection conn, long pkid)
        {
            const string sql = @"SELECT ca.PKID ,
                                        ca.ProductType ,
                                        ca.Brand ,
                                        ca.CoverType ,
                                        ca.CoverRegionId ,
                                        ca.Pid ,
                                        ca.Remark ,
                                        ca.IsEnabled ,
                                        ca.IsDeleted ,
                                        ca.CreateDateTime ,
                                        ca.LastUpdateDateTime
                                 FROM   Configuration.dbo.VendorProductCoverAreaConfig AS ca WITH ( NOLOCK )
                                 WHERE  PKID = @PKID;";
            var paras = new[]
            {
                new SqlParameter("@PKID", pkid),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paras)
                .ConvertTo<VendorProductCoverAreaModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取覆盖区域配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public VendorProductCoverAreaModel GetVendorProductConverAreaByBrand
            (SqlConnection conn, VendorProductCoverAreaModel model)
        {
            const string sql = @"SELECT  s.PKID ,
                                        s.ProductType ,
                                        s.CoverType ,
                                        s.Brand ,
                                        s.Pid ,
                                        s.CoverRegionId ,
                                        s.Remark ,
                                        s.IsEnabled ,
                                        s.IsDeleted ,
                                        s.CreateDateTime ,
                                        s.LastUpdateDateTime
                                FROM    Configuration..VendorProductCoverAreaConfig AS s WITH ( NOLOCK )
                                WHERE   s.ProductType = @ProductType
                                        AND s.CoverType = @CoverType
                                        AND s.CoverRegionId = @CoverRegionId
                                        AND (@Brand IS NULL OR @Brand=N'' OR s.Brand = @Brand)
                                        AND (@Pid IS NULL OR @Pid=N'' OR s.Pid = @Pid)
                                        AND s.PKID <> @PKID;";
            var paras = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@CoverType", model.CoverType),
                new SqlParameter("@CoverRegionId", model.CoverRegionId),
                new SqlParameter("@Brand", model.Brand),
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@PKID", model.PKID)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paras)
                .ConvertTo<VendorProductCoverAreaModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 是否存在品牌覆盖区域配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistVendorProductConverAreaByBrand
            (SqlConnection conn, VendorProductCoverAreaModel model)
        {
            const string sql = @"SELECT  COUNT(1)
                                FROM    Configuration..VendorProductCoverAreaConfig AS s WITH ( NOLOCK )
                                WHERE   s.ProductType = @ProductType
                                        AND s.CoverType = @CoverType
                                        AND s.CoverRegionId = @CoverRegionId
                                        AND s.Brand = @Brand
                                        AND s.IsDeleted = 0
                                        AND s.PKID <> @PKID;";
            var paras = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@CoverType", model.CoverType),
                new SqlParameter("@CoverRegionId", model.CoverRegionId),
                new SqlParameter("@Brand", model.Brand),
                new SqlParameter("@PKID", model.PKID)
            };
            var result = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, paras)) > 0;
            return result;
        }

        /// <summary>
        /// 是否存在Pid覆盖区域配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistVendorProductConverAreaByPid
            (SqlConnection conn, VendorProductCoverAreaModel model)
        {
            const string sql = @"SELECT  COUNT(1)
                                FROM    Configuration..VendorProductCoverAreaConfig AS s WITH ( NOLOCK )
                                WHERE   s.ProductType = @ProductType
                                        AND s.CoverType = @CoverType
                                        AND s.CoverRegionId = @CoverRegionId
                                        AND s.Pid = @Pid
                                        AND s.IsDeleted = 0
                                        AND s.PKID <> @PKID;";
            var paras = new[]
            {
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@CoverType", model.CoverType),
                new SqlParameter("@CoverRegionId", model.CoverRegionId),
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@PKID", model.PKID)
            };
            var result = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, paras)) > 0;
            return result;
        }
    }
}

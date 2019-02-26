using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalVendorProductCouponPriceConfig

    {
        public DalVendorProductCouponPriceConfig()
        {

        }

        /// <summary>
        /// 添加供应商产品券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddVendorProductCouponPriceConfig(SqlConnection conn, VendorProductCouponPriceConfigModel model)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..VendorProductCouponPriceConfig
                                ( ProductType, Pid, IsShow )
                        OUTPUT  inserted.PKID
                        VALUES  ( @ProductType, @Pid, @IsShow );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@IsShow", model.IsShow)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新供应商产品券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateVendorProductCouponPriceConfig(SqlConnection conn, VendorProductCouponPriceConfigModel model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration.dbo.VendorProductCouponPriceConfig
                        SET     IsShow = @IsShow ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@IsShow", model.IsShow),
                new SqlParameter("@PKID", model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 查询供应商产品券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public List<VendorProductCouponPriceConfigViewModel> SelectVendorProductCouponPriceConfig
            (SqlConnection conn, List<string> pids, string productType, string brand)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.ProductType ,
                                p.PID ,
                                s.IsShow ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime ,
                                p.CP_Brand AS Brand ,
                                p.DisplayName ,
                                p.oid AS Oid ,
                                p.cy_list_price AS OriginalPrice
                        FROM    Configuration..VendorProductCouponPriceConfig AS s WITH ( NOLOCK )
                                RIGHT JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK ) ON s.Pid = p.PID
                                                                                      AND s.ProductType = @ProductType
                                INNER JOIN Configuration..SplitString(@Pids, N',', 1) AS c ON c.Item = p.PID
                        WHERE   p.CP_Brand = @Brand
                                AND p.i_ClassType IN ( 2, 4 )
                                AND p.cy_list_price > 0
                        ORDER BY p.oid;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ProductType", productType),
                new SqlParameter("@Pids", string.Join(",", pids)),
                new SqlParameter("@Brand", brand)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VendorProductCouponPriceConfigViewModel>().ToList();
        }

        /// <summary>
        /// 根据Pid获取供应商产品券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public VendorProductCouponPriceConfigModel GetVendorProductCouponPriceConfig
            (SqlConnection conn, string productType, string pid)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.ProductType ,
                                s.Pid ,
                                s.IsShow ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VendorProductCouponPriceConfig AS s WITH ( NOLOCK )
                        WHERE   s.ProductType = @ProductType
                                AND s.Pid = @Pid;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Pid", pid),
                new SqlParameter("@ProductType", productType)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VendorProductCouponPriceConfigModel>().FirstOrDefault();
        }

        /// <summary>
        /// 供应商产品券后价展示配置是否重复
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistVendorProductCouponPriceConfig(SqlConnection conn, VendorProductCouponPriceConfigModel model)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VendorProductCouponPriceConfig AS s WITH ( NOLOCK )
                        WHERE   s.Pid = @Pid
                                AND s.ProductType = @ProductType
                                AND s.IsDeleted = 0
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@ProductType", model.ProductType),
                new SqlParameter("@PKID", model.PKID)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }
    }
}

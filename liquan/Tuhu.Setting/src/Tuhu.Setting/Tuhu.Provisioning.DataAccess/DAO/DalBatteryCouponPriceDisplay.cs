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
    public class DalBatteryCouponPriceDisplay
    {
        public DalBatteryCouponPriceDisplay()
        {

        }

        /// <summary>
        /// 添加蓄电池券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddBatteryCouponPriceDisplay(SqlConnection conn, BatteryCouponPriceDisplayModel model)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..BatteryCouponPriceDisplay
                                ( Pid, IsShow )
                        OUTPUT  inserted.PKID
                        VALUES  ( @Pid, @IsShow );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@IsShow", model.IsShow)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新蓄电池券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateBatteryCouponPriceDisplay(SqlConnection conn, BatteryCouponPriceDisplayModel model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration.dbo.BatteryCouponPriceDisplay
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
        /// 查询蓄电池券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public List<BatteryCouponPriceDisplayViewModel> SelectBatteryCouponPriceDisplay(SqlConnection conn, string brand)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                p.Pid ,
                                s.IsShow ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime ,
                                p.CP_Brand AS Brand ,
                                p.DisplayName ,
                                p.oid AS Oid ,
                                p.cy_list_price as OriginalPrice
                        FROM    Configuration..BatteryCouponPriceDisplay AS s WITH ( NOLOCK )
                                RIGHT JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK ) ON s.Pid = p.PID
                        WHERE   p.PrimaryParentCategory = N'battery'
                                AND ( @Brand IS NULL
                                      OR @Brand = N''
                                      OR p.CP_Brand = @Brand
                                    )
                                AND p.cy_list_price > 0
                        ORDER BY p.CP_Brand;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Brand", brand)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<BatteryCouponPriceDisplayViewModel>().ToList();
        }

        /// <summary>
        /// 根据Pid获取蓄电池券后价展示配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public BatteryCouponPriceDisplayModel GetBatteryCouponPriceDisplay(SqlConnection conn, string pid)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.Pid ,
                                s.IsShow ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..BatteryCouponPriceDisplay AS s WITH ( NOLOCK )
                        WHERE   s.Pid = @Pid;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Pid", pid)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<BatteryCouponPriceDisplayModel>().FirstOrDefault();
        }

        /// <summary>
        /// 蓄电池券后价展示配置是否重复
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistBatteryCouponPriceDisplay(SqlConnection conn, BatteryCouponPriceDisplayModel model)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..BatteryCouponPriceDisplay AS s WITH ( NOLOCK )
                        WHERE   s.Pid = @Pid
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@Pid", model.Pid),
                new SqlParameter("@PKID", model.PKID)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }
    }
}

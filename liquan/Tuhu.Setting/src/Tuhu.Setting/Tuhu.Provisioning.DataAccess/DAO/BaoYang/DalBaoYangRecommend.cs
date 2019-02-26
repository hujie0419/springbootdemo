using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.DataAccess.DAO.BaoYang
{
    public class DalBaoYangRecommend
    {
        /// <summary>
        /// 添加机油优先级
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="item"></param>
        /// <param name="grade"></param>
        /// <param name="viscosity"></param>
        /// <returns></returns>
        public static int InsertOilProductPriority(SqlConnection conn, BaoYangProductPriorityModel item, string grade, string viscosity)
        {
            const string sql = @"INSERT INTO BaoYang..ProductPriority_Regular_Oil
                                            (
                                                [Viscosity] ,
                                                [Grade] ,
                                                [Brand] ,
                                                [Series] ,
                                                [Seq] ,
                                                [CreateDateTime] ,
                                                [LastUpdateDateTime]
                                            )
                                            OUTPUT Inserted.PKID
                                            VALUES
                                            (
                                                @Viscosity ,
                                                @Grade ,
                                                @Brand ,
                                                @Series ,
                                                @Priority ,
                                                GetDate() ,
                                                GetDate() 
                                            );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Brand", item.Brand),
                new SqlParameter("@Series", item.Series ?? string.Empty),
                new SqlParameter("@Grade", grade ?? string.Empty ),
                new SqlParameter("@Priority", item.Priority),
                new SqlParameter("@Viscosity", viscosity ?? string.Empty)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 根据粘度与等级删除机油排序
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="grade"></param>
        /// <param name="viscosity"></param>
        /// <returns></returns>
        public static int DeleteOilProductPriority(SqlConnection conn, string grade, string viscosity)
        {
            const string sql = @"DELETE  FROM BaoYang..ProductPriority_Regular_Oil WHERE   Grade = @Grade   AND Viscosity = @Viscosity;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@Grade",grade),
                 new SqlParameter("@Viscosity",viscosity)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter);
        }

        /// <summary>
        /// 根据partName删除排序
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public static int DeleteProductPrioritySettingNewByPartName(SqlConnection conn, string partName)
        {
            const string sql = @"DELETE  FROM BaoYang..ProductPriority_Regular WHERE   PartName = @partName;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@partName",partName)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter);
        }

        /// <summary>
        /// 获取当前partName最大的AreaId
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public static int GetMaxAreaIdByPartName(SqlConnection conn, string partName)
        {
            const string sql = @"  SELECT ISNULL(MAX(AreaId),0)  FROM BaoYang..ProductPriority_Area WHERE PartName = @partName";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@partName",partName)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameter));
        }

        /// <summary>
        /// 根据partName获取地区配置
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>g
        public static List<ProductPriorityArea> GetProductPriorityArea(SqlConnection conn, string partName)
        {
            const string sql = @"SELECT  a.AreaId ,
                                        a.RegionId , 
                                        a.partName , 
                                        a.IsEnabled
                                FROM    BaoYang..ProductPriority_Area AS a WITH (NOLOCK)
                                WHERE   a.partName = @partName;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@partName",partName)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<ProductPriorityArea>().ToList();
        }

        public static bool DeleteProductPriorityArea(SqlConnection conn, string partName, int areaId)
        {
            const string sql = @"DELETE  BaoYang..ProductPriority_Area
                                WHERE   PartName = @partName  AND AreaId = @areaId;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@partName",partName),
                   new SqlParameter("@areaId",areaId)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static int InsertProductPriorityArea(SqlConnection conn, ProductPriorityArea entity)
        {
            const string sql = @"INSERT INTO [BaoYang].[dbo].[ProductPriority_Area]
                                (
                                    [PartName] ,
                                    [AreaId] ,
                                    [RegionId] ,
                                    [IsEnabled] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime]
                                )
                                OUTPUT Inserted.PKID
                                VALUES
                                (
                                    @PartName ,
                                    @AreaId ,
                                    @RegionId ,
                                    @IsEnabled ,
                                    GETDATE() ,
                                    GETDATE()
                                );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PartName", entity.PartName??string.Empty),
                new SqlParameter("@RegionId", entity.RegionId),
                new SqlParameter("@AreaId", entity.AreaId),
                new SqlParameter("@IsEnabled",  entity.IsEnabled )
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        public static bool UpdatePriorityAreaIsEnabled(SqlConnection conn, int areaId, bool isEnabled, string partName)
        {
            const string sql = @"UPDATE  BaoYang..ProductPriority_Area
                                SET     IsEnabled = @isEnabled
                                WHERE   PartName = @partName
                                        AND AreaId = @areaId;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@partName",partName),
                   new SqlParameter("@areaId",areaId),
                   new SqlParameter("@isEnabled",isEnabled)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static List<ProductPriorityAreaDetail> GetProductPriorityAreaDetail(SqlConnection conn, VehicleProductPriorityRequest request)
        {
            const string sql = @"
                                    SELECT  ppad.PKID ,
                                            ppad.PartName ,
                                            ppad.AreaId ,
                                            ppad.VehicleId ,
                                            ppad.Brand ,
                                            ppad.Series ,
                                            ppad.Seq ,
                                            ppad.IsEnabled
                                    FROM    BaoYang..ProductPriority_Area_Detail AS ppad WITH (NOLOCK)
                                    WHERE   EXISTS (
                                        SELECT  1
                                        FROM    dbo.ProductPriority_Area_Detail AS odd WITH (NOLOCK)
                                        WHERE   odd.PartName = @PartName
                                                AND odd.AreaId = @areaid
                                                AND odd.VehicleId =  ppad.VehicleId
                                                AND odd.Seq >= @Seq
                                                AND (
                                                    @ProductBrand = N''
                                                    OR  @ProductBrand IS NULL
                                                    OR  (
                                                        odd.Brand = @ProductBrand
                                                        AND (
                                                            @Seq = 0
                                                            OR  odd.Seq = @Seq
                                                        )
                                                    )
                                                )
                                                AND (
                                                    @ProductSeries = N''
                                                    OR  @ProductSeries IS NULL
                                                    OR  (
                                                        odd.Series = @ProductSeries
                                                        AND (
                                                            @Seq = 0
                                                            OR  odd.Seq = @Seq
                                                        )
                                                    )
                                                )
                                    )
                                            AND ppad.AreaId = @areaid
                                        AND (
                                            @VehicleId = N''
                                            OR  @VehicleId IS NULL
                                            OR  ppad.VehicleId = @VehicleId
                                        )
                                            AND ppad.PartName = @PartName
                                            AND ppad.IsDeleted = 0;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@partName",request.PartName),
                 new SqlParameter("@areaid",request.AreaId),
                 new SqlParameter("@ProductBrand",request.ProductBrand) ,
                 new SqlParameter("@ProductSeries",request.ProductSeries) ,
                 new SqlParameter("@Seq",request.Seq),
                 new SqlParameter("@VehicleId",request.VehicleId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<ProductPriorityAreaDetail>().ToList();
        }

        public static bool DeleteVehicleProductPriority(SqlConnection conn, int areaId, string[] vehicleIds, string partName)
        {
            const string sql = @"DELETE  ProductPriority_Area_Detail
                                WHERE   PartName = @PartName
                                        AND AreaId = @AreaId
                                        AND VehicleId IN (
                                                SELECT  Item FROM   BaoYang..SplitString(@VehicleIds, ',', 1)
                                            );";
            SqlParameter[] parameter = new SqlParameter[] {
                  new SqlParameter("@AreaId",areaId),
                  new SqlParameter("@PartName",partName),
                  new SqlParameter("@VehicleIds",string.Join(",",vehicleIds))

            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;

        }

        public static int InsertVehicleProductPriority(SqlConnection conn, ProductPriorityAreaDetail model)
        {
            const string sql = @"INSERT INTO BaoYang..ProductPriority_Area_Detail
                                            (
                                                PartName ,
                                                AreaId ,
                                                VehicleId ,
                                                Brand ,
                                                Series ,
                                                Seq ,
                                                IsEnabled ,
                                                IsDeleted ,
                                                CreateDateTime ,
                                                LastUpdateDateTime
                                            ) OUTPUT Inserted.PKID
                                            VALUES
                                            (
                                                @PartName ,     
                                                @AreaId ,       
                                                @VehicleId ,    
                                                @Brand ,        
                                                @Series ,       
                                                @Seq ,          
                                                @IsEnabled ,    
                                                0,              
                                                GETDATE() ,     
                                                GETDATE()       
                                            );";

            SqlParameter[] parameter = new SqlParameter[] {
                  new SqlParameter("@AreaId",model.AreaId),
                  new SqlParameter("@PartName",model.PartName),
                  new SqlParameter("@Brand",model.Brand),
                  new SqlParameter("@Series",model.Series),
                  new SqlParameter("@Seq",model.Seq),
                  new SqlParameter("@VehicleId",model.VehicleId),
                  new SqlParameter("@IsEnabled",model.IsEnabled),

            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameter));
        }

        public static List<ProductPriorityOilDetail> GetOilProductPriorityAreaDetail(SqlConnection conn)
        {
            const string sql = @"   SELECT  o.NewViscosity , 
                                        o.AreaId ,
                                        o.VehicleId ,
                                        o.Viscosity ,
                                        o.Grade ,
                                        o.NewViscosity ,
                                        o.IsEnabled,          
		                                      od.AreaOilId,
                                        od.Brand ,
                                        od.Series ,
                                        od.Seq ,
                                        od.Grade AS ProductPriorityGrade
                                FROM    ProductPriority_Area_Oil                     AS o WITH (NOLOCK)
                                        LEFT JOIN dbo.ProductPriority_Area_OilDetail AS od WITH (NOLOCK)
                                            ON o.PKID = od.AreaOilId    ";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<ProductPriorityOilDetail>().ToList();
        }

        public static List<ProductPriorityOilDetail> GetOilProductPriorityAreaDetail(SqlConnection conn, OilVehicleProductPriorityRequst request)
        {
            #region SQL
            const string sql = @"SELECT  o.NewViscosity ,
                                        o.AreaId ,
                                        o.VehicleId ,
                                        o.Viscosity ,
                                        o.Grade ,
                                        o.NewViscosity ,
                                        o.IsEnabled ,
                                        o.PKID   AS AreaOilId ,
                                        od.Brand ,
                                        od.Series ,
                                        od.Seq ,
                                        od.Grade AS ProductPriorityGrade
                                FROM    ProductPriority_Area_Oil                     AS o WITH (NOLOCK)
                                        LEFT JOIN dbo.ProductPriority_Area_OilDetail AS od WITH (NOLOCK)
                                            ON o.PKID = od.AreaOilId
                                WHERE   (
                                    (
                                            @Seq = 0
                                            AND (
                                                @ProductPriorityGrade = N''
                                                OR  @ProductPriorityGrade IS NULL
                                            )
                                            AND (
                                                @Brand = N''
                                                OR  @Brand IS NULL
                                            )
                                            AND (
                                                @Series = N''
                                                OR  @Series IS NULL
                                            )
                                        )
                                    OR  EXISTS (
                                    SELECT  1
                                    FROM    dbo.ProductPriority_Area_OilDetail AS odd WITH (NOLOCK)
                                    WHERE   odd.AreaOilId = o.PKID
                                            AND odd.Seq >= @Seq
                                            AND (
                                                @ProductPriorityGrade = N''
                                                OR  @ProductPriorityGrade IS NULL
                                                OR  (
                                                    odd.Grade = @ProductPriorityGrade
                                                    AND (
                                                        @Seq = 0
                                                        OR  odd.Seq = @Seq
                                                    )
                                                )
                                            )
                                            AND (
                                                @Brand = N''
                                                OR  @Brand IS NULL
                                                OR  (
                                                    odd.Brand = @Brand
                                                    AND (
                                                        @Seq = 0
                                                        OR  odd.Seq = @Seq
                                                    )
                                                )
                                            )
                                            AND (
                                                @Series = N''
                                                OR  @Series IS NULL
                                                OR  (
                                                    odd.Series = @Series
                                                    AND (
                                                        @Seq = 0
                                                        OR  odd.Seq = @Seq
                                                    )
                                                )
                                            )
                                )
                                )
                                        AND (
                                            @NewViscosity = N''
                                            OR  @NewViscosity IS NULL
                                            OR  o.NewViscosity = @NewViscosity
                                        )
                                        AND (
                                            @Viscosity = N''
                                            OR  @Viscosity IS NULL
                                            OR  o.Viscosity = @Viscosity
                                        )
                                        AND (
                                            @Grade = N''
                                            OR  @Grade IS NULL
                                            OR  o.Grade = @Grade
                                        )
                                        AND o.IsDeleted = 0
                                        AND o.AreaId = @AreaId
                                        AND (
                                            @VehicleId = N''
                                            OR  @VehicleId IS NULL
                                            OR  o.VehicleId = @VehicleId
                                        )
                                     ;";
            #endregion

            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@AreaId",request.AreaId),
                  new SqlParameter("@Grade",request.Grade),
                  new SqlParameter("@NewViscosity",request.NewViscosity),
                  new SqlParameter("@Brand",request.ProductBrand),
                  new SqlParameter("@ProductPriorityGrade",request.ProductPriorityGrade),
                  new SqlParameter("@Series",request.ProductSeries),
                  new SqlParameter("@Seq",request.Seq),
                  new SqlParameter("@Viscosity",request.Viscosity),
                   new SqlParameter("@VehicleId",request.VehicleId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<ProductPriorityOilDetail>().ToList();
        }

        /// <summary>
        /// 添加特殊车型机油推荐
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="productPriorityAreaOil"></param>
        /// <returns></returns>
        public static int InsertProductPriorityAreaOil(SqlConnection conn, ProductPriorityAreaOil productPriorityAreaOil)
        {
            string sql = @"INSERT INTO BaoYang..ProductPriority_Area_Oil
                                        (
                                            AreaId ,
                                            VehicleId ,
                                            Viscosity ,
                                            Grade ,
                                            NewViscosity ,
                                            IsEnabled,
                                            IsDeleted,
                                            CreateDateTime ,
                                            LastUpdateDateTime
                                        )
                                        OUTPUT Inserted.PKID
                                        VALUES
                                        (
                                            @AreaId ,
                                            @VehicleId ,
                                            @Viscosity ,
                                            @Grade ,
                                            @NewViscosity , 
                                            @IsEnabled ,
                                            0 ,
                                            GETDATE() ,
                                            GETDATE()
                                        );";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@AreaId",productPriorityAreaOil.AreaId),
                new SqlParameter("@VehicleId",productPriorityAreaOil.VehicleId),
                new SqlParameter("@Viscosity",productPriorityAreaOil.Viscosity),
                new SqlParameter("@IsEnabled",productPriorityAreaOil.IsEnabled),
                new SqlParameter("@Grade",productPriorityAreaOil.Grade),
                new SqlParameter("@NewViscosity",productPriorityAreaOil.NewViscosity?? string.Empty)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameter));
        }
        public static bool UpdateProductPriorityAreaOilByPkid(SqlConnection conn, ProductPriorityAreaOil model)
        {
            string sql = @"UPDATE  BaoYang..ProductPriority_Area_Oil
                            SET     NewViscosity = @NewViscosity ,
                                    LastUpdateDateTime = GETDATE() ,
                                    VehicleId = @VehicleId ,
                                    Viscosity = @Viscosity ,
                                    IsEnabled = @IsEnabled ,
                                    IsDeleted = 0 ,
                                    Grade = @Grade
                            WHERE   PKID = @pkid;";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@PkId",model.PKID),
                 new SqlParameter("@NewViscosity",model.NewViscosity ?? string.Empty),
                 new SqlParameter("@IsEnabled",model.IsEnabled),
                 new SqlParameter("@VehicleId",model.VehicleId),
                 new SqlParameter("@Grade",model.Grade),
                 new SqlParameter("@Viscosity",model.Viscosity),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool DeleteProductPriorityAreaOilDetailByAreaOilId(SqlConnection conn, int areaOilId)
        {
            string sql = @"DELETE  BaoYang..ProductPriority_Area_OilDetail
                            WHERE   AreaOilId = @AreaOilId;";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@AreaOilId",areaOilId),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        /// <summary>
        /// 添加特殊车型机油详情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="productPriorityAreaOilDetail"></param>
        /// <returns></returns>
        public static int InsertProductPriorityAreaOilDetail(SqlConnection conn, ProductPriorityAreaOilDetail productPriorityAreaOilDetail)
        {
            string sql = @"INSERT INTO BaoYang..ProductPriority_Area_OilDetail
                                        (
                                            AreaOilId ,
                                            Brand ,
                                            Series ,
                                            Seq ,
                                            Grade ,
                                            CreateDateTime ,
                                            LastUpdateDateTime
                                        )
                                        OUTPUT Inserted.PKID
                                        VALUES
                                        (
                                            @AreaOilId ,
                                            @Brand ,
                                            @Series ,
                                            @Seq ,
                                            @Grade ,
                                            GETDATE() ,
                                            GETDATE()
                                        );";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@AreaOilId",productPriorityAreaOilDetail.AreaOilId),
                new SqlParameter("@Brand",productPriorityAreaOilDetail.Brand),
                new SqlParameter("@Series",productPriorityAreaOilDetail.Series),
                new SqlParameter("@Grade",productPriorityAreaOilDetail.Grade),
                new SqlParameter("@Seq",productPriorityAreaOilDetail.Seq)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameter));
        }


        public static List<ProductPriorityOilDetail> GetOilProductPriorityAreaDetailByPkids(SqlConnection conn, IEnumerable<int> pkids)
        {
            string sql = @"WITH pkids
                                    AS (SELECT  *
                                        FROM    BaoYang..SplitString(@Pkids, ',', 1) )
                                    SELECT  o.NewViscosity ,
                                            o.AreaId ,
                                            o.VehicleId ,
                                            o.Viscosity ,
                                            o.Grade ,
                                            o.NewViscosity ,
                                            o.IsEnabled ,
                                            od.AreaOilId ,
                                            od.Brand ,
                                            od.Series ,
                                            od.Seq ,
                                            od.Grade AS ProductPriorityGrade
                                    FROM    ProductPriority_Area_Oil                     AS o WITH (NOLOCK)
                                            LEFT JOIN dbo.ProductPriority_Area_OilDetail AS od WITH (NOLOCK)
                                                ON o.PKID = od.AreaOilId
                                    WHERE   EXISTS (
                                        SELECT  1 FROM  pkids WHERE pkids.Item = od.AreaOilId
                                    )
                                            AND o.IsDeleted = 0;";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@pkids",string.Join(",",pkids)),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<ProductPriorityOilDetail>().ToList();
        }

        /// <summary>
        /// 修改机油特殊推荐IsDeleted状态
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="areaOilIds"></param>
        /// <returns></returns>
        public static bool DeleteProductPriorityAreaOil(SqlConnection conn, IEnumerable<int> areaOilIds)
        {
            string sql = @"UPDATE  BaoYang..ProductPriority_Area_Oil
                            SET     IsDeleted = @IsDeleted
                            WHERE   EXISTS (
                                SELECT  1
                                FROM    BaoYang..SplitString(@pkids, ',', 1) AS pkids
                                WHERE   PKID = pkids.Item
                            );";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@pkids",string.Join(",",areaOilIds)),
                new SqlParameter("@IsDeleted", true),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static List<ProductPriorityAreaDetail> GetProductPriorityAreaByVehicleIds(SqlConnection conn, IEnumerable<string> vehicleIds)
        {
            string sql = @"WITH vehicleIds
                            AS (SELECT  *
                                FROM    BaoYang..SplitString(@vehicleIds, ',', 1) )
                            SELECT  a.PKID ,
                                    a.PartName ,
                                    a.AreaId ,
                                    a.VehicleId ,
                                    a.Brand ,
                                    a.Series ,
                                    a.Seq ,
                                    a.IsEnabled
                            FROM    ProductPriority_Area_Detail AS a WITH (NOLOCK)
                            WHERE   EXISTS (
                                SELECT  1 FROM  vehicleIds WHERE vehicleIds.Item = a.VehicleId
                            )
                                    AND a.IsDeleted = 0;";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@vehicleIds",string.Join(",",vehicleIds)),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<ProductPriorityAreaDetail>().ToList();
        }

        public static bool DeleteProductPriorityAreaDetail(SqlConnection conn, IEnumerable<string> vehicleIds)
        {
            string sql = @"UPDATE  BaoYang..ProductPriority_Area_Detail
                            SET     IsDeleted = @IsDeleted
                            WHERE   EXISTS (
                                SELECT  1
                                FROM    BaoYang..SplitString(@vehicleIds, ',', 1) AS vehicleIds
                                WHERE   vehicleId = vehicleIds.Item
                            );";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@vehicleIds",string.Join(",",vehicleIds)),
                new SqlParameter("@IsDeleted", true),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool UpdateAreaDetailEnabledBypkid(SqlConnection conn, string vehicleId, bool isEnabled)
        {
            string sql = @"UPDATE  BaoYang..ProductPriority_Area_Detail
                        SET     IsEnabled = @IsEnabled
                        WHERE   vehicleId = @vehicleId;";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@vehicleId",vehicleId),
                new SqlParameter("@isEnabled", isEnabled),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool UpdateOilAreaEnabledBypkid(SqlConnection conn, int pkid, bool isEnabled)
        {
            string sql = @"UPDATE  BaoYang..ProductPriority_Area_Oil
                        SET     IsEnabled = @IsEnabled
                        WHERE   PKID = @pkid;";
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@pkid",pkid),
                new SqlParameter("@isEnabled", isEnabled),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }


    }
}


using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;
using Microsoft.ApplicationBlocks.Data;
using System;
using Tuhu.Component.Common.Extensions;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class ProductLibraryConfigDAL
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SqlConnection sqlcon, ProductLibraryConfigModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Configuration..SE_ProductLibraryConfig(");
            strSql.Append("Oid,PID,CouponIds,State,CreateTime)");
            strSql.Append(" values (");
            strSql.Append("@Oid,@PID,@CouponIds,@State,@CreateTime)");
            SqlParameter[] parameters = {
                    new SqlParameter("@Oid", SqlDbType.Int),
                    new SqlParameter("@PID", SqlDbType.NVarChar),
                    new SqlParameter("@CouponIds", SqlDbType.NVarChar),
                    new SqlParameter("@State", SqlDbType.Int),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime)
            };
            parameters[1].Value = model.Oid;
            parameters[2].Value = model.PID;
            parameters[3].Value = model.CouponIds;
            parameters[4].Value = model.State;
            parameters[5].Value = model.CreateTime;

            return SqlHelper.ExecuteNonQuery(sqlcon, CommandType.Text, strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SqlConnection sqlcon, ProductLibraryConfigModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Configuration..SE_ProductLibraryConfig set ");
            strSql.Append("Oid=@Oid,");
            strSql.Append("PID=@PID,");
            strSql.Append("CouponIds=@CouponIds,");
            strSql.Append("State=@State,");
            strSql.Append("CreateTime=@CreateTime");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Oid", SqlDbType.Int),
                    new SqlParameter("@PID", SqlDbType.NVarChar),
                    new SqlParameter("@CouponIds", SqlDbType.NVarChar),
                    new SqlParameter("@State", SqlDbType.Int),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime),
                    new SqlParameter("@Id", SqlDbType.Int)};
            parameters[0].Value = model.Oid;
            parameters[1].Value = model.PID;
            parameters[2].Value = model.CouponIds;
            parameters[3].Value = model.State;
            parameters[4].Value = model.CreateTime;
            parameters[5].Value = model.Id;

            return SqlHelper.ExecuteNonQuery(sqlcon, CommandType.Text, strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        public static bool BatchDelete(SqlConnection sqlcon, string Idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Configuration..SE_ProductLibraryConfig ");
            strSql.Append(" where Id in (" + Idlist + ")  ");
            return SqlHelper.ExecuteNonQuery(sqlcon, CommandType.Text, strSql.ToString(), null) > 0;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static IEnumerable<ProductLibraryConfigModel> GetList(SqlConnection sqlcon, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM Configuration..SE_ProductLibraryConfig WITH(NOLOCK) ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, strSql.ToString(), null).ConvertTo<ProductLibraryConfigModel>();
        }

        public static IEnumerable<ProductLibraryConfigModel> GetProductCouponConfigByOids(SqlConnection sqlcon, List<int> oids)
        {
            var sql = @"WITH    Oids
          AS ( SELECT   *
               FROM     Configuration..Split(@Oids, ',')
             )
    SELECT  *
    FROM    Configuration..SE_ProductLibraryConfig AS C WITH ( NOLOCK )
            JOIN Oids ON Oids.col = C.Oid; ";
            SqlParameter parameter = new SqlParameter("@Oids", string.Join(",", oids));
            return SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, sql, parameter)
                .ConvertTo<ProductLibraryConfigModel>();
        }

        public static ProductSalesPredic QueryProductSalesInfoByPID(SqlConnection conn,string pid)
        {
            const string sql = @"
    SELECT  *
    FROM    Tuhu_bi..dm_Product_SalespredictData AS ps WITH ( NOLOCK )
            WHERE ps.pid = @PID;";
            SqlParameter parameter = new SqlParameter("@PID", pid);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter)
                .ConvertTo<ProductSalesPredic>().FirstOrDefault();
        }

        public static IEnumerable<ProductSalesPredic> QueryProductSalesInfo(SqlConnection conn)
        {
            const string sql = @" SELECT  *  FROM    Tuhu_bi..dm_Product_SalespredictData AS ps WITH ( NOLOCK )";
           
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql)
                .ConvertTo<ProductSalesPredic>();
        }

        /// <summary>
        /// 根据查询条件获取产品列表
        /// </summary>
        /// <param name="sqlcon">链接对象</param>
        /// <param name="model">查询对象</param>
        /// <returns></returns>
        public static IEnumerable<QueryProductsModel> QueryProductsForProductLibrary(SqlConnection sqlcon, SeachProducts model)
        {
            if (string.IsNullOrWhiteSpace(model.Category))
            {
                return null;
            }
            StringBuilder strSql = new StringBuilder();
            List<SqlParameter> paramsList = new List<SqlParameter>();
            #region 查询脚本
            strSql.Append(@"
                    SELECT * From(
                    SELECT 
	                    ROW_NUMBER() over(order by   tab1.OrigProductID desc ) rowNumber,
                        tab1.PrimaryParentCategory AS Category ,
                        tab1.oid ,
                        tab1.DisplayName ,
                        tab1.CP_Brand ,
                        tab1.CP_Tab ,
                        tab1.CP_ShuXing5 ,
                        tab1.OnSale ,
                        tab1.PID ,
                        tab1.CP_Tire_Pattern ,
                        tab1.CP_Tire_Rim ,
                        tab1.cy_marketing_price ,
                        tab2.CouponIds ,
                        tab3.NodeNo ,
	                    tab4.IsShow              
                    FROM  Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab3 WITH ( NOLOCK )
                      INNER JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1
                      WITH ( NOLOCK ) ON tab3.child_oid = tab1.oid AND tab1.PID IS NOT NULL
                      INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_CatalogProducts] AS tab4 WITH(NOLOCK) ON tab1.oid=tab4.oid 
                      LEFT JOIN Configuration..SE_ProductLibraryConfig AS tab2
                      WITH ( NOLOCK ) ON tab1.oid = tab2.Oid
                     WHERE  1 = 1  
                ");
            #endregion

            //拼接条件
            var strCondition = GetQueryProductsCondition(paramsList, model);
            strSql.Append(strCondition);
            //分页条件
            strSql.AppendFormat(" ) temp where temp.rowNumber>{0} and temp.rowNumber<={1}",
                (model.PageIndex - 1) * model.PageSize, model.PageSize * model.PageIndex);

            var r = new List<QueryProductsModel>();
            string GetString(SqlDataReader reader, string name) => reader[name] != DBNull.Value ? reader[name].ToString() : null;
            decimal GetDecimal(SqlDataReader reader, string name) => reader[name] != DBNull.Value ? Convert.ToDecimal(reader[name]) : 0;
            int GetInt(SqlDataReader reader, string name) => reader[name] != DBNull.Value ? Convert.ToInt32(reader[name]) : 0;
            bool GetBool(SqlDataReader reader, string name) => reader[name] != DBNull.Value && Convert.ToBoolean(reader[name]);
            using (var reader = SqlHelper.ExecuteReader(sqlcon, CommandType.Text, strSql.ToString(),
                paramsList.ToArray()))
            {
                while (reader.Read())
                {
                    var item = new QueryProductsModel
                    {
                        Oid = GetInt(reader, "oid"),
                        DisplayName = GetString(reader, "DisplayName"), 
                        CP_Brand = GetString(reader, "CP_Brand"), 
                        CP_Tab = GetString(reader, "CP_Tab"),
                        CP_ShuXing5 = GetString(reader, "CP_ShuXing5"), 
                        OnSale = GetBool(reader, "OnSale"),
                        PID = GetString(reader, "PID"),
                        CP_Tire_Pattern = GetString(reader, "CP_Tire_Pattern"), 
                        CP_Tire_Rim = GetString(reader, "CP_Tire_Rim"),
                        cy_marketing_price = GetDecimal(reader, "cy_marketing_price"), 
                        CouponIds = GetString(reader, "CouponIds"), 
                        IsShow = GetBool(reader, "IsShow") ? 1 : 0 
                    };
                    r.Add(item);
                }
            }
            return r;
        }

        /// <summary>
        /// 生成产品列表查询条件
        /// </summary>
        /// <param name="paramsList">参数化赋值集合</param>
        /// <param name="model">查询条件对象</param>
        /// <returns></returns>
        private static string GetQueryProductsCondition(List<SqlParameter> paramsList,SeachProducts model)
        {
            if (model == null)
            {
                return "";
            }
            if (paramsList == null)
            {
                paramsList = new List<SqlParameter>();
            }
            var strbWhere = new StringBuilder();
            strbWhere.Append("  AND tab3.NodeNo like @NodeNo ");
            paramsList.Add(new SqlParameter("@NodeNo", model.Category + "%"));

            //品牌
            if (!string.IsNullOrWhiteSpace(model.Brand))
            {
                var brandValues = model.Brand.Split(',');
                strbWhere.Append("AND tab1.CP_Brand IN(");
                for (int i = 0; i < brandValues.Length; i++)
                {
                    strbWhere.AppendFormat("@CP_Brand{0},",i);
                    paramsList.Add(new SqlParameter("@CP_Brand"+i, brandValues[i]));
                }
                strbWhere = strbWhere.Remove(strbWhere.Length - 1, 1);
                strbWhere.Append(")");
               
            }
            //标签
            if (!string.IsNullOrWhiteSpace(model.Tab))
            {
                var tabValues = model.Tab.Split(',');
                strbWhere.Append("AND tab1.CP_Tab IN(");
                for (int i = 0; i < tabValues.Length; i++)
                {
                    strbWhere.AppendFormat("@CP_Tab{0},", i);
                    paramsList.Add(new SqlParameter("@CP_Tab" + i, tabValues[i]));
                }
                strbWhere = strbWhere.Remove(strbWhere.Length - 1, 1);
                strbWhere.Append(")");
            }
            //尺寸
            if (!string.IsNullOrWhiteSpace(model.Rim))
            {
                var rimValues = model.Rim.Split(',');
                strbWhere.Append("AND tab1.CP_Tire_Rim IN(");
                for (int i = 0; i < rimValues.Length; i++)
                {
                    strbWhere.AppendFormat("@CP_Tire_Rim{0},", i);
                    paramsList.Add(new SqlParameter("@CP_Tire_Rim" + i, rimValues[i]));
                }
                strbWhere = strbWhere.Remove(strbWhere.Length - 1, 1);
                strbWhere.Append(")");
            }
            if (model.PidList != null && model.PidList.Any())
            {
                strbWhere.AppendFormat(" And (tab1.OrigProductID IN('{0}') or tab1.PID in ('{0}'))", string.Join("','", model.PidList));
            }
            //优惠券
            if (!string.IsNullOrWhiteSpace(model.CouponIds))
            {
                strbWhere.Append(" AND tab2.CouponIds LIKE @CouponIds ");
                paramsList.Add(new SqlParameter("@CouponIds", "%" + model.CouponIds.Trim() + "%"));
            }
            //花纹
            if (!string.IsNullOrWhiteSpace(model.Pattern))
            {
                strbWhere.Append("AND tab1.CP_Tire_Pattern=@CP_Tire_Pattern ");
                paramsList.Add(new SqlParameter("@CP_Tire_Pattern", model.Pattern.Trim()));
            }
            //是否显示
            if (model.IsShow.HasValue && model.IsShow.Value >= 0)
            {
                strbWhere.AppendFormat(" AND tab4.IsShow={0} ", model.IsShow.Value);
            }
            //上下架
            if (model.OnSale.HasValue)
            {
                strbWhere.AppendFormat(" AND tab1.OnSale={0} ", model.OnSale.Value);
            }

            return strbWhere.ToString();
        }

        /// <summary>
        /// 根据查询条件获取产品总行数
        /// </summary>
        /// <param name="sqlcon"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int QueryProductsForProductLibraryCount(SqlConnection sqlcon, SeachProducts model)
        {
            StringBuilder strSql = new StringBuilder();
            List<SqlParameter> paramsList = new List<SqlParameter>();
            strSql.Append(@"
             SELECT COUNT(1)           
FROM  Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab3 WITH ( NOLOCK )
  INNER JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1
  WITH ( NOLOCK ) ON tab3.child_oid = tab1.oid AND tab1.PID IS NOT NULL
  LEFT JOIN Configuration..SE_ProductLibraryConfig AS tab2
  WITH ( NOLOCK ) ON tab1.oid = tab2.Oid
 WHERE  1 = 1 
            ");
            //拼接条件
            var strCondition = GetQueryProductsCondition(paramsList, model);
            strSql.Append(strCondition);
            object count = SqlHelper.ExecuteScalar(sqlcon, CommandType.Text, strSql.ToString(), paramsList.ToArray());
            if (count == null)
                return 0;
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 获取产品信息
        /// </summary>
        public static QueryProductsModel QueryProduct(SqlConnection sqlcon, int oid)
        {
            if (oid > 0)
            {
                string strSql = " SELECT tab1.cy_cost,p.* FROM Tuhu_productcatalog..vw_Products AS p WITH(NOLOCK) LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1 WITH (NOLOCK) ON p.oid=tab1.oid WHERE p.Oid = @Oid ";
                return SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, strSql, new SqlParameter("@Oid", oid)).ConvertTo<QueryProductsModel>().FirstOrDefault();
            }
            return null;
        }
        public static IEnumerable<QueryProductsModel> QueryProducts(SqlConnection sqlcon, List<int> oids)
        {

            string strSql = @" WITH    Oids
              AS ( SELECT   *
                   FROM     Gungnir..Split(@Oids, ',')
                 )
        SELECT  *
        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS P WITH ( NOLOCK )
                JOIN Oids ON P.Oid = Oids.col; ";
            return SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, strSql, 
                new SqlParameter("@Oids", string.Join(",", oids))).ConvertTo<QueryProductsModel>();
        }

        public static List<string> GetPattern(SqlConnection sqlcon)
        {
            List<string> list = new List<string>();
            string sql = @" SELECT DISTINCT
                                    CP_Tire_Pattern
                            FROM    Tuhu_productcatalog..[CarPAR_zh-CN] WITH ( NOLOCK )";
            DataTable dt = SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, sql);
            foreach (DataRow item in dt.Rows)
            {
                list.Add(item[0].ToString());
            }
            return list;
        }
        /// <summary>
        /// 批量修改或添加产品
        /// </summary>
        public static MsgResultModel BatchAddCoupon(SqlConnection sqlcon, string oids, string coupons, int isBatch)
        {
            MsgResultModel _MsgResultModel = new MsgResultModel();

            var oidsArr = oids.Split(',');

            bool compareState = false;

            if (oidsArr.Length > 0)
            {
                string queryProductsSQL = "  SELECT DISTINCT Oid FROM Configuration.[dbo].[SE_ProductLibraryConfig] WITH(NOLOCK) WHERE Oid IN (SELECT * FROM [dbo].[SplitString](@oids,',',1))";
                DataTable productsTable = SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, queryProductsSQL, new SqlParameter("@oids", oids)) ?? new DataTable();

                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        List<SqlParameter> paramsList = new List<SqlParameter>();
                        StringBuilder sbUpdate = new StringBuilder();
                        StringBuilder sbInsert = new StringBuilder();                        
                        string updateSQL = "";                        
                        updateSQL = isBatch == 0
                                   ? " UPDATE Configuration.[dbo].[SE_ProductLibraryConfig] SET CouponIds = CouponIds + ',' + @CouponIds WHERE Oid IN (SELECT * FROM [dbo].[SplitString](@OidStr,',',1))"
                                   : " UPDATE Configuration.[dbo].[SE_ProductLibraryConfig] SET CouponIds = @CouponIds WHERE Oid IN (SELECT * FROM [dbo].[SplitString](@OidStr,',',1))";
                      

                        for (int i = 0; i < oidsArr.Length; i++)
                        {
                            for (int j = 0; j < productsTable.Rows.Count; j++)
                            {
                                if (oidsArr[i].Equals(productsTable.Rows[j]["Oid"].ToString()))
                                {
                                    compareState = true;
                                    break;
                                }
                            }

                            if (!compareState)
                            {
                                sbInsert.Append(oidsArr[i]);
                                sbInsert.Append(',');
                            }
                            else
                            {
                                sbUpdate.Append(oidsArr[i]);
                                sbUpdate.Append(',');

                            }
                            compareState = false;
                        }
                        var insertOids = sbInsert.ToString().Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                        sbInsert.Clear();
                        if (insertOids.Any())
                        {
                            MultBatchAddCoupon(sqlcon, tran, insertOids, coupons, isBatch);
                            _MsgResultModel.State = 1;
                        }
                        var updateOids = sbUpdate.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                        sbUpdate.Clear();
                        if (updateOids.Any())
                        {
                            paramsList.Add(new SqlParameter("@OidStr", string.Join(",", updateOids)));
                            paramsList.Add(new SqlParameter("@CouponIds", coupons));
                            if (SqlHelper.ExecuteNonQuery(tran, CommandType.Text, updateSQL.ToString(), paramsList.ToArray()) > 0)
                                _MsgResultModel.State = 1;
                            else
                                _MsgResultModel.State = 0 | _MsgResultModel.State;
                        }              
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        _MsgResultModel.State = 0;
                        _MsgResultModel.Message = ex.ToString();
                        tran.Rollback();
                    }
                }
            }
            return _MsgResultModel;
        }


        public static bool UpdateProductCouponConfig(SqlConnection conn, int oid, List<int> couponPkids)
        {
            string sql = @"
            UPDATE  Configuration..SE_ProductLibraryConfig
            SET     CouponIds = @CouponIds
            WHERE   Oid = @Oid;";
            SqlParameter[] parameter =
                { new SqlParameter("@Oid", oid), new SqlParameter("@CouponIds", string.Join(",",couponPkids)) };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        /// <summary>
        /// 批量删除产品
        /// </summary>
        public static MsgResultModel BatchDeleteCoupon(SqlConnection sqlcon, string oids, string coupon)
        {
            MsgResultModel _MsgResultModel = new MsgResultModel();
            var oidsArr = oids.Split(',');
            bool compareState = false;
            if (oidsArr.Length > 0)
            {
                string queryProductsSQL = "  SELECT DISTINCT Oid FROM Configuration.[dbo].[SE_ProductLibraryConfig] WITH(NOLOCK) WHERE Oid IN (SELECT * FROM [dbo].[SplitString](@oids,',',1))";
                DataTable productsTable = SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, queryProductsSQL, new SqlParameter("@oids", oids)) ?? new DataTable();

                //sql 替换 ，号分割的数据
                //string sql = @"                        
                //        DECLARE @coupon VARCHAR(1000)
                //        DECLARE @result VARCHAR(1000)
                //        SET @coupon = ',' + ( SELECT    CouponIds
                //                                FROM      Configuration.[dbo].[SE_ProductLibraryConfig]
                //                                WHERE     Oid = @Oid
                //                            ) + ','
                //        IF @coupon = @couponnew
                //            BEGIN
                //                UPDATE  Configuration.[dbo].[SE_ProductLibraryConfig]
                //                SET     CouponIds = ''
                //                WHERE   Oid = @Oid
                //            END

                //        IF CHARINDEX(@couponnew, @coupon) > 0
                //            BEGIN 		
                //                BEGIN		
                //                    DECLARE @NewIndex NVARCHAR(200)
                //                    DECLARE @NewIndex1 NVARCHAR(200) 
                //                    SET @NewIndex = SUBSTRING(@coupon, 0,
                //                                                CHARINDEX(@couponnew, @coupon))		
                //                    SET @NewIndex1 = SUBSTRING(@coupon,
                //                                                CHARINDEX(@couponnew, @coupon)
                //                                                + LEN(@couponnew), LEN(@coupon))          
                //                    IF ( LEN(@NewIndex) > 0
                //                            AND LEN(@NewIndex1) > 0
                //                        )
                //                        BEGIN
                //                            SET @result = SUBSTRING(@NewIndex, 2, LEN(@NewIndex))
                //                                + ',' + SUBSTRING(@NewIndex1, 1, LEN(@NewIndex1) - 1) 
                //                        END
                //                    IF ( LEN(@NewIndex) <= 0
                //                            AND LEN(@NewIndex1) > 0
                //                        )
                //                        BEGIN
                //                            SET @result = SUBSTRING(@NewIndex1, 1, LEN(@NewIndex1) - 1) 
                //                        END
                //                    IF ( LEN(@NewIndex) > 0
                //                            AND LEN(@NewIndex1) < = 0
                //                        )
                //                        BEGIN
                //                            SET @result = SUBSTRING(@NewIndex, 2, LEN(@NewIndex)) 

                //                        END 

                //                    UPDATE  Configuration.[dbo].[SE_ProductLibraryConfig]
                //                    SET     CouponIds = @result
                //                    WHERE   Oid = @Oid
                //                END	
                //            END
                //                ";

                string sql = "UPDATE Configuration.[dbo].[SE_ProductLibraryConfig] SET CouponIds=@CouponIds WHERE Oid=@Oid";
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < oidsArr.Length; i++)
                        {
                            for (int j = 0; j < productsTable.Rows.Count; j++)
                            {
                                if (oidsArr[i].Equals(productsTable.Rows[j]["Oid"].ToString()))
                                {
                                    compareState = true;
                                    break;
                                }
                            }
                            if (compareState)
                            {
                                string sqlSelect = "SELECT CouponIds FROM Configuration.[dbo].[SE_ProductLibraryConfig] WITH(NOLOCK)  WHERE Oid =@oid";

                                object coupons = SqlHelper.ExecuteScalar(tran, CommandType.Text, sqlSelect, new SqlParameter("@oid", oidsArr[i]));
                                if (coupons != DBNull.Value && coupons != null)
                                {
                                    string couponNew = string.Empty;
                                    var couponArr = coupons.ToString()?.Split(',');
                                    foreach (var item in couponArr)
                                    {
                                        if (item.Trim() != coupon.Trim())
                                        {
                                            couponNew += item.Trim() + ",";
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(couponNew))
                                    {
                                        couponNew = couponNew.Substring(0, couponNew.Length - 1);
                                    }
                                    var prams = new SqlParameter[]
                                    {
                                        new SqlParameter("@CouponIds",couponNew),
                                        new SqlParameter("@Oid",oidsArr[i])
                                     };
                                    SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, prams);
                                }

                            }
                        }

                        _MsgResultModel.State = 1;

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        _MsgResultModel.State = 0;
                        _MsgResultModel.Message = ex.ToString();
                        tran.Rollback();
                    }
                }
            }
            return _MsgResultModel;
        }

        public static MsgResultModel BatchDeleteCoupon(SqlConnection sqlcon, string oids, string coupons, int isBatch)
        {
            MsgResultModel _MsgResultModel = new MsgResultModel();

            var oidsArr = oids.Split(',');

            bool compareState = false;

            if (oidsArr.Length > 0)
            {
                string queryProductsSQL = "SELECT DISTINCT Oid FROM Configuration.[dbo].[SE_ProductLibraryConfig] WITH(NOLOCK) WHERE Oid IN(" + oids + ")";
                DataTable productsTable = SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, queryProductsSQL, null) ?? new DataTable();

                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        List<SqlParameter> paramsList = new List<SqlParameter>();
                        StringBuilder sbUpdate = new StringBuilder();
                        StringBuilder sbInsert = new StringBuilder();
                        string execSQL = "",
                               updateSQL =
                               isBatch == 0
                                    ? " UPDATE Configuration.[dbo].[SE_ProductLibraryConfig] SET CouponIds = CouponIds + ',' + {0} WHERE Oid = {1} "
                                    : " UPDATE Configuration.[dbo].[SE_ProductLibraryConfig] SET CouponIds = {0} WHERE Oid = {1} ",
                               insertSQL = " INSERT INTO Configuration.[dbo].[SE_ProductLibraryConfig](Oid,CouponIds,State,CreateTime) VALUES({0},{1},{2},{3}) ";

                        for (int i = 0; i < oidsArr.Length; i++)
                        {
                            for (int j = 0; j < productsTable.Rows.Count; j++)
                            {
                                if (oidsArr[i].Equals(productsTable.Rows[j]["Oid"].ToString()))
                                {
                                    compareState = true;
                                    break;
                                }
                            }

                            if (!compareState)
                            {
                                sbInsert.AppendFormat(insertSQL, "@Oid" + i, "@CouponIds" + i, "@State" + i, "@CreateTime" + i);
                                paramsList.Add(new SqlParameter("@Oid" + i, oidsArr[i]));
                                paramsList.Add(new SqlParameter("@CouponIds" + i, coupons));
                                paramsList.Add(new SqlParameter("@State" + i, 1));
                                paramsList.Add(new SqlParameter("@CreateTime" + i, DateTime.Now));
                            }
                            else
                            {
                                sbUpdate.AppendFormat(updateSQL, "@CouponIds" + i, "@Oid" + i);
                                paramsList.Add(new SqlParameter("@Oid" + i, oidsArr[i]));
                                paramsList.Add(new SqlParameter("@CouponIds" + i, coupons));
                            }
                            compareState = false;
                        }

                        execSQL = sbInsert.ToString() + sbUpdate.ToString();

                        if (SqlHelper.ExecuteNonQuery(tran, CommandType.Text, execSQL.ToString(), paramsList.ToArray()) > 0)
                            _MsgResultModel.State = 1;
                        else
                            _MsgResultModel.State = 0;

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        _MsgResultModel.State = 0;
                        _MsgResultModel.Message = ex.ToString();
                        tran.Rollback();
                    }
                }
            }
            return _MsgResultModel;
        }
        /// <summary>
        /// 获取 品牌，标签，尺寸 
        /// </summary>
        public static DataSet GetFilterCondition( string category)
        {
            StringBuilder sbsql = new StringBuilder();

            sbsql.AppendLine(@"
                                SELECT DISTINCT
                                        CC.CP_Brand AS 'Name'
                                FROM    Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab1 WITH ( NOLOCK )
                                        INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN_Catalog] AS CC WITH ( NOLOCK ) ON tab1.oid = CC.#Catalog_Lang_Oid
                                WHERE   NodeNo LIKE  @category 
                                        AND LEN(ISNULL(CC.CP_Brand, '')) > 0
                                ORDER BY CC.CP_Brand;  ");

            sbsql.AppendLine(@"
                                SELECT DISTINCT
                                        tab2.CP_Tab AS 'Name'
                                FROM    Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab1 WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog.dbo.CarPAR_CatalogProducts AS tab2 WITH ( NOLOCK ) ON tab1.child_oid = tab2.oid
                                        INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN_Catalog] AS CC WITH ( NOLOCK ) ON tab2.oid = CC.#Catalog_Lang_Oid
                                WHERE   NodeNo LIKE  @category 
                                        AND LEN(ISNULL(tab2.CP_Tab, '')) > 0
                                ORDER BY tab2.CP_Tab;  ");

            sbsql.AppendLine(@"
                                SELECT DISTINCT
                                        CC.CP_Tire_Rim AS 'Name'
                                FROM    Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab1 WITH ( NOLOCK )
                                        INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN_Catalog] AS CC WITH ( NOLOCK ) ON tab1.oid = CC.#Catalog_Lang_Oid
                                WHERE   NodeNo LIKE @category 
                                        AND LEN(ISNULL(CC.CP_Tire_Rim, '')) > 0
                                ORDER BY CC.CP_Tire_Rim;");
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (var cmd = new SqlCommand(sbsql.ToString()))
                {
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@category",
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        Value = category + "%"
                    });
                    //cmd.Parameters.Add("@category", SqlDbType.NVarChar, 50, category + "%");
                    return dbHelper.ExecuteDataSet(cmd);
                }
            }
        }

        public static void MultBatchAddCoupon(SqlConnection sqlcon, SqlTransaction transaction, List<string> oidArrs, string coupons, int isBatch)
        {

            using (var sbc = new SqlBulkCopy(sqlcon, SqlBulkCopyOptions.KeepIdentity, transaction))
            {
                sbc.BatchSize = 1000;
                sbc.BulkCopyTimeout = 300;
                sbc.DestinationTableName = "Configuration.[dbo].[SE_ProductLibraryConfig]";
                DataTable table = new DataTable();
                table.Columns.Add("Oid");
                table.Columns.Add("CouponIds");
                table.Columns.Add("State");
                table.Columns.Add("CreateTime");
                foreach (DataColumn col in table.Columns)
                {
                    sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                foreach (var oid in oidArrs)
                {
                    var row = table.NewRow();
                    row["Oid"] = oid;
                    row["CouponIds"] = coupons;
                    row["State"] = 1;
                    row["CreateTime"] = DateTime.Now;
                    table.Rows.Add(row);
                }
                sbc.WriteToServer(table);
            }
        }
    }
}
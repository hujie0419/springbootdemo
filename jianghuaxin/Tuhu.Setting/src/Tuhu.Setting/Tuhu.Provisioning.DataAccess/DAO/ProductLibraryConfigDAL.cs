
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
        /// 查询产品信息
        /// </summary>
        public static IEnumerable<QueryProductsModel> QueryProducts(SqlConnection sqlcon, SeachProducts model)
        {
            if (!string.IsNullOrWhiteSpace(model.category))
            {
                string orderBySql = "";
                StringBuilder strSql = new StringBuilder();
                StringBuilder sbWhere = new StringBuilder();
                List<SqlParameter> paramsList = new List<SqlParameter>();

                strSql.Append(" SELECT * FROM ( ");
                strSql.Append(@"SELECT  *
                                FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY tab1.oid ) AS 'RowNumber' ,
                                                    tab1.PrimaryParentCategory AS Category ,
                                                    tab1.oid ,
                                                    tab1.DisplayName ,
                                                    CP_Brand ,
                                                    tab1.CP_Tab ,
                                                    tab1.CP_ShuXing5 ,
                                                    tab1.OnSale ,
                                                    tab1.PID ,
                                                    CP_Tire_Pattern ,
                                                    CP_Tire_Rim ,
                                                    tab1.cy_marketing_price ,
                                                    tab2.CouponIds ,
                                                    tab3.NodeNo ,
					                                tab4.IsShow              
                                          FROM      Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab3 WITH ( NOLOCK )
                                                    INNER JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1
                                                    WITH ( NOLOCK ) ON tab3.child_oid = tab1.oid
					                                INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_CatalogProducts] AS tab4 WITH(NOLOCK) ON tab1.oid=tab4.oid
                                                    LEFT JOIN Configuration..SE_ProductLibraryConfig AS tab2
                                                    WITH ( NOLOCK ) ON tab1.oid = tab2.Oid
                                          WHERE     1 = 1
                                        ) AS Result
                                WHERE   1 = 1 ");

                sbWhere.Append(" AND Result.NodeNo LIKE ''+ @NodeNo +'%' AND Result.PID IS NOT NULL ");
                paramsList.Add(new SqlParameter("@NodeNo", model.category));

                #region 筛选条件
                //品牌
                if (!string.IsNullOrWhiteSpace(model.brand))
                {
                    var brandValues = model.brand.Split(',');
                    var brandField = new string[brandValues.Length];
                    for (int i = 0; i < brandValues.Length; i++)
                    {
                        brandField[i] = "@CP_Brand" + i;
                        paramsList.Add(new SqlParameter(brandField[i], brandValues[i]));
                    }
                    sbWhere.Append(" AND Result.CP_Brand IN(" + string.Join(",", brandField) + ") ");
                }
                //标签
                if (!string.IsNullOrWhiteSpace(model.tab))
                {
                    sbWhere.Append(" AND( ");
                    var tabValues = model.tab.Split(',');
                    string tabField = "";
                    for (int i = 0; i < tabValues.Length; i++)
                    {
                        tabField = "@CP_Tab" + i;
                        sbWhere.Append(" Result.CP_Tab LIKE '%'+ " + tabField + " +'%' ");
                        if (tabValues.Length > 0 && i < tabValues.Length - 1) { sbWhere.Append(" OR "); }
                        paramsList.Add(new SqlParameter(tabField, tabValues[i]));
                    }
                    sbWhere.Append(" ) ");
                }
                //尺寸
                if (!string.IsNullOrWhiteSpace(model.rim))
                {
                    var rimValues = model.rim.Split(',');
                    var rimField = new string[rimValues.Length];
                    for (int i = 0; i < rimValues.Length; i++)
                    {
                        rimField[i] = "@CP_Tire_Rim" + i;
                        paramsList.Add(new SqlParameter(rimField[i], rimValues[i]));
                    }
                    sbWhere.Append(" AND Result.CP_Tire_Rim IN(" + string.Join(",", rimField) + ") ");
                }
                //PID
                if (!string.IsNullOrWhiteSpace(model.pid))
                {
                    sbWhere.Append(" AND Result.PID LIKE '%'+ @PID +'%' ");
                    paramsList.Add(new SqlParameter("@PID", model.pid.Trim()));
                }
                //优惠券
                if (!string.IsNullOrWhiteSpace(model.couponIds))
                {
                    sbWhere.Append(" AND Result.CouponIds LIKE '%'+ @CouponIds +'%' ");
                    paramsList.Add(new SqlParameter("@CouponIds", model.couponIds.Trim()));
                }
                //花纹
                if (!string.IsNullOrWhiteSpace(model.pattern))
                {
                    sbWhere.Append("AND Result.CP_Tire_Pattern LIKE '%'+ @CP_Tire_Pattern +'%' ");
                    paramsList.Add(new SqlParameter("@CP_Tire_Pattern", model.pattern.Trim()));
                }
                //价格
                //if (!string.IsNullOrWhiteSpace(model.price))
                //{
                //    var brandValues = model.price.Split('|');
                //    if (brandValues.Length == 2)
                //    {
                //        paramsList.Add(new SqlParameter("@begin_cy_list_price", brandValues[0].Trim()));
                //        paramsList.Add(new SqlParameter("@end_cy_list_price", brandValues[1].Trim()));
                //        sbWhere.Append(" AND Result.cy_list_price BETWEEN @begin_cy_list_price AND @end_cy_list_price ");
                //    }
                //}
               
                //毛利
                //if (!string.IsNullOrWhiteSpace(model.maoli))
                //{
                //    var maolis = model.maoli.Split('|');
                //    if (!string.IsNullOrWhiteSpace(maolis[0]) && !string.IsNullOrWhiteSpace(maolis[1]))
                //    {
                //        paramsList.Add(new SqlParameter("@begin_maoli_price", maolis[0].Trim()));
                //        paramsList.Add(new SqlParameter("@end_maoli_price", maolis[1].Trim()));
                //        sbWhere.Append(" AND Result.Maoli BETWEEN @begin_maoli_price AND @end_maoli_price  ");
                //    }
                //}
                //成本价
                //if(!string.IsNullOrEmpty(model.CostPrice))
                //{
                //    var costPrice = model.CostPrice.Split('|');
                //    if (costPrice.Length == 2)
                //    {
                //        paramsList.Add(new SqlParameter("@begin_cyCost_list_price", costPrice[0].Trim()));
                //        paramsList.Add(new SqlParameter("@end_cyCost_list_price", costPrice[1].Trim()));
                //        sbWhere.Append(" AND Result.cy_cost BETWEEN @begin_cyCost_list_price AND @end_cyCost_list_price ");
                //    }

                //}

                //isshow
                if (!string.IsNullOrWhiteSpace(model.isShow))
                {
                    if (model.isShow.Equals("1"))
                    {
                        sbWhere.Append(" AND Result.IsShow=1  ");
                    }
                    else if (model.isShow.Equals("0"))
                    {
                        sbWhere.Append(" AND Result.IsShow=0  ");
                    }
                }

                //上下架
                if (!string.IsNullOrWhiteSpace(model.onSale))
                {
                    if (model.onSale.Equals("上架"))
                    {
                        sbWhere.Append(" AND Result.OnSale=1  ");
                    }
                    else if (model.onSale.Equals("下架"))
                    {
                        sbWhere.Append(" AND Result.OnSale=0  ");
                    }
                }

                //if (!string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    model.maoliSort = model.maoliSort.Equals("升序") ? "ASC" : (model.maoliSort.Equals("降序") ? "DESC" : "");
                //}
                //排序
                //if (!string.IsNullOrWhiteSpace(model.soft) && !string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    orderBySql = string.Format(" ORDER BY tab1.cy_list_price {0} ,tab1.Maoli {1} ", model.soft, model.maoliSort);
                //}
                //if (!string.IsNullOrWhiteSpace(model.soft) && string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    orderBySql = string.Format(" ORDER BY tab1.cy_list_price {0} ", model.soft);
                //}
                //if (string.IsNullOrWhiteSpace(model.soft) && !string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    orderBySql = string.Format(" ORDER BY tab1.Maoli {0} ", model.maoliSort);
                //}
                #endregion

                string quereyCountSQL = @" SELECT  COUNT(1)
                                           FROM    (SELECT   
                                                    tab1.PrimaryParentCategory AS Category ,
                                                    tab1.oid ,
                                                    tab1.DisplayName ,
                                                    CP_Brand ,
                                                    tab1.CP_Tab ,
                                                    tab1.CP_ShuXing5 ,
                                                    tab1.OnSale ,
                                                    tab1.PID ,
                                                    CP_Tire_Pattern ,
                                                    CP_Tire_Rim ,
                                                    tab1.cy_marketing_price ,
                                                    tab2.CouponIds ,
                                                    tab3.NodeNo ,
					                                tab4.IsShow              
                                          FROM      Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab3 WITH ( NOLOCK )
                                                    INNER JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1
                                                    WITH ( NOLOCK ) ON tab3.child_oid = tab1.oid
					                                INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_CatalogProducts] AS tab4 WITH(NOLOCK) ON tab1.oid=tab4.oid
                                                    LEFT JOIN Configuration..SE_ProductLibraryConfig AS tab2
                                                    WITH ( NOLOCK ) ON tab1.oid = tab2.Oid
                                          WHERE     1 = 1
                                                    ) AS Result
                                            WHERE   1 = 1 " + sbWhere.ToString();


                int quereyCount = (int)SqlHelper.ExecuteScalar(sqlcon, CommandType.Text, quereyCountSQL, paramsList.ToArray()), pageCount = 0;

                if (quereyCount % model.pageSize == 0)
                    pageCount = quereyCount / model.pageSize;
                else
                    pageCount = (quereyCount / model.pageSize) + 1;

                strSql.Append(sbWhere);
                strSql.Append(" )AS tab1 ");
                if (!string.IsNullOrWhiteSpace(orderBySql))
                {
                    strSql.Append(orderBySql);
                }
                else
                {
                    strSql.Append(" ORDER BY tab1.oid ASC  ");
                }

                strSql.AppendFormat("  OFFSET {0} ROWS  FETCH NEXT {1} ROWS ONLY  ", (model.pageIndex - 1) * model.pageSize, model.pageSize);

                var resultData = SqlHelper.ExecuteDataTable(sqlcon, CommandType.Text, strSql.ToString(), paramsList.ToArray()).ConvertTo<QueryProductsModel>() ?? null;

                if (resultData != null)
                {
                    resultData.ToList().ForEach(w =>
                    {
                        w.PageCount = pageCount;
                    });
                }
                return resultData;
            }
            return null;
        }

        public static IEnumerable<QueryProductsModel> QueryProductsForProductLibrary(SqlConnection sqlcon, SeachProducts model)
        {
            if (!string.IsNullOrWhiteSpace(model.category))
            {
                StringBuilder strSql = new StringBuilder();
                StringBuilder sbWhere = new StringBuilder();
                List<SqlParameter> paramsList = new List<SqlParameter>();

                //strSql.Append(" SELECT * FROM ( ");
                strSql.Append(@"
                                SELECT    
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
                                          FROM      Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab3 WITH ( NOLOCK )
                                                    INNER JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1
                                                    WITH ( NOLOCK ) ON tab3.child_oid = tab1.oid
					                                INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_CatalogProducts] AS tab4 WITH(NOLOCK) ON tab1.oid=tab4.oid
                                                    LEFT JOIN Configuration..SE_ProductLibraryConfig AS tab2
                                                    WITH ( NOLOCK ) ON tab1.oid = tab2.Oid
                                          WHERE  1 = 1 ");

                sbWhere.Append(" AND tab3.NodeNo LIKE ''+ @NodeNo +'%' AND tab1.PID IS NOT NULL ");
                paramsList.Add(new SqlParameter("@NodeNo", model.category));

                #region 筛选条件
                //品牌
                if (!string.IsNullOrWhiteSpace(model.brand))
                {
                    var brandValues = model.brand.Split(',');
                    var brandField = new string[brandValues.Length];
                    for (int i = 0; i < brandValues.Length; i++)
                    {
                        brandField[i] = "@CP_Brand" + i;
                        paramsList.Add(new SqlParameter(brandField[i], brandValues[i]));
                    }
                    sbWhere.Append(" AND tab1.CP_Brand IN(" + string.Join(",", brandField) + ") ");
                }
                //标签
                if (!string.IsNullOrWhiteSpace(model.tab))
                {
                    sbWhere.Append(" AND( ");
                    var tabValues = model.tab.Split(',');
                    string tabField = "";
                    for (int i = 0; i < tabValues.Length; i++)
                    {
                        tabField = "@CP_Tab" + i;
                        sbWhere.Append(" tab1.CP_Tab LIKE '%'+ " + tabField + " +'%' ");
                        if (tabValues.Length > 0 && i < tabValues.Length - 1) { sbWhere.Append(" OR "); }
                        paramsList.Add(new SqlParameter(tabField, tabValues[i]));
                    }
                    sbWhere.Append(" ) ");
                }
                //尺寸
                if (!string.IsNullOrWhiteSpace(model.rim))
                {
                    var rimValues = model.rim.Split(',');
                    var rimField = new string[rimValues.Length];
                    for (int i = 0; i < rimValues.Length; i++)
                    {
                        rimField[i] = "@CP_Tire_Rim" + i;
                        paramsList.Add(new SqlParameter(rimField[i], rimValues[i]));
                    }
                    sbWhere.Append(" AND tab1.CP_Tire_Rim IN(" + string.Join(",", rimField) + ") ");
                }
                //PID
                if (!string.IsNullOrWhiteSpace(model.pid))
                {
                    sbWhere.Append(" AND tab1.PID LIKE '%'+ @PID +'%' ");
                    paramsList.Add(new SqlParameter("@PID", model.pid.Trim()));
                }
                //优惠券
                if (!string.IsNullOrWhiteSpace(model.couponIds))
                {
                    sbWhere.Append(" AND tab2.CouponIds LIKE '%'+ @CouponIds +'%' ");
                    paramsList.Add(new SqlParameter("@CouponIds", model.couponIds.Trim()));
                }
                //花纹
                if (!string.IsNullOrWhiteSpace(model.pattern))
                {
                    sbWhere.Append("AND tab1.CP_Tire_Pattern LIKE '%'+ @CP_Tire_Pattern +'%' ");
                    paramsList.Add(new SqlParameter("@CP_Tire_Pattern", model.pattern.Trim()));
                }
                //价格
                //if (!string.IsNullOrWhiteSpace(model.price))
                //{
                //    var brandValues = model.price.Split('|');
                //    if (brandValues.Length == 2)
                //    {
                //        paramsList.Add(new SqlParameter("@begin_cy_list_price", brandValues[0].Trim()));
                //        paramsList.Add(new SqlParameter("@end_cy_list_price", brandValues[1].Trim()));
                //        sbWhere.Append(" AND Result.cy_list_price BETWEEN @begin_cy_list_price AND @end_cy_list_price ");
                //    }
                //}

                //毛利
                //if (!string.IsNullOrWhiteSpace(model.maoli))
                //{
                //    var maolis = model.maoli.Split('|');
                //    if (!string.IsNullOrWhiteSpace(maolis[0]) && !string.IsNullOrWhiteSpace(maolis[1]))
                //    {
                //        paramsList.Add(new SqlParameter("@begin_maoli_price", maolis[0].Trim()));
                //        paramsList.Add(new SqlParameter("@end_maoli_price", maolis[1].Trim()));
                //        sbWhere.Append(" AND Result.Maoli BETWEEN @begin_maoli_price AND @end_maoli_price  ");
                //    }
                //}
                //成本价
                //if(!string.IsNullOrEmpty(model.CostPrice))
                //{
                //    var costPrice = model.CostPrice.Split('|');
                //    if (costPrice.Length == 2)
                //    {
                //        paramsList.Add(new SqlParameter("@begin_cyCost_list_price", costPrice[0].Trim()));
                //        paramsList.Add(new SqlParameter("@end_cyCost_list_price", costPrice[1].Trim()));
                //        sbWhere.Append(" AND Result.cy_cost BETWEEN @begin_cyCost_list_price AND @end_cyCost_list_price ");
                //    }

                //}

                //isshow
                if (!string.IsNullOrWhiteSpace(model.isShow))
                {
                    if (model.isShow.Equals("1"))
                    {
                        sbWhere.Append(" AND tab4.IsShow=1  ");
                    }
                    else if (model.isShow.Equals("0"))
                    {
                        sbWhere.Append(" AND tab4.IsShow=0  ");
                    }
                }

                //上下架
                if (!string.IsNullOrWhiteSpace(model.onSale))
                {
                    if (model.onSale.Equals("上架"))
                    {
                        sbWhere.Append(" AND tab1.OnSale=1  ");
                    }
                    else if (model.onSale.Equals("下架"))
                    {
                        sbWhere.Append(" AND tab1.OnSale=0  ");
                    }
                }

                //if (!string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    model.maoliSort = model.maoliSort.Equals("升序") ? "ASC" : (model.maoliSort.Equals("降序") ? "DESC" : "");
                //}
                //排序
                //if (!string.IsNullOrWhiteSpace(model.soft) && !string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    orderBySql = string.Format(" ORDER BY tab1.cy_list_price {0} ,tab1.Maoli {1} ", model.soft, model.maoliSort);
                //}
                //if (!string.IsNullOrWhiteSpace(model.soft) && string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    orderBySql = string.Format(" ORDER BY tab1.cy_list_price {0} ", model.soft);
                //}
                //if (string.IsNullOrWhiteSpace(model.soft) && !string.IsNullOrWhiteSpace(model.maoliSort))
                //{
                //    orderBySql = string.Format(" ORDER BY tab1.Maoli {0} ", model.maoliSort);
                //}
                #endregion

                //string quereyCountSQL = @" SELECT  COUNT(1)
                //                           FROM    (SELECT   
                //                                    tab1.PrimaryParentCategory AS Category ,
                //                                    tab1.oid ,
                //                                    tab1.DisplayName ,
                //                                    CP_Brand ,
                //                                    tab1.CP_Tab ,
                //                                    tab1.CP_ShuXing5 ,
                //                                    tab1.OnSale ,
                //                                    tab1.PID ,
                //                                    CP_Tire_Pattern ,
                //                                    CP_Tire_Rim ,
                //                                    tab1.cy_marketing_price ,
                //                                    tab2.CouponIds ,
                //                                    tab3.NodeNo ,
                //                     tab4.IsShow              
                //                          FROM      Tuhu_productcatalog..CarPAR_CatalogHierarchy AS tab3 WITH ( NOLOCK )
                //                                    INNER JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS tab1
                //                                    WITH ( NOLOCK ) ON tab3.child_oid = tab1.oid
                //                     INNER JOIN Tuhu_productcatalog.dbo.[CarPAR_CatalogProducts] AS tab4 WITH(NOLOCK) ON tab1.oid=tab4.oid
                //                                    LEFT JOIN Configuration..SE_ProductLibraryConfig AS tab2
                //                                    WITH ( NOLOCK ) ON tab1.oid = tab2.Oid
                //                          WHERE     1 = 1
                //                                    ) AS Result
                //                            WHERE   1 = 1 " + sbWhere.ToString();


                //int quereyCount = (int)SqlHelper.ExecuteScalar(sqlcon, CommandType.Text, quereyCountSQL, paramsList.ToArray()), pageCount = 0;

                //if (quereyCount % model.pageSize == 0)
                //    pageCount = quereyCount / model.pageSize;
                //else
                //    pageCount = (quereyCount / model.pageSize) + 1;

                strSql.Append(sbWhere);
                //strSql.Append(" )AS tab1 ");
                //if (!string.IsNullOrWhiteSpace(orderBySql))
                //{
                //    strSql.Append(orderBySql);
                //}
                //else
                //{
                //    strSql.Append(" ORDER BY tab1.oid ASC  ");
                //}

                //strSql.AppendFormat("  OFFSET {0} ROWS  FETCH NEXT {1} ROWS ONLY  ", (model.pageIndex - 1) * model.pageSize, model.pageSize);
                var r = new List<QueryProductsModel>();
                string GetString(SqlDataReader reader, int i) => reader[i] != DBNull.Value ? reader.GetString(i) : null;
                decimal GetDecimal(SqlDataReader reader, int i) => reader[i] != DBNull.Value ? reader.GetDecimal(i) : 0;
                int GetInt(SqlDataReader reader,int i) => reader[i] != DBNull.Value ? reader.GetInt32(i) : 0;
                bool GetBool(SqlDataReader reader, int i) => reader[i] != DBNull.Value && reader.GetBoolean(i);
                using (var reader = SqlHelper.ExecuteReader(sqlcon, CommandType.Text, strSql.ToString(),
                    paramsList.ToArray()))
                {
                    while (reader.Read())
                    {
                        var item = new QueryProductsModel
                        {
                            Oid = GetInt(reader, 1), //oid
                            DisplayName = GetString(reader, 2), //x.GetValue<string>("DisplayName"),
                            CP_Brand = GetString(reader, 3), //x.GetValue<string>("CP_Brand"),
                            CP_Tab = GetString(reader, 4), //x.GetValue<string>("CP_Tab"),
                            CP_ShuXing5 = GetString(reader, 5), //x.GetValue<string>("CP_ShuXing5"),
                            OnSale = GetBool(reader, 6).ToString(), //x.GetValue<string>("OnSale"),
                            PID = GetString(reader, 7), //x.GetValue<string>("PID"),
                            CP_Tire_Pattern = GetString(reader, 8), //x.GetValue<string>("CP_Tire_Pattern"),
                            CP_Tire_Rim = GetString(reader, 9), //x.GetValue<string>("CP_Tire_Rim"),
                            cy_marketing_price = GetDecimal(reader, 10), //x.GetValue<decimal>("cy_marketing_price"),
                            CouponIds = GetString(reader, 11), //x.GetValue<string>("CouponIds"),
                            IsShow = GetBool(reader, 13) ? 1 : 0 //x.GetValue<int>("IsShow"),
                        };
                        r.Add(item);
                    }
                }
                //if (resultData != null)
                //{
                //    resultData.ToList().ForEach(w =>
                //    {
                //        w.PageCount = pageCount;
                //    });
                //}
                return r;
            }
            return null;
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
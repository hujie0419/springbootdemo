using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using System.Configuration;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess
{
    public static class DalRegion
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string strConnAlwaysOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;

        private static SqlConnection conn = new SqlConnection(strConn.Decrypt());
        private static SqlConnection connAlwaysOnRead = new SqlConnection(strConnAlwaysOnRead.Decrypt());
        public static List<BizRegion> SelectRegions(int parent = 0)
        {
            var sql = "select PKID as Pkid,RegionName,ProvinceID,CityID from tbl_region where (ParentID=@ParentID OR @ParentID=-1) AND IsActive=1 ";
            #region 参数
            var sqlParamters = new[]
            {
                 new SqlParameter("@ParentID", parent)
            };
            #endregion
            var list = SqlHelper.ExecuteDataTable(connAlwaysOnRead, CommandType.Text, sql, sqlParamters).ConvertTo<BizRegion>().ToList();
            BizRegion region = new BizRegion();
            region.Pkid = -1;
            region.RegionName = "-请选择-";
            list.Add(region);
            return list.OrderBy(n => n.Pkid).ToList();
        }
        public static List<BizRegion> SelectRegionByName(string RegionName)
        {
            var sql = "select PKID as Pkid,RegionName,ProvinceID,CityID from tbl_region where RegionName=@RegionName AND IsActive=1 ";
            #region 参数
            var sqlParamters = new[]
            {
                 new SqlParameter("@RegionName", RegionName)
            };
            #endregion
            var list = SqlHelper.ExecuteDataTable(connAlwaysOnRead, CommandType.Text, sql, sqlParamters).ConvertTo<BizRegion>().ToList();
            return list.OrderBy(n => n.Pkid).ToList();
        }

        public static BizRegion GetRegionsOne(int pkid)
        {
            var sql = "select PKID as Pkid,RegionName,PinYin,ProvinceID,CityID from tbl_region where PKID=@PKID  AND IsActive=1 ";
            #region 参数
            var sqlParamters = new[]
            {
                 new SqlParameter("@PKID", pkid)
            };
            #endregion
            var list = SqlHelper.ExecuteDataTable(connAlwaysOnRead, CommandType.Text, sql, sqlParamters).ConvertTo<BizRegion>().ToList();
            BizRegion region = new BizRegion();
            region.Pkid = list[0].Pkid;
            region.RegionName = list[0].RegionName;
            region.PinYin = list[0].PinYin;
            region.ProvinceId = list[0].ProvinceId;
            region.CityId = list[0].CityId;
            return region;
        }

        public static bool UpdateRegionIsInstall(int PKID, string regionName, System.Data.SqlClient.SqlTransaction sqlbulkTransaction)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" update tbl_Region set IsInstall=1 where PKID=@PKID or RegionName=@RegionName ");
            #region 参数
            var sqlParamters = new[]
            {
                 new SqlParameter("@PKID", PKID),
                 new SqlParameter("@RegionName", regionName)
            };
            #endregion

            int i = SqlHelper.ExecuteNonQuery(sqlbulkTransaction, CommandType.Text, strSql.ToString(), sqlParamters);
            if (i > 0)
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<ShopRegion> SelectRegionDetail(int parentID)
        {
            string sql = @" SELECT  C.PKID ,
                                    C.RegionName ,
                                    C.PinYin
                            FROM    dbo.tbl_region AS C WITH ( NOLOCK )
                            WHERE   C.ParentID = @ParentID
                                    AND PKID NOT IN ( 32, 33, 34 );";
            //var cmd = new SqlCommand("[Gungnir].dbo.[Member_Region_SelectRegionDetail]")
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            var cmd = new SqlCommand(sql)
            {
                CommandType = CommandType.Text
            };
            #region AddParameters
            cmd.Parameters.AddWithValue("@ParentID", parentID);
            #endregion

            return DbHelper.ExecuteDataTable(cmd).ConvertTo<ShopRegion>().ToList();
        }

        public static Region GetRegion(SqlConnection connection, int pkid)
        {
            Region region = null;

            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            string sql = @"	SELECT
									[PKID],
									[RegionName],
									[Zipcode],
									[Phone],
									[Disabled],
									[ProvinceID],
									[CityID],
									[DistrictID],
									[IsBusiness],
									[IsInstall],
									[Tag],
									[ParentID],
									[IsActive],
									[LastUpdateTime],
									[TuhuStockID],
									[LogisticStockID],
									[ExpCompany],
									[LogisticCo],
									[ArriveShopExpCo],
									BYTuhuStockID
								FROM
									[Gungnir].[dbo].[tbl_Region] WITH(NOLOCK)
								WHERE
									[PKID] = @PKID";
            //using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Region_GetRegionByPKID", parameters))
            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, parameters))
            {
                if (dataReader.Read())
                {
                    region = new Region();

                    region.PKID = dataReader.GetTuhuValue<int>(0);
                    region.RegionName = dataReader.GetTuhuString(1);
                    region.Zipcode = dataReader.GetTuhuString(2);
                    region.Phone = dataReader.GetTuhuString(3);
                    region.Disabled = dataReader.GetTuhuValue<bool>(4);
                    region.ProvinceID = dataReader.GetTuhuValue<int>(5);
                    region.CityID = dataReader.GetTuhuValue<int>(6);
                    region.DistrictID = dataReader.GetTuhuValue<int>(7);
                    region.IsBusiness = dataReader.GetTuhuNullableValue<bool>(8);
                    region.IsInstall = dataReader.GetTuhuNullableValue<int>(9);
                    region.Tag = dataReader.GetTuhuString(10);
                    region.ParentID = dataReader.GetTuhuNullableValue<int>(11);
                    region.IsActive = dataReader.GetTuhuValue<bool>(12);
                    region.LastUpdateTime = dataReader.GetTuhuValue<System.DateTime>(13);
                    region.TuhuStockID = dataReader.GetTuhuNullableValue<int>(14);
                    region.LogisticStockID = dataReader.GetTuhuNullableValue<int>(15);
                    region.ExpCompany = dataReader.GetTuhuString(16);
                    region.LogisticCo = dataReader.GetTuhuString(17);
                    region.ArriveShopExpCo = dataReader.GetTuhuString(18);
                    region.BYTuhuStockID = dataReader.GetTuhuNullableValue<int>(19);
                }
            }

            return region;
        }

        public static List<CPRegion> SelectCPRegions(SqlConnection connection, int shopId)
        {
            var parameters = new[]
            {
               new SqlParameter("@ShopId", shopId)
            };

            var cPRegions = new List<CPRegion>();
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Region_SelectCPRegionsByShopId", parameters))
            {
                while (reader.Read())
                {
                    var cPRegion = new CPRegion();
                    cPRegion.PKID = reader.GetTuhuValue<int>(0);
                    cPRegion.ShopId = reader.GetTuhuValue<int>(1);
                    cPRegion.RegionId = reader.GetTuhuValue<int>(2);
                    cPRegion.IsOnlyShop = reader.GetTuhuValue<bool>(3);
                    cPRegion.LastUpdateTime = reader.GetTuhuValue<DateTime>(4);

                    cPRegions.Add(cPRegion);
                }
            }

            return cPRegions;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        public static void AddRegion(BizRegion model)
        {
            string sql = string.Format(@"
                                            INSERT INTO dbo.tbl_Region
		                                ( RegionName,
		                                  Zipcode,
		                                  Phone,
		                                  Disabled,
		                                  ProvinceID,
		                                  CityID,
		                                  DistrictID,
		                                  IsBusiness,
		                                  IsInstall,
		                                  Tag,
		                                  ParentID,
		                                  IsActive,
		                                  LastUpdateTime,
		                                  TuhuStockID,
		                                  LogisticStockID,
		                                  ExpCompany,
		                                  LogisticCo,
		                                  ArriveShopExpCo,
                                          PinYin
		                                )
                                VALUES	( N'{0}',
		                                  N'',
		                                  N'',
		                                  0,
		                                  {1},
		                                  {2},
		                                  0,
		                                  0,
		                                  0,
		                                  N'',
		                                  {3},
		                                  1,
		                                  GETDATE(),
		                                  0,
		                                  0,
		                                  N'',
		                                  N'',
		                                  N'',
                                          N'{4}'
		)", model.RegionName, model.ProvinceId, model.CityId, model.ParentId, model.PinYin);
            DbHelper.ExecuteNonQuery(sql, CommandType.Text);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="regionId"></param>
        public static void UpdateRegion(string regionName, int regionId,string pinYin)
        {
            string sql = string.Format(@"
UPDATE dbo.tbl_Region SET RegionName = N'{0}',PinYin='{1}' WHERE PKID = {2}", regionName, pinYin,regionId);
            DbHelper.ExecuteNonQuery(sql, CommandType.Text);
        }
        /// <summary>
        /// 根据城市ID获取对应省的信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cityId">城市ID</param>
        /// <returns></returns>
        public static Region GetProvinceMessageByCityId(SqlConnection conn, int cityId)
        {
            const string commandText = @"SELECT	R2.PKID,
		                                        R2.RegionName
                                        FROM	dbo.tbl_Region AS R WITH (NOLOCK)
                                        INNER JOIN dbo.tbl_Region AS R2 WITH (NOLOCK)
		                                        ON R.ProvinceID = R2.PKID
                                        WHERE	R.PKID = @CityId";
            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, new SqlParameter("@CityId", cityId)))
            {
                if (reader.Read())
                {
                    var region = new Region { PKID = reader.GetTuhuValue<int>(0), RegionName = reader.GetTuhuString(1) };
                    return region;
                }
                return null;
            }
        }

        public static List<BizRegion> GetCityList(int? provinceId)
        {
            var sql = "SELECT PKID, RegionName FROM dbo.tbl_Region WITH(NOLOCK) WHERE ProvinceID = " + provinceId + " AND IsActive=1 ";
            return DbHelper.ExecuteDataTable(sql).ConvertTo<BizRegion>().ToList();
        }

        public static List<Region> SelectRegionsByParentID(SqlConnection conn, int parentId)
        {
            var Regions = new List<Region>();
            var parm = new[]
            {
                new SqlParameter("@ParentID",parentId)
            };
            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "Kuaidi100_Selecttbl_RegionsByParentID", parm))
            {
                while (reader.Read())
                {
                    var region = new Region();
                    region.PKID = reader.GetTuhuValue<int>(0);
                    region.RegionName = reader.GetTuhuString(1);
                    region.ParentID = reader.GetTuhuValue<int>(2);
                    Regions.Add(region);
                }
            }
            return Regions;
        }

        /// <summary>
        /// 获得所有省
        /// </summary>
        public static List<Region> SelectAllProvince(SqlConnection connection, int parentId)
        {
            var regions = new List<Region>();
            string sql = @"SELECT	R.PKID,
		                            R.RegionName
                            FROM	dbo.tbl_region AS R WITH (NOLOCK)
                            WHERE	R.ParentID = @ParentID
		                            AND IsActive = 1";
            var sqlParameter = new[]
            {
                new SqlParameter("@ParentID",parentId)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameter))
            {
                while (reader.Read())
                {
                    var region = new Region();
                    region.PKID = reader.GetTuhuValue<int>(0);
                    region.RegionName = reader.GetTuhuString(1);

                    regions.Add(region);
                }
            }
            return regions;
        }

        public static Region SelectRegionByRegionName(SqlConnection connection, string regionName, int parentId = 0)
        {
            string sql = @"SELECT	PKID,RegionName
                           FROM	dbo.tbl_region AS R with(nolock)
                           WHERE	R.RegionName = @RegionName
                           AND R.ParentID = @ParentID";

            var sqlParameters = new[]
            {
                new SqlParameter("@RegionName",regionName),
                new SqlParameter("@ParentID",parentId)
            };


            var region = new Region();
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameters))
            {
                if (reader.Read())
                {
                    region.PKID = reader.GetTuhuValue<int>(0);
                    region.RegionName = reader.GetTuhuString(1);
                }
            }
            return region;
        }

        /// <summary>
        /// 获取所有省市仓库配置
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static List<Region> SelectAllProvinceList(SqlConnection connection, int? parentId, int type, int PKID)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            var regions = new List<Region>();
            string sql = @"SELECT  PKID ,
                                    RegionName , 
                                    Zipcode ,
                                    Phone ,
                                    Disabled ,
                                    ProvinceID ,
                                    CityID ,
                                    DistrictID ,
                                    IsBusiness ,
                                    IsInstall ,
                                    Tag ,
                                    ParentID ,
                                    IsActive ,
                                    LastUpdateTime ,
                                    TuhuStockID ,
                                    LogisticStockID ,
                                    ExpCompany ,
                                    LogisticCo ,
                                    ArriveShopExpCo ,
                                    BYTuhuStockID ,
                                    BYLogisticStockID, 
                                    BYLogisticCo, 
                                    BYExpCompany, 
                                    BYArriveShopExpCo
                            FROM    dbo.tbl_region R WITH (NOLOCK)
		                 WHERE IsActive=1";
            if (parentId != null)
            {
                sql += " and ParentID = @ParentID";
                sqlParameters.Add(new SqlParameter("@ParentID", SqlDbType.Int) { Value = (int)parentId });
            }
            if (type == 1)
            {
                sql += " and PKID = @PKID ";
                sqlParameters.Add(new SqlParameter("@PKID", SqlDbType.Int) { Value = PKID });
            }
            sql += " ORDER BY PKID ";

            return SqlHelper.ExecuteDataTable2(connection, CommandType.Text, sql, sqlParameters.ToArray()).ConvertTo<Region>().ToList();
        }

        public static Region SelectRegionByPKID(SqlConnection connection, int PKID)
        {
            var sqlParameters = new List<SqlParameter>();
            Region region = null;
            string sql = @"SELECT  PKID ,
                                    RegionName , 
                                    Zipcode ,
                                    Phone ,
                                    Disabled ,
                                    ProvinceID ,
                                    CityID ,
                                    DistrictID ,
                                    IsBusiness ,
                                    IsInstall ,
                                    Tag ,
                                    ParentID ,
                                    IsActive ,
                                    LastUpdateTime ,
                                    TuhuStockID ,
                                    LogisticStockID ,
                                    ExpCompany ,
                                    LogisticCo ,
                                    ArriveShopExpCo ,
                                    BYTuhuStockID ,
                                    BYLogisticStockID, 
                                    BYLogisticCo, 
                                    BYExpCompany, 
                                    BYArriveShopExpCo
                            FROM    dbo.tbl_region R WITH (NOLOCK)
		                 WHERE IsActive=1";
            sql += " and PKID = @PKID ";
            sqlParameters.Add(new SqlParameter("@PKID", PKID));

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameters.ToArray()))
            {
                if (reader.Read())
                {
                    region = new Region();
                    region.PKID = reader.GetTuhuValue<int>(0);
                    region.RegionName = reader.GetTuhuString(1);
                    region.Zipcode = reader.GetTuhuString(2);
                    region.Phone = reader.GetTuhuString(3);
                    region.Disabled = reader.GetTuhuValue<bool>(4);
                    region.ProvinceID = reader.GetTuhuValue<int>(5);
                    region.CityID = reader.GetTuhuValue<int>(6);
                    region.DistrictID = reader.GetTuhuValue<int>(7);
                    region.IsBusiness = reader.GetTuhuValue<bool>(8);
                    region.IsInstall = reader.GetTuhuValue<int>(9);
                    region.Tag = reader.GetTuhuString(10);
                    region.ParentID = reader.GetTuhuValue<int>(11);
                    region.IsActive = reader.GetTuhuValue<bool>(12);
                    region.LastUpdateTime = reader.GetTuhuValue<DateTime>(13);
                    region.TuhuStockID = reader.GetTuhuValue<int>(14);
                    region.LogisticStockID = reader.GetTuhuValue<int>(15);
                    region.ExpCompany = reader.GetTuhuString(16);
                    region.LogisticCo = reader.GetTuhuString(17);
                    region.ArriveShopExpCo = reader.GetTuhuString(18);
                    region.BYTuhuStockID = reader.GetTuhuValue<int>(19);
                    region.BYLogisticStockID = reader.GetTuhuValue<int>(20);
                    region.BYLogisticCo = reader.GetTuhuString(21);
                    region.BYExpCompany = reader.GetTuhuString(22);
                    region.BYArriveShopExpCo = reader.GetTuhuString(23);
                }
            }
            return region;
        }

        ///// <summary>
        ///// 根据地区，配送方式，获得保养仓库
        ///// </summary>
        ///// <param name="connection"></param>
        ///// <param name="regionId"></param>
        ///// <param name="deliveryType"></param>
        ///// <returns></returns>
        //public static Dictionary<int, string> SelectBaoYangWareHouseId(SqlConnection connection, int regionId, string deliveryType, int installShopId)
        //{
        //    int wareHouseId = -1;//仓库Id
        //    string deliveryCompany = string.Empty;//快递公司
        //    Dictionary<int, string> dic = new Dictionary<int, string>();
        //    string sql = @"SELECT   R.BYTuhuStockID,
		      //                      R.BYLogisticStockID,
		      //                      R.BYExpCompany,
		      //                      BYExpCompany.DicKey,
		      //                      R.BYLogisticCo,
		      //                      BYLogisticCo.DicKey,
		      //                      R.BYArriveShopExpCo,
		      //                      BYArriveShopExpCo.DicKey
        //                    FROM	dbo.tbl_region AS R WITH (NOLOCK)
        //                    LEFT JOIN dbo.tbl_Dictionaries AS BYExpCompany WITH (NOLOCK)
		      //                      ON R.BYExpCompany = BYExpCompany.DicValue
		      //                          AND BYExpCompany.DicType = 'ExpressCo'
        //                    LEFT JOIN dbo.tbl_Dictionaries AS BYLogisticCo WITH (NOLOCK)
		      //                      ON R.BYLogisticCo = BYLogisticCo.DicValue
		      //                          AND BYLogisticCo.DicType = 'LogisticCo'
        //                    LEFT JOIN dbo.tbl_Dictionaries AS BYArriveShopExpCo WITH (NOLOCK)
		      //                      ON R.BYArriveShopExpCo = BYArriveShopExpCo.DicValue
		      //                          AND BYArriveShopExpCo.DicType = 'ExpressCo'
        //                    WHERE	R.PKID = @RegionId
        //                    ORDER BY R.PKID";

        //    var sqlParameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@RegionId",regionId)
        //    };


        //    using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameters))
        //    {
        //        if (reader.Read())
        //        {
        //            wareHouseId = reader.GetTuhuValue<int>(1);
        //            //if (deliveryType == DeliveryType.Express || deliveryType == DeliveryType.Logistic)//如果是快递，或者物流
        //            //{
        //            //    wareHouseId = reader.GetTuhuValue<int>(1);
        //            //}
        //            //else//途虎配送
        //            //{
        //            //    wareHouseId = reader.GetTuhuValue<int>(0);
        //            //}
        //            if (deliveryType == DeliveryType.Express && installShopId == 0)//快递到家
        //            {
        //                deliveryCompany = reader.GetTuhuString(3);
        //            }
        //            if (deliveryType == DeliveryType.Logistic)//物流
        //            {
        //                deliveryCompany = reader.GetTuhuString(5);
        //            }
        //            if (deliveryType == DeliveryType.Express && installShopId > 0)//快递到门店
        //            {
        //                deliveryCompany = reader.GetTuhuString(7);
        //            }
        //        }
        //    }

        //    if (string.IsNullOrEmpty(deliveryCompany))//没有找到，那么查看上层省，是否配置
        //    {
        //        string sqlParent = @"SELECT	Parent.BYTuhuStockID,
		      //                              Parent.BYLogisticStockID,
		      //                              Parent.BYExpCompany,
		      //                              BYExpCompany.DicKey,
		      //                              Parent.BYLogisticCo,
		      //                              BYLogisticCo.DicKey,
		      //                              Parent.BYArriveShopExpCo,
		      //                              BYArriveShopExpCo.DicKey,
		      //                              Child.RegionName,
		      //                              Parent.RegionName
        //                            FROM	dbo.tbl_region AS Parent WITH (NOLOCK)
        //                            LEFT JOIN dbo.tbl_Dictionaries AS BYExpCompany WITH (NOLOCK)
		      //                              ON Parent.BYExpCompany = BYExpCompany.DicValue
		      //                                 AND BYExpCompany.DicType = 'ExpressCo'
        //                            LEFT JOIN dbo.tbl_Dictionaries AS BYLogisticCo WITH (NOLOCK)
		      //                              ON Parent.BYExpCompany = BYLogisticCo.DicValue
		      //                                 AND BYLogisticCo.DicType = 'LogisticCo'
        //                            LEFT JOIN dbo.tbl_Dictionaries AS BYArriveShopExpCo WITH (NOLOCK)
		      //                              ON Parent.BYArriveShopExpCo = BYArriveShopExpCo.DicValue
		      //                                 AND BYArriveShopExpCo.DicType = 'ExpressCo'
        //                            LEFT JOIN dbo.tbl_region AS Child WITH (NOLOCK)
		      //                              ON Parent.PKID = Child.ParentID
        //                            WHERE	Child.PKID = @RegionId
        //                            ORDER BY Parent.PKID";

        //        using (var reader1 = SqlHelper.ExecuteReader(connection, CommandType.Text, sqlParent, sqlParameters))
        //        {
        //            if (reader1.Read())
        //            {
        //                if (deliveryType == DeliveryType.Express && installShopId == 0)//快递到家
        //                {
        //                    deliveryCompany = reader1.GetTuhuString(3);
        //                }
        //                if (deliveryType == DeliveryType.Logistic)//物流
        //                {
        //                    deliveryCompany = reader1.GetTuhuString(5);
        //                }
        //                if (deliveryType == DeliveryType.Express && installShopId > 0)//快递到门店
        //                {
        //                    deliveryCompany = reader1.GetTuhuString(7);
        //                }
        //            }
        //        }
        //    }
        //    dic.Add(wareHouseId, deliveryCompany);
        //    return dic;
        //}

        //public static Dictionary<int, string> SelectWareHouseId(SqlConnection connection, int regionId, string deliveryType, int installShopId)
        //{
        //    int wareHouseId = -1;//仓库Id
        //    string deliveryCompany = string.Empty;//快递公司
        //    Dictionary<int, string> dic = new Dictionary<int, string>();
        //    string sql = @"SELECT   R.TuhuStockID,
        //                            R.LogisticStockID,

        //                            R.ExpCompany,
        //                            ExpCompany.DicKey,

        //                            R.LogisticCo,
        //                            LogisticCo.DicKey,

        //                            R.ArriveShopExpCo,
        //                            ArriveShopExpCo.DicKey
        //                    FROM    dbo.tbl_region AS R WITH (NOLOCK)
        //                    LEFT JOIN dbo.tbl_Dictionaries AS ExpCompany WITH (NOLOCK)
        //                    ON R.ExpCompany = ExpCompany.DicValue
        //                    AND ExpCompany.DicType = 'ExpressCo'
        //                    LEFT JOIN dbo.tbl_Dictionaries AS LogisticCo WITH (NOLOCK)
        //                    ON R.LogisticCo = LogisticCo.DicValue
        //                    AND LogisticCo.DicType = 'LogisticCo'
        //                    LEFT JOIN dbo.tbl_Dictionaries AS ArriveShopExpCo WITH (NOLOCK)
        //                    ON R.ArriveShopExpCo = ArriveShopExpCo.DicValue
        //                    AND ArriveShopExpCo.DicType = 'ExpressCo'
        //                    WHERE   R.PKID = @RegionId
        //                    ORDER BY R.PKID";

        //    var sqlParameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@RegionId",regionId)
        //    };

        //    using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, sqlParameters))
        //    {
        //        if (reader.Read())
        //        {
        //            if (deliveryType == DeliveryType.Express || deliveryType == DeliveryType.Logistic)//如果是快递，或者物流
        //            {
        //                wareHouseId = reader.GetTuhuValue<int>(1);
        //            }
        //            else//途虎配送
        //            {
        //                wareHouseId = reader.GetTuhuNullableValue<int>(0) ?? reader.GetTuhuValue<int>(1);
        //            }
        //            if (deliveryType == DeliveryType.Express && installShopId == 0)//快递到家
        //            {
        //                deliveryCompany = reader.GetTuhuString(3);
        //            }
        //            if (deliveryType == DeliveryType.Logistic)//物流
        //            {
        //                deliveryCompany = reader.GetTuhuString(5);
        //            }
        //            if (deliveryType == DeliveryType.Express && installShopId > 0)//快递到门店
        //            {
        //                deliveryCompany = reader.GetTuhuString(7);
        //            }
        //        }
        //    }

        //    if (!StringUtils.IngoreCaseCompare(deliveryType, DeliveryType.ShopInstall) && string.IsNullOrEmpty(deliveryCompany) && regionId > 0)//没有找到，那么查看上层省，是否配置
        //    {
        //        //var region = GetRegion(connection, regionId);
        //        Region region = new Region();
        //        try
        //        {
        //            region = DalRegion.GetRegion(regionId);
        //            if (region == null || region.PKID == 0)
        //                throw new Exception("系统错误");
        //        }
        //        catch (Exception ex)
        //        {
        //            region = GetRegion(connection, regionId);
        //        }
        //        if (region != null && region.ParentID.HasValue)
        //        {
        //            var dicParentDeliveryCompany = SelectWareHouseId(connection, region.ParentID.Value, deliveryType, installShopId);
        //            if (dicParentDeliveryCompany.Values.Count() > 0)
        //            {
        //                deliveryCompany = dicParentDeliveryCompany.Values.First();
        //            }
        //        }
        //    }

        //    dic.Add(wareHouseId, deliveryCompany);
        //    return dic;
        //}

        //public static List<Region> GetRegionForTest(SqlConnection connection, List<int> pkidList)
        //{
        //    List<Region> regionList = new List<Region>();

        //    string pkId = string.Join(",", pkidList);
        //    var parameters = new[]
        //    {
        //        new SqlParameter("@PKID", pkId),
        //        new SqlParameter("@split",",")
        //    };

        //    string sql = @"SELECT	[PKID],
		      //                      [RegionName],
		      //                      [Zipcode],
		      //                      [Phone],
		      //                      [Disabled],
		      //                      [ProvinceID],
		      //                      [CityID],
		      //                      [DistrictID],
		      //                      [IsBusiness],
		      //                      [IsInstall],
		      //                      [Tag],
		      //                      [ParentID],
		      //                      [IsActive],
		      //                      [LastUpdateTime],
		      //                      [TuhuStockID],
		      //                      [LogisticStockID],
		      //                      [ExpCompany],
		      //                      [LogisticCo],
		      //                      [ArriveShopExpCo]
        //                    FROM	[dbo].[tbl_region] AS R WITH (NOLOCK)
        //                    INNER JOIN dbo.Split(@PKID, @split) AS S
		      //                      ON S.col = R.PKID";

        //    using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, parameters))
        //    {
        //        while (dataReader.Read())
        //        {
        //            Region region = new Region();

        //            region.PKID = dataReader.GetTuhuValue<int>(0);
        //            region.RegionName = dataReader.GetTuhuString(1);
        //            region.Zipcode = dataReader.GetTuhuString(2);
        //            region.Phone = dataReader.GetTuhuString(3);
        //            region.Disabled = dataReader.GetTuhuValue<bool>(4);
        //            region.ProvinceID = dataReader.GetTuhuValue<int>(5);
        //            region.CityID = dataReader.GetTuhuValue<int>(6);
        //            region.DistrictID = dataReader.GetTuhuValue<int>(7);
        //            region.IsBusiness = dataReader.GetTuhuNullableValue<bool>(8);
        //            region.IsInstall = dataReader.GetTuhuNullableValue<int>(9);
        //            region.Tag = dataReader.GetTuhuString(10);
        //            region.ParentID = dataReader.GetTuhuNullableValue<int>(11);
        //            region.IsActive = dataReader.GetTuhuValue<bool>(12);
        //            region.LastUpdateTime = dataReader.GetTuhuValue<System.DateTime>(13);
        //            region.TuhuStockID = dataReader.GetTuhuNullableValue<int>(14);
        //            region.LogisticStockID = dataReader.GetTuhuNullableValue<int>(15);
        //            region.ExpCompany = dataReader.GetTuhuString(16);
        //            region.LogisticCo = dataReader.GetTuhuString(17);
        //            region.ArriveShopExpCo = dataReader.GetTuhuString(18);

        //            regionList.Add(region);
        //        }
        //    }

        //    return regionList;
        //}

        /// <summary>
        /// 获得region表里面所有的数据
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Region> SelectAllRegions(SqlConnection connection)
        {
            List<Region> regions = new List<Region>();

            var sql = @"SELECT	R.PKID,
		                        R.RegionName,
		                        R.ParentID
                        FROM	dbo.tbl_region AS R WITH (NOLOCK)
                        WHERE	R.IsActive = 1
                        ORDER BY R.PKID";

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql))
            {
                while (dataReader.Read())
                {
                    Region region = new Region();

                    region.PKID = dataReader.GetTuhuValue<int>(0);
                    region.RegionName = dataReader.GetTuhuString(1);
                    region.ParentID = dataReader.GetTuhuValue<int>(2);
                    region.IsActive = true;
                    regions.Add(region);
                }
            }
            return regions;
        }

        
    }
}
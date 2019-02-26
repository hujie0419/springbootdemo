using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
//    public class DalPromotionConfigures
//    {

//        /// <summary>
//        /// 添加赠品
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <param name="promotionConfigures"></param>
//        /// <returns></returns>
//        public static int AddPromotionConfigure(SqlConnection connection, PromotionConfigures promotionConfigures)
//        {
//            var sqlParamters = new[]
//            {
//                new SqlParameter("@PromotionID",promotionConfigures.PromotionID??string.Empty),
//                new SqlParameter("@PromotionName",promotionConfigures.PromotionName??string.Empty),
//                new SqlParameter("@GiftProductID",promotionConfigures.GiftProductID??string.Empty),
//                new SqlParameter("@GiftName",promotionConfigures.GiftName??string.Empty),
//                new SqlParameter("@GiftNum",promotionConfigures.GiftNum),
//                new SqlParameter("@PromotionNum",promotionConfigures.PromotionNum),
//                new SqlParameter("@IsCertain",promotionConfigures.IsCertain),
//                new SqlParameter("@CreateBy",promotionConfigures.CreateBy??string.Empty),
//                new SqlParameter("@LastUpdateBy",promotionConfigures.LastUpdateBy??string.Empty),
//                new SqlParameter("@CreateDate",promotionConfigures.CreateDate==null?DateTime.Now:promotionConfigures.CreateDate),
//                new SqlParameter("@LastUpdateDate",promotionConfigures.LastUpdateDate==null?DateTime.Now:promotionConfigures.LastUpdateDate),
//                new SqlParameter("@IsActive",promotionConfigures.IsActive==null?false:promotionConfigures.IsActive),
//                new SqlParameter("@MarketPrice",promotionConfigures.MarketPrice),
//                new SqlParameter("@OrderChannel",promotionConfigures.OrderChannel??string.Empty),
//                new SqlParameter("@CateOrSingle",promotionConfigures.CateOrSingle),
//                new SqlParameter("@GiftMethod",promotionConfigures.GiftMethod),
//                new SqlParameter("@GiftDescription",promotionConfigures.GiftDescription??string.Empty),
//                new SqlParameter("@InstallType",promotionConfigures.InstallType??string.Empty),
//                new SqlParameter("@GiftsType",promotionConfigures.GiftsType)

//            };
//            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text,
//                @"insert into PromotionConfigure(PromotionID,PromotionName,GiftProductID,GiftName,GiftNum,PromotionNum,IsCertain,CreateBy,LastUpdateBy,CreateDate,LastUpdateDate,IsActive,MarketPrice,OrderChannel,CateOrSingle,GiftMethod,GiftDescription,InstallType,GiftsType) values
//                 (@PromotionID,@PromotionName,@GiftProductID,@GiftName,@GiftNum,@PromotionNum,@IsCertain,@CreateBy,@LastUpdateBy,@CreateDate,@LastUpdateDate,@IsActive,@MarketPrice," + (string.IsNullOrEmpty(promotionConfigures.OrderChannel) ? "null," : "@OrderChannel,") + "@CateOrSingle,@GiftMethod,@GiftDescription,@InstallType,@GiftsType);SELECT @@IDENTITY"
//                , sqlParamters));
//        }
//        /// <summary>
//        /// 查询套餐信息
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <param name="PKID"></param>
//        /// <returns></returns>
//        public static PromotionConfigures SelectPromotionConfigureByPKID(SqlConnection connection, int PKID)
//        {
//            {
//                PromotionConfigures _PromotionConfigures = new PromotionConfigures();
//                var parameters = new[]
//                {
//                new SqlParameter("@PKID", PKID)
//            };
//                using (var _DR = SqlHelper.ExecuteReader(
//                    connection,
//                    CommandType.Text,
//                    @"SELECT TOP 1 
//                                PKID,
//		                        PromotionID,
//		                        PromotionName,
//		                        GiftProductID,
//		                        GiftName,
//		                        GiftNum,
//		                        PromotionNum,
//		                        IsCertain,
//		                        CreateBy,
//		                        LastUpdateBy,
//		                        CreateDate,
//		                        LastUpdateDate,
//		                        IsActive,
//		                        MarketPrice,
//		                        OrderChannel,
//		                        CateOrSingle,
//		                        GiftMethod,
//		                        GiftDescription,
//                                InstallType,
//                                GiftsType
//                        FROM PromotionConfigure WITH(NOLOCK)
//                        WHERE PKID=@PKID",
//                    parameters))
//                {
//                    if (_DR.Read())
//                    {
//                        _PromotionConfigures.PKID = _DR.GetTuhuValue<int>(0);
//                        _PromotionConfigures.PromotionID = _DR.GetTuhuString(1);
//                        _PromotionConfigures.PromotionName = _DR.GetTuhuString(2);
//                        _PromotionConfigures.GiftProductID = _DR.GetTuhuString(3);
//                        _PromotionConfigures.GiftName = _DR.GetTuhuString(4);
//                        _PromotionConfigures.GiftNum = _DR.GetTuhuValue<int>(5);
//                        _PromotionConfigures.PromotionNum = _DR.GetTuhuValue<int>(6);
//                        _PromotionConfigures.IsCertain = _DR.GetTuhuValue<bool>(7);
//                        _PromotionConfigures.CreateBy = _DR.GetTuhuString(8);
//                        _PromotionConfigures.LastUpdateBy = _DR.GetTuhuString(9);
//                        _PromotionConfigures.CreateDate = _DR.GetTuhuValue<DateTime>(10);
//                        _PromotionConfigures.LastUpdateDate = _DR.GetTuhuValue<DateTime>(11);
//                        _PromotionConfigures.IsActive = _DR.GetTuhuValue<bool>(12);
//                        _PromotionConfigures.MarketPrice = _DR.GetTuhuValue<decimal>(13);
//                        _PromotionConfigures.OrderChannel = _DR.GetTuhuString(14);
//                        _PromotionConfigures.CateOrSingle = _DR.GetTuhuValue<int>(15);
//                        _PromotionConfigures.GiftMethod = _DR.GetTuhuValue<bool>(16);
//                        _PromotionConfigures.GiftDescription = _DR.GetTuhuString(17);
//                        _PromotionConfigures.InstallType = _DR.GetTuhuString(18);
//                        _PromotionConfigures.GiftsType = _DR.GetTuhuValue<int>(19);
//                    }
//                }
//                return _PromotionConfigures;
//            }
//        }
//        /// <summary>
//        /// 修改套餐活动渠道
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <param name="PKID"></param>
//        /// <returns></returns>
//        public static PromotionConfigures SelectPromotionChannelByPKID(SqlConnection connection, int PKID)
//        {
//            {
//                PromotionConfigures _PromotionConfigures = new PromotionConfigures();
//                var parameters = new[]
//                {
//                new SqlParameter("@PKID", PKID)
//            };
//                using (var _DR = SqlHelper.ExecuteReader(
//                    connection,
//                    CommandType.Text,
//                    @"SELECT TOP 1 
//                                PKID,
//		                        OrderChannel
//                        FROM PromotionConfigure WITH(NOLOCK)
//                        WHERE PKID=@PKID",
//                    parameters))
//                {
//                    if (_DR.Read())
//                    {
//                        _PromotionConfigures.PKID = _DR.GetTuhuValue<int>(0);
//                        _PromotionConfigures.OrderChannel = _DR.GetTuhuString(1);
//                    }
//                }
//                return _PromotionConfigures;
//            }
//        }

//        /// <summary>
//        /// 修改套餐IsActive
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <param name="promotionConfigures"></param>
//        /// <returns></returns>
//        public static int UpdatePromotionConfigureIsActive(SqlConnection connection, int PKID)
//        {
//            var sql = @"UPDATE  PromotionConfigure WITH(ROWLOCK)
//                        SET     IsActive = 0
//                        WHERE   PKID = @PKID";

//            var sqlParameters = new[]
//            {
//                new SqlParameter("@PKID", PKID)
//            };

//            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParameters);
//        }

//        public static void UpdatePromotionConfigure(SqlConnection connection, PromotionConfigures promotionConfigures)
//        {
//            var sqlParamters = new[]
//            {
//                new SqlParameter("@PKID",promotionConfigures.PKID==null?0:promotionConfigures.PKID),
//                new SqlParameter("@PromotionID",promotionConfigures.PromotionID==null?"":promotionConfigures.PromotionID),
//                new SqlParameter("@PromotionName",promotionConfigures.PromotionName==null?"":promotionConfigures.PromotionName),
//                new SqlParameter("@GiftProductID",promotionConfigures.GiftProductID==null?"":promotionConfigures.GiftProductID),
//                new SqlParameter("@GiftName",promotionConfigures.GiftName==null?"":promotionConfigures.GiftName),
//                new SqlParameter("@GiftNum",promotionConfigures.GiftNum==null?0:promotionConfigures.GiftNum),
//                new SqlParameter("@PromotionNum",promotionConfigures.PromotionNum==null?0:promotionConfigures.PromotionNum),
//                new SqlParameter("@IsCertain",promotionConfigures.IsCertain==null?false:promotionConfigures.IsCertain),
//                new SqlParameter("@CreateBy",promotionConfigures.CreateBy==null?"":promotionConfigures.CreateBy),
//                new SqlParameter("@LastUpdateBy",promotionConfigures.LastUpdateBy==null?"":promotionConfigures.LastUpdateBy),
//                new SqlParameter("@CreateDate",promotionConfigures.CreateDate==null?DateTime.Now:promotionConfigures.CreateDate),
//                new SqlParameter("@LastUpdateDate",promotionConfigures.LastUpdateDate==null?DateTime.Now:promotionConfigures.LastUpdateDate),

//                new SqlParameter("@MarketPrice",promotionConfigures.MarketPrice==null?0:promotionConfigures.MarketPrice),
//                new SqlParameter("@OrderChannel",promotionConfigures.OrderChannel==null?"":promotionConfigures.OrderChannel),
//                new SqlParameter("@CateOrSingle",promotionConfigures.CateOrSingle==null?1:promotionConfigures.CateOrSingle),
//                new SqlParameter("@GiftMethod",promotionConfigures.GiftMethod==null?true:promotionConfigures.GiftMethod),
//                new SqlParameter("@GiftDescription",promotionConfigures.GiftDescription==null?"":promotionConfigures.GiftDescription),
//                new SqlParameter("@InstallType",promotionConfigures.InstallType??string.Empty)
//            };
//            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
//                @"update PromotionConfigure set
//PromotionID=@PromotionID,
//PromotionName=@PromotionName,
//GiftProductID=@GiftProductID,
//GiftName=@GiftName,
//GiftNum=@GiftNum,
//PromotionNum=@PromotionNum,
//IsCertain=@IsCertain,
//CreateBy=@CreateBy,
//LastUpdateBy=@LastUpdateBy,
//CreateDate=@CreateDate,
//LastUpdateDate=@LastUpdateDate,

//MarketPrice=@MarketPrice,
//OrderChannel=
//" + (string.IsNullOrEmpty(promotionConfigures.OrderChannel) ? "null" : "@OrderChannel") + @"
//,
//CateOrSingle=@CateOrSingle,
//GiftMethod=@GiftMethod,
//GiftDescription=@GiftDescription,
//InstallType=@InstallType
//where PKID=@PKID", sqlParamters);
//        }


//        public static void UpdatePromotionChannel(SqlConnection connection, PromotionConfigures promotionConfigures)
//        {
//            var sqlParamters = new[]
//            {
//                new SqlParameter("@PKID",promotionConfigures.PKID==null?0:promotionConfigures.PKID),
//                new SqlParameter("@OrderChannel",promotionConfigures.OrderChannel==null?"":promotionConfigures.OrderChannel),
//                new SqlParameter("@LastUpdateBy",promotionConfigures.LastUpdateBy==null?"":promotionConfigures.LastUpdateBy),
//                new SqlParameter("@LastUpdateDate",promotionConfigures.LastUpdateDate==null?DateTime.Now:promotionConfigures.LastUpdateDate)

//                  };
//            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
//                @"update PromotionConfigure set
//                    LastUpdateBy=@LastUpdateBy,
//                    LastUpdateDate=@LastUpdateDate,
//                    OrderChannel=
//                    " + (string.IsNullOrEmpty(promotionConfigures.OrderChannel) ? "null" : "@OrderChannel") + @"
//                    where PKID=@PKID", sqlParamters);
//        }

//        /// <summary>
//        /// 必选，可选
//        /// </summary>
//        /// <param name="pkid"></param>
//        /// <param name="isCertain"></param>
//        /// <returns></returns>
//        public static int UpdatePromotionConfigureIsCertain(
//            SqlConnection connection,
//            int pkid,
//            bool isCertain)
//        {
//            return SqlHelper.ExecuteNonQuery(
//                connection,
//                CommandType.Text,
//                 @" UPDATE  dbo.PromotionConfigure WITH ( ROWLOCK )
//                    SET     IsCertain = @IsCertain
//                    WHERE   PKID = @PKID",

//                  new SqlParameter("@PKID", pkid),
//                  new SqlParameter("@IsCertain", isCertain)
//                  );
//        }


//        public static List<PromotionConfigures> GetAllPromotionConfigures(int pageIndex, int pageSize, string filter, out int count)
//        {
//            var parameters = new[]
//            {
//                new SqlParameter("@pageIndex", pageIndex),
//                new SqlParameter("@pageSize", pageSize),
//                new SqlParameter("@filter", filter),
//                new SqlParameter("@count", SqlDbType.Int, 20) { Direction = ParameterDirection.Output }
//            };
//            var list = DbHelper.ExecuteDataTable("proc_PromotionConfigure", CommandType.StoredProcedure, parameters).ConvertTo<PromotionConfigures>().ToList();
//            count = int.Parse(parameters.LastOrDefault().Value.ToString());
//            return list;
//        }
//        public static PromotionConfigures GetPromotionConfiguresByPKID(SqlConnection connection, int PKID)
//        {
//            PromotionConfigures _PromotionConfigures = new PromotionConfigures();
//            var parameters = new[]
//            {
//                new SqlParameter("@PKID", PKID)
//            };
//            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
//        PKID,
//		PromotionID,
//		PromotionName,
//		GiftProductID,
//		GiftName,
//		GiftNum,
//		PromotionNum,
//		IsCertain,
//		CreateBy,
//		LastUpdateBy,
//		CreateDate,
//		LastUpdateDate,
//		IsActive,
//		MarketPrice,
//		OrderChannel,
//		CateOrSingle,
//		GiftMethod,
//		GiftDescription,
//        InstallType
//FROM PromotionConfigure WHERE PKID=@PKID", parameters))
//            {
//                if (_DR.Read())
//                {
//                    _PromotionConfigures.PKID = _DR.GetTuhuValue<int>(0);
//                    _PromotionConfigures.PromotionID = _DR.GetTuhuString(1);
//                    _PromotionConfigures.PromotionName = _DR.GetTuhuString(2);
//                    _PromotionConfigures.GiftProductID = _DR.GetTuhuString(3);
//                    _PromotionConfigures.GiftName = _DR.GetTuhuString(4);
//                    _PromotionConfigures.GiftNum = _DR.GetTuhuValue<int>(5);
//                    _PromotionConfigures.PromotionNum = _DR.GetTuhuValue<int>(6);
//                    _PromotionConfigures.IsCertain = _DR.GetTuhuValue<bool>(7);
//                    _PromotionConfigures.CreateBy = _DR.GetTuhuString(8);
//                    _PromotionConfigures.LastUpdateBy = _DR.GetTuhuString(9);
//                    _PromotionConfigures.CreateDate = _DR.GetTuhuValue<DateTime>(10);
//                    _PromotionConfigures.LastUpdateDate = _DR.GetTuhuValue<DateTime>(11);
//                    _PromotionConfigures.IsActive = _DR.GetTuhuValue<bool>(12);
//                    _PromotionConfigures.MarketPrice = _DR.GetTuhuValue<decimal>(13);
//                    _PromotionConfigures.OrderChannel = _DR.GetTuhuString(14);
//                    _PromotionConfigures.CateOrSingle = _DR.GetTuhuValue<int>(15);
//                    _PromotionConfigures.GiftMethod = _DR.GetTuhuValue<bool>(16);
//                    _PromotionConfigures.GiftDescription = _DR.GetTuhuString(17);
//                    _PromotionConfigures.InstallType = _DR.GetTuhuString(18);
//                }
//            }
//            return _PromotionConfigures;
//        }

//       // /// <summary>
//       // /// 根据订单号获取符合规则的赠品（yangpeipei）
//       // /// </summary>
//       // /// <param name="connction"></param>
//       // /// <param name="orderId">订单号</param>
//       // /// <returns></returns>
//       // public static List<PromotionConfigures> SelectPromotionProductByOrderId(SqlConnection connection, int orderId)
//       // {

//       //     var sqlParameters = new[]
//       //     {
//       //         new SqlParameter("@OrderId", orderId)
//       //     };

//       //     string sql = @"SELECT   IsCertain ,
//							//		GiftProductID ,
//							//		GiftNum ,
//							//		CPZCC.DisplayName AS GiftName ,
//							//		0 ,
//							//		0 ,
//							//		0 ,
//							//		0 ,
//							//		0 ,
//							//		GiftsType
//							//FROM    ( SELECT    IsCertain ,
//							//					GiftProductID ,
//							//					SUM(Quantity) AS GiftNum ,
//							//					GiftsType
//							//		  FROM      vw_OrderGift
//							//		  WHERE     OrderID = @OrderId
//							//		  GROUP BY  GiftProductID ,
//							//					IsCertain,
//							//					GiftsType
//							//		) AS OrderGift
//							//		LEFT JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CPCP WITH ( NOLOCK ) ON OrderGift.GiftProductID COLLATE Chinese_PRC_CI_AS = CPCP.ProductID
//							//															  + '|'
//							//															  + CPCP.VariantID
//							//		LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS CPZCC WITH ( NOLOCK ) ON CPZCC.#Catalog_Lang_Oid = CPCP.oid;";

//       //     return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlParameters).ConvertTo<PromotionConfigures>().ToList();

//       // }

//        /// <summary>
//        /// 判断一个订单是否有赠品
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <param name="orderId"></param>
//        /// <returns></returns>
//        public static bool CheckOrderHasPromotionByOrderId(SqlConnection connection, int orderId)
//        {

//            var sqlParameters = new[]
//            {
//                new SqlParameter("@OrderId", orderId)
//            };

//            string sql = @"SELECT COUNT(1)
//                            FROM    vw_OrderGift WITH ( NOLOCK )
//                            WHERE   OrderID = @OrderId;";

//            var result = SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, sqlParameters);
//            if (result != null
//                && int.Parse(result.ToString()) > 0)
//                return true;
//            return false;
//        }


//        public static List<PromotionConfigures> SelectPromotionConfiguresesByPIDOrName(SqlConnection conn, string text)
//        {
//            var PromotionConfigures = new List<PromotionConfigures>();
//            var sqlp = new SqlParameter("@where", SqlDbType.NVarChar);
//            sqlp.Value = text;
//            var parm = new[] { sqlp };

//            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "Logistic_SelectPromotionConfigures", parm))
//            {
//                while (reader.Read())
//                {
//                    var p = new PromotionConfigures()
//                    {
//                        GiftProductID = reader.GetTuhuString(0),
//                        GiftName = reader.GetTuhuString(1)
//                    };
//                    PromotionConfigures.Add(p);
//                }
//            }
//            return PromotionConfigures;
//        }

//        /// <summary>
//        /// 查询所有套餐配置活动数据
//        /// </summary>
//        /// <param name="conn"></param>
//        /// <returns></returns>
//        public static List<PromotionConfigureModel> SelectPromotionConfigureAll(SqlConnection conn, string SqlWhere)
//        {

//            var sqlParms = new[]
//            {
//                new SqlParameter("@SqlWhere",SqlWhere)
//            };

//            var promotionConfigurelist = new List<PromotionConfigureModel>();
//            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "Proc_PromotionConfigure_activity", sqlParms))
//            {
//                while (reader.Read())
//                {
//                    PromotionConfigureModel promotionConfigure = new PromotionConfigureModel()
//                    {
//                        PKID = reader.GetTuhuValue<int>(0),
//                        PromotionID = reader.GetTuhuString(1),
//                        PromotionName = reader.GetTuhuString(2),
//                        GiftProductID = reader.GetTuhuString(3),
//                        GiftName = reader.GetTuhuString(4),
//                        GiftNum = reader.GetTuhuValue<int>(5),
//                        PromotionNum = reader.GetTuhuValue<int>(6),
//                        IsCertain = reader.GetTuhuValue<bool>(7),
//                        CreateBy = reader.GetTuhuString(8),
//                        LastUpdateBy = reader.GetTuhuString(9),
//                        CreateDate = reader.GetTuhuValue<DateTime>(10),
//                        LastUpdateDate = reader.GetTuhuValue<DateTime>(11),
//                        IsActive = reader.GetTuhuValue<bool>(12),
//                        MarketPrice = reader.GetTuhuValue<decimal>(13),
//                        OrderChannel = reader.GetTuhuString(14),
//                        CateOrSingle = reader.GetTuhuValue<int>(15),
//                        GiftMethod = reader.GetTuhuValue<bool>(16),
//                        GiftDescription = reader.GetTuhuString(17),
//                        InstallType = reader.GetTuhuString(18),
//                        Titile = reader.GetTuhuString(19)
//                    };
//                    promotionConfigurelist.Add(promotionConfigure);

//                }
//            }
//            return promotionConfigurelist;
//        }
        

//        public static List<PromotionConfigures> GetAllPromotionConfiguresByWhere(int pageIndex, int pageSize, string startDateTime, string endDateTime, string cateOrSingle, string selChannel, string proPid, string proName, int isActive, out int count)
//        {
//            List<SqlParameter> ls = new List<SqlParameter>();
//            string filter = "";
//            if (!string.IsNullOrWhiteSpace(startDateTime))
//            {
//                filter += " and CreateDate >= @CreateDate";
//                ls.Add(new SqlParameter("@CreateDate", startDateTime));
//            }
//            if (!string.IsNullOrWhiteSpace(endDateTime))
//            {
//                filter += " and CreateDate < @CreateEndTime";
//                ls.Add(new SqlParameter("@CreateEndTime", Convert.ToDateTime(endDateTime).AddDays(1).ToString("yyyy-MM-dd")));
//            }
//            if (!string.IsNullOrWhiteSpace(cateOrSingle))
//            {
//                filter += " and CateOrSingle = @CateOrSingle";
//                ls.Add(new SqlParameter("@CateOrSingle", cateOrSingle));
//            }
//            if (!string.IsNullOrWhiteSpace(proPid))
//            {
//                filter += " and GiftProductID like N'%'+@GiftProductID+'%'";
//                ls.Add(new SqlParameter("@GiftProductID", proPid));
//            }
//            if (!string.IsNullOrWhiteSpace(proName))
//            {
//                filter += " and GiftName like N'%'+@GiftName+'%'";
//                ls.Add(new SqlParameter("@GiftName", proName));
//            }
//            if (isActive >= 0)
//            {
//                filter += " and IsActive = @IsActive";
//                ls.Add(new SqlParameter("@IsActive", isActive));
//            }
//            if (!string.IsNullOrWhiteSpace(selChannel))
//            {
//                int index = 0;
//                foreach (var item in selChannel.Split(','))
//                {
//                    if (index == 0)
//                    {
//                        filter += " and (','+OrderChannel+',' like N'%," + item + ",%'";
//                    }
//                    else
//                    {
//                        filter += " or ','+OrderChannel+',' like N'%," + item + ",%'";
//                    }
//                    index++;
//                }
//                filter += " or OrderChannel is null or OrderChannel = N'')";
//            }
//            ls.Add(new SqlParameter("@pageIndex", pageIndex));
//            ls.Add(new SqlParameter("@pageSize", pageSize));
//            ls.Add(new SqlParameter("@count", SqlDbType.Int, 12) { Direction = ParameterDirection.Output });

//            string sSql = @"SELECT  @count = COUNT(1)
//                            FROM    PromotionConfigure AS o WITH ( NOLOCK )
//                            WHERE   1 = 1 " + filter + ';';
//            if (pageIndex == 1)
//                sSql += @"SELECT TOP(@pageSize) *  
//                            FROM    dbo.PromotionConfigure AS act WITH ( NOLOCK )
//                            WHERE   1 = 1 " + filter + @"
//                            ORDER BY IsActive DESC,act.PKID DESC";
//            else
//                sSql += @"SELECT TOP (@pageSize) *
//                            FROM    dbo.PromotionConfigure AS act WITH ( NOLOCK )
//                            WHERE   1 = 1  " + @filter + @"  
//                                    AND PKID NOT IN (
//                                    SELECT TOP ( ( @pageIndex - 1 ) * @pageSize )
//                                            PKID
//                                    FROM    dbo.PromotionConfigure AS act WITH ( NOLOCK )
//                                    WHERE   1 = 1 "
//                                            + @filter + @"
//                                    ORDER BY IsActive DESC ,
//                                            act.PKID DESC )
//                            ORDER BY IsActive DESC ,
//                                    act.PKID DESC";

//            var list = DbHelper.ExecuteDataTable(sSql, CommandType.Text, ls.ToArray()).ConvertTo<PromotionConfigures>().ToList();
//            count = int.Parse(ls.ToArray().LastOrDefault().Value.ToString());
//            return list;
//        }

//        /// <summary>
//        /// 查询所有有效的赠品
//        /// </summary>
//        /// <param name="connection"></param>
//        /// <returns></returns>
//        public static IEnumerable<PromotionConfigures> SelectAllPromotionConfigures(SqlConnection connection)
//        {
//            var sql = @"SELECT  *
//						FROM    Gungnir..PromotionConfigure AS PC WITH ( NOLOCK )
//						WHERE   PC.IsActive = 1;";

//            return SqlHelper.ExecuteDataTable2(connection, CommandType.Text, sql).ConvertTo<PromotionConfigures>();

//        }
//    }
}

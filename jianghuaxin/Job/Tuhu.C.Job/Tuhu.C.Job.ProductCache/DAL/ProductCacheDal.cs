using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product.Models.New;
using Tuhu.C.Job.ProductCache.Model;

namespace Tuhu.C.Job.ProductCache.DAL
{
    internal class ProductCacheDal
    {
        public static int SelectSkuProductCount()
        {
            using (var cmd = new SqlCommand(@"SELECT COUNT(1)
    FROM    Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK)
    WHERE   P.i_ClassType IN(2, 4) AND pid NOT LIKE N'TEMP-%'"))
            {
                cmd.CommandType = CommandType.Text;
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        public static List<string> SelectProductPids(int pageNum, int pageSize)
        {
            using (var cmd = new SqlCommand(@"SELECT  P.PID
FROM    Tuhu_productcatalog..vw_Products AS P WITH ( NOLOCK )
WHERE   P.i_ClassType IN ( 2, 4 )
ORDER BY P.PID OFFSET (@PageNumber - 1) * @PageSize ROW
				FETCH NEXT @PageSize ROW ONLY;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PageNumber", pageNum);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return DbHelper.ExecuteQuery(cmd,
                    dt => dt.AsEnumerable().Select(x => x[0].ToString()).ToList());
            }
        }

        public static IEnumerable<SkuProductDetailModel> SelectSkuProduct(int pageNum, int pageSize)
        {
            using (var cmd = new SqlCommand(@"SELECT  PS.OrderQuantity ,
            PS.SalesQuantity,
            PS.CommentTimes,
            PS.CommentR1,
            PS.CommentR2,
            PS.CommentR3,
            PS.CommentR4,
            PS.CommentR5,
            cc.NodeNo,
            p.oid,p.ProductID,p.VariantID,p.PID,p.Category,p.DefinitionName,p.i_ClassType,p.ProductCode,p.ProductSize,p.ProductColor,p.DisplayName,p.Description,p.invoice,p.cy_list_price,p.cy_bj_price,p.cy_marketing_price,p.CP_Brand,p.CP_Unit,p.CP_Place,p.ProductRefer,p.OnSale,p.stockout,PartNo,p.Image,p.Image_filename,p.Image_filename_2,p.Image_filename_3,p.Image_filename_4,p.Image_filename_5,p.Image_filename_Big,
			p.Variant_Image_filename_1,p.Variant_Image_filename_2,p.Variant_Image_filename_3,p.Variant_Image_filename_4,p.CP_Tire_Width,p.CP_Tire_AspectRatio,p.CP_Tire_Rim,p.CP_Tire_SpeedRating,p.CP_Tire_LoadIndex,p.CP_Tire_ROF,p.CP_Tire_Type,p.CP_Tire_Pattern,p.IsUsedInAdaptation,p.VehicleMatchLevel2,p.CP_Remark,p.CP_ShuXing1,p.CP_ShuXing2,p.CP_ShuXing3,p.CP_ShuXing4,p.CP_ShuXing5,p.Weight,p.Color,p.CP_Wiper_Size,
            p.CP_Wiper_Stand,p.CP_Wiper_Series,p.CP_Wiper_Baffler,p.CP_Brake_Position,p.CP_Brake_Type,p.CP_Brief_Auto,p.CP_Battery_Info,
			p.CP_Battery_Size,p.CP_Filter_Type,p.CP_Hub_H,p.CP_Hub_Stand,p.AdditionalProperties,p.CP_Hub_CB,p.CP_Hub_ET,p.CP_Hub_PCD,p.CP_Hub_Width,p.CP_Tab,p.IsShow,p.TireSize,p.IsDaiFa,p.CP_Tire_Snow,p.CreateDatetime,p.isOE,p.CP_ShuXing6
    FROM    Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK)
            LEFT JOIN Tuhu_productcatalog..tbl_ProductStatistics AS PS WITH(NOLOCK) ON PS.ProductID = P.ProductID
                                                              AND PS.VariantID = P.VariantID
            join Tuhu_productcatalog..[CarPAR_CatalogHierarchy](NOLOCK) as cc
                                              on  P.oid =cc.child_oid
    WHERE   P.i_ClassType IN(2, 4)
    ORDER BY p.PID
    OFFSET (@PageNumber - 1) * @PageSize ROW
				FETCH NEXT @PageSize ROW ONLY;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PageNumber", pageNum);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return DbHelper.ExecuteSelect<SkuProductDetailModel>(true, cmd);
            }

        }


        public static IEnumerable<IEnumerable<SuggestWord>> SelectSuggestWord(int pageSize)
        {
            var pageSql = @"
SELECT  Keyword ,
        Weight ,
        Source
FROM    Tuhu_productcatalog..tbl_SuggestWord WITH ( NOLOCK )
ORDER BY PKID OFFSET (@PageNumber - 1) * @PageSize ROW
				FETCH NEXT @PageSize ROW ONLY;";
            var cntSql = @"
SELECT  count(1) 
FROM    Tuhu_productcatalog..tbl_SuggestWord WITH ( NOLOCK )";

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                var cnt = 0.0;
                var result = DbHelper.ExecuteScalar(cmd) ?? "";
                double.TryParse(result.ToString(), out cnt);
                if (cnt == 0)
                    yield break;

                var pages = Math.Ceiling(cnt / pageSize);
                cmd.CommandText = pageSql;
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                var pageIndex = cmd.Parameters.AddWithValue("@PageNumber", 0);
                for (var i = 1; i <= pages; i++)
                {
                    pageIndex.Value = i;
                    yield return DbHelper.ExecuteSelect<SuggestWord>(cmd);
                }
            }
        }

        public static bool UpdateSuggestActive(List<string> keywords, bool isActive)
        {
            if (keywords == null && !keywords.Any())
                return true;

            using (var cmd = new SqlCommand(@"
UPDATE  Tuhu_productcatalog..tbl_SuggestWord WITH ( ROWLOCK )
SET     Tuhu_productcatalog..tbl_SuggestWord.isActive = @isActive
FROM    Tuhu_productcatalog..tbl_SuggestWord
WHERE   Keyword IN ( SELECT Item
                     FROM   Tuhu_productcatalog..SplitString(@Keys, ',', 1) )"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Keys", string.Join(",", keywords));
                cmd.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);
                var result = DbHelper.ExecuteNonQuery(cmd);
                return result == keywords.Count();
            }
        }
        public static IEnumerable<ProductNodeModel> SelectAllCategoryNodeNo()
        {
            var cntSql = @"
SELECT  DISTINCT NodeNo
FROM    Tuhu_productcatalog.dbo.CarPAR_CatalogHierarchy WITH ( NOLOCK )
WHERE   CategoryName IS NOT NULL;";

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                return DbHelper.ExecuteSelect<ProductNodeModel>(true, cmd);
            }
        }

        public static IEnumerable<ProductNodeModel> SelectAllProductNode()
        {
            var cntSql = @"
SELECT  CCH.NodeNo,
        SUM(1) ProductCount
FROM    Tuhu_productcatalog.[dbo].[CarPAR_CatalogHierarchy]
        AS CCH WITH ( NOLOCK )
        JOIN Tuhu_productcatalog.[dbo].[CarPAR_CatalogProducts]
        AS CCP WITH ( NOLOCK ) ON CCP.oid = CCH.child_oid
WHERE   CCP.i_ClassType IN ( 2, 4 )
        AND CCP.OnSale = 1
        AND CCP.stockout = 0
GROUP BY NodeNo";

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                return DbHelper.ExecuteSelect<ProductNodeModel>(true, cmd);
            }
        }
        public static bool UpdateProductNodeCNT(string nodeNo, int cnt)
        {
            if (string.IsNullOrEmpty(nodeNo))
                return true;

            using (var cmd = new SqlCommand(@"
IF EXISTS ( SELECT  *
            FROM    Tuhu_productcatalog..CarPAR_NodeProductNum WITH ( NOLOCK) WHERE NodeNo=@NodeNo)
    BEGIN
        UPDATE Tuhu_productcatalog..CarPAR_NodeProductNum WITH (ROWLOCK) SET Num=@Num WHERE  NodeNo=@NodeNo
    END
ELSE
    BEGIN
        INSERT INTO	Tuhu_productcatalog..CarPAR_NodeProductNum(Num,NodeNo)
		VALUES(@Num,@NodeNo)
    END "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@NodeNo", nodeNo);
                cmd.Parameters.AddWithValue("@Num", cnt);
                var result = DbHelper.ExecuteNonQuery(cmd);
                return result == 1;
            }
        }

        public static int GetProductEsCount()
        {
            var cntSql = @"
SELECT  COUNT(DISTINCT PID)
FROM    Tuhu_productcatalog.dbo.vw_ProductService_ProductRebuildEs WITH ( NOLOCK );";

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                return (int)(DbHelper.ExecuteScalar(true, cmd) ?? "0");
            }
        }

        public static List<string> GetProductEsPids(int pageSize, int pageNum)
        {
            var cntSql = @"
SELECT  PID
FROM    ( SELECT  DISTINCT
                    PID
          FROM      Tuhu_productcatalog.dbo.vw_ProductService_ProductRebuildEs
                    WITH ( NOLOCK )
        ) AS P
ORDER BY P.PID
        OFFSET ( @PageNumber - 1 ) * @PageSize ROW
				FETCH NEXT @PageSize ROW ONLY;";
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                cmd.Parameters.AddWithValue("@PageNumber", pageNum);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return (DbHelper.ExecuteQuery(cmd, dt =>
                {
                    var result = new List<string>();
                    if (dt != null && dt.Rows.Count > 0)
                        foreach (DataRow one in dt.Rows)
                            result.Add(one[0]?.ToString());
                    return result;
                }));
            }
        }

        public static IEnumerable<int> SelectCityIds()
        {
            string sql = @"SELECT DISTINCT CityID FROM  Gungnir..tbl_region WITH (NOLOCK);";
            using (var helper = DbHelper.CreateDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;

                    var resultdt = helper.ExecuteQuery(cmd, dt => dt);
                    var result = new List<int>() { -1, 1, 2, 19, 20 }; //全国+直辖市
                    if (resultdt != null && resultdt.Rows.Count > 0)
                    {
                        foreach (DataRow row in resultdt.AsEnumerable())
                        {
                            int temp;
                            if (int.TryParse(row[0]?.ToString(), out temp))
                            {
                                result.Add(temp);
                            }
                        }
                    }
                    return result.Distinct();
                }
            }
        }

        public static List<FlashSaleProductDetailModel> SelectAllSaleActivity()
        {
            using (var cmd = new SqlCommand(@"
SELECT  vvfs.ActivityID ,
        vvfs.PID ,
        vvfs.Price ,
        vvfs.ActiveType ,
        ISNULL(vvfs.TotalQuantity, 99999999) TotalQuantity ,
        SaleOutQuantity ,
        StartDateTime,
        IsUsePcode
FROM    Activity.dbo.vw_ValidFlashSale vvfs WITH ( NOLOCK )
WHERE   vvfs.ActiveType = 0
        AND vvfs.IsNewUserFirstOrder = 0
        AND vvfs.StartDateTime <= GETDATE()
        AND vvfs.EndDateTime >= GETDATE()
        AND ActivityID NOT IN (
                N'3464d2b9-cb9f-4f7d-bbb0-196bd8436197',
                N'a03e8ce5-728c-40cc-8598-2659c9ac015f',
                N'590c63bf-2348-4c72-9810-3495d0847dac',
                N'1a448a4a-a44d-4d27-8493-a6286a0de5ca',
                N'b92954b2-6cd4-4388-b735-c57f24749ada',
                N'69792836-cea3-49f3-8433-d00b0516c424',
                N'03c0fc0f-a30f-4930-9704-dea5e4f2e38e',
                N'5547058f-6a1d-465b-a58f-e5121aaf717a',
                N'138746f6-101c-4d72-a8ba-ead201a9d30a',
                N'13f19102-998c-4440-8f1e-f418527926f1',
                N'b93dd5bd-8d89-442a-8d64-16482e2de06a',
                N'95ce2cfb-d4c4-4851-b3a3-68ee61f5d596',
                N'eb5a3a03-4dbc-4079-981d-e025d25d45eb',
                N'2cb2c918-2616-4f79-a743-c41815e1cfcc',
                N'cdb8a5be-2756-427b-8ef7-2e8701f12a3e',
                N'ac9a66e8-6fdf-4a96-a157-c5e166297d52',
                N'8312fe9d-6dce-4cd5-87d5-99ce064336a3',
                N'650c6d05-51f8-46fb-a84f-f136d727a59b',
                N'663543d4-5b62-4133-935d-f0a4c68751f8',
                N'88c467db-622e-4f02-89a5-f80405076599'
        );"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteSelect<FlashSaleProductDetailModel>(cmd).ToList();
            }
        }

        public static List<int> SelectAllTids()
        {
            using (var cmd = new SqlCommand(@"
 SELECT DISTINCT TID FROM Gungnir..tbl_Vehicle_Type_Timing_Config WITH (NOLOCK) "))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    if (dt == null || dt.Rows.Count == 0)
                        return new List<int>();
                    return dt.AsEnumerable().Select(p =>
                    {
                        int.TryParse(p[0]?.ToString(), out int tid);
                        return tid;
                    }).Where(p => p != 0)?.ToList() ?? new List<int>();
                });
            }
        }

        public static IEnumerable<string> GetProductRecommendByUserId()
        {
            string sql = @"
                            SELECT  DISTINCT(RPID) AS Pid
                            FROM    Tuhu_bi..tbl_RecommendByUserId AS R WITH ( NOLOCK )";
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    return db.ExecuteQuery(cmd,dt=>dt.ToList<string>());
                }
            }
        }
        public static IEnumerable<string> GetCommonRecommend()
        {
            string sql = @"
                            SELECT  RPID AS Pid
                            FROM    Tuhu_bi..tbl_CommonRecommend AS R WITH ( NOLOCK )
                            ORDER BY Score DESC;";
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    return db.ExecuteQuery(cmd, dt => dt.ToList<string>());
                }
            }
        }

        public static string SelectRuntimeSwitchBySwitchName()
        {
            string sql = @"SELECT Description from Gungnir..RuntimeSwitch WITH(NOLOCK) WHERE SwitchName=N'RecommendRefresh'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return ( DbHelper.ExecuteScalar(true, cmd))?.ToString();
            }

        }

        public static IEnumerable<Dictionary<string, bool>> SelectCommonProductEsPids(int maxcount)
        {
            string countsql = @"SELECT  COUNT(1)
FROM    ( SELECT    PID ,
                    0 AS isshowproduct
          FROM      Tuhu_productcatalog..vw_Products WITH ( NOLOCK )
          UNION
          SELECT    PID ,
                    1 AS isshowproduct
          FROM      Tuhu_productcatalog..ShopProduct WITH ( NOLOCK )
        ) AS s;";
            int totalcount = 0;
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                var countresult = helper.ExecuteScalar(countsql);
                int.TryParse(countresult?.ToString(), out totalcount);
                if (totalcount > 0)
                {
                    string querysql = @"SELECT  *
FROM    ( SELECT    PID ,
                    0 AS isshowproduct
          FROM      Tuhu_productcatalog..vw_Products WITH ( NOLOCK )
          UNION
          SELECT    PID ,
                    1 AS isshowproduct
          FROM      Tuhu_productcatalog..ShopProduct WITH ( NOLOCK )
        ) AS s
ORDER BY s.PID
        OFFSET ( @PageNumber - 1 ) * @PageSize ROW
				FETCH NEXT @PageSize ROW ONLY;";

                    var pageCnt = (int)Math.Ceiling((double)totalcount / maxcount) + 1;
                    for (int i = 1; i <= pageCnt; i++)
                    {
                        using (var queryhelper = DbHelper.CreateDbHelper(true))
                        {
                            using (var cmd = new SqlCommand(querysql))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@PageNumber", i);
                                cmd.Parameters.AddWithValue("@PageSize", maxcount);
                                var result = queryhelper.ExecuteQuery(cmd, dt => dt);
                                if (result != null && result.Rows.Count > 0)
                                {
                                    Dictionary<string, bool> dict = new Dictionary<string, bool>();
                                    foreach (var row in result.AsEnumerable())
                                    {
                                        var pid = row["PID"]?.ToString();
                                        var isshowproduct = string.Equals("1", row["isshowproduct"]?.ToString());
                                        dict[pid] = isshowproduct;
                                    }
                                    yield return dict;
                                }
                            }

                        }
                    }
                }
            }
        }

        public static List<BiVehicleInfo> SelectBiTireSortSize()
        {
            string sql = @"
SELECT  DISTINCT CTR.VehicleId,CTR.TireSize
FROM	Tuhu_bi..tbl_CarTireRecommendation AS CTR WITH (NOLOCK)";
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    return db.ExecuteQuery(cmd, dt => dt.ConvertTo<BiVehicleInfo>())?.ToList() ?? new List<BiVehicleInfo>();
                }
            }
        }
        public static List<BiVehicleInfo> SelectManualTireSortSize()
        {
            string sql = @"
SELECT  DISTINCT VehicleId ,
        TireSize
FROM    [Tuhu_productcatalog].[dbo].[tbl_VehicleTireRecommend] WITH ( NOLOCK );";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    return db.ExecuteQuery(cmd, dt => dt.ConvertTo<BiVehicleInfo>())?.ToList() ?? new List<BiVehicleInfo>();
                }
            }
        }


    }
    public class BiVehicleInfo
    {
        public string VehicleId { get; set; }
        public string TireSize { get; set; }
    }
}

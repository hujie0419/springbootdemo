using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO.ShareBargain
{
    public class DALShareBargain
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("DALShareBargain");
        public static IEnumerable<BackgroundThemeModel> GetBackgroundTheme()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(ShareBargainSqlText.sql4getBackgroundTheme, CommandType.Text)
                    .ConvertTo<BackgroundThemeModel>();
            }
        }

        public static IEnumerable<ShareBargainItemModel> SelectBargainProductList(BargainProductRequest request, PagerModel pager)
        {
            string sql = @"SELECT  PKID ,
                                    PID ,
                                    productName AS ProductName ,
                                    CurrentStockCount ,
                                    FinalPrice ,
                                    BeginDateTime ,
                                    EndDateTime ,
                                    Operator ,
                                    ShowBeginTime ,
                                    ProductType
                            FROM    Configuration..BargainProduct WITH ( NOLOCK )
                            WHERE   IsDelete=0 
                                    AND ( @Operator IS NULL
                                      OR Operator = @Operator
                                    )
                                    AND ( @PID IS NULL
                                          OR PID LIKE '%' + @PID + '%'
                                        )
                                    AND ( @ProductName IS NULL
                                          OR ProductName LIKE '%' + @ProductName + '%'
                                        )
                                    AND ( @OnSale = 0
                                          OR ( @OnSale = 1
                                               AND BeginDateTime < GETDATE()
                                               AND EndDateTime > GETDATE()
                                             )
                                          OR ( @OnSale = 2
                                               AND EndDateTime < GETDATE()
                                             )
                                          OR ( @OnSale = 3
                                               AND BeginDateTime > GETDATE()
                                             )
                                        )
                            ORDER BY CreateDateTime DESC
                                    OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY; ";
            pager.TotalItem = SelectBargainProductCount(request);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@Operator", request.Operator),
                    new SqlParameter("@PID", request.PID),
                    new SqlParameter("@ProductName", request.ProductName),
                    new SqlParameter("@OnSale", request.OnSale),
                    new SqlParameter("@begin", (request.PageIndex - 1) * request.PageSize),
                    new SqlParameter("@step", request.PageSize),
                };
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, sqlPrams).ConvertTo<ShareBargainItemModel>();
            }
        }

        private static int SelectBargainProductCount(BargainProductRequest request)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@Operator", request.Operator),
                    new SqlParameter("@PID", request.PID),
                    new SqlParameter("@ProductName", request.ProductName),
                    new SqlParameter("@OnSale", request.OnSale),
                };
                return Convert.ToInt32(dbHelper.ExecuteScalar(ShareBargainSqlText.sql4selectBargainProductCount,
                    CommandType.Text,
                    sqlPrams));
            }
        }

        public static BargainGlobalConfigModel FetchBargainProductGlobalConfig()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var dat = dbHelper.ExecuteDataTable(ShareBargainSqlText.Sql4FetchBargainProductGlobalConfig,
                    CommandType.Text).ConvertTo<BargainGlobalConfigModel>().FirstOrDefault();
                if (dat != null)
                {
                    dat.BargainRule = JsonConvert.DeserializeObject<List<BargainRules>>(dat.QAData);
                    return dat;
                }
                return new BargainGlobalConfigModel();
            }
        }

        public static bool UpdateGlobalConfig(BargainGlobalConfigModel request)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@backgroundimage", request.BackgroundImage),
                    new SqlParameter("@backgroundtheme", request.BackgroundTheme),
                    new SqlParameter("@qacount", request.RulesCount),
                    new SqlParameter("@qadata", request.QAData),
                    new SqlParameter("@title", request.Title),
                    new SqlParameter("@wxapplistsharetext",request.WXAPPListShareText),
                    new SqlParameter("@wxapplistshareimg",request.WXAPPListShareImg),
                    new SqlParameter("@wxappdetailsharetext",request.WXAPPDetailShareText),
                    new SqlParameter("@applistsharetag",request.APPListShareTag),
                    new SqlParameter("@appdetailsharetag",request.AppDetailShareTag),
                    new SqlParameter("@sliceshowtext",request.SliceShowText)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4UpdateGlobalConfig, CommandType.Text,
                           sqlPrams) > 0;
            }
        }

        public static ShareBargainProductModel FetchBargainProductById(int apId)
        {
            string sql = @" SELECT PID ,
                            productName AS ProductName ,
                            CurrentStockCount ,
                            FinalPrice ,
                            BeginDateTime ,
                            EndDateTime ,
                            OriginalPrice ,
                            Sequence ,
                            Image1 ,
                            TotalStockCount ,
                            Times ,
                            PageName ,
                            SuccessfulHint ,
                            WXShareTitle ,
                            APPShareId ,
                            ShowBeginTime ,
                            SimpleDisplayName ,
                            HelpCutPriceTimes ,
                            CutPricePersonLimit ,
                            BigCutBeforeCount,
                            BigCutPriceRate,
                            ProductDetailImg1,
                            ProductDetailImg2,
                            ProductDetailImg3,
                            ProductDetailImg4,
                            ProductDetailImg5
                     FROM   Configuration..BargainProduct WITH ( NOLOCK )
                     WHERE  PKID = @pkid
                            AND IsDelete = 0; ";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pkid", apId)
                };
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, sqlPrams)
                    .ConvertTo<ShareBargainProductModel>().FirstOrDefault();
            }
        }

        //添加砍价商品/优惠券
        public static int AddSharBargainProduct(ShareBargainProductModel request, string Operator)
        {
            string sql = @"INSERT INTO Configuration..BargainProduct
                                       ([PID]
                                       ,[productName]
                                       ,[OriginalPrice]
                                       ,[FinalPrice]
                                       ,[Sequence]
                                       ,[Image1]
                                       ,[WXShareTitle]
                                       ,[APPShareId]
                                       ,[Times]
                                       ,[BeginDateTime]
                                       ,[EndDateTime]
                                       ,[TotalStockCount]
                                       ,[CurrentStockCount]
                                       ,[Operator]
                                       ,[CreateDateTime] 
                                       ,[PageName]
                                       ,[SuccessfulHint]
                                       ,[ShowBeginTime]
                                       ,[ProductType]
                                       ,[SimpleDisplayName]
                                       ,[HelpCutPriceTimes]
                                       ,[CutPricePersonLimit]
                                       ,BigCutBeforeCount
                                       ,BigCutPriceRate
                                       ,ProductDetailImg1
                                       ,ProductDetailImg2
                                       ,ProductDetailImg3
                                       ,ProductDetailImg4
                                       ,ProductDetailImg5
                                       ,[IsDelete])
                                 VALUES
                                       (@PID
                                       ,@ProductName
                                       ,@OriginalPrice
                                       ,@FinalPrice
                                       ,@Sequence
                                       ,@Image1
                                       ,@WXShareTitle
                                       ,@APPShareId
                                       ,@Times
                                       ,@BeginDateTime
                                       ,@EndDateTime
                                       ,@TotalStockCount
                                       ,@TotalStockCount
                                       ,@Operator
                                       ,GETDATE() 
                                       ,@PageName
                                       ,@SuccessfulHint
                                       ,@ShowBeginTime
                                       ,@ProductType
                                       ,@SimpleDisplayName
                                       ,@HelpCutPriceTimes
                                       ,@CutPricePersonLimit
                                       ,@BigCutBeforeCount
                                       ,@BigCutPriceRate
                                       ,@ProductDetailImg1
                                       ,@ProductDetailImg2
                                       ,@ProductDetailImg3
                                       ,@ProductDetailImg4
                                       ,@ProductDetailImg5
                                       ,0);
                                       SELECT SCOPE_IDENTITY()";
            var result = 0;
            try
            {
                using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
                {
                    var sqlPrams = new SqlParameter[]
                    {
                    new SqlParameter("@PID", request.PID),
                    new SqlParameter("@ProductName", request.ProductName),
                    new SqlParameter("@OriginalPrice", request.OriginalPrice),
                    new SqlParameter("@FinalPrice", request.FinalPrice),
                    new SqlParameter("@Sequence", request.Sequence),
                    new SqlParameter("@Image1", request.Image1??""),
                    new SqlParameter("@WXShareTitle", request.WXShareTitle),
                    new SqlParameter("@APPShareId", request.APPShareId),
                    new SqlParameter("@Times", request.Times),
                    new SqlParameter("@BeginDateTime", request.BeginDateTime),
                    new SqlParameter("@EndDateTime", request.EndDateTime),
                    new SqlParameter("@TotalStockCount", request.TotalStockCount),
                    new SqlParameter("@Operator", Operator),
                    new SqlParameter("@PageName", request.PageName),
                    new SqlParameter("@SuccessfulHint", request.SuccessfulHint),
                    new SqlParameter("@ShowBeginTime", request.ShowBeginTime),
                    new SqlParameter("@ProductType", request.ProductType),
                    new SqlParameter("@SimpleDisplayName",request.SimpleDisplayName),
                    new SqlParameter("@HelpCutPriceTimes",request.HelpCutPriceTimes),
                    new SqlParameter("@CutPricePersonLimit",request.CutPricePersonLimit),
                    new SqlParameter("@BigCutBeforeCount",request.BigCutBeforeCount),
                    new SqlParameter("@BigCutPriceRate",request.BigCutPriceRate),
                    new SqlParameter("@ProductDetailImg1",request.ProductDetailImg1),
                    new SqlParameter("@ProductDetailImg2",request.ProductDetailImg2),
                    new SqlParameter("@ProductDetailImg3",request.ProductDetailImg3),
                    new SqlParameter("@ProductDetailImg4",request.ProductDetailImg4),
                    new SqlParameter("@ProductDetailImg5",request.ProductDetailImg5),
                    };
                    int.TryParse(dbHelper.ExecuteScalar(sql, CommandType.Text, sqlPrams).ToString(), out result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 添加砍价优惠券 - 未调用
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static bool AddSharBargainCoupon(ShareBargainProductModel request, string Operator)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pid", request.PID),
                    new SqlParameter("@productName", request.ProductName),
                    new SqlParameter("@OriginalPrice", request.OriginalPrice),
                    new SqlParameter("@FinalPrice", request.FinalPrice),
                    new SqlParameter("@Sequence", request.Sequence),
                    new SqlParameter("@Image1", request.Image1??""),
                    new SqlParameter("@WXShareTitle", request.WXShareTitle),
                    new SqlParameter("@APPShareId", request.APPShareId),
                    new SqlParameter("@Times", request.Times),
                    new SqlParameter("@BeginDateTime", request.BeginDateTime),
                    new SqlParameter("@EndDateTime", request.EndDateTime),
                    new SqlParameter("@TotalStockCount", request.TotalStockCount),
                    new SqlParameter("@Operator", Operator),
                    new SqlParameter("@PageName", request.PageName),
                    new SqlParameter("@SuccessfulHint", request.SuccessfulHint),
                    new SqlParameter("@ShowBeginTime", request.ShowBeginTime),
                    new SqlParameter("@ProductType", 2),
                    new SqlParameter("@simpleDisplayName",request.SimpleDisplayName),
                    new SqlParameter("@HelpCutPriceTimes",request.HelpCutPriceTimes),
                    new SqlParameter("@CutPricePersonLimit",request.CutPricePersonLimit)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4AddSharBargainCoupon, CommandType.Text,
                           sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 检查商品pid是否添加砍价活动
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="beginDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public static bool CheckBargainProductByPid(string PID, DateTime beginDateTime, DateTime endDateTime)
        {
            string sql = @" SELECT  COUNT(1)
                            FROM    Configuration..BargainProduct WITH ( NOLOCK )
                            WHERE   IsDelete = 0
                                    AND PID = @pid
                                    AND ( ( BeginDateTime > DATEADD(DAY, -1, @begin)
                                            AND BeginDateTime < DATEADD(DAY, 1, @end)
                                          )
                                          OR ( EndDateTime > DATEADD(DAY, -1, @begin)
                                               AND EndDateTime < DATEADD(DAY, 1, @end)
                                             )
                                        );";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pid", PID),
                    new SqlParameter("@begin",beginDateTime),
                    new SqlParameter("@end",endDateTime)
                };
                int result = 0;
                var dat = dbHelper.ExecuteScalar(sql, CommandType.Text, sqlPrams);
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result > 0;
                }
                else
                {
                    Logger.Log(Level.Warning, $"{dat}转换失败");
                    return false;
                }
            }
        }

        public static CheckPidResult CheckProductByPid(string PID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pid", PID)
                };
                var dat = dbHelper.ExecuteDataTable(ShareBargainSqlText.Sql4CheckProductByPid, CommandType.Text,
                    sqlPrams).ConvertTo<CheckPidResult>().FirstOrDefault();
                if (dat == null)
                {
                    return new CheckPidResult
                    {
                        Code = 3,
                        Info = "产品库中当前没有该商品"
                    };
                }
                return dat;
            }
        }

        public static bool UpdateBargainProductById(ShareBargainProductModel request)
        {
            string sql = @" UPDATE  Configuration..BargainProduct WITH ( ROWLOCK )
                            SET     BeginDateTime = @begindate ,
                                    EndDateTime = @enddate ,
                                    TotalStockCount = @totalstockcount ,
                                    CurrentStockCount = @currentstockcount ,
                                    PageName = @pagename ,
                                    Sequence = @sequence ,
                                    Image1 = @image ,
                                    SuccessfulHint = @successfulhint ,
                                    WXShareTitle = @wxshretitle ,
                                    APPShareId = @appshareid ,
                                    ShowBeginTime = @ShowBeginTime ,
                                    SimpleDisplayName = @simpleDisplayName,
                                    ProductDetailImg1 = @ProductDetailImg1,
                                    ProductDetailImg2 = @ProductDetailImg2,
                                    ProductDetailImg3 = @ProductDetailImg3,
                                    ProductDetailImg4 = @ProductDetailImg4,
                                    ProductDetailImg5 = @ProductDetailImg5
                            WHERE   PKID = @pkid
                                    AND IsDelete = 0;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@sequence", request.Sequence),
                    new SqlParameter("@image", request.Image1),
                    new SqlParameter("@wxshretitle ", request.WXShareTitle),
                    new SqlParameter("@appshareid", request.APPShareId),
                    new SqlParameter("@begindate", request.BeginDateTime),
                    new SqlParameter("@enddate", request.EndDateTime),
                    new SqlParameter("@ShowBeginTime", request.ShowBeginTime),
                    new SqlParameter("@totalstockcount",request.TotalStockCount),
                    new SqlParameter("@currentstockcount", request.CurrentStockCount),
                    new SqlParameter("@pagename", request.PageName),
                    new SqlParameter("@successfulhint", request.SuccessfulHint),
                    new SqlParameter("@pkid",request.PKID),
                    new SqlParameter("@simpleDisplayName",request.SimpleDisplayName),
                    new SqlParameter("@ProductDetailImg1",request.ProductDetailImg1),
                    new SqlParameter("@ProductDetailImg2",request.ProductDetailImg2),
                    new SqlParameter("@ProductDetailImg3",request.ProductDetailImg3),
                    new SqlParameter("@ProductDetailImg4",request.ProductDetailImg4),
                    new SqlParameter("@ProductDetailImg5",request.ProductDetailImg5)
                };
                return dbHelper.ExecuteNonQuery(sql, CommandType.Text, sqlPrams) > 0;
            }
        }

        public static bool DeleteBargainProductById(int PKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"UPDATE  Configuration..BargainProduct
                                SET     IsDelete = 1
                               WHERE   PKID = @pkid;";

                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pkid", PKID)
                };
                return dbHelper.ExecuteNonQuery(sql, CommandType.Text, sqlPrams) > 0;
            }
        }
    }
}

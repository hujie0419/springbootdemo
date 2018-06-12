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
                return dbHelper.ExecuteDataTable(ShareBargainSqlText.sql4selectBargainProductList, CommandType.Text,
                    sqlPrams).ConvertTo<ShareBargainItemModel>();
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
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pkid", apId)
                };
                return dbHelper.ExecuteDataTable(ShareBargainSqlText.Sql4FetchBargainProductById, CommandType.Text,
                    sqlPrams).ConvertTo<ShareBargainProductModel>().FirstOrDefault();
            }
        }

        //添加砍价商品
        public static bool AddSharBargainProduct(ShareBargainProductModel request, string Operator)
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
                    new SqlParameter("@Image1", request.Image1),
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
                    new SqlParameter("@ProductType", 1),
                    new SqlParameter("@simpleDisplayName",request.SimpleDisplayName)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4AddSharBargainProduct, CommandType.Text,
                           sqlPrams) > 0;
            }
        }
        //添加砍价优惠券
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
                    new SqlParameter("@Image1", request.Image1),
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
                    new SqlParameter("@simpleDisplayName",request.SimpleDisplayName)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4AddSharBargainCoupon, CommandType.Text,
                           sqlPrams) > 0;
            }
        }

        //检查商品pid是否可用
        public static bool CheckBargainProductByPid(string PID, DateTime beginDateTime, DateTime endDateTime)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pid", PID),
                    new SqlParameter("@begin",beginDateTime),
                    new SqlParameter("@end",endDateTime)
                };
                int result = 0;
                var dat = dbHelper.ExecuteScalar(ShareBargainSqlText.Sql4CheckBargainProductByPid,
                    CommandType.Text,
                    sqlPrams);
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
                    new SqlParameter("@simpleDisplayName",request.SimpleDisplayName)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4UpdateBargainProductById, CommandType.Text,
                           sqlPrams) > 0;
            }

        }

        public static bool UpdateBargainCouponById(ShareBargainProductModel request)
        {
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
                    new SqlParameter("@simpleDisplayName",request.SimpleDisplayName)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4UpdateBargainCouponById, CommandType.Text,
                           sqlPrams) > 0;
            }

        }

        public static bool DeleteBargainProductById(int PKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var sqlPrams = new SqlParameter[]
                {
                    new SqlParameter("@pkid", PKID)
                };
                return dbHelper.ExecuteNonQuery(ShareBargainSqlText.Sql4DeleteBargainProductById, CommandType.Text,
                           sqlPrams) > 0;
            }
        }
    }
}

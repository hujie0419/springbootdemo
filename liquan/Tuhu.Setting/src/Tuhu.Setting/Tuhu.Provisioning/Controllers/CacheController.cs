using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using Tuhu.Service.Activity.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Controllers
{
    public class CacheController : Controller
    {
        //
        // GET: /Cache/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UpdateFlashSale(DateTime StartTime, DateTime EndTime)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(@"SELECT  FS.ActivityID ,
                                            FS.ActivityName ,
                                            FS.StartDateTime ,
                                            FS.EndDateTime ,
                                            FS.CreateDateTime ,
                                            FS.UpdateDateTime ,
                                            FS.Area ,
                                            FS.BannerUrlAndroid ,
                                            FS.BannerUrlIOS ,
                                            FS.AppVlueAndroid ,
                                            FS.AppVlueIOS ,
                                            FS.BackgoundColor ,
                                            FS.TomorrowText ,
                                            FS.IsBannerIOS ,
                                            FS.IsBannerAndroid ,
                                            FS.ShowType ,
                                            FS.ShippType ,
                                            FS.IsTomorrowTextActive ,
                                            FS.CountDown ,
                                            FS.Status ,
                                            FS.WebBanner ,
                                            FS.WebCornerMark ,
                                            FS.WebBackground ,
                                            FS.IsNoActiveTime ,
                                            FS.EndImage ,
                                            FS.IsEndImage ,
                                            FS.WebOtherPart ,
                                            FS.ActiveType ,
                                            FS.PCodeIDS ,
                                            FS.ShoppingCart ,
                                            FS.H5Url ,
                                            FS.PlaceQuantity ,
                                            FSP.PKID ,
                                            FSP.PID ,
                                            FSP.Position ,
                                            FSP.Price ,
                                            FSP.TotalQuantity ,
                                            FSP.MaxQuantity ,
                                            FSP.SaleOutQuantity ,
                                            ISNULL(FSP.ProductName, VP.DisplayName) AS ProductName ,
                                            ISNULL(VP.Image_filename,
                                                   ISNULL(VP.Image_filename_2,
                                                          ISNULL(VP.Image_filename_3,
                                                                 ISNULL(Image_filename_4, Image_filename_5)))) AS ProductImg ,
                                            FSP.InstallAndPay ,
                                            FSP.Level ,
                                            FSP.ImgUrl ,
                                            FSP.IsUsePCode ,
                                            FSP.Channel ,
                                            FSP.FalseOriginalPrice ,
                                            FSP.IsJoinPlace
                                    FROM    Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
                                            JOIN Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FS.ActivityID = FSP.ActivityID
                                            JOIN Tuhu_productcatalog..vw_Products AS VP WITH ( NOLOCK ) ON FSP.PID = VP.PID
                                    WHERE   VP.OnSale = 1
                                            AND VP.stockout = 0
                                            AND ( FS.CreateDateTime >= @StartTime
                                                  OR FS.UpdateDateTime >= @StartTime
                                                )
                                            AND ( FS.CreateDateTime <= @EndTime
                                                  OR FS.UpdateDateTime <= @EndTime
                                                );");
                cmd.Parameters.AddWithValue("@StartTime", StartTime);
                cmd.Parameters.AddWithValue("@EndTime", EndTime);

                var list = DbHelper.ExecuteQuery(true, cmd, dt =>
               {
                   return new KeyValuePair<IEnumerable<FlashSaleModel>, IEnumerable<FlashSaleProductModel>>(dt.ConvertTo<FlashSaleModel>(), dt.ConvertTo<FlashSaleProductModel>());
               });
                var listResult = new List<FlashSaleModel>();
                if (list.Key.Any() && list.Value.Any())
                {
                    foreach (var activity in list.Key)
                    {
                        if (!listResult.Any(C => C.ActivityID == activity.ActivityID))
                            listResult.Add(activity);
                    }
                    foreach (var item in listResult)
                    {

                        item.Products = list.Value.Where(C => C.ActivityID == item.ActivityID);
                    }
                    return Json(1);
                }
                return Json(-1);
            }
        }

        public ActionResult TireCacheRefresh()
        {
            var useremail = ThreadIdentity.Operator.Name;
            var userArr = new List<string>() { "wangxiaoyu", "diaojingwen", "hulang", "liuzichao", "wangminyou", "liuchao" };
            if (!userArr.Any(_ => useremail.Contains(_)) && !useremail.Contains("途虎系统"))
                throw new Exception($"如需刷新缓存请联系{String.Join(",", userArr)}");
            else
                return View();
        }
        [HttpPost]
        public ActionResult RefreshProductStock()
        {
            using (var client = new CacheClient())
            {
                var result = client.RefreshProductStockCache();
                result.ThrowIfException(true);
                return Json(result.Success && result.Result);
            }
        }
    }
}

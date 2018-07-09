using System;
using System.ServiceModel;
using System.Web.Mvc;
using Tuhu.Service.Activity;
using Tuhu.Service.Config;
using Tuhu.Service.Member;
using Tuhu.Service.Vehicle;

namespace Tuhu.Provisioning.Controllers
{
    public class ServiceUtTestController : Controller
    {
        // GET: ServiceUtTest
        public ActionResult Index()
        {
            return View();
        }
        private static EndpointAddress GetUThost(EndpointAddress address, string host, int port)
        {
            var ub = new UriBuilder(address.Uri) { Host = host, Port = port };
            return new EndpointAddress(ub.Uri, address.Identity, address.Headers);
        }
        public ActionResult FlashSaleTest(string parm, string host, int port)
        {
            try
            {
                using (var client = new FlashSaleClient())
                {
                    client.Endpoint.Address = GetUThost(client.Endpoint.Address, host, port);
                    var result = client.SelectFlashSaleDataByActivityID(new Guid(parm));
                    result.ThrowIfException();
                    return Json(result.Success ? new { Status = 1, Message = "成功" } : new { Status = 0, Message = result.Exception + result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message + ex.InnerException });
            }
        }
        public ActionResult ActivityTest(string parm, string host, int port)
        {
            try
            {
                using (var client = new ActivityClient())
                {
                    client.Endpoint.Address = GetUThost(client.Endpoint.Address, host, port);
                    var result = client.GetActivityUserShareInfo(new Guid(parm));
                    result.ThrowIfException();
                    return Json(result.Success ? new { Status = 1, Message = "成功" } : new { Status = 0, Message = result.Exception + result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message + ex.InnerException });
            }
        }
        public ActionResult ZeroActivityTest(int parm, string host, int port)
        {
            try
            {
                using (var client = new ZeroActivityClient())
                {
                    client.Endpoint.Address = GetUThost(client.Endpoint.Address, host, port);
                    var result = client.FetchZeroActivityDetail(parm);
                    result.ThrowIfException();
                    return Json(result.Success ? new { Status = 1, Message = "成功" } : new { Status = 0, Message = result.Exception + result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message + ex.InnerException });
            }
        }
        #region Member服务
        public ActionResult PromotionTest(string parm, string host, int port)
        {
            try
            {
                using (var client = new PromotionClient())
                {
                    client.Endpoint.Address = GetUThost(client.Endpoint.Address, host, port);
                    var result = client.SelectUserPromotion(new Guid(parm));
                    result.ThrowIfException();
                    return Json(result.Success ? new { Status = 1, Message = "成功" } : new { Status = 0, Message = result.Exception + result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message + ex.InnerException });
            }
        }
        #endregion

        #region MyRegion
        public ActionResult VehicleTest(string parm, string host, int port)
        {
            try
            {
                using (var client = new VehicleClient())
                {
                    client.Endpoint.Address = GetUThost(client.Endpoint.Address, host, port);
                    var result = client.GetAllUserVehicles(new Guid(parm));
                    result.ThrowIfException();
                    return Json(result.Success ? new { Status = 1, Message = "成功" } : new { Status = 0, Message = result.Exception + result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message + ex.InnerException });
            }
        }

        #endregion
        //#region Config服务
        //public ActionResult HomePageTest(string parm, string host, int port)
        //{
        //    try
        //    {
        //        var cityid = Convert.ToInt32(parm.Split(',')[0]);
        //        var version = parm.Split(',')[1];
        //        using (var client = new HomePageClient())
        //        {
        //            client.Endpoint.Address = GetUThost(client.Endpoint.Address, host, port);
        //            var result = client.SelectHomePageConfigs(cityid, version,"_hw");
        //            result.ThrowIfException();
        //            return Json(result.Success ? new { Status = 1, Message = "成功" } : new { Status = 0, Message = result.Exception + result.ErrorMessage });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Status = 0, Message = ex.Message + ex.InnerException });
        //    }
        //}
        //#endregion
    }
}
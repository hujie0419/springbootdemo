using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using ThBiz.Business;
using ThBiz.Business.EmployeeManagement;
using ThBiz.Business.Record;
using ThBiz.Common.ValidatedCode;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.DataAccess.Entity;
using swc = System.Web.Configuration;
using BaoYangManager = Tuhu.Provisioning.Business.BaoYang.BaoYangManager;
using Common.Logging;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangYearCardController : Controller
    {
        #region 保养年卡适配

        [PowerManage]
        public ActionResult BaoYangYearCard()
        {
            return View();
        }

        /// <summary>
        /// 查询年卡信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pid"></param>
        /// <param name="category"></param>
        /// <param name="fuelBrand"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBaoYangYearCardInfo(int pageIndex, string pid, string category, string fuelBrand, string productId)
        {
            BaoYangManager manager = new BaoYangManager();
            var pageSize = 15;
            var result = manager.SelectAllBaoYangYearCard(pageIndex, pageSize, pid, category, fuelBrand, productId);
            var totalCount = manager.SelectBaoYangYearCardCount(pid, category, fuelBrand, productId);
            return Json( new { Status = result.Item1, Data = result.Item2.OrderByDescending(p=>p.Pkid), TotalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取机油品牌
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllFuelBrand()
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectAllFuelBrand();
            return Json(new {Status = result.Item1, Data = result.Item2}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取机油级别
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectAllOilLevel()
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectAllOilLevel();
            return Json(new { Status = result.Item1, Data = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取机油粘度
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectOilviscosity()
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectOilviscosity();
            return Json(new { Status = result.Item1, Data = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOilBundle(string brand, string level, string viscosity)
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectOilBundle(brand, level, viscosity);
            return Json(new { Status = result.Item1, Data = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取机油产品名
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="level"></param>
        /// <param name="viscosity"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public JsonResult SelectOilDisplayName(string brand, string level, string viscosity, string unit)
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectOilDisplayNameByProperty(brand, level, viscosity, unit);
            return Json(new { Status = result!=null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证年卡PID
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public JsonResult IsExistBaoYangYearCardPid(string pid)
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.IsBaoYangYearCardPidValidate(pid);
            return Json(new {Status = result.Item1, Msg = result.Item2}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除保养年卡
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [PowerManage]
        [HttpPost]
        public JsonResult DeleteBaoYangYearCard(int pkid)
        {
            BaoYangManager manager = new BaoYangManager();
            var user = User.Identity.Name;
            var result = manager.DeleteBaoYangYearCard(pkid, user);
            return Json(new {Status = result});
        }

        #endregion

        #region 保养年卡详细信息

        [PowerManage]
        public ActionResult YearCardDetail(int YearCardId = 0)
        {
            ViewBag.Pkid = 0;
            if (YearCardId == 0)
            {               
                return View();
            }
            BaoYangManager manager = new BaoYangManager();
            var data = manager.SelectYearCardInfoByPkid(YearCardId);
            if (data != null && data.Count > 0)
            {
                ViewBag.Pkid = YearCardId;
                return View(data);
            }
            return View();
        }

        /// <summary>
        /// 获取年卡保养项目
        /// </summary>
        /// <param name="yearCardId"></param>
        /// <returns></returns>
        public JsonResult GetYearCardDetails(int yearCardId)
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectBaoYangYearCardDetails(yearCardId);
            return Json(new {Status = result.Item1, Data = result.Item2}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 上传年卡图片
        /// </summary>
        /// <returns></returns>
        [PowerManage]
        public ActionResult ImageUploadToAli()
        {
            string _Image = string.Empty;
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var filename = Path.GetFileName(Imgfile.FileName);
                    var ext = Path.GetExtension(Imgfile.FileName);
                    var client = new WcfClinet<IFileUpload>();
                    //_Image = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"] + filename;
                    _Image = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"] +Guid.NewGuid().ToString()+ ext;
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    var result_1 = client.InvokeWcfClinet(w => w.UploadImage(_Image, buffer, 0, 0));
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            return Json(new
            {
                Image = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _Image,
                Msg = ex == null ? "上传成功" : ex.Message
            }, "text/html");
        }

        public class WcfClinet<TService> where TService : class
        {
            public TReturn InvokeWcfClinet<TReturn>(Expression<Func<TService, TReturn>> operation)
            {
                var channelFactory = new ChannelFactory<TService>("*");
                TService channel = channelFactory.CreateChannel();

                using (var client = (IClientChannel)channel)
                {
                    client.Open();
                    TReturn result = operation.Compile().Invoke(channel);
                    try
                    {
                        if (client.State != CommunicationState.Faulted)
                        {
                            client.Close();
                        }
                    }
                    catch
                    {
                        client.Abort();
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 读取年卡配置文件
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBaoYangYearCardConfig()
        {
            BaoYangConfigManager configManager = new BaoYangConfigManager();
            var config = configManager.GetBaoYangYearCardConfig();
            return Json(new { Status = config != null, Data = config }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证产品PID
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public JsonResult CheckProductName(string pid)
        {
            string result =string.Empty;
            BaoYangManager manager = new BaoYangManager();
            if (!string.IsNullOrWhiteSpace(pid))
            {
                result = manager.SelectProductNameByPid(pid);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            return !string.IsNullOrWhiteSpace(result) ? Json(new { status = "success", Data = result }, JsonRequestBehavior.AllowGet)
                                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增或更改年卡信息
        /// </summary>
        /// <param name="yearCard"></param>
        /// <param name="shop"></param>
        /// <param name="detail"></param>
        /// <param name="promotion"></param>
        /// <returns></returns>
        [PowerManage]
        [HttpPost]
        public JsonResult AddBaoYangYearCardInfo(string yearCard, string shop, string detail, string promotion)
        {
            var mainYearCard = JsonConvert.DeserializeObject<YearCardParameter>(yearCard);
            var yearCardShop = JsonConvert.DeserializeObject<List<BaoYangYearCardShop>>(shop);
            var yearCardDetail = JsonConvert.DeserializeObject<List<BaoYangYearCardDetail>>(detail);
            var promotionPercentage = JsonConvert.DeserializeObject<List<BaoYangYearCardPromotion>>(promotion);
            bool IsAddShopSucceed = false;
            bool IsAddCardDetailSucceed = false;
            bool IsAddPromotionSuccess = false;
            BaoYangManager manager = new BaoYangManager();
            var user = User.Identity.Name;
            if (mainYearCard.Pkid == 0) //新增一张年卡
            {
                var addYearCard = manager.AddBaoYangYearCard(mainYearCard, user);
                if (addYearCard.Item1)
                {
                    var newYearCardId = addYearCard.Item2;
                    foreach (var itemShop in yearCardShop)
                    {
                        itemShop.YearCardId = newYearCardId;
                    }
                    foreach (var itemDeatil in yearCardDetail)
                    {
                        itemDeatil.YearCardId = newYearCardId;
                    }
                    foreach (var itemPromotion in promotionPercentage) {
                        itemPromotion.YearCardId = newYearCardId;
                    }
                    IsAddShopSucceed = yearCardShop.Count <= 0 || manager.AddBaoYangYearCardShop(yearCardShop, user);
                    IsAddCardDetailSucceed = yearCardDetail.Count <= 0 || manager.AddBaoYangYearCardDetail(yearCardDetail, user);
                    IsAddPromotionSuccess = promotionPercentage.Count <= 0 || manager.AddYearCardPromotionPercentage(promotionPercentage, user);
                    if (IsAddShopSucceed && IsAddCardDetailSucceed && IsAddPromotionSuccess)
                    {
                        return Json(new {Status = true, Data = newYearCardId });
                    }
                } 
            }
            else //更新原有年卡数据
            {
                var IsUpdateYearCardSucceed = manager.UpdateBaoYangYearCardInfo(mainYearCard, user);
                IsAddShopSucceed = yearCardShop.Count <= 0 || manager.AddBaoYangYearCardShop(yearCardShop, user);
                IsAddCardDetailSucceed = yearCardDetail.Count <= 0 || manager.AddBaoYangYearCardDetail(yearCardDetail, user);
                IsAddPromotionSuccess = promotionPercentage.Count <= 0 || manager.AddYearCardPromotionPercentage(promotionPercentage, user);
                if (IsUpdateYearCardSucceed && IsAddShopSucceed && IsAddCardDetailSucceed && IsAddPromotionSuccess)
                {
                    return Json(new { Status = true, Data = mainYearCard.Pkid });
                }
            }
            return Json(new { Status = false });
        }

        #endregion

        #region 年卡机油推荐适配页

        [PowerManage]
        public ActionResult BaoYangYearCardConfig()
        {
            BaoYangManager manager = new BaoYangManager();
            List<string> oil = manager.SelectAllFuelBrand().Item2;
            List<YearCardConfig> config = manager.SelectYearCardConfig();
            var model = new Tuple<List<YearCardConfig>, List<string>> (config, oil);
            return View(model);
        }

        /// <summary>
        /// 更新年卡机油推荐配置
        /// </summary>
        /// <param name="configObj"></param>
        /// <returns></returns>
        [PowerManage]
        [HttpPost]
        public JsonResult AddOrUpdateYearCardConfig(string configObj)
        {
            var user = User.Identity.Name;
            var configParameter = JsonConvert.DeserializeObject<List<YearCardConfig>>(configObj);
            BaoYangManager manager = new BaoYangManager();
            var isConfigSuccess = manager.AddOrUpdateYearCardConfig(configParameter,user);
            var isRecommendSuccess = manager.AddOrUpdateYearCardRecommendConfig(configParameter, user);
            if (isConfigSuccess && isRecommendSuccess)
            {
                return Json(new { Status = true });
            }
            return Json(new { Status = false });
        }

        /// <summary>
        /// 更新年卡缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateBaoYangYearcardConfig()
        {
            var msg = "更新线上缓存成功！";
            BaoYangManager manager = new BaoYangManager();
            bool result;
            try
            {
                result = manager.UpdateBaoYangYearcardConfig();
            }
            catch (Exception ex) {
                result = false;
                msg = "更新线上缓存失败, 错误信息：" + ex.Message;
            }    
            return Json(new {Status = result, Msg = msg});
        }

        #endregion

        #region 保养年卡优惠券配置

        public ActionResult BaoYangYearCardPromotion()
        {
            BaoYangConfigManager configManager = new BaoYangConfigManager();
            var config = configManager.GetBaoYangYearCardConfig();
            return View(config);
        }

        public JsonResult GetYearCardInfo(long orderId = 0)
        {
            if (orderId <= 0)
            {
                return Json(new { Status = false, Msg = "OrderId是必须的" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangManager();
            var result = manager.SelectBaoYangYearCardInfo(orderId);
            return Json(new { Status = result.Item1, Data = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditYearCardInfo(string promotionCode, string cardItemStr, string PID)
        {
            var manager = new BaoYangManager();
            bool result = manager.EditYearCardInfo(promotionCode, cardItemStr, PID, User.Identity.Name);
            return Json(new { Status = result });
        }

        [HttpPost]
        public JsonResult CreateYearCardPromotionCode(long orderId = 0)
        {
            if (orderId <= 0)
            {
                return Json(new { Status = false, Msg = "OrderId是必须的" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangManager();
            var result = manager.CreateYearCardPromotionCode(orderId);
            return Json(result);
        }

        #endregion
    }
}
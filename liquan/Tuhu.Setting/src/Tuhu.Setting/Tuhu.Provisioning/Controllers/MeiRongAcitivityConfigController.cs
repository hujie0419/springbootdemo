using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.TuhuShop;
using Tuhu.Service.TuhuShop.Request;
using Tuhu.Service.TuhuShop.Models;
using Tuhu.Service.GroupBuying;

namespace Tuhu.Provisioning.Controllers
{
    public class MeiRongAcitivityConfigController : Controller
    {
        private static readonly Lazy<MeiRongAcitivityConfigManager> lazy = new Lazy<MeiRongAcitivityConfigManager>();

        private MeiRongAcitivityConfigManager MeiRongAcitivityConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            ViewBag.Prod = SE_MDBeautyCategoryConfigBLL.SelectList().Where(x => x.ParentId != 0).ToList();
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);
            return View();
        }

        public ActionResult List(MeiRongAcitivityConfig model, int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;

            var lists = MeiRongAcitivityConfigManager.GetMeiRongAcitivityConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<MeiRongAcitivityConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<MeiRongAcitivityConfig>(list.ReturnValue, pager));
        }

        public ActionResult Export(MeiRongAcitivityConfig model)
        {
            using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/门店美容活动列表.xlsx"))))
            {
                int count = 0;
                var list = MeiRongAcitivityConfigManager.GetMeiRongAcitivityConfigList(model, int.MaxValue, 1, out count);
                var xssfWorkbook = new XSSFWorkbook(stream); //创建Workbook对象  2007+   
                                                             // var hssfWorkbook = new HSSFWorkbook(stream)://2003
                if (list.Count > 0)
                {
                    var i = 0;
                    var sheet = xssfWorkbook.GetSheetAt(0);

                    foreach (var item in list)
                    {

                        model.RegionList = MeiRongAcitivityConfigManager.GetRegionRelation(item.Id, 1);
                        string region = string.Empty;
                        if (model.RegionList != null && model.RegionList.Count > 0)
                        {
                            foreach (var regionItem in model.RegionList)
                            {
                                region += regionItem.ProvinceName + "：" + regionItem.CityName + "；";
                            }
                        }

                        var row = sheet.CreateRow((i++) + 1);
                        row.CreateCell(0).SetCellValue(item.Id);
                        row.CreateCell(1).SetCellValue(item.Name);
                        row.CreateCell(2).SetCellValue(region);
                        row.CreateCell(3).SetCellValue(item.CategoryName);
                        row.CreateCell(4).SetCellValue(item.MinShopQuantity);
                        row.CreateCell(5).SetCellValue(item.SignUpStartTime.Value.ToString() + "—" + item.SignUpEndTime.Value.ToString());
                        row.CreateCell(6).SetCellValue(item.PlanStartTime.ToString() + "—" + item.ActivityEndTime.ToString());
                        row.CreateCell(7).SetCellValue(item.MinPrice + "—" + item.MaxPrice);
                        row.CreateCell(8).SetCellValue(item.Status == 1 ? "启用" : "禁用");
                    }
                    Response.AppendHeader("Content-Disposition", "attachment;fileName=美容活动" + ".xlsx");
                    xssfWorkbook.Write(Response.OutputStream);
                    Response.End();
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id = 0)
        {
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);
            ViewBag.ShopService = MeiRongAcitivityConfigManager.GetShopCosmetologyServers(0);
            if (id == 0)
            {
                ViewBag.ZTreeJsonForCategory = SE_MDBeautyCategoryConfigController.SE_MDBeautyCategoryTreeJson("", true);
                MeiRongAcitivityConfig model = new MeiRongAcitivityConfig();
                model.PlanStartTime = DateTime.Now;
                model.ActivityEndTime = DateTime.Now.AddDays(30);
                model.SignUpEndTime = DateTime.Now.AddDays(30);
                model.SignUpStartTime = DateTime.Now;
                return View(model);
            }
            else
            {
                MeiRongAcitivityConfig model = MeiRongAcitivityConfigManager.GetMeiRongAcitivityConfigById(id);
                model.RegionList = MeiRongAcitivityConfigManager.GetRegionRelation(model.Id, 1);
                ViewBag.ZTreeJsonForCategory = SE_MDBeautyCategoryConfigController.SE_MDBeautyCategoryTreeJson(model.CategoryId.ToString(), true);
                return View(model);
            }
        }


        public JsonResult ShopCount(string Regions, string ShopBusinessType, string ShopServices, string ShopTechLevel)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(Regions))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择地区!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(ShopBusinessType))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择门店类型!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(ShopServices))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择门店服务!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(ShopTechLevel))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择门店等级!");
                return Json(dic);
            }

            try
            {
                List<Shop> result = GetShop(Regions, ShopBusinessType, ShopServices, ShopTechLevel);

                List<IGrouping<string, Shop>> group = result?.GroupBy(x => x.City).ToList();
                List<Shop> result1 = new List<Shop>();
                if (group != null && group.Any())
                {
                    foreach (IGrouping<string, Shop> item in group)
                    {
                        Shop model = item.FirstOrDefault();
                        model.TotalCount = item.Count();
                        result1.Add(model);
                    }
                }
                dic.Add("Succeed", "True");
                dic.Add("Message", result1);
                return Json(dic);
            }
            catch
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "服务器错误");
                return Json(dic);
            }
        }

        public JsonResult SendMessage(string Regions, string ShopBusinessType, string ShopServices, string ShopTechLevel, int activityId, string notification)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(Regions))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择地区!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(ShopBusinessType))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择门店类型!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(ShopServices))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择门店服务!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(ShopTechLevel))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "请选择门店等级!");
                return Json(dic);
            }
            if (string.IsNullOrWhiteSpace(notification))
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "通知内容不能为没空!");
                return Json(dic);
            }

            if (activityId == 0)
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "活动为空!");
                return Json(dic);

            }

            List<Shop> result = GetShop(Regions, ShopBusinessType, ShopServices, ShopTechLevel);

            if (result == null || !result.Any())
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "该条件无门店!");
                return Json(dic);
            }

            try
            {
                foreach (var item in result)
                {
                    ShopNotificationRecord model = new ShopNotificationRecord();
                    model.ActivityId = activityId;
                    model.Notification = notification;
                    model.ShopId = item.PKID;

                    MeiRongAcitivityConfigManager.InsertShopNotificationRecord(model);

                    if (!Request.Url.Host.Contains(".tuhu.cn"))
                    {
                        //TuhuMessage.SendSms("18521709141", "亲爱的测试你好！");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(item.Mobile))
                        {
                            //TuhuMessage.SendSms(item.Mobile, "");
                        }
                    }
                }

                dic.Add("Succeed", "True");
                dic.Add("Message", "成功发送!");
                return Json(result);

            }
            catch (Exception)
            {
                dic.Add("Succeed", "False");
                dic.Add("Message", "服务器错误!");
                return Json(result);
            }
        }

        public List<Shop> GetShop(string Regions, string ShopBusinessType, string ShopServices, string ShopTechLevel)
        {
            ShopsForPromotionSearchRequest search = new ShopsForPromotionSearchRequest();
            if (!string.IsNullOrWhiteSpace(Regions))
            {
                search.Regions = JsonConvert.DeserializeObject<RegionInfo[]>(Regions);

            }
            if (!string.IsNullOrWhiteSpace(ShopBusinessType))
            {
                ShopBusinessType = ShopBusinessType.Substring(0, ShopBusinessType.Length - 1);
                search.ShopBusinessType = ShopBusinessType.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(ShopServices))
            {
                ShopServices = ShopServices.Substring(0, ShopServices.Length - 1);
                search.ShopServices = ShopServices.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(ShopTechLevel))
            {
                ShopTechLevel = ShopTechLevel.Substring(0, ShopTechLevel.Length - 1);

                var list = ShopTechLevel.Split(',');
                int[] arr = new int[list.Length];
                for (int i = 0; i < list.Length; i++)
                {
                    arr[i] = Convert.ToInt32(list[i]);
                }
                search.ShopTechLevel = arr;
            }

            search.PageIndex = 1;
            search.PageSize = int.MaxValue;
            List<Shop> result = new List<Shop>();
            using (var client = new ShopClient())
            {
                var action = client.SelectShopForPromotion(search);
                if (action.Success && action.Result.Any())
                {
                    result = action.Result.ToList();
                }
            }
            return result;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(MeiRongAcitivityConfig model)
        {
            if (model.Id != 0)
            {
                if (MeiRongAcitivityConfigManager.UpdateMeiRongAcitivityConfig(model))
                {
                    CleanCache(model.ActivityId);
                    CleanCahce();
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            else
            {
                int outId = 0;
                if (MeiRongAcitivityConfigManager.InsertMeiRongAcitivityConfig(model, ref outId))
                {
                    CleanCahce();
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }

            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(MeiRongAcitivityConfigManager.DeleteMeiRongAcitivityConfig(id));
        }

        [HttpPost]
        public JsonResult CompelStart(int id)
        {
            return Json(MeiRongAcitivityConfigManager.CompelStart(id));
        }
        public JsonResult GetRegion(int id = 0)
        {
            return Json(MeiRongAcitivityConfigManager.GetRegion(id), JsonRequestBehavior.AllowGet);
        }

        public void CleanCache(string activityid)
        {
            try
            {
                Guid actid;
                if (Guid.TryParse(activityid, out actid))
                {
                    using (var client = new Tuhu.Service.Shop.CacheClient())
                    {
                        var result = client.UpdateShopBeautyActivityByActivityId(actid);
                        result.ThrowIfException(true);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void CleanCahce()
        {
            try
            {
                using (var client = new GroupBuyingClient())
                {
                    client.RefreshActivityConfigCache("ios");
                    client.RefreshActivityConfigCache("android");
                    client.RefreshActivityConfigCache("wx");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

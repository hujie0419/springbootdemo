using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Models;
using Tuhu.Provisioning.Business.BaoYang;
using static Tuhu.Provisioning.DataAccess.Entity.OilBrandPriorityModel;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;
using Tuhu.Provisioning.Business.VipBaoYangPackage;

namespace Tuhu.Provisioning.Controllers
{
    public class OilBrandPriorityController : Controller
    {
        #region 机油品牌推荐优先级配置--指定手机号
        public ViewResult OilBrandPhonePriority()
        {
            return View();
        }

        public JsonResult GetAllBrands()
        {
            string PrimaryParentCategory = "Oil";//类目
            var Branddt = BaoYangManager.GetBaoYangCP_Brand(PrimaryParentCategory);
            return Json(new { Status = Branddt != null, Data = Branddt },
               JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOilBrandPhonePriority(string phoneNumber, string brand, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new BaoYangManager();
            var result = manager.SelectOilBrandPhonePriority(phoneNumber, brand, pageIndex, pageSize);
            var totalPage = (result.Item2 % pageSize == 0) ? ((int)result.Item2 / pageSize) : ((int)result.Item2 / pageSize + 1);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2, TotalPage = totalPage },
                JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult MultAddOilBrandPhonePriority(List<string> phones, string brand)
        {

            if (phones == null || !phones.Any() || string.IsNullOrWhiteSpace(brand))
            {
                return Json(new { Status = false, Msg = "请填写推荐信息" });
            }
            var manager = new BaoYangManager();
            List<OilBrandPhonePriorityModel> models = new List<OilBrandPhonePriorityModel>();
            var phoneList = phones.Distinct().ToList();
            foreach (var phone in phoneList)
            {
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    OilBrandPhonePriorityModel model = new OilBrandPhonePriorityModel
                    {
                        PKID = 0,
                        PhoneNumber = phone,
                        Brand = brand
                    };
                    bool? isExistVehicle = DALBaoyang.IsRepeatOilBrandPhonePriority(model);
                    if (isExistVehicle == true)
                    {
                        return Json(new { Status = false, Msg = model.PhoneNumber + "存在重复数据" });
                    }
                    models.Add(model);
                }
            }
            bool? Result = manager.AddOilBrandPhonePriority(models, User.Identity.Name);
            if (Result == null)
            {
                return Json(new { Status = false, Msg = "出现未知错误，请查看错误日志" });
            }
            else if (Result == true)
            {

                return Json(new { Status = true, Msg = "添加成功" });
            }
            else
            {
                return Json(new { Status = false, Msg = "添加失败" });
            }
        }

        [HttpPost]
        public JsonResult AddOrEditOilBrandPhonePriority(OilBrandPhonePriorityModel model)
        {

            if (string.IsNullOrWhiteSpace(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.Brand))
            {
                return Json(new { Status = false, Msg = "请填写推荐信息" });
            }
            var manager = new BaoYangManager();
            bool? isExistVehicle = DALBaoyang.IsRepeatOilBrandPhonePriority(model);
            if (isExistVehicle == true)
            {
                return Json(new { Status = false, Msg = model.PhoneNumber + "存在重复数据" });
            }
            bool? Result = model.PKID > 0 ? manager.EditOilBrandPhonePriority(model, User.Identity.Name) : manager.AddOilBrandPhonePriority(new List<OilBrandPhonePriorityModel> { model }, User.Identity.Name);

            if (Result == null)
            {
                return Json(new { Status = false, Msg = "出现未知错误，请查看错误日志" });
            }
            else if (Result == true)
            {

                return Json(new { Status = true, Msg = "修改成功" });
            }
            else
            {
                return Json(new { Status = false, Msg = "修改失败" });
            }
        }

        [HttpPost]
        public JsonResult DeleteOilBrandPriorityByPKID(string type, int pkid)
        {
            if (pkid < 1)
            {
                return Json(new { Status = false, Msg = "不明白要删除什么" });
            }
            var manager = new BaoYangManager();
            bool? deleteResult = null;
            if (type.ToLower() == "phone")
            {
                deleteResult = manager.DeleteOilBrandPhonePriorityByPKID(pkid, User.Identity.Name);
            }
            else if (type.ToLower() == "region")
            {
                deleteResult = manager.DeleteOilBrandRegionPriorityByPKID(pkid, User.Identity.Name);
            }
            else
            {
                return Json(new { Status = false, Msg = "不明白要删除什么" });
            }
            if (deleteResult == null)
            {
                return Json(new { Status = false, Msg = "出现未知错误，请查看错误日志" });
            }
            else if (deleteResult == true)
            {
                return Json(new { Status = true, Msg = "删除成功" });
            }
            else
            {
                return Json(new { Status = false, Msg = "删除失败" });
            }
        }
        #endregion

        #region 机油品牌推荐优先级配置--指定城市
        public ViewResult OilBrandRegionPriority()
        {
            return View();
        }

        /// <summary>
        /// 获取所有省份
        /// </summary>
        /// <returns>Json格式</returns>
        public async Task<ActionResult> GetAllProvince()
        {
            IEnumerable<SimpleRegion> result = null;
            using (var client = new RegionClient())
            {
                var serviceResult = await client.GetAllProvinceAsync();
                serviceResult.ThrowIfException(true);
                result = serviceResult?.Result;
            }
            return Json(new { isSuccess = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过regionId获取市/区
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns>Json格式</returns>
        public async Task<ActionResult> GetRegionByRegionId(int regionId)
        {
            List<Tuhu.Service.Shop.Models.Region.Region> result = null;
            using (var client = new RegionClient())
            {
                var serviceResult = await client.GetRegionByRegionIdAsync(regionId);
                serviceResult.ThrowIfException(true);
                result = serviceResult?.Result?.ChildRegions.ToList().GroupBy(x => x.CityId).Select(s => s.First()).ToList();
            }
            return Json(new { isSuccess = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOilBrandRegionPriority(string brand, int provinceId = -1, int regionId = -1, int pageIndex = 1, int pageSize = 20)
        {
            #region 直辖市处理
            List<int> directCity = new List<int> { 1, 2, 19, 20 };
            if (directCity.Contains(provinceId))
            {
                regionId = (provinceId == regionId) ? regionId : provinceId;
                provinceId = 0;
            }
            #endregion
            var manager = new BaoYangManager();
            var result = manager.SelectOilBrandRegionPriority(provinceId, regionId, brand, pageIndex, pageSize);
            var totalPage = (result.Item2 % pageSize == 0) ? ((int)result.Item2 / pageSize) : ((int)result.Item2 / pageSize + 1);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2, TotalPage = totalPage },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddOrEditOilBrandRegionPriority(OilBrandRegionPriorityModel model)
        {
            if (model.ProvinceId < 0 || model.RegionId < 0)
            {
                return Json(new { Status = false, Msg = "请填写城市信息" });
            }
            else if (string.IsNullOrWhiteSpace(model.ProvinceName) || string.IsNullOrWhiteSpace(model.CityName))
            {
                return Json(new { Status = false, Msg = "请填写城市信息" });
            }
            var manager = new BaoYangManager();
            bool? isExistVehicle = DALBaoyang.IsRepeatOilBrandRegionPriority(model);
            if (isExistVehicle == true)
            {
                return Json(new { Status = false, Msg = "存在重复数据" });
            }
            bool? Result = model.PKID > 0 ? manager.EditOilBrandRegionPriority(model, User.Identity.Name)
                : manager.AddOilBrandRegionPriority(model, User.Identity.Name);

            if (Result == null)
            {
                return Json(new { Status = false, Msg = "出现未知错误，请查看错误日志" });
            }
            else if (Result == true)
            {

                return Json(new { Status = true, Msg = model.PKID > 0 ? "修改" : "添加" + "成功" });
            }
            else
            {
                return Json(new { Status = false, Msg = model.PKID > 0 ? "修改" : "添加" + "失败" });
            }
        }
        #endregion

        #region 机油品牌优先级配置--用户购买过的品牌
        public ActionResult OilBrandUserOrder()
        {
            return View();
        }

        /// <summary>
        /// 获取用户购买过的机油
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectOilBrandUserOrder(string phoneNumber, int pageIndex = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var userId = VipBaoYangPackageService.GetUserByMobile(phoneNumber)?.UserId ?? Guid.Empty;
            if (userId == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "未能找到该手机号对应的用户" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangManager();
            var result = manager.SelectOilBrandUserOrder(phoneNumber, userId, pageIndex, pageSize);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 },
                JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 查看日志
        [HttpPost]
        public JsonResult GetVehicleOprLog(string logType, string identityID, DateTime? startTime, DateTime? endTime, int pageIndex = 1, int pageSize = 20)
        {
            string operateUser = null;
            var totalCount = 0;
            if (string.IsNullOrWhiteSpace(logType))
            {
                return Json(new { Status = false, Msg = "日志类型不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var result = new BaoYangManager().SelectBaoYangOprLog(logType, identityID, operateUser, startTime, endTime, pageIndex, pageSize, out totalCount);
            return Json(new { Status = result.Item1, Data = result.Item2, TotalCount = totalCount }, behavior: JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetBaoYangOprLogByPKID(int pkid)
        {
            var manager = new BaoYangManager();
            var Result = manager.GetBaoYangOprLogByPKID(pkid);
            if (Result == null)
            {
                return Json(new { Status = false, Msg = "出现未知错误，请查看错误日志" });
            }
            else
            {
                return Json(new { Status = true, Data = Result });
            }
        }
        #endregion

        #region 清除缓存
        [HttpPost]
        public JsonResult CleanOilBrandPriorityCache(string oilBrandPriorityType)
        {
            var manager = new BaoYangManager();
            var Result = manager.CleanOilBrandPriorityCache(oilBrandPriorityType);
            if (Result)
            {
                return Json(new { Status = Result, Msg = "缓存清理成功" });
            }
            else
            {
                return Json(new { Status = Result, Msg = "缓存清理失败" });
            }
        }
        #endregion


    }
}
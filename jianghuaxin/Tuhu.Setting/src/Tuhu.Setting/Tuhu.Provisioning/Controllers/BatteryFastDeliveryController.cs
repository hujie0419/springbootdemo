using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BatteryFastDeliveryConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Controllers
{
    public class BatteryFastDeliveryController : Controller
    {
        #region BatteryFastDelivery

        // GET: BatteryFastDelivery
        public ActionResult BatteryFastDelivery()
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
                result = serviceResult.Result;
            }
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过regionId获取市/区
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns>Json格式</returns>
        public async Task<ActionResult> GetRegionByRegionId(int regionId)
        {
            Tuhu.Service.Shop.Models.Region.Region result = null;
            using (var client = new RegionClient())
            {
                var serviceResult = await client.GetRegionByRegionIdAsync(regionId);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
            }
            return Json(new { isSuccess = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBatteryServiceType()
        {
            var manager = new BatteryFastDeliveryConfigManager();
            var serviceList = manager.GetAllBatteryServiceType();
            if (serviceList != null && serviceList.Any())
            {
                return Json(new { Status = true, Data = serviceList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "无蓄电池极速达服务" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllBatteryForView(int fastDeliveryId = -1)
        {
            var manager = new BatteryFastDeliveryConfigManager();
            var batteryList = manager.GetAllBatteryForView(fastDeliveryId);
            if (batteryList != null && batteryList.Any())
            {
                return Json(new { Status = true, Data = batteryList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "无蓄电池品牌信息" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBatteryFastDeliveryProductsByFastDeliveryId(int fastDeliveryId)
        {
            if (fastDeliveryId < 1)
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryFastDeliveryConfigManager();
            var batteryList = manager.GetBatteryFastDeliveryProductsByFastDeliveryId(fastDeliveryId);
            if (batteryList.Any())
            {
                return Json(new { Status = true, Data = batteryList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "无适配产品" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SelectBatteryFastDelivery(string serviceTypePid, int pageIndex = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(serviceTypePid))
            {
                return Json(new { Status = false, Msg = "请选择蓄电池极速达服务类目" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryFastDeliveryConfigManager();
            var result = manager.SelectBatteryFastDelivery(serviceTypePid, pageIndex, pageSize);
            if (result.Item1 == null)
            {
                return Json(new { Status = false, Msg = "查询失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var totalPage = (result.Item2 % pageSize == 0) ? ((int)result.Item2 / pageSize) : ((int)result.Item2 / pageSize + 1);
                return Json(new { Status = true, Data = result.Item1, TotalCount = result.Item2, TotalPage = totalPage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddBatteryFastDelivery(BatteryFastDeliveryModel model, List<BatteryFastDeliveryProductsModel> productModels)
        {
            if (model.RegionId < 1)
            {
                return Json(new { Status = false, Msg = "请选择区域信息" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ServiceTypePid))
            {
                return Json(new { Status = false, Msg = "请选择蓄电池极速达服务类目" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryFastDeliveryConfigManager();
            var isRepeat = manager.IsRepeatBatteryFastDelivery(model.ServiceTypePid, model.RegionId);
            if (isRepeat)
            {
                return Json(new { Status = false, Msg = $"已存在重复数据" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddBatteryFastDelivery(model, productModels);
            if (!result.Item1)
            {
                return Json(new { Status = false, Msg = "添加失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = true, Msg = "添加成功,刷新缓存" + (result.Item2 ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteBatteryFastDelivery(int fastDeliveryId, int regionId)
        {
            if (fastDeliveryId < 1 || regionId < 1)
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryFastDeliveryConfigManager();
            var result = manager.DeleteBatteryFastDelivery(fastDeliveryId, regionId);
            if (!result.Item1)
            {
                return Json(new { Status = false, Msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = true, Msg = "删除成功,刷新缓存" + (result.Item2 ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateBatteryFastDelivery(BatteryFastDeliveryModel model, List<BatteryFastDeliveryProductsModel> productModels)
        {
            if (model.PKID < 1)
            {
                return Json(new { Status = false, Msg = "未知的更新对象" }, JsonRequestBehavior.AllowGet);
            }
            if (model.RegionId < 1 || string.IsNullOrWhiteSpace(model.ServiceTypePid))
            {
                return Json(new { Status = false, Msg = "区域信息及服务类目不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BatteryFastDeliveryConfigManager();
            var result = manager.UpdateBatteryFastDelivery(model, productModels);
            if (!result.Item1)
            {
                return Json(new { Status = false, Msg = "更新失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = true, Msg = "更新成功,刷新缓存" + (result.Item2 ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeBatteryFastDeliveryStatus(int pkid = 0, bool isEnabled = false)
        {
            if (pkid <= 0)
            {
                return Json(new { status = false, msg = "指定配置不存在" });
            }
            var manager = new BatteryFastDeliveryConfigManager();
            bool result = manager.ChangeBatteryFastDeliveryStatus(pkid, isEnabled, User.Identity.Name);
            return Json(new { status = result });
        }

        #endregion

        #region 服务类蓄电池产品排序

        /// <summary>
        /// 服务类蓄电池产品排序
        /// </summary>
        /// <returns></returns>
        public ActionResult BatteryFastDeliveryPriority()
        {
            return View();
        }

        /// <summary>
        /// 添加或者修改服务类蓄电池产品优先级
        /// </summary>
        /// <returns></returns>
        public ActionResult AddOrUpdateBatteryFastDeliveryPriority(BatteryFastDeliveryPriority priority)
        {
            var manager = new BatteryFastDeliveryConfigManager();

            Func<string> validFunc = () =>
            {
                if (priority == null)
                {
                    return "参数不能为空";
                }

                if (priority.ProvinceId < 0 || string.IsNullOrWhiteSpace(priority.ProvinceName) ||
                    priority.CityId < 0 || string.IsNullOrWhiteSpace(priority.CityName))
                {
                    return "地区不能为空";
                }

                if (priority.Priorities == null || !priority.Priorities.Any())
                {
                    return "至少选择一个优先级";
                }

                if (manager.IsExistsBatteryFastDeliveryPriority(priority))
                {
                    return "数据已经存在";
                }
                return string.Empty;
            };
            var validResult = validFunc();
            if (!string.IsNullOrEmpty(validResult))
            {
                return Json(new { status = false, msg = validResult });
            }

            var result = priority.PKID > 0 ?
                manager.UpdateBatteryFastDeliveryPriority(priority, User.Identity.Name) :
                manager.AddBatteryFastDeliveryPriority(priority, User.Identity.Name);
            return Json(new { status = result });
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteBatteryFastDeliveryPriority(int id = 0)
        {
            var manager = new BatteryFastDeliveryConfigManager();
            var result = id > 0 ? manager.DeleteBatteryFastDeliveryPriority(id, User.Identity.Name) : false;
            return Json(new { status = result });
        }

        /// <summary>
        /// 获取服务类蓄电池产品配置
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult GetBatteryFastDeliveryPriorities(int? province, int? city, int index = 1, int size = 10)
        {
            var manager = new BatteryFastDeliveryConfigManager();
            var result = manager.GetBatteryFastDeliveryPriorities(province, city, index, size);
            return Json(new
            {
                status = result.Item2 != null,
                data = result.Item2,
                total = result.Item1
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取蓄电池品牌
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBatteryBrands()
        {
            var manager = new BatteryFastDeliveryConfigManager();
            var brands = manager.GetBatteryBrands();
            return Json(brands, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefreshBatteryFastDeliveryPrioritiesCache(int provinceId = -1, int cityId = -1)
        {
            if (provinceId < 0 && cityId < 0)
            {
                return Json(new { status = false, msg = "地区不能为空" });
            }
            var manager = new BatteryFastDeliveryConfigManager();
            var result = manager.RefreshBatteryFastDeliveryPrioritiesCache(provinceId, cityId);
            return Json(new { status = result });
        }

        #endregion
    }
}
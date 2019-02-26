using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Product;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;
using Tuhu.Provisioning.DataAccess.Request;


namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangRecommendController : Controller
    {
        /// <summary>
        /// 保存特殊排序地区配置
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        public async Task<JsonResult> SavePriorityAreaConfig(BaoYangProductPriorityAreaView areas)
        {
            if (areas == null || !areas.Details.Any())
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var regions = areas.Details.Select(x => x.RegionId).Distinct().ToArray();
            if (regions.Length != areas.Details.Count())
            {
                return Json(new { Status = false, Msg = "省级标识不能存在多个!" }, JsonRequestBehavior.AllowGet);
            }
            var citys = areas.Details.Where(x => x.Citys != null && x.Citys.Any()).SelectMany(x => x.Citys)?.ToList() ?? new List<City>();
            if (citys.Any() && citys.Count != citys.Select(x => x.CityId).Distinct().ToArray().Length)
            {
                return Json(new { Status = false, Msg = "市级标识不能存在多个!" }, JsonRequestBehavior.AllowGet);
            }
            var result = await new BaoYangRecommendManager().SaveProductPriorityArea(areas, User.Identity.Name);

            return Json(new { Status = string.IsNullOrWhiteSpace(result.Item2), Msg = result.Item2, Data = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取特殊车型配置地区模板
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetPriorityAreaConfig(string partName)
        {
            if (string.IsNullOrWhiteSpace(partName))
            {
                return Json(new { Status = false, Msg = "参数验证失败" });
            }
            var manager = new BaoYangRecommendManager();
            return Json(new { Status = true, Data = await manager.GetProductPriorityArea(partName.Trim()) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 停用/启用 地区配置模板
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="IsEnabled"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdatePriorityAreaIsEnabled(int areaId, bool isEnabled, string partName)
        {
            if (string.IsNullOrWhiteSpace(partName) || areaId <= 0)
            {
                return Json(new { Status = false, Msg = "参数验证失败" });
            }
            var manager = new BaoYangRecommendManager();
            var result = await manager.UpdatePriorityAreaIsEnabled(areaId, isEnabled, partName.Trim());
            return Json(new { Status = result.Item1, Msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取地区
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetRegion(int areaId, string partName)
        {
            if (string.IsNullOrEmpty(partName))
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangRecommendManager();
            var result = await manager.GetRegion(areaId, partName.Trim());
            return Json(new { Status = true, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取车型推荐排序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult GetVehicleProductPriority(VehicleProductPriorityRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PartName))
            {
                return Json(new { Status = false, Msg = "参数验证失败" });
            }
            request.PageIndex = request.PageIndex > 0 ? request.PageIndex : 1;
            request.PageSize = request.PageSize > 0 ? request.PageSize : 100;
            var manager = new BaoYangRecommendManager();
            var result = manager.GetVehicleProductPriorityView(request);
            return Json(new { Status = true, Data = result.Item2, Total = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存车型推荐排序
        /// </summary>
        /// <param name="views"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveVehicleProductPriority(VehicleProductPriorityRequst request)
        {
            if (request == null || request.views == null || !request.views.Any() ||
                request.Details == null || !request.Details.Any() || request.Details.Any(x => string.IsNullOrWhiteSpace(x.Brand) || string.IsNullOrWhiteSpace(x.Series)) ||
                string.IsNullOrWhiteSpace(request.partName))
            {
                return Json(new { Status = false, Msg = "参数验证失败" });
            }
            var manager = new BaoYangRecommendManager();
            var checkResult = await manager.BatchAddCheckAsync(new List<VehicleProductPriorityRequst>() { request });
            if (!string.IsNullOrWhiteSpace(checkResult))
            {
                return Json(new { Status = false, Msg = checkResult }, JsonRequestBehavior.AllowGet);
            }
            var views = request.views;
            views.ForEach(x => x.Details.AddRange(request.Details));
            var result = manager.SaveVehicleProductPriorityView(views, request.partName, request.AreaId, User.Identity.Name);
            return Json(new { Status = string.IsNullOrWhiteSpace(result.Item2), Msg = result.Item2, Data = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取机油车型推荐排序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult GetVehicleOilProductPriority(OilVehicleProductPriorityRequst request)
        {
            if (request == null)
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            request.PageIndex = request.PageIndex > 0 ? request.PageIndex : 1;
            request.PageSize = request.PageSize > 0 ? request.PageSize : 100;
            var manager = new BaoYangRecommendManager();
            var data = manager.GetVehicleOilProductPriorityView(request);
            return Json(new { Status = true, Msg = string.Empty, Data = data.Item2, Total = data.Item1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存机油车型推荐排序
        /// </summary>
        /// <param name="views"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveVehicleOilProductPriority(VehicleOilProductPriorityRequest request)
        { 
            var manager = new BaoYangRecommendManager();
            var cehckResult = await manager.BatchAddOilCheckAsync(new List<VehicleOilProductPriorityRequest>() { request });
            if (!string.IsNullOrWhiteSpace(cehckResult))
            {
                return Json(new { Status = false, Msg = cehckResult }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.SaveVehicleOilProductPriorityView(request.Views.ToList(), request.AreaId, User.Identity.Name);
            return Json(new { Status = string.IsNullOrWhiteSpace(result.Item2), Msg = result.Item2, Data = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除机油车型推荐排序
        /// </summary>
        /// <param name="areaOilIds"></param>
        /// <returns></returns>
        public JsonResult DeleOilProductPriorityAreaOil(IEnumerable<int> areaOilIds)
        {
            if (areaOilIds == null || !areaOilIds.Any())
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangRecommendManager();
            string result = manager.DeleProductPriorityAreaOilByAreaOilIds(areaOilIds, User.Identity.Name);
            return Json(new { Status = string.IsNullOrWhiteSpace(result), Msg = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除车型推荐排序
        /// </summary>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public JsonResult DeleteProductPriorityAreaDetail(IEnumerable<string> vehicleIds)
        {
            if (vehicleIds == null || !vehicleIds.Any())
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangRecommendManager();
            string result = manager.DeleteProductPriorityAreaDetailByvehicleIds(vehicleIds, User.Identity.Name);
            return Json(new { Status = string.IsNullOrWhiteSpace(result), Msg = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改车型排序配置状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public JsonResult UpdateAreaDetailEnabled(string vehicleId, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangRecommendManager();
            return Json(new { Status = manager.UpdateVehicleAreaDetailEnabledByPkid(vehicleId, isEnabled) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改机油车型推荐排序配置状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public JsonResult UpdateOilAreaEnabled(int areaOilId, bool isEnabled)
        {
            if (areaOilId <= 0)
            {
                return Json(new { Status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangRecommendManager();
            return Json(new { Status = manager.UpdateVehicleOilAreaEnabledByPkid(areaOilId, isEnabled) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取车身类别
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVehicleBodyType()
        {
            var manager = new BaoYangRecommendManager();
            return Json(new { Status = true, Data = manager.GetVehicleBodyType() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> BatchImportExcel()
        {
            if (Request.Files.Count <= 0)
            {
                return Json(new { Status = false, Msg = "没有查找到导入文件" });
            }
            if (Request.Files.Count > 1)
            {
                return Json(new { Status = false, Msg = "只允许导入单个文件" });
            }

            var file = Request.Files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
             file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" }, "text/html");
            }
            if (file.InputStream.Length <= 0)
            {
                return Json(new { Status = false, Msg = "文件中没有包含任何数据" });
            }
            var workBook = new XSSFWorkbook(file.InputStream);
            var sheet = workBook.GetSheetAt(0);
            var manager = new BaoYangRecommendManager();
            var result = await manager.BatchImportExcel(sheet, User.Identity.Name);
            return Json(new { Status = string.IsNullOrWhiteSpace(result), Msg = result });
        }


    }
}
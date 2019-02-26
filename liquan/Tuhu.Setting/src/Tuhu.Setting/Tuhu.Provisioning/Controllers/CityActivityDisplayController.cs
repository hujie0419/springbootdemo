using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.RegionActivityPageConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class CityActivityDisplayController : Controller
    {
        private readonly CityActivityPageConfig cityActivityConfig = new CityActivityPageConfig();
        // GET: CityActivityDisplayConfig
        public ActionResult Index(RegionVehicleIdActivityConfig filter, int pageIndex = 1)
        {
            if (filter.ActivityName != null)
                filter.ActivityName = filter.ActivityName.Trim();
            if (filter.CreateUser != null)
                filter.CreateUser = filter.CreateUser.Trim();
            if (filter.UpdateUser != null)
                filter.UpdateUser = filter.UpdateUser.Trim();
            int totalItem;
            int pageSize = 12;
            var activityList = cityActivityConfig.GetAllActivity(filter, pageIndex, pageSize, out totalItem);
            ViewBag.pageIndex = pageIndex;
            ViewBag.totalRecords = totalItem;
            ViewBag.totalPage = Math.Ceiling(totalItem / (pageSize + 0.0));
            ViewBag.activityList = activityList;
            return View();
        }

        #region vechile
        public ActionResult GetAllBrand()
        {
            var brandList = cityActivityConfig.GetAllBrand();
            return Json(brandList.Select(b => new { b.Brand }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicleByBrand(string brand)
        {
            var vehicleList = cityActivityConfig.GetVehicleByBrand(brand);
            return Json(vehicleList.Select(v => new { v.ProductID, v.Vehicle }), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region activity
        /// <summary>
        /// 删除活动配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpPost, PowerManage]
        public ActionResult DeleteActivity(Guid? activityId)
        {
            MJsonResult json = new MJsonResult();

            if (activityId == null)
            {
                json.Status = false;
                json.Msg = "活动ID不合法";
                return Json(json);
            }
            int id = cityActivityConfig.GetActivityPKIDByActivityId(activityId.Value);
            if (!cityActivityConfig.DeleteActivity(activityId.Value))
            {
                json.Status = false;
                json.Msg = "删除失败";
            }
            else
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = activityId.Value.ToString() + "删除活动", ObjectType = "CVPage", ObjectID = id.ToString() });
                json.Status = true;
                json.Msg = "ok";
            }
            return Json(json);

        }

        /// <summary>
        /// 创建活动配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, PowerManage]
        public ActionResult CreateActivity(RegionVehicleIdActivityConfig model)
        {
            MJsonResult json = new MJsonResult();
            if (string.IsNullOrEmpty(model.ActivityName))
            {
                json.Status = false;
                json.Msg = "活动名不能为空";
            }
            else if (string.IsNullOrEmpty(model.ActivityType))
            {
                json.Status = false;
                json.Msg = "活动类型不合法";
            }
            else if (model.StartTime == null)
            {
                json.Status = false;
                json.Msg = "开始时间不能为空";
            }
            else if (model.EndTime == null)
            {
                json.Status = false;
                json.Msg = "结束时间不能为空";
            }
            else
            {
                model.ActivityName = model.ActivityName.Trim();
                string userno = User.Identity.Name;
                if (string.IsNullOrEmpty(userno))
                {
                    json.Status = false;
                    json.Msg = "获取用户名异常";
                }
                else
                {
                    var activityId = Guid.NewGuid();
                    model.ActivityId = activityId;
                    model.CreateUser = model.UpdateUser = userno;
                    var result = cityActivityConfig.CreateActivity(model);
                    if (result)
                    {
                        int id = cityActivityConfig.GetActivityPKIDByActivityId(activityId);
                        var AfterValue = $"活动名:{model.ActivityName},活动类型:{model.ActivityType},开始时间:{model.StartTime.ToString()},结束时间:{model.EndTime.ToString()},是否启用:{model.IsEnabled}";
                        LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = AfterValue, Author = User.Identity.Name, Operation = $"{activityId.ToString()}创建活动", ObjectType = "CVPage", ObjectID = id.ToString() });
                    }

                    return Json(new { Status = result, Msg = result ? "OK" : "创建失败", Data = activityId });
                }
            }
            return Json(json);
        }

        /// <summary>
        /// 更新活动配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, PowerManage]
        public ActionResult UpdateActivity(RegionVehicleIdActivityConfig model)
        {
            MJsonResult json = new MJsonResult();
            if (model.ActivityId == Guid.Empty)
            {
                json.Status = false;
                json.Msg = "活动ID不合法";
            }
            else if (string.IsNullOrEmpty(model.ActivityName))
            {
                json.Status = false;
                json.Msg = "活动名不能为空";
            }
            else if (model.StartTime == null || model.EndTime == null)
            {
                json.Status = false;
                json.Msg = "活动开始时间或结束时间不合法";
            }
            else if (model.IsEnabled != 0 && model.IsEnabled != 1)
            {
                json.Status = false;
                json.Msg = "是否启用字段不合法";
            }
            string userno = User.Identity.Name;
            if (string.IsNullOrEmpty(userno))
            {
                json.Status = false;
                json.Msg = "用户登录信息获取失败";
            }
            else
            {
                model.UpdateUser = userno;
                json.Status = cityActivityConfig.UpdateActivity(model);
                if (json.Status)
                {
                    int id = cityActivityConfig.GetActivityPKIDByActivityId(model.ActivityId);
                    var AfterValue = $"活动名:{ model.ActivityName },开始时间:{model.StartTime.ToString()},结束时间:{model.EndTime.ToString()},是否启用:{model.IsEnabled}";
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = AfterValue, Author = userno, Operation = $"{model.ActivityId.ToString()}更新活动", ObjectType = "CVPage", ObjectID = id.ToString() });
                }
                json.Msg = json.Status ? "OK" : "更新失败";
            }
            return Json(json);
        }

        #endregion

        #region activityUrl

        /// <summary>
        /// 创建活动页
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateActivityUrl(RegionVehicleIdActivityUrlConfig model, string vehicleIds, string regionIds)
        {
            if (model == null || model.ActivityId == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "未知的创建对象" });
            }
            if (string.IsNullOrWhiteSpace(model.TargetUrl) && string.IsNullOrWhiteSpace(model.WxappUrl))
            {
                return Json(new { Status = false, Msg = "活动页面地址不能全为空" });
            }
            if (model.IsDefault != 0 && model.IsDefault != 1)
            {
                return Json(new { Status = false, Msg = "是否默认页面字段出错" });
            }
            if (model.IsDefault != 1 && string.IsNullOrWhiteSpace(vehicleIds) && string.IsNullOrWhiteSpace(regionIds))
            {
                return Json(new { Status = false, Msg = "车型或地区不能为空" });
            }
            if (!string.IsNullOrWhiteSpace(model.TargetUrl) && cityActivityConfig.IsExistActivityTargetUrl(model.ActivityId, model.TargetUrl))
            {
                return Json(new { Status = false, Msg = "该H5活动页已存在，请勿重复添加" });
            }
            if (!string.IsNullOrWhiteSpace(model.WxappUrl) && cityActivityConfig.IsExistActivityWxappUrl(model.ActivityId, model.WxappUrl))
            {
                return Json(new { Status = false, Msg = "该小程序活动页已存在，请勿重复添加" });
            }
            var result = cityActivityConfig.CreateActivityUrl(model, vehicleIds, regionIds);
            if (result)
            {
                int id = cityActivityConfig.GetActivityPKIDByActivityId(model.ActivityId);//适配日志重构
                var AfterValue = $"H5活动页地址:{model.TargetUrl}" +
                                $",小程序活动页地址:{model.WxappUrl}" +
                                $",是否默认页面:{model.IsDefault},车型ID:{vehicleIds},城市ID:{regionIds}";
                var log = new ConfigHistory()
                {
                    AfterValue = AfterValue,
                    Author = User.Identity.Name,
                    Operation = $"{model.ActivityId}新增活动页",
                    ObjectType = "CVPage",
                    ObjectID = id.ToString()
                };
                LoggerManager.InsertOplog(log);
            }
            return Json(new { Status = result, Msg = $"活动页添加{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 更新活动页
        /// </summary>
        /// <param name="model"></param>
        /// <param name="h5OldUrl"></param>
        /// <param name="wxOldUrl"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateActivityUrl(RegionVehicleIdActivityUrlConfig model, string h5OldUrl, string wxOldUrl, string vehicleIds, string regionIds)
        {
            if (model == null || model.ActivityId == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "未知的更新对象" });
            }
            if (string.IsNullOrWhiteSpace(model.TargetUrl) && string.IsNullOrWhiteSpace(model.WxappUrl))
            {
                return Json(new { Status = false, Msg = "活动页面地址不能全为空" });
            }
            if (model.IsDefault != 0 && model.IsDefault != 1)
            {
                return Json(new { Status = false, Msg = "是否默认页面字段出错" });
            }
            if (model.IsDefault != 1 && string.IsNullOrWhiteSpace(vehicleIds) && string.IsNullOrWhiteSpace(regionIds))
            {
                return Json(new { Status = false, Msg = "车型或地区不能为空" });
            }
            if(string.IsNullOrWhiteSpace(h5OldUrl) && string.IsNullOrWhiteSpace(wxOldUrl))
            {
                return Json(new { Status = false, Msg = "原有活动页链接为空" });
            }
            if (!string.IsNullOrWhiteSpace(model.TargetUrl) && !string.Equals(h5OldUrl, model.TargetUrl))
            {
                var isExist = cityActivityConfig.IsExistActivityTargetUrl(model.ActivityId, model.TargetUrl);
                if (isExist)
                {
                    return Json(new { Status = false, Msg = "已存在该H5活动页" });
                }
            }
            if (!string.IsNullOrWhiteSpace(model.WxappUrl) && !string.Equals(wxOldUrl, model.WxappUrl))
            {
                var isExist = cityActivityConfig.IsExistActivityWxappUrl(model.ActivityId, model.WxappUrl);
                if (isExist)
                {
                    return Json(new { Status = false, Msg = "已存在该小程序活动页" });
                }
            }
            var user = User.Identity.Name;
            var result = cityActivityConfig.UpdateActivityUrl(model, h5OldUrl, wxOldUrl, vehicleIds, regionIds);
            if (result)
            {
                int id = cityActivityConfig.GetActivityPKIDByActivityId(model.ActivityId);
                var AfterValue = $"H5活动页地址:{h5OldUrl}改为{model.TargetUrl}" +
                    $",小程序活动页地址:{wxOldUrl}改为{model.WxappUrl}" +
                    $",是否默认页面:{model.IsDefault},车型ID:{vehicleIds},城市ID:{regionIds}";//适配日志重构
                var log = new ConfigHistory()
                {
                    AfterValue = AfterValue,
                    Author = user,
                    Operation = $"{model.ActivityId.ToString()}更新活动页",
                    ObjectType = "CVPage",
                    ObjectID = id.ToString()
                };
                LoggerManager.InsertOplog(log);
            }
            return Json(new { Status = result, Msg = $"活动页更新{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 删除活动页
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteActivityUrl(Guid activityId, string targetUrl, string wxappUrl)
        {
            if (activityId == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "活动ID不合法" });
            }
            if (string.IsNullOrEmpty(targetUrl))
            {
                return Json(new { Status = false, Msg = "目标Url不能为空" });
            }
            targetUrl = targetUrl.Trim();
            var result = cityActivityConfig.DeleteActivityUrl(activityId, targetUrl, wxappUrl);
            if (result)
            {
                int id = cityActivityConfig.GetActivityPKIDByActivityId(activityId);//适配日志重构
                var AfterValue = $"H5活动页地址:{targetUrl},小程序活动页地址:{wxappUrl}";
                var log = new ConfigHistory()
                {
                    AfterValue = AfterValue,
                    Author = User.Identity.Name,
                    Operation = activityId.ToString() + "删除活动页",
                    ObjectType = "CVPage",
                    ObjectID = id.ToString()
                };
                LoggerManager.InsertOplog(log);
            }
            return Json(new { Status = result, Msg = $"删除活动页面{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 获取该活动下的活动页
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetActivityUrlByActivityId(Guid? activityId)
        {
            if (activityId == null || activityId == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "活动ID无效" }, JsonRequestBehavior.AllowGet);
            }
            var urlList = cityActivityConfig.GetActivityUrlByActivityId(activityId.Value);
            if (urlList == null)
            {
                return Json(new { Status = false, Msg = "获取活动页面地址列表失败" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = true, Msg = "OK", Data = urlList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取该活动页配置的地区
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public ActionResult GetRegionInfoByActivityIdUrl(Guid? activityId, string targetUrl, string wxappUrl)
        {
            var json = new MJsonResult();
            if (activityId == null)
            {
                json.Status = false;
                json.Msg = "活动ID不合法";
            }
            targetUrl = targetUrl?.Trim();
            wxappUrl = wxappUrl?.Trim();
            if (string.IsNullOrWhiteSpace(targetUrl) && string.IsNullOrWhiteSpace(wxappUrl))
            {
                json.Status = false;
                json.Msg = "活动地址不能全为空";
            }
            else
            {
                var regionList = cityActivityConfig.GetRegionIdByActivityIdUrl(activityId.Value, targetUrl, wxappUrl);
                return Json(new { Status = true, Msg = "OK", Data = regionList }, JsonRequestBehavior.AllowGet);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取该活动页配置的车型
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public ActionResult GetVehicleInfoByActivityIdUrl(Guid? activityId, string targetUrl, string wxappUrl)
        {
            var json = new MJsonResult();
            targetUrl = targetUrl?.Trim();
            wxappUrl = wxappUrl?.Trim();
            if (activityId == null)
            {
                json.Status = false;
                json.Msg = "活动ID不合法";
            }
            else if (string.IsNullOrWhiteSpace(targetUrl) && string.IsNullOrWhiteSpace(wxappUrl))
            {
                json.Status = false;
                json.Msg = "活动地址不能全为空";
            }
            else
            {
                var regionList = cityActivityConfig.GetVehicleIdByActivityIdUrl(activityId.Value, targetUrl, wxappUrl);
                return Json(new { Status = true, Msg = "OK", Data = regionList }, JsonRequestBehavior.AllowGet);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 该活动下是否配置了默认活动页
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityChannel"></param>
        /// <returns></returns>
        public ActionResult GetIsExistDefaultUrl(Guid? activityId)
        {
            MJsonResult json = new MJsonResult();
            if (activityId == null)
            {
                json.Status = false;
                json.Msg = "活动ID不合法";
            }
            else
            {
                var res = cityActivityConfig.IsExistDefaultUrl(activityId.Value);
                return Json(new { Status = true, Msg = "OK", Data = res }, JsonRequestBehavior.AllowGet);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 根据活动页链接获取活动页名称
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult GetActivityTitleByUrl(string url)
        {

            MJsonResult json = new MJsonResult();
            if (string.IsNullOrEmpty(url))
            {
                json.Status = false;
                json.Msg = "活动页Url不能为空";
            }
            else
            {
                var result = cityActivityConfig.GetActivityTitleByUrl(url);
                json.Status = true;
                json.Msg = result;
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新活动配置缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult RefreshRegionVehicleIdActivityUrlCache(string activityId)
        {
            Guid ActivityId;
            if (Guid.TryParse(activityId, out ActivityId))
            {
                using (var client = new Service.Activity.ActivityClient())
                {
                    var serviceResult = client.RefreshRegionVehicleIdActivityUrlCache(ActivityId);
                    return Json(new { Status = serviceResult.Success, Msg = serviceResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Status = false, Msg = "请输入有效的活动Id" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看活动配置操作日志
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetOpRecords(int pkid, DateTime? startTime, DateTime? endTime)
        {
            DateTime startDT;
            DateTime endDT;
            if (startTime == null)
                startDT = new DateTime(1970, 1, 1);
            else
                startDT = startTime.Value;
            if (endTime == null)
                endDT = DateTime.Now;
            else
                endDT = endTime.Value;
            List<ConfigHistory> opList = cityActivityConfig.GetOprLog(pkid, startDT, endDT);
            return Json(new { Status = opList == null ? false : true, Msg = opList == null ? "获取失败" : "OK", Data = opList == null ? null : opList.Select(op => new { Author = op.Author, ChangeDatetime = op.ChangeDatetime.ToString("yyyy/MM/dd HH:mm:ss"), Operation = $"{op.Operation.Substring(36)}({op.AfterValue})" }) }, JsonRequestBehavior.AllowGet);
        }
    }
}
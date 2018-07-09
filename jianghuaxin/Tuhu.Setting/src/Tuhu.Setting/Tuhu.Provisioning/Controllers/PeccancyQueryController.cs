using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.PeccancyQueryConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;
using static Tuhu.Provisioning.DataAccess.Entity.PeccancyQueryModel;

namespace Tuhu.Provisioning.Controllers
{
    public class PeccancyQueryController : Controller
    {
        /// <summary>
        /// 获取所有配置的省份
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllProvince()
        {
            var manager = new PeccancyQueryConfigManager();
            var result = manager.GetAllPeccancyProvinces();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取该省份下的城市--下拉框选项
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public ActionResult GetPeccancyCitiesByProvinceId(int provinceId = -1)
        {
            var manager = new PeccancyQueryConfigManager();
            var result = manager.GetPeccancyCitiesByProvinceId(provinceId);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        #region 违章查询城市后台配置--设置省份
        // GET: BreakRuleQuery
        public ActionResult ProvinceConfig()
        {
            return View();
        }

        /// <summary>
        /// 添加违章查询省份配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddPeccancyProvinceConfig(PeccancyQueryProvinceModel model)
        {
            if (model.ProvinceId < 1 || string.IsNullOrWhiteSpace(model.ProvinceName) || string.IsNullOrWhiteSpace(model.ProvinceSimpleName))
            {
                return Json(new { Status = false, Msg = "请确认省份信息" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var existModel = manager.GetRepeatPeccancyProvinceConfig(model);
            if (existModel != null)
            {
                return Json(new { Status = false, Msg = $"存在重复数据:省份Id{existModel.ProvinceId},省份名称{existModel.ProvinceName},省份简称{existModel.ProvinceSimpleName}" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.AddPeccancyProvinceConfig(model, user);
            if (result)
            {
                return Json(new { Status = true, Msg = "添加成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "添加失败" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 删除违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public ActionResult DeletePeccancyProvinceConfig(int provinceId)
        {
            if (provinceId < 1)
            {
                return Json(new { Status = false, Msg = "不明白删除哪个省份" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var user = User.Identity.Name;
            var result = manager.DeletePeccancyProvinceConfig(provinceId, user);
            if (result)
            {
                return Json(new { Status = true, Msg = "删除省份成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "删除省份失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新违章查询省份配置
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        public ActionResult UpdatePeccancyQueryProvinceConfig(PeccancyQueryProvinceModel province)
        {
            if (province.ProvinceId < 1 || string.IsNullOrWhiteSpace(province.ProvinceName) || string.IsNullOrWhiteSpace(province.ProvinceSimpleName))
            {
                return Json(new { Status = false, Msg = "请确认省份信息" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var user = User.Identity.Name;
            var result = manager.UpdatePeccancyProvinceConfig(province, user);
            if (result)
            {
                return Json(new { Status = true, Msg = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "修改失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPeccancyProvinceConfig(int provinceId = -1, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new PeccancyQueryConfigManager();
            var result = manager.SelectPeccancyProvinceConfig(provinceId, pageIndex, pageSize);
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

        /// <summary>
        /// 获取该省份下的城市配置数量
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public ActionResult GetCityConfigCountByProvinceId(int provinceId)
        {
            if (provinceId < 1)
            {
                return Json(new { Status = false, Msg = "不明白查找哪个省份" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var count = manager.GetPeccancyCityConfigCountByPrvinceId(provinceId);
            if (count < 0)
            {
                return Json(new { Status = false, Msg = $"无法获取省份id为{provinceId}的省份下的城市信息" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = true, Data = count }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 违章查询城市后台配置--设置城市
        public ActionResult CityConfig()
        {
            return View();
        }

        /// <summary>
        /// 添加违章查询城市配置
        /// </summary>
        /// <param name="peccancyCityModel"></param>
        /// <returns></returns>
        public ActionResult AddPeccancyQueryCityConfig(PeccancyQueryCityModel peccancyCityModel)
        {
            if (peccancyCityModel.ProvinceId < 1)
            {
                return Json(new { Status = false, Msg = "请确认省份信息" }, JsonRequestBehavior.AllowGet);
            }
            if (peccancyCityModel.CityId < 1 || peccancyCityModel.CityCode < 1 || string.IsNullOrWhiteSpace(peccancyCityModel.CityName))
            {
                return Json(new { Status = false, Msg = "请确认城市信息" }, JsonRequestBehavior.AllowGet);
            }
            else if (peccancyCityModel.NeedEngine && peccancyCityModel.EngineLen < 1)
            {
                return Json(new { Status = false, Msg = "发动机号如必要，请输入发动机号长度" }, JsonRequestBehavior.AllowGet);
            }
            else if (peccancyCityModel.NeedFrame && peccancyCityModel.FrameLen < 1)
            {
                return Json(new { Status = false, Msg = "车架号如必要，请输入车架号长度" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var existModel = manager.GetPeccancyCityConfigByCityId(peccancyCityModel.CityId);
            if (existModel != null)
            {
                return Json(new { Status = false, Msg = $"已存在城市Id为{existModel.CityId}的城市！城市名称{existModel.CityName},城市代码{existModel.CityCode}" });
            }
            var user = User.Identity.Name;
            var result = manager.AddPeccancyCityConfig(peccancyCityModel, user);
            if (result)
            {
                return Json(new { Status = true, Msg = "添加成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "添加失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除违章查询城市配置
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public ActionResult DeletePeccancyQueryCityConfig(int cityId)
        {
            if (cityId < 1)
            {
                return Json(new { Status = false, Msg = "不明白删除哪个城市" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var user = User.Identity.Name;
            var result = manager.DeletePeccancyCityConfig(cityId, user);
            if (result)
            {
                return Json(new { Status = true, Msg = "删除城市成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "删除城市失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新违章查询城市配置
        /// </summary>
        /// <param name="peccancyCityModel"></param>
        /// <returns></returns>
        public ActionResult UpdatePeccancyQueryCityConfig(PeccancyQueryCityModel peccancyCityModel)
        {
            if (peccancyCityModel.ProvinceId < 1)
            {
                return Json(new { Status = false, Msg = "请确认省份信息" }, JsonRequestBehavior.AllowGet);
            }
            if (peccancyCityModel.CityId < 1 || peccancyCityModel.CityCode < 1 || string.IsNullOrWhiteSpace(peccancyCityModel.CityName))
            {
                return Json(new { Status = false, Msg = "请确认城市信息" }, JsonRequestBehavior.AllowGet);
            }
            else if (peccancyCityModel.NeedEngine && peccancyCityModel.EngineLen < 1)
            {
                return Json(new { Status = false, Msg = "发动机号如必要，请输入发动机号长度" }, JsonRequestBehavior.AllowGet);
            }
            else if (peccancyCityModel.NeedFrame && peccancyCityModel.FrameLen < 1)
            {
                return Json(new { Status = false, Msg = "车架号如必要，请输入车架号长度" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var user = User.Identity.Name;
            var result = manager.UpdatePeccancyCityConfig(peccancyCityModel, user);
            if (result)
            {
                return Json(new { Status = true, Msg = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "修改失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询违章查询城市配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPeccancyCityConfig(int provinceId = -1, int cityId = -1, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new PeccancyQueryConfigManager();
            var result = manager.SelectPeccancyCityConfig(provinceId, cityId, pageIndex, pageSize);
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

        /// <summary>
        /// 根据城市Id获取违章查询城市配置
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public ActionResult GetPeccancyCityConfigByCityId(int cityId)
        {
            if (cityId < 1)
            {
                return Json(new { Status = false, Msg = "不明白查找哪个城市" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var result = manager.GetPeccancyCityConfigByCityId(cityId);
            if (result != null)
            {
                return Json(new { Status = true, Data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = $"无法获取城市id为{cityId}的城市信息" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据省份Id获取违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public ActionResult GetPeccancyProvinceConfigByProvinceId(int provinceId)
        {
            if (provinceId < 1)
            {
                return Json(new { Status = false, Msg = "不明白查找哪个省份" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var model = manager.GetPeccancyProvinceConfigByProvinceId(provinceId);
            if (model == null)
            {
                return Json(new { Status = false, Msg = $"无法获取省份id为{provinceId}的省份信息" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = true, Data = model }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 刷新缓存
        /// <summary>
        /// 刷新违章查询城市服务缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult CleanPeccancyCitysCache()
        {
            var manager = new PeccancyQueryConfigManager();
            var result = manager.CleanPeccancyCitysCache();
            return Json(new { Status = result, Msg = "清除缓存" + (result ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 查看操作日志
        /// <summary>
        /// 查看违章配置操作日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <returns></returns>
        public ActionResult SelectPeccancyConfigOprLog(string logType, string identityId)
        {
            if (string.IsNullOrWhiteSpace(logType) || string.IsNullOrWhiteSpace(identityId))
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var result = manager.SelectPeccancyConfigOprLog(logType, identityId);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看违章配置操作记录详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult GetPeccancyConfigOprLog(int pkid)
        {
            if (pkid < 1)
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PeccancyQueryConfigManager();
            var result = manager.GetPeccancyConfigOprLog(pkid);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
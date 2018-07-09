using System.Web.Mvc;
using Tuhu.Provisioning.Business.VehicleAnnualInspectionAgent;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VehicleAnnualInspectionAgentController : Controller
    {
        // GET: AnnualInspectionAgent
        public ActionResult VehicleAnnualInspectionAgent()
        {
            return View();
        }

        /// <summary>
        /// 获取所有服务
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllAnnualInspectionService()
        {
            var manager = new VehicleAnnualInspectionAgentManager();
            var result = manager.GetAllAnnualInspectionService();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有车牌前缀
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllCarNoPrefix()
        {
            var manager = new VehicleAnnualInspectionAgentManager();
            var result = manager.GetAllCarNoPrefix();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllAnnualInspectionVender()
        {
            var manager = new VehicleAnnualInspectionAgentManager();
            var result = manager.GetAllAnnualInspectionVender();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加年检代办配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAgentConfig(VehicleAnnualInspectionAgentModel model)
        {
            if (model == null)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ServicePid))
            {
                return Json(new { Status = false, Msg = "请选择服务Pid" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.CarNoPrefix))
            {
                return Json(new { Status = false, Msg = "请选择车牌前缀" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.VenderShortName))
            {
                return Json(new { Status = false, Msg = "供应商简称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.Contact))
            {
                return Json(new { Status = false, Msg = "联系人不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.TelNum))
            {
                return Json(new { Status = false, Msg = "联系电话不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.OfficeAddress))
            {
                return Json(new { Status = false, Msg = "邮寄地址不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (model.SalePrice < 0 || model.CostPrice < 0)
            {
                return Json(new { Status = false, Msg = "成本或售价不能为负数" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleAnnualInspectionAgentManager();
            var user = User.Identity.Name;
            var isExist = manager.IsExistAnnualInspectionAgent(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddVehicleAnnualInspectionAgent(model, user);
            return Json(new { Status = result, Msg = $"添加{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除年检代办配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="telNum"></param>
        /// <param name="carNoPrefix"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAgentConfig(string servicePid, string telNum, string carNoPrefix)
        {
            if (string.IsNullOrWhiteSpace(servicePid) || string.IsNullOrWhiteSpace(carNoPrefix) || string.IsNullOrWhiteSpace(telNum))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleAnnualInspectionAgentManager();
            var user = User.Identity.Name;
            var result = manager.DeleteVehicleAnnualInspectionAgent(servicePid, telNum, carNoPrefix, user);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 更新年检代办配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateAgentConfig(VehicleAnnualInspectionAgentModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ServicePid) || string.IsNullOrWhiteSpace(model.CarNoPrefix) || string.IsNullOrWhiteSpace(model.TelNum))
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleAnnualInspectionAgentManager();
            var isExist = manager.IsExistAnnualInspectionAgent(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复编辑" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.UpdateVehicleAnnualInspectionAgent(model, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取年检代办配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectAgentConfig(string servicePid, string carNoPrefix, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new VehicleAnnualInspectionAgentManager();
            var result = manager.SelectVehicleAnnualInspectionAgent(servicePid, carNoPrefix, pageIndex, pageSize);
            return Json(new { Status = result != null && result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        #region 查看日志
        /// <summary>
        /// 获取年检代办操作日志
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="telNum"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAnnualInspectionOprLog(string servicePid, string carNoPrefix, string telNum)
        {
            if (string.IsNullOrEmpty(servicePid) || string.IsNullOrEmpty(carNoPrefix) || string.IsNullOrEmpty(telNum))
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var logType = "VehicleAnnualInspectionAgent";
            var identityID = $"{carNoPrefix}_{servicePid}_{telNum}";
            var manager = new VehicleAnnualInspectionAgentManager();
            var result = manager.SelectAnnualInspectionOprLog(logType, identityID);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 刷新缓存
        /// <summary>
        /// 刷新年检代办服务缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshAnnualInspectionAgentCache()
        {
            var manager = new VehicleAnnualInspectionAgentManager();
            var result = manager.RefreshAnnualInspectionAgentCache();
            return Json(new { Status = result, Msg = $"刷新缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
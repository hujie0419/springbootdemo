using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CarFriendsGroup;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.DataAccess.Entity.CarFriendsGroup;
using Tuhu.Service.Vehicle;
using Tuhu.Service.Vehicle.Model;

namespace Tuhu.Provisioning.Controllers
{
    public class CarFriendsGroupController: Controller
    {
        #region 车友群

        /// <summary>
        /// 获取车友群列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult GetCarFriendsGroupList(int pageSize=20, int pageIndex=1)
        {
            var manager = new CarFriendsGroupManager();
            int recordCount = 0;
            var carFriendsGroupList=manager.GetCarFriendsGroupList(out recordCount, pageSize, pageIndex);
            return Json(new { data = carFriendsGroupList, count = recordCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新建车友群
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddCarFriendsGroup(CarFriendsGroupModel model)
        {
            var manager = new CarFriendsGroupManager();
            model.GroupCreateTime = DateTime.Now;
            model.GroupOverdueTime = model.GroupCreateTime.AddDays(7);
            model.CreateBy = User.Identity.Name;
            model.LastUpdateBy = User.Identity.Name;
            bool isSuccess = manager.AddCarFriendsGroup(model);
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "新建成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "新建失败！" });
            }
        }

        /// <summary>
        /// 编辑车友群
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateCarFriendsGroup(CarFriendsGroupModel model)
        {
            var manager = new CarFriendsGroupManager();
            model.GroupCreateTime = DateTime.Now;
            model.GroupOverdueTime = model.GroupCreateTime.AddDays(7);
            model.CreateBy = User.Identity.Name;
            model.LastUpdateBy = User.Identity.Name;
            bool isSuccess = manager.UpdateCarFriendsGroup(model);
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "编辑成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "编辑失败！" });
            }
        }

        /// <summary>
        /// 逻辑删除车友群
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public ActionResult DeleteCarFriendsGroup(int pkid)
        {
            var manager = new CarFriendsGroupManager();
            string lastUpdateBy = User.Identity.Name;
            bool isSuccess=manager.DeleteCarFriendsGroup(pkid, lastUpdateBy);
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "删除成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "删除失败！" });
            }
        }

        /// <summary>
        /// 获取所有品牌
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllBrand()
        {
            using(var client=new VehicleClient())
            {
                var queryParam = new VehicleQueryCategoryParam();
                var result = client.GetVehicleInfoList(queryParam, VehicleQueryCategoryEnum.None);
                result.ThrowIfException(true);
                return Json(new { data = result.Result.ToList()}, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据品牌获取二级车型
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public ActionResult GetAllVehicleByBrandName(string brand)
        {
            List<VehicleBrand> result = null;
            var msg = string.Empty;

            var getResult = VehicleService.GetVehicleInfoList(new VehicleQueryCategoryParam { Brand = brand }, VehicleQueryCategoryEnum.ByBrand);
            if (getResult != null && getResult.Any())
            {
                var vehicleList = getResult.OrderBy(x => x.Vehicle).ToList();
                result = new List<VehicleBrand>();
                foreach (var item in vehicleList)
                {
                    item.Vehicle = item.Vehicle.Split('-')[0];
                    if (result.Where(_ => String.Equals(_.Vehicle, item.Vehicle)).Count() <= 0)
                    {
                        result.Add(item);
                    }
                }
            }
            else
            {
                msg = "未查询到二级车型数据";
            }

            return Json(new { msg = msg, data = result ?? new List<VehicleBrand>() }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 途虎管理员

        /// <summary>
        /// 获取途虎管理员列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult GetCarFriendsAdministratorList(int pageSize=20, int pageIndex = 1)
        {
            var manager = new CarFriendsGroupManager();
            int recordCount = 0;
            var carFriendsAdministratorList = manager.GetCarFriendsAdministratorList(out recordCount, pageSize, pageIndex);
            return Json(new { data = carFriendsAdministratorList, count = recordCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增途虎管理员
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddCarFriendsAdministrator(CarFriendsAdministratorsModel model)
        {
            var manager = new CarFriendsGroupManager();
            model.CreateBy = User.Identity.Name;
            model.LastUpdateBy = User.Identity.Name;
            bool isSuccess = manager.AddCarFriendsAdministrator(model);
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "新建成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "新建失败！" });
            }
        }

        /// <summary>
        /// 编辑途虎管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateCarFriendsAdministrator(CarFriendsAdministratorsModel model)
        {
            var manager = new CarFriendsGroupManager();
            model.CreateBy = User.Identity.Name;
            model.LastUpdateBy = User.Identity.Name;
            bool isSuccess = manager.UpdateCarFriendsAdministrator(model);
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "编辑成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "编辑失败！" });
            }
        }

        /// <summary>
        /// 逻辑删除途虎管理员信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public ActionResult DeleteCarFriendsAdministrator(int pkid)
        {
            var manager = new CarFriendsGroupManager();
            string lastUpdateBy = User.Identity.Name;
            bool isSuccess = manager.DeleteCarFriendsAdministrator(pkid, lastUpdateBy);
            if (isSuccess)
            {
                return Json(new { status = isSuccess, msg = "删除成功！" });
            }
            else
            {
                return Json(new { status = isSuccess, msg = "删除失败！" });
            }
        }
        #endregion


    }
}

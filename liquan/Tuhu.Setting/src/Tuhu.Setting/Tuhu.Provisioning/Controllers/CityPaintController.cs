using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business.CityPaint;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using swc = System.Web.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Tuhu.Provisioning.Business.ProductVehicleType;
using System.Text;
using System.Data;
using Tuhu.Provisioning.Business.Promotion;
using System.Threading.Tasks;
using Tuhu.Service.Product.Models.New;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class CityPaintController : Controller
    {
        public ActionResult MultiCityPaintList()
        {
            IEnumerable<PaintInfoModel> countyList = CityPaintManager.SelectCountryPaintList();
            IEnumerable<PaintInfoModel> cityList = CityPaintManager.SelectCityPaintList();
            MultiCityPaintModel multiCityModel = new MultiCityPaintModel();
            multiCityModel.CountryPaintList = countyList;
            multiCityModel.CityPaintList = cityList;
            return View(multiCityModel);
        }
        public ActionResult CountryPaintList()
        {
            IEnumerable<PaintInfoModel> list = CityPaintManager.SelectCountryPaintList();
            return View(list);
        }

        public ActionResult CountryPaintItem(string pkid = null)
        {
            List<SelectListItem> lsSelItem = new List<SelectListItem>();
            IEnumerable<PaintModel> list = CityPaintManager.SelectPaintList();
            if (list != null && list.Any())
            {
                foreach (var paint in list)
                {
                    SelectListItem sel = new SelectListItem();
                    sel = new SelectListItem();
                    sel.Value = paint.PID.ToString();
                    sel.Text = paint.DisplayName;
                    lsSelItem.Add(sel);
                }
            }
            List<SelectListItem> listItem = new List<SelectListItem>();

            PaintInfoModel model = new PaintInfoModel();
            if (pkid != null)
            {
                model = CityPaintManager.GetCountryPaintByPKID(Int32.Parse(pkid));
                if (model.PID != null)
                {

                    foreach (var ss in lsSelItem)
                    {
                        if (ss.Value == model.PID.ToString())
                        {
                            ss.Selected = true;
                        }
                        listItem.Add(ss);
                    }
                }
                else
                {
                    listItem = lsSelItem;
                }
            }
            else
            {
                listItem = lsSelItem;
            }
            SelectList ddlSelData = new SelectList(listItem.AsEnumerable(), "Value", "Text", "请选择");
            ViewData["ddlPaintTypeData"] = ddlSelData;

            return View("CountryPaintItem", model);
        }

        public ActionResult InsertCountryPaintItem(string pid, string displayName, decimal price)
        {
            var paintInfo = CityPaintManager.GetCountryPaintByPID(pid);
            if (paintInfo != null && !string.IsNullOrEmpty(paintInfo.DisplayName))
            {
                return Json(new { msg = "has" });
            }
            PaintInfoModel model = new PaintInfoModel();
            model.PID = pid;
            model.DisplayName = displayName;
            model.Price = price;
            model.IsCountry = true;
            bool result = CityPaintManager.InsertCountryPaintModelModel(model);
            if (!result)
            {
                return Json(new { msg = "fail" });
            }
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = 0;
            modelLog.ObjectType = "CountryPaint";
            modelLog.Operation = "添加油漆产品:" + model.DisplayName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "添加类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = "0" });
            return Json(new { msg = "success" });
        }
        public ActionResult UpdateCountryPaintItem(int pkid, string pid, string displayName, decimal price)
        {
            var paintInfo = CityPaintManager.GetCountryPaintByPID(pid);
            if (paintInfo != null && !string.IsNullOrEmpty(paintInfo.DisplayName))
            {
                if (pkid != paintInfo.PKID)
                {
                    return Json(new { msg = "has" });
                }
            }
            PaintInfoModel model = new PaintInfoModel();
            model.PKID = pkid;
            model.PID = pid;
            model.DisplayName = displayName;
            model.Price = price;
            model.IsCountry = true;
            bool result = CityPaintManager.UpdateCountryPaintModel(model);
            if (!result)
            {
                return Json(new { msg = "fail" });
            }
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = pkid;
            modelLog.ObjectType = "CountryPaint";
            modelLog.Operation = "修改油漆产品:" + model.DisplayName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            return Json(new { msg = "success" });
        }

        public ActionResult GetCityPaintByPIDAndCityId(string pid, int cityId, int pkid)
        {
            IEnumerable<PaintInfoModel> modelList = CityPaintManager.GetCityPaintByPIDAndCityId(pid, cityId, pkid);

            if (modelList != null && modelList.Any())
            {
                return Json(new { msg = "fail" });
            }
            return Json(new { msg = "success" });
        }

        public ActionResult SelectPaintCityListByPid(string pid, int pkid)
        {
            var citys = CityPaintManager.SelectPaintCityListByPid(pid, pkid);
            return Json(citys);
        }

        /// <summary>
        /// 删除油漆产品全国价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCountryPaintItem(int pkid)
        {
            bool result = CityPaintManager.DeleteCountryPaintModelByPkid(pkid);
            return Json(result);
        }

        public ActionResult CityPaintList()
        {
            IEnumerable<PaintInfoModel> list = CityPaintManager.SelectCityPaintList();
            return View(list);
        }

        public ActionResult CityPaintItem(string pkid = null)
        {
            List<SelectListItem> lsSelItem = new List<SelectListItem>();
            IEnumerable<PaintModel> list = CityPaintManager.SelectPaintList();
            if (list != null && list.Any())
            {
                foreach (var paint in list)
                {
                    SelectListItem sel = new SelectListItem();
                    sel = new SelectListItem();
                    sel.Value = paint.PID.ToString();
                    sel.Text = paint.DisplayName;
                    lsSelItem.Add(sel);
                }
            }
            List<SelectListItem> listItem = new List<SelectListItem>();

            PaintInfoModel model = new PaintInfoModel();
            if (pkid != null)
            {
                model = CityPaintManager.GetCityPaintByPKID(Int32.Parse(pkid));
                if (model.PID != null)
                {

                    foreach (var ss in lsSelItem)
                    {
                        if (ss.Value == model.PID.ToString())
                        {
                            ss.Selected = true;
                        }
                        listItem.Add(ss);
                    }
                }
                else
                {
                    listItem = lsSelItem;
                }
            }
            else
            {
                listItem = lsSelItem;
            }
            SelectList ddlSelData = new SelectList(listItem.AsEnumerable(), "Value", "Text", "请选择");
            ViewData["ddlPaintTypeData"] = ddlSelData;
            #region 获取所有城市
            var cityList = CityPaintManager.SelectAllCityRegionList().ToList();

            if (pkid != null)
            {
                var selectedCityList = CityPaintManager.SelectPaintCityList(Convert.ToInt32(pkid)).ToList();
                foreach (var city in cityList)
                {
                    var cl = selectedCityList.Where(x => x.CityId == city.CityID);
                    if (cl.Any())
                    {
                        city.Selected = true;
                    }
                }
            }

            model.CityList = cityList;

            #endregion
            return View("CityPaintItem", model);
        }

        public ActionResult InsertCityPaintItem(string pid, string displayName, decimal price, string pkids)
        {
            PaintInfoModel model = new PaintInfoModel();
            model.PID = pid;
            model.DisplayName = displayName;
            model.Price = price;
            model.IsCountry = false;
            int pkid = CityPaintManager.InsertCityPaintModelModel(model);
            if (pkid < 1)
            {
                return Json(new { msg = "fail" });
            }
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = 0;
            modelLog.ObjectType = "CityPaint";
            modelLog.Operation = "添加油漆产品:" + model.DisplayName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            var citys = CityPaintManager.SelectCityRegionList(pkids);
            List<CityPaintModel> cityPaintList = new List<CityPaintModel>();
            if (citys != null && citys.Any())
            {
                foreach (var city in citys)
                {
                    CityPaintModel mo = new CityPaintModel();
                    mo.PaintId = pkid;
                    mo.CityId = city.CityID;
                    mo.ProvinceId = city.ProvinceID;
                    mo.CityName = city.City;
                    mo.CreatedTime = DateTime.Now;
                    mo.UpdatedTime = DateTime.Now;
                    cityPaintList.Add(mo);
                }

            }
            List<CityPaintModel> existList = new List<CityPaintModel>();
            //获取所有待更新数据
            var dt = GetDtForUpdate(cityPaintList, existList);
            bool result = CityPaintManager.BulkSaveCityPaint(dt);
            if (!result)
            {
                return Json(new { msg = "油漆配置城市添加失败" });
            }

            return Json(new { msg = "success" });
        }
        public ActionResult UpdateCityPaintItem(int pkid, string pid, string displayName, decimal price, string pkids)
        {
            PaintInfoModel model = new PaintInfoModel();
            model.PKID = pkid;
            model.PID = pid;
            model.DisplayName = displayName;
            model.Price = price;
            model.IsCountry = false;
            bool result = CityPaintManager.UpdateCityPaintModel(model);
            if (!result)
            {
                return Json(new { msg = "fail" });
            }
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = pkid;
            modelLog.ObjectType = "CityPaint";
            modelLog.Operation = "修改油漆产品:" + model.DisplayName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            var citys = CityPaintManager.SelectCityRegionList(pkids);
            List<CityPaintModel> cityPaintList = new List<CityPaintModel>();
            if (citys != null && citys.Any())
            {
                foreach (var city in citys)
                {
                    CityPaintModel mo = new CityPaintModel();
                    mo.PaintId = pkid;
                    mo.CityId = city.CityID;
                    mo.ProvinceId = city.ProvinceID;
                    mo.CityName = city.City;
                    mo.CreatedTime = DateTime.Now;
                    mo.UpdatedTime = DateTime.Now;
                    cityPaintList.Add(mo);
                }

            }
            List<CityPaintModel> existList = new List<CityPaintModel>();
            existList = CityPaintManager.SelectPaintCityList(pkid).ToList();
            //获取所有待更新数据
            var dt = GetDtForUpdate(cityPaintList, existList);
            bool allResult = CityPaintManager.BulkSaveCityPaint(dt);
            if (!allResult)
            {
                return Json(new { msg = "油漆配置城市添加失败" });
            }

            return Json(new { msg = "success" });
        }

        /// <summary>
        /// 删除油漆产品特殊价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCityPaintItem(int pkid)
        {
            bool result = CityPaintManager.DeleteCityPaintModelByPkid(pkid);
            return Json(result);
        }

        public DataTable GetDtForUpdate(List<CityPaintModel> cityPaintList, List<CityPaintModel> existCityList)
        {
            var dt = new DataTable();
            dt.Columns.Add("PKID", typeof(int));
            dt.Columns.Add("PaintId", typeof(int));
            dt.Columns.Add("CityId", typeof(int));
            dt.Columns.Add("ProvinceId", typeof(int));
            dt.Columns.Add("CityName", typeof(string));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("UpdatedTime", typeof(DateTime));

            var destList = new List<CityPaintModel>();
            var insertList = new List<CityPaintModel>();
            foreach (var item in cityPaintList)
            {
                if (!string.IsNullOrEmpty(item.CityName) && item.CityId != 0)
                {
                    var exists = existCityList.Where(x => x.PaintId == item.PaintId && x.CityId == item.CityId && x.CityName == item.CityName);
                    if (exists != null && exists.Any())
                    {
                        destList.AddRange(existCityList.FindAll(i => i.PaintId == item.PaintId && i.CityId == item.CityId && i.CityName == item.CityName));
                    }
                    else
                    {
                        destList.Add(item);
                        insertList.Add(item);
                    }
                }

            }

            var distinctList = destList.Distinct(new ListDistinctForCityPaint()).ToList();//待更新数据去重防止相同的重复数据
            insertList = insertList.Distinct(new ListDistinctForCityPaint()).ToList();

            var deleteList = new List<CityPaintModel>();
            foreach (var entity in existCityList)
            {
                var existList =
                     distinctList.FindAll(
                         i =>
                             i.PaintId == entity.PaintId && i.CityId == entity.CityId &&
                             i.CityName == entity.CityName);
                if (existList.Count < 1)
                {
                    deleteList.Add(entity);
                }
            }

            //执行删除配置操作
            if (deleteList.Count > 0)
                CityPaintManager.DeleteCityPaintByList(deleteList);

            foreach (var itemp in insertList)
            {
                var dr = dt.NewRow();
                dr["PKID"] = 0;
                dr["PaintId"] = itemp.PaintId;
                dr["CityId"] = itemp.CityId;
                dr["ProvinceId"] = itemp.ProvinceId;
                dr["CityName"] = itemp.CityName;
                dr["CreatedTime"] = DateTime.Now;
                dr["UpdatedTime"] = DateTime.Now;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public class ListDistinctForCityPaint : IEqualityComparer<CityPaintModel>
        {
            public bool Equals(CityPaintModel x, CityPaintModel y)
            {
                if (x.PaintId == y.PaintId && x.CityId == y.CityId && x.CityName == y.CityName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(CityPaintModel obj)
            {
                return 0;
            }
        }
    }
}
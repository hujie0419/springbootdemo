using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class DiscountManageConfigController : Controller
    {
        private static SE_GiftManageConfigModel _CreateLogs = null;  //存放临时logs信息
        public ActionResult Index(GiftManageConfigRequest request)
        {
            string strWhere = "";
            request.Creater = request.Creater?.Trim('\t').Trim();
            request.Mender = request.Mender?.Trim('\t').Trim();
            if (!string.IsNullOrWhiteSpace(request.Name))
                strWhere += $" and Name like N'%{request.Name}%' ";
            if (!string.IsNullOrWhiteSpace(request.ValidTimeBegin) && !string.IsNullOrWhiteSpace(request.ValidTimeEnd))
                strWhere += $" and (ValidTimeBegin >= '{request.ValidTimeBegin}' and ValidTimeEnd <= '{request.ValidTimeEnd}') ";
            if (!string.IsNullOrWhiteSpace(request.Creater))
                strWhere += $" and Creater like N'%{request.Creater}%' ";
            if (!string.IsNullOrWhiteSpace(request.Mender))
                strWhere += $" and Mender like N'%{request.Mender}%' ";
            if (!string.IsNullOrWhiteSpace(request.productPid))
                strWhere += $" and P_PID like N'%{request.productPid}%' ";
            //if (!string.IsNullOrWhiteSpace(request.giftPid))
            //    strWhere += $" and GiftProducts like N'%{request.giftPid}%' ";
            //if (!string.IsNullOrWhiteSpace(request.giftName))
            //    strWhere += $" and GiftProducts like N'%{request.giftName}%' ";
            if (!string.IsNullOrWhiteSpace(request.channel))
                strWhere += $" and Channel like N'%{request.channel}%' ";
            if (request.state == 1 || request.state == 0)
                strWhere += $" and state ={request.state}";
            if (request.state == 3)
                strWhere += $"  and ValidTimeEnd <= getdate()";
            if (request.state == 4)
                strWhere += $"  and ValidTimeEnd > getdate() and ValidTimeBegin < getdate() and state=1";
            var data = SE_GiftManageConfigBLL.SelectPages(2,request.pageIndex, request.pageSize, strWhere);

            int totalRecords = (data != null && data.Any())
                ? data.FirstOrDefault().TotalCount
                : 0;
            ViewBag.totalRecords = totalRecords;
            ViewBag.totalPage = totalRecords % request.pageSize == 0
                ? totalRecords / request.pageSize
                : (totalRecords / request.pageSize) + 1;
            ViewBag.pageIndex = request.pageIndex;
            ViewBag.GiftManageConfigRequest = request;
            ViewBag.Status = request.state;
            return View(data);
        }

        public ActionResult OperateLogs(int id)
        {
            IEnumerable<SE_DictionaryConfigModel> list = SE_DictionaryConfigBLL.SelectPages(1, int.MaxValue, string.Format(" and ParentId = {0} ", id)) ?? new List<SE_DictionaryConfigModel>();
            return View(list);
        }
        public ActionResult Create(int id = 0)
        {
            SE_GiftManageConfigModel model = new SE_GiftManageConfigModel();
            var U_ChannelPayList = SE_GiftManageConfigBLL.GetU_ChannelPayList();

            if (id > 0)
            {
                model = SE_GiftManageConfigBLL.GetEntity(id);
                if (!string.IsNullOrWhiteSpace(model.Channel))
                {
                    U_ChannelPayList.ForEach(_ =>
                    {
                        var channels = model.Channel.Split(',');
                        foreach (var item in channels)
                        {
                            if (_.ChannelKey.Contains(item))
                                _.IsChecked = true;
                        }
                    });
                }
                _CreateLogs = model;
            }

            ViewBag.U_ChannelPayList = U_ChannelPayList;
            ViewBag.B_Categorys = SE_MDBeautyPartConfigController.InteriorCategorysTreeJson(model.B_Categorys);

            return View(model);
        }

        public ActionResult Save(SE_GiftManageConfigModel model)
        {
            model.ActivityType = 2;
               var dataSource = model;

            if (model.Id <= 0)
            {
                dataSource.Creater = User.Identity.Name;
                dataSource.CreateTime = DateTime.Now;
                DiscountManageConfigManage.Insert(dataSource, new SE_DictionaryConfigModel
                {
                    ParentId = dataSource.Id,
                    Key = "SE_GiftManageConfig",
                    Extend1 = _CreateLogs != null ? JsonConvert.SerializeObject(_CreateLogs) : "",
                    Extend2 = dataSource != null ? JsonConvert.SerializeObject(dataSource) : "",
                    Extend3 = dataSource.Mender,
                    Extend4 = "新增",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                });
                //Logs(dataSource, "新增");
            }
            else
            {
                dataSource.Mender = User.Identity.Name;
                dataSource.UpdateTime = DateTime.Now;
                if (DiscountManageConfigManage.Update(dataSource))
                    Logs(dataSource, "更新");
            }
            return RedirectToAction("Index");
        }
        private void Logs(SE_GiftManageConfigModel source, string type)
        {
            if (source != null)
            {
                var result = SE_DictionaryConfigBLL.Insert(new SE_DictionaryConfigModel
                {
                    ParentId = source.Id,
                    Key = "SE_GiftManageConfig",
                    Extend1 = _CreateLogs != null ? JsonConvert.SerializeObject(_CreateLogs) : "",
                    Extend2 = source != null ? JsonConvert.SerializeObject(source) : "",
                    Extend3 = source.Mender,
                    Extend4 = type,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                });

                //_CreateLogs = null;
            }
        }

        public ActionResult UploadExcelToPID()
        {
            try
            {
                if (Request.Files.Count == 1)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") || !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件" }, "text/html");
                    var excel = new Tuhu.Provisioning.Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("ImportPID", true);
                    return Json(new { Status = 1, Data = (dt != null && dt.Rows.Count > 0) ? JsonConvert.SerializeObject(dt) : "", Count = dt?.Rows.Count }, "text/html");
                }
                else
                    return Json(new { Status = -1, Error = "没有上传任何文件或者一次选择多个文件" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { Status = -1, Error = "请检查文件内容格式是否正确\r\n\r\n" + ex.ToString() }, "text/html");
            }
        }

        public string RefreshCache()
        {
            var _result = new { state = 0, msg = "" };
            try
            {
                using (var client = new Tuhu.Service.Product.ProductClient())
                {
                    var result = client.RefreshDiscountRuleCache();
                    if (result.Result)
                        _result = new { state = 1, msg = "" };
                    else
                        _result = new { state = 0, msg = result.ErrorMessage };
                }
            }
            catch (Exception ex)
            {
                _result = new { state = 0, msg = ex.ToString() };
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(_result);
        }
        public string SetDisabled()
        {
            var _result = new { state = 0, result = 0, msg = "" };
            var result = SE_GiftManageConfigBLL.SetDisabled();
            if (result >= 0)
            {
                using (var client = new Tuhu.Service.Product.CacheClient())
                {
                    var cacheresult = client.RefreshGiftCache();
                    if (cacheresult.Result)
                        _result = new { state = 1, result = result, msg = "" };
                    else
                        _result = new { state = 0, result = 0, msg = cacheresult.ErrorMessage };
                }
            }
            else
            {
                _result = new { state = 0, result = 0, msg = "操作失败请重试" };
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(_result);
        }

        public ActionResult BrandsListControl(string B_Categorys = "1", string B_Brands = "", int type = 0)
        {
            IEnumerable<FilterConditionModel> CP_BrandList = ProductLibraryConfigController.QueryProducts(new SeachProducts() { category = B_Categorys })?.CP_BrandList ?? new List<FilterConditionModel>();

            if (CP_BrandList != null && CP_BrandList.Any() && !string.IsNullOrWhiteSpace(B_Brands))
            {
                CP_BrandList.ForEach(a =>
                {
                    var brands = B_Brands.Split(',');
                    foreach (var item in brands)
                    {
                        if (a.Name.Contains(item))
                            a.Name = a.Name + ":True";
                    }
                });
            }

            var B_CategorysId = (B_Categorys ?? "").Contains(".") ? B_Categorys.Substring(B_Categorys.LastIndexOf(".") + 1) : B_Categorys;

            ViewBag.BrandsListControlCategorys = SE_MDBeautyPartConfigController.InteriorCategorysTreeJson(B_CategorysId);
            ViewBag.CP_BrandList = CP_BrandList;
            ViewBag.Type = type;

            return View();
        }
        /// <summary>
        /// 品牌checkbox列表
        /// </summary>
        public ActionResult BrandsCheckBoxListControl(string categorys = "1")
        {
            IEnumerable<FilterConditionModel> CP_BrandList = ProductLibraryConfigController.QueryProducts(new SeachProducts() { category = categorys })?.CP_BrandList ?? new List<FilterConditionModel>();
            return View(CP_BrandList);
        }
        /// <summary>
        ///  获取产品信息
        /// </summary>
        public ActionResult GetVW_ProductsModel(string pid)
        {
            var data = SE_GiftManageConfigBLL.GetVW_ProductsModel(pid) ?? new VW_ProductsModel() { oid = -1 };
            return Json(new { Status = data.oid, DisplayName = data.DisplayName, Pid = data.PID, money = data.CY_List_Price });
        }
    }
}
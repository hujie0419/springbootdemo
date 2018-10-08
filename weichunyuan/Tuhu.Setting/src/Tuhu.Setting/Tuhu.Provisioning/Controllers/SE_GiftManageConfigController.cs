using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Web.Security;
using System.Collections.Generic;

using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Business;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Request;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.OperationCategory;

namespace Tuhu.Provisioning.Controllers
{
    public class SE_GiftManageConfigController : Controller
    {
        private static SE_GiftManageConfigModel _CreateLogs = null; //存放临时logs信息

        //
        // GET: /SE_GiftManageConfig/

        public ActionResult Index(GiftManageConfigRequest request)
        {
            string strWhere = "";
            request.Creater = request.Creater?.Trim('\t').Trim();
            request.Mender = request.Mender?.Trim('\t').Trim();
            if (!string.IsNullOrWhiteSpace(request.Name))
                strWhere += $" and Name like N'%{request.Name}%' ";
            if (!string.IsNullOrWhiteSpace(request.ValidTimeBegin) && !string.IsNullOrWhiteSpace(request.ValidTimeEnd))
                strWhere +=
                    $" and (ValidTimeBegin >= '{request.ValidTimeBegin}' and ValidTimeEnd <= '{request.ValidTimeEnd}') ";
            if (!string.IsNullOrWhiteSpace(request.Creater))
                strWhere += $" and Creater like N'%{request.Creater}%' ";
            if (!string.IsNullOrWhiteSpace(request.Mender))
                strWhere += $" and Mender like N'%{request.Mender}%' ";
            if (!string.IsNullOrWhiteSpace(request.productPid))
                strWhere += $" and P_PID like N'%{request.productPid}%' ";
            if (!string.IsNullOrWhiteSpace(request.giftPid))
                strWhere += $" and GiftProducts like N'%{request.giftPid}%' ";
            if (!string.IsNullOrWhiteSpace(request.giftName))
                strWhere += $" and GiftProducts like N'%{request.giftName}%' ";
            if (!string.IsNullOrWhiteSpace(request.channel))
                strWhere += $" and Channel like N'%{request.channel}%' ";
            if (request.state == 1 || request.state == 0)
                strWhere += $" and state ={request.state}";
            if (request.state == 3)
                strWhere += $"  and ValidTimeEnd <= getdate()";
            if (request.state == 4)
                strWhere += $"  and ValidTimeEnd > getdate() and ValidTimeBegin < getdate() and state=1";
            if (!string.IsNullOrWhiteSpace(request.Group))
                strWhere += $" and [Group] like N'%{request.Group}%' ";
            if (request.IsPackage == 1 || request.IsPackage == 0)
                strWhere += $" and IsPackage ={request.IsPackage}";
            if (request.GiveAway == 1 || request.GiveAway == 0)
                strWhere += $" and GiveAway ={request.GiveAway}";
            var data = SE_GiftManageConfigBLL.SelectPages(1, request.pageIndex, request.pageSize, strWhere);

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
            ViewBag.IsPackage = request.IsPackage;
            ViewBag.GiveAway = request.GiveAway;
            ViewBag.pageSize = request.pageSize;
            return View(data);
        }

#if !DEBUG
        [PowerManage]
#endif
        ////
        // GET: /SE_GiftManageConfig/Create
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
        public ActionResult Edit(int id = 0)
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
            ViewBag.B_Categorys = InteriorCategorysTreeJson(model.B_Categorys);

            return View(model);
        }
        private static readonly OperationCategoryManager _CategoryTagManager = new OperationCategoryManager();
        /// <summary>
        /// 获取门店美容类目树
        /// </summary>
        public static string InteriorCategorysTreeJson(string opens = "")
        {
            opens = opens ?? "";
            var opensArr = opens?.Split(',');
            var ZTreeModel = _CategoryTagManager.SelectProductCategories()?.Select(m => new
            {
                id = m.id,
                pId = m.pId,
                name = m.name,
                open = opensArr.Contains(m.id.ToString().Trim()),
                @checked = opensArr.Contains(m.id.ToString().Trim()),
                chkDisabled = 1,
                NodeNo = m.NodeNo
            });
            return JsonConvert.SerializeObject(ZTreeModel);
        }
        public ActionResult BrandsListControl(string B_Categorys = "1", string B_Brands = "", int type = 0)
        {
            IEnumerable<FilterConditionModel> CP_BrandList =
                ProductLibraryConfigController.QueryProducts(new SeachProducts() { Category = B_Categorys })?.CP_BrandList ??
                new List<FilterConditionModel>();

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

            var B_CategorysId = (B_Categorys ?? "").Contains(".")
                ? B_Categorys.Substring(B_Categorys.LastIndexOf(".") + 1)
                : B_Categorys;

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
            IEnumerable<FilterConditionModel> CP_BrandList =
                ProductLibraryConfigController.QueryProducts(new SeachProducts() { Category = categorys })?.CP_BrandList ??
                new List<FilterConditionModel>();
            return View(CP_BrandList);
        }

        public ActionResult UploadExcelToPID()
        {
            try
            {
                if (Request.Files.Count == 1)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件" }, "text/html");
                    var excel = new Tuhu.Provisioning.Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("ImportPID", true);
                    return
                        Json(
                            new
                            {
                                Status = 1,
                                Data = (dt != null && dt.Rows.Count > 0) ? JsonConvert.SerializeObject(dt) : "",
                                Count = dt?.Rows.Count
                            }, "text/html");
                }
                else
                    return Json(new { Status = -1, Error = "没有上传任何文件或者一次选择多个文件" }, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new { Status = -1, Error = "请检查文件内容格式是否正确\r\n\r\n" + ex.ToString() }, "text/html");
            }
        }

        /// <summary>
        /// 检测PID是否存在
        /// </summary>
        public ActionResult CheckPID(string pid)
        {
            return Json(new { Status = SE_GiftManageConfigBLL.CheckPID(pid) ? 1 : 0 });
        }

        /// <summary>
        ///  获取产品信息
        /// </summary>
        public ActionResult GetVW_ProductsModel(string pid)
        {
            var data = SE_GiftManageConfigBLL.GetVW_ProductsModel(pid) ?? new VW_ProductsModel() { oid = -1 };
            return
                Json(new { Status = data.oid, DisplayName = data.DisplayName, Pid = data.PID, money = data.CY_List_Price });
        }

        public ActionResult CopySave(SE_GiftManageConfigModel model)
        {
            model.Id = 0;
            return Save(model);
        }
        public ActionResult Save(SE_GiftManageConfigModel model)
        {
            model.ActivityType = 1;
            var dataSource = model;
            if (model.Type == 4)
            {
                var flag = false;
                var pPids = JsonConvert.DeserializeObject<List<PidModel>>(model.P_PID);
                var result = SE_GiftManageConfigBLL.GetProductsModel(pPids.Select(r => r.Pid).ToList());
                foreach (var value in result.Values)
                {
                    if (value.Contains("BaoYang"))
                    {
                        flag = true;
                        break;
                    }

                }
                if (flag)
                {
                    model.Category = "BaoYang";
                }
            }
            if (model.Type == 3)
            {
                var pNodes = new List<string>();
                var list = SE_GiftManageConfigBLL.GetByAllNodes();
                foreach (var item in list)
                {
                    pNodes.AddRange(item.Split('.').ToList());
                }
                var distinctNodes = pNodes.Distinct().ToList();
                var categorys = model.B_Categorys?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ??
                                new string[0];
                var bBrands = model.B_Brands == null
                    ? new List<BrandModel>()
                    : JsonConvert.DeserializeObject<List<BrandModel>>(model.B_Brands);
                var isBrand = false;
                //var isCategory = categorys.Contains("28656");
                var isCategory = distinctNodes.Any() && distinctNodes.Any(category => categorys.Contains(category));
                foreach (var brand in bBrands)
                {
                    isBrand = distinctNodes.Any() && distinctNodes.Contains(brand.Category);
                    if (isBrand)
                        break;
                }
                var isbPid = false;
                if (model.B_PID_Type)
                {
                    var pPids = model.B_PID == null
                        ? new List<PidModel>()
                        : JsonConvert.DeserializeObject<List<PidModel>>(model.B_PID);
                    var result = SE_GiftManageConfigBLL.GetProductsModel(pPids.Select(r => r.Pid).ToList());
                    foreach (var value in result.Values)
                    {
                        if (value.Contains("BaoYang"))
                        {
                            isbPid = true;
                            break;
                        }
                    }
                }
                if (isBrand || isCategory || isbPid)
                {
                    model.Category = "BaoYang";
                }
            }
            //var giftProductModels = JsonConvert.DeserializeObject<List<GiftProductModel>>(dataSource.GiftProducts);
            //giftProductModels.ForEach(g =>
            //{
            //    //if (g.Stock.HasValue)
            //    //{
            //    var stock = SE_GiftManageConfigBLL.GetGiftProductStock(g.Pid);
            //    if (!stock.HasValue)
            //    {
            //        if (g.Stock.HasValue)
            //            SE_GiftManageConfigBLL.InsertGiftProductStock(g.Pid, g.Stock.Value);
            //    }
            //    else
            //    {
            //        if (g.Stock.HasValue)
            //            SE_GiftManageConfigBLL.UpdateGiftProductStock(g.Pid, g.Stock.Value);
            //        else
            //            SE_GiftManageConfigBLL.DeleteGiftProductStock(g.Pid);
            //    }
            //    // }
            //});
            if (model.Id <= 0)
            {
                dataSource.Creater = User.Identity.Name;
                dataSource.CreateTime = DateTime.Now;
                SE_GiftManageConfigBLL.Insert(dataSource, new SE_DictionaryConfigModel
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
                if (SE_GiftManageConfigBLL.Update(dataSource))
                    Logs(dataSource, "更新");
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 保存时判断PID是否已经属于其他进行中的活动中
        /// 只判断 Type=4（按PID）的类型
        /// </summary>
        /// <param name="pPid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CheckGiveAway(string pPid,int id = 0)
        {
            // 取客户端所有PID集合
            // 取数据库所有进行中的开启买三送一属性的PID集合
            // 新增时取出客户端传入的PID在总的PID中的交集
            // 修改时从总的PID中移出当前存在的PID再取交集

            // 前端传入所有pid集合
            var pPids = JsonConvert.DeserializeObject<List<PidModel>>(pPid);

            // 获取所有买三送一的赠品规则
            var giveAwayList = SE_GiftManageConfigBLL.SelectGiveAwayList(id);

            var existsPids = (from giveAway in giveAwayList
                              from pid in pPids
                              let curCfgPid = JsonConvert.DeserializeObject<List<PidModel>>(giveAway.P_PID).ToList()
                              where curCfgPid.Select(t => t.Pid == pid.Pid).Any()
                              select pid).Distinct().ToList();

            return Json(new { result = JsonConvert.SerializeObject(existsPids) });
        }

        // GET: /SE_GiftManageConfig/Delete/5
        public ActionResult Delete(int id)
        {
            if (id > 0)
            {
                var source = SE_GiftManageConfigBLL.GetEntity(id);
                if (SE_GiftManageConfigBLL.Delete(id))
                {
                    Logs(source, "删除");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult OperateLogs(int id)
        {
            IEnumerable<SE_DictionaryConfigModel> list =
                SE_DictionaryConfigBLL.SelectPages(1, int.MaxValue, string.Format(" and ParentId = {0} ", id)) ??
                new List<SE_DictionaryConfigModel>();
            return View(list);
        }

        public ActionResult GiftDetailsIndex()
        {
            ViewBag.U_ChannelPayList = SE_GiftManageConfigBLL.GetU_ChannelPayList();
            return View("GiftDetails");
        }

        [HttpPost]
        public ActionResult GiftDetails(MatchGiftsRequest request, bool? isOrder)
        {
            var gifts = SE_GiftManageConfigBLL.SelectMatchGiftsResponse(request, isOrder ?? true).ToList();
            if (gifts.Any())
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(@"<div><p>");
                sb.Append(@"</p><table class='table table - bordered table - striped'> <tr>
                                        <td>PID</td>
                                        <td>赠品名称</td>
                                        <td>数量</td>
                                        <td>是否必须</td>
                                        <td>赠送类型</td>
                                        <td>赠品描述信息</td>

                                    </tr>");
                foreach (var g in gifts)
                {

                    sb.Append("<tr><td>");
                    sb.Append(g.Pid);
                    sb.Append("</td><td>");
                    sb.Append(g.ProductName);
                    sb.Append("</td><td>");
                    sb.Append(g.Quantity);
                    sb.Append("</td><td>");
                    sb.Append(g.Require);
                    sb.Append("</td><td>");
                    sb.Append(g.GiftsType);
                    sb.Append("</td><td>");
                    sb.Append(g.GiftDescription);
                    sb.Append("</td><tr>");
                }
                sb.Append("</table></div>");

                return Json(new { Status = 1, Html = sb.ToString() });
            }
            else
            {
                StringBuilder sb2 = new StringBuilder();
                sb2.Append(@"<p style='font - weight: bold; color: blue; margin: 300px;font-size: large'>没有赠品</p>");
                return Json(new { Status = 0, Html = sb2.ToString() });
            }

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

        public string RefreshCache()
        {
            var _result = new { state = 0, msg = "" };
            try
            {
                using (var client = new Tuhu.Service.Product.CacheClient())
                {
                    var result = client.RefreshGiftCache();
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


        /// <summary>
        ///  获取库存信息
        /// </summary>
        public ActionResult GetGiftProductStock(string pid)
        {
            var data = SE_GiftManageConfigBLL.GetGiftProductStock(pid);
            if (!data.HasValue)
            {
                return Json(new { state = -1 });
            }
            return Json(new { state = 1, stock = data });
        }
        /// <summary>
        ///  获取赠品送出数量
        /// </summary>
        public ActionResult GetGiftProductSendNum(string pid, int ruleId)
        {
            var data = SE_GiftManageConfigBLL.GetGiftProductSendNum(pid, ruleId);
            if (!data.HasValue)
            {
                return Json(new { state = -1 });
            }
            return Json(new { state = 1, sendNum = data });
        }
        public JsonResult CreateMultistage(string value)
        {
            var models = JsonConvert.DeserializeObject<List<GiftLevelModel>>(value);
            var group = Membership.GeneratePassword(10, 1);
            var result = 0;
            foreach (var model in models)
            {
                result = SE_GiftManageConfigBLL.UpdateGiftLeveL(model.Pkid, model.Sort, group);
                if (result == 0)
                    return Json(new { state = result });
            }
            var groups = SE_GiftManageConfigBLL.GetGiftLeveLs();
            var groupKeys = groups.GroupBy(r => r.Group).Where(r => r.Count() == 1).Select(r => r.Key);
            if (groupKeys.Any())
            {
                SE_GiftManageConfigBLL.DeleteGiftLeveLs(groupKeys.ToList());
            }
            return Json(new { state = result });
        }

        [HttpPost]
        public ActionResult GetGiftModel(int id)
        {
            var source = SE_GiftManageConfigBLL.GetEntity(id);
            return Json(new { Status = 1, Name = source.Name });
        }

        public ActionResult UpdateGiftLevel(string group)
        {
            var result = (SE_GiftManageConfigBLL.GetGiftLeveL(group)).ToList();
            if (!result.Any())
                return Json(new { Status = 0, Message = "不存跟其他规则的互斥关系" });
            else
            {
                return Json(new { Status = 1, Data = result });
            }
        }

        public ViewResult SearchGiftLevel()
        {
            var groups = SE_GiftManageConfigBLL.GetGiftLeveLs();
            ViewBag.pageIndex = 1;
            ViewBag.totalRecords = groups.Count();
            ViewBag.totalPage = Math.Ceiling(Convert.ToDouble(groups.Count()) / 1000);
            ViewBag.GiftManageConfigRequest = new GiftManageConfigRequest();

            return View("Index", groups);
        }

        public ViewResult GiftStockIndex(string pid = null)
        {
            var model = new GiftStockModel();
            var count = SE_GiftManageConfigBLL.SelectGiftProductSaleOutCount(pid);
            var stock = SE_GiftManageConfigBLL.SelectGiftStock(pid);
            model.Stock = stock;
            model.SaleCount = count;
            model.Pid = pid;
            return View(model);
        }

        public ViewResult Copy(int id)
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


        public FileResult Export(int id)
        {
            var mResult = new List<ExportPidModel>();
            var sResult = SE_GiftManageConfigBLL.GetPids(id);
            if (sResult != null)
            {
                mResult = JsonConvert.DeserializeObject<List<ExportPidModel>>(sResult);
            }
            var db = ToDataTable(mResult);
            var fileName = "PID导出模板" + DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".xls";
            var ms = DataTableToExcel2(db, fileName, "PID", true);
            return File(ms, "application/vnd.ms-excel", fileName);
            // var mResult = new List<ExportPidModel>();
            // var sResult = SE_GiftManageConfigBLL.GetPids(id);
            // if (sResult != null)
            // {
            //     mResult = JsonConvert.DeserializeObject<List<ExportPidModel>>(sResult);
            // }
            // var db = ToDataTable(mResult);
            // var route = $"{disk}:\\PID导入模板.xlsx";
            //var result= DataTableToExcel(db, $"{disk}:\\PID导入模板.xlsx", "PID导出模板", true);
            // if (result > 0)
            //return Json(new { Status = 1, disk = disk, result = "fdf" });
            // else
            // {
            //     return Json(new { Status = 0,  msg = "下载失败" });
            // }
        }

        /// <summary>
        /// 批量修改开始结束时间
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchUpdateTime(string content)
        {
            try
            {
                var modules = JsonConvert.DeserializeObject<IEnumerable<SE_GiftManageConfigModel>>(content);

                return Json(SE_GiftManageConfigBLL.BatchUpdateTime(modules) ? 1 : 0);
            }
            catch (Exception em)
            {
                throw new Exception(em.Message);
            }
        }

        #region excel导出相关
        private static int DataTableToExcel(DataTable data, string fileName, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;
            IWorkbook workbook = null;


            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    if (workbook != null)
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }

                    if (isColumnWritten == true) //写入DataTable的列名
                    {
                        IRow row = sheet.CreateRow(0);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }

                    for (i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }
                    workbook.Write(fs); //写入到excel
                    return count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }

        }


        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }


        private DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        private static MemoryStream DataTableToExcel2(DataTable data, string fileName, string sheetName, bool isColumnWritten)
        {
            int i;
            int j;
            int count;
            ISheet sheet = null;
            IWorkbook workbook = null;
            if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0) // 2003版本
                workbook = new HSSFWorkbook();
            //添加一个sheet
            if (workbook != null)
            {
                sheet = workbook.CreateSheet(sheetName);

            }
            if (isColumnWritten) //写入DataTable的列名
            {
                if (sheet != null)
                {
                    var row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                }
                count = 1;
            }
            else
            {
                count = 0;
            }
            for (i = 0; i < data.Rows.Count; ++i)
            {
                if (sheet != null)
                {
                    var row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                }
                ++count;
            }
            MemoryStream ms = new MemoryStream();
            workbook?.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

    }
    #endregion
}
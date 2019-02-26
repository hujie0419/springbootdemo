using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Service.Shop;

namespace Tuhu.Provisioning.Controllers
{
    public class MDBeautyExitConfigController : Controller
    {
        private static readonly Lazy<MDBeautyApplyConfigManager> lazy = new Lazy<MDBeautyApplyConfigManager>();

        private MDBeautyApplyConfigManager MDBeautyApplyConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<MeiRongAcitivityConfigManager> lazy1 = new Lazy<MeiRongAcitivityConfigManager>();

        private MeiRongAcitivityConfigManager MeiRongAcitivityConfigManager
        {
            get
            {
                return lazy1.Value;
            }
        }
        public ActionResult Index()
        {
            ViewBag.Prod = SE_MDBeautyCategoryConfigBLL.SelectList().Where(x => x.ParentId != 0).ToList();
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);
            return View();
        }

        public ActionResult List(MDBeautyApplyConfig model, int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;

            var lists = MDBeautyApplyConfigManager.GetMDBeautyApplyConfigList(model, pageSize, pageIndex, "Exit", out count);

            var list = new OutData<List<MDBeautyApplyConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<MDBeautyApplyConfig>(list.ReturnValue, pager));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AuditMDBeautyApplyConfig(MDBeautyApplyConfig model, string type)
        {
            try
            {
                if (MDBeautyApplyConfigManager.AuditMDBeautyApplyConfig(model, type))
                {
                    CleanCache(model.ShopId, model.ProductId);
                    if (type == "Apply")
                    {
                        AddOprLog(model.Id, model.ApplyAuditStatus.ToString(), "", "Apply", "审核申请报名数据");
                    }
                    else
                    {
                        AddOprLog(model.Id, model.ExitAuditStatus.ToString(), "", "Exit", "审核退出申请数据");
                    }

                }

                return Json(true);

            }
            catch
            {
                return Json(false);
                throw;
            }


        }
        public ActionResult Export(MDBeautyApplyConfig model)
        {
            using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/门店美容活动退出审核列表.xlsx"))))
            {
                int count = 0;
                var list = MDBeautyApplyConfigManager.GetMDBeautyApplyConfigList(model, int.MaxValue, 1, "Exit", out count);
                var xssfWorkbook = new XSSFWorkbook(stream); //创建Workbook对象  2007+   
                                                             // var hssfWorkbook = new HSSFWorkbook(stream)://2003
                if (list.Count > 0)
                {
                    var i = 0;
                    var sheet = xssfWorkbook.GetSheetAt(0);

                    foreach (var item in list)
                    {
                        MeiRongAcitivityConfigManager manager = new MeiRongAcitivityConfigManager();

                        model.RegionList = manager.GetRegionRelation(item.Id, 1);
                        string region = string.Empty;
                        if (model.RegionList != null && model.RegionList.Count > 0)
                        {
                            foreach (var regionItem in model.RegionList)
                            {
                                region += regionItem.ProvinceName + "：" + regionItem.CityName + "；";
                            }
                        }
                        string apply = string.Empty;
                        switch (item.ApplyAuditStatus)
                        {
                            case 1:
                                apply = "待审核";
                                break;
                            case 2:
                                apply = "不通过";
                                break;
                            case 3:
                                apply = "通过";
                                break;
                            default:
                                break;
                        }
                        string exit = string.Empty;
                        switch (item.ExitAuditStatus)
                        {
                            case 1:
                                exit = "待审核";
                                break;
                            case 2:
                                exit = "不通过";
                                break;
                            case 3:
                                exit = "通过";
                                break;
                            default:
                                break;
                        }
                        var row = sheet.CreateRow((i++) + 1);
                        row.CreateCell(0).SetCellValue(item.BeautyAcitivityId);
                        row.CreateCell(1).SetCellValue(item.Name);
                        row.CreateCell(2).SetCellValue(region);
                        row.CreateCell(3).SetCellValue(item.CategoryName);
                        row.CreateCell(4).SetCellValue(item.MinShopQuantity);
                        row.CreateCell(5).SetCellValue(item.SignUpStartTime?.ToString() + "—" + item.SignUpEndTime?.ToString());
                        row.CreateCell(6).SetCellValue(item.PlanStartTime?.ToString() + "—" + item.ActivityEndTime?.ToString());
                        row.CreateCell(7).SetCellValue(item.MinPrice + "—" + item.MaxPrice);
                        row.CreateCell(8).SetCellValue(item.Status == 1 ? "启用" : "禁用");
                        row.CreateCell(9).SetCellValue(item.ShopId);
                        row.CreateCell(10).SetCellValue(item.ShopName);
                        row.CreateCell(11).SetCellValue(item.ProductId);
                        row.CreateCell(12).SetCellValue(item.ProductName);
                        row.CreateCell(13).SetCellValue(apply);
                        row.CreateCell(13).SetCellValue(exit);
                    }
                    Response.AppendHeader("Content-Disposition", "attachment;fileName=门店美容活动退出审核列表" + ".xlsx");
                    xssfWorkbook.Write(Response.OutputStream);
                    Response.End();
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int status, string type, string look, int id = 0)
        {
            MDBeautyApplyConfig model = MDBeautyApplyConfigManager.GetMDBeautyApplyConfigById(id);
            ViewData["status"] = status;
            ViewData["type"] = type;
            ViewData["look"] = look;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditList(string model, string type)
        {
            List<MDBeautyApplyConfig> list = JsonConvert.DeserializeObject<List<MDBeautyApplyConfig>>(model);
            try
            {
                foreach (var item in list)
                {
                    MDBeautyApplyConfigManager.AuditMDBeautyApplyConfig(item, type);

                    CleanCache(item.ShopId, item.ProductId);
                    if (type == "Apply")
                    {
                        AddOprLog(item.Id, item.ApplyAuditStatus.ToString(), "", "Apply", "审核申请报名数据");
                    }
                    else
                    {
                        AddOprLog(item.Id, item.ExitAuditStatus.ToString(), "", "Exit", "审核退出申请数据");
                    }

                }
                return Json(true);
            }
            catch
            {

                return Json(false);
            }
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(MDBeautyApplyConfigManager.DeleteMDBeautyApplyConfig(id));
        }
        public void AddOprLog(int id, string after, string before, string type, string opr)
        {
            OprLogManager OprLogManager = new OprLogManager();
            OprLog oprModel = new OprLog();
            oprModel.AfterValue = after;
            oprModel.BeforeValue = before;
            oprModel.Author = User.Identity.Name;
            oprModel.ChangeDatetime = DateTime.Now;
            oprModel.HostName = Request.UserHostName;
            oprModel.ObjectID = id;
            oprModel.ObjectType = type;
            oprModel.Operation = opr;
            OprLogManager.AddOprLog(oprModel);
        }

        public bool CleanCache(int shopId, int pid)
        {
            try
            {
                using (var client = new ShopClient())
                {
                    var result = client.SendBeautyShopChangeQueue(shopId, pid);
                    return result.Success;
                }             
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
    }
}

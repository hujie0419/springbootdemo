using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.ProductVehicleType;
using Tuhu.Provisioning.Business.Upload;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service;
using Tuhu.Service.Product;
using Tuhu.Service.Utility.Request;
using swc = System.Web.Configuration;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductVehicleTypeController : Controller
    {
        private readonly IProductVehicleInfoMgr ipvimgr = new ProductVehicleInfoMgr();
        private static readonly ILog logger = LoggerFactory.GetLogger("ProductVehicleTypeController");

        /// <summary>
        /// 搜索列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NoImportProduct()
        {
            return View();
        }
        /// <summary>
        /// 产品通用信息页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult Detail(string id)
        {
            var model = ipvimgr.GetProductInfoByPid(id);

            switch (model.CP_Remark)
            {
                case "=请选择=":
                    model.CP_Remark = ""; break;
                case "无需车型":
                    model.CP_Remark = "0"; break;
                case "二级车型":
                    model.CP_Remark = "2"; break;
                //case "四级车型":
                //    model.CP_Remark = "4"; break;
                case "五级车型":
                    model.CP_Remark = "5"; break;
                default:
                    model.CP_Remark = ""; break;
            }
            switch (model.VehicleLevel)
            {
                case "=请选择=":
                    model.VehicleLevel = ""; break;
                case "无需车型":
                    model.VehicleLevel = "0"; break;
                case "二级车型":
                    model.VehicleLevel = "2"; break;
                //case "四级车型":
                //    model.VehicleLevel = "4"; break;
                case "五级车型":
                    model.VehicleLevel = "5"; break;
                default:
                    model.VehicleLevel = ""; break;
            }

            List<SelectListItem> select1 = new List<SelectListItem>()
            {
                new SelectListItem() { Text="=请选择=",Value=""},
                new SelectListItem() { Text="无需车型",Value="0"},
                new SelectListItem() { Text="二级车型",Value="2"},
                //new SelectListItem() { Text="四级车型",Value="4"},
                new SelectListItem() { Text="五级车型",Value="5"},
            };

            foreach (var item in select1)
            {
                if (item.Value == model.CP_Remark)
                {
                    item.Selected = true;
                }
            }
            List<SelectListItem> select2 = new List<SelectListItem>()
            {
                new SelectListItem() { Text="=请选择=",Value=""},
                new SelectListItem() { Text="无需车型",Value="0"},
                new SelectListItem() { Text="二级车型",Value="2"},
                //new SelectListItem() { Text="四级车型",Value="4"},
                new SelectListItem() { Text="五级车型",Value="5"},
            };

            foreach (var item in select2)
            {
                if (item.Value == model.VehicleLevel)
                {
                    item.Selected = true;
                }
            }
            //ViewData["select1"] = new SelectList(select1, "Value", "Text", "无需车型");
            ViewBag.Items = select1;
            ViewBag.Items2 = select2;
            ViewBag.IsAutomatic = model.IsAutoAssociate;
            return View(model);
        }

        /// <summary>
        /// 导入导出页
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult FileInportExport(string id, string msg)
        {
            ViewBag.Pid = id;
            ViewBag.error = msg;
            var model = ipvimgr.GetProductInfoByPid(id);
            var list = ipvimgr.GetExcelInfoByPid(id);
            if (list.Count >= 3)
            {
                model.ExcelInfoList = list.OrderByDescending(i => i.CreatedTime).Take(3).ToList();
            }
            else
            {
                model.ExcelInfoList = list.OrderByDescending(i => i.CreatedTime).ToList();
            }

            return View(model);
        }
        /// <summary>
        /// 产品编辑车型页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult Edit(string id)
        {
            ViewBag.Pid = id;
            var model = ipvimgr.GetProductInfoByPid(id);
            var vehicleBrandAndType = ipvimgr.GetExistVehicleBrandCategoryAndVehicleType();
            ViewBag.BrandCategory = vehicleBrandAndType.BrandCategoryList;//品牌系列如德系等
            ViewBag.VehicleType = vehicleBrandAndType.VehicleTypeList;//车型范围如SUV等
            return View(model);
        }
        /// <summary>
        /// 复制配置信息页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult Copy(string id)
        {
            ViewBag.Pid = id;
            var model = ipvimgr.GetProductInfoByPid(id);
            return View(model);
        }
        /// <summary>
        /// 产品操作日志页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProductLog(string id)
        {
            ViewBag.Pid = id;
            var model = ipvimgr.GetProductInfoByPid(id);

            var tempList = ipvimgr.GetAllLogByPid(id);
            var resultList = new List<ProductVehicleTypeConfigOpLogVm>();
            foreach (var item in tempList)
            {
                var entity = new ProductVehicleTypeConfigOpLogVm()
                {
                    Operator = item.Operator,
                    OperateTime = item.OperateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperateContent = item.OperateContent
                };
                resultList.Add(entity);
            }
            ViewBag.List = resultList.OrderByDescending(i => i.OperateTime);
            return View(model);
        }

        public ActionResult AllLog()
        {
            return View();
        }

        /// <summary>
        /// 根据查询条件获取所有车型信息，编辑页使用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllVehicleTypeInfoByParams(QueryCondition param)//根据PID和首字母筛选出对应的数据给到前端
        {
            //var obj = JsonConvert.DeserializeObject<QueryCondition>(postData);
            var tempList = new List<VehicleTypeInfoDb>() { };
            if (string.IsNullOrEmpty(param.Pid))
            {
                return Json(new { items = tempList, count = 0 });
            }
            else
            {
                //根据PID去查对应产品配置的CP_Remark是几级车型
                var productInfo = ipvimgr.GetProductInfoByPid(param.Pid);
                if (string.IsNullOrEmpty(productInfo.VehicleLevel) || productInfo.VehicleLevel == "无需车型")
                {
                    return Json(new { items = tempList, count = -1 });
                }
                if (string.IsNullOrEmpty(param.Condition))
                {
                    param.Condition = "A";//如果用户未选择首字母则默认拉出A品牌的车型数据
                }
                var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByCharacter(param.Condition);

                var sourceListA = new List<VehicleTypeInfoDb>();
                var sourceListB = new List<VehicleTypeInfoDb>();
                var sourceListC = new List<VehicleTypeInfoDb>();
                var sourceListAll = vehicleInfoList;//初始化
                if (!string.IsNullOrEmpty(param.VehicleSeries))
                {
                    //品牌系列
                    if (!param.VehicleSeries.Contains("全部"))
                    {
                        var brandCategory = param.VehicleSeries.TrimEnd(',').Split(',');
                        foreach (var item in brandCategory)
                        {
                            sourceListA.AddRange(item == "其他"
                                ? vehicleInfoList.FindAll(i => string.IsNullOrWhiteSpace(i.BrandCategory))
                                : vehicleInfoList.FindAll(i => i.BrandCategory == item));
                        }
                        sourceListAll = vehicleInfoList.Intersect(sourceListA).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(param.Price))
                {
                    //价格范围
                    if (!param.Price.Contains("全部"))
                    {
                        var priceStr = param.Price.TrimEnd(',').Split(',');
                        foreach (var price in priceStr)
                        {
                            if (price == "高端车（23万以上）")
                            {
                                sourceListB.AddRange(vehicleInfoList.FindAll(i => i.AvgPrice > 23));
                            }
                            else if (price == "中端车（12-23万）")
                            {
                                sourceListB.AddRange(vehicleInfoList.FindAll(i => i.AvgPrice >= 12 && i.AvgPrice <= 23));
                            }
                            else if (price == "低端车（12万以下）")
                            {
                                sourceListB.AddRange(vehicleInfoList.FindAll(i => i.AvgPrice < 12 && i.AvgPrice > 0));
                            }
                        }
                        sourceListAll = sourceListAll.Intersect(sourceListB).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(param.VehicleType))
                {
                    //车型范围
                    if (!param.VehicleType.Contains("全部"))
                    {
                        var vehicleTypeArr = param.VehicleType.TrimEnd(',').Split(',');
                        foreach (var vehicleType in vehicleTypeArr)
                        {
                            sourceListC.AddRange(vehicleType == "其他"
                                ? vehicleInfoList.FindAll(i => string.IsNullOrWhiteSpace(i.VehicleType))
                                : vehicleInfoList.FindAll(i => i.VehicleType == vehicleType));
                        }
                        sourceListAll = sourceListAll.Intersect(sourceListC).ToList();
                    }
                }

                if (string.IsNullOrEmpty(param.VehicleSeries) && string.IsNullOrEmpty(param.Price) && string.IsNullOrEmpty(param.VehicleType))
                {
                    sourceListAll = vehicleInfoList; //sourceListAll.AddRange(vehicleInfoListNew);
                }
                //只有TID跟生产年份都一致的才去重
                //var distinctList = sourceListAll.Distinct(new List_User_DistinctBy_TID()).ToList();

                var checkedItemList = ipvimgr.GetProductVehicleTypeConfigInfoListByPid(param.Pid);//当前产品已经配置的所有车型

                var vehicleTypeInfoList = new List<VehicleTypeInfoVm>();
                if (productInfo.VehicleLevel == "二级车型")
                {
                    var distinctList2 = sourceListAll.Distinct(new ListUserDistinctByLevel2()).ToList();
                    foreach (var item in distinctList2)
                    {
                        if (string.IsNullOrWhiteSpace(item.VehicleID))
                        {
                            continue;
                        }
                        var temp = new VehicleTypeInfoVm();
                        if (checkedItemList.Exists(i => i.VehicleID == item.VehicleID))
                        {
                            temp.IsChecked = "checked";
                        }
                        temp.VehicleID = item.VehicleID;
                        temp.AvgPrice = item.AvgPrice;
                        temp.Brand = string.IsNullOrEmpty(item.Brand) ? "" : item.Brand;
                        temp.BrandCategory = item.BrandCategory;
                        temp.JointVenture = item.JointVenture;
                        temp.ListedYear = item.ListedYear;
                        temp.Nian = item.Nian;
                        temp.PaiLiang = item.PaiLiang;
                        temp.SalesName = item.SalesName;
                        temp.StopProductionYear = item.StopProductionYear;
                        temp.TID = item.TID;
                        temp.Vehicle = string.IsNullOrEmpty(item.Vehicle) ? "" : item.Vehicle;
                        temp.VehicleSeries = item.VehicleSeries;
                        temp.VehicleType = item.VehicleType;

                        vehicleTypeInfoList.Add(temp);
                    }
                }

                if (productInfo.VehicleLevel == "四级车型")
                {
                    var distinctList4 = sourceListAll.Distinct(new ListUserDistinctByLevel4()).ToList();

                    foreach (var item in distinctList4)
                    {
                        if (string.IsNullOrWhiteSpace(item.VehicleID) || string.IsNullOrWhiteSpace(item.PaiLiang) ||
                            string.IsNullOrWhiteSpace(item.Nian))
                        {
                            continue;
                        }
                        var temp = new VehicleTypeInfoVm();
                        if (checkedItemList.Exists(i => i.VehicleID == item.VehicleID && i.Nian == item.Nian && i.PaiLiang == item.PaiLiang))
                        {
                            temp.IsChecked = "checked";
                        }
                        temp.VehicleID = item.VehicleID;
                        temp.AvgPrice = item.AvgPrice;
                        temp.Brand = string.IsNullOrEmpty(item.Brand) ? "" : item.Brand;
                        temp.BrandCategory = item.BrandCategory;
                        temp.JointVenture = item.JointVenture;
                        temp.ListedYear = item.ListedYear;
                        temp.Nian = string.IsNullOrEmpty(item.Nian) ? "" : item.Nian;
                        temp.PaiLiang = string.IsNullOrEmpty(item.PaiLiang) ? "" : item.PaiLiang;
                        temp.SalesName = item.SalesName;
                        temp.StopProductionYear = item.StopProductionYear;
                        temp.TID = item.TID;
                        temp.Vehicle = string.IsNullOrEmpty(item.Vehicle) ? "" : item.Vehicle;
                        temp.VehicleSeries = item.VehicleSeries;
                        temp.VehicleType = item.VehicleType;

                        vehicleTypeInfoList.Add(temp);
                    }
                }

                if (productInfo.VehicleLevel == "五级车型")
                {
                    var distinctList5 = sourceListAll.Distinct(new ListUserDistinctByLevel5()).ToList();

                    foreach (var item in distinctList5)
                    {
                        if (string.IsNullOrWhiteSpace(item.TID))
                        {
                            continue;
                        }
                        var temp = new VehicleTypeInfoVm();
                        if (checkedItemList.Exists(i => i.VehicleID == item.VehicleID && i.PaiLiang == item.PaiLiang && i.Nian == item.Nian && i.TID == item.TID))
                        {
                            temp.IsChecked = "checked";
                        }
                        temp.VehicleID = item.VehicleID;
                        temp.AvgPrice = item.AvgPrice;
                        temp.Brand = string.IsNullOrEmpty(item.Brand) ? "" : item.Brand;
                        temp.BrandCategory = item.BrandCategory;
                        temp.JointVenture = item.JointVenture;
                        temp.ListedYear = item.ListedYear;
                        temp.Nian = string.IsNullOrEmpty(item.Nian) ? "" : item.Nian;
                        temp.PaiLiang = string.IsNullOrEmpty(item.PaiLiang) ? "" : item.PaiLiang;
                        temp.SalesName = string.IsNullOrEmpty(item.SalesName) ? "" : item.SalesName;
                        temp.StopProductionYear = item.StopProductionYear;
                        temp.TID = item.TID;
                        temp.Vehicle = string.IsNullOrEmpty(item.Vehicle) ? "" : item.Vehicle;
                        temp.VehicleSeries = item.VehicleSeries;
                        temp.VehicleType = item.VehicleType;

                        vehicleTypeInfoList.Add(temp);
                    }
                }



                //var dataList = new List<TreeItem>();
                var treeItems = new List<TreeItem>();
                //车型数据分组
                switch (productInfo.VehicleLevel)
                {
                    case "二级车型":
                        //TODO：查询首字母为X的二级车型信息
                        var dataLevel2 = vehicleTypeInfoList.GroupBy(item => item.Brand)
                            .ToDictionary(i => i.Key,
                                i => i.ToList().GroupBy(t => t.Vehicle).ToDictionary(j => j.Key, j => j.ToList()));

                        foreach (var item in dataLevel2)
                        {
                            var currentKey = item.Key;//品牌
                            var treeItemBrand = new TreeItem
                            {
                                id = item.Key,
                                check = "true",
                                name = currentKey,
                                children = new List<TreeItem>()
                            };

                            foreach (var child in item.Value)
                            {
                                var currentChildKey = child.Key;//车系
                                var treeItemVehicleId = new TreeItem()
                                {
                                    id = item.Key + "$" + child.Key,
                                    name = currentChildKey,
                                    check = "true"
                                };

                                if (child.Value.Exists(i => i.IsChecked == null))
                                {
                                    treeItemVehicleId.check = "false";
                                }
                                treeItemBrand.children.Add(treeItemVehicleId);
                            }

                            if (treeItemBrand.children.Exists(i => i.check == "false"))
                            {
                                treeItemBrand.check = "false";
                            }

                            treeItems.Add(treeItemBrand);
                        }

                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand" }, "Brand", true);
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle" }, "Vehicle");
                        break;
                    case "四级车型":
                        //TODO：查询首字母为X的四级车型信息

                        var dataLevel4 = vehicleTypeInfoList.GroupBy(item => item.Brand)
                            .ToDictionary(i => i.Key,
                                i => i.ToList().GroupBy(t => t.Vehicle).ToDictionary(j => j.Key, j => j.ToList().GroupBy(t => t.PaiLiang).ToDictionary(a => a.Key, a => a.ToList().GroupBy(c => c.ListedYear).ToDictionary(d => d.Key, d => d.ToList()))));

                        foreach (var item in dataLevel4)
                        {
                            var currentKey = item.Key;//品牌
                            var treeItemBrand = new TreeItem
                            {
                                id = item.Key,
                                check = "true",
                                name = currentKey,
                                children = new List<TreeItem>()
                            };

                            foreach (var child in item.Value)
                            {
                                var currentChildKey = child.Key;//车系
                                var treeItemVehicleId = new TreeItem()
                                {
                                    id = item.Key + "$" + child.Key,
                                    name = currentChildKey,
                                    check = "true",
                                    children = new List<TreeItem>()
                                };

                                foreach (var child2 in child.Value)
                                {
                                    var currentChild2Key = child2.Key;//排量
                                    var treeItemPailiang = new TreeItem()
                                    {
                                        id = item.Key + "$" + child.Key + "$" + child2.Key,
                                        name = currentChild2Key,
                                        check = "true",
                                        children = new List<TreeItem>()
                                    };

                                    foreach (var child3 in child2.Value.OrderByDescending(v => v.Key))
                                    {
                                        var currentChild3Key = child3.Key;//年产
                                        var treeItemNian = new TreeItem()
                                        {
                                            id = item.Key + "$" + child.Key + "$" + child2.Key + "$" + child3.Key,
                                            name = currentChild3Key + "年产",
                                            check = "true"
                                        };

                                        if (child3.Value.Exists(i => i.IsChecked == null))
                                        {
                                            treeItemNian.check = "false";
                                        }

                                        treeItemPailiang.children.Add(treeItemNian);
                                    }

                                    if (treeItemPailiang.children.Exists(i => i.check == "false"))
                                    {
                                        treeItemPailiang.check = "false";
                                    }
                                    treeItemVehicleId.children.Add(treeItemPailiang);
                                }
                                if (treeItemVehicleId.children.Exists(i => i.check == "false"))
                                {
                                    treeItemVehicleId.check = "false";
                                }

                                treeItemBrand.children.Add(treeItemVehicleId);
                            }

                            if (treeItemBrand.children.Exists(i => i.check == "false"))
                            {
                                treeItemBrand.check = "false";
                            }

                            treeItems.Add(treeItemBrand);
                        }





                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand" }, "Brand", true);
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle" }, "Vehicle");
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang" }, "PaiLiang");
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang", "Nian" }, "Nian");
                        //GroupByData(distinctList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang", "ListedYear" }, "ListedYear");
                        break;
                    case "五级车型":
                        //TODO：查询首字母为X的五级车型信息
                        var dataLevel5 = vehicleTypeInfoList.GroupBy(item => item.Brand)
                            .ToDictionary(i => i.Key,
                                i => i.ToList().GroupBy(t => t.Vehicle).ToDictionary(j => j.Key, j => j.ToList().GroupBy(t => t.PaiLiang).ToDictionary(a => a.Key, a => a.ToList().GroupBy(c => c.ListedYear).ToDictionary(d => d.Key, d => d.ToList().GroupBy(e => e.SalesName).ToDictionary(f => f.Key, f => f.ToList())))));

                        foreach (var item in dataLevel5)
                        {
                            var currentKey = item.Key;//品牌
                            var treeItemBrand = new TreeItem
                            {
                                id = item.Key,
                                check = "true",
                                name = currentKey,
                                children = new List<TreeItem>()
                            };

                            foreach (var child in item.Value)
                            {
                                var currentChildKey = child.Key;//车系
                                var treeItemVehicleId = new TreeItem()
                                {
                                    id = item.Key + "$" + child.Key,
                                    name = currentChildKey,
                                    check = "true",
                                    children = new List<TreeItem>()
                                };

                                foreach (var child2 in child.Value)
                                {
                                    var currentChild2Key = child2.Key;//排量
                                    var treeItemPailiang = new TreeItem()
                                    {
                                        id = item.Key + "$" + child.Key + "$" + child2.Key,
                                        name = currentChild2Key,
                                        check = "true",
                                        children = new List<TreeItem>()
                                    };

                                    foreach (var child3 in child2.Value.OrderByDescending(v => v.Key))
                                    {
                                        var currentChild3Key = child3.Key;//年产
                                        var treeItemNian = new TreeItem()
                                        {
                                            id = item.Key + "$" + child.Key + "$" + child2.Key + "$" + child3.Key,
                                            name = currentChild3Key + "年产",
                                            check = "true",
                                            children = new List<TreeItem>()
                                        };

                                        foreach (var child4 in child3.Value)
                                        {
                                            var currentChild4Key = child4.Key;//SalesName
                                            var treeItemSalesName = new TreeItem()
                                            {
                                                id =
                                                    item.Key + "$" + child.Key + "$" + child2.Key + "$" + child3.Key +
                                                    "$" + child4.Key,
                                                name = currentChild4Key,
                                                check = "true"
                                            };
                                            if (child4.Value.Exists(i => i.IsChecked == null))
                                            {
                                                treeItemSalesName.check = "false";
                                            }
                                            treeItemNian.children.Add(treeItemSalesName);
                                        }

                                        if (treeItemNian.children.Exists(i => i.check == "false"))
                                        {
                                            treeItemNian.check = "false";
                                        }

                                        treeItemPailiang.children.Add(treeItemNian);
                                    }

                                    if (treeItemPailiang.children.Exists(i => i.check == "false"))
                                    {
                                        treeItemPailiang.check = "false";
                                    }
                                    treeItemVehicleId.children.Add(treeItemPailiang);
                                }
                                if (treeItemVehicleId.children.Exists(i => i.check == "false"))
                                {
                                    treeItemVehicleId.check = "false";
                                }

                                treeItemBrand.children.Add(treeItemVehicleId);
                            }

                            if (treeItemBrand.children.Exists(i => i.check == "false"))
                            {
                                treeItemBrand.check = "false";
                            }

                            treeItems.Add(treeItemBrand);
                        }



                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand" }, "Brand", true);
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle" }, "Vehicle");
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang" }, "PaiLiang");
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang", "Nian" }, "Nian");
                        ////GroupByData(distinctList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang", "ListedYear" }, "ListedYear");
                        ////GroupByData(distinctList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang", "ListedYear", "SalesName" }, "SalesName");
                        //GroupByData(vehicleTypeInfoList, dataList, new string[] { "Brand", "Vehicle", "PaiLiang", "Nian", "SalesName" }, "SalesName");
                        break;
                    default:
                        break;
                }

                //GetTreeData(treeItems, dataList, null, "", checkedItemList);
                //return Json(new { items = treeItems, count = treeItems.Count });
                return new ContentResult
                {
                    Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new { items = treeItems, count = treeItems.Count }),
                    ContentType = "application/json"
                };
            }

        }

        /// <summary>
        /// 根据条件对所有符合的产品做分页查询,产品查询列表页调用
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllProductInfoByParams(string condition, string status, int pageIndex = 1, int pageSize = 10)
        {
            var type = -1;
            var tempaList = new List<ProductInfoViewModel>() { };
            if (string.IsNullOrEmpty(condition))//初始化进入还没有查询
            {
                return Json(new { items = tempaList, count = tempaList.Count });
            }
            if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "已配置已导入":
                        type = 1;
                        break;
                    case "已配置未导入":
                        type = 2;
                        break;
                    case "未配置":
                        type = 3;
                        break;
                }
            }

            var resultItems = ipvimgr.SearchProductInfoByParam(condition, pageIndex, pageSize, type);

            if (resultItems == null || resultItems.Count <= 0)
            {
                return Json(new { items = tempaList, count = tempaList.Count });
            }

            var tempList = new List<ProductInfoViewModel>() { };
            foreach (var item in resultItems)
            {
                var temp = new ProductInfoViewModel()
                {
                    Pid = item.PID,
                    DisplayName = item.DisplayName,
                    Brand = item.Brand,
                    ConfigStatus = string.IsNullOrWhiteSpace(item.CP_Remark) || item.CP_Remark == "无需车型" ? "未配置" : "已配置",//批量复制的时候要去设置下对应产品的CP_Remark字段
                    ConfigTime = string.IsNullOrWhiteSpace(item.UpdateTime) ? "无" : item.UpdateTime
                };
                if (!string.IsNullOrWhiteSpace(item.CP_Remark) && item.CP_Remark != "无需车型")
                {
                    temp.ConfigStatus = !string.IsNullOrWhiteSpace(item.UpdateTime) ? "已配置已导入" : "已配置未导入";
                }

                tempList.Add(temp);
            }
            return Json(new { items = tempList, count = resultItems.FirstOrDefault().Total });
        }
        /// <summary>
        /// 根据给定的pids对相关产品做分页查询,复制产品配置时查询目标产品
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllProductInfoByPids(string pids, int pageIndex = 1, int pageSize = 10)
        {
            var tempList = new List<ProductInfoViewModel>() { };
            var resultList = new List<ProductInfoViewModel>() { };
            if (string.IsNullOrEmpty(pids))
            {
                return Json(new { items = tempList, count = tempList.Count });

                //return Json(new { message = "PID不能为空！" });
            }
            else
            {
                var tempPids = pids.Replace('\n', ',').Replace("，", ",").TrimEnd(',').Trim(' ');//先中文逗号转为英文逗号，然后按英文逗号分隔
                var tempArray = tempPids.Split(',');
                var pidSb = new StringBuilder();
                foreach (var pid in tempArray)
                {
                    pidSb.AppendFormat("{0},", pid.Trim(' ').Trim('\t'));
                }
                tempPids = pidSb.ToString().TrimEnd(',');
                var searchList = ipvimgr.GetProductInfoByPids(tempPids);
                foreach (var item in searchList)
                {
                    var temp = new ProductInfoViewModel()
                    {
                        DisplayName = item.DisplayName,
                        Pid = item.PID,
                        Brand = item.Brand
                    };
                    tempList.Add(temp);
                }

                resultList = tempList.Skip(pageSize * (pageIndex - 1)).Take(pageSize).Select(i => i).ToList();
                return Json(new { items = resultList, count = tempList.Count });
            }
        }
        /// <summary>
        /// 查看操作日志页根据搜索时间查找所有相关日志
        /// </summary>
        /// <param name="param"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllLogByTime(LogCondition param, int pageIndex = 1, int pageSize = 10)
        {
            var tempList = new List<ProductVehicleTypeConfigOpLog>();
            var resultList = new List<ProductVehicleTypeConfigOpLogVm>();

            if (string.IsNullOrEmpty(param.TimeStart) && string.IsNullOrEmpty(param.TimeEnd))
            {
                tempList = ipvimgr.GetAllLog().OrderByDescending(i => i.OperateTime).ToList();
                resultList = tempList.Select(item => new ProductVehicleTypeConfigOpLogVm()
                {
                    Operator = item.Operator,
                    OperateTime = item.OperateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperateContent = item.OperateContent
                }).ToList();
                resultList = resultList.Skip(pageSize * (pageIndex - 1)).Take(pageSize).Select(i => i).ToList();
                return Json(new { items = resultList, count = tempList.Count });
            }
            tempList = ipvimgr.GetAllLogByTime(param.TimeStart, param.TimeEnd);

            resultList.AddRange(tempList.Select(item => new ProductVehicleTypeConfigOpLogVm()
            {
                Operator = item.Operator,
                OperateTime = item.OperateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                OperateContent = item.OperateContent
            }));

            resultList = resultList.Skip(pageSize * (pageIndex - 1)).Take(pageSize).Select(i => i).ToList();
            return Json(new { items = resultList, count = tempList.Count });
        }

        /// <summary>
        /// 产品通用信息页保存所选车型信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="cpremark"></param>
        /// <param name="isAuto"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult SaveProductInfo(string pid, string cpremark, bool isAuto, string vehicleLevel)
        {
            try
            {
                var flag = ipvimgr.SaveProductInfoByPid(pid, cpremark, isAuto, vehicleLevel);
                var isAutoAssociation = isAuto ? "" : "不";
                if (flag)
                {
                    using (var cacheClient = new CacheClient())
                    {
                        var cacheResult = cacheClient.RefreshProductCacheByPids(new List<string> { pid });
                        if (!cacheResult.Result)
                        {
                            var log = new ProductVehicleTypeConfigOpLog()
                            {
                                PID = pid,
                                Operator = User.Identity.Name,
                                OperateContent = $"缓存更新：产品库车型缓存更新失败！",
                                OperateTime = DateTime.Now,
                                CreatedTime = DateTime.Now,
                            };
                            ipvimgr.WriteOperatorLog(log);
                        }
                    }
                    using (var client = new ProductClient())
                    {
                        var productResult = client.UpdateConfigLevelCacheByPid(pid);
                        if (!productResult.Result)
                        {
                            var log = new ProductVehicleTypeConfigOpLog()
                            {
                                PID = pid,
                                Operator = User.Identity.Name,
                                OperateContent = $"缓存更新：将产品{pid}的级别改为{cpremark}，缓存更新失败！",
                                OperateTime = DateTime.Now,
                                CreatedTime = DateTime.Now,
                            };
                            ipvimgr.WriteOperatorLog(log);
                        }
                    }

                    TuhuNotification.SendNotification("notification.productmatch.modify", new Dictionary<string, object>
                    {
                        ["type"] = "UpdateAdpter",
                        ["pids"] = new List<string>
                    {
                        pid
                    }
                    });



                    var entity = new ProductVehicleTypeConfigOpLog()
                    {
                        PID = pid,
                        Operator = User.Identity.Name,
                        OperateContent = string.Format("修改配置：将产品{0}的车型改为{1}", pid, cpremark),
                        OperateTime = DateTime.Now,
                        CreatedTime = DateTime.Now,
                    };
                    ipvimgr.WriteOperatorLog(entity);

                    var dataLog = new ProductVehicleTypeConfigOpLog()
                    {
                        PID = pid,
                        Operator = User.Identity.Name,
                        OperateContent = $"修改配置：将产品{pid}改为{isAutoAssociation}自动适配新增车型",
                        OperateTime = DateTime.Now,
                        CreatedTime = DateTime.Now,
                    };
                    ipvimgr.WriteOperatorLog(dataLog);
                    return Json(new { msg = "success" });
                }
                else
                {
                    return Json(new { msg = "fail" });
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, $"SaveProductInfo;ErrorMessage:{ex.Message};ErrorStackTrace:{ex.StackTrace}");
                return Json(new { msg = "fail", erreMessage = ex.Message, stackTracee = ex.StackTrace });
            }
        }
        /// <summary>
        /// 上传Excel文件处理
        /// </summary>
        /// <param name="filebase"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult FileImport(HttpPostedFileBase filebase)
        {
            var pid = "";
            try
            {


                HttpPostedFileBase file = Request.Files["files"];
                pid = System.Web.HttpUtility.UrlDecode(Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1]);
                string fileName;
                string ext;
                string noFileName;
                string savePath = "";

                var productInfo = ipvimgr.GetProductInfoByPid(pid);
                if (string.IsNullOrEmpty(productInfo.VehicleLevel) || productInfo.VehicleLevel.Contains("无需车型"))
                {
                    ViewBag.error = "此产品不需要配置车型数据！";
                    return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
                }

                if (file == null || file.ContentLength <= 0)
                {
                    ViewBag.error = "文件不能为空";
                    return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
                }
                else
                {
                    //string filename = Path.GetFileName(file.FileName);
                    int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                    ext = System.IO.Path.GetExtension(file.FileName);//获取上传文件的扩展名
                    noFileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);//获取无扩展名的文件名
                    int Maxsize = 6000 * 1024;//定义上传文件的最大空间大小为6M
                    string FileType = ".xls,.xlsx";//定义上传文件的类型字符串

                    fileName = noFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + ext;
                    if (!FileType.Contains(ext))
                    {
                        ViewBag.error = "文件类型不对，只能导入xls和xlsx格式的文件";
                        return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
                    }
                    //注释掉文件大小的限制
                    //if (filesize >= Maxsize)
                    //{
                    //    ViewBag.error = "上传文件超过6M，不能上传";
                    //    return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
                    //}
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/excel/";
                    //savePath = string.Join("/", path, fileName);
                    //file.SaveAs(savePath);//文件先保存到服务器上
                    var virtualPath = swc.WebConfigurationManager.AppSettings["UploadDoMain_excel"];
                    var domainPath = swc.WebConfigurationManager.AppSettings["DoMain_image"];
                    //TODO 去掉上传文件老的方法
                    //var serverPath = virtualPath + fileName;//http://file.tuhu.test/vehicletype/excel/aaa.xls
                    //savePath = domainPath + serverPath;
                    //byte[] input = new byte[filesize];
                    //var fileStream = file.InputStream;
                    //fileStream.Read(input, 0, filesize);

                    //var client = new WcfClinet<IFileUpload>();
                    //var result = client.InvokeWcfClinet(w => w.UploadFile(serverPath, input));

                    //TODO 用新方法实现上传文件
                    var result =  UpLoadManage.UpLoadFile(virtualPath, file, "xls");
                    if (result.Success)
                    {
                        savePath = domainPath + result.Result;
                    }
                    else
                    {
                        ViewBag.error = "文件上传失败！Error:" + result.ErrorMessage;
                        return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
                    }
                    //将上传的Excel文件信息入库
                    var excelInfo = new ProductVehicleTypeFileInfoDb()
                    {
                        PID = pid,
                        Operator = User.Identity.Name,
                        FilePath = savePath,
                        CreatedTime = DateTime.Now
                    };

                    ipvimgr.SaveProductVehicleExcelInfo(excelInfo);

                }

                var table = new DataTable();
                XSSFWorkbook wb1;
                HSSFWorkbook wb2;

                //var sw = new Stopwatch();


                var bytes = StreamToBytes(file.InputStream);

                using (MemoryStream memstream = new MemoryStream(bytes))
                {
                    //file.InputStream.CopyTo(memstream);
                    //memstream.Position = 0; // <-- Add this, to make it work

                    if (fileName.EndsWith(".xlsx"))
                    {
                        wb1 = new XSSFWorkbook(memstream);
                        table = UploadData(wb1);
                    }
                    else if (fileName.EndsWith(".xls"))
                    {
                        wb2 = new HSSFWorkbook(memstream);
                        table = UploadData(wb2);
                    }
                }

                if (ipvimgr.ImportVehicleInfoByPid(pid, table, noFileName, productInfo.VehicleLevel))
                {
                    var entity = new ProductVehicleTypeConfigOpLog()
                    {
                        PID = pid,
                        Operator = User.Identity.Name,
                        OperateContent = string.Format("导入配置：{0}/{1}", productInfo.DisplayName, productInfo.ProductCode),
                        OperateTime = DateTime.Now,
                        CreatedTime = DateTime.Now,
                    };
                    ipvimgr.WriteOperatorLog(entity);

                    ViewBag.error = "上传成功！"; //+ sw.ElapsedMilliseconds;
                }
                else
                {
                    ViewBag.error = "上传车型级别与该产品不匹配或上传失败！";
                }
            }
            catch (Exception e)
            {
                ViewBag.error = "异常：" + e.Message;
            }
            return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
        }


    


        /// <summary>
        /// 编辑页保存修改数据
        /// </summary>
        /// <param name="param">前端传过来的选中车型数据</param>
        /// <param name="level">当前产品设置的车型级别</param>
        /// <param name="pid"></param>
        /// <param name="selectItems"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult UpdateProductVehicleTypeConfigInfo(string param, string level, string pid, string selectItems)
        {
            var conditionList = JsonConvert.DeserializeObject<List<Condition>>(param);
            var queryParams = JsonConvert.DeserializeObject<QueryCondition>(selectItems);//前端选择的筛选条件
            var productInfo = ipvimgr.GetProductInfoByPid(pid);
            var vehicleInfoListB = new List<VehicleTypeInfoDb>();//车系筛选结果
            var vehicleInfoListC = new List<VehicleTypeInfoDb>();//价格筛选结果
            var vehicleInfoListD = new List<VehicleTypeInfoDb>();//车型筛选结果

            var brandList = conditionList.Select(item => item.Brand).ToList();
            var brands = brandList.Distinct().ToList();
            var sb = new StringBuilder();
            foreach (var item in brands)
            {
                sb.AppendFormat("{0},", item);
            }
            //先查出所有选择的品牌的车型信息，然后过滤出待插入数据
            var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByBrandName(sb.ToString().TrimEnd(','));
            if (vehicleInfoList.Count == 0 && !string.IsNullOrEmpty(queryParams.Condition))
            {
                //防止没有数据的情况
                vehicleInfoList = ipvimgr.GetVehicleTypeInfoByCharacter(queryParams.Condition);
            }
            var vehicleInfoListAll = vehicleInfoList;

            if (!string.IsNullOrEmpty(queryParams.VehicleSeries))
            {
                if (!queryParams.VehicleSeries.Contains("全部"))
                {
                    var brandCategory = queryParams.VehicleSeries.TrimEnd(',').Split(',');
                    foreach (var item in brandCategory)
                    {
                        vehicleInfoListB.AddRange(item == "其他"
                            ? vehicleInfoList.FindAll(i => string.IsNullOrWhiteSpace(i.BrandCategory))
                            : vehicleInfoList.FindAll(i => i.BrandCategory == item));
                        //vehicleInfoListB.AddRange(vehicleInfoList.FindAll(i => i.BrandCategory == item));
                    }
                    vehicleInfoListAll = vehicleInfoList.Intersect(vehicleInfoListB).ToList();
                }
            }

            if (!string.IsNullOrEmpty(queryParams.Price))
            {
                if (!queryParams.Price.Contains("全部"))
                {
                    var priceStr = queryParams.Price.TrimEnd(',').Split(',');
                    foreach (var price in priceStr)
                    {
                        if (price == "高端车（23万以上）")
                        {
                            vehicleInfoListC.AddRange(vehicleInfoList.FindAll(i => i.AvgPrice > 23));
                        }
                        else if (price == "中端车（12-23万）")
                        {
                            vehicleInfoListC.AddRange(vehicleInfoList.FindAll(i => i.AvgPrice >= 12 && i.AvgPrice <= 23));
                        }
                        else if (price == "低端车（12万以下）")
                        {
                            vehicleInfoListC.AddRange(vehicleInfoList.FindAll(i => i.AvgPrice < 12));
                        }
                    }
                    vehicleInfoListAll = vehicleInfoListAll.Intersect(vehicleInfoListC).ToList();
                }
            }

            if (!string.IsNullOrEmpty(queryParams.VehicleType))
            {
                if (!queryParams.VehicleType.Contains("全部"))
                {
                    var vehicleTypeArr = queryParams.VehicleType.TrimEnd(',').Split(',');
                    foreach (var vehicleType in vehicleTypeArr)
                    {
                        vehicleInfoListD.AddRange(vehicleType == "其他"
                            ? vehicleInfoList.FindAll(i => string.IsNullOrWhiteSpace(i.VehicleType))
                            : vehicleInfoList.FindAll(i => i.VehicleType == vehicleType));
                    }
                    vehicleInfoListAll = vehicleInfoListAll.Intersect(vehicleInfoListD).ToList();
                }
            }

            if (string.IsNullOrEmpty(queryParams.VehicleSeries) && string.IsNullOrEmpty(queryParams.Price) && string.IsNullOrEmpty(queryParams.VehicleType))
            {
                vehicleInfoListAll.AddRange(vehicleInfoList);
            }
            var distinctList = new List<VehicleTypeInfoDb>();
            switch (productInfo.VehicleLevel)
            {
                case "二级车型":
                    distinctList = vehicleInfoListAll.Distinct(new ListUserDistinctByLevel2()).ToList();
                    break;
                case "四级车型":
                    distinctList = vehicleInfoListAll.Distinct(new ListUserDistinctByLevel4()).ToList();
                    break;
                case "五级车型":
                    distinctList = vehicleInfoListAll.Distinct(new ListUserDistinctByLevel5()).ToList();
                    break;
            }
            //var distinctList = vehicleInfoListAll.Distinct(new List_User_DistinctBy_TID()).ToList();
            var existConfigList = ipvimgr.GetProductVehicleTypeConfigInfoListByPid(pid);
            var alphabet = "";
            if (distinctList.Count > 0)
            {
                alphabet = distinctList.First().Brand.Replace('—', '-').Split('-')[0].Trim();
            }
            var listByAlpha = ipvimgr.GetVehicleTypeInfoByCharacter(alphabet);
            //var existConfigListBak = existConfigList;
            var levelConfig = 0;
            if (existConfigList.Count > 0)
            {
                var productVehicleTypeConfigDb = existConfigList.FirstOrDefault();
                if (productVehicleTypeConfigDb != null)
                    levelConfig = productVehicleTypeConfigDb.ConfigLevel;
            }

            for (var t = existConfigList.Count - 1; t >= 0; t--)
            {
                //判断当前首字母的车型数据中是否包含已经在配置表中的数据，如果不包含则直接进待插入列表，反之进后面判断是否进待删除列表
                switch (levelConfig)
                {
                    case 2:
                    case 4:
                        if (!listByAlpha.Exists(i => i.VehicleID == existConfigList[t].VehicleID))
                        {
                            existConfigList.RemoveAt(t);
                        }
                        break;
                    case 5:
                        //if (!listByAlpha.Exists(i => i.TID == existConfigList[t].TID))
                        //{
                        //    existConfigList.RemoveAt(t);
                        //}TID有可能为空
                        if (!listByAlpha.Exists(i => i.VehicleID == existConfigList[t].VehicleID && i.PaiLiang == existConfigList[t].PaiLiang && i.Nian == existConfigList[t].Nian && i.SalesName == existConfigList[t].SalesName))
                        {
                            existConfigList.RemoveAt(t);
                        }
                        break;
                }
            }
            //if (levelConfig == 5)
            //{
            //    existConfigList = existConfigList.Distinct(new ListVehicleTypeConfigDistinctByLevel5()).ToList();
            //}
            //获取所有待更新数据
            var dt = GetDtForUpdate(distinctList, conditionList, pid, level, existConfigList, productInfo);
            //"二级车型"://需要vehicleid加pid
            //"四级车型"://需要vehicleid加pid加pailiang加nian
            //"五级车型"://需要tid加pid
            if (ipvimgr.UpdateVehicleInfoByPid(pid, dt, level, level))
            {
                var entity = new ProductVehicleTypeConfigOpLog()
                {
                    PID = pid,
                    Operator = User.Identity.Name,
                    OperateContent = string.Format("修改配置：{0}/{1}", productInfo.DisplayName, productInfo.ProductCode),
                    OperateTime = DateTime.Now,
                    CreatedTime = DateTime.Now,
                };
                ipvimgr.WriteOperatorLog(entity);
            }


            return Json(new { msg = "success" });
        }
        /// <summary>
        /// 车型模板文件下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DownLoadFile(string id)
        {
            var pid = id;
            var productInfo = ipvimgr.GetProductInfoByPid(pid);
            //string path = AppDomain.CurrentDomain.BaseDirectory + "Content/Export/";
            //string fileName = "";
            if (string.IsNullOrEmpty(productInfo.VehicleLevel))
            {
                ViewBag.error = "此产品无需配置车型数据！";
                return RedirectToAction("FileInportExport", "ProductVehicleType", new { id = pid, msg = ViewBag.error });
            }
            //if (productInfo.CP_Remark.Contains("二级"))
            //{
            //    fileName = "二级车型_模板.xlsx";
            //}
            //else if (productInfo.CP_Remark.Contains("四级"))
            //{
            //    fileName = "四级车型_模板.xlsx";
            //}
            //else if (productInfo.CP_Remark.Contains("五级"))
            //{
            //    fileName = "五级车型_模板.xlsx";

            //}
            //return File(path + fileName, "text/plain", fileName);
            var sourceList = ipvimgr.GetVehicleInfoEx();//获取所有车型信息

            if (productInfo.VehicleLevel == "二级车型")
            {
                sourceList = sourceList.Distinct(new ListDistinctByVehicleId()).ToList();
            }
            if (productInfo.VehicleLevel == "四级车型")
            {
                sourceList = sourceList.Distinct(new ListDistinctForLevel4()).ToList();
            }
            if (productInfo.VehicleLevel == "五级车型")
            {
                sourceList = sourceList.Distinct(new ListDistinctForLevel5()).ToList();
            }

            var vmList = new List<ProductVehicleTypeConfigInfoVm>() { };
            sourceList = sourceList.OrderBy(i => i.Brand).ToList();
            vmList.AddRange(sourceList.Select(item => new ProductVehicleTypeConfigInfoVm()
            {
                BrandName = item.Brand, //temp != null ? temp.Brand : "未知",
                VehicleType = item.Vehicle, //temp != null ? temp.Vehicle : "未知",
                ListYear = item.ListedYear, //temp != null ? temp.ListedYear : "未知",
                StopYear = item.StopProductionYear, //temp != null ? temp.StopProductionYear : "未知",
                Nian = item.Nian,
                PaiLiang = item.PaiLiang,
                SalesName = item.SalesName,
                TID = item.TID,
                PID = item.PID,
                ConfigLevel = item.ConfigLevel,
                VehicleID = item.VehicleID,
            }));

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            var fileName = pid + "-" + productInfo.VehicleLevel + ".xls";
            if (productInfo.VehicleLevel == "二级车型")
            {
                row1.CreateCell(0).SetCellValue("ProductID");
                row1.CreateCell(1).SetCellValue("品牌");
                row1.CreateCell(2).SetCellValue("车系");
                row1.CreateCell(3).SetCellValue("是否可用");
                for (int i = 0; i < vmList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(vmList[i].VehicleID);
                    rowtemp.CreateCell(1).SetCellValue(vmList[i].BrandName);
                    rowtemp.CreateCell(2).SetCellValue(vmList[i].VehicleType);
                }
            }
            if (productInfo.VehicleLevel == "四级车型")
            {
                row1.CreateCell(0).SetCellValue("品牌");
                row1.CreateCell(1).SetCellValue("车系");
                row1.CreateCell(2).SetCellValue("VehicleID");
                row1.CreateCell(3).SetCellValue("排量");
                row1.CreateCell(4).SetCellValue("年款");
                row1.CreateCell(5).SetCellValue("是否可用");
                for (int i = 0; i < vmList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(vmList[i].BrandName);
                    rowtemp.CreateCell(1).SetCellValue(vmList[i].VehicleType);
                    rowtemp.CreateCell(2).SetCellValue(vmList[i].VehicleID);
                    rowtemp.CreateCell(3).SetCellValue(vmList[i].PaiLiang);
                    rowtemp.CreateCell(4).SetCellValue(vmList[i].Nian);
                }
            }
            if (productInfo.VehicleLevel == "五级车型")
            {
                row1.CreateCell(0).SetCellValue("品牌");
                row1.CreateCell(1).SetCellValue("车系");
                row1.CreateCell(2).SetCellValue("VehicleID");
                row1.CreateCell(3).SetCellValue("TID");
                row1.CreateCell(4).SetCellValue("排量");
                row1.CreateCell(5).SetCellValue("年款");
                row1.CreateCell(6).SetCellValue("上市年份");
                row1.CreateCell(7).SetCellValue("停止生产年份");
                row1.CreateCell(8).SetCellValue("版型");
                row1.CreateCell(9).SetCellValue("是否可用");
                for (int i = 0; i < vmList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(vmList[i].BrandName);
                    rowtemp.CreateCell(1).SetCellValue(vmList[i].VehicleType);
                    rowtemp.CreateCell(2).SetCellValue(vmList[i].VehicleID);
                    rowtemp.CreateCell(3).SetCellValue(vmList[i].TID);
                    rowtemp.CreateCell(4).SetCellValue(vmList[i].PaiLiang);
                    rowtemp.CreateCell(5).SetCellValue(vmList[i].Nian);
                    rowtemp.CreateCell(6).SetCellValue(vmList[i].ListYear);
                    rowtemp.CreateCell(7).SetCellValue(vmList[i].StopYear);
                    rowtemp.CreateCell(8).SetCellValue(vmList[i].SalesName);
                }
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }
        /// <summary>
        /// 最新修改过的车型数据下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult DownLoadFileNew(string id)
        {
            var pid = id;
            if (string.IsNullOrEmpty(pid))
            {
                //没拿到pid 不操作
                return Content("PID 不能为空");
            }

            var productInfo = ipvimgr.GetProductInfoByPid(id);

            if (string.IsNullOrEmpty(productInfo.VehicleLevel) || productInfo.VehicleLevel == "无需车型")
            {
                return Content("此产品无需车型配置");
            }

            //var productVehicleTypeConfigList = ipvimgr.GetProductVehicleTypeConfigInfoListByPid(pid);
            var sourceList = ipvimgr.GetVehicleInfoExByPid(pid, productInfo.VehicleLevel);

            if (productInfo.VehicleLevel == "二级车型")
            {
                sourceList = sourceList.Distinct(new ListDistinctByVehicleId()).ToList();
            }
            if (productInfo.VehicleLevel == "四级车型")
            {
                sourceList = sourceList.Distinct(new ListDistinctForLevel4()).ToList();
            }
            if (productInfo.VehicleLevel == "五级车型")
            {
                sourceList = sourceList.Distinct(new ListDistinctForLevel5()).ToList();
            }

            var vmList = new List<ProductVehicleTypeConfigInfoVm>() { };
            sourceList = sourceList.OrderBy(i => i.Brand).ToList();
            vmList.AddRange(sourceList.Select(item => new ProductVehicleTypeConfigInfoVm()
            {
                BrandName = item.Brand, //temp != null ? temp.Brand : "未知",
                VehicleType = item.Vehicle, //temp != null ? temp.Vehicle : "未知",
                ListYear = item.ListedYear, //temp != null ? temp.ListedYear : "未知",
                StopYear = item.StopProductionYear, //temp != null ? temp.StopProductionYear : "未知",
                Nian = item.Nian,
                PaiLiang = item.PaiLiang,
                SalesName = item.SalesName,
                TID = item.TID,
                PID = item.PID,
                ConfigLevel = item.ConfigLevel,
                VehicleID = item.VehicleID,
            }));

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            var fileName = pid + "-" + productInfo.VehicleLevel + ".xls";
            if (productInfo.VehicleLevel == "二级车型")
            {
                row1.CreateCell(0).SetCellValue("ProductID");
                row1.CreateCell(1).SetCellValue("品牌");
                row1.CreateCell(2).SetCellValue("车系");
                row1.CreateCell(3).SetCellValue("是否可用");
                for (int i = 0; i < vmList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(vmList[i].VehicleID);
                    rowtemp.CreateCell(1).SetCellValue(vmList[i].BrandName);
                    rowtemp.CreateCell(2).SetCellValue(vmList[i].VehicleType);
                    rowtemp.CreateCell(3).SetCellValue("1");
                }
            }
            if (productInfo.VehicleLevel == "四级车型")
            {
                row1.CreateCell(0).SetCellValue("品牌");
                row1.CreateCell(1).SetCellValue("车系");
                row1.CreateCell(2).SetCellValue("VehicleID");
                row1.CreateCell(3).SetCellValue("排量");
                row1.CreateCell(4).SetCellValue("年款");
                row1.CreateCell(5).SetCellValue("是否可用");
                for (int i = 0; i < vmList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(vmList[i].BrandName);
                    rowtemp.CreateCell(1).SetCellValue(vmList[i].VehicleType);
                    rowtemp.CreateCell(2).SetCellValue(vmList[i].VehicleID);
                    rowtemp.CreateCell(3).SetCellValue(vmList[i].PaiLiang);
                    rowtemp.CreateCell(4).SetCellValue(vmList[i].Nian);
                    rowtemp.CreateCell(5).SetCellValue("1");
                }
            }
            if (productInfo.VehicleLevel == "五级车型")
            {
                row1.CreateCell(0).SetCellValue("品牌");
                row1.CreateCell(1).SetCellValue("车系");
                row1.CreateCell(2).SetCellValue("VehicleID");
                row1.CreateCell(3).SetCellValue("TID");
                row1.CreateCell(4).SetCellValue("排量");
                row1.CreateCell(5).SetCellValue("年款");
                row1.CreateCell(6).SetCellValue("上市年份");
                row1.CreateCell(7).SetCellValue("停止生产年份");
                row1.CreateCell(8).SetCellValue("版型");
                row1.CreateCell(9).SetCellValue("是否可用");
                for (int i = 0; i < vmList.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(vmList[i].BrandName);
                    rowtemp.CreateCell(1).SetCellValue(vmList[i].VehicleType);
                    rowtemp.CreateCell(2).SetCellValue(vmList[i].VehicleID);
                    rowtemp.CreateCell(3).SetCellValue(vmList[i].TID);
                    rowtemp.CreateCell(4).SetCellValue(vmList[i].PaiLiang);
                    rowtemp.CreateCell(5).SetCellValue(vmList[i].Nian);
                    rowtemp.CreateCell(6).SetCellValue(vmList[i].ListYear);
                    rowtemp.CreateCell(7).SetCellValue(vmList[i].StopYear);
                    rowtemp.CreateCell(8).SetCellValue(vmList[i].SalesName);
                    rowtemp.CreateCell(9).SetCellValue("1");
                }
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 复制产品车型配置到其他产品
        /// </summary>
        /// <param name="sourcePid">源产品ID</param>
        /// <param name="desPids">目的产品ID集合</param>
        /// <param name="isCheckAll">是否全选</param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult CopyProductVehicleConfig(string sourcePid, string desPids, string isCheckAll)
        {
            var productInfo = ipvimgr.GetProductInfoByPid(sourcePid);//可以去掉

            if (isCheckAll == "1")
            {//复制所有
                if (string.IsNullOrEmpty(desPids))
                {
                    return Json(new { msg = "fail" });
                }
                var tempPids = desPids.Replace('\n', ',').Replace("，", ",").TrimEnd(',').Trim(' ');
                var tempArray = tempPids.Split(',');
                var pidSb = new StringBuilder();
                var pidTemp = new StringBuilder();
                foreach (var pid in tempArray)
                {
                    pidSb.AppendFormat("{0},", pid.Trim(' ').Trim('\t'));
                }
                tempPids = pidSb.ToString().TrimEnd(',');
                var searchList = ipvimgr.GetProductInfoByPids(tempPids);
                foreach (var item in searchList)
                {
                    pidTemp.AppendFormat("{0},", item.PID);
                }
                desPids = pidTemp.ToString().TrimEnd(',');
            }

            var success = ipvimgr.InsertProductVehicleTypeConfigInfoByPid(sourcePid, desPids, productInfo.CP_Remark, productInfo.VehicleLevel);
            if (success)
            {
                var destPidArray = desPids.TrimEnd(',').Split(',');

                var logList = new List<ProductVehicleTypeConfigOpLog>();
                foreach (var id in destPidArray)
                {
                    var entity = new ProductVehicleTypeConfigOpLog()
                    {
                        PID = id,
                        Operator = User.Identity.Name,
                        OperateContent = $"复制配置：将产品{sourcePid} 复制到 {id}",
                        OperateTime = DateTime.Now,
                        CreatedTime = DateTime.Now
                    };
                    logList.Add(entity);
                }
                //ipvimgr.BulkSaveOperateLogInfo(dt);
                ipvimgr.BulkSaveOperateLogInfo(logList);

                return Json(new { msg = "success" });
            }
            else
            {
                return Json(new { msg = "fail" });
            }
        }

        #region 数据分组，树形结构车型数据展示
        /// <summary>
        /// 对后台捞出的待展示数据进行层级分组
        /// </summary>
        /// <param name="sourceItems">待分组数据list</param>
        /// <param name="treeItems">分组之后的数据</param>
        /// <param name="groupByField">参与分组的字段</param>
        /// <param name="valueField">前端需要展示的字段</param>
        /// <param name="isRoot">是否是根</param>
        public void GroupByData(List<VehicleTypeInfoVm> sourceItems, List<TreeItem> treeItems, string[] groupByField, string valueField, bool isRoot = false)
        {
            foreach (var item in sourceItems)
            {
                var key = GetFieldValue(item, groupByField);
                var pkey = isRoot ? "" : GetFieldValue(item, groupByField.ToList().Where(x => x != valueField).ToArray());
                var keyItem = treeItems.Where(x => x.id == key);
                if (!keyItem.Any())
                {
                    treeItems.Add(new TreeItem()
                    {
                        id = key,
                        pid = pkey,
                        name = item.GetFieldValues(new[] { valueField }),
                        check = string.IsNullOrEmpty(item.IsChecked) ? "false" : "true",
                        children = new List<TreeItem>()
                    });
                }
            }
        }

        public string GetFieldValue(VehicleTypeInfoDb carItem, string[] fields)
        {
            return carItem.GetFieldValues(fields);
        }

        public void GetTreeData(List<TreeItem> treeData, List<TreeItem> dataList, TreeItem currentItem, string strPid, List<ProductVehicleTypeConfigDb> existConfigList)
        {
            GetRootData(treeData, dataList, strPid, existConfigList);
            treeData.ForEach(item => GetChildData(dataList, item, item.id, existConfigList));
        }

        public void GetRootData(List<TreeItem> treeData, List<TreeItem> dataList, string strPid, List<ProductVehicleTypeConfigDb> existConfigList)
        {
            var pitems = dataList.Where(x => x.pid == strPid);

            pitems.ToList().ForEach(item =>
            {
                var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByBrandName(item.name);//获得该品牌下所有车型信息
                var distinctVehicleList = vehicleInfoList.Distinct(new List_User_DistinctForVehicleID()).ToList();//按VehicleID去重
                var existCount = 0;
                distinctVehicleList.ForEach((tt) =>
                {
                    var vehicleId = tt.VehicleID;
                    var tempCount = existConfigList.FindAll(i => i.VehicleID == vehicleId).Count;//依次从已经配置的列表中拉出对应VehicleID的车型数量
                    existCount += tempCount;
                });
                item.check = vehicleInfoList.Count == existCount ? "true" : "false";
                var pid = item.pid;
                var treeItem = treeData.FirstOrDefault(x => x.id == pid);
                if (treeItem == null)
                {
                    treeData.Add(item);
                }
            });
        }

        public void GetChildData(List<TreeItem> treeItems, TreeItem currentItem, string strPid, List<ProductVehicleTypeConfigDb> existConfigList)
        {
            var level = -1;
            var m = existConfigList.FirstOrDefault();
            if (m != null)
            {
                level = m.ConfigLevel;
            }
            var pitems = treeItems.Where(x => x.pid == strPid);
            var enumerable = pitems as TreeItem[] ?? pitems.ToArray();
            if (enumerable.Any())
            {
                //currentItem是根元素
                var curStr = currentItem.id.Split('$');
                var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByBrandName(curStr[0]);//拿到该品牌下所有车型信息
                //var realContainListedYearList = GetListContainsListedYear(vehicleInfoList);//拿到该品牌下所有车型信息 包含生产年份的数据
                var currentList = new List<VehicleTypeInfoDb>();

                enumerable.ToList().ForEach(item =>
                {
                    if (curStr.Length == 1)//解决车系是否选中
                    {
                        var condition = item.id.Split('$');
                        currentList = vehicleInfoList.FindAll(i => i.Vehicle == condition[1]);//获取当前品牌下某个车系的所有车型信息
                        if (level == 2 || level == 4)
                        {
                            if (currentList.Count > 0)
                            {
                                var itemlist = existConfigList.FindAll(i => i.VehicleID == currentList.First().VehicleID);
                                item.check = currentList.Count <= itemlist.Count ? "true" : "false";
                            }
                        }
                        if (level == 5)
                        {
                            foreach (var entity in currentList)
                            {
                                if (!existConfigList.Exists(i => i.TID == entity.TID))
                                {
                                    item.check = "false";
                                    break;
                                }
                            }
                        }

                    }
                    else if (curStr.Length == 2)//解决排量是否选中
                    {
                        var condition = item.id.Split('$');//当前子节点条件
                        currentList = vehicleInfoList.FindAll(i => i.Vehicle == condition[1] && i.PaiLiang == condition[2]);//第二级
                        if (level == 2 || level == 4)
                        {
                            if (currentList.Count > 0)
                            {
                                var itemList = existConfigList.FindAll(i => i.VehicleID == currentList.First().VehicleID && i.PaiLiang == currentList.First().PaiLiang);
                                item.check = itemList.Count >= currentList.Count ? "true" : "false";
                            }
                        }
                        if (level == 5)
                        {
                            foreach (var entity in currentList)
                            {
                                if (!existConfigList.Exists(i => i.TID == entity.TID))
                                {
                                    item.check = "false";
                                    break;
                                }
                            }
                        }

                    }
                    else if (curStr.Length == 3)//解决生产年份是否选中
                    {
                        var condition = item.id.Split('$');//当前子节点条件
                        currentList = vehicleInfoList.FindAll(i => i.Vehicle == condition[1] && i.PaiLiang == condition[2] && i.Nian == condition[3]);//第三级 2.2T 所有车型
                        if (level == 2 || level == 4)
                        {
                            if (currentList.Count > 0)
                            {
                                var itemList = existConfigList.FindAll(i => i.VehicleID == currentList.First().VehicleID && i.PaiLiang == currentList.First().PaiLiang);
                                foreach (var entity in currentList)
                                {
                                    if (!itemList.Exists(i => i.Nian == entity.Nian))
                                    {
                                        item.check = "false";
                                        break;
                                    }
                                }
                            }
                        }
                        if (level == 5)
                        {
                            foreach (var entity in currentList)
                            {
                                if (!existConfigList.Exists(i => i.TID == entity.TID))
                                {
                                    item.check = "false";
                                    break;
                                }
                            }
                        }


                    }
                    else if (curStr.Length == 4)//解决SaleName是否选中
                    {
                        var condition = item.id.Split('$');//当前子节点条件

                        currentList = vehicleInfoList.FindAll(i => i.Vehicle == condition[1] && i.PaiLiang == condition[2] && i.Nian == condition[3] && i.SalesName == condition[4]);//第四级

                        foreach (var entity in currentList)
                        {
                            if (!existConfigList.Exists(i => i.TID == entity.TID))
                            {
                                item.check = "false";
                                break;
                            }
                        }


                    }
                    currentItem.children.Add(item);
                    GetChildData(treeItems, item, item.id, existConfigList);
                });
            }
        }

        public DataTable UploadData(IWorkbook wb)
        {
            var dt = new DataTable();
            var sheet = wb.GetSheetAt(0);//获取excel第一个sheet
            var headerRow = sheet.GetRow(0);//获取sheet首行

            int cellCount = headerRow.LastCellNum;//获取总列数

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dt.Columns.Add(column);
            }
            //最后一列的标号  即总的行数
            //int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                //HSSFRow row = (HSSFRow)sheet.GetRow(i);
                var row = sheet.GetRow(i);
                var dataRow = dt.NewRow();
                var itemHandle = row.GetCell(cellCount - 1);
                if (itemHandle == null)
                {
                    continue;
                }
                //if (itemHandle.ToString() == "0")//如果标记为0则该行车型无需关联
                //{
                //    continue;
                //}
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;
            return bytes;
        }
        #endregion

        /// <summary>
        /// 获取去重之后需要更新的车型数据，编辑车型数据页调用
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="items"></param>
        /// <param name="pid"></param>
        /// <param name="level"></param>
        /// <param name="existConfigList">该产品已经配置的车型信息</param>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public DataTable GetDtForUpdate(List<VehicleTypeInfoDb> sourceList, List<Condition> items, string pid, string level, List<ProductVehicleTypeConfigDb> existConfigList, ProductInfo productInfo)
        {
            var dt = new DataTable();
            dt.Columns.Add("PID", typeof(string));
            dt.Columns.Add("VehicleID", typeof(string));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("UpdateTime", typeof(DateTime));
            dt.Columns.Add("Nian", typeof(string));
            dt.Columns.Add("PaiLiang", typeof(string));
            dt.Columns.Add("TID", typeof(string));
            dt.Columns.Add("SalesName", typeof(string));
            dt.Columns.Add("ConfigLevel", typeof(int));

            var destList = new List<VehicleTypeInfoDb>();
            foreach (var item in items)
            {
                switch (level)
                {
                    case "二级车型":
                        destList.AddRange(!string.IsNullOrWhiteSpace(item.Vehicle)
                            ? sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle)
                            : sourceList.FindAll(i => i.Brand == item.Brand));
                        break;
                    case "四级车型":
                        if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang) && !string.IsNullOrEmpty(item.Nian))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang && i.Nian == item.Nian));
                        }
                        else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang));
                        }
                        else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle));
                        }
                        else
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand));
                        }

                        break;
                    case "五级车型":
                        if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang) && !string.IsNullOrEmpty(item.Nian) && !string.IsNullOrEmpty(item.SalesName))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang && i.Nian == item.Nian && i.SalesName == item.SalesName));
                        }
                        else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang) && !string.IsNullOrEmpty(item.Nian))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang && i.Nian == item.Nian));
                        }
                        else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang));
                        }
                        else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle))
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle));
                        }
                        else
                        {
                            destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand));
                        }
                        break;
                    default:
                        break;
                }

            }

            //var distinctListN = destList.Distinct(new List_User_DistinctForTID()).ToList();//待更新数据按TID去重防止TID相同生产年份不同的重复数据

            var deleteList = new List<ProductVehicleTypeConfigDb>();//待删除配置列表
            List<VehicleTypeInfoDb> existList;
            //所有已经在配置表中但不在待插入数据表中的数据都进待删除列表
            //五级车型按VehicleID、PaiLiang、Nian、SalesName搜索；四级车型按VehicleID、NIan、PaiLiang；二级车型按VehicleID
            switch (level)
            {
                case "二级车型":
                    foreach (var entity in existConfigList)
                    {
                        existList = destList.FindAll(i => i.VehicleID == entity.VehicleID);
                        if (existList.Count <= 0)
                        {
                            deleteList.Add(entity);
                        }
                    }
                    break;
                case "四级车型":
                    foreach (var entity in existConfigList)
                    {
                        existList =
                            destList.FindAll(
                                i =>
                                    i.VehicleID == entity.VehicleID && i.PaiLiang == entity.PaiLiang &&
                                    i.Nian == entity.Nian);
                        if (existList.Count <= 0)
                        {
                            deleteList.Add(entity);
                        }
                    }
                    break;
                case "五级车型":
                    foreach (var entity in existConfigList) //所有已经在配置表中但不在待插入数据表中的数据都进待删除列表
                    {
                        existList =
                            destList.FindAll(
                                i =>
                                    i.VehicleID == entity.VehicleID && i.PaiLiang == entity.PaiLiang &&
                                    i.Nian == entity.Nian && i.SalesName == entity.SalesName);//destList.FindAll(i => i.TID == entity.TID);
                        if (existList.Count <= 0)
                        {
                            deleteList.Add(entity);
                        }
                        else
                        {
                            destList.RemoveAll(i => i.VehicleID == entity.VehicleID && i.PaiLiang == entity.PaiLiang &&
                                                    i.Nian == entity.Nian && i.SalesName == entity.SalesName);
                        }
                    }
                    break;
            }
            //执行删除配置操作
            if (ipvimgr.DeleteProductVehicleTypeConfigByParams(deleteList))
            {
                var entity = new ProductVehicleTypeConfigOpLog()
                {
                    PID = pid,
                    Operator = User.Identity.Name,
                    OperateContent = string.Format("编辑页删除配置：{0}/{1}", productInfo.DisplayName, productInfo.ProductCode),
                    OperateTime = DateTime.Now,
                    CreatedTime = DateTime.Now,
                };
                ipvimgr.WriteOperatorLog(entity);

            }

            foreach (var itemp in destList)
            {
                var dr = dt.NewRow();
                switch (level)
                {
                    case "二级车型":
                        dr["PID"] = pid;
                        dr["VehicleID"] = itemp.VehicleID;
                        dr["CreatedTime"] = DateTime.Now;
                        dr["UpdateTime"] = DateTime.Now;
                        dr["ConfigLevel"] = 2;
                        //dr["TID"] = itemp.TID;
                        dt.Rows.Add(dr);
                        break;
                    case "四级车型":
                        dr["PID"] = pid;
                        dr["VehicleID"] = itemp.VehicleID;
                        dr["Nian"] = itemp.Nian;
                        dr["PaiLiang"] = itemp.PaiLiang;
                        dr["CreatedTime"] = DateTime.Now;
                        dr["UpdateTime"] = DateTime.Now;
                        dr["ConfigLevel"] = 4;
                        //dr["TID"] = itemp.TID;
                        dt.Rows.Add(dr);
                        break;
                    case "五级车型":
                        dr["PID"] = pid;
                        dr["TID"] = itemp.TID;
                        dr["VehicleID"] = itemp.VehicleID;
                        dr["Nian"] = itemp.Nian;
                        dr["PaiLiang"] = itemp.PaiLiang;
                        dr["SalesName"] = itemp.SalesName;
                        dr["CreatedTime"] = DateTime.Now;
                        dr["UpdateTime"] = DateTime.Now;
                        dr["ConfigLevel"] = 5;
                        dt.Rows.Add(dr);
                        break;
                    default:
                        break;
                }
            }

            return dt;
        }

        [HttpPost]
        public ActionResult GetAllNoImportProduct(int pageIndex = 1, int pageSize = 15)
        {
            var result = ipvimgr.GetAllNoImportProduct(pageIndex, pageSize);

            result = result.Distinct(new ListDistinctForProductInfo()).ToList();//按PID去重

            if (result != null && result.Any())
            {
                return Json(new { items = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { items = new List<ProductInfo>(), count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class VehicleTypeInfoVm : VehicleTypeInfoDb
    {
        public string IsChecked { get; set; }
    }

    public class ProductVehicleTypeConfigInfoVm
    {
        public string PID { get; set; }

        public string TID { get; set; }

        public string VehicleID { get; set; }

        public string Nian { get; set; }

        public string PaiLiang { get; set; }

        public string SalesName { get; set; }

        public int ConfigLevel { get; set; }

        public string BrandName { get; set; }

        public string VehicleType { get; set; }

        public string ListYear { get; set; }

        public string StopYear { get; set; }

    }

    public class ProductVehicleTypeConfigOpLogVm
    {
        public string Operator { get; set; }

        public string OperateTime { get; set; }

        public string OperateContent { get; set; }
    }

    public class ProductInfoViewModel
    {
        public string DisplayName { get; set; }

        public string Pid { get; set; }

        public string Brand { get; set; }

        public string ConfigStatus { get; set; }

        public string ConfigTime { get; set; }
    }

    public class VehicleInfoViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public VehicleInfoViewModel[] Children { get; set; }
    }

    public class QueryCondition
    {
        public string Condition { get; set; }//品牌首字母

        public string Pid { get; set; }//产品ID

        public string VehicleSeries { get; set; }//车系如 德系

        public string Price { get; set; }//价格

        public string VehicleType { get; set; }//车型如 SUV

        public string ArticleID { get; set; }//文章ID
    }

    public class LogCondition
    {
        public string TimeStart { get; set; }

        public string TimeEnd { get; set; }
    }


    [Serializable]
    public class TreeItem
    {
        public string id { get; set; }

        public string pid { get; set; }

        public string name { get; set; }

        public List<TreeItem> children { get; set; }

        public string tid { get; set; }

        public string check { get; set; }
    }

    public class Condition
    {
        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public string PaiLiang { get; set; }

        public string Nian { get; set; }

        public string SalesName { get; set; }
    }

    public class List_User_DistinctForVehicleID : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            if (x.VehicleID == y.VehicleID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }
    public class List_User_DistinctForTID : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            if (x.TID == y.TID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }

    public class ListDistinctForSelectLevel4 : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            if (x.VehicleID == y.VehicleID && x.Nian == y.Nian && x.PaiLiang == y.PaiLiang)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }

    public class List_User_DistinctBy_TID : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            if (x.TID == y.TID && x.ListedYear == y.ListedYear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }

    public class ListUserDistinctByLevel5 : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            return x.VehicleID == y.VehicleID && x.PaiLiang == y.PaiLiang && x.Nian == y.Nian && x.SalesName == y.SalesName;
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }

    public class ListVehicleTypeConfigDistinctByLevel5 : IEqualityComparer<ProductVehicleTypeConfigDb>
    {
        public bool Equals(ProductVehicleTypeConfigDb x, ProductVehicleTypeConfigDb y)
        {
            return x.VehicleID == y.VehicleID && x.PaiLiang == y.PaiLiang && x.Nian == y.Nian && x.SalesName == y.SalesName;
        }

        public int GetHashCode(ProductVehicleTypeConfigDb obj)
        {
            return 0;
        }
    }

    public class ListUserDistinctByLevel4 : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            if (x.VehicleID == y.VehicleID && x.Nian == y.Nian && x.PaiLiang == y.PaiLiang)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }

    public class ListUserDistinctByLevel2 : IEqualityComparer<VehicleTypeInfoDb>
    {
        public bool Equals(VehicleTypeInfoDb x, VehicleTypeInfoDb y)
        {
            if (x.VehicleID == y.VehicleID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleTypeInfoDb obj)
        {
            return 0;
        }
    }

    public class ListDistinctByVehicleId : IEqualityComparer<VehicleInfoExDb>
    {
        public bool Equals(VehicleInfoExDb x, VehicleInfoExDb y)
        {
            if (x.VehicleID == y.VehicleID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleInfoExDb obj)
        {
            return 0;
        }
    }

    public class ListDistinctForLevel4 : IEqualityComparer<VehicleInfoExDb>
    {
        public bool Equals(VehicleInfoExDb x, VehicleInfoExDb y)
        {
            if (x.VehicleID == y.VehicleID && x.Nian == y.Nian && x.PaiLiang == y.PaiLiang)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VehicleInfoExDb obj)
        {
            return 0;
        }
    }

    public class ListDistinctForLevel5 : IEqualityComparer<VehicleInfoExDb>
    {
        public bool Equals(VehicleInfoExDb x, VehicleInfoExDb y)
        {
            if (!string.IsNullOrWhiteSpace(x.TID) && !string.IsNullOrWhiteSpace(y.TID))
            {
                return x.TID == y.TID;//如果TID都不为空，则直接按TID去重
            }
            return x.VehicleID == y.VehicleID && x.PaiLiang == y.PaiLiang && x.Nian == y.Nian && x.SalesName == y.SalesName;
        }

        public int GetHashCode(VehicleInfoExDb obj)
        {
            return 0;
        }
    }

    public class ListDistinctForProductInfo : IEqualityComparer<ProductInfo>
    {
        public bool Equals(ProductInfo x, ProductInfo y)
        {
            if (x.PID == y.PID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(ProductInfo obj)
        {
            return 0;
        }
    }
}

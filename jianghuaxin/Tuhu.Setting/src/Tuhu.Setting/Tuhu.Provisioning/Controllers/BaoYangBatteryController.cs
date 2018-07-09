using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.DataAccess.Entity.Battery;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangBatteryController : Controller
    {
        private BatteryManager _batteryManager;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _batteryManager = new BatteryManager(User.Identity.Name);
        }

        #region 公共的

        public JsonResult GetBatteryBrands()
        {
            var manager = new BaoYangManager();
            var result = _batteryManager.GetBatteryBrands();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 保养流程蓄电池配置

        private const string OnlineChannel = "online";
        private const string UShopChannel = "u门店";

        /// <summary>
        /// 保养流程的蓄电池覆盖区域
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBaoYangBatteryCoverAreas(SearchBaoYangBatteryCoverAreaRequest request)
        {
            var result = _batteryManager.GetBaoYangBatteryCoverAreaList(request);
            return Json(new
            {
                status = result != null,
                data = result,
                total = result?.FirstOrDefault()?.Total ?? 0,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ActionResult AddOrUpdateBaoYangBatteryCoverArea(BaoYangBatteryCoverArea item)
        {
            Func<string> validFunc = () =>
            {
                var msg = string.Empty;
                if (item == null || string.IsNullOrWhiteSpace(item.Brand) || item.CityId <= 0 || item.ProvinceId <= 0)
                {
                    msg = "参数不能为空";
                }
                if (_batteryManager.ExistsBaoYangBatteryCoverArea(item))
                {
                    msg = "已存在相同数据";
                }
                var channels = item.Channels?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList() ?? new List<string>();
                var tmplChannels = new List<string> { UShopChannel, OnlineChannel };
                if (!channels.All(channel => tmplChannels.Contains(channel)))
                {
                    msg = "渠道不是有效值";
                }
                item.Channels = string.Join(",", channels);
                return msg;
            };
            var validResult = validFunc();
            if (!string.IsNullOrEmpty(validResult))
            {
                return Json(new { status = false, msg = validResult });
            }
            var success = item.PKID > 0 ? _batteryManager.UpdateBaoYangBatteryCoverArea(item) :
                _batteryManager.AddBaoYangBatteryCoverArea(item);
            return Json(new { status = success });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteBaoYangBatteryCoverArea(Int64 id = 0)
        {
            var result = id > 0 ? _batteryManager.DeleteBaoYangBatteryCoverArea(id) : false;
            return Json(new { status = result });
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportBaoYangBatteryCoverArea()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var index = 0;
            var row = sheet.CreateRow(index++);

            var num = 0;
            row.CreateCell(num++).SetCellValue("品牌");
            row.CreateCell(num++).SetCellValue("省份");
            row.CreateCell(num++).SetCellValue("城市");
            row.CreateCell(num++).SetCellValue("线上展示");
            row.CreateCell(num++).SetCellValue("线下展示");
            row.CreateCell(num++).SetCellValue("是否隐藏");

            num = 0;
            sheet.SetColumnWidth(num++, 8 * 256);
            sheet.SetColumnWidth(num++, 8 * 256);
            sheet.SetColumnWidth(num++, 8 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);

            var list = _batteryManager.GetAllBaoYangBatteryCoverArea();
            foreach (var item in list)
            {
                row = sheet.CreateRow(index++);
                num = 0;
                var online = item.Channels?.IndexOf(OnlineChannel) ?? -1;
                var uShop = item.Channels?.IndexOf(UShopChannel) ?? -1;
                row.CreateCell(num++).SetCellValue(item.Brand);
                row.CreateCell(num++).SetCellValue(item.ProvinceName);
                row.CreateCell(num++).SetCellValue(item.CityName);
                row.CreateCell(num++).SetCellValue(online > -1 ? "是" : "否");
                row.CreateCell(num++).SetCellValue(uShop > -1 ? "是" : "否");
                row.CreateCell(num++).SetCellValue(item.IsEnabled ? "否" : "是");
            }

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"保养流程蓄电池品牌覆盖区域结果-{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadBaoYangBatteryTmpl()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);

            var num = 0;
            row.CreateCell(num++).SetCellValue("品牌");
            row.CreateCell(num++).SetCellValue("省份");
            row.CreateCell(num++).SetCellValue("城市");
            row.CreateCell(num++).SetCellValue("线上展示");
            row.CreateCell(num++).SetCellValue("线下展示");
            row.CreateCell(num++).SetCellValue("是否隐藏");

            num = 0;
            sheet.SetColumnWidth(num++, 8 * 256);
            sheet.SetColumnWidth(num++, 8 * 256);
            sheet.SetColumnWidth(num++, 8 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"保养流程蓄电池品牌覆盖区域模板-{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportBaoYangBatteryCoverArea()
        {
            var list = new List<BaoYangBatteryCoverArea>();

            #region 验证文件

            var files = Request.Files;
            if (files == null || files.Count <= 0)
            {
                return Json(new { status = false, msg = "请先选择文件上传" });
            }
            var file = files[0];
            if (file.ContentLength > 200 * 1024)
            {
                return Json(new { Status = false, Msg = "文件大小不得超过200KB(不同品牌分批上传)" });
            }
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { status = false, msg = "文件格式不正确, 请上传Excel文件" });
            }

            #endregion

            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
            var sheet = workBook.GetSheetAt(0);


            #region 读取验证excel信息

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var cellNum = titleRow.FirstCellNum;
            if (titleRow.GetCell(cellNum++)?.StringCellValue != "品牌" ||
                titleRow.GetCell(cellNum++)?.StringCellValue != "省份" ||
                titleRow.GetCell(cellNum++)?.StringCellValue != "城市" ||
                titleRow.GetCell(cellNum++)?.StringCellValue != "线上展示" ||
                titleRow.GetCell(cellNum++)?.StringCellValue != "线下展示" ||
                titleRow.GetCell(cellNum++)?.StringCellValue != "是否隐藏")
            {
                return Json(new { status = false, msg = "导入模板不正确，请用正确的模板导入!" });
            }

            Func<ICell, string> getStringValueFunc = cell =>
            {
                if (cell != null)
                {
                    if (cell.CellType == CellType.Numeric)
                    {
                        return DateUtil.IsCellDateFormatted(cell) ?
                            cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                            cell.NumericCellValue.ToString();
                    }
                    return cell.StringCellValue?.Trim();
                }
                return null;
            };

            Func<string, bool> getBooleanFunc = input =>
            {
                return "1".Equals(input) || "是".Equals(input) || "true".Equals(input, StringComparison.OrdinalIgnoreCase);
            };

            var msgs = new List<string>();
            for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    var cellIndex = row.FirstCellNum;
                    var brand = getStringValueFunc(row.GetCell(cellIndex++));
                    var province = getStringValueFunc(row.GetCell(cellIndex++));
                    var city = getStringValueFunc(row.GetCell(cellIndex++));
                    var online = getStringValueFunc(row.GetCell(cellIndex++));
                    var uShop = getStringValueFunc(row.GetCell(cellIndex++));
                    var disabled = getStringValueFunc(row.GetCell(cellIndex++));
                    if (!string.IsNullOrEmpty(brand) && !string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(province))
                    {
                        var onlineChannel = getBooleanFunc(online) ? OnlineChannel : string.Empty;
                        var uShopChannel = getBooleanFunc(uShop) ? UShopChannel : string.Empty;
                        var channels = $"{OnlineChannel},{uShopChannel}".Trim(',');
                        var item = new BaoYangBatteryCoverArea
                        {
                            PKID = rowIndex,
                            Brand = brand,
                            ProvinceName = province,
                            CityName = city,
                            IsEnabled = !getBooleanFunc(disabled),
                            Channels = channels,
                        };
                        var existsItem = list.FirstOrDefault(x =>
                            x.Brand == item.Brand &&
                            x.ProvinceName == item.ProvinceName &&
                            x.CityName == item.CityName
                        );
                        if (existsItem != null)
                        {
                            msgs.Add($"第{existsItem.PKID}和{item.PKID}行数据重复");
                        }
                        else
                        {
                            list.Add(item);
                        }
                    }
                }
            }

            if (!list.Any())
            {
                return Json(new { status = false, msg = "导入数据不能为空!" });
            }

            if (msgs.Any())
            {
                return Json(new { status = false, msg = string.Join(Environment.NewLine, msgs) });
            }

            #endregion

            #region 填充核对地区信息

            var provinceNames = list.Select(x => x.ProvinceName).Distinct().ToList();
            var regions = provinceNames.Select(p => _batteryManager.GetRegionByRegionName(p)).ToList();

            list.ForEach(x =>
            {
                var region = regions.FirstOrDefault(r => r.ProvinceName == x.ProvinceName);
                if (region != null)
                {
                    //直辖市
                    var city = region.IsMunicipality ? region : region.ChildRegions?.FirstOrDefault(cr => cr.CityName == x.CityName);
                    if (city != null && city.RegionName == x.CityName)
                    {
                        x.CityId = city.RegionId;
                        x.ProvinceId = city.ProvinceId;
                    }
                }
            });

            var indexs = list.Where(x => x.ProvinceId <= 0 || x.CityId <= 0).Select(x => x.PKID).ToList();
            if (indexs.Any())
            {
                return Json(new { status = false, msg = $"第{string.Join(",", indexs)}行地区信息有误(直辖市只支持到市), 请核对" });
            }

            #endregion

            #region 蓄电池品牌验证

            var byManager = new BaoYangManager();
            var brands = _batteryManager.GetBatteryBrands();
            var ids = list.Where(x => !brands.Contains(x.Brand)).Select(x => x.PKID);
            if (ids.Any())
            {
                return Json(new { status = false, msg = $"第{string.Join(",", ids)}行蓄电池品牌不合法" });
            }

            #endregion

            bool result = _batteryManager.BatchUpdateBaoYangBatteryCoverArea(list);
            return Json(new { status = result });
        }

        #endregion
    }
}
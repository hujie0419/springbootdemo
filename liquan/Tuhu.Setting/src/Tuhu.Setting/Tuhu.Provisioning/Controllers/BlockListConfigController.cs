using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ConfigLog;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Tuhu.Provisioning.Controllers
{
    public class BlockListConfigController : Controller
    {
        #region utility

        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        public static readonly Dictionary<string, int> TypeName2TypeId = new Dictionary<string, int>
        {
            { "IP地址", 1 },
            { "用户手机号", 2 },
            { "设备号", 4 },
            { "用户Id", 8 },
            { "支付账号", 16 }
        };

        #endregion utility

        /// <summary>
        /// ConfigLogManager
        /// </summary>
        private static readonly CommonConfigLogManager ConfigLogManager = new CommonConfigLogManager();

        [HttpGet]
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult List(string blockSystem, int blockType, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0 || pageSize > 500)
            {
                pageSize = 20;
            }
            using (var client = new BlockListConfigClient())
            {
                var result = client.SelectPagedBlockList(blockSystem, blockType, pageIndex, pageSize);
                if (result.Success)
                {
                    return Content(JsonConvert.SerializeObject(new PagedDataModel<BlockListItem>
                    {
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalSize = result.Result.Item1,
                        Data = result.Result.Item2.ToArray(),
                        Status = 1
                    }, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Add(BlockListItem item)
        {
            using (var client = new BlockListConfigClient())
            {
                item.UpdateBy = User.Identity.Name;

                var result = client.AddBlockListItem(item);
                if (result.Success)
                {
                    if (result.Result)
                    {
                        ConfigLogManager.AddCommonConfigLogInfo(new CommonConfigLogModel
                        {
                            ObjectId = $"{item.BlockType}-{item.BlockValue}",
                            AfterValue = JsonConvert.SerializeObject(item),
                            ObjectType = $"{item.BlockSystem}BlockListLog",
                            Creator = User.Identity.Name,
                            Remark = $"新增黑名单{(BlockListType)item.BlockType}-{item.BlockValue}"
                        });
                    }
                    return Json(new
                    {
                        Status = 1,
                        Success = result.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(BlockListItem item)
        {
            using (var client = new BlockListConfigClient())
            {
                item.UpdateBy = User.Identity.Name;

                var result = client.DeleteBlockListItem(item);
                if (result.Success)
                {
                    if (result.Result)
                    {
                        ConfigLogManager.AddCommonConfigLogInfo(new CommonConfigLogModel
                        {
                            ObjectId = $"{item.BlockType}-{item.BlockValue}",
                            BeforeValue = JsonConvert.SerializeObject(item),
                            ObjectType = $"{item.BlockSystem}BlockListLog",
                            Creator = User.Identity.Name,
                            Remark = $"删除黑名单{(BlockListType)item.BlockType}-{item.BlockValue}"
                        });
                    }
                    return Json(new
                    {
                        Status = 1,
                        Success = result.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// 根据pkid删除黑名单
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteById(BlockListItem item)
        {
            using (var client = new BlockListConfigClient())
            {
                var blackModel = new BlockListItem
                {
                    PKID = item.PKID,
                    UpdateBy = User.Identity.Name
                };

                var result = client.DeleteBlockListItemByPkid(blackModel);
                if (result.Success)
                {
                    if (result.Result)
                    {
                        ConfigLogManager.AddCommonConfigLogInfo(new CommonConfigLogModel
                        {
                            ObjectId = $"{item.BlockType}-{item.BlockValue}",
                            BeforeValue = JsonConvert.SerializeObject(item),
                            ObjectType = $"{item.BlockSystem}BlockListLog",
                            Creator = User.Identity.Name,
                            Remark = $"删除黑名单{(BlockListType)item.BlockType}-{item.BlockValue}"
                        });
                    }
                    return Json(new
                    {
                        Status = 1,
                        Success = result.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// 根据系统和黑名单值查询黑名单列表
        /// </summary>
        /// <param name="blockSystem">系统名称</param>
        /// <param name="blockValue">黑名单值</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SearchBlockList(string blockSystem, string blockValue, int pageIndex = 1, int pageSize = 20)
        {
            var configService = new BlockListConfigService();
            var result = await configService.SearchPagedBlockList(blockSystem, blockValue, pageIndex, pageSize);

            return Content(JsonConvert.SerializeObject(new PagedDataModel<BlockListItem>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalSize = result.Pager.Total,
                Data = result.Source?.ToArray(),
                Status = 1
            },
            DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 下载拼团黑名单Excel模板
        /// </summary>
        /// <returns></returns>
        public FileResult DownloadTemplate(string blockSystem)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);

            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("黑名单类型");
            row.CreateCell(cellNum++).SetCellValue("黑名单值");
            row.CreateCell(cellNum++).SetCellValue("黑名单开始时间");
            row.CreateCell(cellNum++).SetCellValue("黑名单结束时间");
            row.CreateCell(cellNum++).SetCellValue("拉黑原因");
            row.CreateCell(cellNum++).SetCellValue("备注");

            for (var i = 0; i < cellNum; i++)
            {
                sheet.SetColumnWidth(i, 18 * 256);
            }

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{blockSystem}黑名单模板 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 拼团黑名单导入Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ImportExcel(string blockSystem)
        {
            var files = Request.Files;
            if (files == null || files.Count <= 0)
                return Json(new { code = 1, status = false, msg = "请先选择文件上传" });

            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return Json(new { code = 1, status = false, msg = "文件格式不正确, 请上传Excel文件" });

            var blockDict = new Dictionary<string, BlockListItem>();
            using (var stream = file.InputStream)
            {
                // 读取Excel表格数据
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);

                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        var blockTypeName = GetCellStringValue(row.GetCell(0));
                        if (string.IsNullOrEmpty(blockTypeName) || !TypeName2TypeId.ContainsKey(blockTypeName))
                            return Json(new { code = 1, status = false, msg = $"第{rowIndex + 1}行黑名单类型不正确" });

                        var tempOutParameter = new Guid();
                        var blockValue = GetCellStringValue(row.GetCell(1));
                        if (blockTypeName.Equals("用户Id") && !Guid.TryParse(blockValue, out tempOutParameter))
                            return Json(new { code = 1, status = false, msg = $"第{rowIndex + 1}行用户Id应为Guid格式" });

                        try
                        {
                            // 避免导入重复记录
                            if (!blockDict.ContainsKey(blockValue))
                            {
                                blockDict[blockValue] = new BlockListItem
                                {
                                    BlockSystem = blockSystem,
                                    BlockType = GetTypeIdByTypeName(blockTypeName),
                                    BlockValue = blockValue,
                                    BlockBeginTime = row.GetCell(2)?.DateCellValue,
                                    BlockEndTime = row.GetCell(3)?.DateCellValue,
                                    Reason = GetCellStringValue(row.GetCell(4)),
                                    Remark = GetCellStringValue(row.GetCell(5)),
                                    UpdateBy = User.Identity.Name
                                };
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            return Json(new { code = 1, status = false, msg = $"第{rowIndex + 1}行时间格式不正确" });
                        }
                    }
                }
            }

            if (blockDict.Any())
            {
                // 将黑名单列表入库
                var configService = new BlockListConfigService();
                foreach (var item in blockDict)
                    await configService.AddBlockListItem(item.Value);

                return Json(new { code = 1, status = true });
            }

            return Json(new { code = 1, status = false, msg = "导入数据不能为空" });
        }

        /// <summary>
        /// 通过类型名称获取类型Id
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private int GetTypeIdByTypeName(string typeName)
        {
            return TypeName2TypeId[typeName];
        }

        /// <summary>
        /// 获取Excel单元格数据
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private string GetCellStringValue(ICell cell)
        {
            if (cell != null)
            {
                if (cell.CellType == CellType.Numeric)
                {
                    return DateUtil.IsCellDateFormatted(cell) ?
                        cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                        cell.NumericCellValue.ToString();
                }
                return cell.StringCellValue;
            }
            return null;
        }
    }
}
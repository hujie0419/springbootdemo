using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.PaintDiscountConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PaintDiscountConfigController : Controller
    {

        #region 喷漆打折详情配置

        /// <summary>
        /// 获取所有服务
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllPaintDiscountService()
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetAllPaintDiscountService();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加喷漆打折配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPaintConfig(PaintDiscountConfigModel model)
        {
            if (model == null)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ServicePid))
            {
                return Json(new { Status = false, Msg = "请选择服务Pid" }, JsonRequestBehavior.AllowGet);
            }
            if (model.SurfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "面数须为正整数" }, JsonRequestBehavior.AllowGet);
            }
            if (!(model.ActivityPrice > 0))
            {
                return Json(new { Status = false, Msg = "活动价格须大于0" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityName))
            {
                return Json(new { Status = false, Msg = "权益名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityImage))
            {
                return Json(new { Status = false, Msg = "活动图片不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var user = User.Identity.Name;
            var isExist = manager.IsExistPaintDiscountConfig(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddPaintDiscountConfig(model, user);
            return Json(new { Status = result, Msg = $"添加{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePaintConfig(PaintDiscountConfigModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ServicePid) || model.SurfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var user = User.Identity.Name;
            var result = manager.DeletePaintDiscountConfig(model.PackageId, model.ServicePid, model.SurfaceCount, user);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        /// <summary>
        /// 更新喷漆打折配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdatePaintConfig(PaintDiscountConfigModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ServicePid) || model.SurfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (!(model.ActivityPrice > 0))
            {
                return Json(new { Status = false, Msg = "活动价格须大于0" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityName))
            {
                return Json(new { Status = false, Msg = "权益名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityImage))
            {
                return Json(new { Status = false, Msg = "活动图片不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var isExist = manager.IsExistPaintDiscountConfig(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复编辑" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.UpdatePaintDiscountConfig(model, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        /// <summary>
        /// 批量更新喷漆打折配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public ActionResult MultUpdatePaintConfig(List<PaintDiscountConfigModel> models, string activityImage)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(activityImage))
            {
                return Json(new { Status = false, Msg = "活动图片不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            foreach (var model in models)
            {
                if (string.IsNullOrWhiteSpace(model.ServicePid) || model.SurfaceCount < 1)
                {
                    return Json(new { Status = false, Msg = "所有记录的PID不能为空和面数须大于0" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.ActivityImage = activityImage;
                }
            }
            var user = User.Identity.Name;
            var result = manager.MultUpdatePaintConfig(models, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPaintConfig(string servicePid, int packageId = 0, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.SelectPaintDiscountConfig(packageId, servicePid, pageIndex, pageSize);
            return Json(new { Status = result != null && result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadExcel(int packageId = 0)
        {
            var files = Request.Files;
            if (files.Count < 1)
            {
                return Json(new { Status = false, Msg = "请选择文件" }, JsonRequestBehavior.AllowGet);
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" }, JsonRequestBehavior.AllowGet);
            }
            var convertResult = ConvertExcelToList(file);
            if (!string.IsNullOrEmpty(convertResult.Item2))
            {
                return Json(new { Status = false, Msg = convertResult.Item2 }, JsonRequestBehavior.AllowGet);
            }
            else if (convertResult.Item1 == null || !convertResult.Item1.Any())
            {
                return Json(new { Status = false, Msg = "Excel内容为空" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var manager = new PaintDiscountConfigManager();
                if (packageId > 0)
                {
                    var package = manager.GetPaintDiscountPackage(packageId);
                    if (package == null)
                    {
                        return Json(new { Status = false, Msg = "未知的喷漆打折价格体系" }, JsonRequestBehavior.AllowGet);
                    }
                }
                var user = User.Identity.Name;
                convertResult.Item1.ForEach(s => s.PackageId = packageId);
                var result = manager.UploadPaintDiscountConfig(convertResult.Item1, user);
                return Json(new { Status = result, Msg = $"导入{(result ? "成功" : "失败")}" }
                        , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 导出数据至Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcel(int packageId = 0)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("PID");
            row.CreateCell(cellNum++).SetCellValue("面数");
            row.CreateCell(cellNum++).SetCellValue("活动价");
            row.CreateCell(cellNum++).SetCellValue("权益名称");
            row.CreateCell(cellNum++).SetCellValue("活动说明");
            row.CreateCell(cellNum++).SetCellValue("活动图片");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            var manager = new PaintDiscountConfigManager();
            var packageName = string.Empty;
            if (packageId > 0)
            {
                var package = manager.GetPaintDiscountPackage(packageId);
                if (package == null)
                {
                    return Json(new { Status = false, Msg = "未知的喷漆打折价格体系" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    packageName = package.PackageName;
                }
            }
            var result = manager.GetPaintDiscountDetailByPackageId(packageId);
            if (result != null && result.Any())
            {
                int modelRowCount = 1;
                foreach (var model in result)
                {
                    int modelCol = 0;
                    var modelRow = sheet.CreateRow(modelRowCount);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ServicePid);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.SurfaceCount);
                    modelRow.CreateCell(modelCol++).SetCellValue((double)model.ActivityPrice);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ActivityName);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ActivityExplain);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ActivityImage);
                    modelRowCount++;
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls",
                $"喷漆打折价格体系{packageName}的服务价格配置 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #region 查看日志
        /// <summary>
        /// 获取喷漆打折操作日志
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPaintDiscountOprLog(string servicePid, int surfaceCount, int packageId = 0)
        {
            if (string.IsNullOrEmpty(servicePid) || surfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var logType = "PaintDiscountConfig";
            var identityID = $"{packageId}_{servicePid}_{surfaceCount}";
            var manager = new PaintDiscountConfigManager();
            var result = manager.SelectPaintDiscountOprLog(logType, identityID);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// Excel转为List<PaintDiscountConfigModel>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Tuple<List<PaintDiscountConfigModel>, string> ConvertExcelToList(HttpPostedFileBase file)
        {
            var message = string.Empty;
            var result = null as List<PaintDiscountConfigModel>;
            try
            {
                var stream = file.InputStream;
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
                var temp = ConvertExcelToList(sheet);
                result = temp.Item1;
                message = temp.Item2;
                if (!string.IsNullOrEmpty(message))
                {
                    return Tuple.Create(result, message);
                }
                if (!result.Any())
                {
                    return Tuple.Create(result, "Excel内容为空");
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// sheet=>List<PaintDiscountConfigModel>
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static Tuple<List<PaintDiscountConfigModel>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<PaintDiscountConfigModel>();
            var message = string.Empty;

            Func<ICell, string> getStringValue = cell =>
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
            };

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (titleRow.GetCell(index++)?.StringCellValue == "PID" &&
                titleRow.GetCell(index++)?.StringCellValue == "面数" &&
                titleRow.GetCell(index++)?.StringCellValue == "活动价" &&
                titleRow.GetCell(index++)?.StringCellValue == "权益名称" &&
                titleRow.GetCell(index++)?.StringCellValue == "活动说明" &&
                titleRow.GetCell(index)?.StringCellValue == "活动图片")
            {
                var nowTime = DateTime.Now;
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var pid = getStringValue(row.GetCell(cellIndex++));
                    var surfaceCountStr = getStringValue(row.GetCell(cellIndex++));
                    var activityPriceStr = getStringValue(row.GetCell(cellIndex++));
                    var activityName = getStringValue(row.GetCell(cellIndex++));
                    var activityExplain = getStringValue(row.GetCell(cellIndex++));

                    int surfaceCount;
                    surfaceCount = int.TryParse(surfaceCountStr, out surfaceCount) ? surfaceCount : 0;

                    decimal activityPrice;
                    activityPrice = decimal.TryParse(activityPriceStr, out activityPrice) ? activityPrice : 0;

                    if (!string.IsNullOrWhiteSpace(pid) && surfaceCount > 0
                        && activityPrice > 0 && !string.IsNullOrWhiteSpace(activityName))
                    {
                        result.Add(new PaintDiscountConfigModel()
                        {
                            ServicePid = pid,
                            SurfaceCount = surfaceCount,
                            ActivityPrice = activityPrice,
                            ActivityName = activityName,
                            ActivityExplain = activityExplain ?? string.Empty,
                            ActivityImage = string.Empty
                        });
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行格式不正确," +
                            $" 必须价格体系Id大于0,填写PID、面数为正整数、活动价大于0、权益名称不能为空";
                        break;
                    }
                }
            }
            else
            {
                message = "与导入模板不一致，请下载模板之后根据模板填写";
            }
            return Tuple.Create(result, message);
        }

        #endregion

        #region 喷漆打折价格体系配置

        /// <summary>
        /// 新增价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddPackageConfig(PaintDiscountPackageModel model)
        {
            if (model == null)
            {
                return Json(new { Status = false, Msg = "未知的添加对象" }, JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrWhiteSpace(model.PackageName))
            {
                return Json(new { Status = false, Msg = "活动名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            else if (!Enum.IsDefined(typeof(UserType), model.UserType))
            {
                return Json(new { Status = false, Msg = "未定义的用户类型" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var isExist = manager.IsExistPaintDiscountPackage(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "活动已存在，不能重复编辑" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.AddPaintDiscountPackage(model, user);
            return Json(new { Status = result > 0, Msg = $"添加{(result > 0 ? "成功" : "失败")}", PackageId = result }
            , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult DeletePackageConfig(PaintDiscountPackageModel model)
        {
            if (model.PKID < 1 || string.IsNullOrWhiteSpace(model.PackageName))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var user = User.Identity.Name;
            var result = manager.DeletePaintDiscountPackage(model, user);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdatePackageConfig(PaintDiscountPackageModel model)
        {
            if (model == null || model.PKID < 1)
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.PackageName))
            {
                return Json(new { Status = false, Msg = "活动名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var isExist = manager.IsExistPaintDiscountPackage(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复编辑" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.UpdatePaintDiscountPackage(model, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询价格体系配置
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPackageConfigForView(int packageId, int userType, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.SelectPaintDiscountPackageForView(packageId, userType, pageIndex, pageSize);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取喷漆打折价格体系
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public ActionResult GetPackageConfig(int packageId)
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetPaintDiscountPackage(packageId);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有价格体系配置
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllPaintDiscountPackage()
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetAllPaintDiscountPackage();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 喷漆打折城市配置

        /// <summary>
        /// 获取所有二级城市
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllRegions()
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetAllRegion();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑城市门店配置
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="regionShops"></param>
        /// <returns></returns>
        public ActionResult UpsertPackageRegion(int packageId, string regionShops)
        {
            if (packageId < 0)
            {
                return Json(new { Status = false, Msg = "未知的喷漆打折价格体系" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(regionShops))
            {
                return Json(new { Status = false, Msg = "请配置门店和城市" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var package = manager.GetPaintDiscountPackage(packageId);
            if (package == null)
            {
                return Json(new { Status = false, Msg = "未知的喷漆打折价格体系" }, JsonRequestBehavior.AllowGet);
            }
            var regionShopPair = null as List<RegionShopPairModel>;
            try
            {
                regionShopPair = JsonConvert.DeserializeObject<List<RegionShopPairModel>>(regionShops);
            }
            catch (Exception)
            {
                regionShopPair = null;
            }
            if (regionShopPair != null && regionShopPair.Any())
            {
                if (regionShopPair.Any(s => (s.RegionId < 1 && s.ShopIds != null && s.ShopIds.Any())
                || (s.ShopIds != null && s.ShopIds.Any(v => v < 1))))
                {
                    return Json(new { Status = false, Msg = $"未知的门店" }, JsonRequestBehavior.AllowGet);
                }
                var list = manager.ConvertToPackageRegionModel(packageId, regionShopPair);
                var exist = manager.GetRepeatPackageRegion(packageId, package.UserType, list);
                if (exist != null && exist.Any())
                {
                    return Json(new { Status = false, Msg = string.Join("</br>", exist) }, JsonRequestBehavior.AllowGet);
                }
                var result = manager.UpsertPackageRegion(packageId, package.UserType, list, User.Identity.Name);
                return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = $"请配置门店和城市" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取该城市下所有喷漆门店
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public ActionResult GetPaintShopsByRegionId(int regionId)
        {
            if (regionId < 1)
            {
                return Json(new { Status = false, Msg = "未知的城市" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetAllPaintShopsByRegionId(regionId);
            return Json(new { Status = result.Any(), Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取该价格体系的城市门店展示
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public ActionResult GetPackageRegionForView(int packageId)
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetPackageRegionForView(packageId);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 刷新缓存

        /// <summary>
        /// 移除喷漆打折城市配置缓存
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        public ActionResult RemovePackageRegionCache(int packageId, List<int> regionIds)
        {
            var result = false;
            if (packageId < 1 || regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var package = manager.GetPaintDiscountPackage(packageId);
            var userTypes = new List<bool>();
            switch (package?.UserType)
            {
                case (int)UserType.AllUser:
                    userTypes.AddRange(new List<bool>() { true, false }); break;
                case (int)UserType.NewUser:
                    userTypes.Add(true); break;
                case (int)UserType.OldUser:
                    userTypes.Add(false); break;
                default: break;
            }
            if (userTypes.Any())
            {
                result = regionIds.All(regionId => userTypes.All(isnew =>
                {
                    var cacheKey = $"PaintDiscountPackageRegion/{regionId}/{isnew}";
                    return manager.RemovePaintRedisCache(cacheKey);
                }));
            }
            return Json(new { Status = result, Msg = $"刷新缓存{(result ? "成功" : "失败")}" }
            , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 移除喷漆打折详情缓存
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="servicePid"></param>
        /// <returns></returns>
        public ActionResult RemovePaintDiscountDetailCache(int packageId, string servicePid = "")
        {
            var manager = new PaintDiscountConfigManager();
            var result = false;
            if (string.IsNullOrWhiteSpace(servicePid))
            {
                result = manager.RefreshPaintDiscountConfigCache(packageId);
            }
            else
            {
                var cacheKey = $"PaintDiscountPackageDetail/{packageId}/{servicePid}";
                result = manager.RemovePaintRedisCache(cacheKey);
            }
            return Json(new { Status = result, Msg = $"刷新缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新喷漆打折旧配置的服务缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshPaintDiscountPaintCache()
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.RefreshPaintDiscountConfigCache();
            return Json(new { Status = result, Msg = $"刷新缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
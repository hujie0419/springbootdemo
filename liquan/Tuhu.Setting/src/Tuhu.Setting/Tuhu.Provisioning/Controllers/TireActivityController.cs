using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.TireActivity;
using Tuhu.Provisioning.DataAccess.Entity.TireActivity;
using Tuhu.Service.ConfigLog;
using Newtonsoft.Json;
using System.Threading;
using System.Text.RegularExpressions;

namespace Tuhu.Provisioning.Controllers
{
    public class TireActivityController : Controller
    {
        private readonly Lazy<TireActivityManage> lazyTireActivityManager = new Lazy<TireActivityManage>();
        private TireActivityManage TireActivityManager
        {
            get { return this.lazyTireActivityManager.Value; }
        }

        #region 小保养套餐优惠价格

        /// <summary>
        /// 获取小保养套餐优惠价格列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult GetMaintenancePackageOnSaleList(int pageSize = 20, int pageIndex = 1)
        {
            int recordCount = 0;
            var list=TireActivityManager.GetMaintenancePackageOnSaleList(out recordCount,pageSize, pageIndex);
            return Json(new { data = list, dataCount = recordCount, pageSize = pageSize, pageIndex = pageIndex }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载每一次上传的excel
        /// </summary>
        /// <param name="updateID"></param>
        /// <returns></returns>
        public ActionResult ExportEachExcel(int updateID)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var index = 0;
            var row = sheet.CreateRow(index++);

            var num = 0;
            row.CreateCell(num++).SetCellValue("小保养套餐PID");
            row.CreateCell(num++).SetCellValue("原价");
            row.CreateCell(num++).SetCellValue("一条轮胎优惠价");
            row.CreateCell(num++).SetCellValue("二条轮胎优惠价");
            row.CreateCell(num++).SetCellValue("三条轮胎优惠价");
            row.CreateCell(num++).SetCellValue("四条轮胎优惠价");

            num = 0;
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            sheet.SetColumnWidth(num++, 16 * 256);
            var exm = new TireActivityManage();
            var paklist = exm.GetEachMaintenancePackageList(updateID);
            var validPackgeList = paklist.GroupBy(x => new { x.PID }).Select(x => x.OrderBy(y => y.PKID).Last()).ToList();
            foreach (var item in validPackgeList)
            {
                row = sheet.CreateRow(index++);
                num = 0;
                row.CreateCell(num++).SetCellValue(item.PID);
                row.CreateCell(num++).SetCellValue((double)item.Price);
                row.CreateCell(num++).SetCellValue((double)item.OnetirePrice);
                row.CreateCell(num++).SetCellValue((double)item.TwotirePrice);
                row.CreateCell(num++).SetCellValue((double)item.ThreetirePrice);
                row.CreateCell(num++).SetCellValue((double)item.FourtirePrice);
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"小保养套餐优惠价格列表-{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }



        /// <summary>
        /// 导入excel-小保养套餐优惠价格数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportMaintenancePackage()
        {
            var list = new List<MaintenancePackageOnSaleModel>();

            #region 验证文件

            var files = Request.Files;
            if (files == null || files.Count <= 0)
            {
                return Json(new { code = 1, status = false, msg = "请先选择文件上传" });
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { code = 1, status = false, msg = "文件格式不正确, 请上传Excel文件" });
            }

            #endregion

            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
            var sheet = workBook.GetSheetAt(0);


            #region 读取验证excel信息

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

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var cellNum = titleRow.FirstCellNum;
            int pidNum = 0;//小保养套餐PID的索引
            int priceNum = 0;//原价索引
            int oneTireNum = 0;//一条轮胎优惠价索引
            int twoTireNum = 0;//二条轮胎优惠价索引
            int threeTireNum = 0;//三条轮胎优惠价索引
            int fourTireNum = 0;//四条轮胎优惠价索引
            for (int i = cellNum; i < titleRow.LastCellNum; i++)
            {
                if (getStringValueFunc(titleRow.GetCell(i)) == "小保养套餐PID") pidNum = i;
                if (getStringValueFunc(titleRow.GetCell(i)) == "原价") priceNum = i;
                if (getStringValueFunc(titleRow.GetCell(i)) == "一条轮胎优惠价") oneTireNum = i;
                if (getStringValueFunc(titleRow.GetCell(i)) == "二条轮胎优惠价") twoTireNum = i;
                if (getStringValueFunc(titleRow.GetCell(i)) == "三条轮胎优惠价") threeTireNum = i;
                if (getStringValueFunc(titleRow.GetCell(i)) == "四条轮胎优惠价") fourTireNum = i;
            }


            int invalidPriceCount = 0;
            var msgs = new List<string>();
            var allPidList = new List<string>();
            for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    var cellIndex = row.FirstCellNum;
                    var pid = getStringValueFunc(row.GetCell(pidNum));
                    allPidList.Add(pid);
                    if (string.IsNullOrWhiteSpace(pid))
                    {
                        continue;
                    }
                    Regex decimalPattern = new Regex("^[0-9]+(.[0-9]*)?$");
                    string priceString = getStringValueFunc(row.GetCell(priceNum));
                    string onePriceString = getStringValueFunc(row.GetCell(oneTireNum));
                    string twoPriceString = getStringValueFunc(row.GetCell(twoTireNum));
                    string threePriceString = getStringValueFunc(row.GetCell(threeTireNum));
                    string fourPriceString = getStringValueFunc(row.GetCell(fourTireNum));

                    if ((!string.IsNullOrWhiteSpace(priceString)&&!decimalPattern.IsMatch(priceString))|| (!string.IsNullOrWhiteSpace(onePriceString) &&!decimalPattern.IsMatch(onePriceString))|| (!string.IsNullOrWhiteSpace(twoPriceString) &&!decimalPattern.IsMatch(twoPriceString))
                        || (!string.IsNullOrWhiteSpace(threePriceString) &&!decimalPattern.IsMatch(threePriceString))|| (!string.IsNullOrWhiteSpace(fourPriceString) &&!decimalPattern.IsMatch(fourPriceString)))
                    {
                        invalidPriceCount++;
                        continue;
                    }
                    decimal price = 0;
                    decimal oneTirePrice = 0;
                    decimal twoTirePrice = 0;
                    decimal threeTirePrice = 0;
                    decimal fourTirePrice = 0;
                    if (!string.IsNullOrWhiteSpace(priceString)) price = Convert.ToDecimal(priceString);
                    if (!string.IsNullOrWhiteSpace(onePriceString)) oneTirePrice = Convert.ToDecimal(onePriceString);
                    if(!string.IsNullOrWhiteSpace(twoPriceString)) twoTirePrice= Convert.ToDecimal(twoPriceString);
                    if(!string.IsNullOrWhiteSpace(threePriceString)) threeTirePrice = Convert.ToDecimal(threePriceString);
                    if(!string.IsNullOrWhiteSpace(fourPriceString)) fourTirePrice= Convert.ToDecimal(fourPriceString);

                    if (!string.IsNullOrEmpty(pid) )
                    {
                        var item = new MaintenancePackageOnSaleModel
                        {
                           PID=pid,
                           Price=price,
                           OnetirePrice=oneTirePrice,
                           TwotirePrice=twoTirePrice,
                           ThreetirePrice=threeTirePrice,
                           FourtirePrice=fourTirePrice
                        };
                        int sameIndex=list.FindIndex(x => x.PID == item.PID);
                        if (sameIndex >= 0)
                        {
                            list[sameIndex].Status = 0;
                        }
                        item.CreateBy = User.Identity.Name;
                        item.LastUpdateBy = User.Identity.Name;
                        item.Status = 1;
                        list.Add(item);
                    }
                }
            }

            if (!list.Any())
            {
                return Json(new { code = 1, status = false, msg = "导入数据不能为空!" });
            }

            #endregion

            #region 小保养套餐PID验证

            var pidList=list.Select(x => x.PID).ToList();//excel中所有的pid集合
            var validPidList=TireActivityManager.GetMaintenancePackagePidList(pidList).Select(x => x.PID).ToList();//有效pid集合
            var validMainPackageList = list.Where(x => validPidList.Contains(x.PID)).ToList();//有效的小保养套餐数据

            var allValidList=TireActivityManager.GetMaintenancePackagePidList(allPidList).Select(x => x.PID).ToList();
            var invalidcount = allPidList.Where(x => !allValidList.Contains(x)).ToList().Count;//无效PID数量
            if (!validMainPackageList.Any())
            {
                return Json(new { code = 1, status = false, msg = "小保养套餐PID全部无效!" });
            }
            #endregion

            

            if (invalidcount == 0&& invalidPriceCount==0)
            {
                return AddMaintenancePackageOnSaleList(validMainPackageList);
            }
            else
            {
                return Json(new { code = 0,data=validMainPackageList, invalidCount= invalidcount, invalidPriceCount= invalidPriceCount });
            }
           
        }

        /// <summary>
        /// 添加小保养套餐优惠价格数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMaintenancePackageOnSaleList(List<MaintenancePackageOnSaleModel> list)
        {
            int maxUpdateID = 0;
            var success = TireActivityManager.AddMaintenancePackageOnSaleList(out maxUpdateID,list);
            #region 日志记录
            using (var client = new ConfigLogClient())
            {
                var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = maxUpdateID,
                    ObjectType = "MaintenancePackageOnSale",
                    Remark = "导入小保养套餐优惠价格excel",
                    Creator = User.Identity.Name,
                }));
            }
            #endregion

            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            if (success == true)
            {
                return Json(new { status = success,code=1, msg = "上传成功" });
            }
            else
            {
                return Json(new { status = success,code = 1, msg = "上传失败" });
            }
        }
        #endregion

        #region  轮保定价

        /// <summary>
        /// 获取轮胎活动列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult GetTireActivityList(int pageSize = 20, int pageIndex = 1)
        {
            int recordCount = 0;
            var list = TireActivityManager.GetTireActivityList(out recordCount, pageSize, pageIndex);
            var nowtime = DateTime.Now;
            foreach (var item in list)
            {
                if (item.Status != -1)
                {
                    if (nowtime < item.BeginDatetime)
                    {
                        item.Status = 0;//未开始
                    }
                    if (item.BeginDatetime <= nowtime && nowtime <= item.EndDatetime)
                    {
                        item.Status = 1;//进行中
                    }
                    if (nowtime > item.EndDatetime)
                    {
                        item.Status = 2;//已过期
                    }
                }
            }
            return Json(new { data = list, dataCount = recordCount, pageSize = pageSize, pageIndex = pageIndex }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 停止轮胎活动计划
        /// </summary>
        /// <returns></returns>
        [HttpPost] 
        public ActionResult UpdateTireActivityStatus(int pkid)
        {
            var model=TireActivityManager.GetTireActivityModel(pkid);
            var nowtime = DateTime.Now;
            if (nowtime > model.EndDatetime)
            {
                return Json(new { status = true,msg="该轮胎活动计划已过期！",code=0 },JsonRequestBehavior.AllowGet);
            }
            else
            {
                var oldModel=TireActivityManager.GetTireActivityModel(pkid);
                bool sucess = TireActivityManager.UpdateTireActivityStatus(pkid, User.Identity.Name) > 0;
                var newModel= TireActivityManager.GetTireActivityModel(pkid);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = pkid,
                        ObjectType = "TireActivity",
                        BeforeValue = JsonConvert.SerializeObject(oldModel),
                        AfterValue = JsonConvert.SerializeObject(newModel),
                        Remark = "停止计划",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion

                //等待1秒，写库同步到读库
                Thread.Sleep(1000);
                return Json(new { status = sucess,msg="",code=1 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 下载excel-轮胎pid列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelTireActivity(int tireActivityID)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var index = 0;
            var row = sheet.CreateRow(index++);

            var num = 0;
            row.CreateCell(num++).SetCellValue("PID");

            num = 0;
            sheet.SetColumnWidth(num++, 16 * 256);
            var exm = new TireActivityManage();
            var list = exm.GetTireActivityPIDList(tireActivityID);
            foreach (var item in list)
            {
                row = sheet.CreateRow(index++);
                num = 0;
                row.CreateCell(num++).SetCellValue(item.PID);
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            var model= TireActivityManager.GetTireActivityModel(tireActivityID);
            #region 日志记录
            using (var client = new ConfigLogClient())
            {
                var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = tireActivityID,
                    ObjectType = "TireActivity",
                    BeforeValue = JsonConvert.SerializeObject(model),
                    AfterValue = JsonConvert.SerializeObject(model),
                    Remark = "下载",
                    Creator = User.Identity.Name,
                }));
            }
            #endregion

            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            return File(ms.ToArray(), "application/x-xls", $"轮胎活动列表-{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 导入excel-轮胎PID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportTireActivityPid(string planName,string planDesc,string beginDateTime,string endDateTime)
        {
            var list = new List<TireActivityPIDModel>();

            #region 验证文件
            
            var files = Request.Files;
            if (files == null || files.Count <= 0)
            {
                return Json(new { code = 1, status = false, msg = "请先选择文件上传" });
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { code = 1, status = false, msg = "文件格式不正确, 请上传Excel文件" });
            }
            var desc = Request["planName"].ToString();
            #endregion

            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
            var sheet = workBook.GetSheetAt(0);


            #region 读取验证excel信息

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

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var cellNum = titleRow.FirstCellNum;
            int pidNum = 0;//轮胎PID的索引
            for (int i = cellNum; i < titleRow.LastCellNum; i++)
            {
                if (titleRow.GetCell(i) != null)
                {
                    if(getStringValueFunc(titleRow.GetCell(i))== "PID") pidNum = i;
                }
            }
           

            var msgs = new List<string>();
            for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    var cellIndex = row.FirstCellNum;
                    var pid = getStringValueFunc(row.GetCell(pidNum));
                    if (!string.IsNullOrEmpty(pid))
                    {
                        var item = new TireActivityPIDModel
                        {
                            PID = pid
                        };
                        var existsItem = list.FirstOrDefault(x =>
                            x.PID == item.PID 
                        );
                        if (existsItem != null)
                        {
                            list.Remove(existsItem);
                        };
                        item.CreateBy = User.Identity.Name;
                        item.LastUpdateBy = User.Identity.Name;
                        list.Add(item);
                    }
                }
            }

            if (!list.Any())
            {
                return Json(new { code = 1, status = false, msg = "导入数据不能为空!" });
            }

            #endregion

            #region 轮胎PID验证

            var pidList = list.Select(x => x.PID).ToList();//excel中所有的pid集合
            var validPidList = TireActivityManager.GetValidTirePid(pidList).Select(x => x.PID).ToList();//有效pid集合
            var validMainPackageList = list.Where(x => validPidList.Contains(x.PID)).ToList();//有效的轮胎活动PID
            if (!validMainPackageList.Any())
            {
                return Json(new { code = 1, status = false, msg = "轮胎PID全部无效!" });
            }

            var tireActivityModel = new TireActivityModel();
            tireActivityModel.PlanDesc = planDesc.Trim();
            tireActivityModel.PlanName = planName.Trim();
            tireActivityModel.BeginDatetime = DateTime.Parse(beginDateTime);
            tireActivityModel.EndDatetime = DateTime.Parse(endDateTime);
            tireActivityModel.PIDNum = validMainPackageList.Count;
            var nowTime = DateTime.Now;
            if(nowTime < tireActivityModel.BeginDatetime)
            {
                tireActivityModel.Status = 0;
            }
            if(tireActivityModel.BeginDatetime<=nowTime&& nowTime <= tireActivityModel.EndDatetime)
            {
                tireActivityModel.Status = 1;
            }
            if (nowTime > tireActivityModel.EndDatetime)
            {
                tireActivityModel.Status = 2;
            }
            tireActivityModel.CreateBy = User.Identity.Name;
            tireActivityModel.LastUpdateBy = User.Identity.Name;
            int maxUpdateID = TireActivityManager.GetMaxTireActivityUpdateID();
            tireActivityModel.UpdateID = maxUpdateID + 1;
            tireActivityModel.PlanNumber = "计划" + (maxUpdateID + 2);
            var notvalidCount= list.Where(x => !validPidList.Contains(x.PID)).ToList().Count;
            #endregion

            var repeatList=TireActivityManager.GetRepeatTirePids(validMainPackageList.Select(x => x.PID).ToList().ToList());
            return Json(new { code = 0, activityModel = tireActivityModel, list = validMainPackageList, repeatList = repeatList, notvalidCount = notvalidCount,validCount= validPidList.Count });
        }

        /// <summary>
        /// 新建计划与添加轮胎PID
        /// </summary>
        /// <param name="model"></param>
        /// <param name="notvalidCount"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTireActivityPid(TireActivityModel model,int notvalidCount,List<TireActivityPIDModel> list)
        {
            var success = TireActivityManager.ImportTireActivityPid(model, list);
            if (success == true)
            {
                return Json(new { code=1,status = success, msg = "文件上传成功" });
            }
            else
            {
                return Json(new { code=1,status = success, msg = "上传失败" });
            }
        }


        #endregion
    }
}
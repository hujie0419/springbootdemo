using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity.TireSecurityCode;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.TireSecurityCode;
using Tuhu.Provisioning.Business.CommonServices;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using Tuhu.Component.Framework.Identity;

namespace Tuhu.Provisioning.Controllers
{
    public class TireSecurityCodeController : Controller
    {
        #region 
        private readonly Lazy<Manager> lazy = new Lazy<Manager>();
        private Manager TireSecurityCodeConfigManager
        {
            get { return lazy.Value; }
        }
        #endregion

        // GET: TireSecurityCode
        public ActionResult Search()
        {
            return View();
        }

        public PartialViewResult SearchList(TireSecurityCodeConfigQuery query)
        {
            List<TireSecurityCodeConfig> list = new List<TireSecurityCodeConfig>();
            list = TireSecurityCodeConfigManager.QuerySecurityCodeConfigModel(query);

            var pager = new PagerModel(query.PageIndex, query.PageDataQuantity);
            ViewBag.query = query;
            pager.TotalItem = query.TotalCount;

            return PartialView(new ListModel<TireSecurityCodeConfig>()
            {
                Pager = pager,
                Source = list
            });
        }

        public ActionResult UploadSecurityCodeLog(string msg = null)
        {
            ViewBag.error = msg;
            return View();
        }

        public ActionResult UploadSecurityCodeLogImport(HttpPostedFileBase fileBase)
        {
            HttpPostedFileBase file = Request.Files["files"];

            #region 【参数初始化】
            string uploadFileName = "";
            string uploadFileAddress = "";
            string successFileName = "";
            string successFileAddress = "";
            string failFileName = "";
            string failFileAddress = "";
            string ext = "";
            string noFileName = "";
            string uploadDomain = "/TireLog/SecurityCode";
            string batchNum = DateTime.Now.ToString("yyyyMMddHHmmss");
            #endregion

            #region 【导入文件预判断】
            if (file == null || file.ContentLength <= 0)
            {
                return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件不能为空" });
            }

            var filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            ext = System.IO.Path.GetExtension(file.FileName);//获取上传文件的扩展名
            noFileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);//获取无扩展名的文件名
            const string fileType = ".txt"; //定义上传文件的类型字符串

            if (string.IsNullOrWhiteSpace(ext))
            {
                return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件扩展名不能为空" });
            }
            if (!fileType.Contains(ext))
            {
                return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件类型不对，只能导入txt格式的文件" });
            }
            #endregion

            #region 【源文件上传】
            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            uploadFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + ext;
            var result = FileUploadService.UploadFile(buffer, ext, uploadFileName, uploadDomain);
            if (!string.IsNullOrWhiteSpace(result)) uploadFileAddress = "https://img1.tuhu.org" + result;
            else return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
            #endregion

            #region 【拆分异常数据、去重】
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream, System.Text.Encoding.GetEncoding("gb2312"));
            StringBuilder successSB = new StringBuilder();
            StringBuilder failSB = new StringBuilder();
            string line;
            List<TireSecurityCodeConfig> congifList = new List<TireSecurityCodeConfig>();
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("二") && line.Contains("维"))
                {
                }
                else
                {
                    string uCode = "";
                    string fCode = "";
                    string securityCode = "";
                    try
                    {
                        uCode = line.Substring(line.IndexOf("?u=") + 3, line.IndexOf("&f") - line.IndexOf("?u=") - 3);
                        fCode = line.Substring(line.IndexOf("&f=") + 3, line.LastIndexOf(",") - line.IndexOf("&f=") - 3);
                        securityCode = line.Substring(line.LastIndexOf(",") + 1, line.Length - line.LastIndexOf(",") - 1).Trim();
                    }
                    catch (Exception ex)
                    {
                        failSB.AppendLine(line + ":参数错误");
                        continue;
                    }

                    if (uCode == "" || !IsNumeric(uCode)
                        || fCode == "" || !IsNumeric(fCode)
                        || securityCode == "" || !IsNumeric(securityCode))
                    {
                        failSB.AppendLine(line + ":参数错误");
                        continue;
                    }
                    else
                    {
                        TireSecurityCodeConfig config = new TireSecurityCodeConfig
                        {
                            CodeID = Guid.NewGuid(),
                            CreateTime = DateTime.Now,
                            LastUpdateDataTime = DateTime.Now,
                            UCode = uCode,
                            FCode = fCode,
                            SecurityCode = securityCode,
                            DataIntegrity = false,
                            BatchNum = batchNum
                        };
                        congifList.Add(config);
                        continue;
                    }
                }
            }

            congifList = congifList
                .GroupBy(item => item.SecurityCode)
                .Select(item => item.First())
                .ToList<TireSecurityCodeConfig>();
            #endregion

            #region 【批量插入数据】
            var insertResult = TireSecurityCodeConfigManager.InsertTireSecurityCodeConfig(congifList);
            if (insertResult)
            {
                var insertList = TireSecurityCodeConfigManager.QuerySecurityCodeConfigModelByBatchNum(batchNum);
                if (insertList == null || insertList.Count == 0)
                    return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "本次上传数据均存在,不做更新." });
                foreach (var insert in insertList)
                {
                    string lineSuccees = "https://wx.tuhu.cn/vue/antifake/pages/home/select?guid=" + insert.CodeID.ToString("D") + "," + insert.SecurityCode;
                    successSB.AppendLine(lineSuccees);
                }
            }
            else
            {
                failSB.AppendLine("数据失败,刷新页面重试.");
            }
            #endregion

            #region 【生成成功\失败文件地址】
            if (successSB.Length > 0)
            {
                byte[] arraySuccess = Encoding.GetEncoding("gb2312").GetBytes(successSB.ToString());
                successFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + "_二维码" + ext;
                result = FileUploadService.UploadFile(arraySuccess, ext, successFileName, uploadDomain);
                if (!string.IsNullOrWhiteSpace(result)) successFileAddress = "https://img1.tuhu.org" + result;
                else
                {
                    TireSecurityCodeConfigManager.DeleleSecurityCodeConfigModelByBatchNum(batchNum);
                    return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                }
            }

            if (failSB.Length > 0)
            {
                byte[] arrayFail = Encoding.GetEncoding("gb2312").GetBytes(failSB.ToString());
                failFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + "_Error" + ext;
                result = FileUploadService.UploadFile(arrayFail, ext, failFileName, uploadDomain);
                if (!string.IsNullOrWhiteSpace(result)) failFileAddress = "https://img1.tuhu.org" + result;
                else
                {
                    TireSecurityCodeConfigManager.DeleleSecurityCodeConfigModelByBatchNum(batchNum);
                    return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                }
            }
            #endregion

            #region 【记录上传文件日志】
            UploadSecurityCodeLog scLog = new UploadSecurityCodeLog
            {
                UploadFileName = uploadFileName,
                UploadFileAddress = uploadFileAddress,
                SuccessFileName = successFileName,
                SuccessFileAddress = successFileAddress,
                FailFileName = failFileName,
                FailFileAddress = failFileAddress,
                CreateTime = DateTime.Now,
                LastUpdateDataTime = DateTime.Now,
                Operator = ThreadIdentity.Operator.Name,
            };

            var insertLog = TireSecurityCodeConfigManager.InsertUploadSecurityCodeLog(scLog);
            #endregion

            return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "导入成功" });
        }

        public PartialViewResult UploadSecurityCodeLogList(LogSearchQuery query)
        {
            List<UploadSecurityCodeLog> list = new List<UploadSecurityCodeLog>();
            list = TireSecurityCodeConfigManager.QueryUploadSecurityCodeLogModel(query);

            var pager = new PagerModel(query.PageIndex, query.PageDataQuantity);
            ViewBag.query = query;
            pager.TotalItem = query.TotalCount;

            return PartialView(new ListModel<UploadSecurityCodeLog>()
            {
                Pager = pager,
                Source = list
            });
        }

        public ActionResult UploadBarCodeLog(string msg)
        {
            ViewBag.error = msg;
            return View();
        }

        public ActionResult UploadBarCodeLogImport(HttpPostedFileBase fileBase)
        {
            HttpPostedFileBase file = Request.Files["files"];

            #region 【参数初始化】
            string uploadFileName = "";
            string uploadFileAddress = "";
            string failFileName = "";
            string failFileAddress = "";
            string ext = "";
            string noFileName = "";
            string uploadDomain = "/TireLog/BarCode";
            string barCodeBatchNum = DateTime.Now.ToString("yyyyMMddHHmmss");
            #endregion

            #region 【导入文件预判断】
            if (file == null || file.ContentLength <= 0)
            {
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件不能为空" });
            }
            var filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            ext = System.IO.Path.GetExtension(file.FileName);//获取上传文件的扩展名
            noFileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);//获取无扩展名的文件名
            const string fileType = ".xls,.xlsx"; //定义上传文件的类型字符串

            if (string.IsNullOrWhiteSpace(ext))
            {
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件扩展名不能为空" });
            }
            if (!fileType.Contains(ext))
            {
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件类型不对，只能导入xls和xlsx格式的文件" });
            }
            #endregion

            #region 【源文件上传】
            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            uploadFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + ext;
            var result = FileUploadService.UploadFile(buffer, ext, uploadFileName, uploadDomain);
            if (!string.IsNullOrWhiteSpace(result)) uploadFileAddress = "https://img1.tuhu.org" + result;
            else return RedirectToAction("UploadSecurityCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });

            UploadBarCodeLog barLog = new UploadBarCodeLog
            {
                UploadFileName = uploadFileName,
                UploadFileAddress = uploadFileAddress,
                Operator = ThreadIdentity.Operator.Name
            };
            #endregion

            #region 【检查数据缺失】
            stream.Position = 0;
            var excel = new Controls.ExcelHelper(stream, file.FileName);
            var dt = excel.ExcelToDataTable("sheet1", true);

            if (buffer.Length > 0 && (dt == null || dt.Rows == null || dt.Rows.Count < 1))
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });

            StringBuilder failSB = new StringBuilder();
            List<InputBarCode> congifList = new List<InputBarCode>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (null == dt.Rows[i]["条码"] || string.IsNullOrWhiteSpace(dt.Rows[i]["条码"].ToString()) ||
                    null == dt.Rows[i]["防伪码"] || string.IsNullOrWhiteSpace(dt.Rows[i]["防伪码"].ToString()))
                {
                    failSB.AppendLine("第" + (i + 2) + "行:条码或防伪码数据不完整.");
                }
                else
                {
                    congifList.Add(new InputBarCode
                    {
                        BarCode = dt.Rows[i]["条码"].ToString(),
                        SecurityCode = dt.Rows[i]["防伪码"].ToString(),
                        BarCodeBatchNum = barCodeBatchNum
                    });
                }
            }

            if (failSB.Length > 0)
            {
                byte[] arrayFail_Params = Encoding.GetEncoding("gb2312").GetBytes(failSB.ToString());
                failFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + "_Error" + ".txt";
                result = FileUploadService.UploadFile(arrayFail_Params, ".txt", failFileName, uploadDomain);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    failFileAddress = "https://img1.tuhu.org" + result;
                }
                else
                {
                    return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                }

                barLog.FailFileName = failFileName;
                barLog.FailFileAddress = failFileAddress;
                barLog.CreateTime = DateTime.Now;
                barLog.LastUpdateDataTime = DateTime.Now;
                var insertLog_Params = TireSecurityCodeConfigManager.InsertUploadBarCodeLog(barLog);
                if (insertLog_Params)
                    return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "数据有缺失,请检查错误日志,整理数据后重新上传." });
                else
                    return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
            }
            #endregion

            #region 【检查数据重复】
            var checkSCodeDup = congifList
                .GroupBy(item => item.SecurityCode)
                .Select(item => item.First())
                .ToList<InputBarCode>().Count;
            if (checkSCodeDup < congifList.Count)
            {
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "防伪码数据有重复，请Excel排查后重新上传." });
            }

            var checkBarCodeDup = congifList
                .GroupBy(item => item.BarCode)
                .Select(item => item.First())
                .ToList<InputBarCode>().Count;
            if (checkBarCodeDup < congifList.Count)
            {
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "条码数据有重复，请Excel排查后重新上传." });
            }
            #endregion

            #region 【批量更新数据】
            congifList = congifList
                .GroupBy(item => item.SecurityCode)
                .Select(item => item.First())
                .ToList<InputBarCode>();
            var updateBarCode = TireSecurityCodeConfigManager.InsertBarCodeConfig(congifList);
            List<InputBarCode> errorInputList = new List<InputBarCode>();
            switch (updateBarCode)
            {
                case -2:
                    errorInputList = TireSecurityCodeConfigManager.QueryInputBarCodeByError("ItemNotExist", congifList);
                    failSB.AppendLine("以下防伪码还没上传过,请检查数据.");
                    failSB.AppendLine("防伪码,条码");
                    foreach (var error in errorInputList)
                    {
                        failSB.AppendLine(error.SecurityCode + "," + error.BarCode);
                    }

                    byte[] arrayFail_ItemNotExist = Encoding.GetEncoding("gb2312").GetBytes(failSB.ToString());
                    failFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + "_Error" + ".txt";
                    result = FileUploadService.UploadFile(arrayFail_ItemNotExist, ".txt", failFileName, uploadDomain);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        failFileAddress = "https://img1.tuhu.org" + result;
                    }
                    else
                    {
                        return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                    }
                    barLog.FailFileName = failFileName;
                    barLog.FailFileAddress = failFileAddress;
                    barLog.CreateTime = DateTime.Now;
                    barLog.LastUpdateDataTime = DateTime.Now;
                    var insertLog_ItemNotExist = TireSecurityCodeConfigManager.InsertUploadBarCodeLog(barLog);
                    if (insertLog_ItemNotExist)
                        return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "数据里有不存在的防伪码,请检查错误日志,整理数据后重新上传." });
                    else
                        return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                case -3:
                    return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "本次上传数据均存在,不做更新." });
                case -4:
                    errorInputList = TireSecurityCodeConfigManager.QueryInputBarCodeByError("Difference", congifList);
                    failSB.AppendLine("以下防伪码或条码已经存在过,请删除已存在数据后重新上传.");
                    failSB.AppendLine("防伪码,条码");
                    foreach (var error in errorInputList)
                    {
                        failSB.AppendLine(error.SecurityCode + "," + error.BarCode);
                    }
                    byte[] arrayFail_Difference = Encoding.GetEncoding("gb2312").GetBytes(failSB.ToString());
                    failFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + '_' + noFileName + "_Error" + ".txt";
                    result = FileUploadService.UploadFile(arrayFail_Difference, ".txt", failFileName, uploadDomain);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        failFileAddress = "https://img1.tuhu.org" + result;
                    }
                    else
                    {
                        return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                    }
                    barLog.FailFileName = failFileName;
                    barLog.FailFileAddress = failFileAddress;
                    barLog.CreateTime = DateTime.Now;
                    barLog.LastUpdateDataTime = DateTime.Now;
                    var insertLog_Difference = TireSecurityCodeConfigManager.InsertUploadBarCodeLog(barLog);
                    if (insertLog_Difference)
                        return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "部分数据已存在,请检查错误日志,整理数据后重新上传." });
                    else
                        return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
                case -1:
                    return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "上传失败,刷新页面重试." });
            }
            #endregion

            #region 【记录上传文件日志】
            barLog.FailFileName = failFileName;
            barLog.FailFileAddress = failFileAddress;
            barLog.CreateTime = DateTime.Now;
            barLog.LastUpdateDataTime = DateTime.Now;
            var insertLog = TireSecurityCodeConfigManager.InsertUploadBarCodeLog(barLog);
            if (!insertLog)
            {
                TireSecurityCodeConfigManager.DeleleBarCodeByBatchNum(barCodeBatchNum);
                return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "文件上传失败" });
            }
            #endregion

            return RedirectToAction("UploadBarCodeLog", "TireSecurityCode", new { msg = "导入成功" });
        }

        public PartialViewResult UploadBarCodeLogList(LogSearchQuery query)
        {
            List<UploadBarCodeLog> list = new List<UploadBarCodeLog>();
            list = TireSecurityCodeConfigManager.QueryUploadBarCodeLogModel(query);

            var pager = new PagerModel(query.PageIndex, query.PageDataQuantity);
            ViewBag.query = query;
            pager.TotalItem = query.TotalCount;

            return PartialView(new ListModel<UploadBarCodeLog>()
            {
                Pager = pager,
                Source = list
            });
        }

        private bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]*$");
        }
    }
}
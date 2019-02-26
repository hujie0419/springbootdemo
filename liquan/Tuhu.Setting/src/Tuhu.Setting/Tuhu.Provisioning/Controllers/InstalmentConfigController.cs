using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.Product.Request;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Service.Product.Enum;
using swc = System.Web.Configuration;

namespace Tuhu.Provisioning.Controllers
{
    public class InstalmentConfigController : Controller
    {
        // GET: InstalmentConfig
        /// <summary>
        /// 轮胎 分期配置
        /// </summary>
        /// <returns></returns>
        [PowerManage]
        public ActionResult Index()
        {
            string categoryName = "Tires";
            ViewBag.CategoryName = categoryName;//跟 Tuhu_productcatalog..CarPAR_CatalogHierarchy 里的大分类一致
            return TireIndex(categoryName);
        }
        [PowerManage]
        public ActionResult IndexHub()
        {
            string categoryName = "hub";
            ViewBag.CategoryName = categoryName;//跟 Tuhu_productcatalog..CarPAR_CatalogHierarchy 里的大分类一致
            return TireIndex(categoryName);
        }
        [PowerManage]
        public ActionResult IndexAutoProduct()
        {
            string categoryName = "AutoProduct";
            ViewBag.CategoryName = categoryName;//跟 Tuhu_productcatalog..CarPAR_CatalogHierarchy 里的大分类一致
            return TireIndex(categoryName);
        }
        [PowerManage]
        public ActionResult IndexMR1()
        {
            string categoryName = "MR1";
            ViewBag.CategoryName = categoryName;
            return TireIndex(categoryName);
        }

        private ActionResult TireIndex(string categoryName)
        {
            ViewBag.Title = "轮胎分期购配置";
            var brands = RecommendManager.GetBrandsByCategoryName(categoryName);
            IEnumerable<TireSizeModel> tireSizes = null;
            if(categoryName== "Tires")
                tireSizes = RecommendManager.SelectALLTireSize();
            else if (categoryName == "hub")
                tireSizes = RecommendManager.SelectALLHubSize();
            else
                tireSizes = new List<TireSizeModel>();
            return View("Index",Tuple.Create(brands, tireSizes));
        }

        private ActionResult HubIndex(string categoryName)
        {
            ViewBag.Title = "轮毂分期购配置";
            var brands = RecommendManager.GetBrandsByCategoryName(categoryName);
            var hubSizes = RecommendManager.SelectALLHubSize();
            return View(Tuple.Create(brands,hubSizes));
        }

        private ActionResult ChePinIndex(string categoryName)
        {
            ViewBag.Title = "车品分期购配置";
            var brands = RecommendManager.GetBrandsByCategoryName(categoryName);
            return View(Tuple.Create(brands));
        }


        public PartialViewResult InstallmentList(TireInstallmentQueryRequest query)
        {
            using (var client = new Tuhu.Service.Product.ProductConfigClient())
            {
                if (!string.IsNullOrEmpty(Request.QueryString["IsOnSale"]))
                {
                    query.IsOnSale = Request.QueryString["IsOnSale"] == "1";
                }
                if (!string.IsNullOrEmpty(Request.QueryString["IsInstallmentOpen"]))
                {
                    query.IsInstallmentOpen = Request.QueryString["IsInstallmentOpen"] == "1";
                }
                Service.OperationResult<Tuhu.Models.PagedModel<Service.Product.Response.TireInstallmentQueryResponse>>
                    result = null;
                string categoryName = query.CategoryName ?? "Tires";
                switch (categoryName)
                {
                    case "Tires":
                        result = client.QueryTireInstallment(query);
                        break;
                    default:
                        query.CategoryName = categoryName;
                        result = client.QueryInstallmentByCategoryName(query);
                        break;
                }
                result.ThrowIfException(true);
                ViewBag.Query = query;
                ViewBag.CategoryName = categoryName;
                return PartialView(result.Result);
            }
        }

        public ActionResult ShowTireInstallmentLog(string pid)
        {
            using (var client = new Tuhu.Service.Product.ProductConfigClient())
            {
                var result = client.SelectTireInstallmentLogs(pid);
                result.ThrowIfException(true);
                var datas = result.Result;
                return View(datas);
            }
        }

        private async Task<string> GetModifyLogStringAsync(TireInstallmentModel model)
        {
            using (var client = new Tuhu.Service.Product.ProductConfigClient())
            {
                var queryresult = await client.SelectTireInstallmentByPIDAsync(model.PID);
                queryresult.ThrowIfException(true);
                var originmodel = queryresult.Result;
                if (originmodel == null)
                {
                    return "创建";
                }
                var props = typeof(TireInstallmentModel).GetProperties();
                Dictionary<string, string> propnames = new Dictionary<string, string>()
                {
                    {"IsInstallmentOpen","分期是否打开" },
                    {"ThreePeriods","三期" },
                    {"SixPeriods","六期" },
                    {"TwelvePeriods","十二期" },
                };
                StringBuilder sb = new StringBuilder();
                foreach (var prop in props)
                {
                    if (propnames.ContainsKey(prop.Name))
                    {
                        var ogiginvalue = prop.GetValue(originmodel);
                        var modifyvalue = prop.GetValue(model);
                        if (!string.Equals(ogiginvalue, modifyvalue))
                        {
                            sb.Append($"  {propnames[prop.Name]} => 修改前:{(string.IsNullOrEmpty(ogiginvalue?.ToString()) ? "无" : ogiginvalue)}. " +
                                      $"修改后:{(string.IsNullOrEmpty(modifyvalue?.ToString()) ? "无" : modifyvalue)}");
                        }
                    }
                }
                return sb.ToString();
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveTireInstallmentConfigAsync(TireInstallmentModel model)
        {
            JsonResult jr = new JsonResult();
            try
            {

                using (var logclient = new Tuhu.Service.Product.ProductConfigClient())
                {
                    using (var client = new Tuhu.Service.Product.ProductConfigClient())
                    {


                        string userNo = HttpContext.User.Identity.Name;
                        string content = await GetModifyLogStringAsync(model);

                        var log = new TireInstallmentConfigLog()
                        {
                            PID = model.PID,
                            User = userNo,
                            Content = content
                        };
                        var result = await client.CreateOrUpdateTireInstallmentAsync(model);
                        result.ThrowIfException(true);
                        jr.Data = new { code = result.Result > 0 ? 1 : 0, msg = result.Result > 0 ? "操作成功" : "操作失败..." };
                        var logresult = await logclient.LogTireInstallmentConfigAsync(log);
                        logresult.ThrowIfException(true);
                        return jr;
                    }
                }


            }
            catch (System.Exception ex)
            {
                jr.Data = new { code = 0, msg = $"操作失败....ex:{ex}" };
                return jr;
            }
        }

        [HttpPost]
        public async Task<JsonResult> BatchSaveByExcel()
        {
            JsonResult jr = new JsonResult();
            if (Request.Files.Count == 0)
            {
                jr.Data = new { code = 0, msg = "未选择Excel文件" };
                return jr;
            }
            var file = Request.Files[0];
            var ext = System.IO.Path.GetExtension(file.FileName);
            string fileType = ".xls,.xlsx";
            if (!fileType.Contains(ext))
            {
                jr.Data = new { code = 0, msg = "文件格式不正确" };
                return jr;
            }
            int filesize = file.ContentLength;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + HttpContext.User.Identity.Name + DateTime.Now.ToString("yyyyMMddhhmmss") + ext;
            var virtualPath = "/product/excel/";
            var domainPath = swc.WebConfigurationManager.AppSettings["DoMain_news"];
            var serverPath = virtualPath + fileName;//http://file.tuhu.test/vehicletype/excel/aaa.xls
            string savePath = domainPath + serverPath;
            byte[] input = new byte[filesize];
            var fileStream = file.InputStream;
            fileStream.Read(input, 0, filesize);

            var client = new WcfClinet<IFileUpload>();
            var result = client.InvokeWcfClinet(w => w.UploadFile(serverPath, input));
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
                    table = GetExcelData(wb1);
                }
                else if (fileName.EndsWith(".xls"))
                {
                    wb2 = new HSSFWorkbook(memstream);
                    table = GetExcelData(wb2);
                }
            }
            Func<string, InstallmentType?> gettype = v =>
            {
                if (string.Equals(v?.Trim(), "途虎"))
                {
                    return InstallmentType.Tuhu;
                }
                if (string.Equals(v?.Trim(), "用户"))
                {
                    return InstallmentType.User;
                }
                return null;
            };
            if (table != null && table.Rows.Count > 0)
            {
                if (table.Rows.Count > 500)
                {
                    jr.Data = new {code = 0, msg = "一次只能上传最多500条配置"};
                    return jr;
                }
                var models = table.AsEnumerable().Select(r =>
                {
                    var three = gettype(r[1]?.ToString());
                    var six = gettype(r[2]?.ToString());
                    var twelve = gettype(r[3]?.ToString());
                    if (three != null && six != null && twelve != null)
                    {
                        var model = new TireInstallmentModel()
                        {
                            PID = r[0]?.ToString(),
                            ThreePeriods = three.Value,
                            SixPeriods = six.Value,
                            TwelvePeriods = twelve.Value,
                            IsInstallmentOpen = true
                        };
                        return model;
                    }
                    return null;
                });

                int errorcount = models.Where(x => x == null).Count();
                int successcount = 0;
                var results = Parallel.ForEach<TireInstallmentModel>(models,
                    new ParallelOptions() {MaxDegreeOfParallelism = 10},
                    async (x) =>
                    {
                        if (await CreateOrUpdateTireInstallmentAsync(x))
                            successcount++;
                        else
                            errorcount++;

                        System.Threading.Thread.Sleep(300);
                    }
                );
                jr.Data = new {code = 1, msg = $"成功导入{successcount}个,失败:{errorcount}个"};
            }
            return jr;
        }

        private async Task<bool> CreateOrUpdateTireInstallmentAsync(TireInstallmentModel m)
        {
            try
            {
                string userNo = HttpContext.User.Identity.Name;

                using (var productclient = new Tuhu.Service.Product.ProductConfigClient())
                {
                    string content = await GetModifyLogStringAsync(m);
                    var log = new TireInstallmentConfigLog()
                    {
                        PID = m.PID,
                        User = userNo,
                        Content = content
                    };
                    var createresult = await productclient.CreateOrUpdateTireInstallmentAsync(m);
                    createresult.ThrowIfException(true);
                    var logresult = await productclient.LogTireInstallmentConfigAsync(log);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
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
        public DataTable GetExcelData(IWorkbook wb)
        {
            var dt = new DataTable();
            var sheet = wb.GetSheetAt(0);//获取excel第一个sheet
            var headerRow = sheet.GetRow(0);//获取sheet首行

            int cellCount = headerRow.LastCellNum;//获取总列数

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dt.Columns.Add(column);
            }

            for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                var dataRow = dt.NewRow();
                var itemHandle = row.GetCell(cellCount - 1);
                if (itemHandle == null)
                {
                    continue;
                }

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        [HttpPost]
        public async Task<JsonResult> BatchSaveTireInstallmentConfigAsync(TireInstallmentModel model, IEnumerable<string> pids)
        {
            JsonResult jr = new JsonResult();

            try
            {
                foreach (var pid in pids)
                {
                    using (var logclient = new Tuhu.Service.Product.ProductConfigClient())
                    {
                        using (var client = new Tuhu.Service.Product.ProductConfigClient())
                        {
                            model.PID = pid;
                            string userNo = HttpContext.User.Identity.Name;
                            string content = await GetModifyLogStringAsync(model);
                            var log = new TireInstallmentConfigLog()
                            {
                                PID = pid,
                                User = userNo,
                                Content = content
                            };
                            var result = await client.CreateOrUpdateTireInstallmentAsync(model);
                            result.ThrowIfException(true);
                            var logresult = await logclient.LogTireInstallmentConfigAsync(log);
                        }
                    }
                }
                jr.Data = new { code = 1, msg = "操作成功" };
                return jr;
            }
            catch (System.Exception ex)
            {
                jr.Data = new { code = 0, msg = $"操作失败....ex:{ex}" };
                return jr;
            }
        }
    }
}
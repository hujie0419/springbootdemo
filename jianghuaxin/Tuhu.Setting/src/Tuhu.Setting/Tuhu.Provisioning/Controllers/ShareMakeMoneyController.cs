using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.ShareMakeMoney;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Reflection;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareMakeMoneyController : Controller
    {
        //
        // GET: /ShareMakeMoney/


        public ActionResult List()
        {
            return View(new ShareMakeMoneyManager().GetList());
        }


        public ActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Redirect("/ShareMakeMoney/List");
            ShareMakeMoneyManager manger = new ShareMakeMoneyManager();
            var model = manger.GetEntity(Convert.ToInt32(id)) ?? new SE_ShareMakeMoneyConfig();
            return View(model);
        }

        public ActionResult GetProduct(int id,int pageIndex=1,int pageSize=10, string datetime="", string pid="", string displayName="", string times="", string status="")
        {
            ShareMakeMoneyManager manager = new ShareMakeMoneyManager();
            int total = 0;
            var list = manager.GetProductEntities(id, pageIndex, pageSize, datetime, pid, displayName, times, status, out total).OrderByDescending(p => p.CreateDate).Select(o => new
            {
                BatchGuid = o.BatchGuid,
                CreateDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", o.CreateDate),
                DisplayName = o.DisplayName,
                ID = o.ID,
                IsMakeMoney = o.IsMakeMoney == true ? "启用" : "禁用",
                IsShare = o.IsShare == true ? "显示" : "不显示",
                Orderby = o.Orderby,
                PID = o.PID,
                Times = o.Times
            });
            ViewBag.pageNumber = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.Total = total;
            return Content(JsonConvert.SerializeObject(new
            {
                total = total,
                rows = list
            }));
        }

        public ActionResult Export(int id)
        {

            ShareMakeMoneyManager manager = new ShareMakeMoneyManager();
            using (var stream = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/分享赚钱商品.xlsx"))))
            {
                var list = manager.GetProductEntities(id).OrderByDescending(p => p.CreateDate).ToList();
                var xssfWorkbook = new XSSFWorkbook(stream); //创建Workbook对象  2007+   
                                                             // var hssfWorkbook = new HSSFWorkbook(stream)://2003
                if (list.Count > 0)
                {
                    var i = 0;
                    var sheet = xssfWorkbook.GetSheetAt(0);
                    foreach (var item in list)
                    {
                        var row = sheet.CreateRow((i++) + 1);
                        row.CreateCell(0).SetCellValue(item.ID);
                        row.CreateCell(1).SetCellValue(item.FKID.Value);
                        row.CreateCell(2).SetCellValue(item.BatchGuid);
                        row.CreateCell(3).SetCellValue(item.CreateDate.ToString());
                        row.CreateCell(4).SetCellValue(item.PID);
                        row.CreateCell(5).SetCellValue(item.DisplayName);
                        row.CreateCell(6).SetCellValue(item.Times.ToString());
                        row.CreateCell(7).SetCellValue(item.IsMakeMoney ? "启用" : "禁用");
                        row.CreateCell(8).SetCellValue(item.IsShare ? "显示" : "不显示");
                        row.CreateCell(9).SetCellValue(item.Orderby.Value);

                    }
                }

                Response.AppendHeader("Content-Disposition", "attachment;fileName=分享赚钱商品" + ".xlsx");
                xssfWorkbook.Write(Response.OutputStream);
                Response.End();
            }

            return Json(true);

        }

        /// <summary>  
        /// 转换为一个DataTable  
        /// </summary>  
        /// <typeparam name="TResult"></typeparam>  
        ///// <param name="value"></param>  
        /// <returns></returns>  
        public static DataTable ToDataTable<TResult>(IEnumerable<TResult> value) where TResult : class
        {
            //创建属性的集合  
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口  
            Type type = typeof(TResult);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列  
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in value)
            {
                //创建一个DataRow实例  
                DataRow row = dt.NewRow();
                //给row 赋值  
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable  
                dt.Rows.Add(row);
            }
            return dt;
        }
        /// <summary>
        /// 上传 xlsx
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProduct(int id)
        {
            try
            {

                //var file = Request.Files[0];

                // if (!file.FileName.Contains(".xlsx") || !file.FileName.Contains(".xls"))
                //     return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件" }, "text/html");

                // var excel = new Tuhu.Provisioning.Controls.ExcelHelper(file.InputStream, file.FileName);
                // var dt = excel.ExcelToDataTable("分享赚钱活动产品", true);
                ShareMakeMoneyManager manager = new ShareMakeMoneyManager();
                var dt = manager.GetFenxiangzhuanqianProduct();
                if (dt == null || dt.Rows.Count == 0)
                    return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "没有要同步的数据" }));
                List<SE_ShareMakeImportProducts> list = new List<SE_ShareMakeImportProducts>();
                Guid BatchGuid = Guid.NewGuid();
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrWhiteSpace(dr["PID"].ToString()))
                        continue;

                    double times = Convert.ToDouble(dr["multiple"].ToString());
                    if (times < 0.1 || times > 10)
                    {
                        return Json(new { Status = -1, Error = "积分奖励倍数不符合，配置的PID：" + dr["PID"].ToString() + "积分奖励倍数:" + dr[2].ToString() }, "text/html");
                    }
                    list.Add(new SE_ShareMakeImportProducts()
                    {
                        BatchGuid = BatchGuid.ToString(),
                        IsMakeMoney = true,
                        IsShare = false,
                        Orderby = i++,
                        PID = dr["PID"].ToString(),
                        Times = dr["multiple"].ToString()
                    });
                }

                using (var client = new Tuhu.Service.Product.ProductClient())
                {
                    int baseNumber = 1000;
                    int number = (list.Count / baseNumber);
                    for (int j = 0; j <= number; j++)
                    {
                        var temp = list.GetRange(j * baseNumber, list.Count < baseNumber ? list.Count : ((j + 1) * baseNumber - list.Count) > 0 ? (list.Count - (j * baseNumber)) : baseNumber);
                        var result = client.SelectSkuProductListByPids(temp.Select(o => o.PID).ToList());
                        result.ThrowIfException(true);

                        if (result.Success)
                        {

                            var pids = result.Result.Select(o => o.Pid);
                            foreach (var item in temp)
                                if (!pids.Contains(item.PID))
                                    return Content(JsonConvert.SerializeObject((new { Status = -1, Error = "PID配置有误:PID:" + item.PID })));
                                else
                                {
                                    if (temp.Where(o => o.PID == item.PID).Count() > 1)
                                        return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "PID配置有误,同一批次不能有相同的:PID:" + item.PID }));
                                    var product = result.Result.Where(o => o.Pid == item.PID).FirstOrDefault();
                                    if (product == null)
                                        return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "PID未查询出来误:PID:" + item.PID }));
                                    else
                                        item.DisplayName = product.DisplayName;
                                }
                        }
                        else
                        {

                            return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "调用服务验证产品信息失败" + result.ErrorMessage }));
                        }
                    }
                }


                int resultID = manager.AddProducts(list, id);
                if (resultID > 0)
                    return Content(JsonConvert.SerializeObject(new { Status = 0, ID = resultID }));
                else
                    return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "保存失败" }));


            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "请检查文件内容格式是否正确",ErrorMsg = em.Message }));
            }
        }



        [ValidateInput(false)]
        public ActionResult Save(SE_ShareMakeMoneyConfig model)
        {
            ShareMakeMoneyManager manager = new ShareMakeMoneyManager();

            if (manager.Save(model, User.Identity.Name))
                return Json(1);
            else
                return Json(0);

        }


        public ActionResult SelectProducts(string datetime, string pid, string displayName, string times, string status, int id)
        {
            ShareMakeMoneyManager manager = new ShareMakeMoneyManager();
            return Content(JsonConvert.SerializeObject(manager.GetProductEntities(datetime, pid, displayName, times, status, id).OrderByDescending(p => p.CreateDate).Select(o => new
            {
                BatchGuid = o.BatchGuid,
                CreateDate = string.Format("{0:yyyy-MM-dd}", o.CreateDate),
                DisplayName = o.DisplayName,
                ID = o.ID,
                IsMakeMoney = o.IsMakeMoney == true ? "启用" : "禁用",
                IsShare = o.IsShare == true ? "显示" : "不显示",
                Orderby = o.Orderby,
                PID = o.PID,
                Times = o.Times
            })));
        }

        public ActionResult DeleteProduct(int id)
        {
            return new ShareMakeMoneyManager().DeleteProduct(id) == true ? Json(1) : Json(0);
        }


        public ActionResult Delete(int id)
        {
            if (new ShareMakeMoneyManager().Delete(id))
                return Json(1);
            else
                return Json(0);

        }


        public ActionResult UpdateProductIsShare(int id, int isShare)
        {
            return new ShareMakeMoneyManager().UpdateProductIsShare(id, isShare.ToString()) == true ? Json(1) : Json(0);
        }


    }
}

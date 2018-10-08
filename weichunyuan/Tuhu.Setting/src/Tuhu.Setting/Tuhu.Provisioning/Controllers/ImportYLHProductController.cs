using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Provisioning.Business.YLHProduct;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft;
using NPOI.XSSF.UserModel;


namespace Tuhu.Provisioning.Controllers
{
    public class ImportYLHProductController : Controller
    {
        //
        // GET: /ImportYLHProduct/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]

        public JsonResult ImportGrade()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new {Status = -1, Error = "请上传.xlsx文件或者.xls文件！"}, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("Sheet1", true);

                    List<YLHProductModel> errordt = new List<YLHProductModel>();
                    // var exceldate=new Controls.ExcelHelper(new mer)

                    #region 批量将读取到的excel数据导入到数据库

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        DataRow dr = dt.Rows[i];
                        string productId = "YLH-" + dr["ProductNunber"]?.ToString();
                        string variantId = "1";
                        var product = new WholeProductInfo
                            #region 创建增加的产品

                        {
                            FactoryNumber = dr["ProductNunber"]?.ToString(),
                            ProductID = "YLH-" + dr["ProductNunber"]?.ToString(),
                            VariantID = "1",
                            cy_list_price =
                                Convert.ToDecimal(dr["cy_list_price"].ToString() == string.Empty
                                    ? 9999
                                    : dr["cy_list_price"]),

                            DisplayName = dr["ProductName"]?.ToString() + dr["Specification"]?.ToString(),
                            CatalogName = "CarPAR",
                            PrimaryParentCategory = "YLHProduct",
                            DefinitionName = "YLHProduct",
                            Image_filename = "/Images/Products/1609/1CA808C19E3FF0E9.jpg",
                            Description = "<p></p>"
                        };
                        product.DisplayName = product.DisplayName.Length < 128
                            ? product.DisplayName
                            : dr["ProductName"]?.ToString();

                        #endregion

                        var ylhProduct = new YLHProductModel();

                        #region 创建永隆行各商品的对照关系;

                        ylhProduct.StoreName = dr["StoreName"]?.ToString();
                        ylhProduct.ProductType1St = dr["[1stProductType]"]?.ToString();
                        ylhProduct.ProductType2Nd = dr["[2ndProductType]"]?.ToString();
                        ylhProduct.ProductType3Rd = dr["[3rdProductType]"]?.ToString();
                        ylhProduct.ProductType4Th = dr["[4thProductType]"]?.ToString();
                        ylhProduct.ProductType5Th = dr["[5thProductType]"]?.ToString();
                        ylhProduct.counter_id = Convert.ToInt32(dr["counter_id"]);
                        ylhProduct.ProductNunber = dr["ProductNunber"]?.ToString();
                        ylhProduct.ProductName = dr["ProductName"]?.ToString();
                        ylhProduct.Specification = dr["Specification"]?.ToString();
                        ylhProduct.Price =
                            Convert.ToDecimal(dr["Price"].ToString() == string.Empty ? 9999 : dr["Price"]);
                        ylhProduct.SystemQuantity = float.Parse(dr["SystemQuantity"].ToString());
                        ylhProduct.SyetemSettlement =
                            Convert.ToDecimal(dr["SyetemSettlement"].ToString() == string.Empty
                                ? 9999
                                : dr["SyetemSettlement"]);
                        ylhProduct.RealQuantity = float.Parse(dr["RealQuantity"].ToString());
                        ylhProduct.RealSettlement =
                            Convert.ToDecimal(dr["RealSettlement"].ToString() == string.Empty
                                ? 9999
                                : dr["RealSettlement"]);
                        ylhProduct.QuantityDiff = Convert.ToInt32(dr["QuantityDiff"].ToString() == string.Empty
                            ? ""
                            : dr["QuantityDiff"]);
                        ylhProduct.SettlementDiff =
                            Convert.ToDecimal(dr["SettlementDiff"].ToString() == string.Empty
                                ? 9999
                                : dr["SettlementDiff"]);
                        ylhProduct.DiffReason = dr["DiffReason"]?.ToString();
                        ylhProduct.LastPurchaseDate = Convert.ToDateTime(dr["LastPurchaseDate"]);
                        ylhProduct.YearInWareHouse = Convert.ToInt32(dr["YearInWareHouse"]);
                        ylhProduct.DayInWareHouse = Convert.ToInt32(dr["DayInWareHouse"]);
                        ylhProduct.DistributionAmount =
                            Convert.ToDecimal(dr["DistributionAmount"].ToString() == string.Empty
                                ? 9999
                                : dr["DistributionAmount"]);
                        ylhProduct.BuyoutAmount = Convert.ToDecimal(dr["BuyoutAmount"].ToString() == string.Empty
                            ? 9999
                            : dr["BuyoutAmount"]);
                        ylhProduct.MonthlySales = float.Parse(dr["MonthlySales"].ToString());
                        ylhProduct.QualityClassification = dr["QualityClassification"]?.ToString();
                        ylhProduct.Remark = dr["Remark"]?.ToString();
                        ylhProduct.cy_list_price = Convert.ToDecimal(dr["cy_list_price"].ToString() == string.Empty
                            ? 9999
                            : dr["cy_list_price"]);
                        ylhProduct.MonthInWareHouse = Convert.ToInt32(dr["MonthInWareHouse"]);
                        ylhProduct.PID = dr["PID"]?.ToString();

                        #endregion


                        if (dr["PID"].ToString() == string.Empty) //产品在数据库中不存在，需要通过服务导入到产品库
                        {
                            //通过服务创建产品库的产品
                            using (var client = new ProductClient())
                            {
                                var result =
                                    client.Invoke(
                                        o => o.CreateProduct(product, User.Identity.Name, ChannelType.Tuhu));
                                if (!result.Success)
                                {
                                    //服务调用失败
                                }
                            }
                        }
                        else
                        {
                            #region 如果数据库中存在pid则通过已有的pid找到Oid

                            var tempId = dr["PID"].ToString();
                            var Index = tempId.IndexOf('|');
                            if (Index > 0)
                            {
                                //pid有variantId,i_class_type=2
                                productId = tempId.SafeSubstring(0, Index);
                                variantId = tempId.Substring(Index + 1);
                            }
                            else
                            {
                                //pid没有variantId,i_class_type=4
                                productId = tempId.Substring(Index + 1);
                                variantId = "";
                            }

                            #endregion
                        }
                        //查找oid                          
                        ylhProduct.oid = YLHProductManager.GetOid(productId, variantId);
                        int ylhCount = YLHProductManager.CheckoutProduct(ylhProduct.ProductNunber,
                            ylhProduct.counter_id ?? 0);
                        if (ylhProduct.oid == 0) //产品库中没有该产品
                        {
                            errordt.Add(ylhProduct);
                        }
                        else if (ylhCount > 0) //已存在该永隆行产品
                        {
                        }
                        else //添加产品
                        {
                            YLHProductManager.AddProducts(ylhProduct);
                        }
                        if (i%100 == 0)
                        {
                            Thread.Sleep(3000); //每执行100条数据，休眠3秒钟
                        }

                    }

                    #endregion

                    #region 将问题输数据导出到excel

                    using (
                        MemoryStream ms =
                            new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/Content/Export/分享赚钱商品.xlsx")))
                        )
                    {
                        if (errordt.Any())
                        {
                            //创建工作簿对象
                            XSSFWorkbook book = new XSSFWorkbook(ms); //创建excel 2007工作簿对象，
                            //创建工作表
                            ISheet sheet1 = book.GetSheetAt(0);
                            //创建行row
                            IRow row1 = sheet1.CreateRow(0);

                            #region 工作簿的首行，头部标题

                            row1.CreateCell(0).SetCellValue("StoreName");
                            row1.CreateCell(1).SetCellValue("[1stProductType]");
                            row1.CreateCell(2).SetCellValue("[2ndProductType]");
                            row1.CreateCell(3).SetCellValue("[3rdProductType]");
                            row1.CreateCell(4).SetCellValue("[4thProductType]");
                            row1.CreateCell(5).SetCellValue("[5thProductType]");
                            row1.CreateCell(6).SetCellValue("counter_id");
                            row1.CreateCell(7).SetCellValue("ProductNunber");
                            row1.CreateCell(8).SetCellValue("ProductName");
                            row1.CreateCell(9).SetCellValue("Specification");
                            row1.CreateCell(10).SetCellValue("Price");
                            row1.CreateCell(11).SetCellValue("cy_list_price");
                            row1.CreateCell(12).SetCellValue("SystemQuantity");
                            row1.CreateCell(13).SetCellValue("SyetemSettlement");
                            row1.CreateCell(14).SetCellValue("RealQuantity");
                            row1.CreateCell(15).SetCellValue("RealSettlement");
                            row1.CreateCell(16).SetCellValue("QuantityDiff");
                            row1.CreateCell(17).SetCellValue("SettlementDiff");
                            row1.CreateCell(18).SetCellValue("DiffReason");
                            row1.CreateCell(19).SetCellValue("LastPurchaseDate");
                            row1.CreateCell(20).SetCellValue("YearInWareHouse");
                            row1.CreateCell(21).SetCellValue("MonthInWareHouse");
                            row1.CreateCell(22).SetCellValue("DayInWareHouse");

                            row1.CreateCell(23).SetCellValue("DistributionAmount");
                            row1.CreateCell(24).SetCellValue("BuyoutAmount");
                            row1.CreateCell(25).SetCellValue("MonthlySales");
                            row1.CreateCell(26).SetCellValue("QualityClassification");
                            row1.CreateCell(27).SetCellValue("Remark");
                            row1.CreateCell(28).SetCellValue("PID");

                            #endregion

                            for (var i = 0; i < errordt.Count(); i++)
                            {
                                YLHProductModel item = errordt[i];

                                var row = sheet1.CreateRow(i + 1);

                                #region 将list换成行放在工作簿中

                                row.CreateCell(0).SetCellValue(item.StoreName);
                                row.CreateCell(1).SetCellValue(item.ProductType1St);
                                row.CreateCell(2).SetCellValue(item.ProductType2Nd);
                                row.CreateCell(3).SetCellValue(item.ProductType3Rd);
                                row.CreateCell(4).SetCellValue(item.ProductType4Th);
                                row.CreateCell(5).SetCellValue(item.ProductType5Th);
                                row.CreateCell(6).SetCellValue(item.counter_id ?? 9999);
                                row.CreateCell(7).SetCellValue(item.ProductNunber);
                                row.CreateCell(8).SetCellValue(item.ProductName);
                                row.CreateCell(9).SetCellValue(item.Specification);
                                row.CreateCell(10).SetCellValue(item.Price.ToString());

                                row.CreateCell(11).SetCellValue(item.cy_list_price.ToString());
                                row.CreateCell(12).SetCellValue(item.SystemQuantity.ToString());
                                row.CreateCell(13)
                                    .SetCellValue(item.SyetemSettlement.ToString(CultureInfo.InvariantCulture));
                                row.CreateCell(14).SetCellValue(item.RealQuantity.ToString());
                                row.CreateCell(15).SetCellValue(item.RealSettlement.ToString());
                                row.CreateCell(16).SetCellValue(item.QuantityDiff);
                                row.CreateCell(17).SetCellValue(item.SettlementDiff.ToString());
                                row.CreateCell(18).SetCellValue(item.DiffReason);
                                row.CreateCell(19)
                                    .SetCellValue(
                                        Convert.ToDateTime(item.LastPurchaseDate.ToString() ?? "")
                                            .ToString(CultureInfo.InvariantCulture));
                                row.CreateCell(20).SetCellValue(item.YearInWareHouse.ToString());

                                row.CreateCell(21).SetCellValue(item.MonthInWareHouse.ToString());
                                row.CreateCell(22).SetCellValue(item.DayInWareHouse.ToString());
                                row.CreateCell(23).SetCellValue(item.DistributionAmount.ToString());
                                row.CreateCell(24).SetCellValue(item.BuyoutAmount.ToString());
                                row.CreateCell(25).SetCellValue(item.MonthlySales.ToString());
                                row.CreateCell(26).SetCellValue(item.QualityClassification);
                                row.CreateCell(27).SetCellValue(item.Remark);
                                row.CreateCell(28).SetCellValue(item.PID);

                                #endregion
                            }
                            Response.ContentType = "application/vnd.ms-excel";
                            Response.Charset = "";
                            Response.AppendHeader("Content-Disposition", "attachment;fileName=永隆行问题商品" + ".xlsx");
                            book.Write(Response.OutputStream);
                            Response.End();
                        }
                    }

                    #endregion

                    return Json(new {Status = 0, Result = "写入完成"}, "text/html");
                }
                return Json(new {Status = -1, Error = "请选中文件"}, "text/html");
            }
            catch (Exception em)
            {
                return Json(new {Status = -2, Error = em.Message}, "text/html");
            }
        }

    }
}

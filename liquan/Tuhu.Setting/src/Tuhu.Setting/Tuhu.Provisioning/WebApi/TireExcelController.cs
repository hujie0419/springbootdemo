using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.WebApi
{
    public class TireExcelController : ApiController
    {

      

        [System.Web.Mvc.HttpPost]
        public async Task<HttpResponseMessage> ExportExcel()
        {
            JObject o = JObject.Parse(ContextRequest["searchCondition"]);
            var selectModel = o.ToObject<PriceSelectModel>();
            selectModel.MatchWarnLine = -99;
            var pagesize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExportSize"]);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var user = HttpContext.Current.User.Identity.Name;
            var fileName = $"轮胎价格管理{user}.xls";

            row.CreateCell(0).SetCellValue("品牌");
            row.CreateCell(1).SetCellValue("PID");
            row.CreateCell(2).SetCellValue("产品名称");
            row.CreateCell(3).SetCellValue("原配车型");
            row.CreateCell(4).SetCellValue("上架状态");
            row.CreateCell(5).SetCellValue("展示状态");
            row.CreateCell(6).SetCellValue("库存");
            row.CreateCell(7).SetCellValue("近7天销量");
            row.CreateCell(8).SetCellValue("近30天销量");
            row.CreateCell(9).SetCellValue("进货价");
            row.CreateCell(10).SetCellValue("最近一次采购价");
            row.CreateCell(11).SetCellValue("理论指导价");
            row.CreateCell(12).SetCellValue("实际指导价");
            row.CreateCell(13).SetCellValue("官网价格");
            row.CreateCell(14).SetCellValue("毛利率");
            row.CreateCell(15).SetCellValue("毛利额");
            row.CreateCell(16).SetCellValue("途虎淘宝");
            row.CreateCell(17).SetCellValue("途虎淘宝2");
            row.CreateCell(18).SetCellValue("途虎天猫1");
            row.CreateCell(19).SetCellValue("途虎天猫2");
            row.CreateCell(20).SetCellValue("途虎天猫3");
            row.CreateCell(21).SetCellValue("途虎天猫4");
            row.CreateCell(22).SetCellValue("途虎京东");
            row.CreateCell(23).SetCellValue("途虎京东旗舰");
            row.CreateCell(24).SetCellValue("途虎京东机油");
            row.CreateCell(25).SetCellValue("途虎京东服务");
            row.CreateCell(26).SetCellValue("汽配龙");
            row.CreateCell(27).SetCellValue("京东自营");
            row.CreateCell(28).SetCellValue("特维轮天猫");
            row.CreateCell(29).SetCellValue("汽车超人零售");
            row.CreateCell(30).SetCellValue("汽车超人批发");
            row.CreateCell(31).SetCellValue("劵后价");
            row.CreateCell(32).SetCellValue("汽配龙毛利额");
            row.CreateCell(33).SetCellValue("汽配龙毛利率");
            row.CreateCell(34).SetCellValue("工厂店毛利额");
            row.CreateCell(35).SetCellValue("工厂店毛利率");
            row.CreateCell(36).SetCellValue("采购在途");
            row.CreateCell(37).SetCellValue("是否防爆");
            row.CreateCell(38).SetCellValue("天猫养车零配件官方直营");

            PagerModel pager = new PagerModel(1, pagesize);
            var list = PriceManager.SelectPriceProductList(selectModel, pager, true);
            var i = 0;
            while (list.Any())
            {
                var pids = list.Select(g => g.PID).ToList();
              
                //var activePrices = PriceManager.SelectActivePriceByPids(pids);
                var WhiteList = StockoutStatusManager.GetShowStatusByPids(pids);
                //var CaigouZaitu = PriceManager.SelectCaigouZaituByPids(pids);

                foreach (var item in list)
                {
                    //var val = activePrices.Where(g => g.PID == item.PID).FirstOrDefault();
                    var Status = WhiteList.Where(g => g.PID == item.PID).FirstOrDefault();
                    var guidePriceStr = item.cost == null ? "" : (item.cost.Value + item.cost.Value * item.JiaQuan / 100).ToString("0.00");
                    //item.ActivePrice = val == null ? null : val.ActivePrice;
                    item.ShowStatus = Status == null ? 0 : Status.ShowStatus;
                    //item.CaigouZaitu = CaigouZaitu?.Where(g => g.PID == item.PID).FirstOrDefault()?.CaigouZaitu;
                    var guidePriceStr_1 = "";
                    if (guidePriceStr == "")
                    {
                        if (item.JDSelfPrice != null)
                        {
                            guidePriceStr_1 = item.JDSelfPrice.Value.ToString("0.00");
                        }
                    }
                    else
                    {
                        if (item.JDSelfPrice == null)
                        {
                            guidePriceStr_1 = guidePriceStr;
                        }
                        else
                        {
                            if (Convert.ToDecimal(guidePriceStr) >= Convert.ToDecimal(item.JDSelfPrice))
                            {
                                guidePriceStr_1 = item.JDSelfPrice.Value.ToString("0.00");
                            }
                            else
                            {
                                guidePriceStr_1 = guidePriceStr;
                            }
                        }
                    }

                    var onSale = "";

                    onSale = item.OnSale == 0 ? "下架" : "上架";

                    var ShowStatus = "N/A";
                    if (item.ShowStatus == 1)
                    {
                        ShowStatus = "有货";
                    }
                    else if (item.ShowStatus == 2)
                    {
                        ShowStatus = "缺货";
                    }
                    else if (item.ShowStatus == 3)
                    {
                        ShowStatus = "不展示";
                    }
                   
                    var QPLProfi = item.QPLPrice != null && item.cost != null ? (item.QPLPrice.Value - item.cost.Value).ToString("0.00") : "-";
                    var QPLProfitRate = item.cost != null && item.QPLPrice != null && item.QPLPrice.Value != 0 ? ((item.QPLPrice.Value - item.cost.Value) / item.QPLPrice.Value).ToString("0.00%") : "-";
                    var ShopProfit = item.Price > 0 && item.QPLPrice != null ? (item.Price - item.QPLPrice.Value).ToString("0.00") : "-";
                    var ShopProfitRate = item.Price > 0 && item.QPLPrice != null ? ((item.Price - item.QPLPrice.Value) / item.Price).ToString("0.00%") : "-";
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                    rowtemp.CreateCell(0).SetCellValue(item.Brand);
                    rowtemp.CreateCell(1).SetCellValue(item.PID);
                    rowtemp.CreateCell(2).SetCellValue(item.ProductName);
                    rowtemp.CreateCell(3).SetCellValue(item.VehicleCount);
                    rowtemp.CreateCell(4).SetCellValue(onSale);
                    rowtemp.CreateCell(5).SetCellValue(ShowStatus);
                    rowtemp.CreateCell(6).SetCellValue(item.totalstock == null ? "" : item.totalstock.Value.ToString());
                    rowtemp.CreateCell(7).SetCellValue(item.num_week == null ? "" : item.num_week.Value.ToString());
                    rowtemp.CreateCell(8).SetCellValue(item.num_month == null ? "" : item.num_month.Value.ToString());
                    rowtemp.CreateCell(9).SetCellValue(item.cost == null ? "" : item.cost.Value.ToString("0.00"));
                    rowtemp.CreateCell(10).SetCellValue(item.PurchasePrice == null ? "" : item.PurchasePrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(11).SetCellValue(guidePriceStr);
                    rowtemp.CreateCell(12).SetCellValue(guidePriceStr_1);
                    rowtemp.CreateCell(13).SetCellValue(item.Price.ToString("0.00"));
                    rowtemp.CreateCell(14).SetCellValue(item.cost.GetValueOrDefault(0) > 0 && item.Price > 0 ? ((item.Price - item.cost.Value) / item.Price).ToString("0.00%") : "");
                    rowtemp.CreateCell(15).SetCellValue(item.cost.GetValueOrDefault(0) > 0 && item.Price > 0 ? (item.Price - item.cost.Value).ToString("0.00") : "");
                    rowtemp.CreateCell(16).SetCellValue(item.TBPrice == null ? "" : item.TBPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(17).SetCellValue(item.TB2Price == null ? "" : item.TB2Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(18).SetCellValue(item.TM1Price == null ? "" : item.TM1Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(19).SetCellValue(item.TM2Price == null ? "" : item.TM2Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(20).SetCellValue(item.TM3Price == null ? "" : item.TM3Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(21).SetCellValue(item.TM4Price == null ? "" : item.TM4Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(22).SetCellValue(item.JDPrice == null ? "" : item.JDPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(23).SetCellValue(item.JDFlagShipPrice == null ? "" : item.JDFlagShipPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(24).SetCellValue(item.JDJYPrice == null ? "" : item.JDJYPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(25).SetCellValue(item.JDFWPrice == null ? "" : item.JDFWPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(26).SetCellValue(item.QPLPrice == null ? "" : item.QPLPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(27).SetCellValue(item.JDSelfPrice == null ? "" : item.JDSelfPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(28).SetCellValue(item.TWLTMPrice == null ? "" : item.TWLTMPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(29).SetCellValue(item.MLTTMPrice == null ? "" : item.MLTTMPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(30).SetCellValue(item.MLTPrice == null ? "" : item.MLTPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(31).SetCellValue(item.CouponPrice == null ? "" : item.CouponPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(32).SetCellValue(QPLProfi);
                    rowtemp.CreateCell(33).SetCellValue(QPLProfitRate);
                    rowtemp.CreateCell(34).SetCellValue(ShopProfit);
                    rowtemp.CreateCell(35).SetCellValue(ShopProfitRate);
                    rowtemp.CreateCell(36).SetCellValue(item.CaigouZaitu == null ? "" : item.CaigouZaitu.Value.ToString());
                    rowtemp.CreateCell(37).SetCellValue(item.Rof ?? "");
                    rowtemp.CreateCell(38).SetCellValue(item.TMLPJPrice == null ? "" : item.TMLPJPrice.Value.ToString("0.00"));
                }
                pager.CurrentPage += 1;
                list = PriceManager.SelectPriceProductList(selectModel, pager, true);
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(fileName)
            };
            return response;
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage ExportRecommendByVehicleExcel()
        {
            JObject o = JObject.Parse(ContextRequest["searchCondition"]);
            var selectModel = o.ToObject<VehicleRequestModel>();




            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var user = HttpContext.Current.User.Identity.Name;
            var fileName = $"轮胎人工推荐配置{user}.xls";

            row.CreateCell(0).SetCellValue("品牌序列");
            row.CreateCell(1).SetCellValue("品牌");
            row.CreateCell(2).SetCellValue("车系");
            row.CreateCell(3).SetCellValue("规格");
            row.CreateCell(4).SetCellValue("车价");
            row.CreateCell(5).SetCellValue("1 人工推荐");
            row.CreateCell(6).SetCellValue("推荐理由");
            row.CreateCell(7).SetCellValue("2 人工推荐");
            row.CreateCell(8).SetCellValue("推荐理由");
            row.CreateCell(9).SetCellValue("3 人工推荐");
            row.CreateCell(10).SetCellValue("推荐理由");


            var list = RecommendManager.SelectListNew(selectModel.Departments, selectModel.VehicleIDS, selectModel.PriceRanges, selectModel.VehicleBodyTypes,
                selectModel.Specifications, selectModel.Brands, selectModel.IsShow, selectModel.IsRof, selectModel.PID, selectModel.Province, 
                selectModel.City,selectModel.StartPrice,selectModel.EndPrice);
            var i = 0;

            foreach (var item in list)
            {
               
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                rowtemp.CreateCell(0).SetCellValue(item.BrandCategory ?? "其它");
                rowtemp.CreateCell(1).SetCellValue(item.Brand);
                rowtemp.CreateCell(2).SetCellValue(item.Vehicle);
                rowtemp.CreateCell(3).SetCellValue(item.TireSize);
                rowtemp.CreateCell(4).SetCellValue(item.MinPrice);
                rowtemp.CreateCell(5).SetCellValue(item.RecommendTires?.FirstOrDefault(C => C.Postion == 1)?.PID ?? "无");
                rowtemp.CreateCell(6).SetCellValue(item.RecommendTires?.FirstOrDefault(C => C.Postion == 1)?.Reason ?? "无");
                rowtemp.CreateCell(7).SetCellValue(item.RecommendTires?.FirstOrDefault(C => C.Postion == 2)?.PID ?? "无");
                rowtemp.CreateCell(8).SetCellValue(item.RecommendTires?.FirstOrDefault(C => C.Postion == 2)?.Reason ?? "无");
                rowtemp.CreateCell(9).SetCellValue(item.RecommendTires?.FirstOrDefault(C => C.Postion == 3)?.PID ?? "无");
                rowtemp.CreateCell(10).SetCellValue(item.RecommendTires?.FirstOrDefault(C => C.Postion == 3)?.Reason ?? "无");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(fileName)
            };
            return response;
        }


        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage ExportQZTJIndexExcel()
        {
            JObject o = JObject.Parse(ContextRequest["searchCondition"]);
            var selectModel = o.ToObject<QZTJSelectModel>(); 

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var user = HttpContext.Current.User.Identity.Name;
            var fileName = $"轮胎强制推荐配置{user}.xls";

            row.CreateCell(0).SetCellValue("原轮胎");
            row.CreateCell(1).SetCellValue("推荐轮胎1");
            row.CreateCell(2).SetCellValue("推荐原因1");
            row.CreateCell(3).SetCellValue("轮胎图片1");
            row.CreateCell(4).SetCellValue("推荐轮胎2");
            row.CreateCell(5).SetCellValue("推荐原因2");
            row.CreateCell(6).SetCellValue("轮胎图片2");
            row.CreateCell(7).SetCellValue("推荐轮胎3");
            row.CreateCell(8).SetCellValue("推荐原因3");
            row.CreateCell(9).SetCellValue("轮胎图片3");
            row.CreateCell(10).SetCellValue("浮层推荐PID");
            row.CreateCell(11).SetCellValue("浮层推荐语");

            var pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExportSize"]);
            PagerModel pager = new PagerModel(1, pageSize);
            var list = RecommendManager.SelectQZTJTires(selectModel, pager);
            var i = 0;
            while (list.Any())
            {
                foreach (var item in list)
                {
                  
                    
                    var temp1 = item.Products.FirstOrDefault(C => C.Postion == 1);
                    var temp2 = item.Products.FirstOrDefault(C => C.Postion == 2);
                    var temp3 = item.Products.FirstOrDefault(C => C.Postion == 3);
                    var temp4 = item.Products.FirstOrDefault(C => C.Postion == 4);

                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                    rowtemp.CreateCell(0).SetCellValue(item.PID);
                    rowtemp.CreateCell(1).SetCellValue(string.IsNullOrWhiteSpace(temp1?.PID) ? "无": temp1?.RecommendPID);
                    rowtemp.CreateCell(2).SetCellValue(string.IsNullOrWhiteSpace(temp1?.Reason) ? "无" : temp1?.Reason);
                    rowtemp.CreateCell(3).SetCellValue(string.IsNullOrWhiteSpace(temp1?.Image) ? "无" : "https://img1.tuhu.org" +temp1?.Image);

                    rowtemp.CreateCell(4).SetCellValue(string.IsNullOrWhiteSpace(temp2?.PID) ? "无" : temp2?.RecommendPID);
                    rowtemp.CreateCell(5).SetCellValue(string.IsNullOrWhiteSpace(temp2?.Reason) ? "无" : temp2?.Reason);
                    rowtemp.CreateCell(6).SetCellValue(string.IsNullOrWhiteSpace(temp2?.Image) ? "无" : "https://img1.tuhu.org" + temp2?.Image);

                    rowtemp.CreateCell(7).SetCellValue(string.IsNullOrWhiteSpace(temp3?.PID) ? "无" : temp3?.RecommendPID);
                    rowtemp.CreateCell(8).SetCellValue(string.IsNullOrWhiteSpace(temp3?.Reason) ? "无" : temp3?.Reason);
                    rowtemp.CreateCell(9).SetCellValue(string.IsNullOrWhiteSpace(temp3?.Image) ? "无" : "https://img1.tuhu.org" + temp3?.Image);

                    rowtemp.CreateCell(10).SetCellValue(string.IsNullOrWhiteSpace(temp4?.PID) ? "无" : temp4?.RecommendPID);
                    rowtemp.CreateCell(11).SetCellValue(string.IsNullOrWhiteSpace(temp4?.Reason) ? "无" : temp4?.Reason);
                }
                pager.CurrentPage += 1;
                list = RecommendManager.SelectQZTJTires(selectModel, pager);
            }

         
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(fileName)
            };
            return response;
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage ExportListActivityListExcel()
        {
            JObject o = JObject.Parse(ContextRequest["searchCondition"]);
            var selectModel = o.ToObject<ListActCondition>();

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var fileName = $"轮胎列表页活动配置{ThreadIdentity.Operator.Name}.xls";

          
            row.CreateCell(0).SetCellValue("品牌");
            row.CreateCell(1).SetCellValue("车型");
            row.CreateCell(2).SetCellValue("规格");
            row.CreateCell(3).SetCellValue("车价");
            row.CreateCell(4).SetCellValue("活动ID");
            row.CreateCell(5).SetCellValue("活动名称");
            row.CreateCell(6).SetCellValue("优先级");
            row.CreateCell(7).SetCellValue("活动状态");
            row.CreateCell(8).SetCellValue("进行状态");
            row.CreateCell(9).SetCellValue("开始时间");
            row.CreateCell(10).SetCellValue("结束时间");
            row.CreateCell(11).SetCellValue("关联PID");


           
            var pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExportSize"]);
            PagerModel pager = new PagerModel(1, pageSize);
            var list = ListActivityManager.SelectList(selectModel, pager);
            var i = 0;
            while (list.Any())
            {
                foreach (var item in list)
                {
                    
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                  
                    rowtemp.CreateCell(0).SetCellValue(item.Brand);
                    rowtemp.CreateCell(1).SetCellValue(item.Vehicle);
                    rowtemp.CreateCell(2).SetCellValue(item.TireSize);
                    rowtemp.CreateCell(3).SetCellValue(item.MinPrice == null ? "-" : item.MinPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(4).SetCellValue(item.ActivityID == null ? "-" : item.ActivityID.Value.ToString());
                    rowtemp.CreateCell(5).SetCellValue(item.ActivityName ?? "-");
                    rowtemp.CreateCell(6).SetCellValue(item.Sort == 0 ? "-" : (item.Sort == 1 ? "高" : item.Sort == 2 ? "中" : "低"));
                    rowtemp.CreateCell(7).SetCellValue(item.Status == null ? "-" : (item.Status.Value == 0 ? "禁用" : "启用"));
                    rowtemp.CreateCell(8).SetCellValue(item.StartTime == null ? "-" : (item.Status.Value == 0 ? "禁用" : (item.StartTime.Value > DateTime.Now ? "未开始" : (item.EndTime <= DateTime.Now ? "已结束" : "进行中"))));
                    rowtemp.CreateCell(9).SetCellValue(item.StartTime == null ? "-" : item.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    rowtemp.CreateCell(10).SetCellValue(item.EndTime == null ? "-" : item.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    rowtemp.CreateCell(11).SetCellValue(item.RelationPids);



                }
                pager.CurrentPage += 1;
                list = ListActivityManager.SelectList(selectModel, pager);
            }


            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(ms) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(fileName)
            };
            return response;
        }

       



        protected HttpRequestBase ContextRequest
        {
            get
            {
                HttpContextWrapper context = (HttpContextWrapper)Request.Properties["MS_HttpContext"]; //获取传统context
                
                return context.Request; //定义传统request对象
            }
        }

       
    }
}

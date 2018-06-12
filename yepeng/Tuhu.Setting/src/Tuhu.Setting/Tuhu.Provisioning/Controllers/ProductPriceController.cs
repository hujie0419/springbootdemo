using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.Business.ProductInfomation;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ProductPrice;
using Tuhu.Provisioning.Models.CarProductModel;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Member;
using Tuhu.Service;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductPriceController : BaseController
    {
        private readonly Lazy<ActivityManager> lazyActivityManager = new Lazy<ActivityManager>();
        private ActivityManager ActivityManager
        {
            get { return this.lazyActivityManager.Value; }
        }

        [PowerManage(IwSystem = "OperateSys")]
        public async Task<ActionResult> GetCarProductMutliPriceByCatalog(string catalogId, string merchandiseBrand,string couponsString, int onSale,
            int isDaifa, int isOutOfStock, int isHavePintuanPrice, string keyWord, int keyWordSearchType,int curPage, int pageSize)
        {
            try
            {
                int totalRecord = await CarProductPriceManager.GetCarProductMutliPriceByCatalog_Count(catalogId, onSale, isDaifa, isOutOfStock, isHavePintuanPrice, keyWord, keyWordSearchType, merchandiseBrand);
                List<CarProductPriceModel> res = await CarProductPriceManager.GetCarProductMutliPriceByCatalog(catalogId, onSale, isDaifa, isOutOfStock, isHavePintuanPrice, keyWord, keyWordSearchType, curPage, pageSize, merchandiseBrand);

                List<GetCouponRules> coupons = JsonConvert.DeserializeObject<List<GetCouponRules>>(couponsString);
                if (coupons != null && coupons.Count > 0)
                {
                    foreach (var item in res)
                    {
                        List<GetCouponRules> rules = await GetCarProductUsedCouponPriceList(item, coupons);
                        if (rules != null && rules.Count > 0)
                        {

                            item.UsedCouponPrice = (rules.Where(p => p.UsedCouponPrice > 0).FirstOrDefault() ?? new GetCouponRules()).UsedCouponPrice;
                            item.UsedCouponProfit = (rules.Where(p => p.UsedCouponPrice > 0).FirstOrDefault() ?? new GetCouponRules()).UsedCouponProfit;
                        }
                        item.Coupons = rules;
                    }
                }
                return Json(Tuple.Create(res, totalRecord), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                 return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
           
          
        }


        public ActionResult GetCatalogs()
        {
            using (var client = new ProductClient())
            {
                //车品分类查询
                var res = client.GetCategoryDetailLevelsByCategory("AutoProduct");

                if (!res.Success)
                    return Json(new { Success = false });

                var categorys = res.Result.ChildCategorys;
                var list = new List<ForIviewCascaderModel>();

                foreach (var item in categorys) 
                {
                    if (item.Level == 2)
                    {
                        list.Add(new ForIviewCascaderModel
                        {
                            label = item.DisplayName,
                            value = item.Id.ToString(),
                            children = categorys
                            .Where(x => x.ParentId == item.Id && x.Level == 3)
                            .Select(x => new ForIviewCascaderModel
                            {
                                label = x.DisplayName,
                                value = x.Id.ToString(),
                                children=new List<ForIviewCascaderModel>()
                            }).ToList()
                        });
                    }
                }
                return Json(new { Success = true, Data = list });
            }
        }

        /// <summary>
        /// 获取 商品品牌 list
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public JsonResult GetMerchandiseBrands(int CategoryID)
        {
            //ResponseModel _ResponseModel = new ResponseModel();
            using (var client = new ProductClient())
            {
                var res = client.GetCategoryDetailLevelsByCategory("AutoProduct");
                if (!res.Success)
                    return Json(new { Result = 0 ,Message="获取 所有分类出错"});
                List<ChildCategoryModel> categorys = res.Result.ChildCategorys.ToList();

                var category = categorys.Where(p => p.Id == CategoryID).FirstOrDefault();
                if (category == null)
                    return Json(new { Result = 0, Message = "获取 分类异常" });

                List<ProductBrand> MerchandiseBrands = PromotionManager.SelectProductBrand(category.CategoryName).Where(c => !string.IsNullOrWhiteSpace(c.Cp_Brand)).ToList();

                return Json(new { Result =1,Data = MerchandiseBrands } , JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<FileResult> ExportExcel_ForCarProductMutliPriceByCatalog(string catalogId, int onSale,
            int isDaifa, int isOutOfStock, int isHavePintuanPrice, string keyWord, int keyWordSearchType)
        {
            var book = new HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            sheet1.SetColumnWidth(0, 300 * 20);
            sheet1.SetColumnWidth(1, 300 * 80);

            var fileName = $"车品相关商品价格信息{ThreadIdentity.Operator.Name.Split('@')[0]}.xls";

            //标头
            var columns = new List<string> { "PID", "商品名称", "天天秒杀价", "拼团价", "限时抢购价", "官网原价", "采购价格", "是否代发", "上下架", "是否缺货", "义乌仓可用库存", "义乌仓在途库存", "上海仓可用库存", "上海仓在途库存 武汉仓可用库存", "武汉仓在途库存", "北京仓可用库存", "北京仓在途库存", "广州仓可用库存", "广州仓在途库存", "全部可用库存", "全部在途库存" };
            for (int i = 0; i < columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(columns[i]);
            }

            //获取相关数据
            int totalRecord = await CarProductPriceManager.GetCarProductMutliPriceByCatalog_Count(catalogId, onSale, isDaifa, isOutOfStock, isHavePintuanPrice, keyWord, keyWordSearchType);
            var res = await CarProductPriceManager.GetCarProductMutliPriceByCatalog(catalogId, onSale, isDaifa, isOutOfStock, isHavePintuanPrice, keyWord, keyWordSearchType, 1, totalRecord);

            //给每一行赋值
            int ind = 1;
            NPOI.SS.UserModel.IRow rowtemp;
            foreach (var item in res)
            {
                rowtemp = sheet1.CreateRow(ind++);

                #region SetCellValue
                rowtemp.CreateCell(0).SetCellValue(item.PID);
                rowtemp.CreateCell(1).SetCellValue(item.ProductName);
                rowtemp.CreateCell(2).SetCellValue((double)item.DaydaySeckillPrice);
                rowtemp.CreateCell(3).SetCellValue((double)item.PintuanPrice);
                rowtemp.CreateCell(4).SetCellValue((double)item.FlashSalePrice);
                rowtemp.CreateCell(5).SetCellValue((double)item.OriginalPrice);
                rowtemp.CreateCell(6).SetCellValue(0);
                rowtemp.CreateCell(7).SetCellValue(item.IsDaifa ? "是" : "否");
                rowtemp.CreateCell(8).SetCellValue(item.OnSale ? "是" : "否");
                rowtemp.CreateCell(9).SetCellValue(item.StockOut ? "是" : "否");
                rowtemp.CreateCell(10).SetCellValue(item.YW_AvailableStockQuantity);
                rowtemp.CreateCell(11).SetCellValue(item.YW_ZaituStockQuantity);
                rowtemp.CreateCell(12).SetCellValue(item.SH_AvailableStockQuantity);
                rowtemp.CreateCell(13).SetCellValue(item.SH_ZaituStockQuantity);
                rowtemp.CreateCell(14).SetCellValue(item.WH_AvailableStockQuantity);
                rowtemp.CreateCell(15).SetCellValue(item.WH_ZaituStockQuantity);
                rowtemp.CreateCell(16).SetCellValue(item.BJ_AvailableStockQuantity);
                rowtemp.CreateCell(17).SetCellValue(item.BJ_ZaituStockQuantity);
                rowtemp.CreateCell(18).SetCellValue(item.GZ_AvailableStockQuantity);
                rowtemp.CreateCell(19).SetCellValue(item.GZ_ZaituStockQuantity);
                rowtemp.CreateCell(20).SetCellValue(item.TotalAvailableStockQuantity);
                rowtemp.CreateCell(21).SetCellValue(item.TotalZaituStockQuantity);
                #endregion
            }


            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 优惠券 验证 
        /// </summary>
        /// <param name="couponRulePKID"></param>
        /// <returns></returns>
        public JsonResult CouponVlidate(string couponRulePKID)
        {
            try
            {
                JObject coupon = JObject.Parse(ActivityManager.CouponVlidate(couponRulePKID));
                return Json(new { Result = string.IsNullOrWhiteSpace(coupon["GetRuleGUID"].ToString()) ? 0 : 1, Data = coupon }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Message=ex.Message  }, JsonRequestBehavior.AllowGet);
            }
          
           
        }


        /// <summary>
        ///计算 车品使用 优惠券的详情
        /// </summary>
        /// <param name="couponRulePKID"></param>
        /// <returns></returns>
        public async Task<JsonResult>  CalulateCarProductUsedCouponPriceList(string carProductPKID, string couponsString)
        {
            try
            {
                List<GetCouponRules> coupons = JsonConvert.DeserializeObject<List<GetCouponRules>>(couponsString);
                CarProductPriceModel _CarProductPriceModel =await CarProductPriceManager.GetShopProductsByPidsAsync(carProductPKID);
                List<GetCouponRules> rules =await GetCarProductUsedCouponPriceList(_CarProductPriceModel, coupons);
                return Json(new { Result =  1, Data = rules }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = 0, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region private
        /// <summary>
        /// 计算 车品 使用 优惠券 使用所有的优惠券的 价格
        /// </summary>
        /// <param name="_CarProductPriceModel"></param>
        /// <param name="coupons"></param>
        /// <returns></returns>
        private async Task<List<GetCouponRules>> GetCarProductUsedCouponPriceList(CarProductPriceModel _CarProductPriceModel, List<GetCouponRules> coupons)
        {
            List<GetCouponRules> models = new List<GetCouponRules>();

            //获取所有 可用的优惠券 【此处获取的 是大券 的 pkid】
            List<PromotionCouponRulesModel> usefulCoupons = await SelectProductPromotionGetRuleByOidsAsyn(_CarProductPriceModel);
            List<WebCouponActivityRuleModel> coupon = ActivityManager.SelectCouponRuleByCouponRulesPKID(usefulCoupons.Select(p=>p.Pkid).ToList());

            List<Guid> usefulCoupons_Pkid = coupon.Select(p => p.GetRuleGUID).Distinct().ToList();

            foreach (var c in coupons)
            {
                //此处 model 需要重新赋值 不然 会产生覆盖的的问题 
                GetCouponRules model = new GetCouponRules() { UsedCouponPrice = c.UsedCouponPrice, UsedCouponProfit = c.UsedCouponProfit,GetRuleGUID=c.GetRuleGUID,Description=c.Description,Minmoney=c.Minmoney,Discount=c.Discount,CouponDuration=c.CouponDuration,CouponStartTime=c.CouponStartTime };
                //需要 验证优惠券 对商品是否可用
                Guid g = Guid.Empty;
                if (!Guid.TryParse(c.GetRuleGUID, out g))
                {
                    continue;
                }
                model.IsUseful = usefulCoupons_Pkid.Contains(g); //CheckCoupon(_CarProductPriceModel, c);
                if (model.IsUseful)
                {
                    model.UsedCouponPrice = CalulateUseCouponPrice(_CarProductPriceModel, c);

                    //券后毛利 = 券后价格 - 采购价格（代发商品，读取代发价格；非代发商品，读取采购价格）
                    model.UsedCouponProfit = model.UsedCouponPrice - (_CarProductPriceModel.IsDaifa ? _CarProductPriceModel.ContractPrice : _CarProductPriceModel.PurchasePrice) ;
                }
                models.Add(model);
            }
            return models.OrderBy(p=>p.UsedCouponPrice).ToList();
        }

        /// <summary>
        /// 计算 车品 使用单张优惠券的价格
        /// </summary>
        /// <param name="price"></param>
        /// <param name="coupons"></param>
        /// <returns></returns>

        private decimal CalulateUseCouponPrice(CarProductPriceModel _CarProductPriceModel, GetCouponRules coupons)
        {
            //车品的售价
            decimal sellPrice = _CarProductPriceModel.FlashSalePrice == 0 ? _CarProductPriceModel.OriginalPrice : _CarProductPriceModel.FlashSalePrice;
            // 优惠券 的 使用 最低价
            decimal couponsMinmoney = (decimal)Convert.ToDouble(coupons.Minmoney);
            if (sellPrice <= couponsMinmoney)//车品售价 小于或等于 优惠券使用额度
            {
                //使用优惠券 购买的最小 次数
                int num = (int)Math.Ceiling(couponsMinmoney / sellPrice);
                //优惠券 使用后的价格
                coupons.UsedCouponPrice = (num * sellPrice - (decimal)Convert.ToDouble(coupons.Discount)) / num;
            }
            else//车品售价 大于 优惠券使用额度
            {
                coupons.UsedCouponPrice = sellPrice - (decimal)Convert.ToDouble(coupons.Discount);
            }
           
            return coupons.UsedCouponPrice;
        }

    
        /// <summary>
        /// 根据车品的oid 获取 所有可用的优惠券
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private async Task<List<PromotionCouponRulesModel>> SelectProductPromotionGetRuleByOidsAsyn(CarProductPriceModel _CarProductPriceModel)
        {
            List<PromotionCouponRulesModel> models = new List<PromotionCouponRulesModel>();
            using (PromotionClient client =new PromotionClient ())
            {
                OperationResult<IEnumerable<PromotionCouponRulesModel>> res =  await client.SelectPromotionCouponRulesByProducrIDAsync(new string[] { _CarProductPriceModel.PID }, null);
                if (res.Success)
                {
                    models = res.Result.ToList();
                }
                return models;
            }
        }

        #endregion

    }



}
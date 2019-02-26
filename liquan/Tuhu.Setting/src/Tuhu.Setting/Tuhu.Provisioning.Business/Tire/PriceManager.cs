using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Nosql;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Tire
{
    public class PriceManager
    {
        private static readonly ILog logger = LogManager.GetLogger("PriceManager");
        /// <summary>
        /// 根据加权类型获取加权信息
        /// </summary>
        public static IEnumerable<GuidePara> SelectGuideParaByType(string type) => DalTirePrice.SelectGuideParaByType(type);

        public static int SaveGuidePara(GuidePara model)
        {
            if (model.PKID.GetValueOrDefault(0) > 0)
            {
                if (model.Value.GetValueOrDefault(0) == 0 && model.Type != "Base")
                    return DalTirePrice.DeleteGuidePara(model.PKID.Value);
                else
                    return DalTirePrice.UpdateGuidePara(model);
            }
            else
            {
                if (model.Value.GetValueOrDefault(0) != 0)
                    return DalTirePrice.InsertGuidePara(model);
                else
                    return 99;
            }
        }

        public static IEnumerable<TireListModel> SelectPriceProductList(PriceSelectModel model, PagerModel pager,bool isExport=false)
        {
            IEnumerable<TireListModel> result = new List<TireListModel>();
            try
            {
                //兼容逻辑处理
                model.Rof = string.IsNullOrEmpty(model.Rof?.Trim()) ? "不限" : model.Rof.Trim(); //是否防爆 
                model.IsShow = model.IsShow ?? 2; //默认查所有的
                model.StockOut = model.StockOut ?? 2; //默认查所有的
                result = DalTirePrice.SelectPriceProductList2(model, pager,isExport);
                result?.ToList().ForEach((t) =>
                {
                    t.QPLPrice = null;
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }



        public static IEnumerable<Product_Warehouse> GetStock(string pid) => DalTirePrice.GetStock(pid);


        public static int ApplyUpdatePrice(ExamUpdatePriceModel model) => DalTirePrice.ApplyUpdatePrice(model);


        public static IEnumerable<string> GetTireSizesByBrand(string brand)
        {
            var dt = DalTirePrice.GetTireSizesByBrand(brand);

            var list = new List<string>();
            if (dt?.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                {
                    var TireSize = row["TireSize"].ToString();
                    if (!string.IsNullOrWhiteSpace(TireSize))
                        list.Add(TireSize);
                }
            return list;
        }

        public static UpdatePriceBitchReasultModel NoGuidePriceBeyond(string brand, string tireSize, string pattern, int addOrJian, decimal price)
        {
            UpdatePriceBitchReasultModel result = new UpdatePriceBitchReasultModel()
            {
                Status = -99,
            };
            List<string> pids = new List<string>();
            var model = DalTirePrice.SelectUpdateBitch(brand, pattern, tireSize);
            if (!model.Any())
                result.Message = "无满足条件的产品！";
            else
            {
                if (addOrJian < 0 && model.Any(c => c.Price - price < 0))
                    result.Message = "无满足条件的产品！";
                else
                {
                    foreach (var product in model)
                    {
                        if (product.Price > 0 && product.cost == null)
                        {
                            decimal NEW_price;
                            if (addOrJian > 0)
                                NEW_price = product.Price + price;
                            else
                                NEW_price = product.Price - price;
                            if (Math.Abs((NEW_price - product.Price) / product.Price) >= 0.1M)
                                pids.Add(product.PID);
                        }
                    }
                    if (pids.Any())
                    {
                        result.Status = -1;
                        result.Message = String.Join("，", pids);
                    }
                    else
                    {
                        //去修改
                    }
                }

            }
            return result;
        }

        public static IEnumerable<ExamUpdatePriceModel> SelectNeedExamTire() => DalTirePrice.SelectNeedExamTire();

        public static int GotoExam(bool isAccess, string auther, string pid, decimal? cost, decimal? PurchasePrice, int? totalstock, int? num_week, int? num_month, decimal? guidePrice, decimal nowPrice, string maoliLv, string chaochu, decimal? jdself, decimal? maolie) => DalTirePrice.GotoExam(isAccess, auther, pid, cost, PurchasePrice, totalstock, num_week, num_month, guidePrice, nowPrice, maoliLv, chaochu, jdself, maolie);

        public static IEnumerable<ExamUpdatePriceModel> SelectExamLogByPID(string pID, PagerModel pager) => DalTirePrice.SelectExamLogByPID(pID, pager);

        public static IEnumerable<PriceChangeLog> PriceChangeLog(string pid) => DalTirePrice.PriceChangeLog(pid);




        //public static object UpdateListPriceBitch(string brand, string tireSize, string pattern, string addOrJian, string price)
        //{
        //    UpdatePriceBitchReasultModel result = new UpdatePriceBitchReasultModel();
        //    var model = DalTirePrice.SelectUpdateBitch(brand, pattern, tireSize);
        //    if (model.Any())
        //    {
        //        foreach (var product in model)
        //        {
        //            //价格0
        //            //无指导价
        //            //有指导价
        //        }
        //    }

        //}

        public static IEnumerable<ZXTCost> GetZXTPurchaseByPID(string pid) => DalTirePrice.GetZXTPurchaseByPID(pid);

        public static IEnumerable<VehicleModel> LookYuanPeiByPID(string pid) => DalTirePrice.LookYuanPeiByPID(pid);

        public static IEnumerable<WarningLineModel> SelectWarningLine()
        => DalTirePrice.SelectWarningLine();

        public static int UpdateWarningLine(WarningLineModel model)
        => DalTirePrice.UpdateWarningLine(model);

        public static ExamUpdatePriceModel FetchPriceExam(int pkid)
       => DalTirePrice.FetchPriceExam(pkid);

        public static IEnumerable<QiangGouProductModel> GetFlashSalePriceByPID(string pid)
     => DalTirePrice.GetFlashSalePriceByPID(pid);

        public static IEnumerable<CouponPriceHistory> CouponPriceChangeLog(string PID)
            => DalTirePrice.CouponPriceChangeLog(PID);

        /// <summary>
        /// 添加价格修改信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsrtCouponPriceHistory(CouponPriceHistory model) => DalTirePrice.InsrtCouponPriceHistory(model);
        public static int UpdateCouponPrice(CouponPriceHistory model)
            => DalTirePrice.UpdateCouponPrice(model);
        public static int ApplyUpdateCouponPrice(PriceApplyRequest model, string applyperson)
            => DalTirePrice.ApplyUpdateCouponPrice(model, applyperson);
        public static IEnumerable<PriceApply> GetCouponPriceApply()
            => DalTirePrice.GetCouponPriceApply();

        /// <summary>
        /// 查询汽配龙审批列表 或者 日志清单 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<QPLPriceApplyModel> GetQplPriceApplyList(int DataType, string PID)
        {
            var result= DalTirePrice.GetQplPriceApplyList(DataType, PID);
            if (result!=null&& result.Any())
            {
                result.ToList().ForEach((item) =>
                {
                    item.ApplyStatusName = item.ApplyStatus == "1" ? "通过" : item.ApplyStatus == "2" ? "驳回" : "";
                });
            }
         
            return result;
        }

        /// <summary>
        ///  申请价格变更
        /// </summary>
        /// <returns></returns>
        public static int AddQPLPriceApply(string Pid, decimal NowPrice, decimal OldPrice, string ApplyReason,
            string ApplyUser,int ApplyStatus, string Remark)
        {
            return DalTirePrice.AddQPLPriceApply( Pid,  NowPrice,  OldPrice,  ApplyReason,
             ApplyUser,  ApplyStatus, Remark);
        }

        /// <summary>
        ///  审核价格申请
        /// </summary>
        /// <returns></returns>
        public static int UpdateQPLPriceApply(int PKID, int ApplyStatus, string Auditor)
        {
            return DalTirePrice.UpdateQPLPriceApply(PKID, ApplyStatus, Auditor);
        }


        public static string ApprovalCouponPrice(int PKID, bool pass, string ApprovalUser)
            => DalTirePrice.ApprovalCouponPrice(PKID, pass, ApprovalUser);

        public static IEnumerable<PriceApproval> CouponPriceApprovalLog(string PID, PagerModel pager)
            => DalTirePrice.CouponPriceApprovalLog(PID, pager);

        public static IEnumerable<ActivePriceModel> SelectActivePriceByPids(List<string> pids)
            => DalTirePrice.SelectActivePriceByPids(pids);

        public static IEnumerable<CaigouZaituModel> SelectCaigouZaituByPids(List<string> pids)
            => DalTirePrice.SelectCaigouZaituByPids(pids);

        public static int DeleteCouponPrice(string pid,string ChangeUser,decimal? price)
            => DalTirePrice.DeleteCouponPrice(pid, ChangeUser,price);


        public static List<TireCouponModel> TireCouponManage(string ShopName=null)
            => DalTirePrice.TireCouponManage(ShopName);
        public static int AddTireCoupon(string ShopName, decimal QualifiedPrice, decimal Reduce, DateTime EndTime,string Operator)
        {
            var result=DalTirePrice.AddTireCoupon(ShopName, QualifiedPrice, Reduce, EndTime);
            if (result > 0)
            {
                var CouponName = $"满{QualifiedPrice.ToString("00")}减{Reduce.ToString("00")}券";
                DalTirePrice.AddTireCouponLog(ShopName, CouponName, EndTime, Operator, "添加");
            }
            return result;
        }
            

        public static int DeleteTireCoupon(int pkid, string Operator)
        {
            var result= DalTirePrice.DeleteTireCoupon(pkid);
            if (result.PKID > 0)
            {
                var CouponName = $"满{result.QualifiedPrice.ToString("00")}减{result.Reduce.ToString("00")}券";
                DalTirePrice.AddTireCouponLog(result.ShopName, CouponName, result.EndTime, Operator, "删除");
            }
            return result.PKID;
        }
        public static List<TireCouponLogModel> FetchCouponLogByShopName(string ShopName)
            => DalTirePrice.FetchCouponLogByShopName(ShopName);  
        public static TireCouponModel FetchLowestPrice(string ShopName,decimal Price)
        {
            var Count = 4;
            var maxPrice = Count * Price;
            var data = new List<TireCouponModel>();
            using(var client= CacheHelper.CreateCacheClient("TireCouponManage"))
            {
                var cache = client.GetOrSet($"TireCoupon/{ShopName}", ()=>TireCouponManage(ShopName), TimeSpan.FromMinutes(1));
                if (cache.Success && cache.Value.Any())
                {
                    data = cache.Value;
                }
                else
                {
                    data = TireCouponManage(ShopName);
                }
            }
            return data?.Where(g => g.QualifiedPrice < maxPrice)
                .OrderBy(g => Price - g.Reduce / Math.Ceiling(g.QualifiedPrice / Price))
                .ThenBy(g => g.QualifiedPrice)
                .FirstOrDefault();
        }
    }
}

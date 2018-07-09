using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Business.Tire
{
    public class RecommendManager
    {
        #region 根据车型推荐轮胎
        /// <summary> 获取所有车型的品牌系列(德系,日系...)</summary>
        public static IEnumerable<string> GetVehicleDepartment()
        {
            var dt = DALRecommendTire.GetVehicleDepartment();
            if (dt?.Rows.Count == 0)
                return new string[0];
            List<string> list = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(item["BrandCategory"]?.ToString());
            }
            return list;
        }

        /// <summary>获取所有轮胎品牌(宝通/BOTO,三角/TRIANGLE)</summary>
        public static IEnumerable<string> GetBrands()
        {
            var dt = DALRecommendTire.GetBrands();
            if (dt?.Rows.Count == 0)
                return new string[0];
            List<string> list = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(item["CP_Brand"]?.ToString());
            }
            return list;
        }
        /// <summary>
        /// 根据分类获取品牌列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetBrandsByCategoryName(string categoryName)
        {
            var dt = DALRecommendTire.GetBrandsByCategoryName(categoryName);
            if (dt?.Rows.Count == 0)
                return new string[0];
            List<string> list = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(item["CP_Brand"]?.ToString());
            }
            return list;
        }

        /// <summary>获取所有车型类型(轿车,SUV...)</summary>
        public static IEnumerable<string> GetVehicleBodys()
        {
            var dt = DALRecommendTire.GetVehicleBodys();
            if (dt?.Rows.Count == 0)
                return new string[0];
            List<string> list = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(item["VehicleBodyType"]?.ToString());
            }
            return list;
        }
        /// <summary> 根据车型获取适配规格 </summary>
        public static IEnumerable<string> GetSpecificationsByVehicle(string vehicleIDS)
        {
            var dt = DALRecommendTire.GetSpecificationsByVehicle(vehicleIDS);
            if (dt?.Rows.Count == 0)
                return new string[0];
            List<string> list = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                var value = item["Tires"].ToString();
                foreach (var v in value.Split(';'))
                {
                    if (!list.Any(c => c == v) && !string.IsNullOrWhiteSpace(v))
                        list.Add(v);
                }
            }
            return list;
        }
        /// <summary>获取一二级车型供筛选做查询条件</summary>
        public static Dictionary<string, Dictionary<string, List<VehicleModel>>> GetVehicleOneTwoLevel()
        {
            Dictionary<string, Dictionary<string, List<VehicleModel>>> dic = new Dictionary<string, Dictionary<string, List<VehicleModel>>>();
            var list = DALRecommendTire.GetVehicleOneTwoLevel();
            if (list.Any())
            {

                dic = list.Where(v => !string.IsNullOrWhiteSpace(v.Brand)).GroupBy(v => v.Brand.Substring(0, 1)).OrderBy(g => g.Key)
                  .ThenByDescending(g => g.Count()).ToDictionary(g => g.Key, g => g.GroupBy(c => c.Brand).ToDictionary(c => c.Key, c => c.ToList()));
            }
            return dic;
        }

        public static IEnumerable<VehicleTireModel> SelectList(string Departments, string VehicleIDS, string PriceRanges, string VehicleBodyTypes, string Specifications, string Brands, int IsShow, string PID)
        {
            List<VehicleTireModel> list_vehicle_tiresize = new List<VehicleTireModel>();
            //车型表的数据
            var list_vehicle = DALRecommendTire.SelectVehicleList(Departments, VehicleIDS, PriceRanges, VehicleBodyTypes);
            //对车型规格配置的推荐轮胎的数据
            var list_tire = DALRecommendTire.SelectVehicleRecommendTire();
            //对BI车型规格配置的推荐轮胎的数据
            var list_tireBI = DALRecommendTire.SelectVehicleRecommendTireBI();
            if (list_vehicle.Any())
            {
                //根据匹配轮胎尺寸拆分车型数据
                foreach (var vehicle in list_vehicle)
                {
                    foreach (var tiresize in vehicle.Tires.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(tiresize))
                        {
                            list_vehicle_tiresize.Add(new VehicleTireModel()
                            {
                                Brand = vehicle.Brand,
                                BrandCategory = vehicle.BrandCategory,
                                MinPrice = vehicle.MinPrice,
                                TireSize = tiresize,
                                Vehicle = vehicle.Vehicle,
                                TiresMatch = GetMatchBySize(vehicle.TiresMatch, tiresize),//vehicle.TiresMatch,
                                VehicleId = vehicle.VehicleId,
                                RecommendTires = list_tire.Where(c => c.Vehicleid == vehicle.VehicleId && c.TireSize == tiresize).OrderBy(c => c.Postion),
                                RecommendTiresBI = list_tireBI.Where(c => c.Vehicleid == vehicle.VehicleId && c.TireSize == tiresize).OrderByDescending(c => c.Grade).Take(4)
                            });
                        }
                    }
                }
                //将车型表数据根据匹配规格拆分后的集合 
                list_vehicle_tiresize = list_vehicle_tiresize.OrderBy(c => c.VehicleId).ToList();
                if (!Specifications.Contains("不限") && !string.IsNullOrWhiteSpace(Specifications))//根据规格筛选
                    list_vehicle_tiresize = list_vehicle_tiresize.Where(c => Specifications.Contains(c.TireSize)).ToList();

                if (!Brands.Contains("不限") && !string.IsNullOrWhiteSpace(Brands))
                    list_vehicle_tiresize = list_vehicle_tiresize.Where(c => c.RecommendTires.Any(v => Brands.Contains(v.CP_Brand))).ToList();

                if (!string.IsNullOrWhiteSpace(PID))
                    list_vehicle_tiresize = list_vehicle_tiresize.Where(c => c.RecommendTires.Any(v => v.PID == PID)).ToList();
                if (IsShow == 2)
                    list_vehicle_tiresize = list_vehicle_tiresize.Where(c => c.RecommendTires.Any()).ToList();
                else if (IsShow == 3)
                    list_vehicle_tiresize = list_vehicle_tiresize.Where(c => !c.RecommendTires.Any()).ToList();
                else if (IsShow == 4)
                {
                    List<VehicleTireModel> repeatList = new List<VehicleTireModel>();
                    foreach (var item in list_vehicle_tiresize)
                    {
                        List<string> listStr = new List<string>();
                        var flag = false;
                        if (item.RecommendTires.Any())
                        {
                            foreach (var item1 in item.RecommendTires)
                            {
                                if (!string.IsNullOrWhiteSpace(item1.PID))
                                {
                                    if (listStr.Contains(item1.PID))
                                        flag = true;
                                    else
                                        listStr.Add(item1.PID);
                                }
                            }
                        }
                        if (item.RecommendTiresBI.Any())
                        {
                            foreach (var item2 in item.RecommendTiresBI)
                            {
                                if (!string.IsNullOrWhiteSpace(item2.PID))
                                {
                                    if (listStr.Contains(item2.PID))
                                        flag = true;
                                    else
                                        listStr.Add(item2.PID);
                                }
                            }
                        }
                        if (flag)
                            repeatList.Add(item);
                    }
                    list_vehicle_tiresize = repeatList;
                }
            }
            return list_vehicle_tiresize;
        }


        public static IEnumerable<VehicleTireModel> SelectListNew(string Departments, string VehicleIDS, string PriceRanges, string VehicleBodyTypes, string Specifications, string Brands, int IsShow, string PID, string Province, string City)
        {
            var source = DALRecommendTire.SelectListNew(Departments, VehicleIDS, PriceRanges, VehicleBodyTypes, Specifications, Brands, PID, Province, City);
            if (!source.Any())
                return new VehicleTireModel[0];
            var list = source.GroupBy(V => V.VehicleId + V.TireSize).Select(C => new VehicleTireModel()
            {
                Brand = C.FirstOrDefault()?.Brand,
                BrandCategory = C.FirstOrDefault()?.BrandCategory,
                MinPrice = C.FirstOrDefault()?.MinPrice,
                TireSize = C.FirstOrDefault()?.TireSize,
                Vehicle = C.FirstOrDefault()?.Vehicle,
                TiresMatch = C.FirstOrDefault()?.TiresMatch,//GetMatchBySize(C.FirstOrDefault()?.TiresMatch, C.FirstOrDefault()?.TireSize),
                VehicleId = C.FirstOrDefault().VehicleId,
                RecommendTires = C.Select(D => new VehicleTireRecommend()
                {
                    PKID = D.PKID,
                    Vehicleid = D.VehicleId,
                    TireSize = D.TireSize,
                    PID = D.PID,
                    Postion = D.Postion,
                    Reason = D.Reason
                }),
                RecommendTiresBI = C.Select(BI => new VehicleTireRecommend()
                {
                    Vehicleid = BI.VehicleId,
                    TireSize = BI.TireSize,
                    PID = BI.ProductId,
                    Postion = BI.RowNumber
                })
            });

            if (IsShow == 2)
                list = list.Where(c => c.RecommendTires.Any(C => !string.IsNullOrWhiteSpace(C.PID)));
            else if (IsShow == 3)
                list = list.Where(c => !c.RecommendTires.Where(C => !string.IsNullOrWhiteSpace(C.PID)).Any());
            else if (IsShow == 4)
            {
                List<VehicleTireModel> repeatList = new List<VehicleTireModel>();
                foreach (var item in list)
                {
                    List<string> listStr = new List<string>();
                    var flag = false;
                    if (item.RecommendTires.Any())
                    {
                        foreach (var item1 in item.RecommendTires)
                        {
                            if (!string.IsNullOrWhiteSpace(item1.PID))
                            {
                                if (listStr.Contains(item1.PID))
                                    flag = true;
                                else
                                    listStr.Add(item1.PID);
                            }
                        }
                    }
                    if (item.RecommendTiresBI.Any())
                    {
                        foreach (var item2 in item.RecommendTiresBI)
                        {
                            if (!string.IsNullOrWhiteSpace(item2.PID))
                            {
                                if (listStr.Contains(item2.PID))
                                    flag = true;
                                else
                                    listStr.Add(item2.PID);
                            }
                        }
                    }
                    if (flag)
                        repeatList.Add(item);
                }
                list = repeatList;
            }
            return list.ToList();

        }
        public static string GetMatchBySize(string matchTires, string tireSize)
        {
            if (string.IsNullOrWhiteSpace(matchTires) || string.IsNullOrWhiteSpace(tireSize))
                return null;
            var dt = DALRecommendTire.GetMatchBySize(matchTires, tireSize);
            if (dt?.Rows.Count == 0)
                return null;
            List<string> list = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(item["PID"].ToString());
            }
            return string.Join(";", list);
        }
        public static string CheckPID(string PID, string TireSize)
        {
            var dr = DALRecommendTire.CheckPID(PID);
            if (dr == null)
                return "1";//产品不存在
            if (dr["TireSize"].ToString() != TireSize)
                return "3";//尺寸不匹配
            else
                return dr["DisplayName"].ToString();
        }

        /// <summary>
        /// 保存单个车型 单个规格的多个PID
        /// </summary>
        public static int SaveSingle(IEnumerable<VehicleTireRecommend> list) => DALRecommendTire.SaveSingle(list);

        /// <summary>
        /// 保存多个车型 单个规格的多个PID
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int SaveMany(IEnumerable<VehicleTireRecommend> list) => DALRecommendTire.SaveMany(list);

        public static string CheckPIDReplace(string PID, string Type)
        {
            var dr = DALRecommendTire.CheckPID(PID);
            if (dr == null)
                return "-1";//产品不存在

            if (!DALRecommendTire.CheckPIDNew(PID) && Type == "old")
                return "-2";//老的未推荐

            return dr["DisplayName"].ToString();

        }

        public static int ReplacePID(string OldPID, string NewPID, string Reason) => DALRecommendTire.ReplacePID(OldPID, NewPID, Reason);
        public static int Delete(string TireSize, string VehicleID) => DALRecommendTire.Delete(TireSize, VehicleID);

        public static IEnumerable<TireSizeModel> SelectALLTireSize() => DALRecommendTire.SelectALLTireSize();

        public static IEnumerable<TireSizeModel> SelectALLHubSize() => DALRecommendTire.SelectALLHubSize();

        //根据车型和规格取出配置的推荐
        public static IEnumerable<VehicleTireRecommend> SelectTireRecommendByVehicleAndSize(string TireSize, string VehicleID) => DALRecommendTire.SelectTireRecommendByVehicleAndSize(TireSize, VehicleID);

        //根据多个车型和规格取出配置的推荐
        public static IEnumerable<VehicleTireRecommend> SelectTireRecommendByVehicleSAndSize(string TireSize, string VehicleIDS) => DALRecommendTire.SelectTireRecommendByVehicleSAndSize(TireSize, VehicleIDS);

        //根据PID取出配置的推荐
        public static IEnumerable<VehicleTireRecommend> SelectTireRecommendByPID(string PID) => DALRecommendTire.SelectTireRecommendByPID(PID);

        //根据车型ID获取车型信息
        public static VehicleModel FetchVehicleByID(string vehicleId) => DALRecommendTire.FetchVehicleByID(vehicleId);
        #endregion

        #region 轮胎强制推荐

        /// <summary>
        /// 加在强制推荐数据
        /// </summary>
        /// <param name="model">条件</param>
        /// <param name="pager">分页</param>
        /// <returns></returns>
        public static IEnumerable<QZTJModel> SelectQZTJTires(QZTJSelectModel model, PagerModel pager)
        {
            var dt = DALRecommendTire.SelectQZTJTires(model, pager);
            if (dt == null || dt.Rows.Count == 0)
                return new QZTJModel[0];

            List<string> pids = dt.Rows.Cast<DataRow>().Select(row => row["PID"].ToString()).Distinct().ToList();
            var recommendResult = GetTireEnforceRecommendByPids(pids);

            var list =
            dt.Rows.Cast<DataRow>().GroupBy(row => row["PID"].ToString()).Select(T => new QZTJModel()
            {
                PID = T.Key,
                DisplayName = T.FirstOrDefault()["DisplayName"].ToString(),
                TireSize = T.FirstOrDefault()["TireSize"].ToString(),
                Products = recommendResult.Where(item => item.PID == T.Key)
            });
            return list;
        }

        public static List<RecommendProductModel> GetTireEnforceRecommendByPids(List<string> pids)
        {
            var returnResult = new List<RecommendProductModel>();
            if (pids == null || !pids.Any() || pids.Count == 0)
                return returnResult;
            try
            {
                using (var client = new ProductConfigClient())
                {
                    var result = client.GetTireEnforceRecommendByPids(pids.Distinct().ToList());
                    result.ThrowIfException(true);
                    if (result.Success && result.Result != null && result.Result.Any() && result.Result.Count > 0)
                    {
                        var recommendPids = result.Result.Where(p => !string.IsNullOrWhiteSpace(p.RecommendPID))
                            .Select(p => p.RecommendPID)
                            .Distinct()
                            .ToList();
                        var displayNames = DALRecommendTire.GetDisplayName(recommendPids);
                        result.Result.ForEach(item => returnResult.Add(new RecommendProductModel
                        {
                            PID = item.PId,
                            Image = item.Image,
                            Postion = item.Position,
                            Reason = item.Reason,
                            RecommendPID = item.RecommendPID,
                            ProductName = displayNames.Where(p => p.PID == item.RecommendPID).FirstOrDefault().DisplayName
                        }));
                    }
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                //TODO:log
                return returnResult;
            }
        }

        public static void RefreashTireEnforceRecommendCache(IEnumerable<string> pids)
        {
            try
            {
                using (var client = new ProductConfigClient())
                {
                    var clientResult = client.RefreashTireEnforceRecommendCache(pids);
                    clientResult.ThrowIfException(true);
                }
            }
            catch (Exception ex)
            {
                //TODO:log
            }
        }

        public static int SaveTireEnforceRecommend(List<QZTJModel> list)
        {
            List<TireEnforceRecommend> changeList = new List<TireEnforceRecommend>();
            foreach (var recommend in list)
            {
                DALRecommendTire.DeleteQZTJByPID(recommend.PID);
                recommend.Products.ForEach(item => changeList.Add(new TireEnforceRecommend
                {
                    PId = item.PID,
                    RecommendPID = item.RecommendPID,
                    Reason = item.Reason,
                    Position = item.Postion,
                    Image = item.Image,
                    CreateDateTime = DateTime.Now
                }));
            }
            try
            {
                using (var client = new ProductConfigClient())
                {
                    var clientResult = client.CreateTireEnforceRecommend(changeList);
                    clientResult.ThrowIfException(true);
                    if (clientResult.Success) return clientResult.Result;
                    else return 0;
                }
            }
            catch (Exception ex)
            {
                //TODO:log
                return 0;
            }
        }

        public static int DeleteQZTJByPID(string PID) => DALRecommendTire.DeleteQZTJByPID(PID);

        public static string CheckPIDQZTJ(string TireSize, string PID) => DALRecommendTire.CheckPIDQZTJ(TireSize, PID);

        //public static int SaveQZTJSingle(QZTJModel model) => DALRecommendTire.SaveQZTJSingle(model);

        //public static int SaveQZTJMany(List<QZTJModel> list) => DALRecommendTire.SaveQZTJMany(list);

        //public static Dictionary<string, List<string>> SelectQZTJForCache()
        //{
        //    var list = DALRecommendTire.SelectQZTJForCache();

        //    var dic = list.Where(c => c.PID != c.RecommendPID).GroupBy(P => P.PID).ToDictionary(C => C.Key, C => C.Select(P => P.RecommendPID).Distinct().ToList());

        //    return dic;
        //}
        #endregion

        #region 新增券后价相关

        #endregion
    }
}

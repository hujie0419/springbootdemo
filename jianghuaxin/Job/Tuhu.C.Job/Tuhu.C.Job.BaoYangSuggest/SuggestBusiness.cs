using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Models;
using Tuhu.C.Job.BaoYangSuggest.Model;
using Tuhu.Service.TuhuShopWork;
using Tuhu.Service.TuhuShopWork.Models;
using Tuhu.Service.BaoYang.Models.Category;

namespace Tuhu.C.Job.BaoYangSuggest
{
    public class SuggestBusiness
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BaoYangSuggestJob));
        static readonly TimeSpan CacheTime = new TimeSpan(720, 0, 0);

        public static void MaintainBaoYangRecordNewData()
        {
            int maxId = SuggestDal.SelectMaxId();
            int batch = maxId / 10000;
            int index = 1;
            logger.Info($"维护新数据：{batch}");
            while(index <= batch)
            {
                logger.Info($"维护新数据：{index} 批开始");
                SuggestDal.UpdateNewData(index);
                logger.Info($"维护新数据：{index} 批结束");
                index++;
            }
        }

        public static bool UpdateBaoYangRecord()
        {
            bool result = false;

            try
            {
                var caneledIds = new List<int>();
                int pageIndex = 1;

                logger.Info("开始更新用户保养记录！");

                logger.Info($"删除取消订单相关的保养记录, 开始！");
                do
                {
                    logger.Info($"第{pageIndex}批");
                    caneledIds = SuggestDal.SelectCanceledOrderIds(pageIndex, 500);
                    if (caneledIds != null && caneledIds.Any())
                    {
                        logger.Info($"删除取消订单相关的保养记录, 共{caneledIds.Count()}条！");
                        SuggestDal.DeleteBaoYangRecord(caneledIds);
                        logger.Info($"删除取消订单相关的保养记录, 共{caneledIds.Count()}条结束！");
                    }

                    pageIndex++;
                }
                while (caneledIds != null && caneledIds.Any());

                logger.Info($"删除取消订单相关的保养记录, 结束！");

                logger.Info($"更新订单相关的保养记录, 开始！");
                var updateIds = new List<int>();
                pageIndex = 1;
                var config = SuggestDal.SelectUserBaoYangRecordConfig();
                do
                {
                    logger.Info($"第{pageIndex}批");
                    updateIds = SuggestDal.SelectUpdateddOrderIds(pageIndex, 500);
                    var newData = SuggestDal.SelectBaoYangRecordsFromOrder(updateIds);
                    var oldData = SuggestDal.SelectBaoYangRecordsFromMaintainedData(updateIds);

                    var operationResult = CompareData(ConvertOrderData(newData, config), oldData);

                    var toDelete = operationResult.Item1;
                    if(toDelete != null && toDelete.Any())
                    {
                        logger.Info($"更新订单相关的保养记录, 共{operationResult.Item1.Count()}条待删除！");
                        SuggestDal.DeleteBaoYangRecordByPkids(toDelete);
                        logger.Info($"更新订单相关的保养记录, 共{operationResult.Item1.Count()}条待删除结束！");
                    }

                    var toUpdate = operationResult.Item2;
                    if (toUpdate != null && toUpdate.Any())
                    {
                        logger.Info($"更新订单相关的保养记录, 共{operationResult.Item2.Count()}条待更新！");
                        SuggestDal.UpdateBaoYangRecord(toUpdate);
                        logger.Info($"更新订单相关的保养记录, 共{operationResult.Item2.Count()}条待更新结束！");
                    }

                    var toInsert = operationResult.Item3;
                    if (toInsert != null && toInsert.Any())
                    {
                        logger.Info($"更新订单相关的保养记录, 共{operationResult.Item3.Count()}条待插入！");
                        SuggestDal.InsertBaoYangRecords(toInsert);
                        logger.Info($"更新订单相关的保养记录, 共{operationResult.Item3.Count()}条待插入结束！");
                    }

                    pageIndex++;
                }
                while (updateIds != null && updateIds.Any());

                logger.Info($"更新订单相关的保养记录, 完成！");

                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static Tuple<List<int>, List<UserBaoYangRecordModel>, List<UserBaoYangRecordModel>> CompareData(List<UserBaoYangRecordModel> newdata,
            List<UserBaoYangRecordModel> oldData)
        {
            List<int> toDeletePids = new List<int>();
            List<UserBaoYangRecordModel> toUpdateData = new List<UserBaoYangRecordModel>();
            List<UserBaoYangRecordModel> toInsertData = new List<UserBaoYangRecordModel>();

            foreach(var item in newdata)
            {
                if (!oldData.Any(o => o.RelatedOrderID == item.RelatedOrderID
                                    && string.Equals(o.BaoYangType, item.BaoYangType)))
                {
                    toInsertData.Add(item);
                }
            }

            foreach(var item in oldData)
            {
                if (!item.IsDeleted && !newdata.Any(o => o.RelatedOrderID == item.RelatedOrderID
                                    && string.Equals(o.BaoYangType, item.BaoYangType)))
                {
                    toDeletePids.Add(item.PKID);
                }
            }

            foreach(var item in oldData)
            {
                var newDataItem = newdata.FirstOrDefault(o => o.RelatedOrderID == item.RelatedOrderID
                                                            && string.Equals(o.BaoYangType, item.BaoYangType));
                if (newDataItem != null)
                {
                    if (item.IsDeleted)
                    {
                        newDataItem.PKID = item.PKID;
                        toUpdateData.Add(newDataItem);
                    }
                    else if(item.UserID != newDataItem.UserID || !string.Equals(item.VechileID, newDataItem.VechileID)
                        || item.BaoYangDateTime != newDataItem.BaoYangDateTime ||item.Distance != newDataItem.Distance
                        || item.RelatedOrderID != newDataItem.RelatedOrderID || !string.Equals(item.RelatedOrderNo, newDataItem.RelatedOrderNo)
                        || item.Status != newDataItem.Status || item.OrderPrice != newDataItem.OrderPrice || item.InstallShopId != newDataItem.InstallShopId
                        || !string.Equals(item.InstallShopName, newDataItem.InstallShopName))
                    {
                        newDataItem.PKID = item.PKID;
                        toUpdateData.Add(newDataItem);
                    }
                }
            }


            return Tuple.Create(toDeletePids, toUpdateData, toInsertData);
        }

        public static List<UserBaoYangRecordModel> ConvertOrderData(List<UserBaoYangRecordModel> records, List<Model.UserBaoYangRecordConfig> config)
        {
            foreach (var record in records)
            {
                if (config.Any(o => string.Equals(o.ServiceId, record.PID)))
                {
                    record.BaoYangType = config.First(o => string.Equals(o.ServiceId, record.PID)).PackageType;
                }

                if (config.Any(o => string.Equals(o.ProductCategory, record.Category)))
                {
                    record.BaoYangType = config.First(o => string.Equals(o.ProductCategory, record.Category)).PackageType;
                }
            }

            foreach (var orderId in records.Select(o => o.RelatedOrderID).Distinct().ToList())
            {
                var current = records.Where(o => o.RelatedOrderID == orderId);

                if (current.Any(o => string.Equals(o.BaoYangType, "xby")) && current.Any(o => string.Equals(o.BaoYangType, "dby")))
                {
                    records.RemoveAll(o => o.RelatedOrderID == orderId && string.Equals(o.BaoYangType, "xby"));
                }
            }

            records = records.Where(o => !string.IsNullOrEmpty(o.BaoYangType)).
                GroupBy(o => $"{o.RelatedOrderID}/{o.BaoYangType}").Select(o => o.First()).ToList();
            List<MileageInfo> distanceInfos = new List<MileageInfo>();
            using (var client = new WorkShopClient())
            {
                var IdStr = string.Join(",", records.Where(o => o.Status == 1).Select(o => o.RelatedOrderID));
                var serviceResult = client.QueryMileageInfoByMultipleOrder(IdStr);
                if (serviceResult.Success && serviceResult.Result != null && serviceResult.Result.Any())
                {
                    distanceInfos = serviceResult.Result;
                }
            }

            if(distanceInfos != null && distanceInfos.Any())
            {
                foreach(var distanceInfo in distanceInfos)
                {
                    foreach(var record in records)
                    {
                        int orderId = 0;
                        int distance = 0;
                        if(Int32.TryParse(distanceInfo.Mileage, out distance) &&
                            Int32.TryParse(distanceInfo.Order, out orderId) &&
                            record.RelatedOrderID == orderId)
                        {
                            record.Distance = distance;
                        }
                    }
                }
            }

            return records;
        }

        public static Tuple<bool, int> AddCarObjectRecord()
        {
            Tuple<bool,int> result=null;

            try
            {
                result = SuggestDal.AddCarObjectRecord();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message,ex);
            }

            return result;

        }

        public static List<CarObject> GetCarObjectRecordAndBaoYangRecord(int startIndex, int endIndex)
        {
            var result = new List<CarObject>();

            try
            {
                var dt = SuggestDal.GetCarObjectRecordAndBaoYangRecord(startIndex, endIndex).AsEnumerable();
                if (dt.Any())
                {
                    foreach (var row in dt)
                    {
                        var item = new CarObject();
                        Guid userId;
                        Guid carId;
                        DateTime odometerTime;
                        int kmTotal;
                        int buyYear;
                        int buyMonth;
                        DateTime baoYangDateTime;
                        int distance;
                        int nian = 0;
                        bool isTuhuRecord = false;
                        if (Guid.TryParse(row["UserID"].ToString(), out userId) &&
                            Guid.TryParse(row["CarID"].ToString(), out carId) &&
                            DateTime.TryParse(row["OdometerUpdatedTime"].ToString(), out odometerTime) &&
                            (DateTime.TryParse(row["BaoYangDateTime"].ToString(), out baoYangDateTime) ||
                             row["BaoYangDateTime"].ToString().Equals(string.Empty)) &&
                            (int.TryParse(row["Distance"].ToString(), out distance) ||
                             row["Distance"].ToString().Equals(string.Empty)))
                        {
                            item.UserId = userId;
                            item.CarId = carId;
                            item.VehicleId = (string) (row["u_cartype_pid_vid"].ToString());
                            item.OdometerTime = odometerTime;
                            item.I_km_total = int.TryParse(row["i_km_total"].ToString(), out kmTotal)?kmTotal:0;
                            item.BuyYear = int.TryParse(row["i_buy_year"].ToString(), out buyYear) ? buyYear : 0;
                            item.BuyMonth = int.TryParse(row["i_buy_month"].ToString(), out buyMonth)? buyMonth:0;
                            item.Nian = Int32.TryParse(row["u_Nian"].ToString(), out nian)?nian:0;
                            item.BaoyangDateTime = baoYangDateTime;
                            item.BaoYangType = (string) (row["BaoYangType"].ToString());
                            item.BaoYangDistance = int.TryParse(row["Distance"].ToString(), out distance)?distance:0;
                            item.IsTuhuRecord = Boolean.TryParse(row["isTuhuRecord"].ToString(), out isTuhuRecord) && isTuhuRecord;
                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return result;

        }

        public static void BaoYangSuggestBusiness(List<CarObject> carObjectAndBaoYangRecord, Dictionary<string, string> generalTypes)
        {
            try
            {
                var suggestList = new List<SuggestModel>();
                var generalSuggest = new List<BaoYangGeneralSuggest>();
                var carObjectAndBaoYangRecordDistinct =
                    carObjectAndBaoYangRecord.OrderByDescending(x => x.OdometerTime).GroupBy(x => x.CarId)
                        .Select(x => x.FirstOrDefault()).ToList();
                foreach (var item in carObjectAndBaoYangRecordDistinct)
                {
                    var userCarObject =
                        carObjectAndBaoYangRecordDistinct
                            .FirstOrDefault(x => x.UserId.Equals(item.UserId) && x.CarId.Equals(item.CarId));
                    //用户车型最近一次更新车型距离，目的是得到上路时间和行驶里程

                    var userBaoYangRecord =
                        carObjectAndBaoYangRecord.Where(
                            x => x.UserId.Equals(item.UserId) && x.VehicleId.Equals(item.VehicleId))
                            .OrderByDescending(x => x.BaoyangDateTime).ToList(); //用户该车型做的所有保养
                    var lastBaoYangRecord = new Dictionary<string, Tuple<DateTime, int>>();
                    foreach (var record in userBaoYangRecord)
                    {
                        if (!lastBaoYangRecord.ContainsKey(record.BaoYangType))
                        {
                            lastBaoYangRecord[record.BaoYangType] = new Tuple<DateTime, int>(record.BaoyangDateTime,
                                record.BaoYangDistance);
                        }
                    }
                    if (userCarObject != null)
                    {
                        IEnumerable<BaoYangSuggestCategory> suggestCategories = new List<BaoYangSuggestCategory>();
                        //using (var client = new BaoYangClient())
                        //{
                        //    var serviceResult = client.GetSuggestBaoYangCategoriesWithRecord();
                        //    if (serviceResult.Success)
                        //    {
                        //        suggestCategories = serviceResult.Result;
                        //    }
                        //}
                        if (suggestCategories != null && suggestCategories.Any())
                        {
                            int suggestNum = 0;
                            int urgentNum = 0;
                            int veryUrgentNum = 0;

                            foreach (var category in suggestCategories)
                            {
                                if (category.CategoryType.Equals(SuggestCategoryType.Suggest))
                                {
                                    suggestNum += category.BaoYangTypes.Count;
                                }
                                else if (category.CategoryType.Equals(SuggestCategoryType.Urgent))
                                {
                                    urgentNum += category.BaoYangTypes.Count;
                                    veryUrgentNum += category.BaoYangTypes.Count(t => t.IsVeryUrgent == true);
                                }
                            }

                            var tuhuBaoYangRecord = userBaoYangRecord.Where(y => y.IsTuhuRecord);//途虎保养
                            DateTime? lastTuhuBaoYangTime = null;
                            CarObject lastTuhuBaoYang = null;
                            if (tuhuBaoYangRecord.Any())
                            {
                                lastTuhuBaoYang = tuhuBaoYangRecord.FirstOrDefault();
                                lastTuhuBaoYangTime = lastTuhuBaoYang?.BaoyangDateTime;
                            }

                            var suggestItem = new SuggestModel
                            {
                                UserId = item.UserId,
                                VehicleId = item.VehicleId,
                                CarId = item.CarId,
                                SuggestNum = suggestNum,
                                UrgentNum = urgentNum,
                                VeryUrgentNum = veryUrgentNum,
                                LastTuhuBaoYangTime = lastTuhuBaoYangTime ?? (DateTime?) null,
                                SuggestCategory = JsonConvert.SerializeObject(suggestCategories)
                            };
                            suggestList.Add(suggestItem);

                            if (tuhuBaoYangRecord.Any())
                            {
                                if (lastTuhuBaoYang != null&& lastTuhuBaoYangTime!=null)
                                {
                                    var noBaoYangMonth = DateDiff(lastTuhuBaoYang.BaoyangDateTime);
                                    if (noBaoYangMonth >= 6)
                                    {
                                        foreach (var category in suggestCategories)
                                        {
                                            if (category.CategoryType.Equals(SuggestCategoryType.Suggest) ||
                                                category.CategoryType.Equals(SuggestCategoryType.Urgent))
                                            {
                                                var general =
                                                    category.BaoYangTypes.Select(x => x.Type)
                                                        .Where(y => generalTypes.ContainsKey(y));
                                                if (general.Any())
                                                {
                                                    var generalItem = new BaoYangGeneralSuggest
                                                    {
                                                        UserId = item.UserId,
                                                        VehicelId = item.VehicleId,
                                                        NoBaoYangMonth = noBaoYangMonth,
                                                        Suggest =
                                                            general.Select(
                                                                x =>
                                                                    new BaoYangTypeModel()
                                                                    {
                                                                        BaoYangType = x,
                                                                        ZhName = generalTypes[x]
                                                                    }).ToList()
                                                    };
                                                    generalSuggest.Add(generalItem);
                                                }

                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                if (suggestList.Count > 0)
                {
                    UpdateBaoYangSuggest(suggestList);
                }
                if (generalSuggest.Count > 0)
                {
                    var cacheResult = UpdateBaoYangGeneralSuggestCache(generalSuggest);
                    if (!cacheResult)
                    {
                        logger.Error("批量更新缓存generalSuggest失败");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        public static void UpsertCache()
        {
            try
            {
                var result = SuggestDal.TemporarySaveActiveUserSuggest();
                if (result)
                {
                    var index = 0;
                    List<SuggestModel> suggestModels = null;
                    do
                    {
                        var startIndex = index*1000 + 1;
                        var endIndex = (index + 1)*1000;
                        suggestModels = GetActiveUserSuggestModels(startIndex, endIndex);
                        if (suggestModels != null && suggestModels.Any())
                        {
                            foreach (var sugggest in suggestModels)
                            {
                                UpsertCache(sugggest.UserId.ToString(), sugggest.CarId, sugggest.SuggestNum,
                                    sugggest.UrgentNum, sugggest.VeryUrgentNum);
                            }
                            index++;
                            logger.Info("成功向缓存 upsert第" + index + "批数据");
                        }
                    } while (suggestModels != null && suggestModels.Any());
                }
                else
                {
                    logger.Info("没有活跃用户在今天的推荐里");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static bool UpsertCache(string userId, Guid carId, int suggestNum, int urgentNum, int veryUrgentNum)
        {
            bool result = false;
            try
            {
                var key = "AppBaoYangSuggest/" + userId.ToString() + "/" + carId;
                var value = new Tuple<int, int, int>(suggestNum, urgentNum, veryUrgentNum);
                using (var client = new CacheClient())
                {
                    var updateResult =
                        client.UpdateBaoYangSuggestCount(Guid.Parse(userId), carId, suggestNum, urgentNum, veryUrgentNum);
                    result = updateResult.Success && updateResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static List<SuggestModel> GetActiveUserSuggestModels(int startIndex, int endIndex)
        {
            List<SuggestModel> result = null;
            try
            {
                result = SuggestDal.GetActiveUserSuggestModels(startIndex, endIndex);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }


        public static void UpdateBaoYangSuggest(List<SuggestModel> suggestData)
        {
            try
            {
                var dt = PrepareDataTable(suggestData);
                SuggestDal.UpdateBaoYangSuggest(dt);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message,ex);
            }
        }

        public static bool UpdateBaoYangGeneralSuggestCache(List<BaoYangGeneralSuggest> suggestData)
        {
            var result = false;

            try
            {
                using (var client = new BaoYangClient())
                {
                    var cacheResult = client.BatchUpdateGeneralSuggestCache(suggestData);
                    cacheResult.ThrowIfException(true);
                    return result = cacheResult.Success && cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static DataTable PrepareDataTable(List<SuggestModel> suggestData)
        {
            DataTable dt = new DataTable("SuggestData");

            try
            {
                dt.Columns.Add("UserId", Type.GetType("System.Guid"));
                dt.Columns.Add("CarId", Type.GetType("System.Guid"));
                dt.Columns.Add("VehicleId", Type.GetType("System.String"));
                dt.Columns.Add("SuggestNum", Type.GetType("System.Int32"));
                dt.Columns.Add("UrgentNum", Type.GetType("System.Int32"));
                dt.Columns.Add("VeryUrgentNum", Type.GetType("System.Int32"));
                dt.Columns.Add("SuggestTypes", Type.GetType("System.String"));
                dt.Columns.Add("LastTuhuBaoYangTime", Type.GetType(" System.DateTime"));

                foreach (var suggest in suggestData)
                {
                    dt.Rows.Add(new object[]
                    {
                        suggest.UserId, suggest.CarId, suggest.VehicleId, suggest.SuggestNum, suggest.UrgentNum,
                        suggest.VeryUrgentNum, suggest.SuggestCategory, suggest.LastTuhuBaoYangTime
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return dt;
        }
        /// <summary>
        /// 求月数时间差
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private static int DateDiff(DateTime datetime)
        {
            DateTime now = DateTime.Now;
            DateTime temp = datetime;
            if (DateTime.Compare(now, temp) < 0)
            {
                temp = now;
                now = datetime;
            }
            int year = now.Year - temp.Year;
            int month = now.Month - temp.Month;
            month = year * 12 + month;
            if (now.Day - temp.Day < -15)
            {
                month--;
            }
            else if (now.Day - temp.Day > 14)
            {
                month++;
            }
            return month;
        }
    }
}

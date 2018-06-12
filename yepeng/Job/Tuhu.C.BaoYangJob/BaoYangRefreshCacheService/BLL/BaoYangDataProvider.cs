using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaoYangRefreshCacheService.DAL;
using Common.Logging;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Enums;
using Tuhu.Service.BaoYang.Models;
using Tuhu.Service.BaoYang.Models.External;
using Tuhu.Service.BaoYang.Models.Request;

namespace BaoYangRefreshCacheService.BLL
{
    public class BaoYangDataProvider
    {
        private ILog logger;

        public BaoYangDataProvider(ILog logger)
        {
            this.logger = logger;
        }

        private List<List<T>> Split<T>(List<T> list, int size)
        {
            List<List<T>> result = new List<List<T>>() { new List<T>() };

            for (int index = 0; index < list.Count; index++)
            {
                var lastList = result.Last();
                if (lastList.Count == size)
                {
                    var newList = new List<T>(size) { list[index] };
                    result.Add(newList);
                }
                else
                {
                    lastList.Add(list[index]);
                }
            }

            return result;
        }

        public void GetAdaptationData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PKID");
            dt.Columns.Add("Tid");
            dt.Columns.Add("PackageName");
            dt.Columns.Add("PackageItemName");
            dt.Columns.Add("OeCode");
            dt.Columns.Add("PartCode");
            dt.Columns.Add("Pid");
            dt.Columns.Add("IsDefault");
            dt.Columns.Add("IsAll");
            dt.Columns.Add("CreateTime");
            dt.Columns.Add("Brand");
            dt.Columns.Add("PartName");
            var count = DalBaoYangPrice.GetBIAdaptationDataCount();

            logger.Info($"current count: {count}");
            if (count == 0)
            {
                logger.Info("begin get vehicles");
                var vehicles = VehicleDal.GetAllTidVehicles();
                vehicles = vehicles.Distinct(o => o.Tid).ToList();
                var splitedVehicles = Split(vehicles.ToList(), 500);
                logger.Info($"total vehicle count. vechile count:{vehicles.Count()}, splited vehicle count:{splitedVehicles.Sum(o => o.Count())}");
                var task = new TaskFactory().StartNew(async () =>
                {
                    foreach (var splitedItem in splitedVehicles)
                    {
                        var tasks = splitedItem.ParallelSelect(async (item) =>
                        {
                            using (var client = new BaoYangClient())
                            {
                                var serviceResult = await client.GetBIAdpationDataByTidAsync(item.Tid);
                                if (serviceResult.Success && serviceResult.Result != null && serviceResult.Result.Any())
                                {
                                    return serviceResult.Result.ToList();
                                }
                                else
                                {
                                    logger.Info($"invoke service failed, tid:{item.Tid}", serviceResult.Exception);
                                    return new List<BIAdaptationData>();
                                }
                            }
                        }, 200);

                        var result = await Task.WhenAll(tasks);

                        result.SelectMany(o => o).ForEach(item =>
                          {
                              var row = dt.NewRow();
                              row["PKID"] = 0;
                              row["Tid"] = item.Tid;
                              row["PackageName"] = item.PackageName;
                              row["PackageItemName"] = item.PackageItemName;
                              row["PartName"] = item.PartName;
                              row["OeCode"] = item.OeCode;
                              row["PartCode"] = item.PartCode;
                              row["Brand"] = item.Brand;
                              row["Pid"] = item.Pid;
                              row["IsDefault"] = item.IsDefault;
                              row["IsAll"] = item.IsAll;
                              row["CreateTime"] = DateTime.Now;
                              dt.Rows.Add(row);
                          });
                        logger.Info($"get data success! count: {result.Count()}, data count:{dt.Rows.Count}");
                        DalBaoYangPrice.Save("BaoYangAdaptationData", dt, "Tuhu_BI");
                        dt.Rows.Clear();
                    }

                });

                task.Wait();
            }
        }

        public void GetPriceData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PKID");
            dt.Columns.Add("Tid");
            dt.Columns.Add("PackageType");
            dt.Columns.Add("BaoYangType");
            dt.Columns.Add("PackageName");
            dt.Columns.Add("BaoYangTypeName");
            dt.Columns.Add("ProductIds");
            dt.Columns.Add("MinProudctPrice");
            dt.Columns.Add("MaxProudctPrice");
            dt.Columns.Add("DefaultProductId");
            dt.Columns.Add("DefaultProductCount");
            dt.Columns.Add("DefaultProductPrice");
            dt.Columns.Add("InAdaptationReason");

            DataTable serviceTable = new DataTable();
            serviceTable.Columns.Add("PKID");
            serviceTable.Columns.Add("Tid");
            serviceTable.Columns.Add("PackageType");
            serviceTable.Columns.Add("ServiceId");
            serviceTable.Columns.Add("ServicePrice");
            serviceTable.Columns.Add("ServiceCount");
            DalBaoYangPrice.CleanBaoYangPackageProductPriceTable();
            DalBaoYangPrice.CleanBaoYangPackageServicePriceTable();
            var vehicles = VehicleDal.GetAllTidVehicles();
            logger.Info($"BI数据Job，共{vehicles.Count()}个车型");
            int pkid1 = 1;
            int pkid2 = 1;
            int count = 0;
            foreach (var vehicle in vehicles)
            {
                try
                {
                    using (var client = new BaoYangClient())
                    {
                        var data = client.GetAllBaoYangTypePriceShortcut(vehicle);
                        if (data != null && data.Success && data.Result != null && data.Result.Any())
                        {
                            foreach (var item in data.Result)
                            {
                                var row = dt.NewRow();

                                row["PKID"] = pkid1++;
                                row["Tid"] = vehicle.Tid;
                                row["PackageType"] = item.PackageType;
                                row["BaoYangType"] = item.BaoYangType;
                                row["PackageName"] = item.PackageName;
                                row["BaoYangTypeName"] = item.BaoYangTypeName;
                                row["ProductIds"] = item.ProductIds;
                                row["MinProudctPrice"] = item.MinProudctPrice;
                                row["MaxProudctPrice"] = item.MaxProudctPrice;
                                row["DefaultProductId"] = item.DefaultProductId;
                                row["DefaultProductCount"] = item.DefaultProductCount;
                                row["DefaultProductPrice"] = item.DefaultProudctPrice;
                                row["InAdaptationReason"] = item.InAdaptationReason;

                                dt.Rows.Add(row);
                            }

                            var installRequest =
                                data.Result.GroupBy(o => o.PackageType).Select(o => new InstallServiceRequestModel2()
                                {
                                    PackageType = o.Key,
                                    Items =
                                        o.Where(
                                            t => string.Equals(t.InAdaptationReason, InAdaptableType.None.ToString()))
                                            .GroupBy(t => t.BaoYangType)
                                            .Select(t => new InstallServiceRequestModel()
                                            {
                                                BaoYangType = t.Key,
                                                Products = t.Select(p => new ItemCountModel<string>()
                                                {
                                                    Count = p.DefaultProductCount,
                                                    Item = p.DefaultProductId
                                                })
                                            })
                                });

                            var serviceResult = client.GetInstallServices(installRequest, vehicle, 0, Guid.Empty);
                            if (serviceResult != null && serviceResult.Success && serviceResult.Result != null &&
                                serviceResult.Result.Any())
                            {
                                foreach (var group in serviceResult.Result)
                                {
                                    if (group.Value != null && group.Value.Any())
                                    {
                                        foreach (var item in group.Value)
                                        {
                                            var row = serviceTable.NewRow();

                                            row["PKID"] = pkid2++;
                                            row["Tid"] = vehicle.Tid;
                                            row["PackageType"] = group.Key;
                                            row["ServiceId"] = item.Item.ServiceId;
                                            row["ServicePrice"] = item.Item.ShopServicePrice.TotalPice;
                                            row["ServiceCount"] = item.Count;

                                            serviceTable.Rows.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                        if (dt.Rows.Count > 5000)
                        {
                            DalBaoYangPrice.Save("BaoYangPackageProductPrice", dt);
                            DalBaoYangPrice.Save("BaoYangPackageServicePrice", serviceTable);
                            dt.Rows.Clear();
                            serviceTable.Rows.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }

                count++;
                if (count % 1000 == 0)
                {
                    logger.Info($"BI数据Job，已完成{count}个车型，共{vehicles.Count()}个车型");
                }
            }

            if (dt.Rows.Count > 0)
            {
                DalBaoYangPrice.Save("BaoYangPackageProductPrice", dt);
                DalBaoYangPrice.Save("BaoYangPackageServicePrice", serviceTable);
                dt.Rows.Clear();
            }
        } 
    }
}

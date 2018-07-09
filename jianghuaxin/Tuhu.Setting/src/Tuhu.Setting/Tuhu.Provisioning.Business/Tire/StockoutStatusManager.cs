using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.DAO.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Business.Tire
{
    public sealed class StockoutStatusManager
    {
        public static IEnumerable<StockoutStatusModel> SelectList(StockoutStatusRequest request, PagerModel pager)
       => DalStockoutStatus.SelectList(request, pager);

        public static IEnumerable<StockoutStatusWhiteModel>  SelectWhiteList(WhiteRequest model, PagerModel pager)
       => DalStockoutStatus.SelectWhiteList(model, pager);

        public static IEnumerable<ShowStatusModel> GetShowStatusByPids(List<string> pids)
        {
            var data = DalStockoutStatus.GetStockoutStatusByPids(pids);
            var result = new List<ShowStatusModel>();
            foreach(var item in data)
            {
                var Status = 0;
                var val = new ShowStatusModel()
                {
                    PID = item.PID
                };
                if (!item.OnSale)
                {
                    Status = 3;
                }
                else if (!item.IsShow)
                {
                    Status = 3;
                }
                else if (item.Status)
                {
                    Status = 1;
                }
                else if (item.Stuckout)
                {
                    Status = 2;
                }
                else if (item.SystemStuckout!="有货")
                {
                    Status = 2;
                }
                else
                {
                    Status = 1;
                }
                val.ShowStatus = Status;
                result.Add(val);
            }
            return result;
        }

        public static int SaveTireStockWhite(string pids)
        {
            var result = -99;
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                foreach (var pid in pids.Split(';'))
                {
                   var isUpdate= DalStockoutStatus.FetchPidStatus(dbHelper,pid);
                    if (isUpdate)
                       result= DalStockoutStatus.UpdateWhite(dbHelper, pid);
                    else
                       result= DalStockoutStatus.InsertWhite(dbHelper, pid);
                    if (result <= 0)
                    {
                        dbHelper.Rollback();
                        return result;
                    }
                }
                dbHelper.Commit();
                return 1;
            }
        }

        public static int RemoveWhite(string pid)
        {
           return  DalStockoutStatus.RemoveWhite(pid);
        }

        public static List<RegionStockModel> SelectRegionStockList(RegionStockRequest model, PagerModel pager)
            => DalStockoutStatus.SelectRegionStockList(model, pager);

        public static List<MiniRegion> GetAllRegion()
        {
            using(var client=new RegionClient())
            {
                var vals = client.GetAllMiniRegion();
                var data = new List<MiniRegion>();
                if (vals.Success)
                {
                    foreach (var item in vals.Result.ToList())
                    {
                        var val = new MiniRegion()
                        {
                            RegionId = item.RegionId,
                            RegionName = item.RegionName,
                            PinYin = item.PinYin
                        };
                        if (item.RegionId == 1 || item.RegionId == 2 || item.RegionId == 19 || item.RegionId == 20)
                        {
                            val.ChildRegions = new List<MiniRegion>()
                            {
                                new MiniRegion()
                                {
                                    RegionId = item.RegionId,
                                    RegionName = item.RegionName,
                                    PinYin = item.PinYin
                                }
                            };
                        }
                        else
                        {
                            var child = new List<MiniRegion>();
                            foreach(var it in item.ChildRegions)
                            {
                                child.Add(new MiniRegion()
                                {
                                    RegionId = it.RegionId,
                                    RegionName = it.RegionName,
                                    PinYin = it.PinYin
                                });
                            }
                            val.ChildRegions = child;

                        }
                        data.Add(val);
                    }
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

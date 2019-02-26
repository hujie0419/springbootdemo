using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Shop.Models;

namespace Tuhu.Service.Activity.Server
{
    ///<summary>六周年投票</summary>///
    public class SexAnnualVoteService:ISexAnnualVoteService
    {
        ///<summary>门店报名</summary>///
        public async Task<OperationResult<bool>> AddShopSignUpAsync(ShopVoteModel model)
        {
            try
            {
                //验证是否可以反序列化，保证数据有效
                JsonConvert.DeserializeObject<JArray>(model.ImageUrls);
                var check = await CheckShopSignUpAsync(model.ShopId);
                if (check.Result)
                {
                    return OperationResult.FromResult(true);
                }
                else
                {
                    var result = await SexAnnualVoteManager.InsertShopVoteAsync(model);
                    return OperationResult.FromResult(result.Result);
                }
                
            }
            catch (Exception e)
            {
                return OperationResult.FromError<bool>("0", e.Message);
            }
        }
        ///<summary>技师报名</summary>///
        public async Task<OperationResult<bool>> AddEmployeeSignUpAsync(ShopEmployeeVoteModel model)
        {
            try
            {
                //验证是否可以反序列化，保证数据有效
                JsonConvert.DeserializeObject<JArray>(model.ImageUrls);
                var check = await CheckEmployeeSignUpAsync(model.ShopId, model.EmployeeId);
                if (check.Result)
                {
                    return OperationResult.FromResult(true);
                }
                else
                {
                    var result = await SexAnnualVoteManager.InsertShopEmployeeVoteAsync(model);
                    return OperationResult.FromResult(result.Result);
                }
                
            }
            catch (Exception e)
            {
                return OperationResult.FromError<bool>("0", e.Message);
            }
        }
        ///<summary>验证门店是否已经报过名了</summary>///
        public async Task<OperationResult<bool>> CheckShopSignUpAsync(long shopId)
        {
            var result = await SexAnnualVoteManager.GetShopVoteAsync(shopId);
            return OperationResult.FromResult(result.Result != null);
        }
        ///<summary>验证技师是否已经报过名了</summary>///
        public async Task<OperationResult<bool>> CheckEmployeeSignUpAsync(long shopId, long employeeId)
        {
            var result = await SexAnnualVoteManager.GetShopEmployeeVoteAsync(shopId, employeeId);
            return OperationResult.FromResult(result.Result != null);
        }

        public async Task<OperationResult<PagedModel<ShopVoteBaseModel>>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query)
        {
            return await SexAnnualVoteManager.SelectShopRankingAsync(query);
        }

        public async Task<OperationResult<PagedModel<ShopEmployeeVoteBaseModel>>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query)
        {
            return await SexAnnualVoteManager.SelectShopEmployeeRankingAsync(query);
        }

        public Task<OperationResult<ShopVoteModel>> FetchShopBaseInfoAsync(long pkid)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<ShopVoteModel>> FetchShopDetailAsync(long shopId)
        {
            return await SexAnnualVoteManager.FetchShopDetailAsync(shopId);
        }

        public Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeBaseInfoAsync(long pkid)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeDetailAsync(long shopId, long employeeId)
        {
            return await SexAnnualVoteManager.FetchShopEmployeeDetailAsync(shopId, employeeId);
        }

        public async Task<OperationResult<bool>> AddShopVoteAsync(Guid userId, long shopId)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<bool>("userId is is empty", "userId 不能为空");
            return await SexAnnualVoteManager.AddShopVoteAsync(userId, shopId);
        }

        public async Task<OperationResult<IEnumerable<ShopVoteRecordModel>>> SelectShopVoteRecordAsync(Guid userId,
            DateTime startDate, DateTime endDate)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<IEnumerable<ShopVoteRecordModel>>("userId is is empty", "userId 不能为空");
            if (startDate == DateTime.MinValue || startDate == DateTime.MaxValue)
                return OperationResult.FromError<IEnumerable<ShopVoteRecordModel>>("startDate is is empty",
                    "startDate 必须在 MinValue和MaxValue之间");
            if (endDate == DateTime.MinValue || endDate == DateTime.MaxValue)
                return OperationResult.FromError<IEnumerable<ShopVoteRecordModel>>("endDate is is empty",
                    "endDate 必须在 MinValue和MaxValue之间");
            return await SexAnnualVoteManager.SelectShopVoteRecordAsync(userId, startDate, endDate);
        }

        public async Task<OperationResult<bool>> AddShopEmployeeVoteAsync(Guid userId, long shopId, long employeeId)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<bool>("userId is is empty", "userId 不能为空");
            return await SexAnnualVoteManager.AddShopEmployeeVoteAsync(userId, shopId, employeeId);
        }

        public async Task<OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>>> SelectShopEmployeeVoteRecordAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<IEnumerable<ShopEmployeeVoteRecordModel>>("userId is is empty", "userId 不能为空");
            if (startDate == DateTime.MinValue || startDate == DateTime.MaxValue)
                return OperationResult.FromError<IEnumerable<ShopEmployeeVoteRecordModel>>("startDate is is empty",
                    "startDate 必须在 MinValue和MaxValue之间");
            if (endDate == DateTime.MinValue || endDate == DateTime.MaxValue)
                return OperationResult.FromError<IEnumerable<ShopEmployeeVoteRecordModel>>("endDate is is empty",
                    "endDate 必须在 MinValue和MaxValue之间");
            return await SexAnnualVoteManager.SelectShopEmployeeVoteRecordAsync(userId, startDate, endDate);
        }

        public async Task<OperationResult<bool>> AddShareShopVoteAsync(Guid userId, long shopId)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<bool>("userId is is empty", "userId 不能为空");
            return await SexAnnualVoteManager.AddShareShopVoteAsync(userId, shopId);
        }

        public async Task<OperationResult<bool>> AddShareShopEmployeeVoteAsync(Guid userId, long shopId, long employeeId)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<bool>("userId is is empty", "userId 不能为空");
            return await SexAnnualVoteManager.AddShareShopEmployeeVoteAsync(userId, shopId, employeeId);
        }

        public async Task<OperationResult<IDictionary<int,Shop.Models.RegionModel>>> GetShopRegionAsync()
        {
            var filters = await SexAnnualVoteManager.SelectShopRegionAsync();
            var region = await FilterRegion(filters);
            return region;
        }

        private static async Task<OperationResult<IDictionary<int, RegionModel>>> FilterRegion(OperationResult<IDictionary<int, List<int>>> filters)
        {
            OperationResult<IDictionary<int, Shop.Models.RegionModel>> region;
            using (var client = new Shop.RegionClient())
            {
                region = await client.SelectAllRegionAsync();
            }
            IDictionary<int, Shop.Models.RegionModel> result = new Dictionary<int, Shop.Models.RegionModel>();
            List<int> zhixiashi = new List<int>() {1, 2, 19, 20};
            foreach (var filter in filters.Result.Keys)
            {
                if (region.Result.ContainsKey(filter))
                {
                    var model = region.Result[filter];
                    var item = new Shop.Models.RegionModel()
                    {
                        ParentId = model.ParentId,
                        RegionName = model.RegionName,
                        Pkid = model.Pkid,
                        Children = new Dictionary<int, Shop.Models.RegionModel>()
                    };

                    //四个直辖市除外

                    if (zhixiashi.Contains(filter))
                    {
                        item.Children.Add(filter, new Shop.Models.RegionModel()
                        {
                            ParentId = model.ParentId,
                            RegionName = model.RegionName,
                            Pkid = model.Pkid,
                            Children = new Dictionary<int, Shop.Models.RegionModel>()
                        });
                    }
                    else
                    {
                        foreach (var c in filters.Result[filter])
                        {
                            if (model.Children.ContainsKey(c))
                            {
                                item.Children.Add(c, model.Children[c]);
                                //区县 第三级 不再返回
                                item.Children[c].Children = new Dictionary<int, Shop.Models.RegionModel>();
                            }
                        }
                    }


                    result.Add(filter, item);
                }
            }
            return OperationResult.FromResult(result);
        }

        public async Task<OperationResult<IDictionary<int, Shop.Models.RegionModel>>> GetShopEmployeeRegionAsync()
        {
            var filters = await SexAnnualVoteManager.SelectShopEmployeeRegionAsync();
            var region = await FilterRegion(filters);
            return region;
        }
    }
}

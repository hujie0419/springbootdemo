using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class SexAnnualVoteManager
    {
        public static async Task<OperationResult<bool>> InsertShopVoteAsync(ShopVoteModel model)
        {
            return OperationResult.FromResult(await DalSexAnnualVote.InsertShopVoteAsync(model) > 0);
        }


        public static async Task<OperationResult<bool>> InsertShopEmployeeVoteAsync(ShopEmployeeVoteModel model)
        {
            return OperationResult.FromResult(await DalSexAnnualVote.InsertShopEmployeeVoteAsync(model) > 0);
        }


        public static async Task<OperationResult<ShopVoteModel>> GetShopVoteAsync(long shopId)
        {
            return OperationResult.FromResult(await DalSexAnnualVote.GetShopVoteByShopIdAsync(shopId));
        }

        public static async Task<OperationResult<ShopEmployeeVoteModel>> GetShopEmployeeVoteAsync(long shopId,
            long employeeId)
        {
            return OperationResult.FromResult(
                await DalSexAnnualVote.GetShopEmployeeIdVoteByEmployeeIdAsync(shopId, employeeId));
        }

        public static async Task<OperationResult<PagedModel<ShopVoteBaseModel>>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query)
        {
            return await OperationResult.FromResultAsync(DalSexAnnualVote.SelectShopRankingAsync(query));
        }

        public static async Task<OperationResult<PagedModel<ShopEmployeeVoteBaseModel>>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query)
        {
            return await OperationResult.FromResultAsync(DalSexAnnualVote.SelectShopEmployeeRankingAsync(query));
        }

        public static async Task<OperationResult<IDictionary<int, List<int>>>> SelectShopRegionAsync()
        {
            var region = await DalSexAnnualVote.SelectShopRegionAsync();
            return OperationResult.FromResult<IDictionary<int, List<int>>>(region.GroupBy(_ => _.ProvinceId)
                .ToDictionary(k => k.Key, v => v.Select(_ => _.CityId).ToList()));
        }
        public static async Task<OperationResult<IDictionary<int, List<int>>>> SelectShopEmployeeRegionAsync()
        {
            var region = await DalSexAnnualVote.SelectShopEmployeeRegionAsync();
            return OperationResult.FromResult<IDictionary<int, List<int>>>(region.GroupBy(_ => _.ProvinceId)
                .ToDictionary(k => k.Key, v => v.Select(_ => _.CityId).ToList()));
        }

        public static async Task<OperationResult<ShopVoteModel>> FetchShopDetailAsync(long shopId)
        {
            var detail = await DalSexAnnualVote.FetchShopDetailAsync(shopId);
            if (!string.IsNullOrEmpty(detail.ImageUrls))
            {
                try
                {
                    detail.ImageList = JsonConvert.DeserializeObject<List<string>>(detail.ImageUrls);
                }
                catch (Exception e)
                {
                }
            }
            return  OperationResult.FromResult(detail);
        }

        public static async Task<OperationResult<ShopEmployeeVoteModel>> FetchShopEmployeeDetailAsync(long shopId, long employeeId)
        {
            var detail = await DalSexAnnualVote.FetchShopEmployeeDetailAsync(shopId, employeeId);
            if (detail != null)
            {
                if (!string.IsNullOrEmpty(detail.ImageUrls))
                {
                    try
                    {
                        detail.ImageList = JsonConvert.DeserializeObject<List<string>>(detail.ImageUrls);

                    }
                    catch (Exception e)
                    {
                    }
                }
                detail.CommentArray =
                    (await DalSexAnnualVote.SelectShopEmployeeCommentAsync(shopId, employeeId)).ToList();
                detail.Comments = detail.Comments ?? "";
                return OperationResult.FromResult(detail);
            }
            else
            {
                return OperationResult.FromResult(new ShopEmployeeVoteModel());
            }
            
        }

        /// <summary>
        /// 给门店投票
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> AddShopVoteAsync(Guid userId, long shopId)
        {
            var record = await DalSexAnnualVote.SelectShopVoteRecordAsync(userId, DateTime.Now, DateTime.Now);
            //判断当前登录用户今天是否有投票记录
            var shopVoteRecordModels = record as IList<ShopVoteRecordModel> ?? record.ToList();
            if (shopVoteRecordModels.Count >= 2) //如果投票记录已经等于2 说明今天投了两次，不能再投票了
                return OperationResult.FromError<bool>("2", "今天的投票次数已经用完，请明天再来投票");
            if (shopVoteRecordModels.Any()&&!shopVoteRecordModels.Any(_ => _.Share))
            {
                return OperationResult.FromError<bool>("1", "分享投票可以再获得一次投票机会");
            }
            return OperationResult.FromResult((await DalSexAnnualVote.AddShopVoteAsync(userId, shopId)) > 0);
        }

        public static async Task<OperationResult<IEnumerable<ShopVoteRecordModel>>> SelectShopVoteRecordAsync(
            Guid userId, DateTime startDate, DateTime endDate)
        {
            return await OperationResult.FromResultAsync(DalSexAnnualVote.SelectShopVoteRecordAsync(userId, startDate, endDate));
        }

        public static async Task<OperationResult<bool>> AddShopEmployeeVoteAsync(Guid userId, long shopId,
            long employeeId)
        {
            var record = await DalSexAnnualVote.SelectShopEmployeeVoteRecordAsync(userId, DateTime.Now, DateTime.Now);
            //判断当前登录用户今天是否有投票记录
            var shopEmployeeVoteRecordModels = record as IList<ShopEmployeeVoteRecordModel> ?? record.ToList();
            if (shopEmployeeVoteRecordModels.Count() >= 2) //如果投票记录已经等于2 说明今天投了两次，不能再投票了
                return OperationResult.FromError<bool>("2", "今天的投票次数已经用完，请明天再来投票");
            if (shopEmployeeVoteRecordModels.Any() && !shopEmployeeVoteRecordModels.Any(_ => _.Share))
            {
                return OperationResult.FromError<bool>("1", "分享投票可以再获得一次投票机会");
            }
            return OperationResult.FromResult(
                (await DalSexAnnualVote.AddShopEmployeeVoteAsync(userId, shopId, employeeId)) > 0);
        }

        public static async Task<OperationResult<IEnumerable<ShopEmployeeVoteRecordModel>>>
            SelectShopEmployeeVoteRecordAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await OperationResult.FromResultAsync(
                DalSexAnnualVote.SelectShopEmployeeVoteRecordAsync(userId, startDate, endDate));
        }

        public static async Task<OperationResult<bool>> AddShareShopVoteAsync(Guid userId, long shopId)
        {
            return OperationResult.FromResult(await DalSexAnnualVote.AddShareShopVoteAsync(userId, shopId) > 0);
        }

        public static async Task<OperationResult<bool>> AddShareShopEmployeeVoteAsync(Guid userId, long shopId, long employeeId)
        {
            return OperationResult.FromResult(await DalSexAnnualVote.AddShareShopEmployeeVoteAsync(userId, shopId,employeeId) > 0);
        }
    }
}

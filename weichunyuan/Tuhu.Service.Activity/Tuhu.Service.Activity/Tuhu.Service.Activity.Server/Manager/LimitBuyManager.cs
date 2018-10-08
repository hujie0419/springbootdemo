using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class LimitBuyManager
    {
        // <summary>
        /// 添加限购数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public static async Task<BaseResponse> AddBuyLimitInfo(List<BuyLimitDetailModel> models)
        {
            var result = new BaseResponse { Code = 0 };
            models.ForEach(g => g.LimitObjectId = g.LimitObjectId.ToLower());
            foreach (var model in models)
            {
                var itemResult = await DalBuyLimit.AddBuyLimitInfo(model);
                if (itemResult)
                {
                    result.Code = 1;
                    result.Msg += $"{model.ModuleName}/{model.ModuleProductId}/{model.Reference};";
                }
            }
            var keys = models.Select(g => $"limit/{g.ModuleName}/{g.ModuleProductId}/{g.LimitObjectId}/{g.ObjectType}").ToList();
            await SetLimitCache(keys);
            return result;
        }
        /// <summary>
        /// 更新（移除）限购信息
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static async Task<BaseResponse> RemoveBuyLimitInfo(string moduleName, string reference)
        {
            var data = await DalBuyLimit.RemoveBuyLimitInfo(moduleName, reference);
            var result = new BaseResponse { Code = 0 };
            if (data.Any())
            {
                var keys = data.Select(g => $"limit/{g.ModuleName}/{g.ModuleProductId}/{g.LimitObjectId}/{g.ObjectType}").ToList();
                result.Code = 1;
                result.Msg = string.Join(";", keys);
                await SetLimitCache(keys);
            }
            return result;
        }
        /// <summary>
        /// 查询限购数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public static async Task<List<BuyLimitInfoModel>> SelectBuyLimitInfo(List<BuyLimitModel> models)
        {

            var result = new List<BuyLimitInfoModel>();
            models.ForEach(g => g.LimitObjectId = g.LimitObjectId.ToLower());
            foreach (var model in models)
            {
                var key = $"limit/{model.ModuleName}/{model.ModuleProductId}/{model.LimitObjectId}/{model.ObjectType}";
                var readOnly = !(await GetLimitCache(key));
                var itemResult = await DalBuyLimit.SelectBuyLimitInfo(model, readOnly);
                result.Add(itemResult);
            }
            return result;
        }

        private static async Task SetLimitCache(List<string> keys)
        {
            using(var client = CacheHelper.CreateCacheClient("BuyLimitCache"))
            {
                foreach(var key in keys)
                {
                    await client.SetAsync(key, true, TimeSpan.FromSeconds(10));
                }
                
            }
        }
        private static async Task<bool> GetLimitCache(string key)
        {
            using (var client = CacheHelper.CreateCacheClient("BuyLimitCache"))
            {
                var result = await client.GetAsync<bool>(key);
                return result.Success && result.Value;
            }
        }
    }
}

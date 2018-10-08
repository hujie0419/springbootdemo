using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using Tuhu.C.Job.CompetingProducts.Models;


namespace Tuhu.C.Job.CompetingProducts.ShopManages
{
    public class JingDongShopManage 
    {
        private static readonly ILog Logger;

        static JingDongShopManage()
        {
            Logger = LogManager.GetLogger<JingDongShopManage>();
        }

        /// <summary>获得京东的价格</summary>
        /// <param name="itemId"></param>
        /// <returns>Item1：价格；Item2：是否促销；Item3：商品名称</returns>
        public static async Task<ItemPriceModel> GetJingDongPrice(string itemId)
        {
            IHttpProxyClient client = null;
            try
            {
                client = new HttpProxyClient();

                Logger.Info("开始获取京东商品价格: " + itemId);

                var result = await client.GetJingdongPriceAsync(itemId);

                result.ThrowIfException(true);

                if (result.Result != null)
                {
                    Logger.Info($"获取到的京东商品{itemId}的价格为{result.Result.Price}，名称为：{result.Result.Title}");

                    return result.Result;
                }

                Logger.Warn($"获取京东商品{itemId}价格失败");

                return result.Result;
            }
            catch (Exception ex)
            {
                //Logger.Error(new LogModel { Message = "下载商品信息失败", RefNo = itemId }, ex);
                Logger.Info(new LogModel { Message = ex.Message, RefNo = itemId });
                return null;
            }
            finally
            {
                client?.Dispose();
            }
        }
    }

}

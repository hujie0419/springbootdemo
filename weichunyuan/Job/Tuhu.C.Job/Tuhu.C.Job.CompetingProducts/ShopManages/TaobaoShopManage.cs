using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using Tuhu.C.Job.CompetingProducts.Models;

namespace Tuhu.C.Job.CompetingProducts.ShopManages
{
    public class TaobaoShopManage
    {
        private static readonly ILog Logger;

        static TaobaoShopManage()
        {
            Logger = LogManager.GetLogger<TaobaoShopManage>();
        }

        /// <summary>获得淘宝商品的价格</summary>
        /// <param name="itemId">商品编号</param>
        /// <returns>价格和名称</returns>
        public static async Task<ItemPriceModel> GetTabaoPrice(string itemId)
        {
            IHttpProxyClient client = null;
            try
            {
                client = new HttpProxyClient();

                var result = await client.GetTaobaoPriceAsync(itemId);

                result.ThrowIfException(true);

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

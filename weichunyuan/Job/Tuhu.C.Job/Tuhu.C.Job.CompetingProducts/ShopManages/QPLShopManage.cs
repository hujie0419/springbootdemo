using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using Tuhu.C.Job.CompetingProducts.Models;
using QPL.WebService.TuHu.Core;
using static Tuhu.C.Job.CompetingProducts.ShopManages.WCFClients;
using QPL.WebService.TuHu.Core.Model;

namespace Tuhu.C.Job.CompetingProducts.ShopManages
{
    public class QPLShopManage
    {
        private static readonly ILog Logger;

        static QPLShopManage()
        {
            Logger = LogManager.GetLogger<QPLShopManage>();
        }
        /// <summary>
        /// 汽配龙获取商品价格
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<ResultMsg<TrProductInfoModel>> GetQPLPrice(string pid)
        {
            ProductInfoServiceClient client = null;
            try
            {
                client = new ProductInfoServiceClient();
                var result = await client.Instance.GetUserProductInfoByPID(pid);
                return result;
            }
            catch (Exception ex)
            {
                //Logger.Error(new LogModel { Message = "下载商品信息失败", RefNo = pid }, ex);
                Logger.Info(new LogModel { Message = ex.Message, RefNo = pid });
                return null;
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}

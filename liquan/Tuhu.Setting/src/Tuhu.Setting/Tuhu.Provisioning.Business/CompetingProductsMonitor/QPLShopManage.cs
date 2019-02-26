using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using QPL.WebService.TuHu.Core.Model;
using static Tuhu.Provisioning.Business.CompetingProductsMonitorManager.WCFClients;
using QPL.WebService.TuHu.Core;

namespace Tuhu.Provisioning.Business.CompetingProductsMonitorManager
{
    public class QPLShopManage
    {
        private static readonly ILog Logger;

        static QPLShopManage()
        {
            //Logger = LogManager.GetLogger<QPLShopManage>();
        }

        /// <summary>
        /// 汽配龙获取商品价格
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static Tuple<decimal, string> GetQPLPrice(string pid)
        {
            decimal price = 0;
            string title = string.Empty;
            ProductInfoServiceClient client = null;
            try
            {
                client = new ProductInfoServiceClient();
                var result = client.Instance.GetUserProductInfoByPID(pid);
                if (result.Result.Data != null)
                {
                    price = result.Result.Data.ListPrice;
                    title = result.Result.Data.ProductName;
                }
                return Tuple.Create<decimal, string>(price, title);
            }
            catch (Exception ex)
            {
                //Logger.Error(new LogModel { Message = "下载商品信息失败", RefNo = pid }, ex);
                return null;
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}

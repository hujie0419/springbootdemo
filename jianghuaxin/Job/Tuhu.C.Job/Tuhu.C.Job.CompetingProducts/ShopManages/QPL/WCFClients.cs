using QPL.WebService.TuHu.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CompetingProducts.ShopManages
{
    public class WCFClients
    {
        /// <summary>
        /// 汽配龙商品信息服务
        /// </summary>
        public class ProductInfoServiceClient : ServiceClient<IProductInfoService>
        {
            public ProductInfoServiceClient() : base(
                ConfigurationManager.AppSettings["QPLProductInfoServiceWcfUrl"] ??
                throw new Exception("请在appsetting中配置QPL商品服务地址"))
            { }
        }
    }
}

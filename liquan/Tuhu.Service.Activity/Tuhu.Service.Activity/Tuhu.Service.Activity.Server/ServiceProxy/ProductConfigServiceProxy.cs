using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.ProductConfig;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    /// <summary>
    /// 产品配置服务代理
    /// </summary>
    public class ProductConfigServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductConfigServiceProxy));

        /// <summary>
        /// 根据标签获取产品属性信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, List<ProductCommonTagDetail>>>
            SelectProductInfoByTagRequestAsync(ProductInfoByTagRequest request)
        {
            try
            {
                using (var client = new ProductConfigClient())
                {
                    var selectResult = await client.SelectProductInfoByTagRequestAsync(request);
                    if (!selectResult.Success)
                    {
                        Logger.Error($"SelectProductInfoByTagRequestAsync fail => " +
                            $"ErrorCode ={selectResult.ErrorCode} & ErrorMessage ={selectResult.ErrorMessage} ");
                    }
                    return selectResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectProductInfoByTagRequestAsync 接口异常", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取商品的每单限购信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public async static Task<OperationResult<Dictionary<string, int>>> GetProductLimitCountWithPidListAsync(List<string> pids)
        {
            using (var client = new ProductConfigClient())
            {
                var result = await client.GetProductLimitCountWithPidListAsync(pids);
                if (!result.Success)
                {
                    Logger.Warn($"GetProductLimitCountWithPidListAsync失败,ErrorMessage:{result.ErrorMessage},pids:{string.Join(",", pids)} ");
                }
                return result;
            }
        }

    }
}

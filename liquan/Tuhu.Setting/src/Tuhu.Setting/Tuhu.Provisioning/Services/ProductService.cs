using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Services
{
    public class ProductService
    {
        private static readonly ILog logger = LoggerFactory.GetLogger("FeedBackController");
        public static List<string> GetBrandsByCategoryName(string categoryName)
        {
            try
            {
                using (var productClient = new ProductClient())
                {
                    var activityInfoResult = productClient.GetBrandsByCategoryName(categoryName);

                    if (activityInfoResult.Success)
                    {
                        return activityInfoResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetBrandsByCategoryName");
            }
            return new List<string>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.ShopDistribution;
using Tuhu.Service.Product;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.DataAccess.Entity.ShopDistribution;

namespace Tuhu.Provisioning.Business.ShopDistribution
{
    public class ShopDistributionManager
    {
        private ShopDistributionDAL dal = null;
        public ShopDistributionManager()
        {
            dal = new ShopDistributionDAL();
        }
        /// <summary>
        /// 将商品标题组装到门店铺货配置中
        /// </summary>
        /// <param name="shopDistributionlist"></param>
        /// <returns></returns>
        public List<ShopDistributionModel> AddProductName(List<ShopDistributionModel> shopDistributionlist)
        {
            //组装PID
            var pidList = new List<string>();
            foreach (var entity in shopDistributionlist)
            {
                pidList.Add(entity.FKPID);
            }
            using (var client = new ProductClient())
            {
                //批量查询商品信息
                var productList = client.SelectProduct(pidList).Result.ToList();
                foreach (var product in productList)
                {
                    foreach (var item in shopDistributionlist)
                    {
                        if (item.FKPID == product.Pid)
                            item.DisplayName = product.DisplayName;
                    }
                }
            }
            return shopDistributionlist;
        }
        /// <summary>
        /// 获取商品的列表数据[车品类目下的商品]
        /// </summary>
        /// <param name="keyword">商品PID/商品标题</param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<DataAccess.Entity.ShopDistribution.SDProductModel> GetProductList(string keyword, Pagination pagination)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetProductList(conn, keyword, pagination);
            }
        }

    }
}

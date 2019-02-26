using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.YLHProduct
{
    public class YLHProductManager
    {
        public static int AddProducts(YLHProductModel product)
        {
            DalYLHProduct dal = new DalYLHProduct();
            return dal.AddProducts(product);
        }
        public static int GetOid(string productId, string variantId)
        {
            DalYLHProduct dal = new DalYLHProduct();
            return dal.GetOid(productId, variantId);
        }
        public static int InsertLog(string user, string operateMethod, string operateDetail, string pid)
        {
            DalYLHProduct dal = new DalYLHProduct();
            return dal.InsertLog(user, operateMethod, operateDetail, pid);
            ;
        }
        public static int CheckoutProduct(string productNunber, int counterId)
        {
            DalYLHProduct dal = new DalYLHProduct();
            return dal.CheckoutProduct(productNunber, counterId);
        }


    }
}

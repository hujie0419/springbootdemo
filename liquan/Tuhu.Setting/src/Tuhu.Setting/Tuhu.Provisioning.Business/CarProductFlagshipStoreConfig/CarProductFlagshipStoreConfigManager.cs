using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Models.ProductConfig;
using System.Data;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.CarProductFlagshipStoreConfig
{
    public class CarProductFlagshipStoreConfigManager
    {
        private static DALCarProductFlagshipStoreConfig dal = null;

        public CarProductFlagshipStoreConfigManager()
        {
            dal = new DALCarProductFlagshipStoreConfig();
        }

        public List<Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig> GetList()
        {
            var dt = dal.GetDataTable();
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.ConvertTo<Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig>().ToList();
        }

        public DataTable GetBrand()
        {
            return dal.GetBrand();
        }

        public Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig GetConfigByBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand)) return null;

            List<string> list = new List<string>();
            list.Add(brand);
            using (var client = new ProductConfigClient())
            {
                var result = client.GetCarProductFlagshipStoreConfigByBrand(list);
                if (result.Success
                    && result.Result != null
                    && result.Result.Any())
                    return result.Result.FirstOrDefault();
                else return null;
            }
        }

        public bool InsertConfig(Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig config)
        {
            if (config == null) return false;

            List<Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig> list = new List<Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig>();
            list.Add(config);

            using (var client = new ProductConfigClient())
            {
                var result = client.CreateCarProductFlagshipStoreConfig(list);
                if (result.Success && result.Result > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateConfig(Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig config)
        {
            if (config == null) return false;

            using (var client = new ProductConfigClient())
            {
                var result = client.UpdateCarProductFlagshipStoreConfig(config);
                if (result.Success && result.Result)
                    return true;
                else return false;
            }
        }

        public bool DeleteConfig(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand)) return false;

            using (var client = new ProductConfigClient())
            {
                var result = client.DeleteCarProductFlagshipStoreConfigByBrand(brand);
                if (result.Success && result.Result)
                    return true;
                else return false;
            }
        }
    }
}

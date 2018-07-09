using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class AdvertisingConfigManager
    {
        private DALAdvertisingConfig dal = null;
        public AdvertisingConfigManager()
        {
            dal = new DALAdvertisingConfig();
        }

        /// <summary>
        /// 获取下单完成页广告配置列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<AdvertisingConfigModel> GetAdvertisingConfigList(Pagination pagination)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetAdvertisingConfigList(conn, pagination);
            }
        }
        /// <summary>
        /// 获取下单完成页广告配置详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AdvertisingConfigModel GetAdvertisingConfigInfo(long id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetAdvertisingConfigInfo(conn, id);
            }
        }
        /// <summary>
        /// 获取下单完成页广告位配置详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="productLine"></param>
        /// <param name="adType"></param>
        /// <returns></returns>
        public AdvertisingConfigModel GetAdvertisingConfigInfo(int provinceId, int cityId, string productLine,int adType)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetAdvertisingConfigInfo(conn, provinceId, cityId, productLine,adType);
            }
        }
        /// <summary>
        /// 新增下单完成页广告配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddAdvertisingConfig(AdvertisingConfigModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddAdvertisingConfig(conn, model);
            }
        }
        /// <summary>
        /// 删除下单完成页广告配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteAdvertisingConfig(long id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteAdvertisingConfig(conn, id);
            }
        }
        /// <summary>
        /// 修改下单完成页广告配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateAdvertisingConfig(AdvertisingConfigModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateAdvertisingConfig(conn, model);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class PaymentPageConfigManager
    {
        private DALPaymentPageConfig dal = null;
        public PaymentPageConfigManager()
        {
            dal = new DALPaymentPageConfig();
        }

        /// <summary>
        /// 获取下单完成页配置列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<PaymentPageConfigModel> GetPaymentPageConfigList(Pagination pagination)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetPaymentPageConfigList(conn, pagination);
            }
        }
        /// <summary>
        /// 获取下单完成页配置详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PaymentPageConfigModel GetPaymentPageConfigInfo(long id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetPaymentPageConfigInfo(conn, id);
            }
        }
        /// <summary>
        /// 获取下单完成页配置详细信息
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="productLine"></param>
        /// <returns></returns>
        public PaymentPageConfigModel GetPaymentPageConfigInfo(int provinceId, int cityId, string productLine)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetPaymentPageConfigInfo(conn,provinceId, cityId, productLine); 
            }
        }
        /// <summary>
        /// 新增下单完成页配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddPaymentPageConfig(PaymentPageConfigModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddPaymentPageConfig(conn, model);
            }
        }
        /// <summary>
        /// 删除下单完成页配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePaymentPageConfig(long id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeletePaymentPageConfig(conn, id);
            }
        }
        /// <summary>
        /// 修改下单完成页广告配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdatePaymentPageConfig(PaymentPageConfigModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdatePaymentPageConfig(conn, model);
            }
        }
    }
}

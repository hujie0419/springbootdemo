using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.PaymentWay
{
    public class PaymentWayManager : IPaymentWayManager
    {
        #region Private Fields

        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        private static readonly ILog logger = LoggerFactory.GetLogger("PaymentWayModel");

        private PaymentWayHandler handler = null;
        #endregion 

        public PaymentWayManager()
        {
            handler = new PaymentWayHandler(DbScopeManager);
        }

        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <returns></returns>
        public List<PaymentWayModel> GetAllPaymentWay()
        {
            return handler.GetAllPaymentWay();
        }

        /// <summary>
        /// 修改/添加
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public bool UpdatePaymentWay(List<PaymentWayModel> pwModelList)
        {
            return handler.UpdatePaymentWay(pwModelList);
        }
    }
}

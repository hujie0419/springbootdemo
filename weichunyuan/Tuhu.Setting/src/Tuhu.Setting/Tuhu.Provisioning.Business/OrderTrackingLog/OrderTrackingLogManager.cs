using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.ThreadPools;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.OrderTrackingLog
{
    public class OrderTrackingLogManager : IOrderTrackingLogManager
    {
        private static readonly IConnectionManager SystemlogconnectionManager =
          new ConnectionManager(ConfigurationManager.ConnectionStrings["SystemLogConnectionString"].ConnectionString);

        private static readonly ILog Logger = LoggerFactory.GetLogger("SystemLog.OrderTrackingLog");
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(SystemlogconnectionManager);
        private readonly OrderTrackingLogHandler _handler;
        public OrderTrackingLogManager()
        {
            _handler = new OrderTrackingLogHandler(DbScopeManager);
        }

        public void AddOrderTrackingLog(int orderid, string orderStatus, string deliveryStatus, string installStatus, string logisticTaskStatus, DescriptionType descriptionType)
        {
            var parms = new object[]
            {
               orderid,orderStatus??string.Empty,deliveryStatus??string.Empty,installStatus??string.Empty,logisticTaskStatus??string.Empty,descriptionType
            };

            TuHuThreadPool.QueueUserWorkItem(HandlerAddOrderTrackingLog, parms);
        }

        public void Add(int orderid, string orderStatus, string deliveryStatus, string installStatus, string logisticTaskStatus, DescriptionType descriptionType)
        {
            ParameterChecker.CheckNull(orderStatus, "orderStatus");
            ParameterChecker.CheckNull(deliveryStatus, "deliveryStatus");
            ParameterChecker.CheckNull(installStatus, "installStatus");
            ParameterChecker.CheckNull(logisticTaskStatus, "logisticTaskStatus");
            try
            {

                _handler.Add(orderid, orderStatus, deliveryStatus, installStatus, logisticTaskStatus, descriptionType);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch
            {

            }
        }

        public void HandlerAddOrderTrackingLog(object obj)
        {
            if (obj == null)
            {
                return;
            }
            try
            {
                var result = (object[])obj;
                var orderid = (int)result[0];
                var orderStatus = (string)result[1];
                var deliveryStatus = (string)result[2];
                var installStatus = (string)result[3];
                var logisticTaskStatus = (string)result[4];
                var descriptionType = (DescriptionType)result[5];
                Add(orderid, orderStatus, deliveryStatus, installStatus, logisticTaskStatus, descriptionType);
            }
            catch
            {
            }

        }
    }
}

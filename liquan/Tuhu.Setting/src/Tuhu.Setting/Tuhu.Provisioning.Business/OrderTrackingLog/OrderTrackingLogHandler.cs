using System;
using System.ComponentModel;
using System.Reflection;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.OrderTrackingLog
{
    internal class OrderTrackingLogHandler
    {
        private readonly IDBScopeManager _dbManager;
        private static readonly ILog Logger = LoggerFactory.GetLogger("SystemLog.OrderTrackingLog");
        internal OrderTrackingLogHandler(IDBScopeManager dbManager)
        {
            this._dbManager = dbManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="orderStatus">订单状态</param>
        /// <param name="deliveryStatus">配送状态</param>
        /// <param name="installStatus">安装状态</param>
        /// <param name="logisticTaskStatus">物流状态</param>
        /// <param name="descriptionType">描述类型</param>
        /// <returns></returns>
        public void Add(int orderid, string orderStatus, string deliveryStatus, string installStatus, string logisticTaskStatus, DescriptionType descriptionType)
        {

            //获取描述类型
            var desType = GetEnumDesc(descriptionType);
            var tblOrderTrackingLog = new OrderTrackingLogEntity
            {
                OrderId = orderid,
                OrderStatus = orderStatus,
                DeliveryStatus = deliveryStatus,
                InstallStatus = installStatus,
                LogisticTaskStatus = logisticTaskStatus,
                CreateTime = DateTime.Now,
                IsOver = descriptionType.Equals(DescriptionType.Delivery3Received) || descriptionType.Equals(DescriptionType.Install2Installed)
            };
            if (!string.IsNullOrEmpty(desType))
            {
                string desCription;
                //获取配置好的描述
                XmlDataSource.NodeCollection.TryGetValue(desType, out desCription);
                tblOrderTrackingLog.Description = desCription;
            }
            _dbManager.Execute(conn => DalOrderTrackingLog.Add(conn, tblOrderTrackingLog));
        }

        public static String GetEnumDesc(DescriptionType e)
        {
            FieldInfo enumInfo = e.GetType().GetField(e.ToString());
            var enumAttributes = (DescriptionAttribute[])enumInfo.
                GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (enumAttributes.Length > 0)
            {
                return enumAttributes[0].Description;
            }
            return e.ToString();
        }
    }
}

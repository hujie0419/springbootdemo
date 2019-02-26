using System;
using System.ComponentModel;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OrderTrackingLogEntity
    {

        public int PkId { get; set; }
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public string LogisticTaskStatus { get; set; }

        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
        public bool IsOver { get; set; }
        public string InstallStatus { get; set; }


    }
    //枚举订单状态追踪的状态
    public enum DescriptionType
    {
        [Description("Order0New")]
        Order0New,
        [Description("Order0NewConfirm")]
        Order0NewConfirm,
        [Description("Order1Purchased")]
        Order1Purchased,
        [Description("Order1Booked")]
        Order1Booked,
        [Description("Order1Booking")]
        Order1Booking,
        [Description("Order7Canceled")]
        Order7Canceled,
        [Description("Delivery2Sent")]
        Delivery2Sent,
        [Description("Delivery3ReceivedToShop")]
        Delivery3ReceivedToShop,
        [Description("Delivery3Received")]
        Delivery3Received,
        [Description("Delivery3.5Signed")]
        DeliverySigned,
        [Description("Install2Installed")]
        Install2Installed,
        [Description("Delivery2Prepared")]
        Delivery2Prepared

    }
}

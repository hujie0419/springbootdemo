using System.ComponentModel;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public static class OrderRelationshipType
    {
        public enum OrderRelationshipTypeEnum
        {
            [DescriptionAttribute("部分取消订单")]
            /// <summary>
            /// 部分取消关系：1
            /// </summary>
            PartCancelOrder = 1,

            [DescriptionAttribute("红冲订单")]
            /// <summary>
            /// 红冲关系：2
            /// </summary>
            HCOrder = 2,

            [DescriptionAttribute("安装单订单")]
            /// <summary>
            /// 补安装单：3
            /// </summary>
            InstallOrder = 3,

            /// <summary>
            /// 保养拆单关系：4
            /// </summary>
            SplitOrder = 4,

            /// <summary>
            /// 补礼品单:5
            /// </summary>
            GiftOrder = 5,

            /// <summary>
            /// 轮胎险关联订单
            /// </summary>
            TireInsuranceOrder = 6,

            [DescriptionAttribute("门店购买订单")]
            /// <summary>
            /// 门店购买关联订单
            /// </summary>
            ShopBuyOrder = 7,

            [DescriptionAttribute("补运费订单")]
            /// <summary>
            /// 补运费单：8
            /// </summary>
            DeliveryFeeOrder = 8,

            [DescriptionAttribute("补隔月服务费订单")]
            /// <summary>
            /// 补服务单：9
            /// </summary>
            PatchFUServiceOrder = 9,

            [DescriptionAttribute("复制关联订单")]
            /// <summary>
            /// 复制订单：10
            /// </summary>
            CopyOrder = 10,

            [DescriptionAttribute("门店赔付订单")]
            /// <summary>
            /// 门店赔付订单：11
            /// </summary>
            ShopPayOrder = 11,

            [DescriptionAttribute("补快修服务费订单")]
            /// <summary>
            /// 补快修服务费订单：12
            /// </summary>
            KuaiXiuServiceFeeOrder = 12,

            [DescriptionAttribute("补轮胎险订单")]
            /// <summary>
            /// 补轮胎险订单：13
            /// </summary>
            AddInsuranceOrder = 13
        }
    }

}




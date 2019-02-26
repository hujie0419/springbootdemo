namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     小游戏 - 订单变化
    /// </summary>
    public class GameOrderTackingRequest : GameObjectRequest
    {
        /// <summary>
        ///     订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 安装门店ID
        /// </summary>
        public int? InstallShopId { get; set; }

        /// <summary>
        /// 订单物流状态：
        /// 1.5Prepared	开始打包
        /// 1NotStarted 等待发货
        /// 2Sent 已发货
        /// 3.5Signed 已签收
        /// 3Received 已到店/已收货
        /// 4Error 状态异常
        /// 5PartRecheived 部分取回
        /// 5Recheived 已取回
        /// 7.2SentLoss 发出报损
        /// 7.2ShopLoss 到店报损
        /// 7.2SignedLoss 签收报损
        /// 7.3AcceptLoss 同意报损
        /// 7.4HasLossed 已报损
        /// </summary>
        public string DeliveryStatus { get; set; }

        /// <summary>
        /// 安装状态
        /// 1NotInstalled	尚未安装
        /// 2Installed 已安装
        /// 3NoInstall 无需安装
        /// 4Installing 安装中
        /// </summary>
        public string InstallStatus { get; set; }
    }
}

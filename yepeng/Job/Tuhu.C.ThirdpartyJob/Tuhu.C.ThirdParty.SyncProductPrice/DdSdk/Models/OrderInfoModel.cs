using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DdSdk.Api.Models
{
    public class OrderInfoModel
    {
        /// <summary>订单编号</summary>
        public long OrderID { get; set; }

        /// <summary>
        /// 100等待到款
        /// 101 等待发货（商家后台页面中显示为“等待配货”状态的订单
        /// 也会返回为“等待发货”）
        /// 300 已发货
        /// 400 已送达
        /// 1000 交易成功
        /// -100 取消
        /// 1100 交易失败
        /// -200 已拆单
        /// 50 等待审核
        /// </summary>
        public int OrderState { get; set; }

        /// <summary>买家留言</summary>
        public string Message { get; set; }

        /// <summary>商家备注</summary>
        public string Remark { get; set; }

        /// <summary>固定标记：1红色2黄色3绿色4蓝色5紫色</summary>
        public int? Label { get; set; }

        /// <summary>显示下单后订单“最后修改时间”，包括订单状态变化的修改，默认是下单时间</summary>
        public DateTime lastModifyTime { get; set; }

        /// <summary>付款时间</summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>发票抬头</summary>
        public string ReceiptName { get; set; }

        /// <summary>发票内容</summary>
        public string ReceiptDetails { get; set; }

        public BuyerInfoModel BuyerInfo { get; set; }
        public SendGoodsInfoModel SendGoodsInfo { get; set; }
        [JsonIgnore]
        public OrderItemInfoModel[] ItemsList { get; set; }

        [JsonIgnore]
        public PromotionItemModel[] PromotionList { get; set; }
    }
}

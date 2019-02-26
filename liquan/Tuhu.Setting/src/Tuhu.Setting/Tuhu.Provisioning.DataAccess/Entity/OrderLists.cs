using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OrderLists
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime OrderDatetime { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayMethod { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayMothed { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string iPayMothed { get; set; }

        /// <summary>
        /// 门店核对备注
        /// </summary>
        public string CheckComment { get; set; }

        /// <summary>
        /// 途虎处理备注
        /// </summary>
        public string HandleComment { get; set; }

        /// <summary>
        /// 红冲订单的源订单
        /// </summary>
        public int? ParentOrderId { get; set; }

        /// <summary>
        /// 红冲订单的源订单订单号
        /// </summary>
        public string ParentOrderNo { get; set; }
        /// <summary>
        /// 轮胎订单的源订单号
        /// </summary>
        public string TireOrderNo { get; set; }
        /// <summary>
        /// 核对状态
        /// </summary>
        public bool? CheckMark { get; set; }

        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallDatetime { get; set; }

		/// <summary>
		/// 安装类型
		/// </summary>
		public string InstallType { get; set; }

		/// <summary>
		/// 安装类型
		/// </summary>
		public string InstallTypeValue { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 途虎处理结果
        /// </summary>
        public bool? IsHandle { get; set; }

        //订单创建日期
        public string Date { get; set; }

        //订单状态
        public string Status { get; set; }
        //总价
        public Decimal? Money { get; set; }

        //已付款
        public Decimal? SumPaid { get; set; }

        //安装费
        public Decimal? InstallFree { get; set; }
        //服务费
        public Decimal? ServiceFree { get; set; }
        //详细信息
        public string Details { get; set; }
        //核对状态
        public string OrderMark { get; set; }
        //核对的意见
        public string OrderBillComment { get; set; }
        //结算结果
        public string PayOffResult { get; set; }
        //结算人
        public string OwnerName { get; set; }
        //结算时间
        public DateTime? PayOffDateTime { get; set; }
        //结算账号
        public string EmailAddress { get; set; }
        //门店名称
        public string ShopName { get; set; }
        //结算总额
        public decimal? SumMoney { get; set; }
        //总的安装费
        public decimal? SumInstallFee { get; set; }
        //总的服务费
        public decimal? SumServerFee { get; set; }
        //结算备注
        public string PayOffRemark { get; set; }
        //结算序列号
        public int PKID { get; set; }
        /// <summary>
        /// 交易参考号
        /// </summary>
        public string TranRefNum { get; set; }

        /// <summary>
        /// 关系值
        /// </summary>
        public string Relation { get; set; }

        public string PurchaseStatus { get; set; }

        public List<string> CancelOrders { get; set; }

		////public List<OrderRelationship> OrderRelationships { get; set; }

        //电子签购单拼html拼
		public string TranPicUrlHtml { get; set; }

        public string TranPicUrlBig { get; set; }

		public string TranPicUrlSmall { get; set; }

		/// <summary>
		/// POS机类别
		/// </summary>
		public string DataType { get; set; }

        //产品成本，不包含服务和保险
        //public decimal? SumProCost { get; set; }
        
        //渠道
        //public string OrderChannel { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string PayStatus { get; set; }
    }
}

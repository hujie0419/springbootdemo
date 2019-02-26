using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{

    public class CpsCommissionFlowRecordModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 佣金流水编号(与金融对账使用)
        /// </summary>
        public string CommissionFlowRecordNo { get; set; }

        /// <summary>
        /// 订单商品PKID
        /// </summary>
        public int OrderItemPKID { get; set; }

        /// <summary>
        /// 佣金比例ID（记录当时计算时配置的佣金比例）
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 红冲订单 退款时使用
        /// </summary>
        public string RedRushOrderId { get; set; }

        /// <summary>
        /// 达人ID
        /// </summary>
        public Guid DarenID { get; set; }


        /// <summary>
        /// 商品ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayPrice { get; set; }


        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 计算的佣金比例
        /// </summary>
        public decimal CommissionRatio { get; set; }

        /// <summary>
        /// 计算金额(单位分)
        /// </summary>
        public decimal ActutalAmount { get; set; }

        /// <summary>
        /// 类型; ValueAdded:增值;  Impairment:减值;
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 支付请求编号
        /// </summary>
        public Guid RequestNo { get; set; }


        /// <summary>
        /// 金融交易流水号(支付成功之后返回)
        /// </summary>
        public string TransactionNo { get; set; }

        /// <summary>
        /// NOTSENT：未发送;SENDFAILURE:发送失败，PROCESSING（处理中)，
        /// SUCCESS（成功），FAIL（失败），CALLBACKSUCCESSFUL.回调执行成功，CALLBACKFAILED:回调执行失败
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 是否删除; 1:是; 0:否;
        /// </summary>
        public string IsDelete { get; set; }

    }
}

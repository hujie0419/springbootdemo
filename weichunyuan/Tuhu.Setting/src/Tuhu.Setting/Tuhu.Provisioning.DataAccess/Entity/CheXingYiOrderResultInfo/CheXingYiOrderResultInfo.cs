using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CheXingYiOrderResultInfo
{
    public class CheXingYiOrderResultInfo
    {
        public string Code { set; get; }
        public string Errormsg { set; get; }
        public CheXingYiOrderResultData Data { set; get; }
    }

    public class CheXingYiOrderResultData
    {
        public string UserName { set; get; }
        public string OrderId { set; get; }
        public string CreateTime { set; get; }
        public string IsPost { set; get; }
        public string PostReceiveName { set; get; }
        public string PostReceivePhone { set; get; }
        public string PostReceiveAddress { set; get; }
        public string OrderStatus { set; get; }
        public string PayType { set; get; }
        public string Amount { set; get; }
        public string RecordsCount { set; get; }
        public List<CheXingYiOrderResultDetail> OrderDetails { set; get; }
    }

    public class CheXingYiOrderResultDetail
    {
        /*
         * 违章处理状态
            NeedPay:待付款;
            Deleted:已删除;
            Payed:已付款;
            Proceccing:正在办理;
            Drawbacked:已退款(单)
            Finished：已完成
         */

        public string ID { set; get; }
        /// <summary>车牌号</summary>
        public string Carnumber { set; get; }
        /// <summary>违章时间</summary>
        public string OccurTime { set; get; }
        /// <summary>违章地点</summary>
        public string Location { set; get; }
        public string LocationName { set; get; }
        /// <summary>违章描述</summary>
        public string Reason { set; get; }
        /// <summary>违章罚款</summary>
        public string Fine { set; get; }
        /// <summary>违章扣分</summary>
        public string Degree { set; get; }
        /// <summary>违章代码</summary>
        public string Violationcode { set; get; }
        /// <summary>代缴手续费</summary>
        public string Poundage { set; get; }
        /// <summary>状态</summary>
        public string Preorderstatus { set; get; }
        /// <summary>退单原因</summary>
        public string RefundReson { set; get; }
    }
}

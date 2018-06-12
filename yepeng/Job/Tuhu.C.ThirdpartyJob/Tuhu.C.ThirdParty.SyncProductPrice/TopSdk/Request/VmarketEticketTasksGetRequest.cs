using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.tasks.get
    /// </summary>
    public class VmarketEticketTasksGetRequest : BaseTopRequest<Top.Api.Response.VmarketEticketTasksGetResponse>
    {
        /// <summary>
        /// 码商ID，如果是码商，必须传，如果是信任卖家，不需要传
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 页码。取值范围:大于零的整数; 默认值:1
        /// </summary>
        public Nullable<long> PageNo { get; set; }

        /// <summary>
        /// 每页获取条数。默认值40，最小值1，最大值100。
        /// </summary>
        public Nullable<long> PageSize { get; set; }

        /// <summary>
        /// 卖家家ID(信任卖家不必传，码商可选)
        /// </summary>
        public Nullable<long> SellerId { get; set; }

        /// <summary>
        /// 返回结果类型:  1:返回通知失败的订单  2.返回通知成功回调失败的订单
        /// </summary>
        public Nullable<long> Type { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.tasks.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("page_no", this.PageNo);
            parameters.Add("page_size", this.PageSize);
            parameters.Add("seller_id", this.SellerId);
            parameters.Add("type", this.Type);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("type", this.Type);
        }

        #endregion
    }
}

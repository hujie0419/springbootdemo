using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wlb.item.consignment.create
    /// </summary>
    public class WlbItemConsignmentCreateRequest : BaseTopRequest<WlbItemConsignmentCreateResponse>
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public Nullable<long> ItemId { get; set; }

        /// <summary>
        /// 代销数量
        /// </summary>
        public Nullable<long> Number { get; set; }

        /// <summary>
        /// 货主商品id
        /// </summary>
        public Nullable<long> OwnerItemId { get; set; }

        /// <summary>
        /// 货主id
        /// </summary>
        public Nullable<long> OwnerUserId { get; set; }

        /// <summary>
        /// 通过taobao.wlb.item.authorization.add接口创建后得到的rule_id，规则中设定了代销商可以代销的商品数量
        /// </summary>
        public Nullable<long> RuleId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wlb.item.consignment.create";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("item_id", this.ItemId);
            parameters.Add("number", this.Number);
            parameters.Add("owner_item_id", this.OwnerItemId);
            parameters.Add("owner_user_id", this.OwnerUserId);
            parameters.Add("rule_id", this.RuleId);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("item_id", this.ItemId);
            RequestValidator.ValidateRequired("number", this.Number);
            RequestValidator.ValidateRequired("owner_item_id", this.OwnerItemId);
            RequestValidator.ValidateRequired("owner_user_id", this.OwnerUserId);
            RequestValidator.ValidateRequired("rule_id", this.RuleId);
        }

        #endregion
    }
}

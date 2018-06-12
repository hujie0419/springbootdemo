using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: cainiao.crm.oms.rule.sync
    /// </summary>
    public class CainiaoCrmOmsRuleSyncRequest : BaseTopRequest<Top.Api.Response.CainiaoCrmOmsRuleSyncResponse>
    {
        /// <summary>
        /// 人工审单规则描述
        /// </summary>
        public string CheckRuleMsg { get; set; }

        /// <summary>
        /// 是否系统智能处理订单（无人工介入）
        /// </summary>
        public Nullable<bool> IsAutoCheck { get; set; }

        /// <summary>
        /// 是否开启菜鸟自动流转规则
        /// </summary>
        public Nullable<bool> IsOpenCnauto { get; set; }

        /// <summary>
        /// 是否开启了订单合单
        /// </summary>
        public Nullable<bool> IsSysMergeOrder { get; set; }

        /// <summary>
        /// 订单合单周期（单位：分钟）
        /// </summary>
        public Nullable<long> MergeOrderCycle { get; set; }

        /// <summary>
        /// 其他未定义订单处理规则，格式｛name;stauts;cycle;｝
        /// </summary>
        public string OtherRule { get; set; }

        /// <summary>
        /// 店铺nick
        /// </summary>
        public string ShopCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "cainiao.crm.oms.rule.sync";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("check_rule_msg", this.CheckRuleMsg);
            parameters.Add("is_auto_check", this.IsAutoCheck);
            parameters.Add("is_open_cnauto", this.IsOpenCnauto);
            parameters.Add("is_sys_merge_order", this.IsSysMergeOrder);
            parameters.Add("merge_order_cycle", this.MergeOrderCycle);
            parameters.Add("other_rule", this.OtherRule);
            parameters.Add("shop_code", this.ShopCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("is_auto_check", this.IsAutoCheck);
            RequestValidator.ValidateRequired("is_open_cnauto", this.IsOpenCnauto);
            RequestValidator.ValidateRequired("shop_code", this.ShopCode);
        }

        #endregion
    }
}

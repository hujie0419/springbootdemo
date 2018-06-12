using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.einvoice.create.result.get
    /// </summary>
    public class AlibabaEinvoiceCreateResultGetRequest : BaseTopRequest<Top.Api.Response.AlibabaEinvoiceCreateResultGetResponse>
    {
        /// <summary>
        /// 外部平台店铺名称，需要在阿里发票平台配置，只有当platform_code不为TB和TM时，这个字段才生效。注意：后台配置的店铺平台必须和入参platform_code一致
        /// </summary>
        public string OutShopName { get; set; }

        /// <summary>
        /// 收款方税务登记证号
        /// </summary>
        public string PayeeRegisterNo { get; set; }

        /// <summary>
        /// 电商平台代码。淘宝：taobao，天猫：tmall
        /// </summary>
        public string PlatformCode { get; set; }

        /// <summary>
        /// 电商平台对应的订单号
        /// </summary>
        public string PlatformTid { get; set; }

        /// <summary>
        /// 流水号 (serial_no)和(platform_code,platform_tid)必须填写其中一组,serial_no优先级更高
        /// </summary>
        public string SerialNo { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.einvoice.create.result.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("out_shop_name", this.OutShopName);
            parameters.Add("payee_register_no", this.PayeeRegisterNo);
            parameters.Add("platform_code", this.PlatformCode);
            parameters.Add("platform_tid", this.PlatformTid);
            parameters.Add("serial_no", this.SerialNo);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("payee_register_no", this.PayeeRegisterNo);
        }

        #endregion
    }
}

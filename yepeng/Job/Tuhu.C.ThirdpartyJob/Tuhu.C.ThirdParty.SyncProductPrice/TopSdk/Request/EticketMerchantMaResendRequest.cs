using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.ma.resend
    /// </summary>
    public class EticketMerchantMaResendRequest : BaseTopRequest<Top.Api.Response.EticketMerchantMaResendResponse>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 待重发的码列表
        /// </summary>
        public string IsvMaList { get; set; }

        public List<IsvMaDomain> IsvMaList_ { set { this.IsvMaList = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 业务id（订单号）
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 需要跟发码通知获取到的参数一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.ma.resend";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("isv_ma_list", this.IsvMaList);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("token", this.Token);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("isv_ma_list", this.IsvMaList);
            RequestValidator.ValidateObjectMaxListSize("isv_ma_list", this.IsvMaList, 200);
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
            RequestValidator.ValidateRequired("token", this.Token);
        }

	/// <summary>
/// IsvMaDomain Data Structure.
/// </summary>
[Serializable]

public class IsvMaDomain : TopObject
{
	        /// <summary>
	        /// 串码码值
	        /// </summary>
	        [XmlElement("code")]
	        public string Code { get; set; }
	
	        /// <summary>
	        /// 码的可核销份数
	        /// </summary>
	        [XmlElement("num")]
	        public Nullable<long> Num { get; set; }
	
	        /// <summary>
	        /// 二维码图片文件名。已经申请了上传二维码的码商必填，其它码商无需关心。这个值是taobao.eticket.merchant.img.upload调用后的file_name
	        /// </summary>
	        [XmlElement("qr_image")]
	        public string QrImage { get; set; }
}

        #endregion
    }
}

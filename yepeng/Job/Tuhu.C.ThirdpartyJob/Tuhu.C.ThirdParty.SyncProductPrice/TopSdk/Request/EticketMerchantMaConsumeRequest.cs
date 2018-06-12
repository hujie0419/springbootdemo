using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.ma.consume
    /// </summary>
    public class EticketMerchantMaConsumeRequest : BaseTopRequest<Top.Api.Response.EticketMerchantMaConsumeResponse>
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 需要被核销的码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 核销份数
        /// </summary>
        public Nullable<long> ConsumeNum { get; set; }

        /// <summary>
        /// 核销后换码的码列表
        /// </summary>
        public string IsvMaList { get; set; }

        public List<IsvMaDomain> IsvMaList_ { set { this.IsvMaList = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 业务id（订单号）
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 机具编号
        /// </summary>
        public string PosId { get; set; }

        /// <summary>
        /// 核销序列号，需要保证唯一
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 需要跟发码通知获取到的参数一致
        /// </summary>
        public string Token { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.ma.consume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("code", this.Code);
            parameters.Add("consume_num", this.ConsumeNum);
            parameters.Add("isv_ma_list", this.IsvMaList);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("pos_id", this.PosId);
            parameters.Add("serial_num", this.SerialNum);
            parameters.Add("token", this.Token);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("code", this.Code);
            RequestValidator.ValidateRequired("consume_num", this.ConsumeNum);
            RequestValidator.ValidateObjectMaxListSize("isv_ma_list", this.IsvMaList, 20);
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
            RequestValidator.ValidateRequired("serial_num", this.SerialNum);
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

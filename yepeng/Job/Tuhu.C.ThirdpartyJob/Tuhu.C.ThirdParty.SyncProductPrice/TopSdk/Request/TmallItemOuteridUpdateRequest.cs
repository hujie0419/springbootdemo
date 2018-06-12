using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: tmall.item.outerid.update
    /// </summary>
    public class TmallItemOuteridUpdateRequest : BaseTopRequest<Top.Api.Response.TmallItemOuteridUpdateResponse>
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Nullable<long> ItemId { get; set; }

        /// <summary>
        /// 商品维度商家编码，如果不修改可以不传；清空请设置空串
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 商品SKU更新OuterId时候用的数据
        /// </summary>
        public string SkuOuters { get; set; }

        public List<UpdateSkuOuterIdDomain> SkuOuters_ { set { this.SkuOuters = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "tmall.item.outerid.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("item_id", this.ItemId);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("sku_outers", this.SkuOuters);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("item_id", this.ItemId);
            RequestValidator.ValidateObjectMaxListSize("sku_outers", this.SkuOuters, 2000);
        }

	/// <summary>
/// UpdateSkuOuterIdDomain Data Structure.
/// </summary>
[Serializable]

public class UpdateSkuOuterIdDomain : TopObject
{
	        /// <summary>
	        /// 被更新的Sku的商家外部id
	        /// </summary>
	        [XmlElement("outer_id")]
	        public string OuterId { get; set; }
	
	        /// <summary>
	        /// Sku属性串。格式:pid:vid;pid:vid,如: 1627207:3232483;1630696:3284570,表示机身颜色:军绿色;手机套餐:一电一充；如果填写将以属性对形式查找被更新SKU
	        /// </summary>
	        [XmlElement("properties")]
	        public string Properties { get; set; }
	
	        /// <summary>
	        /// SkuID，如果填写，将以SKUID查找被更新的SKU
	        /// </summary>
	        [XmlElement("sku_id")]
	        public Nullable<long> SkuId { get; set; }
}

        #endregion
    }
}

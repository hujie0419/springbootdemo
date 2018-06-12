using Newtonsoft.Json;
using System;

namespace PddSdk.Response
{
    [Serializable]
    public class PddOrderItem
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [JsonProperty("goods_id")]
        public string GoodsId { get; set; }
        /// <summary>
        /// Sku 编号
        /// </summary>
        [JsonProperty("sku_id")]
        public string SkuId { get; set; }
        /// <summary>
        /// 商家编码-SKU维度
        /// </summary>
        [JsonProperty("outer_id")]
        public string OuterId { get; set; }
        /// <summary>
        /// 商家编码-商品维度
        /// </summary>
        [JsonProperty("outer_goods_id")]
        public string OuterGoodsId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [JsonProperty("goods_name")]
        public string GoodsName { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        [JsonProperty("goods_price")]
        public decimal GoodsPrice { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        [JsonProperty("goods_spec")]
        public string GoodsSpec { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [JsonProperty("goods_count")]
        public int GoodsCount { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [JsonProperty("goods_img")]
        public string GoodsImg { get; set; }
    }
}

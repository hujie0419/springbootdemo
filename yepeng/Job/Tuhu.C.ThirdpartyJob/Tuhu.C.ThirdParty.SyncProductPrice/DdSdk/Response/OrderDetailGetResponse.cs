using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DdSdk.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DdSdk.Api.Response
{
    public class OrderDetailGetResponse : DdResponse
    {
        public OrderInfoModel Order { get; set; }

        protected override void SetValue(XmlNode xml)
        {
            var json = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.None, true));
            Order = json.ToObject<OrderInfoModel>();

            var items = json.Value<JObject>("ItemsList").Value<JToken>("ItemInfo");
            Order.ItemsList = items is JArray ? (items as JArray).ToObject<OrderItemInfoModel[]>() : new[] { (items as JObject).ToObject<OrderItemInfoModel>() };

            var promos = json.Value<JObject>("PromoList").Value<JToken>("promoItem");
            Order.PromotionList = promos is JArray ? (promos as JArray).ToObject<PromotionItemModel[]>() : new[] { (items as JObject).ToObject<PromotionItemModel>() };

            var receiptInfo = xml.SelectSingleNode("receiptInfo");
            if (receiptInfo != null)
            {
                Order.ReceiptName = receiptInfo.SelectSingleNode("receiptName").InnerText;
                Order.ReceiptDetails = receiptInfo.SelectSingleNode("receiptDetails").InnerText;
            }
        }
    }
}

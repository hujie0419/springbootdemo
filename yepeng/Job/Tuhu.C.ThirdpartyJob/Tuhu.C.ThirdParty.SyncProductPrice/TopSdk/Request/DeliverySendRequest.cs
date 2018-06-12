using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.delivery.send
    /// </summary>
    public class DeliverySendRequest : BaseTopRequest<Top.Api.Response.DeliverySendResponse>
    {
        /// <summary>
        /// 物流公司代码.如"POST"就代表中国邮政,"ZJS"就代表宅急送.调用 taobao.logistics.companies.get 获取。如传入的代码非淘宝官方物流合作公司，默认是“其他”物流的方式，在淘宝不显示物流具体进度，故传入需谨慎。如果orderType为delivery_needed，则必传
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// feature参数格式<br>范例: mobileCode=tid1:手机串号1,手机串号2|tid2:手机串号3;machineCode=tid3:3C机器号A,3C机器号B<br>mobileCode无忧购的KEY,machineCode为3C的KEY,多个key之间用”;”分隔<br>“tid1:手机串号1,手机串号2|tid2:手机串号3”为mobileCode对应的value。"|"不同商品间的分隔符。<br>例A商品和B商品都属于无忧购商品，之间就用"|"分开。<br>TID就是商品代表的子订单号，对应taobao.trade.fullinfo.get 接口获得的oid字段。(通过OID可以唯一定位到当前商品上)<br>":"TID和具体传入参数间的分隔符。冒号前表示TID,之后代表该商品的参数属性。<br>"," 属性间分隔符。（对应商品数量，当存在一个商品的数量超过1个时，用逗号分开）。<br>具体:当订单中A商品的数量为2个，其中手机串号分别为"12345","67890"。<br>参数格式：mobileCode=TIDA:12345,67890。TIDA对应了A宝贝，冒号后用逗号分隔的"12345","67890".说明本订单A宝贝的数量为2，值分别为"12345","67890"。<br>当存在"|"时，就说明订单中存在多个无忧购的商品，商品间用"|"分隔了开来。|"之后的内容含义同上。
        /// </summary>
        public string Feature { get; set; }

        /// <summary>
        /// 物流公司取货地址.XXX街道XXX门牌,省市区不需要提供.目的在于让物流公司能清楚的知道在哪取货。校验规则：1.4-60字符(字母\数字\汉字)2.不能全部数字3.不能全部字母
        /// </summary>
        public string FetcherAddress { get; set; }

        /// <summary>
        /// 取货地国家公布的标准地区码.参考:http://www.stats.gov.cn/tjbz/xzqhdm/t20080215_402462675.htm 或者调用 taobao.areas.get 获取
        /// </summary>
        public Nullable<long> FetcherAreaId { get; set; }

        /// <summary>
        /// 取货地手机号码
        /// </summary>
        public string FetcherMobile { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string FetcherName { get; set; }

        /// <summary>
        /// 取货地固定电话.包含区号,电话,分机号,中间用 " – "; 取货地固定电话和取货地手机号码,必须填写一个.
        /// </summary>
        public string FetcherPhone { get; set; }

        /// <summary>
        /// 取货地邮编
        /// </summary>
        public string FetcherZip { get; set; }

        /// <summary>
        /// 卖家备注.最大长度为250个字符。如果orderType为delivery_needed，则必传
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 发货类型. 可选( delivery_needed(物流订单发货),virtual_goods(虚拟物品发货). ) 注:选择virtual_goods类型进行发货的话下面的参数可以不需填写。如果选择delivery_needed 则company_code,out_sid,seller_name,seller_area_id,seller_address,seller_zip,seller_phone,seller_mobile,memo必须要填写
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 运单号.具体一个物流公司的真实运单号码。淘宝官方物流会校验，请谨慎传入；若company_code中传入的代码非淘宝官方物流合作公司，此处运单号不校验。如果orderType为delivery_needed，则必传
        /// </summary>
        public string OutSid { get; set; }

        /// <summary>
        /// 卖家地址(详细地址).如:XXX街道XXX门牌,省市区不需要提供。如果orderType为delivery_needed，则必传.<br><font color="red">校验规则：<br>1.4-60字符(字母\数字\汉字)<br>2.不能全部数字<br>3.不能全部字母<br></font>
        /// </summary>
        public string SellerAddress { get; set; }

        /// <summary>
        /// 卖家所在地国家公布的标准地区码.参考:http://www.stats.gov.cn/tjbz/xzqhdm/t20080215_402462675.htm  或者调用 taobao.areas.get 获取。如果orderType为delivery_needed，则必传
        /// </summary>
        public Nullable<long> SellerAreaId { get; set; }

        /// <summary>
        /// 卖家手机号码，必须由8到16位数字构成<br><font color="red">校验规则：<br>1.8-16位数字<br>2.不能数字全部相同<br>3.不能全为字符格式</font>
        /// </summary>
        public string SellerMobile { get; set; }

        /// <summary>
        /// 卖家姓名。如果orderType为delivery_needed。<font color=red>注：最长支持15个字符</font color=red>
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 卖家固定电话.包含区号,电话,分机号,中间用 " – "; 卖家固定电话和卖家手机号码,必须填写一个.<br><font color="red">校验规则：<br>1.字符不能全部相同<br>2.长度：5-24位<br>3.只能包含数字和横杠‘-’</font>
        /// </summary>
        public string SellerPhone { get; set; }

        /// <summary>
        /// 卖家邮编。如果orderType为delivery_needed，则必传
        /// </summary>
        public string SellerZip { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public Nullable<long> Tid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.delivery.send";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("company_code", this.CompanyCode);
            parameters.Add("feature", this.Feature);
            parameters.Add("fetcher_address", this.FetcherAddress);
            parameters.Add("fetcher_area_id", this.FetcherAreaId);
            parameters.Add("fetcher_mobile", this.FetcherMobile);
            parameters.Add("fetcher_name", this.FetcherName);
            parameters.Add("fetcher_phone", this.FetcherPhone);
            parameters.Add("fetcher_zip", this.FetcherZip);
            parameters.Add("memo", this.Memo);
            parameters.Add("order_type", this.OrderType);
            parameters.Add("out_sid", this.OutSid);
            parameters.Add("seller_address", this.SellerAddress);
            parameters.Add("seller_area_id", this.SellerAreaId);
            parameters.Add("seller_mobile", this.SellerMobile);
            parameters.Add("seller_name", this.SellerName);
            parameters.Add("seller_phone", this.SellerPhone);
            parameters.Add("seller_zip", this.SellerZip);
            parameters.Add("tid", this.Tid);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("tid", this.Tid);
        }

        #endregion
    }
}

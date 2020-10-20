#region head comment
/*
Code generate by JdSdkTool.
2013-12-21 15:53:12.98497 +08:00
*/
#endregion

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JdSdk.Domain;
using JdSdk.Response;

namespace JdSdk.Request
{
    /// <summary>
    /// 新增一个商品 Request
    /// </summary>
    public class WareAddRequest : JdRequestBase<WareAddResponse>
    {
        /// <summary>
        /// 类目id
        /// </summary>
        /// <example>1354</example>
        [XmlElement("cid")]
        [JsonProperty("cid")]
        public String Cid
        {
            get;
            set;
        }

        /// <summary>
        /// 商品所属的自定义店内分类列表。按分号分隔。结构：&quot;cid1-cid11;cid2-cid22;...&quot;如果自定义店内分类存在叶子类目，格式是cid1-cid11如果自定义店内分类不存在叶子类目，格式是cid1-cid1
        /// </summary>
        /// <example>206-208;207-208206(一级)-208(二级);207(一级)-207(一级)</example>
        [XmlElement("shop_category")]
        [JsonProperty("shop_category")]
        public String ShopCategory
        {
            get;
            set;
        }

        /// <summary>
        /// 标题（字数限制：45）
        /// </summary>
        [XmlElement("title")]
        [JsonProperty("title")]
        public String Title
        {
            get;
            set;
        }

        /// <summary>
        /// upc编码
        /// </summary>
        [XmlElement("upc_code")]
        [JsonProperty("upc_code")]
        public String UpcCode
        {
            get;
            set;
        }

        /// <summary>
        /// 操作类型，现只支持：offsale 或onsale下架类型请传入：offsale上架类型请传入：onsale如果不传默认为下架状态
        /// </summary>
        /// <example>如下架传入：offsale</example>
        [XmlElement("option_type")]
        [JsonProperty("option_type")]
        public String OptionType
        {
            get;
            set;
        }

        /// <summary>
        /// 外部商品编号，对应商家后台“货号”
        /// </summary>
        [XmlElement("item_num")]
        [JsonProperty("item_num")]
        public String ItemNum
        {
            get;
            set;
        }

        /// <summary>
        /// 生产厂商
        /// </summary>
        [XmlElement("producter")]
        [JsonProperty("producter")]
        public String Producter
        {
            get;
            set;
        }

        /// <summary>
        /// 包装规格
        /// </summary>
        [XmlElement("wrap")]
        [JsonProperty("wrap")]
        public String Wrap
        {
            get;
            set;
        }

        /// <summary>
        /// 长(单位:mm)
        /// </summary>
        [XmlElement("length")]
        [JsonProperty("length")]
        public String Length
        {
            get;
            set;
        }

        /// <summary>
        /// 宽(单位:mm)
        /// </summary>
        [XmlElement("wide")]
        [JsonProperty("wide")]
        public String Wide
        {
            get;
            set;
        }

        /// <summary>
        /// 高(单位:mm)
        /// </summary>
        [XmlElement("high")]
        [JsonProperty("high")]
        public String High
        {
            get;
            set;
        }

        /// <summary>
        /// 重量(单位:kg)
        /// </summary>
        [XmlElement("weight")]
        [JsonProperty("weight")]
        public String Weight
        {
            get;
            set;
        }

        /// <summary>
        /// 进货价
        /// </summary>
        [XmlElement("cost_price")]
        [JsonProperty("cost_price")]
        public String CostPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 市场价（支持小数点格式的商品类目：图书、音乐、影视、教育音像、个护化妆）
        /// </summary>
        [XmlElement("market_price")]
        [JsonProperty("market_price")]
        public String MarketPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 京东价（支持小数点格式的商品类目：图书、音乐、影视、教育音像、个护化妆）
        /// </summary>
        [XmlElement("jd_price")]
        [JsonProperty("jd_price")]
        public String JdPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 描述（最多支持3万个英文字符）
        /// </summary>
        [XmlElement("notes")]
        [JsonProperty("notes")]
        public String Notes
        {
            get;
            set;
        }

        /// <summary>
        /// 图片信息，通过程序对图片进行Byte[] 格式的处理后，生成参数信息（第一 读取图片；第二 将图片通过base64转化成字符串，然后赋值给ware_image属性）图片类型只支持：png和jpg的，图片须800x800的   图片不能大于1M根据URL换码协议，会将POST发送的数据里面是&quot;+&quot;全部被替换成空格，所以ware_image参数经过MD5加密完毕后，需要将ware_image对应的值中的&quot;+&quot;转换成%2B（如果php.ini配置自动转义，则不需要转码）
        /// </summary>
        [XmlElement("ware_image")]
        [JsonProperty("ware_image")]
        public List<byte> WareImage
        {
            get;
            set;
        }

        /// <summary>
        /// 包装清单
        /// </summary>
        [XmlElement("pack_listing")]
        [JsonProperty("pack_listing")]
        public String PackListing
        {
            get;
            set;
        }

        /// <summary>
        /// 售后服务
        /// </summary>
        [XmlElement("service")]
        [JsonProperty("service")]
        public String Service
        {
            get;
            set;
        }

        /// <summary>
        /// 库存数
        /// </summary>
        [XmlElement("stock_num")]
        [JsonProperty("stock_num")]
        public String StockNum
        {
            get;
            set;
        }

        /// <summary>
        /// 自定义属性值别名：  属性ID:属性值ID:别名  ，多组之间用^分开，如aid:vid:别名^aid1:vid1:别名1
        /// </summary>
        /// <example>1000000041:1500368001:淡蓝色</example>
        [XmlElement("property_alias")]
        [JsonProperty("property_alias")]
        public String PropertyAlias
        {
            get;
            set;
        }

        /// <summary>
        /// 商品属性列表,多组之间用&quot;|&quot;分隔，格式:aid:vid  或 aid:vid|aid1:vid1 或 aid1:vid1（需要从类目服务接口获取）如输入类型input_type为1或2，则attributes为必填属性；如输入类型input_type为3，则用字段input_str填入属性的值
        /// </summary>
        /// <example>3205:22101|3205:22102</example>
        [XmlElement("attributes")]
        [JsonProperty("attributes")]
        public String Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// sku 价格,多组之间用&quot;|&quot;分隔，格式:p1|p2
        /// </summary>
        /// <example>12|13</example>
        [XmlElement("sku_prices")]
        [JsonProperty("sku_prices")]
        public String SkuPrices
        {
            get;
            set;
        }

        /// <summary>
        /// sku 属性,一组sku 属性之间用&quot;^&quot;分隔，多组用&quot;|&quot;分隔格式:aid:vid^aid1:vid2|aid3:vid3^aid4:vid4（需要从类目服务接口获取）
        /// </summary>
        /// <example>100041:150041^1000046:15844|1001:1501^10006:1504</example>
        [XmlElement("sku_properties")]
        [JsonProperty("sku_properties")]
        public String SkuProperties
        {
            get;
            set;
        }

        /// <summary>
        /// sku 库存,多组之间用&quot;|&quot;分隔， 格式:s1|s2
        /// </summary>
        [XmlElement("sku_stocks")]
        [JsonProperty("sku_stocks")]
        public String SkuStocks
        {
            get;
            set;
        }

        /// <summary>
        /// SKU外部ID，对个之间用&quot;|&quot;分隔格，比如：sdf|sds（支持没有sku的情况下，可以输入外部id，并将外部id绑定在默认生成的sku上），对应商家后台“商家skuid”
        /// </summary>
        [XmlElement("outer_id")]
        [JsonProperty("outer_id")]
        public String OuterId
        {
            get;
            set;
        }

        /// <summary>
        /// 流水号
        /// </summary>
        [XmlElement("trade_no")]
        [JsonProperty("trade_no")]
        public String TradeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否先款后货 , false为否，true为是
        /// </summary>
        /// <example>&quot;true&quot;</example>
        [XmlElement("is_pay_first")]
        [JsonProperty("is_pay_first")]
        public String IsPayFirst
        {
            get;
            set;
        }

        /// <summary>
        /// 发票限制：非必须输入，true为限制，false为不限制开增值税发票，FBP、LBP、SOPL、SOP类型商品均可输入；
        /// </summary>
        /// <example>&quot;true&quot;</example>
        [XmlElement("is_can_vat")]
        [JsonProperty("is_can_vat")]
        public String IsCanVat
        {
            get;
            set;
        }

        /// <summary>
        /// 是否进口商品：非必须输入，false为否，true为是，FBP类型商品可输入；
        /// </summary>
        /// <example>&quot;true&quot;</example>
        [XmlElement("is_imported")]
        [JsonProperty("is_imported")]
        public String IsImported
        {
            get;
            set;
        }

        /// <summary>
        /// 是否保健品：非必须输入，false为否，true为是，FBP类型商品可输入；
        /// </summary>
        /// <example>&quot;false&quot;</example>
        [XmlElement("is_health_product")]
        [JsonProperty("is_health_product")]
        public String IsHealthProduct
        {
            get;
            set;
        }

        /// <summary>
        /// 是否保质期管理商品, false为否，true为是
        /// </summary>
        /// <example>false</example>
        [XmlElement("is_shelf_life")]
        [JsonProperty("is_shelf_life")]
        public String IsShelfLife
        {
            get;
            set;
        }

        /// <summary>
        /// 保质期：非必须输入，0-99999范围区间，FBP类型商品可输入；
        /// </summary>
        /// <example>&quot;999&quot;</example>
        [XmlElement("shelf_life_days")]
        [JsonProperty("shelf_life_days")]
        public String ShelfLifeDays
        {
            get;
            set;
        }

        /// <summary>
        /// 是否序列号管理：非必须输入，false为否，true为是，FBP类型商品可输入；
        /// </summary>
        /// <example>&quot;true&quot;</example>
        [XmlElement("is_serial_no")]
        [JsonProperty("is_serial_no")]
        public String IsSerialNo
        {
            get;
            set;
        }

        /// <summary>
        /// 大家电购物卡：非必须输入，false为否，true为是，FBP类型商品可输入；
        /// </summary>
        /// <example>&quot;true&quot;</example>
        [XmlElement("is_appliances_card")]
        [JsonProperty("is_appliances_card")]
        public String IsAppliancesCard
        {
            get;
            set;
        }

        /// <summary>
        /// 是否特殊液体：非必须输入，false为否，true为是，FBP、LBP、SOPL类型商品可输入；
        /// </summary>
        /// <example>&quot;false&quot;</example>
        [XmlElement("is_special_wet")]
        [JsonProperty("is_special_wet")]
        public String IsSpecialWet
        {
            get;
            set;
        }

        /// <summary>
        /// 商品件型：必须输入，0免费、1超大件、2超大件半件、3大件、4大件半件、5中件、6中件半件、7小件、8超小件，FBP类型商品必须输入；
        /// </summary>
        /// <example>&quot;2&quot;</example>
        [XmlElement("ware_big_small_model")]
        [JsonProperty("ware_big_small_model")]
        public String WareBigSmallModel
        {
            get;
            set;
        }

        /// <summary>
        /// 商品包装：必须输入，1普通商品、2易碎品、3裸瓶液体、4带包装液体、5按原包装出库，FBP类型商品必须输入；
        /// </summary>
        /// <example>&quot;2&quot;</example>
        [XmlElement("ware_pack_type")]
        [JsonProperty("ware_pack_type")]
        public String WarePackType
        {
            get;
            set;
        }

        /// <summary>
        /// 用户自行输入的类目属性ID串结构：&quot;pid1|pid2|pid3&quot;,属性的pid调用360buy.ware.get.attribute取得, 输入类型input_type=3即输入
        /// </summary>
        /// <example>1132632|12142|876231</example>
        [XmlElement("input_pids")]
        [JsonProperty("input_pids")]
        public String InputPids
        {
            get;
            set;
        }

        /// <summary>
        /// 用户自行输入的属性值,结构:&quot;输入值|输入值2|输入值3&quot;图书品类输入值规则：ISBN：数字、字母格式出版时间：日期格式“yyyy-mm-dd”版次：数字格式印刷时间：日期格式“yyyy-mm-dd”印次：数字格式页数：数字格式字数：数字格式套装数量：数字格式附件数量：数字格式
        /// </summary>
        /// <example>内容1|内容2|内容3</example>
        [XmlElement("input_strs")]
        [JsonProperty("input_strs")]
        public String InputStrs
        {
            get;
            set;
        }

        /// <summary>
        /// 是否输入验证码 true:是;false:否
        /// </summary>
        /// <example>&quot;true&quot;</example>
        [XmlElement("has_check_code")]
        [JsonProperty("has_check_code")]
        public String HasCheckCode
        {
            get;
            set;
        }

        /// <summary>
        /// 广告词内容最大支持45个字符
        /// </summary>
        /// <example>&quot;广告词内容&quot;</example>
        [XmlElement("ad_content")]
        [JsonProperty("ad_content")]
        public String AdContent
        {
            get;
            set;
        }

        /// <summary>
        /// 定时上架时间  时间格式：yyyy-MM-dd HH:mm:ss ;规则是大于当前时间，10天内。
        /// </summary>
        /// <example>&quot;2013-08-19  15:24:43&quot;</example>
        [XmlElement("list_time")]
        [JsonProperty("list_time")]
        public String ListTime
        {
            get;
            set;
        }

        public override String ApiName
        {
            get{ return "360buy.ware.add"; }
        }

        protected override void PrepareParam(IDictionary<String, Object> paramters)
        {
            paramters.Add("cid" ,this.Cid);
            paramters.Add("shop_category" ,this.ShopCategory);
            paramters.Add("title" ,this.Title);
            paramters.Add("upc_code" ,this.UpcCode);
            paramters.Add("option_type" ,this.OptionType);
            paramters.Add("item_num" ,this.ItemNum);
            paramters.Add("producter" ,this.Producter);
            paramters.Add("wrap" ,this.Wrap);
            paramters.Add("length" ,this.Length);
            paramters.Add("wide" ,this.Wide);
            paramters.Add("high" ,this.High);
            paramters.Add("weight" ,this.Weight);
            paramters.Add("cost_price" ,this.CostPrice);
            paramters.Add("market_price" ,this.MarketPrice);
            paramters.Add("jd_price" ,this.JdPrice);
            paramters.Add("notes" ,this.Notes);
            paramters.Add("ware_image" ,this.WareImage);
            paramters.Add("pack_listing" ,this.PackListing);
            paramters.Add("service" ,this.Service);
            paramters.Add("stock_num" ,this.StockNum);
            paramters.Add("property_alias" ,this.PropertyAlias);
            paramters.Add("attributes" ,this.Attributes);
            paramters.Add("sku_prices" ,this.SkuPrices);
            paramters.Add("sku_properties" ,this.SkuProperties);
            paramters.Add("sku_stocks" ,this.SkuStocks);
            paramters.Add("outer_id" ,this.OuterId);
            paramters.Add("trade_no" ,this.TradeNo);
            paramters.Add("is_pay_first" ,this.IsPayFirst);
            paramters.Add("is_can_vat" ,this.IsCanVat);
            paramters.Add("is_imported" ,this.IsImported);
            paramters.Add("is_health_product" ,this.IsHealthProduct);
            paramters.Add("is_shelf_life" ,this.IsShelfLife);
            paramters.Add("shelf_life_days" ,this.ShelfLifeDays);
            paramters.Add("is_serial_no" ,this.IsSerialNo);
            paramters.Add("is_appliances_card" ,this.IsAppliancesCard);
            paramters.Add("is_special_wet" ,this.IsSpecialWet);
            paramters.Add("ware_big_small_model" ,this.WareBigSmallModel);
            paramters.Add("ware_pack_type" ,this.WarePackType);
            paramters.Add("input_pids" ,this.InputPids);
            paramters.Add("input_strs" ,this.InputStrs);
            paramters.Add("has_check_code" ,this.HasCheckCode);
            paramters.Add("ad_content" ,this.AdContent);
            paramters.Add("list_time" ,this.ListTime);
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("cid", this.Cid);
            RequestValidator.ValidateRequired("title", this.Title);
            RequestValidator.ValidateRequired("length", this.Length);
            RequestValidator.ValidateRequired("wide", this.Wide);
            RequestValidator.ValidateRequired("high", this.High);
            RequestValidator.ValidateRequired("weight", this.Weight);
            RequestValidator.ValidateRequired("market_price", this.MarketPrice);
            RequestValidator.ValidateRequired("jd_price", this.JdPrice);
            RequestValidator.ValidateRequired("notes", this.Notes);
            RequestValidator.ValidateRequired("ware_image", this.WareImage);
            RequestValidator.ValidateRequired("stock_num", this.StockNum);
            RequestValidator.ValidateRequired("attributes", this.Attributes);
        }

    }
}

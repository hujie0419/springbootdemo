using System;
using System.Collections.Generic;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class WebActive : BaseModel
    {
        public WebActive() { }
        public WebActive(DataRow row) : base(row) { }
        /// <summary>
        /// 活动主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public string ActiveID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActiveName { get; set; }
        /// <summary>
        /// 活动链接
        /// </summary>
        public string ActiveLink { get; set; }
        /// <summary>
        /// 活动说明
        /// </summary>
        public string ActiveDescription { get; set; }
        /// <summary>
        /// 活动Banner
        /// </summary>
        public string Banner { get; set; }
        /// <summary>
        /// 活动角标
        /// </summary>
        public string CornerMark { get; set; }
        /// <summary>
        /// 页面背景色
        /// </summary>
        public string backgroundColor { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// 活动创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        public IEnumerable<CommodityFloor> CommodifyFloor { get; set; }
        public IEnumerable<OtherPart> OtherPart { get; set; }
    }
    public class CommodityFloor : BaseModel
    {
        public CommodityFloor() { }
        public CommodityFloor(DataRow row) : base(row) { }
        /// <summary>
        /// 活动ID
        /// </summary>
        public string ActiveID { get; set; }
        /// <summary>
        /// 楼层ID(出现在页面上的顺序)
        /// </summary>
        public int FloorID { get; set; }
        /// <summary>
        /// 楼层图片
        /// </summary>
        public string FloorPicture { get; set; }
        /// <summary>
        /// 楼层链接
        /// </summary>
        public string FloorLink { get; set; }
        /// <summary>
        /// 楼层中的产品
        /// </summary>
        public IEnumerable<Products> Products { get; set; }
    }
    public class Products : BaseModel
    {
        public Products() { }
        public Products(DataRow row) : base(row) { }
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        public string ActiveID { get; set; }
        /// <summary>
        /// 楼层ID(出现在页面上的顺序)
        /// </summary>
        public int FloorID { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 产品VariantID
        /// </summary>
        public string VariantID { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 展示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 途虎价
        /// </summary>
        public Decimal? Price { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public Decimal? MarketPrice { get; set; }
        /// <summary>
        /// 产品出现的顺序
        /// </summary>
        public int orderID { get; set; }
    }
    public class OtherPart : BaseModel
    {
        public OtherPart() { }
        public OtherPart(DataRow row) : base(row) { }
        /// <summary>
        /// 活动ID
        /// </summary>
        public string ActiveID { get; set; }
        /// <summary>
        /// 其他部分的主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 标识是哪块（如：底部链接）
        /// </summary>
        public int PartID { get; set; }
        /// <summary>
        /// 显示的图片
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// 可能需要的链接地址
        /// </summary>
        public string PartLink { get; set; }

        /// <summary>
        /// 出现的顺序
        /// </summary>
        public int orderID { get; set; }

    }

    public class ConfigHistory : BaseModel
    {
        public ConfigHistory() { }
        public ConfigHistory(DataRow row) : base(row) { }
        /// <summary>
        /// 操作历史的主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public string ObjectID { get; set; }
        /// <summary>
        /// 类型  WebAct网站活动
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 操作IP
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime ChangeDatetime { get; set; }

        public string BeforeValue { get; set; }

        public string AfterValue { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2
{
    public class GroupBuyingProductConfigEntity
    {
        public string ProductName { get; set; }
        public string SimpleProductName { get; set; }
        public string PID { get; set; }
        public string ProductGroupId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal SpecialPrice { get; set; }
        public bool DisPlay { get; set; }
        public string Creator { get; set; }
        public bool UseCoupon { get; set; }
        public int UpperLimitPerOrder { get; set; }
        public bool IsShow { get; set; }
        public int BuyLimitCount { get; set; }
        /// <summary>
        /// 配置总库存
        /// </summary>
        public int TotalStockCount { get; set; }
        /// <summary>
        /// 产品已消耗库存
        /// </summary>
        public int CurrentSoldCount { get; set; }
        /// <summary>
        /// 是否自动库存
        /// </summary>
        public bool IsAutoStock { get; set; }
        /// <summary>
        /// 是否显示在APP
        /// </summary>
        public bool IsShowApp { get; set; }
    }
    public class GroupBuyingProductGroupConfigEntity
    {
        public int PKID { get; set; }
        /// <summary>
        /// 团id
        /// </summary>
        public string ProductGroupId { get; set; }
        /// <summary>
        /// 团类型
        /// </summary>
        public int GroupType { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 实际库存
        /// </summary>
        public int? CurrentGroupCount { get; set; }
        /// <summary>
        /// 上限库存
        /// </summary>
        public int? TotalGroupCount { get; set; }
        /// <summary>
        /// 活动价
        /// </summary>
        public decimal? FinalPrice { get; set; }
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 下架时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sequence { get; set; }
        /// <summary>
        /// 拼团人数
        /// </summary>
        public int? MemberCount { get; set; }
        /// <summary>
        /// 宣传图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 商品标签
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 分享文案
        /// </summary>
        public string ShareId { get; set; }
        /// <summary>
        /// 特殊人群
        /// </summary>
        public int SpecialUserTag { get; set; }
        /// <summary>
        /// 首页是否显示
        /// </summary>
        public bool IsShow { get; set; }

        public int IsShowPage { get; set; }
        /// <summary>
        /// 拼团类别（0-购买拼团;1-抽奖拼团）
        /// </summary>
        public int GroupCategory { get; set; }
        /// <summary>
        /// 拼团描述
        /// </summary>
        public string GroupDescription { get; set; }
        /// <summary>
        /// 拼团库存
        /// </summary>
        public GroupBuyingStockModel GroupStockInfo { get; set; }

        public List<GroupBuyingProductConfigEntity> GroupProductDetails { get; set; }
        /// <summary>
        /// 是否允许使用优惠券
        /// </summary>
        public bool ApplyCoupon { get; set; }
        /// <summary>
        /// 分享图片
        /// </summary>
        public string ShareImage { get; set; }
        /// <summary>
        /// 显示渠道
        /// </summary>
        public string Channel { get; set; }

        public int Total { get; set; }
    }

    public class GroupBuyingConfigLogModel
    {
        public string Name { get; set; }
        public string Title { get; set; }

        public string ProductGroupId { get; set; }
        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}

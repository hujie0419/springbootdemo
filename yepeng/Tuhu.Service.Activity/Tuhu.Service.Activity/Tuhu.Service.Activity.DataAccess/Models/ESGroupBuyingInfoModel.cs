using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Service.Activity.Server.Model
{
    [ElasticsearchType(IdProperty = "ProductIndex", Name = "GroupBuyingProduct")]
    public class ESGroupBuyingProductModel
    {
        [String(Name = nameof(ProductIndex), Index = FieldIndexOption.NotAnalyzed)]
        public string ProductIndex { get; set; }

        [String(Name = nameof(PID), Index = FieldIndexOption.NotAnalyzed)]
        public string PID { get; set; }

        [String(Name = nameof(ProductName), Index = FieldIndexOption.NotAnalyzed)]
        public string ProductName { get; set; }

        /// <summary>
        /// 官方售价
        /// </summary>
        [Number(Name = nameof(OriginalPrice), Index = NonStringIndexOption.NotAnalyzed)]
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 活动价
        /// </summary>
        [Number(Name = nameof(ActivityPrice), Index = NonStringIndexOption.NotAnalyzed)]
        public decimal ActivityPrice { get; set; }

        /// <summary>
        /// 团长特价
        /// </summary>
        [Number(Name = nameof(CaptainPrice), Index = NonStringIndexOption.NotAnalyzed)]
        public decimal CaptainPrice { get; set; }

        /// <summary>
        /// 产品是否可用优惠券
        /// </summary>
        [Boolean(Name = nameof(UseCoupon), Index = NonStringIndexOption.NotAnalyzed)]
        public bool UseCoupon { get; set; }

        /// <summary>
        /// 每单产品最大数量
        /// </summary>
        [Number(Name = nameof(UpperLimitPerOrder), Index = NonStringIndexOption.NotAnalyzed)]
        public int UpperLimitPerOrder { get; set; }

        /// <summary>
        /// 每人限购单数
        /// </summary>
        [Number(Name = nameof(BuyLimitCount), Index = NonStringIndexOption.NotAnalyzed)]
        public int BuyLimitCount { get; set; }

        /// <summary>
        /// 是否产品组默认商品
        /// </summary>
        [Boolean(Name = nameof(IsDefault), Index = NonStringIndexOption.NotAnalyzed)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 拼团产品根类目
        /// </summary>
        [String(Name = nameof(RootCategory), Index = FieldIndexOption.NotAnalyzed)]
        public string RootCategory { get; set; }

        /// <summary>
        /// 产品多级类目oid列表
        /// </summary>
        [Number(Name = nameof(ProductCategoryList), Index = NonStringIndexOption.NotAnalyzed)]
        public List<int> ProductCategoryList { get; set; }

        /// <summary>
        /// 拼团类目Code
        /// </summary>
        [String(Name = nameof(NewCategoryCode), Index = FieldIndexOption.NotAnalyzed)]
        public List<string> NewCategoryCode { get; set; }

        /// <summary>
        /// 拼团类目名称
        /// </summary>
        [String(Name = nameof(NewCategoryName), Index = FieldIndexOption.NotAnalyzed)]
        public List<string> NewCategoryName { get; set; }
        /// <summary>
        /// 关联类目ID
        /// </summary>
        [Number(Name = nameof(NewCategoryId), Index = NonStringIndexOption.NotAnalyzed)]
        public List<int> NewCategoryId { get; set; }
        /// <summary>
        /// 拼团产品是否显示
        /// </summary>
        [Boolean(Name = nameof(IsShow), Index = NonStringIndexOption.NotAnalyzed)]
        public bool IsShow { get; set; }

        [String(Name = nameof(ProductGroupId), Index = FieldIndexOption.NotAnalyzed)]
        public string ProductGroupId { get; set; }

        /// <summary>
        /// 拼团活动ID
        /// </summary>
        [String(Name = nameof(ActivityId), Index = FieldIndexOption.NotAnalyzed)]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 团类型
        /// </summary>
        [Number(Name = nameof(GroupType), Index = NonStringIndexOption.NotAnalyzed)]
        public int GroupType { get; set; }

        /// <summary>
        /// 首页显示顺序
        /// </summary>
        [Number(Name = nameof(Sequence), Index = NonStringIndexOption.NotAnalyzed)]
        public int Sequence { get; set; }

        /// <summary>
        /// 已团单数
        /// </summary>
        [Number(Name = nameof(GroupOrderCount), Index = NonStringIndexOption.NotAnalyzed)]
        public int GroupOrderCount { get; set; }
        /// <summary>
        /// 显示渠道
        /// </summary>
        [String(Name = nameof(Channel), Index = FieldIndexOption.NotAnalyzed)]
        public string Channel { get; set; }

        /// <summary>
        /// 特殊人群编号
        /// </summary>
        [Number(Name = nameof(SpecialUser), Index = NonStringIndexOption.NotAnalyzed)]
        public int SpecialUser { get; set; }

        /// <summary>
        /// 针对老用户的排序（3-优惠券团；2-精品团；1-优惠券团；0-其他）
        /// </summary>
        [Number(Name = nameof(OldUserSort), Index = NonStringIndexOption.NotAnalyzed)]
        public int OldUserSort { get; set; }

        /// <summary>
        /// 针对新用户的排序（3-优惠券图案；2-低价团；1-精品团；0-其他）
        /// </summary>
        [Number(Name = nameof(NewUserSort), Index = NonStringIndexOption.NotAnalyzed)]
        public int NewUserSort { get; set; }

        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 运营标签
        /// </summary>
        [String(Name = nameof(Label), Index = FieldIndexOption.NotAnalyzed)]
        public string Label { get; set; }

        /// <summary>
        /// 产品组列表页是否显示
        /// </summary>
        [Boolean(Name = nameof(GroupIsShow), Index = NonStringIndexOption.NotAnalyzed)]
        public bool GroupIsShow { get; set; }

        /// <summary>
        /// 拼团类别：0-普通拼团;1-抽奖拼团; 2-优惠券拼团
        /// </summary>
        [Number(Name = nameof(GroupCategory), Index = NonStringIndexOption.NotAnalyzed)]
        public int GroupCategory { get; set; }
        [String(Name = nameof(ChannelList), Index = FieldIndexOption.NotAnalyzed)]
        public List<string> ChannelList => Channel.Split(';')?.ToList() ?? new List<string>();
        /// <summary>
        /// 最大分词搜索字段
        /// </summary>
        [String(Name = nameof(SearchKeyForMax), Index = FieldIndexOption.Analyzed, Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public string SearchKeyForMax => $"{ProductName}/{PID}";
    }

    [ElasticsearchType(IdProperty = "ProductGroupId", Name = "ProductGroup")]
    public class ESGroupBuyingGroupModel
    {
        /// <summary>
        /// 产品组Id
        /// </summary>
        [String(Name = nameof(ProductGroupId), Index = FieldIndexOption.NotAnalyzed)]
        public string ProductGroupId { get; set; }

        /// <summary>
        /// 列表页显示图片
        /// </summary>
        [String(Name = nameof(Image), Index = FieldIndexOption.NotAnalyzed)]
        public string Image { get; set; }

        /// <summary>
        /// 分享id
        /// </summary>
        [String(Name = nameof(ShareId), Index = FieldIndexOption.NotAnalyzed)]
        public string ShareId { get; set; }

        /// <summary>
        /// 团类型
        /// </summary>
        [Number(Name = nameof(GroupType), Index = NonStringIndexOption.NotAnalyzed)]
        public int GroupType { get; set; }

        /// <summary>
        /// 成团所需成员数量
        /// </summary>
        [Number(Name = nameof(MemberCount), Index = NonStringIndexOption.NotAnalyzed)]
        public int MemberCount { get; set; }

        /// <summary>
        /// 首页显示顺序
        /// </summary>
        [Number(Name = nameof(Sequence), Index = NonStringIndexOption.NotAnalyzed)]
        public int Sequence { get; set; }

        /// <summary>
        /// 当前拼团数量
        /// </summary>
        [Number(Name = nameof(CurrentGroupCount), Index = NonStringIndexOption.NotAnalyzed)]
        public int CurrentGroupCount { get; set; }

        /// <summary>
        /// 配置库存数量
        /// </summary>
        [Number(Name = nameof(TotalGroupCount), Index = NonStringIndexOption.NotAnalyzed)]
        public int TotalGroupCount { get; set; }

        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 运营标签
        /// </summary>
        [String(Name = nameof(Label), Index = FieldIndexOption.NotAnalyzed)]
        public string Label { get; set; }

        /// <summary>
        /// 对应的活动ID
        /// </summary>
        [String(Name = nameof(ActivityId), Index = FieldIndexOption.NotAnalyzed)]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 已团单数
        /// </summary>
        [Number(Name = nameof(GroupOrderCount), Index = NonStringIndexOption.NotAnalyzed)]
        public int GroupOrderCount { get; set; }

        /// <summary>
        /// 列表页是否显示
        /// </summary>
        [Boolean(Name = nameof(IsShow), Index = NonStringIndexOption.NotAnalyzed)]
        public bool IsShow { get; set; }

        /// <summary>
        /// 拼团类别：0-普通拼团;1-抽奖拼团; 2-优惠券拼团
        /// </summary>
        [Number(Name = nameof(GroupCategory), Index = NonStringIndexOption.NotAnalyzed)]
        public int GroupCategory { get; set; }

        /// <summary>
        /// 拼团抽奖规则
        /// </summary>
        [String(Name = nameof(GroupDescription), Index = FieldIndexOption.NotAnalyzed)]
        public string GroupDescription { get; set; }

        /// <summary>
        /// 分享图片
        /// </summary>
        [String(Name = nameof(ShareImage), Index = FieldIndexOption.NotAnalyzed)]
        public string ShareImage { get; set; }
        /// <summary>
        /// 显示渠道
        /// </summary>
        [String(Name = nameof(Channel), Index = FieldIndexOption.NotAnalyzed)]
        public string Channel { get; set; }
        /// <summary>
        /// 特殊人群编号
        /// </summary>
        [Number(Name = nameof(SpecialUser), Index = NonStringIndexOption.NotAnalyzed)]
        public int SpecialUser { get; set; }
        /// <summary>
        /// 针对老用户的排序（3-优惠券团；2-精品团；1-优惠券团；0-其他）
        /// </summary>
        [Number(Name = nameof(OldUserSort), Index = NonStringIndexOption.NotAnalyzed)]
        public int OldUserSort { get; set; }
        /// <summary>
        /// 针对新用户的排序（3-优惠券图案；2-低价团；1-精品团；0-其他）
        /// </summary>
        [Number(Name = nameof(NewUserSort), Index = NonStringIndexOption.NotAnalyzed)]
        public int NewUserSort { get; set; }
        /// <summary>
        /// 默认拼团产品
        /// </summary>
        [String(Name = nameof(DefaultProduct), Index = FieldIndexOption.NotAnalyzed)]
        public string DefaultProduct { get; set; }
        [String(Name = nameof(Pids), Index = FieldIndexOption.NotAnalyzed)]
        public List<string> Pids { get; set; }
    }


    public class CategoryInfoModel
    {
        public string PID { get; set; }
        public string RootCategory { get; set; }
        public List<int> CategoryList { get; set; }
        public List<string> NewCategoryCode { get; set; }
        public List<string> NewCategoryName { get; set; }
        public List<int> NewCategoryId { get; set; }
    }

    public class GetReleaseCategoryInfo
    {
        public int OId { get; set; }
        public string CategoryCode { get; set; }
        public string DisplayName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    #region [请求Model]

    /// <summary>
    /// 查询团信息请求Model
    /// </summary>
    public class GroupInfoRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 请求用户
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 0-全部拼团；1-拼团中；2-已完成拼团；3-拼团失败；5-待付款
        /// </summary>
        public int Type { get; set; } = 0;
    }

    /// <summary>
    /// 按照团类型分页查询拼团产品列表
    /// </summary>
    public class ActivityGroupRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// 0-普通团；1-新人团；2-团长免单;3-团长免单
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 批量查询拼团拼团产品信息请求接口
    /// </summary>
    public class GroupBuyingProductRequest
    {
        public string ProductGroupId { get; set; }
        public string PId { get; set; }
    }
    public class GroupBuyingBuyLimitRequest
    {
        public string ProductGroupId { get; set; }
        public string PID { get; set; }
        public Guid UserId { get; set; }
    }

    public class GroupBuyingQueryRequest
    {
        /// <summary>
        /// 类目Id
        /// </summary>
        public int NewCategoryCode { get; set; }
        public string Channel { get; set; }
        /// <summary>
        /// 0-按照配置优先级排序；1-按照运营标签+销量排序（降序）；2-根据销量排序（降序）；3-价格升序；4-价格降序
        /// </summary>
        public int SortType { get; set; }
        /// <summary>
        /// 是否老用户
        /// </summary>
        public bool IsOldUser { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string KeyWord { get; set; }
    }

    #endregion

    #region [拼团产品信息Model]

    /// <summary>
    /// 拼团产品详情Model
    /// </summary>
    public class GroupBuyingProductModel
    {
        public string PID { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// 官方售价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 活动价
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// 团长特价
        /// </summary>
        public decimal SpecialPrice { get; set; }

        /// <summary>
        /// 产品是否可用优惠券
        /// </summary>
        public bool UseCoupon { get; set; }

        /// <summary>
        /// 每单产品最大数量
        /// </summary>
        public int UpperLimitPerOrder { get; set; }
    }

    /// <summary>
    /// 首页显示产品组信息Model
    /// </summary>
    public class ProductGroupModel : GroupBuyingProductModel
    {
        #region 产品组属性

        /// <summary>
        /// 产品组Id
        /// </summary>
        public string ProductGroupId { get; set; }

        /// <summary>
        /// 列表页显示图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 分享id
        /// </summary>
        public string ShareId { get; set; }

        /// <summary>
        /// 团类型
        /// </summary>
        public int GroupType { get; set; }

        /// <summary>
        /// 成团所需成员数量
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// 首页显示顺序
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 当前拼团数量
        /// </summary>
        public int CurrentGroupCount { get; set; }

        /// <summary>
        /// 配置库存数量
        /// </summary>
        public int TotalGroupCount { get; set; }

        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        public List<string> LabelList { get; set; } = new List<string>();

        /// <summary>
        /// 对应的活动ID
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 该产品组的拼团用户列表
        /// </summary>
        public List<Guid> GroupUserList { get; set; } = new List<Guid>();

        /// <summary>
        /// 已团单数
        /// </summary>
        public int GroupOrderCount { get; set; }

        /// <summary>
        /// 列表页是否显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 拼团类别：0-普通拼团；1-抽奖拼团
        /// </summary>
        public int GroupCategory { get; set; }

        /// <summary>
        /// 拼团抽奖规则
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        /// 分享图片
        /// </summary>
        public string ShareImage { get; set; }

        #endregion
    }

    /// <summary>
    /// PID所在ProductGroupId列表（活动页配置信息查询）
    /// </summary>
    public class GroupBuyingProductInfo
    {
        /// <summary>
        /// 产品PID
        /// </summary>
        public string PId { get; set; }

        /// <summary>
        /// 该产品所在的ProductGroup信息
        /// </summary>
        public List<GroupBuyingProductItem> ProductGroupList { get; set; } = new List<GroupBuyingProductItem>();
    }

    /// <summary>
    /// 拼团类目信息
    /// </summary>
    public class GroupBuyingCategoryModel
    {
        public int NewCategoryCode { get; set; }
        public string NewCategoryName { get; set; }
        public int ProductCount { get; set; }
        public int SortBy { get; set; }
    }

    public class GroupBuyingProductItem
    {
        public string ProductGroupId { get; set; }
        public string Price { get; set; }
        public DateTime StartTime { get; set; }
    }


    public class SimplegroupBuyingModel
    {
        public string ProductGroupId { get; set; }
        public string PID { get; set; }
        public Guid ActivityId { get; set; }
        public bool IsDefault { get; set; }
    }


    #endregion

    #region 团信息

    /// <summary>
    /// 团信息
    /// </summary>
    public class GroupInfoModel
    {
        /// <summary>
        /// 团号
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 团长Id
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 对应的产品组Id
        /// </summary>
        public string ProductGroupId { get; set; }

        /// <summary>
        /// 团类型
        /// </summary>
        public int GroupType { get; set; }

        /// <summary>
        /// 当前团状态
        /// </summary>
        public int GroupStatus { get; set; }

        /// <summary>
        /// 拼团所需人数
        /// </summary>
        public int RequiredMemberCount { get; set; }

        /// <summary>
        /// 当前拼团人数
        /// </summary>
        public int CurrentMemberCount { get; set; }

        /// <summary>
        /// 拼团开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 拼团结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 拼团类别：0-普通拼团；1-抽奖拼团
        /// </summary>
        public int GroupCategory { get; set; }
    }

    /// <summary>
    /// 查询用户可参团列表Model
    /// </summary>
    public class GroupInfoResponse
    {
        /// <summary>
        /// 可参团数量
        /// </summary>
        public int TotalCount { get; set; }

        public List<GroupInfoModel> Items { get; set; }
    }

    #endregion

    #region [拼团用户信息]

    /// <summary>
    /// 查户拼团记录
    /// </summary>
    public class UserGroupBuyingInfoModel : ProductGroupModel
    {
        public Guid GroupId { get; set; }

        /// <summary>
        /// 用户订单号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 当前拼团人数
        /// </summary>
        public int CurrentMemberCount { get; set; }

        /// <summary>
        /// 拼团开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 团长Id
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 当前团状态
        /// </summary>
        public int GroupStatus { get; set; }

        /// <summary>
        /// 用户拼团状态
        /// </summary>
        public int UserStatus { get; set; }

        /// <summary>
        /// 用户抽奖结果:0-未开奖；1-一等奖；2-二等奖；~
        /// </summary>
        public int LotteryResult { get; set; }
    }

    /// <summary>
    /// 查询团用户
    /// </summary>
    public class GroupMemberModel
    {
        /// <summary>
        /// 团长Guid
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 团中所有用户的Guid（弃用）
        /// </summary>
        public List<Guid> Items { get; set; } = new List<Guid>();

        /// <summary>
        /// 当前团用户信息
        /// </summary>
        public List<UserOrderModel> UserItems { get; set; } = new List<UserOrderModel>();
    }

    /// <summary>
    /// 拼团成功的团成员信息
    /// </summary>
    public class GroupFinalUserModel
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// 拼团产品PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 拼团订单号
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 是否是团长
        /// </summary>
        public bool IsCaptain { get; set; }
        /// <summary>
        /// 优惠券发放结果（优惠券拼团）
        /// </summary>
        public bool CreateCouponResult { get; set; }
    }


    // 前期代码逻辑混乱添加多个字段重复Model，之后单个用户相关信息主要使用GroupMemberInfo

    /// <summary>
    /// 拼团成员信息
    /// </summary>
    public class GroupMemberInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 用户UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 产品PID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
    }

    /// <summary>
    /// 团成员信息
    /// </summary>
    public class UserOrderModel
    {
        /// <summary>
        /// 用户UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户订单号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 用户参团时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 是否团长
        /// </summary>
        public bool IsCaptain { get; set; }
    }
    /// <summary>
    /// 用户拼团记录统计
    /// </summary>
    public class GroupBuyingHistoryCount
    {
        /// <summary>
        /// 总拼团订单数量
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// 未付款订单数量
        /// </summary>
        public int UnpaidItemCount { get; set; }

        /// <summary>
        /// 拼团中订单数量
        /// </summary>
        public int UnderwayItemCount { get; set; }

        /// <summary>
        /// 拼团成功订单数量
        /// </summary>
        public int SuccessfulItemCount { get; set; }

        /// <summary>
        /// 拼团失败订单数量
        /// </summary>
        public int UnsuccessfulItemCount { get; set; }
    }

    public class UserOrderInfoModel
    {
        public string PID { get; set; }
        public string ProductName { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime StartTime { get; set; }
        public int OrderId { get; set; }
        public int UserStatus { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal SpecialPrice { get; set; }
        public int LotteryResult { get; set; }
        public string ShareImage { get; set; }
    }

    public class GroupBuyingLotteryInfo
    {
        public Guid UserId { get; set; }
        public int OrderId { get; set; }

        /// <summary>
        /// 中奖等级
        /// </summary>
        public int Level { get; set; }
    }
    public class GroupBuyingBuyLimitModel
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// 拼团产品组Id
        /// </summary>
        public string ProductGroupId { get; set; }
        /// <summary>
        /// 产品PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 改拼团产品限购数量（0-不限）
        /// </summary>
        public int BuyLimitCount { get; set; }
        /// <summary>
        /// 当前订单数量
        /// </summary>
        public int CurrentOrderCount { get; set; }
    }


    public class BuyLimitAndOrderLimitModel : GroupBuyingBuyLimitModel
    {
        public int UpperLimitPerOrder { get; set; }
    }
    #endregion

    #region [其他]

    public class GroupBuyingUserStatusCount
    {
        public int UserStatus { get; set; }
        public int Count { get; set; }
    }

    public class GroupBuyingUserModel
    {
        public string ProductGroupId { get; set; }
        public Guid UserId { get; set; }
    }

    public class GroupBuyingUserCount
    {
        public string ProductGroupId { get; set; }
        public int Count { get; set; }
    }

    public class NewUserCheckResultModel
    {
        public bool CheckResult { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class CheckResultModel
    {
        public int Code { get; set; }
        public int? OrderId { get; set; }
        public string Info { get; set; }
    }

    /// <summary>
    /// 用户抽奖规则Model
    /// </summary>
    public class GroupLotteryRuleModel
    {
        public string RuleDescription { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }


    /// <summary>
    /// 免单券信息
    /// </summary>
    public class FreeCouponModel
    {
        /// <summary>
        /// 免单券来源
        /// </summary>
        public int PreOrderId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }


    public class VerificationResultModel
    {
        /// <summary>
        /// 返回码 0-数据库操作失败；1-成功；2~
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 相关信息
        /// </summary>
        public string Info { get; set; }
    }

    #endregion

    #region [状态枚举]

    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatusEnum
    {
        待付款 = 0,
        已付款 = 1,
        拼团成功 = 2,
        拼团失败 = 3,
        拼团取消 = 4
    }

    /// <summary>
    /// 拼团状态
    /// </summary>
    public enum GroupStatusEnum
    {
        团长下单未付款 = 0,
        拼团中 = 1,
        拼团成功 = 2,
        拼团失败 = 3
    }

    /// <summary>
    /// 拼团类型
    /// </summary>
    public enum GroupTypeEnum
    {
        普通团 = 0,
        新人团 = 1,
        团长特价 = 2,
        团长免单 = 3
    }

    /// <summary>
    /// 拼团类别
    /// </summary>
    public enum GroupCategoryEnum
    {
        普通拼团 = 0,
        拼团抽奖 = 1,
        优惠券拼团 = 2
    }

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum CodeInfoEnum
    {
        数据库查询出错 = 0,
        调用成功或允许参加 = 1,
        商品已抢完 = 2,
        商品已下架 = 3,
        未查询到该产品 = 4,
        用户已参团 = 5,
        该团为新人团用户没有资格加入 = 6,
        拼团已过期 = 7,
        拼团已完成 = 8,
        微信支付账号已经购买过 = 9,
        只有特定用户可见 = 10,
        已达到限购单数 = 11,
        产品库已下架 = 12
    }

    /// <summary>
    /// 用户拼团记录分类
    /// </summary>
    public enum UserHistoryTypeEnum
    {
        全部 = 0,
        拼团中 = 1,
        已完成拼团 = 2,
        拼团失败 = 3,
        待付款 = 5
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class BigBrandRewardListModel
    {

        /*
CREATE TABLE [dbo].[BigBrandRewardList]
(
[PKID] [int] NOT NULL IDENTITY(1, 1),
[HashKeyValue] [varchar] (10) COLLATE Chinese_PRC_CI_AS NULL,
[CreateDateTime] [datetime] NOT NULL,
[LastUpdateDateTime] [datetime] NOT NULL,
[Title] [varchar] (100) COLLATE Chinese_PRC_CI_AS NULL,
[PreTimes] [int] NULL,
[CompletedTimes] [int] NULL,
[NeedIntegral] [int] NULL,
[BigBrandType] [int] NOT NULL,
[Period] [int] NOT NULL,
[PeriodType] [int] NOT NULL,
[CreateUserName] [varchar] (100) COLLATE Chinese_PRC_CI_AS NULL,
[UpdateUserName] [varchar] (100) COLLATE Chinese_PRC_CI_AS NULL
) 
         
            CREATE TABLE Configuration.dbo.BigBrandRewardList
    (
      PKID INT PRIMARY KEY
               IDENTITY(1, 1) ,
      HashKeyValue VARCHAR(10) ,--对外使用的大翻盘活动ID
      CreateDateTime DATETIME NOT NULL ,
      LastUpdateDateTime DATETIME NOT NULL ,
      Title VARCHAR(100) ,--大翻盘抽奖规则标题
      PreTimes INT ,--分享前次数
      CompletedTimes INT ,--分享后次数（如果是积分抽奖不需要加次数）
      NeedIntegral INT ,--抽奖需要的积分
	  BigBrandType INT NOT NULL, --抽奖类型 1：普通抽奖 2：积分抽奖 3：人群抽奖
	  Period INT NOT NULL,--周期数
	  PeriodType INT NOT NULL,--周期类型 1：小时 2：日 3： 天 （从创建日期开始）
      CreateUserName VARCHAR(100) ,--创建人
      UpdateUserName VARCHAR(100)--更新人
    );
             
             */

        public int PKID { get; set; }

        /// <summary>
        /// 对外使用的大翻盘活动ID
        /// </summary>
        public string HashKeyValue { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 大翻盘抽奖规则标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 分享前次数
        /// </summary>
        public int? PreTimes { get; set; }

        /// <summary>
        /// 分享后次数
        /// </summary>
        public int? CompletedTimes { get; set; }

        /// <summary>
        /// 抽奖需要的积分
        /// </summary>
        public int? NeedIntegral { get; set; }

        /// <summary>
        /// 抽奖类型 1：普通抽奖 2：积分抽奖 3：人群抽奖
        /// </summary>
        public int BigBrandType { get; set; }

        /// <summary>
        /// 周期数
        /// </summary>
        public int? Period { get; set; }

        /// <summary>
        /// 周期类型 1：小时 2：日 3： 天 （从创建日期开始）
        /// </summary>
        public int  PeriodType { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUserName { get; set; }

        /// <summary>
        /// 开始时间[配置的时间]
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 周期开始时间 【重新计算后的起始时间】
        /// </summary>
        public DateTime PeriodStartDateTime { get; set; }


        /// <summary>
        /// 周期结束时间【重新计算后的结束时间】
        /// </summary>
        public DateTime PeriodEndDateTime { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }

        /// <summary>
        /// 奖励信息集合
        /// </summary>
        public List<BigBrandRewardPoolModel> ItemPools { get; set; }

        /// <summary>
        /// 页面样式集合
        /// </summary>
        public List<BigBrandPageStyleModel> ItemStyles { get; set; }

        /// <summary>
        /// 轮次奖励池集合
        /// </summary>
        public List<BigBrandWheelModel> ItemTimes { get; set; }

        /// <summary>
        /// 问答抽奖配置
        /// </summary>
        public BigBrandAnsQuesModel AnsQuesConfig { get; set; }
        public int? AfterLoginType { get; set; }
        public string AfterLoginValue { get; set; }
        /// <summary>
        /// 活动页面 配置
        /// </summary>
        public BigBrandPageConfigModel PageConfig { get; set; }

        /// <summary>
        /// 大翻牌页面 配置
        /// </summary>
        public BigBrandPageConfigBrand BigBrandPageConfig { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class BigBrandRewardPoolModel
    {
        /*

          PKID INT PRIMARY KEY
               IDENTITY(1, 1) ,
      FKPKID INT
        FOREIGN KEY REFERENCES Configuration.dbo.BigBrandRewardList ( PKID ) ,--大翻盘关联的外键
	  ParentPKID INT ,--父级PKID 第一级奖励池 第二级奖励包 第三级优惠券集合或者积分或者空奖励
	  Name VARCHAR(30),--奖励池名称 
      CreateDateTime DATETIME NOT NULL ,
      LastUpdateDateTime DATETIME NOT NULL ,
      RewardType INT NOT NULL ,--奖励类型 1：优惠券 2：积分 3：无奖励
      CouponGuid UNIQUEIDENTIFIER ,--优惠券GUID
      Integral INT CHECK ( Integral > 0 ) ,--积分
	  PromptType int ,--提示类型 1：弹窗 2：链接
      PromptMsg VARCHAR(100)  ,--抽中提示信息
      PromptImg VARCHAR(1000) ,--抽中提示图片
      RedirectH5 VARCHAR(300) ,--移动站跳转链接
      RedirectAPP VARCHAR(300) ,--APP跳转链接
      RedirectWXAPP VARCHAR(300) ,--小程序跳转链接
      RedirectBtnText VARCHAR(300) ,--跳转按钮的文案 
      Number INT ,--抽奖的数量
      IsDefault bit , --是否为奖池的默认抽奖项
      CreateUserName VARCHAR(100) ,--创建人
      UpdateUserName VARCHAR(100),--更新人
	   [Status] BIT --状态，如果删除为false
         */

        public int PKID { get; set; }

        public int FKPKID { get; set; }

        public int? ParentPKID { get; set; }

        /// <summary>
        /// 奖励池名称
        /// </summary>
        public string Name { get; set; }


        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 奖励类型 1：优惠券 2：积分 3：无奖励,4:实物奖励，5.微信红包
        /// </summary>
        public int RewardType { get; set; }

        /// <summary>
        /// 优惠券GUID
        /// </summary>

        public string CouponGuid { get; set; }

        /// <summary>
        /// 实物奖励名称
        /// </summary>
        public string RealProductName { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int? Integral { get; set; }

        /// <summary>
        /// 提示类型 1：弹窗 2：链接
        /// </summary>
        public int PromptType { get; set; }

        /// <summary>
        /// 抽中提示信息
        /// </summary>
        public string PromptMsg { get; set; }

        /// <summary>
        /// 抽中提示图片
        /// </summary>
        public string PromptImg { get; set; }

        /// <summary>
        /// 移动站跳转链接
        /// </summary>
        public string RedirectH5 { get; set; }

        /// <summary>
        /// APP跳转链接
        /// </summary>
        public string RedirectAPP { get; set; }

        /// <summary>
        /// 小程序跳转链接
        /// </summary>
        public string RedirectWXAPP { get; set; }
        /// <summary>
        /// 华为轻应用跳转地址
        /// </summary>
        public string RedirectHuaWei { get; set; }
        /// <summary>
        /// 跳转按钮的文案
        /// </summary>
        public string RedirectBtnText { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }

        /// <summary>
        /// 抽奖的数量
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// 是否为奖池的默认抽奖项
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUserName { get; set; }

        /// <summary>
        /// 有效状态
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 子集合
        /// </summary>
        public List<BigBrandRewardPoolModel> PartItem { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime? DateTimeLog { get; set; }

        public string PromotionCodePKIDs { get; set; }
        /// <summary>
        /// 微信红包金额
        /// </summary>
        public decimal? WxRedBagAmount { get; set; }

    }

    /// <summary>
    /// 抽奖轮数对应的奖池信息
    /// </summary>
    public class BigBrandWheelModel
    {
        /*
PKID INT IDENTITY(1,1) PRIMARY KEY,
FKBigBrand INT NOT NULL,--大翻牌规则主键
TimeNumber INT NOT NULL,
FKPoolPKID INT NOT NULL  ,--奖池PKID
CreateDateTime DATETIME NOT NULL ,
LastUpdateDateTime DATETIME NOT NULL ,
CreateUserName VARCHAR(100) ,--创建人
UpdateUserName VARCHAR(100)--更新人
         */

        
        /// <summary>
        /// 
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 大翻牌规则主键
        /// </summary>
        public int FKBigBrand { get; set; }

        /// <summary>
        /// 抽奖的轮次
        /// </summary>
        public int TimeNumber { get; set; }

        /// <summary>
        /// 奖励池主键
        /// </summary>
        public int FKPoolPKID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }


    public class BigBrandPageStyleModel
    {
        /*
PKID INT IDENTITY(1,1) PRIMARY KEY,
FKPKID INT  FOREIGN KEY REFERENCES Configuration.dbo.BigBrandRewardList(PKID) NOT NULL,--大翻牌列表的主键
CreateDateTime DATETIME NOT NULL ,
LastUpdateDateTime DATETIME NOT NULL ,
PageType INT ,--页面类型
IsShare BIT ,--全局分享按钮
Position INT ,--位置 1:左上 2:上 3:右上 4：左 5：右 6：左下 7：下 8：右下 
ImageUri VARCHAR(1000),--图片链接
CreateUserName VARCHAR(100) ,--创建人
UpdateUserName VARCHAR(100),--更新人
GroupGuid  UNIQUEIDENTIFIER --组号
         */

        public int PKID { get; set; }

        public int FKPKID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public int PageType { get; set; }

        /// <summary>
        /// 全局分享按钮
        /// </summary>
        public bool IsShare { get; set; }

        /// <summary>
        /// 位置 1:左上 2:上 3:右上 4：左 5：右 6：左下 7：下 8：右下 
        /// </summary>
        public int Position { get; set; }

        public string ImageUri { get; set; }

        /// <summary>
        /// 翻面图片
        /// </summary>
        public string BImageUri { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }

        /// <summary>
        /// 组号，相同组号的是一个页面的样式配置
        /// </summary>
        public Guid GroupGuid { get; set; }
        /// <summary>
        /// 领取成功弹窗样式
        /// </summary>
        public int? PromptStyle { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }

    /// <summary>
    /// 抽奖结果
    /// </summary>
    public class BigBrandResponse
    {
        /// <summary>
        /// 返回状态  可从枚举【BigBrandStateEnum】中获得 
        /// </summary>
        public int State { get; set; }

        public string Msg { get; set; }

        public int Code { get; set; }

        public string PromptImg { get; set; }

        public string PromptMsg { get; set; }

        public int PromptType { get; set; }

        public string RedirectAPP { get; set; }

        public string RedirectBtnText { get; set; }

        public string RedirectH5 { get; set; }

        public string RedirectWXAPP { get; set; }
        public string RedirectHuaWei { get; set; }
        public int RewardType { get; set; }

        public bool IsShare { get; set; }

        public int? TimeCount { get; set; }

        public int? ShareTimes { get; set; }

        public int Time { get; set; }

        /// <summary>
        /// 获取是否兜底奖池
        /// </summary>
        public bool? DefaultPool { get; set; }

        /// <summary>
        /// 实物奖励标记
        /// </summary>
        public Guid? RealTip { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }

        /// <summary>
        /// 指定hashkey hashKey == "23338FF5" || hashKey == "870F1F2E"
        /// </summary>
        public IEnumerable<CouponRule> CouponRuleItems { get; set; }

        /// <summary>
        /// 不区分hashkey 返回领券结果
        /// </summary>
        public List<CouponRule> CouponRules { get; set; }

        /// <summary>
        /// 总得分
        /// </summary>
        public int RealValue { get; set; }

        /// <summary>
        /// 错误分数
        /// </summary>
        public int ErrorValue { get; set; }




    }

    /// <summary>
    /// 判断是否可以抽奖
    /// </summary>
    public class BigBrandCanResponse
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public int? Times { get; set; }

        public bool? IsShare { get; set; }

        public string Item { get; set; }

        public bool? DefaultPool { get; set; }

        public int? ShareTimes { get; set; }

        public IEnumerable<CouponRule> CouponRuleItems { get; set; }

    }

    public class CouponRule
    {
       
        public string Description { get; set; }

        //     优惠券名称///
        public string PromotionName { get; set; }

        //     优惠金额///
        public decimal Discount { get; set; }

        //     满足最低价///
        public decimal MinMoney { get; set; }

        //     开始日期///
        public DateTime? ValiStartDate { get; set; }

        //     截止日期///
        public DateTime? ValiEndDate { get; set; }

        //     最终有效期
        public DateTime? DeadLineDate { get; set; }

        /// <summary>
        /// 多少天后有效
        /// </summary>
        public int? Term { get; set; }

        /// <summary>
        /// 优惠券的创建日期
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 多少天有效
        /// </summary>
        public int DateNumber { get; set; }

        public int GetDateNumber()
        {
            if (Term == null)
            {
                if (ValiEndDate != null)
                {
                    //开始时间 结束时间
                    if (Convert.ToDateTime( DateTime.Now.ToString("yyyy-MM-dd 00:00:00")).CompareTo(ValiEndDate.Value )> 0)
                        return -1;
                    else
                    {
                        TimeSpan ts = ValiEndDate.Value.Subtract(DateTime.Now);
                        return ts.Days + 1;
                    }
                }
                else
                    return 0;
            }
            else
            {
                if (DeadLineDate != null)
                {
                    //领取后多少天 加最后领取日期 
                    if (CreateDateTime.AddDays(Term.Value).CompareTo(Convert.ToDateTime( DateTime.Now.ToString("yyyy-MM-dd 00:00:00"))) >= 0)
                    {
                        if (CreateDateTime.AddDays(Term.Value).CompareTo(DeadLineDate) < 0)
                        {
                            //过期日期包含在最后有效期内
                            TimeSpan ts = CreateDateTime.AddDays(Term.Value).Subtract(DateTime.Now);
                            return ts.Days + 1;
                        }
                        else
                        {
                            //过期日期按最后有效期
                            TimeSpan ts = DeadLineDate.Value.Subtract(CreateDateTime);
                            if (ts.Days < 0)
                                return -1;
                            return ts.Days + 1;
                        }
                       
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    TimeSpan ts = CreateDateTime.AddDays(Term.Value).Subtract(DateTime.Now);
                    if (ts.Days < 0)
                        return -1;
                    return ts.Days + 1;
                }
            }
        }
        

    }

    /// <summary>
    /// 活动页配置
    /// </summary>
    public class BigBrandPageConfigModel
    {

        public int PKID { get; set; }

        /// <summary>
        /// bigbrandID
        /// </summary>
        public int FKPKID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }


        /// <summary>
        /// 全局分享按钮
        /// </summary>
        public bool IsShare { get; set; }

        /// <summary>
        /// 活动 类型 0- 摇奖机
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        /// 背景图
        /// </summary>
        public string HomeBgImgUri { get; set; }
        /// <summary>
        /// 背景图
        /// </summary>
        public string HomeBgImgUri2 { get; set; }
        /// <summary>
        /// 结果页面图片
        /// </summary>
        public string ResultImgUri { get; set; }
        /// <summary>
        /// 摇奖机图片
        /// </summary>
        public string DrawMachineImgUri { get; set; }
        /// <summary>
        /// 跑马灯 是否开启
        /// </summary>
        public int MarqueeLampIsOn { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }

    /// <summary>
    /// 大翻牌页面配置
    /// </summary>
    public class BigBrandPageConfigBrand
    {
        /// <summary>
        /// 大翻牌背景图
        /// </summary>
        public string BigBrandBackImgUrl { get; set; }

        /// <summary>
        /// 大翻牌中心按钮图
        /// </summary>
        public string BigBrandCenterBtnImgUrl { get; set; }

        /// <summary>
        /// 大翻牌播报框外框图
        /// </summary>
        public string BroadCastOutImgUrl { get; set; }

        /// <summary>
        /// 大翻牌播报框内框图
        /// </summary>
        public string BroadCastInnerImgUrl { get; set; }
    }
}

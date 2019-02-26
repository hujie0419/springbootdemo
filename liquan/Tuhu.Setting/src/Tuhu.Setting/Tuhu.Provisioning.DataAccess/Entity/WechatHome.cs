using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{

    public class HomePageModuleType
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }

    public class HomePageConfiguation
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string KeyValue { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
   public class WechatHomeList
    {
        public int ID { get; set; }

        public string TypeName { get; set; }        
        public string Title { get; set; }

        public int IsEnabled { get; set; }

        public string SVersion { get; set; }


        public string EVersion { get; set; }

        public DateTime CDateTime { get; set; }


        public DateTime UDateTime { get; set; }

        public int? OrderBy { get; set; }

        /// <summary>
        /// 是否新人
        /// </summary>
        public bool? IsNewUser { get; set; }

        public bool? IsShownButtom { get; set; }

        public int HomePageConfigID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Headings { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        public string ImageUrl { get; set; }

        public string Uri { get; set; }

      
    }


    public class WechatHomeContent
    {
        public int ID { get; set; }

        public string AppID { get; set; }

        public int FKID { get; set; }

        public string Title { get; set; }


        public string ImageUrl { get; set; }

        public string Uri { get; set; }


        public int IsEnabled { get; set; }

        public DateTime CDateTime { get; set; }

        public DateTime UDateTime { get; set; }

        public int OrderBy { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Headings { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 跳转类型
        /// </summary>
        public bool? UriType { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string PathImage { get; set; }
    
        public string BuriedPointParam { get; set; }

        public string UriTypeText { get; set; }
    }

    public class WechatHomeAreaContent
    {
        public int ID { get; set; }

        public string AppID { get; set; }

        public int FKID { get; set; }

    
        public string CityList { get; set; }

        public string CityIDs { get; set; }

        public string ImageUrl { get; set; }

        public string Uri { get; set; }


        public int IsEnabled { get; set; }

        public DateTime CDateTime { get; set; }

        public DateTime UDateTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Headings { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }


        public string UriTypeText { get; set; }
    }


    public class WechatHomeProductContent
    {
        public int ID { get; set; }

        

        public int FKID { get; set; }

        public string PID { get; set; }

        public string GroupId { get; set; }

        public string ProductName { get; set; }


        
        public string ImageUrl { get; set; }


        public int OrderBy { get; set; }

        public int IsEnabled { get; set; }

        public DateTime CDateTime { get; set; }

        public DateTime UDateTime { get; set; }

        public string BuriedPointParam { get; set; }

        public int IsTimeSet { get; set; } = 0;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; } 
    }

    /// <summary>
    /// 公众号领红包设置表
    /// </summary>
    public class WXOARedEnvelopeSettingModel
    {

        /// <summary>
		/// 主键
		/// </summary>
		public long PKID { get; set; }

        /// <summary>
        /// 条件 - 价格
        /// </summary>
        public decimal ConditionPrice { get; set; }

        /// <summary>
        /// 条件 - 价格 - FLAG
        /// </summary>
        public int ConditionPriceFlag { get; set; }

        /// <summary>
        /// 条件 - 车型 - FLAG
        /// </summary>
        public int ConditionCarModelFlag { get; set; }

        /// <summary>
        /// 一天上限金额
        /// </summary>
        public decimal DayMaxMoney { get; set; }

        /// <summary>
        /// 每人平均领取红包金额（元）
        /// </summary>
        public decimal AvgMoney { get; set; }

        /// <summary>
        /// 活动规则
        /// </summary>
        public string ActivityRuleText { get; set; }

        /// <summary>
        /// 未获得红包提示
        /// </summary>
        public string FailTipText { get; set; }

        /// <summary>
        /// 长按识别二维码 URL
        /// </summary>
        public string QRCodeUrl { get; set; }

        /// <summary>
        /// 识别二维码提示文字 
        /// </summary>
        public string QRCodeTipText { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        public string ShareTitleText { get; set; }

        /// <summary>
        /// 分享地址
        /// </summary>
        public string ShareUrl { get; set; }

        /// <summary>
        /// 分享图片的地址
        /// </summary>
        public string SharePictureUrl { get; set; }

        /// <summary>
        /// 分享的内容
        /// </summary>
        public string ShareText { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 公众号类型 1 途虎主号
        /// </summary>
        public int OfficialAccountType { get; set; }

        /// <summary>
        /// 这个日期之前的OpenId不生效 字段可空 空字段就不生效
        /// </summary>
        public DateTime OpenIdLegalDate { get; set; }

        /// <summary>
        /// 领红包活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 领红包活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

    }

    /// <summary>
	/// 公众号领红包每日统计表
	/// </summary>
    public class WXOARedEnvelopeStatisticsModel
    {
        /// <summary>
		/// 主键
		/// </summary>
		public long PKID { get; set; }

        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatisticsDate { get; set; }

        /// <summary>
        /// 当天的红包上限
        /// </summary>
        public decimal DayMaxMoney { get; set; }

        /// <summary>
        /// 用户数量 参与人数
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 已发放红包数
        /// </summary>
        public int RedEnvelopeCount { get; set; }

        /// <summary>
        /// 已发放红包金额（元）
        /// </summary>
        public decimal RedEnvelopeSumMoney { get; set; }

        /// <summary>
        /// 红包平均金额
        /// </summary>
        public decimal RedEnvelopeAvg { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Data;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.YLHCouponInit.Model
{
    public class YLHResidueProject
    {
        [Column("Display_Card_NBR")]
        public string DisplayCardNBR { get; set; }
        public int RemainCount { get; set; }
        public string EffectiveDate { get; set; }
        public double DefaultPrice { get; set; }
        public string ProjectName { get; set; }
    }

    public class PromotionCodeModel : BaseModel
    {
        public int Number { set; get; }
        public string EndDateTime { set; get; }
        /// <summary>
        /// 主键编号
        /// </summary>
        public int Pkid { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 优惠券券号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 创建优惠券时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 优惠券面值
        /// </summary>
        public decimal? MinMoney { get; set; }
        /// <summary>
        /// 优惠券有效期（开始时间）
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 优惠券有效期（结束时间）
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 优惠券使用状态 0:未使用  1:已使用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 优惠券类别
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 优惠券使用时间
        /// </summary>
        public DateTime? UsedTime { get; set; }
        public int Discount { get; set; }
        public int OrderId { get; set; }
        public int? Type { get; set; }
        public string UserPhone { get; set; }
        /// <summary>优惠券生成渠道 </summary>
        public string CodeChannel { get; set; }
        /// <summary>优惠券批次号 </summary>
        public int? BatchId { get; set; }
        public int? RuleId { get; set; }
        public int? GetRuleId { get; set; }
    }

    public class UserObjectModel : BaseModel
    {
        /// <summary>用户编号</summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户类型（0：访客用户；1：注册用户;2：编辑用户；3：管理员；10：员工）
        /// ((int) user.UserTag).ToString()
        /// </summary>
        [Column("u_user_type")]
        public string UserType { get; set; }

        /// <summary>
        /// 姓名
        /// user.Profile.UserName
        /// </summary>
        [Column("u_last_name")]
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// user.Email
        /// </summary>
        [Column("u_email_address")]
        public string Email { get; set; }


        /// <summary>
        /// Category, tuhu,taobao...
        /// </summary>
        public string Category { get; set; }

        /// <summary> 
        /// 密码
        /// user.Password
        /// </summary>
        [Column("u_user_security_password")]
        public string Password { get; set; }

        /// <summary> TaoBaoId</summary>
        [Column("u_yahoo")]
        public string TaobaoId { get; set; }

        /// <summary>
        /// 手机
        /// user.MobileNumber
        /// </summary>
        [Column("u_mobile_number")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// 性别
        /// user.Profile.Gender
        /// </summary>
        [Column("i_gender")]
        public int? Gender { get; set; }

        /// <summary>
        /// 生日
        /// user.Profile.Birthday
        /// </summary>
        [Column("dt_birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 购买意向
        /// user.CRMInfo.BuyIntent
        /// </summary>
        [Column("u_Pref2")]
        public string Pref2 { get; set; }

        /// <summary>
        /// 从哪里知道我们
        /// user.CRMInfo.KnownPlace
        /// </summary>
        [Column("u_Pref3")]
        public string Pref3 { get; set; }

        /// <summary>
        /// 您的地区
        /// user.CRMInfo.Region
        /// </summary>
        [Column("u_Pref4")]
        public string Pref4 { get; set; }

        /// <summary>
        /// 昵称账户
        /// user.Profile.NickName
        /// </summary>
        [Column("u_Pref5")]
        public string Pref5 { get; set; }

        /// <summary>
        /// 电话
        /// user.CRMInfo.Telephone
        /// </summary>
        [Column("u_tel_number")]
        public string Telephone { get; set; }

        /// <summary>
        /// 分机号
        /// user.CRMInfo.TelEx
        /// </summary>
        [Column("u_tel_extension")]
        public string TelExtension { get; set; }

        /// <summary>
        /// 传真
        /// user.CRMInfo.Fax
        /// </summary>
        [Column("u_fax_number")]
        public string Fax { get; set; }

        /// <summary>
        /// 传真分机号
        /// user.CRMInfo.FaxEx
        /// </summary>
        [Column("u_fax_extension")]
        public string FaxExtension { get; set; }

        /// <summary>
        /// 渠道
        /// user.CRMInfo.KnownChannel
        /// </summary>
        [Column("u_know_channel")]
        public string KnownChannel { get; set; }

        /// <summary>
        /// 注册时间
        /// SecurityLog
        /// </summary>
        [Column("dt_date_registered")]
        public DateTime? RegisteredDateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// user.UpdatedTime
        /// </summary>
        [Column("dt_date_last_changed")]
        public DateTime UpdatedTime { get; set; }

        /// <summary>
        /// 创建时间
        /// user.CreatedTime
        /// </summary>
        [Column("dt_date_created")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 第一次登录app时间
        /// SecurityLog
        /// </summary>
        [Column("dt_date_first_time_login_app")]
        public DateTime? FirstLoginAppTime { get; set; }

        /// <summary>
        /// 密码变更时间
        /// SecurityLog
        /// </summary>
        [Column("dt_date_last_password_changed")]
        public DateTime? PasswordChangedTime { get; set; }

        /// <summary>
        /// 黑名单截至日期
        /// user.CRMInfo.BlackListEndTime
        /// </summary>
        [Column("u_date_BlackListEndTime")]
        public DateTime? BlackListEndTime { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        [Column("dt_last_lockedout_date")]
        public DateTime? LockedDateTime { get; set; }

        /// <summary>
        /// 头像
        /// user.Profile.HeadUrl
        /// </summary>
        [Column("u_Imagefile")]
        public string HeadImage { set; get; }

        /// <summary>
        /// 个性签名
        /// user.Profile.PersonalSign
        /// </summary>
        [Column("u_person_sign")]
        public string PersonalSign { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [JsonIgnore, IgnoreDataMember]
        public string Nickname
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Pref5))
                    return Pref5;
                if (!string.IsNullOrWhiteSpace(UserName))
                    return UserName;
                if (!string.IsNullOrWhiteSpace(TaobaoId))
                    return TaobaoId;
                return MobileNumber;
            }
        }

        /// <summary>
        /// 微信公众号open id
        /// user.Auths[AuthChannelEnum.WeiXinAppOpen].OpenId
        /// </summary>
        [Column("u_openid_weixin")]
        public string WeiXinOpenId { get; set; }
        /// <summary>
        /// 微信小程序open id
        /// user.Auths[AuthChannelEnum.WeiXinAppOpen].OpenId
        /// </summary>
        [Column("u_openid_wxapp")]
        public string WXAPPOpenId { get; set; }
        /// <summary>
        /// 微信App open id
        /// </summary>
        [Column("u_openid_appweixin")]
        public string AppWeiXinOpenId { get; set; }

        [Column("u_qq")]
        public string QqAccount { get; set; }

        /// <summary>
        /// QQ open id
        /// </summary>
        [Column("u_openid_qq")]
        public string QqOpenId { get; set; }

        /// <summary>
        /// QQ App open id
        /// </summary>
        [Column("u_app_openid_qq")]
        public string QqAppOpenId { get; set; }

        /// <summary>
        /// 手机号是否被验证过
        /// user.IsMobileVerified
        /// </summary>
        public bool? IsMobileVerify { get; set; }

        /// <summary>
        /// 总记录条数，分页使用
        /// </summary>
        public int TotalCount { get; set; }

        protected override void Parse(DataRow row, PropertyInfo[] properties)
        {
            base.Parse(row, properties);

            RegisteredDateTime = Convert.ToDateTime(row.HasValue("dt_date_registered")
                ? row["dt_date_registered"] : row["dt_date_created"]);
        }
    }

    public class CouponRuleModel
    {
        public int PKID { get; set; }

        /// <summary>优惠券规则Id/// </summary>
		public int RuleID { get; set; }

        public string Description { get; set; }

        /// <summary>优惠券名称/// </summary>
        public string PromotionName { get; set; }

        /// <summary>优惠金额/// </summary>
		public decimal Discount { get; set; }

        /// <summary>满足最低价/// </summary>
        public decimal MinMoney { get; set; }

        public int AllowChanel { get; set; }

        public int? Term { get; set; }

        /// <summary>开始日期/// </summary>
		public DateTime? ValiStartDate { get; set; }

        /// <summary>截止日期/// </summary>
        public DateTime? ValiEndDate { get; set; }

        public Guid GetRuleGUID { get; set; }
    }

    public class CouponDiffModel
    {
        public int PKID { get; set; }
        [Column("u_user_id")]
        public string UserID { get; set; }
        public string MemberNumber { get; set; }
        public string ProjectName { get; set; }
        public int? RuleID { get; set; }
        public int Diff { get; set; }
    }

    public class PromotionCodeSimplifyModel
    {
        public int PKID { get; set; }
        public string code { get; set; }
    }
}

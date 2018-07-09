using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data;

namespace Tuhu.C.Job.UserAccountCombine.Model
{
    public class YewuThirdpartyInfo
    {
        public int PKID { get; set; }
        /// <summary>
        /// 业务表用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 第三方ID
        /// </summary>
        public string ThirdpartyId { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }

    public class NeedCombineUserId
    {
        public int PKID { get; set; }
        /// <summary>
        /// 需要被合并的源UserId
        /// </summary>
        public Guid SourceUserId { get; set; }
        /// <summary>
        /// 合并后的主UserId
        /// </summary>
        public Guid TargetUserId { get; set; }
        /// <summary>
        /// 合并操作结果
        /// </summary>
        public bool IsOperateSuccess { get; set; }

        public string Channel { get; set; }
        public string ThirdpartyId { get; set; }
        public DateTime CreateDataTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class RelatedTableCombineLog
    {
        /// <summary>
        /// 需要被合并的源UserId
        /// </summary>
        public Guid SourceUserId { get; set; }
        /// <summary>
        /// 合并后的主UserId
        /// </summary>
        public Guid TargetUserId { get; set; }
        /// <summary>
        /// 执行更新操作的表名
        /// </summary>
        public string RelatedTableName { get; set; }
        /// <summary>
        /// 执行更新操作的表主键，方便找到更新行
        /// 格式是pk={}
        /// </summary>
        public string RelatedTablePK { get; set; }
        /// <summary>
        /// 被更新的字段名字
        /// </summary>
        public string UpdatedParameter { get; set; }
        /// <summary>
        /// 执行操作类型
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 被更新字段原来的值
        /// </summary>
        public string SourceValue { get; set; }
        /// <summary>
        /// 被更新字段后的值
        /// </summary>
        public string TargetValue { get; set; }
        public DateTime CreateDataTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class RelatedTableCombineFailLog
    {
        /// <summary>
        /// 需要被合并的源UserId
        /// </summary>
        public Guid SourceUserId { get; set; }
        /// <summary>
        /// 合并后的主UserId
        /// </summary>
        public Guid TargetUserId { get; set; }
        /// <summary>
        /// 执行更新操作的表名
        /// </summary>
        public string RelatedTableName { get; set; }
        /// <summary>
        /// 执行更新操作的表主键，方便找到更新行
        /// 格式是pk={}
        /// </summary>
        public string RelatedTablePK { get; set; }
        /// <summary>
        /// 被更新的字段名字
        /// </summary>
        public string UpdatedParameter { get; set; }
        /// <summary>
        /// 执行操作类型
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 被更新字段原来的值
        /// </summary>
        public string SourceValue { get; set; }
        /// <summary>
        /// 被更新字段后的值
        /// </summary>
        public string TargetValue { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string FailReason { get; set; }
        public DateTime CreateDataTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class Addresses
    {
        [Column("u_address_id")]
        public string AddreesId { get; set; }

        public Guid? UserId { get; set; }

        public bool? IsDefaultAddress { get; set; }
    }

    public class CarObject
    {
        [Column("u_car_id")]
        public string CarId { get; set; }
        [Column("u_user_id")]
        public string UserId { get; set; }
    }

    public class Order
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class YLHUserInfo
    {
        public int PKID { get; set; }
        [Column("u_user_id")]
        public string UserId { get; set; }
    }

    public class YLHUserVipCardInfo
    {
        public int PKID { get; set; }
        [Column("u_user_id")]
        public string UserId { get; set; }
    }

    public class ShopReserve
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class ShopReceiveCheckThird
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class ShopReceiveCheckSecond
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class ShopReceiveCheckFirst
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class ShopReceiveOrder
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class ShopReceive
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class ShopReceiveNew
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }    

    public class EndUserCase
    {
        public int PKID { get; set; }
        public Guid? UserID { get; set; }
    }

    public class CRMRequisition
    {
        public int ID { get; set; }
        public string EndUserGuid { get; set; }
    }

    public class CRMAppointment
    {
        public int PKID { get; set; }
        public string UserID { get; set; }
    }

    public class CRMContactLog
    {
        public int PKID { get; set; }
        public string UserID { get; set; }
    }

    public class CRMFlagInfo
    {
        public int PKID { get; set; }
        public Guid? UserId { get; set; }
    }

    public class PromotionCode
    {
        public int PKID { get; set; }
        public Guid? UserId { get; set; }
    }

    public class UserIntegral
    {
        public Guid IntegralID { get; set; }
        public Guid UserID { get; set; }
        public int Integral { get; set; }
        public int Status { get; set; }
    }

    public class UserIntegralDetail
    {    
        public Guid IntegralDetailID { get; set; }
        public Guid IntegralID { get; set; }
    }

    public class UserGradeStatisticsDetail
    {
        public int PKID { get; set; }
        public Guid UserID { get; set; }
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
        /// 地址列表
        /// user.Addresses
        /// </summary>
        [Column("u_addresses")]
        public string Addresses { get; set; }

        /// <summary>
        /// 默认地址
        /// </summary>
        [Column("u_preferred_address")]
        public string PreferredAddress { get; set; }

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

        [Column("dt_last_logon")]
        public DateTime? LastLogonTime { get; set; }

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
        /// 暂时当作Channel记录
        /// </summary>
        [Column("u_application_name")]
        public string TempChannel { get; set; }

        /// <summary>
        /// 手机号是否被验证过
        /// user.IsMobileVerified
        /// </summary>
        public bool? IsMobileVerify { get; set; }

        /// <summary>
        /// 合并关联账号，true是主账号，false是被合并的账号，不应该被搜出来
        /// </summary>
        public bool IsActive { get; set; }

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

    public class NeedCombineUserIdViaPhone
    {
        public int PKID { get; set; }
        /// <summary>
        /// 需要被合并的源UserId
        /// </summary>
        public Guid SourceUserId { get; set; }
        /// <summary>
        /// 合并后的主UserId
        /// </summary>
        public Guid TargetUserId { get; set; }

        public string MobileNumber { get; set; }
        /// <summary>
        /// 合并操作结果
        /// </summary>
        public bool IsOperateSuccess { get; set; }

        public DateTime CreateDataTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class UserAuth
    {
        public int PKID { get; set; }

        public Guid UserId { get; set; }
    }
}

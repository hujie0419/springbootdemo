using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenAccount Data Structure.
    /// </summary>
    [Serializable]
    public class OpenAccount : TopObject
    {
        /// <summary>
        /// 支付宝的帐号标识
        /// </summary>
        [XmlElement("alipay_id")]
        public string AlipayId { get; set; }

        /// <summary>
        /// 头像url
        /// </summary>
        [XmlElement("avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        [XmlElement("bank_card_no")]
        public string BankCardNo { get; set; }

        /// <summary>
        /// 银行卡的拥有者姓名
        /// </summary>
        [XmlElement("bank_card_owner_name")]
        public string BankCardOwnerName { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [XmlElement("birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// 创建帐号的App Key
        /// </summary>
        [XmlElement("create_app_key")]
        public string CreateAppKey { get; set; }

        /// <summary>
        /// 帐号创建的设备的ID
        /// </summary>
        [XmlElement("create_device_id")]
        public string CreateDeviceId { get; set; }

        /// <summary>
        /// 账号创建时的位置
        /// </summary>
        [XmlElement("create_location")]
        public string CreateLocation { get; set; }

        /// <summary>
        /// 展示名
        /// </summary>
        [XmlElement("display_name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 数据域
        /// </summary>
        [XmlElement("domain_id")]
        public long DomainId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// 自定义扩展信息Map的Json格式
        /// </summary>
        [XmlElement("ext_infos")]
        public string ExtInfos { get; set; }

        /// <summary>
        /// 1男 2女
        /// </summary>
        [XmlElement("gender")]
        public long Gender { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 记录上次更新时间
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// Open Account Id
        /// </summary>
        [XmlElement("id")]
        public long Id { get; set; }

        /// <summary>
        /// 开发者自定义账号id
        /// </summary>
        [XmlElement("isv_account_id")]
        public string IsvAccountId { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [XmlElement("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [XmlElement("login_id")]
        public string LoginId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [XmlElement("login_pwd")]
        public string LoginPwd { get; set; }

        /// <summary>
        /// 加密算法类型：1、代表单纯MD5，2：代表单一Salt的MD5，3、代表根据记录不同后的MD5
        /// </summary>
        [XmlElement("login_pwd_encryption")]
        public long LoginPwdEncryption { get; set; }

        /// <summary>
        /// 密码加密强度
        /// </summary>
        [XmlElement("login_pwd_intensity")]
        public long LoginPwdIntensity { get; set; }

        /// <summary>
        /// 密码salt
        /// </summary>
        [XmlElement("login_pwd_salt")]
        public string LoginPwdSalt { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// TAOBAO = 1;WEIXIN = 2;WEIBO = 3;QQ = 4;
        /// </summary>
        [XmlElement("oauth_plateform")]
        public long OauthPlateform { get; set; }

        /// <summary>
        /// 第三方oauth openid
        /// </summary>
        [XmlElement("open_id")]
        public string OpenId { get; set; }

        /// <summary>
        /// 账号状态：1、启用，2、删除，3、禁用
        /// </summary>
        [XmlElement("status")]
        public long Status { get; set; }

        /// <summary>
        /// 账号创建类型：1、通过短信创建，2、ISV批量导入，3、ISV OAuth创建
        /// </summary>
        [XmlElement("type")]
        public long Type { get; set; }

        /// <summary>
        /// 记录的版本号
        /// </summary>
        [XmlElement("version")]
        public long Version { get; set; }

        /// <summary>
        /// 旺旺
        /// </summary>
        [XmlElement("wangwang")]
        public string Wangwang { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        [XmlElement("weixin")]
        public string Weixin { get; set; }
    }
}

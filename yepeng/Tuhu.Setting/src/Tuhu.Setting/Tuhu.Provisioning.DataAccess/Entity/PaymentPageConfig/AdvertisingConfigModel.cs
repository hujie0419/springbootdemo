using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 广告配置类
    /// </summary>
    public class AdvertisingConfigModel
    {
        /// <summary>
        /// 主键标识
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 配置标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 广告位置，1=下单完成页
        /// </summary>
        public int AdLocation { get; set; }
        /// <summary>
        /// 广告类型，1=文字广告位；2=图片广告位；3=弹窗
        /// </summary>
        public int AdType { get; set; }
        /// <summary>
        /// 省份标识
        /// </summary>
        public int ProvinceID { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市标识
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 产品线
        /// </summary>
        public string ProductLine { get; set; }
        /// <summary>
        /// 支持渠道
        /// </summary>
        public string SupportedChannels { get; set; }
        /// <summary>
        /// 电脑端广告图标
        /// </summary>
        public string PcIconUrl { get; set; }
        /// <summary>
        /// 移动端广告图标
        /// </summary>
        public string MobileIconUrl { get; set; }
        /// <summary>
        /// 广告语
        /// </summary>
        public string Slogan { get; set; }
        /// <summary>
        /// 操作按钮文字
        /// </summary>
        public string OperatingButtonText { get; set; }
        /// <summary>
        /// 电脑端广告图片
        /// </summary>
        public string PcImageUrl { get; set; }
        /// <summary>
        /// 移动端广告图片
        /// </summary>
        public string MobileImageUrl { get; set; }
        /// <summary>
        /// PC端路由
        /// </summary>
        public string PcRoute { get; set; }
        /// <summary>
        /// 小程序路由
        /// </summary>
        public string MiniProgramRoute { get; set; }
        /// <summary>
        /// 苹果手机路由
        /// </summary>
        public string IosRoute { get; set; }
        /// <summary>
        /// 安卓手机路由
        /// </summary>
        public string AndroidRoute { get; set; }
        /// <summary>
        /// 移动网页端路由
        /// </summary>
        public string WapRoute { get; set; }
        /// <summary>
        /// 华为轻应用路由
        /// </summary>
        public string HuaweiRoute { get; set; }
        /// <summary>
        /// 开始版本号
        /// </summary>
        public string StartVersion { get; set; }
        /// <summary>
        /// 结束版本号
        /// </summary>
        public string EndVersion { get; set; }
        /// <summary>
        /// 上线时间
        /// </summary>
        public string OnlineTime { get; set; }
        /// <summary>
        /// 下线时间
        /// </summary>
        public string OfflineTime { get; set; }
        /// <summary>
        /// 状态，0=停用；1=启用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }


        /// <summary>
        /// 域名+电脑端广告图片路径
        /// </summary>
        public string DomainPcImageUrl { get; set; }
        /// <summary>
        /// 域名+移动端广告图片路径
        /// </summary>
        public string DomainMobileImageUrl { get; set; }
        /// <summary>
        /// 域名+名电脑端广告图标路径
        /// </summary>
        public string DomainPcIconUrl { get; set; }
        /// <summary>
        /// 域名+移动端广告图标路径
        /// </summary>
        public string DomainMobileIconUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 支付页配置类
    /// </summary>
    public class PaymentPageConfigModel
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
        /// 是否显示收货人信息
        /// </summary>
        public bool IsShowConsigneeInfo { get; set; }
        /// <summary>
        /// 是否显示收货地址
        /// </summary>
        public bool IsShowAddress { get; set; }
        /// <summary>
        /// 是否显示预约安装门店
        /// </summary>
        public bool IsShowShop { get; set; }
        /// <summary>
        /// 是否显示查看按钮
        /// </summary>
        public bool IsShowButton { get; set; }
        /// <summary>
        /// 按钮文字
        /// </summary>
        public string ButtonText { get; set; }
        /// <summary>
        /// PC按钮跳转链接
        /// </summary>
        public string ButtonUrl { get; set; }
        /// <summary>
        /// 小程序按钮跳转链接
        /// </summary>
        public string MiniProgramButtonUrl { get; set;}
        /// <summary>
        /// 苹果端按钮跳转链接
        /// </summary>
        public string IosButtonUrl { get; set; }
        /// <summary>
        /// 安卓端按钮跳转链接
        /// </summary>
        public string AndroidButtonUrl { get; set; }
        /// <summary>
        /// 移动网页端按钮跳转链接
        /// </summary>
        public string WapButtonUrl { get; set; }
        /// <summary>
        /// 华为轻应用按钮跳转链接
        /// </summary>
        public string HuaweiButtonUrl { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
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
    }
}

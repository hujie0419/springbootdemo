using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class LiveWorkShopConfigModel
    {
        /// <summary>
        /// 配置表的PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 配置类型 Head 头图 ； Activity 活动； Venue 会场；Shop 门店
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 静态图
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// 文字
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// H5跳转链接
        /// </summary>
        public string H5Url { get; set; }
        /// <summary>
        /// Pc跳转链接
        /// </summary>
        public string PcUrl { get; set; }
        /// <summary>
        /// 门店Gif
        /// </summary>
        public string Gif { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SortNumber { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 门店设备渠道
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }
}

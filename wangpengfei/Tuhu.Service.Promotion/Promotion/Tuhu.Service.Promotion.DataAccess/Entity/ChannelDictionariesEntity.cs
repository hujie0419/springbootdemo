using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 渠道配置
    /// </summary>
    public class ChannelDictionariesEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string ChannelType { get; set; }
        /// <summary>
        /// 渠道key
        /// </summary>
        public string ChannelKey { get; set; }
        /// <summary>
        /// 渠道value
        /// </summary>
        public string ChannelValue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Model
{
    /// <summary>
    /// 渠道
    /// </summary>
    public class ChannelDictionariesModel
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

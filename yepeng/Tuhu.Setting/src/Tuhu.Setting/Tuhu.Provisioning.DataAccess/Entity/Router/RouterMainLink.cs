using System;

namespace Tuhu.Provisioning.DataAccess.Entity.Router
{
    /// <summary>
    /// 待拼接的主链接类
    /// </summary>
    public class RouterMainLink
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String Discription { get; set; }

        /// <summary>
        /// 链接类型
        /// </summary>
        public int LinkKind { get; set; }
    }
}

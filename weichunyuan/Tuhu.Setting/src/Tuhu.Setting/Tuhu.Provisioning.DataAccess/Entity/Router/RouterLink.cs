using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Router
{
    /// <summary>
    /// 完成拼接的完整链接类
    /// </summary>
    public class RouterLink
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
        /// 主链接id
        /// </summary>
        public int MainLinkId { get; set; }

        /// <summary>
        /// 参数id
        /// </summary>
        public int ParameterId { get; set; }
        /// <summary>
        /// 主链接
        /// </summary>
        public RouterMainLink MainLink { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public RouterParameter Parameter { get; set; }

        /// <summary>
        /// 参数组
        /// </summary>
        public IEnumerable<RouterParameter> RouterParameterList { get; set; }


    }
}

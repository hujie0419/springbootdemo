using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MemberPage
{
    public class MemberPageModel
    {
        /// <summary>
        /// 主键标识
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否是模板
        /// </summary>
        public bool IsTemplate { get; set; }
        /// <summary>
        /// 模板标识
        /// </summary>
        public long TemplateId { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayIndex { get; set; }
        /// <summary>
        /// 是否是默认配置
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 开始版本
        /// </summary>
        public string StartVersion { get; set; }
        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersion { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
    }
}

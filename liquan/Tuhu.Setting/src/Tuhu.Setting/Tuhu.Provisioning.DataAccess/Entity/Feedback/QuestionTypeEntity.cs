using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Feedback
{
    /// <summary>
    /// 问题类型实体
    /// </summary>
    public class QuestionTypeEntity
    {
        /// <summary>
        /// 问题类型Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 问题类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 问题类型的默认描述
        /// </summary>
        public string Description { get; set; }
    }
}

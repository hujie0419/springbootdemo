using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 保养类别
    /// </summary>
    public class BaoYangCategoryTreeModel
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 类别类型
        /// </summary>
        public string CategoryType { get; set; }
        public List<BaoYangCategoryLeave> Items { get; set; }
    }
    /// <summary>
    /// 保养类别节点
    /// </summary>
    public class BaoYangCategoryLeave
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string PartName { get; set; }

        public override int GetHashCode()
        {
            return $"{Category}-{PartName}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as BaoYangCategoryLeave;
            if (obj != null)
            {
                return string.Equals(this.Category, target.Category) &&
                    string.Equals(this.PartName, target.PartName);
            }
            return false;
        }
    }
}

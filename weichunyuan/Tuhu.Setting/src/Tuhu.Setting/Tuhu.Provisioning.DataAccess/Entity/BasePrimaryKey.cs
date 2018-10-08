using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 基类 定义了主键ID
    /// </summary>
    public class BasePrimaryKey
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }
    }
}

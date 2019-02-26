using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public abstract class PageBase
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage(int pageSize = 20)
        {
            if (TotalCount % pageSize == 0)
                return TotalCount / pageSize;
            else
                return (TotalCount / pageSize) + 1;
        }
    }
}

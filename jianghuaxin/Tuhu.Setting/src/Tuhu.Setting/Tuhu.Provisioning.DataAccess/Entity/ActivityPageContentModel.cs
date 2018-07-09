using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class ActivityPageContentModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Pkid { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int ProductType { get; set; }

        public string HashKey { get; set; }
    }
}

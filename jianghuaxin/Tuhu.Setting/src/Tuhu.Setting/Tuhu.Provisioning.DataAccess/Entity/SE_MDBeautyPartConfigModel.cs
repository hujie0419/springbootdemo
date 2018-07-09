using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 实体-SE_MDBeautyPartConfigModel 
    /// </summary>
    public partial class SE_MDBeautyPartConfigModel : PageBase
    {
        /// <summary>
        /// 主键标识
        /// </summary>		
        public int Id { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>		
        public string Name { get; set; }

        /// <summary>
        /// 门店类目
        /// </summary>		
        public string InteriorCategorys { get; set; }

        /// <summary>
        /// 自营类目
        /// </summary>		
        public string ExternalCategorys { get; set; }
        /// <summary>
        /// 排除的商品pids
        /// </summary>
        public string ExcludePids { get; set; }

        /// <summary>
        /// 跳转H5 URL
        /// </summary>
        public string H5URL { get; set; }

        /// <summary>
        /// 排序
        /// </summary>		
        public int? Soft { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>		
        public bool IsShow { get; set; }

    }
}

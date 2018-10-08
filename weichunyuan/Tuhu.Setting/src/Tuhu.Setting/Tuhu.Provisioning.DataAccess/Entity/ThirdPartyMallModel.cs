using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ThirdPartyMallModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 批次ID
        /// </summary>
        public Guid BatchGuid { get; set; }
        /// <summary>
        /// 批次名称
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnabled { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool? Visible { get; set; }
        /// <summary>
        /// 记录排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 排序筛选
        /// </summary>
        public int? Select { get; set; }
        /// <summary>
        /// 批次数量
        /// </summary>
        public int? BatchQty { get; set; }
        /// <summary>
        /// 限制数量
        /// </summary>
        public int? LimitQty { get; set; }
        public int? StockQty { get; set; }
        /// <summary>
        /// 图片全路径
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 图片名称
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string operater { get; set; }
       
    }
    public class SerchThirdPartyMallModel
    {
        /// <summary>
        /// 页数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int? Visible { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnabled { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 批次ID
        /// </summary>
        public Guid? BatchGuid { get; set; }
        /// <summary>
        /// 批次名称
        /// </summary>
        public string BatchName { get; set; }
    }

    public class DescriptionModal
    {
      public string title { get; set; }
        public string Description { get; set; }
        public string Sort { get; set; }
    }
}

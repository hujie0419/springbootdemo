using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 优惠券任务对于的类目配置
    /// </summary>
    public class PromotionTaskProductCategoryEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 优惠券任务id
        /// </summary>
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 最后一级产品类目
        /// </summary>
        public int ProductCategoryId { get; set; }
    }
}

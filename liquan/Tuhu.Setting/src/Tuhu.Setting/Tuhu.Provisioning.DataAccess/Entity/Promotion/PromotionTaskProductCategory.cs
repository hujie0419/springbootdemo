using System;

namespace Tuhu.Provisioning.DataAccess
{
    public class PromotionTaskProductCategory
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 优惠券任务表外键
        /// </summary>
        public int PromotionTaskId { get; set; }


        /// <summary>
        /// 产品类型Id
        /// </summary>
        public int ProductCategoryId { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// 节点结构
        /// </summary>
        public string NodeNo { get; set; }

        /// <summary>
        /// 是否全选的父类
        /// </summary>
        public bool IsCheckAll { get; set; }

        /// <summary>
        /// 产品类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 是否删除，0=未删除；1已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
    
}

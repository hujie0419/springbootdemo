using System;
namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 福利类型Model对象
    /// </summary>
    public class TaskTypeModel
    {
        /// <summary>
        /// 主键，自增
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 福利类型值
        /// </summary>
        public string TaskTypeCode { get; set; }

        /// <summary>
        /// 福利类型名称
        /// </summary>
        public string TaskTypeName { get; set; }

        /// <summary>
        /// 是否开启奖励
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// 排序规则
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

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

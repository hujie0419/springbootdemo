namespace Tuhu.Provisioning.DataAccess.Entity.CustomersActivity
{
    /// <summary>
    /// 大客户活动专享配置日志表
    /// </summary>
    public class CustomerExclusiveSettingLogModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PKID { get; set; }

        /// <summary>
        /// 业务对象ID
        /// </summary>
        ///
        public string ObjectId { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        ///
        public string Source { get; set; }

        /// <summary>
        /// 日志分类
        /// </summary>
        ///
        public string ObjectType { get; set; }

        /// <summary>
        /// 修改前数据
        /// </summary>
        ///
        public string BeforeValue { get; set; }


        /// <summary>
        /// 修改后的值
        /// </summary>
        ///
        public string AfterValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        ///
        public string Remark { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        ///
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        ///
        public string CreateDateTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        ///
        public string LastUpdateDateTime { get; set; }

        /// <summary>
        /// 是否删除，0=未删除；1已删除
        /// </summary>
        ///
        public string IsDeleted { get; set; }
    }
}

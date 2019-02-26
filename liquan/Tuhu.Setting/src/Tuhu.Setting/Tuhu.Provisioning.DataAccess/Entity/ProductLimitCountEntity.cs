namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ProductLimitCountEntity
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 类目代码
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 类目层级
        /// </summary>
        public int CategoryLevel { get; set; }

        /// <summary>
        /// 限购数量
        /// </summary>
        public int LimitCount { get; set; }
    }
}
namespace Tuhu.Service.Activity.Models.Requests.Activity
{
    /// <summary>
    /// 佣金商品列表查询请求实体
    /// </summary>
    public class GetCommissionProductListRequest
    {
        /// <summary>
        /// 是否启用; 0:不启用; 1:启用； -1:查询全部
        /// </summary>
        public int IsEnable { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int pageSize { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CommentStatisticsJob.Model
{
    public class ShopCommentTypeRule
    {
        public int PKID { get; set; }
        /// <summary>
        /// 分类code
        /// </summary>
        public string ShopType { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// 产品的 类目
        /// </summary>
        public string ProductType { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string UpdateBy { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>
        /// 分类 规则  中的 产品 pid 前缀
        /// </summary>
        public string PIDPrefix { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public int ShopCommentTypeID { get; set; }
    }

    public class TaskOrderModel
    {
        public int OrderId { get; set; }
        public List<TaskProductModel> Items { get; set; } = new List<TaskProductModel>();
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public string OrderType { get; set; }
    }

    /// <summary>
    /// 产品model
    /// </summary>
    public class TaskProductModel
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 订单 号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 数目
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 所属的分类 【多级】
        /// </summary>
        public List<string> CategoryList { get; set; } = new List<string>();
    }

}

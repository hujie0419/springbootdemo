using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CommentStatisticsJob.Model
{
    public class OrderCommentDataModel
    {
        public int OrderId { get; set; }
        public bool CanComment { get; set; }
        public bool HasShopReceive { get; set; }
        public bool CanReply { get; set; }
        public int? CommentId { get; set; }
        public DateTime? CreateShopReceiveTime { get; set; }
    }

    public class ShopCommentDataModel
    {
        public int OrderId { get; set; }
        public int? ShopId { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public int? CommentId { get; set; }
    }

    public class ShopProductOrderModel
    {
        public int OrderId { get; set; }
        public string Pid { get; set; }
        public int ShopId { get; set; }
        public int CommentId { get; set; }
    }

    public class ShopCommentStatisticsModel {
        public string Pid { get; set; }
        public int ShopId { get; set; }
        public int CommentCount { get; set; }
        public decimal CommentAvgScore { get; set; }
    }

    public class TechCommentStatisticsModel
    {
        public int TechnicianId { get; set; }
        public int ShopId { get; set; }
        public int CommentCount { get; set; }
        public decimal CommentAvgScore { get; set; }
    }
    public class OrderStatusModel
    {
        public int OrderId { get; set; }
    }
}

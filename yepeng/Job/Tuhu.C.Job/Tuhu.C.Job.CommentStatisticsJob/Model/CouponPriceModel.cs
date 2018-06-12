using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CommentStatisticsJob.Model
{
    public class ShopStatisticsModel
    {
        public int ShopID { get; set; }
        public string Type { get; set; }
        public int InstallQuantity { get; set; }
        public int CommentTimes { get; set; }
        public decimal CommentR1 { get; set; }
        public int CommentR2 { get; set; }

        public int CommentR3 { get; set; }
        public int CommentR4 { get; set; }
        public decimal ApplyCommentTimes { get; set; }
        public decimal ApplyCommentRate1 { get; set; }

        public decimal ApplyCommentRate2 { get; set; }
        public decimal ApplyCommentRate3 { get; set; }

        public decimal ApplyCommentRate4 { get; set; }

        /// <summary>
        /// 好评数【不包括 默认好评数】
        /// </summary>
        public decimal FavourableCount { get; set; }

        /// <summary>
        /// 默认好评数
        /// </summary>
        public int DefaultFavourableCount { get; set; }

        /// <summary>
        ///包括默认好评的分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// CommentTimes备份
        /// </summary>
        public int CommentTimesB { get; set; }
        /// <summary>
        /// CommentR1 备份
        /// </summary>
        public decimal CommentR1B { get; set; }
        /// <summary>
        /// CommentR2 备份
        /// </summary>
        public int CommentR2B { get; set; }
        /// <summary>
        /// CommentR3 备份
        /// </summary>
        public int CommentR3B { get; set; }
        /// <summary>
        /// CommentR4 备份
        /// </summary>
        public int CommentR4B { get; set; }
    }

    public class ProductStatisticsModel
    {
        //, , , , , , , , , , 
        public string ProductID { get; set; }
        public string VariantID { get; set; }
        public int OrderQuantity { get; set; }
        public int SalesQuantity { get; set; }
        public int CommentTimes { get; set; }
        public int CommentR1 { get; set; }
        public int CommentR2 { get; set; }
        public int CommentR3 { get; set; }
        public int CommentR4 { get; set; }
        public int CommentR5 { get; set; }
        public float CommentRate { get; set; }


        /// <summary>
        /// 好评数【不包括 默认好评数】
        /// </summary>
        public decimal FavourableCount { get; set; }

        /// <summary>
        /// 默认好评数
        /// </summary>
        public int DefaultFavourableCount { get; set; }

        /// <summary>
        ///包括默认好评的分数
        /// </summary>
        public float Score { get; set; }


        /// <summary>
        /// CommentTimes 备份
        /// </summary>
        public int CommentTimesB { get; set; }
        /// <summary>
        /// CommentR1 备份
        /// </summary>
        public int CommentR1B { get; set; }
        /// <summary>
        /// CommentR2 备份
        /// </summary>
        public int CommentR2B { get; set; }
        /// <summary>
        /// Comment31 备份
        /// </summary>
        public int CommentR3B { get; set; }
        /// <summary>
        /// CommentR4 备份
        /// </summary>
        public int CommentR4B { get; set; }
        /// <summary>
        /// CommentR5 备份
        /// </summary>
        public int CommentR5B { get; set; }


    }

    public class ShopsModel
    {
        public int PKID { get; set; }
    }

    public class TechModel
    {
        public int PKID { get; set; }
    }

    /// <summary>
    /// 产品 订单 统计 
    /// </summary>
    public class ProductOrderStatistics
    {
        public string ProductId { get; set; }
        public int VariantID { get; set; }
        public int OrderQuantity { get; set; }
        public int SalesQuantity { get; set; }
    }

    /// <summary>
    ///产品的 默认好评 model
    /// </summary>
    public class ProductCommentStatusModel
    {
        public string ProductId { get; set; }
        public int DefaultFavourableCount { get; set; }
    }

    /// <summary>
    /// 产品  默认好评的统计 model
    /// </summary>
    public class ProductDefaultFavourableStatisticsModel
    {
        public string ProductFamilyId { get; set; }
        public string VariantID { get; set; }
        public int DefaultFavourableCount { get; set; }
    }

    /// <summary>
    /// 产品 默认好评的统计 model
    /// </summary>
    public class ShopDefaultFavourableStatisticsModel
    {
        public int ShopID { get; set; }
        public string Type { get; set; }
        public int DefaultFavourableCount { get; set; }
    }


    public class ProductScoreModel
    {
        public int ShopId { get; set; }
        public string Pid { get; set; }
        public int CommentCount { get; set; }
        public decimal AvgScore { get; set; }
    }
}

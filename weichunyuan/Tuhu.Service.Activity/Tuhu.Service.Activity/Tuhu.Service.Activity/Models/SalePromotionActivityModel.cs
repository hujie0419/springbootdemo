using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 促销活动实体类 
    /// </summary>
    public class SalePromotionActivityModel
    {
        #region 属性

        public string ActivityId { get; set; }

        public string Name { get; set; }

        public string Banner { get; set; }

        public string Description { get; set; }

        public int Is_DefaultLabel { get; set; }

        public string Label { get; set; }

        public int PromotionType { get; set; }

        public int Is_PurchaseLimit { get; set; }

        public int LimitQuantity { get; set; }

        public int PaymentMethod { get; set; }

        public int InstallMethod { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int? AuditStatus { get; set; }

        public string AuditRemark { get; set; }
        public string AuditDateTime { get; set; }

        public string AuditUserName { get; set; }

        /// <summary>
        ///手动上下架 
        /// </summary>
        public int Is_UnShelve { get; set; }

        /// <summary>
        /// 自动上下架
        /// </summary>
        public int Is_UnShelveAuto { get; set; }
        
        public string CreateDateTime { get; set; }

        public string CreateUserName { get; set; }

        public string LastUpdateDateTime { get; set; }

        public string LastUpdateUserName { get; set; }

        #endregion

        #region 数据保存

        /// <summary>
        /// 活动渠道key集合
        /// </summary>
        public List<string> ChannelKeyList { get; set; }

        public List<SalePromotionActivityDiscount> DiscountContentList { get; set; }

        public int DiscountMethod { get; set; }

        #endregion

        #region 查询条件

        public string Channel { get; set; }

        public string ProductName { get; set; }

        public string Pid { get; set; }

        public int Status { get; set; }

        #endregion

        #region 数据展示

        public IList<SalePromotionActivityProduct> Products { get; set; }

        #endregion

        public SalePromotionActivityModel()
        {
            Products = new List<SalePromotionActivityProduct>();
            DiscountContentList = new List<SalePromotionActivityDiscount>();
        }

    }

    /// <summary>
    /// 活动打折内容
    /// </summary>
    public class SalePromotionActivityDiscount
    {
        public string ActivityId { get; set; }

        /// <summary>
        /// 1满额折，2满件折
        /// </summary>
        public int DiscountMethod { get; set; }
        /// <summary>
        /// 满额度或满件数
        /// </summary>
        public decimal Condition { get; set; }
        /// <summary>
        /// 减金额或折扣比
        /// </summary>
        public int DiscountRate { get; set; }

        public string CreateDateTime { get; set; }

    }


    public class ChannelModel
    {
        public string Name { get; set; }

        public string ChannelKey { get; set; }

    }

    /// <summary>
    /// 活动商品
    /// </summary>
    public class SalePromotionActivityProduct
    {
        public string ActivityId { get; set; }

        public string Pid { get; set; }

        public string ProductName { get; set; }

        public decimal CostPrice { get; set; }

        public int TotalStock { get; set; }

        public decimal SalePrice { get; set; }

        public int LimitQuantity { get; set; }

        public int SoldQuantity { get; set; }

        public string ImageUrl { get; set; }

        public string CreateDateTime { get; set; }

        public string LastUpdateDateTime { get; set; }

        #region 其他属性

        /// <summary>
        /// 添加失败原因
        /// </summary>
        public string FailMessage { get; set; }

        public string ActivityName { get; set; }

        /// <summary>
        /// 折后毛利
        /// </summary>
        public string DiscountMargin { get; set; }

        /// <summary>
        /// 折后毛利率
        /// </summary>
        public string DiscountMarginRate { get; set; }

        public string Remark { get; set; }

        public bool IsMinusProfile { get; set; }

        #endregion

    }


    #region 用来查询的实体

    /// <summary>
    /// 查询活动列表的数据实体
    /// </summary>
    public class SelectActivityListModel
    {
        public List<SalePromotionActivityModel> ActivityList { get; set; }

        public ActicityListCountModel Counts { get; set; }

        public int CurrentStatusCount { get; set; }

        public SelectActivityListModel()
        {
            ActivityList = new List<SalePromotionActivityModel>();
        }
    }

    public class ActicityListCountModel
    {
        public int AllCount { get; set; }

        public int WaitAuditCount { get; set; }

        public int OnlineCount { get; set; }

        public int StayOnlineCount { get; set; }

        public int RejectedCount { get; set; }

        public int EndCount { get; set; }
    }

    /// <summary>
    /// 查询活动商品
    /// </summary>
    public class SelectActivityProduct
    {
        public string ActivityId { get; set; }
        public string Pid { get; set; }
        public string ProductName { get; set; }
        public decimal? LowProfile { get; set; }
        public decimal? HighProfile { get; set; }
        public bool IsDeleted { get; set; }

        public string AfterCreateDateTime { get; set; }
        public string AfterLastUpdateDateTime { get; set; }


    }

    #endregion

}



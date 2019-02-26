using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.SalePromotionActivity
{

    public class SalePromotionSearchProduct
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
}

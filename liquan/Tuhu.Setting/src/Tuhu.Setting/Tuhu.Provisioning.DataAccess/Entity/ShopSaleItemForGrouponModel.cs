using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class ShopSaleItemForGrouponModel : PageBase
    {
        /// <summary>
        /// 主键标识
        /// </summary>		
        public int PKID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int ShopID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string ShopName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int CategoryId { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string CategoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int ProductID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string ProductName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int BrandID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string BrandName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string ShowName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int DayLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public decimal? RetailPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public bool IsSale { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int SaleLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int GrouponStutas { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int CheckStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string CheckMemo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int RecommendationId { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Recommendation { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? SaleStartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? SaleEndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string CreatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string AuditBy { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string PID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int AdaptiveCar { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? ApplyTime { get; set; }

    }
}
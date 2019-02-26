using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_GetPromotionActivityCouponInfoConfig
    {
        /// <summary>
        /// ID
        /// </summary>		
        public int ID { get; set; }
        /// <summary>
        /// FK_GetPromotionActivityID
        /// </summary>		
        public Guid FK_GetPromotionActivityID { get; set; }
        /// <summary>
        /// GetRuleID
        /// </summary>		
        public int GetRuleID { get; set; }
        /// <summary>
        /// GetRuleGUID
        /// </summary>		
        public Guid GetRuleGUID { get; set; }
        /// <summary>
        /// VerificationMode
        /// </summary>		
        public int VerificationMode { get; set; }
        /// <summary>
        /// ValidDays
        /// </summary>		
        public int? ValidDays { get; set; }
        /// <summary>
        /// ValidStartDateTime
        /// </summary>		
        public DateTime? ValidStartDateTime { get; set; }
        /// <summary>
        /// ValidEndDateTime
        /// </summary>		
        public DateTime? ValidEndDateTime { get; set; }
        /// <summary>
        /// UserType
        /// </summary>		
        public int UserType { get; set; }
        /// <summary>
        /// Description
        /// </summary>		
        public string Description { get; set; }
        /// <summary>
        /// Discount
        /// </summary>		
        public decimal Discount { get; set; }
        /// <summary>
        /// MinMoney
        /// </summary>		
        public decimal MinMoney { get; set; }
        /// <summary>
        /// SingleQuantity
        /// </summary>		
        public int SingleQuantity { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>		
        public int? Quantity { get; set; }


        public bool Status { get; set; }

        /// <summary>
        /// 领取用户的类型 0所有用户 1新用户 2老用户
        /// </summary>
        public int GetUserType { get; set; }


    }
}

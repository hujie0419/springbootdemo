using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class tbl_UserPromotionCode:BaseModel
    {
        public tbl_UserPromotionCode() : base() { }

        public tbl_UserPromotionCode(DataRow row) : base(row) { }


        public int ID { get; set; }

        public string SImage { get; set; }

        public string BImage { get; set; }


        public int RuleID { get; set; }

        public string RuleGuid { get; set; }


        /// <summary>
        /// 用户等级 LV1 LV2 LV3 LV4
        /// </summary>
        public string UserRank { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string CouponDescription { get; set; }


    }
}

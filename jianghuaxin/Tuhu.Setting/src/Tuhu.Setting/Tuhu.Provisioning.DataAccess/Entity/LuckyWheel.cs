using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class LuckyWheel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public int isNewUser { get; set; }

        public int isStatus { get; set; }

        public DateTime CreateDate { get; set; }


        public DateTime UpdateDate { get; set; }

        public string DataParames { get; set; }


       public  List<LuckyWheelDeatil> Items { get; set; }

        /// <summary>
        /// 是否关闭分享一次加以1抽奖
        /// </summary>
        public int IsAddOne { get; set; }

        /// <summary>
        /// 是否积分抽奖 1：积分抽奖 0：非积分抽奖
        /// </summary>
        public int IsIntegral { get; set; }

        /// <summary>
        /// 单次积分数
        /// </summary>
        public int Integral { get; set; }

        public List<LuckyItem> LuckyItems { get; set; }


        public string CreatorUser { get; set; }

        public string UpdateUser { get; set; }

        /// <summary>
        /// 分享前的次数
        /// </summary>
        public int? PreShareTimes { get; set; }

        /// <summary>
        /// 分享后增加的次数
        /// </summary>
        public int? CompletedShareTimes { get; set; }



    }


    public class LuckyWheelDeatil
    {
        public int ID { get; set; }

        public string FKLuckyWheelID { get; set; }

        public int UserRank { get; set; }


        public int Type { get; set; }

        public string CouponRuleID { get; set; }

        public string MaxCoupon { get; set; }

        public string BGImage { get; set; }

        public string ContentImage { get; set; }


        public string ChangeImage { get; set; }


        public string GetDescription { get; set; }


        public string GoDescription { get; set; }

        public string APPUrl { get; set; }

        public string WapUrl { get; set; }

        public string WwwUrl { get; set; }

        public string HandlerAndroid { get; set; }

        public string SOAPAndroid { get; set; }

        public string HandlerIOS { get; set; }

        public string SOAPIOS { get; set; }


        public int OrderBy { get; set; }


    }

   
    public class LuckyItem
    {
        public int UserRank { get; set; }
        public int Coupon { get; set; }

        public int? Count { get; set; }
    }

}

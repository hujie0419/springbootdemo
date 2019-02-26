using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ZeroActivityDetail
    {
        //期数
        public int Period { get; set; }
        //产品ProductID
        public string ProductID { get; set; }
        //产品VariantID
        public string VariantID { get; set; }
        //本期获奖人数
        public int SucceedQuota { get; set; }
        //提供的产品总数
        public int Quantity { get; set; }
        //活动开始时间
        public DateTime StartDateTime { get; set; }
        //活动结束时间
        public DateTime EndDateTime { get; set; }
        //创建时间
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 头图url
        /// </summary>
        public string ImgUrl { get; set; }

        public string Pid { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Tuhu.Provisioning.Models
{
    public class AdProductItem
    {
        public int AdvertiseID { get; set; }
        [Display(Name = "产品编号")]
        public string PID { get; set; }
        [Display(Name = "产品名称")]
        public string ProductName { get; set; }
        [Display(Name = "排序顺序")]
        public byte Position { get; set; }
        [Display(Name = "状态")]
        public byte State { get; set; }
        [Display(Name = "状态名称")]
		public string StateName { get; set; }
		[Display(Name = "特价促销价")]
		public decimal PromotionPrice { get; set; }
		[Display(Name = "限量")]
		public int PromotionNum { get; set; }
    }
}
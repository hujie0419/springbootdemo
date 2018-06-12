using System;
using System.ComponentModel.DataAnnotations;

namespace Qpl.Api.Models
{
    public class QcCustomerOrderItemModel
    {
        [Key]
        public int IID { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "{1}到{0}个字符")]
        [Display(Name = "用户编号")]
        public string UID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "{1}到{0}个字符")]
        [Display(Name = "订单ID")]
        public string OrderId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "商品ID")]
        public string UserProductId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "商品ID")]
        public string ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [Display(Name = "商品数量")]
        public int ProductCount { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "商品编号")]
        public string ProductNo { get; set; }

        /// <summary>
        /// 商品单位
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "商品单位")]
        public string ProductUnit { get; set; }

        /// <summary>
        /// 合计价格
        /// </summary>
        //[StringLength(18, ErrorMessage = "最长{0}个数字")]
        [Display(Name = "合计价格")]
        public double ListPrice { get; set; }

        /// <summary>
        /// 最终价格
        /// </summary>
        //[StringLength(18, ErrorMessage = "最长{0}个数字")]
        [Display(Name = "最终价格")]
        public double FinalPrice { get; set; }

        /// <summary>
        /// 扩展
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "扩展")]
        public string Extension { get; set; }

        /// <summary>
        /// 是否移除
        /// </summary>
        [Display(Name = "是否移除")]
        public int IsRemove { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "创建时间")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "更新时间")]
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// 聚划算的组合id
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "聚划算组合id")]
        public string GroupId { get; set; }

        public QcUserProductInfoModel UserProductInfo { get; set; }

        public double TotalPrice { get; set; }
    }
}



using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangModel : BaseModel
    {

        public BaoYangModel(){}

        public BaoYangModel(DataRow row) : base(row)
        {
        }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
            this.isActivity = Convert.ToInt32(row.GetValue("NUM")) > 0 ? true : false;
        }


        public int? PKID { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "类型不能为空")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "类型长度1-20字符")]
        [Display(Name = "类型")]
        public string byType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "类型名不能为空")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "类型名长度1-20字符")]
        [Display(Name = "类型名")]
        public string byValue { get; set; }

        [Display(Name = "是否活动")]
        public bool isActivity { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "文案不能为空")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "文案长度1-20字符")]
        [Display(Name = "文案")]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "序号不能为空")]
        [Range(typeof(Int32),"0","100",ErrorMessage = "数值范围要在0-100之间")]
        [Display(Name = "排序")]

        public int? sequence { get; set; }


        /// <summary>
        /// 图片编码
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "图片编码不能为空")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "图片编码长度5-20字符")]
        [Display(Name = "图片编码")]
        public string ImgCode { get; set; }


        /// <summary>
        ///跳转地址
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "跳转地址不能为空")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "跳转地址长度1-200字符")]
        [Display(Name = "跳转地址")]
        public string StrUrl { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public string PageType { get; set; }





    }


    public class BaoYangItemModel : BaseModel
    {
         public BaoYangItemModel(){}

         public BaoYangItemModel(DataRow row) : base(row) { }

        public int? PKID { get; set; }
        public int? BaoYangActivityStyleID { get; set; }


        [Display(Name = "是否活动")]
        public bool isActivity { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "文案不能为空")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "文案长度1-20字符")]
        [Display(Name = "文案")]
        public string Description { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "开始时间不能为空")]
        [Display(Name = "开始时间")]
        public DateTime StartTime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "结束时间不能为空")]
        [Display(Name = "结束时间")]
        public DateTime EndTime { get; set; }
    }
}

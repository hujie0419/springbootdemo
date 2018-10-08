using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ZeroActivityApply
    {
        public int PKID { get; set; }

        /// <summary>
        /// "活动期数"
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户电话号码
        /// </summary>
        public string UserMobileNumber { get; set; }

        /// <summary>
        /// 产品PID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 产品尺寸
        /// </summary>
        public string ProductSize { get; set; }

        /// <summary>
		/// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 申请数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 所在省ID
        /// </summary>
        public int ProvinceID { get; set; }

        /// <summary>
        /// 所在省名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 所在地区ID
        /// </summary>
        public int CityID { get; set; }

        /// <summary>
        /// 所在地区名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyDateTime { get; set; }

        /// <summary>
        /// 加油数量
        /// </summary>
        public int SupportNumber { get; set; }

        /// <summary>
        /// 是否通过申请审核
        /// </summary>
        public int? Succeed { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        public string ApplyReason { get; set; }

        /// <summary>
        /// 用户订单数
        /// </summary>
        public int? UserOrderQuantity { get; set; }
        /// <summary>
        /// 对应的订单号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 试用报告的状态  0表示还未填写试用报告    1表示仅填写了试用报告   2表示填写了试用报告和试用者信息   3表示试用报告通过可返押金
        /// </summary>
        public int? ReportStatus { get; set; }
        /// <summary>
        /// 申请渠道  0表示网站   1表示手机端
        /// </summary>
        public int? ApplyChannel { get; set; }

        public string CarName { get; set; }
        /// <summary>
        /// 1 获奖  0 申请中  -1 申请失败
        /// </summary>
        public int Status { get; set; }
    }
}
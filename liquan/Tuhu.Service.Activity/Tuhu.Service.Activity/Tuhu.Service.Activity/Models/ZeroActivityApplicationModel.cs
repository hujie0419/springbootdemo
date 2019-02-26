using System;

namespace Tuhu.Service.Activity.Models
{
    public class ZeroActivityRequest
    {
        /// <summary>期数</summary>
        public int Period { get; set; }
        /// <summary>用户名称</summary>
        public Guid UserId { get; set; }
        /// <summary>用户名称</summary>
        public string UserName { get; set; }
        /// <summary>用户所在省</summary>
        public int ProvinceID { get; set; }
        /// <summary>用户所在市</summary>
        public int CityID { get; set; }
        /// <summary>用户车型（五级）</summary>
        public Guid CarID { get; set; }
        /// <summary>申请理由</summary>
        public string ApplicationReason { get; set; }
    }

    public class MyZeroActivityApplications
    {
        /// <summary>期数</summary>
        public int Period { get; set; }
        /// <summary>产品名称</summary>
        public string ProductName { get; set; }
        /// <summary>众测报告提交时间</summary>
        public DateTime ApplyDateTime { get; set; }
        /// <summary>活动头图</summary>
        public string ImgUrl { get; set; }
        /// <summary>众测订单ID</summary>
        public int? OrderID { get; set; }
        /// <summary>活动开始日期</summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>活动结束日期</summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>活动状态（0:即将开始, 1:正在进行, 3:众测体验中, 4:众测已结束）</summary>
        public int StatusOfActivity { get; set; }
        /// <summary>商品PID</summary>
        public string PID { get; set; }
    }
}
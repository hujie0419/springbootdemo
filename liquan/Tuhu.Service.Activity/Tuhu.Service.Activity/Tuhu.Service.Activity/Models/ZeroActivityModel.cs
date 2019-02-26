using System;
using System.Collections.Generic;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class ZeroActivityModel : BaseModel
    {
        /// <summary>期数</summary>
        public int Period { get; set; }
        /// <summary>产品名称</summary>
        public string ProductName { get; set; }
        /// <summary>申请数量</summary>
        public int NumOfApplications { get; set; }
        /// <summary>获奖人数</summary>
        [Column("SucceedQuota")]
        public int NumOfWinners { get; set; }
        /// <summary>提供的奖品总数</summary>
        public decimal SingleValue { get; set; }
        /// <summary>活动开始日期</summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>活动结束日期</summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>活动状态（0:即将开始, 1:正在进行, 3:众测体验中, 4:众测已结束）</summary>
        public int StatusOfActivity { get; set; }
        /// <summary>活动描述</summary>
        public string Description { get; set; }
        /// <summary>活动头图</summary>
        public string ImgUrl { get; set; }
        /// <summary>商品PID</summary>
        public string PID { get; set; }
        /// <summary>奖品总量</summary>
        public int Quantity { get; set; }


        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }
    }

    public class ZeroActivityDetailModel : ZeroActivityModel
    {
        /// <summary>商品头图</summary>
        public IEnumerable<string> ProductImages { get; set; }
    }

    public class ZeroActivitySimpleRespnseModel
    {

        /// <summary>商品PID</summary>
        public string Pid { get; set; }

        /// <summary>申请数量</summary>
        public int NumOfApplications { get; set; }

        /// <summary>活动头图</summary>
        public string ImgUrl { get; set; }

        /// <summary>奖品总量</summary>
        public int Quantity { get; set; }
        /// <summary>获奖人数</summary>

        public int NumOfWinners { get; set; }

    }
}

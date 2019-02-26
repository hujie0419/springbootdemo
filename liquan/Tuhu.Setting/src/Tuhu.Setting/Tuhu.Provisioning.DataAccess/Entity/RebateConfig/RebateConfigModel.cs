using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.RebateConfig
{
    public class RebateConfigModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string UserPhone { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public int PayStatus { get; set; }
        /// <summary>
        /// 红包订单号
        /// </summary>
        public string RedOutbizNo { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 微信ID
        /// </summary>
        public string WXId { get; set; }
        /// <summary>
        /// 微信昵称
        /// </summary>
        public string WXName { get; set; }
        /// <summary>
        /// 百度ID
        /// </summary>
        public string BaiDuId { get; set; }
        /// <summary>
        /// 百度Name
        /// </summary>
        public string BaiDuName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string PrincipalPerson { get; set; }
        /// <summary>
        /// 返现金额
        /// </summary>
        public decimal RebateMoney { get; set; }
        /// <summary>
        /// 返现时间
        /// </summary>
        public DateTime? RebateTime { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? CheckTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 安装门店
        /// </summary>
        public int InstallShopId { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string CarparName { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string QRCodeImg { get; set; }

        public List<RebateApplyImageConfig> ImgList { get; set; }

        public int Total { get; set; }

        public string UserName { get; set; }

        public string CarNumber { get; set; }

        public string Source { get; set; }

        public string ContentUrl { get; set; }

        public string RefusalReason { get; set; }
        public string UnionId { get; set; }
        public string OpenId { get; set; }
    }

    public class RebateApplyImageConfig
    {
        public int ParentId { get; set; }

        public string ImgUrl { get; set; }

        public string Remarks { get; set; }

        public string Source { get; set; }
    }

    public class RebateApplyPageConfig
    {
        public int PKID { get; set; }

        public string BackgroundImg { get; set; }

        public string ActivityRules { get; set; }
        /// <summary>
        /// 返现成功消息
        /// </summary>
        public string RebateSuccessMsg { get; set; }
        /// <summary>
        /// 红包零钱备注
        /// </summary>
        public string RedBagRemark { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string CreateUser { get; set; }

        public List<RebateApplyImageConfig> ImgList { get; set; }

        public int Total { get; set; }
    }

    public enum ImgSource
    {
        PageImg,
        UserImg
    }

    public enum Status
    {
        None,//未申请
        Applying,//待审核
        Approved,//审核通过
        Unapprove,//审核拒绝
        Complete//已完成
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ThirdPartyOrderChannellink
{
    public class ThirdPartyOrderChannellinkModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 三方订单渠道ID
        /// </summary>
        public int OrderChanneID { get; set; }

        /// <summary>
        /// 三方订单渠道英文名
        /// </summary>
        public string OrderChannelEngName { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 初始页面链接
        /// </summary>
        public string InitialPagelink { get; set; }

        /// <summary>
        /// 最终页面链接
        /// </summary>
        public string FinalPagelink { get; set; }

        /// <summary>
        /// 是否聚合页
        /// </summary>
        public bool IsAggregatePage { get; set; }

        /// <summary>
        /// 是否授权登录
        /// </summary>
        public bool IsAuthorizedLogin { get; set; }

        /// <summary>
        /// 是否合作方收银
        /// </summary>
        public bool IsPartnerReceivSilver { get; set; }

        /// <summary>
        /// 是否订单回传
        /// </summary>
        public bool IsOrderBack { get; set; }

        /// <summary>
        /// 是否查看订单（浮层）
        /// </summary>
        public bool IsViewOrders { get; set; }

        /// <summary>
        /// 是否查看优惠券（浮层）
        /// </summary>
        public bool IsViewCoupons { get; set; }

        /// <summary>
        /// 是否联系客服（浮层）
        /// </summary>
        public bool IsContactUserService { get; set; }

        /// <summary>
        /// 是否返回顶部（浮层）
        /// </summary>
        public bool IsBackTop { get; set; }

        /// <summary>
        /// 额外需求
        /// </summary>
        public string AdditionalRequirement { get; set; }

        /// <summary>
        /// 三方订单渠道
        /// </summary>
        public string OrderChannel { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

    }

}

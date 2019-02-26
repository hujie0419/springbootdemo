using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ThirdPartyExchangeCode
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 批次ID
        /// </summary>
        public Guid BatchGuid { get; set; }

        /// <summary>
        /// 是否启用 1启用 0禁用
        /// </summary>
        public bool? IsEnabled { get; set; }

        /// <summary>
        /// 是否领取 1领取 0未领取
        /// </summary>
        public bool? IsGain { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public string IsExpirationed { get; set; }

        /// <summary>
        /// 领取用户UserId  
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 兑换码
        /// </summary>
        public string ExchangeCode { get; set; }

        /// <summary>
        /// 兑换开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 兑换截止时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 导入时间
        /// </summary>
        public DateTime ImportDateTime { get; set; }

        /// <summary>
        /// 领取日期
        /// </summary>
        public DateTime? GainDateTime { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

    }

    public class SerchCodeElement
    {
        /// <summary>
        /// BatchId
        /// </summary>
        public Guid BatchId { get; set; }
        /// <summary>
        /// 当前所在页
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否启用 1启用 0禁用
        /// </summary>
        public bool? IsEnabled { get; set; }

        /// <summary>
        /// 是否领取 1领取 0未领取
        /// </summary>
        public bool? IsGain { get; set; }
        /// <summary>
        /// 过期
        /// </summary>
        public DateTime? OutTime { get; set; }
        /// <summary>
        /// 未过期
        /// </summary>
        public DateTime? OnTime { get; set; }
    }
}

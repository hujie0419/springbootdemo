using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.GFCard
{
    public class GFBankPromotionTask: GFBankTaskBaseModel
    {
        /// <summary>
        /// 优惠券规则
        /// </summary>
        public Guid RuleGuid { get; set; }
        /// <summary>
        /// 优惠券ID,逗号隔开
        /// </summary>
        public string PromotionIds { get; set; }     
    }
}

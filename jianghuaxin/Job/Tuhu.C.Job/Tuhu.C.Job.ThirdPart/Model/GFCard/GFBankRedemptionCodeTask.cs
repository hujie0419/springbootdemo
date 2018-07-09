using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.GFCard
{
    public class GFBankRedemptionCodeTask: GFBankTaskBaseModel
    {
        /// <summary>
        /// 兑换码
        /// </summary>
        public string RedemptionCode { get; set; }
    }
}

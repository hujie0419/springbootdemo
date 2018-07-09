using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class VerificationTiresResponseModel
    {
        /// <summary></summary>
        public bool Result { set; get; }
        /// <summary></summary>
        public List<HitRulesModel> HitRules { set; get; }
        /// <summary></summary>
        public string ErrorMessage { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class HitRulesModel
    {
        /// <summary></summary>
        public long Count { set; get; }
        /// <summary></summary>
        public string HitName { set; get; }
        /// <summary></summary>
        public string Description { set; get; }
    }

    public class ShareProductModel
    {
        public string BatchGuid { set; get; }
        public decimal Times { set; get; }
        public int FirstShareNumber { set; get; }
        public string RuleInfo { set; get; }
        public string PID { set; get; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class RebateApplyPageConfig
    {
        public int PKID { get; set; }

        public string BackgroundImg { get; set; }

        public string ActivityRules { get; set; }

        public List<RebateApplyImageConfig> ImgList { get; set; }
    }

    public class RebateApplyImageConfig
    {
        public string ImgUrl { get; set; }

        public string Remarks { get; set; }

        public string Source { get; set; }
    }
}

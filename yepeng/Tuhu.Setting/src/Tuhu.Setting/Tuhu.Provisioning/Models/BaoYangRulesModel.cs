using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class BaoYangRulesModel
    {
        public Dictionary<string, Tuple<string, bool>> SkuItems { get; set; }

        public Dictionary<string, Tuple<string, bool>> BrandItems { get; set; }

        public Dictionary<string, Tuple<string, bool>> BrandPriceItems { get; set; }

        public bool AntifreezeStatus { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class BaoYangRulesModel
    {
        public List<Tuple<string, Tuple<string, bool>>> SkuItems { get; set; }

        public List<Tuple<string, Tuple<string, bool>>> BrandItems { get; set; }

        public List<Tuple<string, Tuple<string, bool>>> BrandPriceItems { get; set; }

        public bool AntifreezeStatus { get; set; }
    }
}
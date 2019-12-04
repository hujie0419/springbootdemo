using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace Tuhu.C.ActivityJob.Models.GroupBuying
{
    [ElasticsearchType(IdProperty = "ProductIndex", Name = "GroupBuyingProduct")]
    public class ESGroupBuyingProduct
    {
        [String(Name = nameof(ProductIndex), Index = FieldIndexOption.NotAnalyzed)]
        public string ProductIndex { get; set; }

        [String(Name = nameof(PID), Index = FieldIndexOption.NotAnalyzed)]
        public string PID { get; set; }

        [String(Name = nameof(ProductGroupId), Index = FieldIndexOption.NotAnalyzed)]
        public string ProductGroupId { get; set; }
    }
}

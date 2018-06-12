using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    [ElasticsearchType(IdProperty = "PKID", Name = "VehiclePartsLiYang")]
    public class VehiclePartsLiYangEsModel
    {

        public long PKID { get; set; }
        [String(Name = "Category", Index = FieldIndexOption.NotAnalyzed)]
        public string Category { get; set; }
        [String(Name = "MainCategory", Index = FieldIndexOption.NotAnalyzed)]
        public string MainCategory { get; set; }
        [String(Name = "SubGroup", Index = FieldIndexOption.NotAnalyzed)]
        public string SubGroup { get; set; }
        [String(Name = "OeCode", Index = FieldIndexOption.NotAnalyzed)]
        public string OeCode { get; set; }
        [String(Name = "OeName", Index = FieldIndexOption.NotAnalyzed)]
        public string OeName { get; set; }
        [String(Name = "OeCodeWithName", Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public string OeCodeWithName { get; set; }
        [String(Name = "OeEnName", Index = FieldIndexOption.NotAnalyzed)]
        public string OeEnName { get; set; }
        [String(Name = "ImageNo", Index = FieldIndexOption.NotAnalyzed)]
        public string ImageNo { get; set; }
        [String(Name = "Position", Index = FieldIndexOption.NotAnalyzed)]
        public string Position { get; set; }
        [String(Name = "Dosage", Index = FieldIndexOption.NotAnalyzed)]
        public string Dosage { get; set; }
        [String(Name = "Price", Index = FieldIndexOption.NotAnalyzed)]
        public string Price { get; set; }
        [String(Name = "Remarks", Index = FieldIndexOption.NotAnalyzed)]
        public string Remarks { get; set; }
        [String(Name = "LiYangId", Index = FieldIndexOption.NotAnalyzed)]
        public string LiYangId { get; set; }
        [Nested(Name = "LiYangIdList")]
        public List<LiYangIdModel> LiYangIdList
        {
            get; set;
        }
        [String(Name = "BatchCode", Index = FieldIndexOption.NotAnalyzed)]
        public string BatchCode { get; set; }
        [String(Name = "Brand", Index = FieldIndexOption.NotAnalyzed)]
        public string Brand { get; set; }
        [String(Name = "Series", Index = FieldIndexOption.NotAnalyzed)]
        public string Series { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }
    }

    public class LiYangIdModel
    {
        [String(Name = "LyId", Index = FieldIndexOption.NotAnalyzed)]
        public string LyId { get; set; }
        [String(Name = "Tid", Index = FieldIndexOption.NotAnalyzed)]
        public string Tid { get; set; }
    }
}

using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Tuhu.Models;

namespace Tuhu.C.Job.Models
{
    [ElasticsearchType(IdProperty = "Pid", Name = "Product")]
    public class Product : BaseModel
    {
        public string Pid { get; set; }

        /// <summary>
        /// Ik_max_word分词的Keyword
        /// </summary>
        public string KeywordForIkMax { get; set; }


        /// <summary>
        /// Ik_smart分词的Keyword
        /// </summary>
        public string KeywordForIkSmart { get; set; }

        /// <summary>
        /// 标准分词的Keyword
        /// </summary>
        public string KeywordForStandard { get; set; }

        public DateTime InsertDateTime { get; set; }
        [Column("DefinitionName")]
        public string DefinitionName { get; set; }
        [Column("OnSale")]
        public bool OnSale { get; set; }
        [Column("stockout")]
        public bool StockOut { get; set; }
        [Column("CP_Tire_AspectRatio")]
        public string TireAspectRatio { get; set; }
        [Column("CP_Tire_Rim")]
        public string TireRim { get; set; }
        [Column("CP_Tire_Width")]
        public string TireWidth { get; set; }
        public Product() { }

        protected override void Parse(DataRow row, PropertyInfo[] properties)
        {
            base.Parse(row, properties);

            var list = new List<string>(4);

            if (row.HasValue("DisplayName"))
                list.Add(row["DisplayName"]?.ToString());
            if (row.HasValue("CategoryName"))
                list.Add(row["CategoryName"]?.ToString());
            if (row.HasValue("CP_ShuXing3"))
                list.Add(row["CP_ShuXing3"]?.ToString());
            if (row.HasValue("CP_ShuXing5"))
                list.Add(row["CP_ShuXing5"]?.ToString());

            KeywordForIkMax = KeywordForIkSmart = KeywordForStandard = string.Join(" ", list);
            InsertDateTime = DateTime.Now;
        }
    }
}

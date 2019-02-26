using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mime;
using Tuhu.Component.Framework.Extension;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MultiCityPaintModel : BaseModel
    {
        public MultiCityPaintModel() { }
        public MultiCityPaintModel(DataRow row) : base(row) { }
        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }
        public IEnumerable<PaintInfoModel> CountryPaintList { get; set; }
        public IEnumerable<PaintInfoModel> CityPaintList { get; set; }
    }
    public class CityPaintModel : BaseModel
    {
        public CityPaintModel() { }

        public CityPaintModel(DataRow row) : base(row)
        {
        }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }

        public int PKID { get; set; }

        [Display(Name = "产品ID")]
        public int PaintId { get; set; }

        [Display(Name = "城市ID")]
        public int? CityId { get; set; }

        [Display(Name = "省ID")]
        public int? ProvinceId { get; set; }

        [Display(Name = "城市名")]
        public string CityName { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime UpdatedTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }

    public class PaintInfoModel : BaseModel
    {
        public PaintInfoModel() { }

        public PaintInfoModel(DataRow row) : base(row)
        {
        }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }

        public int PKID { get; set; }

        [Display(Name = "产品ID")]
        public string PID { get; set; }

        [Display(Name = "产品名")]
        public string DisplayName { get; set; }
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "是否全国")]
        public bool IsCountry { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime UpdatedTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "城市名")]
        public string CityName { get; set; }

        public List<RegionCityModel> CityList { get; set; }
    }

    public class PaintModel : BaseModel
    {
        public PaintModel() { }

        public PaintModel(DataRow row) : base(row)
        {
        }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }
        [Display(Name = "产品ID")]
        public string PID { get; set; }

        [Display(Name = "油漆名")]
        public string DisplayName { get; set; }
    }

    public class RegionCityModel : BaseModel
    {
        public RegionCityModel() { }

        public RegionCityModel(DataRow row) : base(row)
        {
        }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }
        [Display(Name = "省ID")]
        public int ProvinceID { get; set; }

        [Display(Name = "省名")]
        public string Province { get; set; }
        [Display(Name = "城ID")]
        public int CityID { get; set; }

        [Display(Name = "城名")]
        public string City { get; set; }

        [Display(Name = "地名首字母")]
        public string Letter { get; set; }
        [Display(Name = "是否已选")]
        public bool Selected { get; set; }
    }
}

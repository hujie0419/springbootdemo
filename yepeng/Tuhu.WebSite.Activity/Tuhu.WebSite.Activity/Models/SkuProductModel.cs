using System;
//using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class SkuProductModel : ProductModel
    {
        public SkuProductModel() { }
        internal SkuProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>VariantID</summary>
        public string VariantID { get; set; }

        /// <summary>产品评论统计</summary>
        public ProductStatisticsModel ProductStatistics { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            base.Parse(row);

            this.VariantID = row.GetValue("VariantID");

            var list = new List<string>();
            list.Add(row.GetValue("Variant_Image_filename_1"));
            list.Add(row.GetValue("Variant_Image_filename_2"));
            list.Add(row.GetValue("Variant_Image_filename_3"));
            Image.ImageUrls = list.Where(img => !string.IsNullOrWhiteSpace(img)).Union(Image.ImageUrls);

            ProductStatistics = new ProductStatisticsModel(row);
        }

        public static SkuProductModel CreateSkuProductModel(System.Data.DataRow row)
        {
            if (row == null)
                return null;

            switch (row.GetValue("DefinitionName"))
            {
                case "Tires":
                    return new TireProductModel(row);
                case "Lungu":
                    return new HubProductModel(row);
                case "Wiper":
                    return new WiperProductModel(row);
                case "Brake":
                    return new BrakeProductModel(row);
                case "Battery":
                    return new BatteryProductModel(row);
                case "Filter":
                    return new FilterProductModel(row);
                case "AutoProd":
                    return new AutoProductModel(row);
                default:
                    switch (row.GetValue("Category"))
                    {
                        case "FilterElement":
                            return new FilterElementProductModel(row);
                        case "CarWashing":
                        case "CarMaintenance":
                            return new CarBeautyProductModel(row);
                        case "Oil":
                            return new OilProductModel(row);
                        case "Membrane":
                            return new MembraneProductModel(row);
                        default:
                            return new DefaultProductModel(row);
                    }
            }
        }
    }

    public class ProductStatisticsModel : BaseModel
    {
        public string ProductID { get; set; }
        /// <summary>购买人数</summary>
        public int OrderQuantity { get; set; }
        /// <summary>销售数量</summary>
        public int SalesQuantity { get; set; }
        /// <summary>评论次数</summary>
        public int CommentTimes { get; set; }
        public int CommentR1 { get; set; }
        public int CommentR2 { get; set; }
        public int CommentR3 { get; set; }
        public int CommentR4 { get; set; }
        public int CommentR5 { get; set; }
        /// <summary>平均评分</summary>
        public decimal CommentRate
        {
            get
            {
                if (CommentTimes > 0)
                {
                    if (ProductID.StartsWith("TR-", StringComparison.OrdinalIgnoreCase) || ProductID.StartsWith("LG-", StringComparison.OrdinalIgnoreCase))
                        return (CommentR1 + CommentR2 + CommentR3 + CommentR4 + CommentR5) / 5M / CommentTimes;
                    return CommentR1 * 1M / CommentTimes;
                }
                return 0;
            }
        }

        public ProductStatisticsModel() : base() { }
        public ProductStatisticsModel(System.Data.DataRow row) : base(row) { }
    }

    public class DefaultProductModel : SkuProductModel
    {
        public DefaultProductModel() : base() { }
        internal DefaultProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>重量(Weight)</summary>
        public string Weight { get; set; }
        /// <summary>颜色(Color)</summary>
        public string Color { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            Weight = row.GetValue("Weight");
            Color = row.GetValue("Color");
        }
    }


    public class AutoProductModel : DefaultProductModel
    {
        public IDictionary<string, string> AdditionalProperties { get; set; }

        public AutoProductModel() : base() { }
        internal AutoProductModel(System.Data.DataRow row) : base(row) { }

        protected override void Parse(System.Data.DataRow row)
        {
            base.Parse(row);

            if (row.HasValue("AdditionalProperties"))
                try
                {
                    AdditionalProperties = JsonConvert.DeserializeObject<IDictionary<string, string>>(row.GetValue("AdditionalProperties"));
                }
                catch { }
            if (AdditionalProperties == null)
                AdditionalProperties = new Dictionary<string, string>();
        }
    }

    public class MembraneProductModel : DefaultProductModel
    {
        public MembraneProductModel() : base() { }
        internal MembraneProductModel(System.Data.DataRow row) : base(row) { }
    }

    public class FilterElementProductModel : SkuProductModel
    {
        public FilterElementProductModel() : base() { }
        internal FilterElementProductModel(System.Data.DataRow row) : base(row) { }
        public string ShuXing1 { get; set; }
        public string ShuXing2 { get; set; }
        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;
            base.Parse(row);
            ShuXing1 = row.GetValue("CP_ShuXing1");
            ShuXing2 = row.GetValue("CP_ShuXing2");
        }
    }

    public class BatteryProductModel : SkuProductModel
    {
        public BatteryProductModel() : base() { }
        internal BatteryProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>电池信息(CP_Battery_Info)</summary>
        public string Infomation { get; set; }
        /// <summary>电池打分项-储电量(CP_Battery_SI_Capacity)</summary>
        public string Capacity { get; set; }
        /// <summary>电池打分项-冷启动(CP_Battery_SI_ColdBoot)</summary>
        public string ColdBoot { get; set; }
        /// <summary>电池打分项-便利性(CP_Battery_SI_Convenience)</summary>
        public string Convenience { get; set; }
        /// <summary>电池打分项-输出电流(CP_Battery_SI_Current)</summary>
        public string Current { get; set; }
        /// <summary>电池打分项-寿命(CP_Battery_SI_Life)</summary>
        public string Life { get; set; }
        /// <summary>电池打分项-安全性(CP_Battery_SI_Safety)</summary>
        public string Safety { get; set; }
        /// <summary>包装尺寸(CP_Battery_Size)</summary>
        public string Size { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            Infomation = row.GetValue("CP_Battery_Info");
            Capacity = row.GetValue("CP_Battery_SI_Capacity");
            ColdBoot = row.GetValue("CP_Battery_SI_ColdBoot");
            Convenience = row.GetValue("CP_Battery_SI_Convenience");
            Current = row.GetValue("CP_Battery_SI_Current");
            Life = row.GetValue("CP_Battery_SI_Life");
            Safety = row.GetValue("CP_Battery_SI_Safety");
            Size = row.GetValue("CP_Battery_Size");
        }
    }

    public class BrakeProductModel : SkuProductModel
    {
        public BrakeProductModel() : base() { }
        internal BrakeProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>刹车位置(前/后)(CP_Brake_Position)</summary>
        public string Position { get; set; }
        /// <summary>刹车片打分项-制动力(CP_Brake_SI_BrakeForce)</summary>
        public string BrakeForce { get; set; }
        /// <summary>刹车片打分项-踏板感觉(CP_Brake_SI_Comfortable)</summary>
        public string Comfortable { get; set; }
        /// <summary>刹车片打分项-寿命(CP_Brake_SI_Life)</summary>
        public string Life { get; set; }
        /// <summary>刹车片打分项-噪音(CP_Brake_SI_Noise)</summary>
        public string Noise { get; set; }
        /// <summary>刹车片打分项-掉灰/污染(CP_Brake_SI_Pollute)</summary>
        public string Pollute { get; set; }
        /// <summary>类型(片/蹄)(CP_Brake_Type)</summary>
        public string Type { get; set; }
        /// <summary>适配车型简述(CP_Brief_Auto)</summary>
        public string BriefAuto { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            Position = row.GetValue("CP_Brake_Position");
            BrakeForce = row.GetValue("CP_Brake_SI_BrakeForce");
            Comfortable = row.GetValue("CP_Brake_SI_Comfortable");
            Life = row.GetValue("CP_Brake_SI_Life");
            Noise = row.GetValue("CP_Brake_SI_Noise");
            Pollute = row.GetValue("CP_Brake_SI_Pollute");
            Type = row.GetValue("CP_Brake_Type");
            BriefAuto = row.GetValue("CP_Brief_Auto");
        }
    }

    public class OilProductModel : SkuProductModel
    {
        public OilProductModel() : base() { }
        internal OilProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>重量(Weight)</summary>
        public string Weight { get; set; }
        /// <summary>机油类型(CP_ShuXing1)</summary>
        public string ShuXing1 { get; set; }
        /// <summary>粘稠度(CP_ShuXing2)</summary>
        public string ShuXing2 { get; set; }
        /// <summary>机油等级(CP_ShuXing3)</summary>
        public string ShuXing3 { get; set; }
        /// <summary>机油级别(CP_ShuXing4)</summary>
        public string ShuXing4 { get; set; }
        /// <summary>机油品类(CP_Remark)</summary>
        public string Remark { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            Weight = row.GetValue("Weight");
            ShuXing1 = row.GetValue("CP_ShuXing1");
            ShuXing2 = row.GetValue("CP_ShuXing2");
            ShuXing3 = row.GetValue("CP_ShuXing3");
            ShuXing4 = row.GetValue("CP_ShuXing4");
            Remark = row.GetValue("CP_Remark");
        }
    }

    public class FilterProductModel : SkuProductModel
    {
        public FilterProductModel() : base() { }
        internal FilterProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>适配车型(CP_Brief_Auto)</summary>
        public string BriefAuto { get; set; }
        /// <summary>生产商产品编码2(CP_Filter_RefNo)</summary>
        public string RefNo { get; set; }
        /// <summary>滤清器类型(CP_Filter_Type)</summary>
        public string Type { get; set; }
        /// <summary>特别说明(特别说明)</summary>
        public string SpecialNote { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            BriefAuto = row.GetValue("CP_Brief_Auto");
            RefNo = row.GetValue("CP_Filter_RefNo");
            Type = row.GetValue("CP_Filter_Type");
            SpecialNote = row.GetValue("SpecialNote");
        }
    }

    public class CarBeautyProductModel : DefaultProductModel
    {
        public CarBeautyProductModel() : base() { }
        internal CarBeautyProductModel(System.Data.DataRow row) : base(row) { }

        public string CP_ShuXing1 { set; get; }
        public string CP_ShuXing2 { set; get; }
        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            CP_ShuXing1 = row.GetValue("CP_ShuXing1");
            CP_ShuXing2 = row.GetValue("CP_ShuXing2");
        }
    }

    public class TireProductModel : SkuProductModel
    {
        public TireProductModel() : base() { }
        internal TireProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>轮胎规格(CP_Tire_Width CP_Tire_AspectRatio CP_Tire_Rim)</summary>
        public TireSizeModel Size { get; set; }
        /// <summary>速度级别(CP_Tire_SpeedRating)</summary>
        public string SpeedRating { get; set; }
        /// <summary>载重指数(CP_Tire_LoadIndex)</summary>
        public string LoadIndex { get; set; }
        /// <summary>防爆(CP_Tire_ROF)</summary>
        public bool ROF { get; set; }
        /// <summary>类别(CP_Tire_Type)</summary>
        public string Type { get; set; }
        /// <summary>轮胎花纹(CP_Tire_Pattern)</summary>
        public string Pattern { get; set; }
        /// <summary>胎侧(CP_Tire_TextColor)</summary>
        public string TextColor { get; set; }
        /// <summary>Remark(CP_Tire_Remark)</summary>
        public string Remark { get; set; }

        #region SI
        /// <summary>转弯稳定性(CP_Tire_SI_CorneringStability)</summary>
        public string CorneringStability { get; set; }
        /// <summary>深雪地抓地(CP_Tire_SI_DeepSnowTraction)</summary>
        public string DeepSnowTraction { get; set; }
        /// <summary>干地抓地(CP_Tire_SI_DryTraction)</summary>
        public string DryTraction { get; set; }
        /// <summary>排水性(CP_Tire_SI_HydroplaningResistance)</summary>
        public string HydroplaningResistance { get; set; }
        /// <summary>冰地抓地(CP_Tire_SI_IceTraction)</summary>
        public string IceTraction { get; set; }
        /// <summary>浅雪地抓地(CP_Tire_SI_LightSnowTraction)</summary>
        public string LightSnowTraction { get; set; }
        /// <summary>静音性(CP_Tire_SI_NoiseComfort)</summary>
        public string NoiseComfort { get; set; }
        /// <summary>驾乘舒适性(CP_Tire_SI_RideComfort)</summary>
        public string RideComfort { get; set; }
        /// <summary>转向响应(CP_Tire_SI_SteeringResponse)</summary>
        public string SteeringResponse { get; set; }
        /// <summary>耐磨性(CP_Tire_SI_Treadwear)</summary>
        public string Treadwear { get; set; }
        /// <summary>湿地抓地(CP_Tire_SI_WetTraction)</summary>
        public string WetTraction { get; set; }
        #endregion

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            Size = new TireSizeModel(row);
            LoadIndex = row.GetValue("CP_Tire_LoadIndex");
            SpeedRating = row.GetValue("CP_Tire_SpeedRating");
            ROF = row.GetValue("CP_Tire_ROF") == "防爆";
            Type = row.GetValue("CP_Tire_Type");
            Pattern = row.GetValue("CP_Tire_Pattern");
            TextColor = row.GetValue("CP_Tire_TextColor");
            Remark = row.GetValue("CP_Tire_Remark");
            int result;
            if (int.TryParse(row.GetValue("ProductRefer"), out result))
                ProductRefer = result;
            if (row.HasValue("cy_marketing_price"))
                MarketingPrice = Convert.ToDecimal(row["cy_marketing_price"]);
            CorneringStability = row.GetValue("CP_Tire_SI_CorneringStability");
            DeepSnowTraction = row.GetValue("CP_Tire_SI_DeepSnowTraction");
            DryTraction = row.GetValue("CP_Tire_SI_DryTraction");
            HydroplaningResistance = row.GetValue("CP_Tire_SI_HydroplaningResistance");
            IceTraction = row.GetValue("CP_Tire_SI_IceTraction");
            LightSnowTraction = row.GetValue("CP_Tire_SI_LightSnowTraction");
            NoiseComfort = row.GetValue("CP_Tire_SI_NoiseComfort");
            RideComfort = row.GetValue("CP_Tire_SI_RideComfort");
            SteeringResponse = row.GetValue("CP_Tire_SI_SteeringResponse");
            WetTraction = row.GetValue("CP_Tire_SI_WetTraction");
        }
    }

    public class WiperProductModel : SkuProductModel
    {
        public WiperProductModel() : base() { }
        internal WiperProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>有无导流板(CP_Wiper_Baffler)</summary>
        public bool? Baffler { get; set; }
        /// <summary>系列(CP_Wiper_Series)</summary>
        public string Series { get; set; }
        /// <summary>雨刷原厂编号(CP_Wiper_OriginalNo)</summary>
        public string OriginalNo { get; set; }
        /// <summary>贴合(CP_Wiper_SI_Joint)</summary>
        public string Joint { get; set; }
        /// <summary>静音(CP_Wiper_SI_Silent)</summary>
        public string Silent { get; set; }
        /// <summary>耐磨(CP_Wiper_SI_Wearable)</summary>
        public string Wearable { get; set; }
        /// <summary>雨刷尺寸(CP_Wiper_Size)</summary>
        public string Size { get; set; }

        /// <summary>有无骨架(CP_Wiper_Stand)</summary>
        public bool? Stand { get; set; }
        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            if (row.HasValue("CP_Wiper_Baffler"))
                Stand = row.GetValue("CP_Wiper_Baffler") == "1";
            OriginalNo = row.GetValue("CP_Wiper_OriginalNo");
            Series = row.GetValue("CP_Wiper_Series");
            Joint = row.GetValue("CP_Wiper_SI_Joint");
            Silent = row.GetValue("CP_Wiper_SI_Silent");
            Wearable = row.GetValue("CP_Wiper_SI_Wearable");
            Size = row.GetValue("CP_Wiper_Size");
            if (row.HasValue("CP_Wiper_Stand"))
                Stand = row.GetValue("CP_Wiper_Stand") == "1";
        }
    }

    public class HubProductModel : SkuProductModel
    {
        public HubProductModel() : base() { }
        internal HubProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>轮毂尺寸(CP_Tire_Rim)</summary>
        public string Rim { get; set; }
        /// <summary>中心孔距(CP_Hub_CB)</summary>
        public string CB { get; set; }
        /// <summary>轮毂偏距(CP_Hub_ET)</summary>
        public string ET { get; set; }
        /// <summary>孔数(CP_Hub_H)</summary>
        public string H { get; set; }
        /// <summary>轮毂孔距(CP_Hub_PCD)</summary>
        public string PCD { get; set; }
        /// <summary>幅数(CP_Hub_Stand)</summary>
        public string Stand { get; set; }
        /// <summary>轮毂宽度(CP_Hub_Width)</summary>
        public string Width { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            base.Parse(row);

            Rim = row.GetValue("CP_Tire_Rim");
            CB = row.GetValue("CP_Tire_CB");
            ET = row.GetValue("CP_Tire_ET");
            H = row.GetValue("CP_Hub_H");
            PCD = row.GetValue("CP_Tire_PCD");
            Stand = row.GetValue("CP_Hub_Stand");
            Width = row.GetValue("CP_Hub_Width");
        }
    }

    /// <summary>{Width}/{AspectRatio}R{Rim}</summary>
    public class TireSizeModel : BaseModel
    {
        public TireSizeModel() : base() { }
        public TireSizeModel(System.Data.DataRow row) : base(row) { }
        public TireSizeModel(string width, string aspectRatio, string rim)
            : base()
        {
            this.Width = width;
            this.AspectRatio = aspectRatio;
            this.Rim = rim;
        }

        /// <summary>胎宽(CP_Tire_Width)</summary>
        public string Width { get; set; }
        /// <summary>扁平比(CP_Tire_AspectRatio)</summary>
        public string AspectRatio { get; set; }
        /// <summary>轮毂尺寸(CP_Tire_Rim)</summary>
        public string Rim { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            Width = row.GetValue("CP_Tire_Width");
            AspectRatio = row.GetValue("CP_Tire_AspectRatio");
            Rim = row.GetValue("CP_Tire_Rim");
        }

        public override bool Equals(object obj)
        {
            var that = obj as TireSizeModel;

            if (that == null)
                return false;

            return this.ToString() == that.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static TireSizeModel Parse(string size)
        {
            if (string.IsNullOrWhiteSpace(size))
                return null;

            var array = size.Split(new char[] { '/', 'R' });

            if (array.Length != 3)
                return null;

            return new TireSizeModel(array[0], array[1], array[2]);
        }

        public static bool operator ==(TireSizeModel objA, TireSizeModel objB)
        {
            return object.Equals(objA, objB);
        }
        public static bool operator !=(TireSizeModel objA, TireSizeModel objB)
        {
            return !object.Equals(objA, objB);
        }

        public override string ToString()
        {
            return string.Concat(this.Width, "/", this.AspectRatio, "R", this.Rim);
        }
    }
}
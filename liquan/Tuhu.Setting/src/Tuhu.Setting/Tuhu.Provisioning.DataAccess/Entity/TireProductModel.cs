namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class TireProductModel : ProductInformation
    {
        //重量
        public float Weight { get; set; }
        //颜色
        public string Color { get; set; }

        #region 规格
        public string CP_Tire_Width { get; set; }
        public string CP_Tire_AspectRatio { get; set; }
        public string CP_Tire_Rim { get; set; }

        #endregion

        //速度级别
        public string CP_Tire_SpeedRating { get; set; }
        //轮胎类别
        public string CP_Tire_Type { get; set; }
        public string Name { get; set; }
        //载重指数
        public string CP_Tire_LoadIndex { get; set; }
        //防爆
        public string ROF { get; set; }
        //产品产地
        public string CP_Place { get; set; }
        //轮胎花纹
        public string CP_Tire_Pattern { get; set; }

        //有无导流板(CP_Wiper_Baffler)
        public bool? CP_Wiper_Baffler { get; set; }

        //系列(CP_Wiper_Series)
        public string CP_Wiper_Series { get; set; }

        //雨刷尺寸(CP_Wiper_Size)
        public string CP_Wiper_Size { get; set; }

        //有无骨架(CP_Wiper_Stand)
        public bool? CP_Wiper_Stand { get; set; }
        //包装尺寸
        public string CP_Unit { get; set; }


        public string CP_ShuXing1 { get; set; }
        public string CP_ShuXing2 { get; set; }
        public string CP_ShuXing3 { get; set; }
        public string CP_ShuXing4 { get; set; }
        //中心孔距
        public string CP_Hub_CB { get; set; }
        //偏距
        public string CP_Hub_ET { get; set; }
        //孔数
        public string CP_Hub_H { get; set; }
        //孔距
        public string CP_Hub_PCD { get; set; }
        //幅数
        public string CP_Hub_Stand { get; set; }
        //宽度
        public string CP_Hub_Width { get; set; }
        //刹车位置
        public string CP_Brake_Position { get; set; }
        //刹车类型
        public string CP_Brake_Type { get; set; }
        //适配车型
        public string CP_Brief_Auto { get; set; }
        //电池尺寸
        public string CP_Battery_Size { get; set; }
        //电池信息
        public string CP_Battery_Info { get; set; }
        //类型
        public string CP_Filter_Type { get; set; }
        //特别说明
        public string SpecialNote { get; set; }
    }
}

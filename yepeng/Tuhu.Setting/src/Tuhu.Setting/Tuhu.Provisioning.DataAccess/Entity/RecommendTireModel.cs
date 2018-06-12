using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VehicleModel
    {
        /// <summary>
        /// 车型ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 车型第一级
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车型第二级
        /// </summary>
        public string Vehicle { get; set; }
    }

    public class VehicleTireModel
    {
        /// <summary>
        /// 车系(德系,日系)
        /// </summary>
        public string BrandCategory { get; set; }

        public string VehicleId { get; set; }
        /// <summary>
        /// 车型第一季
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 车型第二级
        /// </summary>
        public string Vehicle { get; set; }
        /// <summary>
        /// 车型原配轮胎
        /// </summary>
        public string Tires { get; set; }
        /// <summary>
        /// 车价格
        /// </summary>
        public string MinPrice { get; set; }
        /// <summary>
        /// 车型适配尺寸
        /// </summary>
        public string TiresMatch { get; set; }
        /// <summary>
        /// 推荐的尺寸
        /// </summary>
        public string TireSize { get; set; }
        public IEnumerable<VehicleTireRecommend> RecommendTires { get; set; }
        public IEnumerable<VehicleTireRecommend> RecommendTiresBI { get; set; }
    }

    public class VehicleTireRecommend
    {
        public int PKID { get; set; }
        /// <summary>
        /// 车型ID
        /// </summary>
        public string Vehicleid { get; set; }
        /// <summary>
        /// 推荐轮胎PID
        /// </summary>
        public string PID { get; set; }
        public string Reason { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Postion { get; set; }

        /// <summary>
        /// 推荐的尺寸
        /// </summary>
        public string TireSize { get; set; }

        public double Grade { get; set; }

        /// <summary>
        /// 轮胎品牌
        /// </summary>
        public string CP_Brand { get; set; }

        public IEnumerable<string> VehicleIDS { get; set; }
    }

    public class TireSizeModel
    {
        public string Width { get; set; }

        public string AspectRatio { get; set; }
        public string Rim { get; set; }

    }
    public class ForLogModel
    {
        public string AfterValue { get; set; }
        public string Vehicleid { get; set; }
        public string TireSize { get; set; }
        public string OldPID1 { get; set; }
        public string OldPID2 { get; set; }
        public string OldPID3 { get; set; }

        public string NewPID1 { get; set; }
        public string NewPID2 { get; set; }
        public string NewPID3 { get; set; }

        public string OldReason1 { get; set; }
        public string OldReason2 { get; set; }
        public string OldReason3 { get; set; }

        public string NewReason1 { get; set; }
        public string NewReason2 { get; set; }
        public string NewReason3 { get; set; }

        public string Group { get; set; }
    }

    public class VehicleTireRecommendLogModel
    {
        public string Author { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public string Operation { get; set; }
        public ForLogModel LogValue { get; set; }

    }


    public class SelectListModel
    {
        public int RowNumber { get; set; }
        public int PKID { get; set; }
        public string Reason { get; set; }
        public string BrandCategory { get; set; }
        public string Brand { get; set; }
        public string Vehicle { get; set; }
        public string Tires { get; set; }
        public string MinPrice { get; set; }
        public string TiresMatch { get; set; }
        public string VehicleId { get; set; }
        public string TireSize { get; set; }

        public string PID { get; set; }

        public int Postion { get; set; }

        public string ProductId { get; set; }

        public string CP_Brand { get; set; }
    }
    public class QZTJLogModel
    {
        public string Author { get; set; }

        public DateTime ChangeDateTime { get; set; }

        public string Operation { get; set; }
        public BeforeValueForQZTJ LogValue { get; set; }

    }

    public class QZTJSelectModel
    {
        /// <summary>
        /// 规格
        /// </summary>
        public string TireSize { get; set; }
        /// <summary>
        /// 原PID
        /// </summary>
        public string PID { get; set; }

        public string RecommendPID { get; set; }
        /// <summary>
        /// 品牌(分号分割)
        /// </summary>
        public string Brands { get; set; }
        /// <summary>
        /// 1全部显示 2显示已配置 3显示未配置 4显示已配置且PID重复
        /// </summary>
        public int ShowType { get; set; }

    }

    public class QZTJModel
    {
        public string PID { get; set; }

        public string DisplayName { get; set; }

        public string TireSize { get; set; }

        public IEnumerable<RecommendProductModel> Products { get; set; }
    }

    public class RecommendProductModel
    {
        public string RecommendPID { get; set; }
        public string PID { get; set; }
        public string Reason { get; set; }
        public string Image { get; set; }
        public string ProductName { get; set; }
        public int Postion { get; set; }
    }

    public class QZTJCountModel
    {
        public int AllCount { get; set; }
        public int YiPeiZhiCount { get; set; }
        public int RepeatCount { get; set; }
    }


    public class BeforeValueForQZTJ
    {
        public string PID { get; set; }
        public string OldPID1 { get; set; }
        public string OldPID2 { get; set; }
        public string OldPID3 { get; set; }
        public string NewPID1 { get; set; }
        public string NewPID2 { get; set; }
        public string NewPID3 { get; set; }

        public string OldReason1 { get; set; }
        public string OldReason2 { get; set; }
        public string OldReason3 { get; set; }
        public string NewReason1 { get; set; }
        public string NewReason2 { get; set; }
        public string NewReason3 { get; set; }

        public string OldImage1 { get; set; }
        public string OldImage2 { get; set; }
        public string OldImage3 { get; set; }
        public string NewImage1 { get; set; }
        public string NewImage2 { get; set; }
        public string NewImage3 { get; set; }

        public string Group { get; set; }

    }
    public class FORCache {
         public string PID { get; set; }
        public string RecommendPID { get; set; }
    }



}

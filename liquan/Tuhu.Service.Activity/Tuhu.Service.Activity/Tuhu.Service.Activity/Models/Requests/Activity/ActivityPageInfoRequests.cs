using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Requests.Activity
{
    public class ActivityPageInfoBaseRequest
    {
        public string HashKey { get; set; }
    }
    #region 活动页配置请求参数
    public class ActivityPageInfoRequest : ActivityPageInfoBaseRequest
    {
        public string Channel { get; set; }

        public Guid UserId { get; set; }
    }
    #endregion

    #region 各模块请求参数
    public class ActivityPageInfoModuleBaseRequest : ActivityPageInfoBaseRequest
    {
        public string Channel { get; set; }

        public List<string> RowNums { get; set; }
        public int Type { get; set; }
    }

    #region 车型头图
    public class ActivityPageInfoModuleVehicleBannerRequest : ActivityPageInfoModuleBaseRequest
    {
        public string VehicleId { get; set; }
    }
    #endregion

    #region 新商品池
    public class ActivityPageInfoModuleNewProductPoolRequest : ActivityPageInfoModuleVehicleBannerRequest
    {
        public string Tid { get; set; }

        #region 规格
        /// <summary>
        /// 胎宽
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 扁平比
        /// </summary>
        public string AspectRatio { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string Rim { get; set; }

        #endregion
    }
    #endregion

    #region 推荐模块
    public class ActivityPageInfoModuleRecommendRequest : ActivityPageInfoModuleProductPoolRequest
    {
        public Guid UserId { get; set; }

        public int RecommendCount { get; set; }
    }
    #endregion

    #region 商品池
    public class ActivityPageInfoModuleProductPoolRequest : ActivityPageInfoModuleNewProductPoolRequest
    {
        /// <summary>
        /// 城市id
        /// </summary>
        public string CityId { get; set; }

    }
    #endregion
    #endregion
    public enum ActivityPageConfigType
    {
        /// <summary>
        /// 普通商品类型
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 商品池
        /// </summary>
        ProductPool = 2,

        /// <summary>
        /// 新商品池
        /// </summary>
        NewProductPool = 3,

        /// <summary>
        /// 推荐商品
        /// </summary>
        Recommend = 4
    }
}

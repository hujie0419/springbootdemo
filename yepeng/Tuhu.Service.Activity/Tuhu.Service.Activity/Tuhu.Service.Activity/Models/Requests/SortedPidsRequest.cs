using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Enum;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class SortedPidsRequest
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public ProductType ProductType { get; set; }

        /// <summary>
        /// 活动id跟类型
        /// </summary>
        public KeyValuePair<string, ActivityIdType> DicActivityId { get; set; }


        /// <summary>
        /// 待排序的适配产品
        /// </summary>
        public List<string> AdaptPids { get; set; }

        /// <summary>
        /// 是否是刷新缓存
        /// </summary>
        public bool IsRefresh { get; set; }

        /// <summary>
        /// 将系统生成的活动id赋值到活动配置表
        /// </summary>
        public int NeedUpdatePkid { get; set; }
    }

    /// <summary>
    /// 产品类型
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// 默认值
        /// </summary>
        Default = -1,
        /// <summary>
        /// 轮胎
        /// </summary>
        Tires = 0,
        /// <summary>
        /// 车品
        /// </summary>
        AutoProduct = 1,
        /// <summary>
        /// 轮毂
        /// </summary>
        hub = 2,

    }
}

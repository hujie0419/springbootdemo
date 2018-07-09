using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class ActivtyPageRequest
    {
        public int Id { get; set; }
        public Guid ActivityId { get; set; }
        public string Channel { get; set; }
        public string OrderChannel { get; set; }
        public string HashKey { get; set; }
        public string VehiclId { get; set; }
        public string Tid { get; set; }
        public Guid UserId { get; set; }

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

        /// <summary>
        /// 城市Id
        /// </summary>
        public string CityId { get; set; }
    }

    public class ActivtyValidityRequest
        {/// <summary>
        /// /渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 活动id集合
        /// </summary>
        public List<string> HashKeys { get; set; }
    }
}

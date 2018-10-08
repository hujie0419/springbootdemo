using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PeccancyQueryModel
    {
        /// <summary>
        /// 违章查询省份配置
        /// </summary>
        public class PeccancyQueryProvinceModel
        {
            /// <summary>
            /// 省份Id
            /// </summary>
            public int ProvinceId { set; get; }

            /// <summary>
            /// 省份名称
            /// </summary>
            public string ProvinceName { set; get; }

            /// <summary>
            /// 省份简称
            /// </summary>
            public string ProvinceSimpleName { set; get; }
        }

        /// <summary>
        /// 地区Model--用以下拉框选项
        /// </summary>
        public class PeccancyRegionMiniModel
        {
            /// <summary>
            /// 地区Id
            /// </summary>
            public int RegionId { set; get; }

            /// <summary>
            /// 地区名称
            /// </summary>
            public string RegionName { set; get; }
        }

        /// <summary>
        /// 违章查询城市配置
        /// </summary>
        public class PeccancyQueryCityModel
        {
            /// <summary>
            /// 省份Id
            /// </summary>
            public int ProvinceId { set; get; }

            /// <summary>
            /// 省份名称
            /// </summary>
            [JsonIgnore]
            public string ProvinceName { set; get; }

            /// <summary>
            /// 省份简称
            /// </summary>
            [JsonIgnore]
            public string ProvinceSimpleName { set; get; }

            /// <summary>
            /// 城市Id
            /// </summary>
            public int CityId { set; get; }

            /// <summary>
            /// 城市代码
            /// </summary>
            public int CityCode { set; get; }

            /// <summary>
            /// 城市名称
            /// </summary>
            public string CityName { set; get; }

            /// <summary>
            /// 发动机号是否必须
            /// </summary>
            public bool NeedEngine { set; get; }

            /// <summary>
            /// 车架号是否必须
            /// </summary>
            public bool NeedFrame { set; get; }

            /// <summary>
            /// 发动机号长度
            /// </summary>
            public int EngineLen { set; get; }

            /// <summary>
            /// 车架号长度
            /// </summary>
            public int FrameLen { set; get; }

            /// <summary>
            /// 是否启用
            /// </summary>
            public bool? IsEnabled { set; get; }
        }
    }

    /// <summary>
    /// 违章配置日志
    /// </summary>
    public class PeccancyConfigOprLogModel
    {
        /// <summary>
        /// 日志表PKID
        /// </summary>
        public string PKID { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public string LogType { get; set; }
        /// <summary>
        /// 唯一识别标识
        /// </summary>
        public string IdentityId { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; }
        /// <summary>
        /// 操作前值
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 操作后值
        /// </summary>
        public string NewValue { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        [JsonIgnore]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
    }
}

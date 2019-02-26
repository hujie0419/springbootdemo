using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 易错车型文章配置
    /// </summary>
    public class VehicleArticleModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 车型VID
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 车型排量
        /// </summary>
        public string PaiLiang { get; set; }

        /// <summary>
        /// 车型年产
        /// </summary>
        public string Nian { get; set; }

        /// <summary>
        /// 发现文章链接
        /// </summary>
        public string ArticleUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

    /// <summary>
    /// 车型搜索
    /// </summary>
    public class VehicleSearchModel
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车型VID
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        public string PaiLiang { get; set; }

        /// <summary>
        /// 生产年份
        /// </summary>
        public string Nian { get; set; }

        /// <summary>
        /// 是否只显示配置的数据
        /// </summary>
        public bool IsOnlyConfiged { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 易错车型文章配置查询展示
    /// </summary>
    public class VehicleArticleViewModel : VehicleArticleModel
    {
        /// <summary>
        /// 车型品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车系
        /// </summary>
        public string VehicleSeries { get; set; }
    }

    /// <summary>
    /// 日志表
    /// </summary>
    public class VehicleArticleOprLogModel
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

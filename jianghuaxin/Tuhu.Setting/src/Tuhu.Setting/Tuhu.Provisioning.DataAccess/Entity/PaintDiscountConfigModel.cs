using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 喷漆打折
    /// </summary>
    public class PaintDiscountConfigModel
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 服务Pid
        /// </summary>
        public string ServicePid { get; set; }
        [JsonIgnore]
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 面数
        /// </summary>
        public int SurfaceCount { get; set; }
        /// <summary>
        /// 活动价格
        /// </summary>
        public decimal ActivityPrice { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动说明
        /// </summary>
        public string ActivityExplain { get; set; }
        /// <summary>
        /// 活动图片
        /// </summary>
        public string ActivityImage { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
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
    /// 喷漆服务
    /// </summary>
    public class PaintDiscountServiceModel
    {
        /// <summary>
        /// 服务Pid
        /// </summary>
        public string ServicePid { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
    }

    /// <summary>
    /// 年检操作日志表
    /// </summary>
    public class PaintDiscountOprLogModel
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

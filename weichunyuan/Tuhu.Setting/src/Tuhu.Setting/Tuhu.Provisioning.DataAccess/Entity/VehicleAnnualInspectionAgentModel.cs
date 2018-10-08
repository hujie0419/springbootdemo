using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    /// <summary>
    /// 车辆年检代办
    /// </summary>
    public class VehicleAnnualInspectionAgentModel : AnnualInspectionVenderModel
    {
        /// <summary>
        /// 自增主键
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
        /// 车牌前缀
        /// </summary>
        public string CarNoPrefix { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
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
    /// 供应商信息
    /// </summary>
    public class AnnualInspectionVenderModel
    {
        /// <summary>
        /// 供应商简称
        /// </summary>
        public string VenderShortName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string OfficeAddress { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string TelNum { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
    }

    /// <summary>
    /// 年检代办服务
    /// </summary>
    public class AnnualInspectionServiceModel
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
    /// 车牌对应城市
    /// </summary>
    public class LicensePlateLocalModel
    {
        /// <summary>
        /// 车牌前缀
        /// </summary>
        public string PlatePrefix { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
    }

    /// <summary>
    /// 年检操作日志表
    /// </summary>
    public class AnnualInspectionOprLogModel
    {
        [JsonIgnore]
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
        public DateTime CreateDateTime { get; set; }
        [JsonIgnore]
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}

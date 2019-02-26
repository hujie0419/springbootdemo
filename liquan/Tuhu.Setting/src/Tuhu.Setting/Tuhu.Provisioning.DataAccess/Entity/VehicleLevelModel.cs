using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VehicleLevelModel
    {
        public VehicleLevelModel()
        {
            PaintService = new List<PaintService>();
            VehicleInfo = new List<VehicleInfo>();
        }
        public string VehicleLevel { get; set; }

        public List<PaintService> PaintService { get; set; }

        public List<VehicleInfo> VehicleInfo { get; set; }
    }

    public class VehicleInfo
    {
        public string VehicleId { get; set; }

        public string VehicleSeries { get; set; }

        public bool IsChecked { get; set; }
        /// <summary>
        /// 喷漆类型  对应PaintVehicleLevelTypeEnum
        /// </summary>
        public string Type { get; set; }
    }
    /// <summary>
    /// 配置类型
    /// </summary>
    public enum PaintVehicleLevelTypeEnum
    {
        /// <summary>
        /// 线上配置
        /// </summary>
        Online,
        /// <summary>
        /// 线下配置
        /// </summary>
        Offline
    }


    /// <summary>
    /// 车型信息
    /// </summary>
    [Serializable]
    public class TreeItem
    {
        public TreeItem(){
           children = new List<TreeItem>();   
        }
        /// <summary>
        /// vehicleId
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 车系名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeItem> children { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public string check { get; set; }

        /// <summary>
        /// 是否禁止勾选
        /// </summary>
        public string disabled { get; set; }
    }

    /// <summary>
    /// 喷漆车型服务
    /// </summary>
    public class PaintService
    {
        public string VehicleLevel { get; set; }

        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public int DisplayIndex { get; set; }
    }

    public class VehicleLevelLog
    {
        public int PKID { get; set; }

        public string VehicleId { get; set; }

        public string VehicleLevel { get; set; }
        /// <summary>
        /// 喷漆类型  对应PaintVehicleLevelTypeEnum
        /// </summary>
        public string Type { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }

    public class PaintServiceLog
    {
        public int PKID { get; set; }

        public string   ServiceId { get; set; }

        public string VehicleLevel { get; set; }

        public int DisplayIndex { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }

    public class PaintVehicleOprLog
    {
        [JsonIgnore]
        public long PKID { get; set; }
        public string LogType { get; set; }
        public string VehicleLevel { get; set; }
        public string VehicleId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string OperateUser { get; set; }
        public string Remarks { get; set; }
        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        public PaintVehicleOprLog()
        {
            OldValue = "";
            NewValue = "";
            OperateUser = "";
            Remarks = "";
            VehicleLevel = "";
            VehicleId = "";
        }
    }

    public class VehicleLevelViewModel : VehicleLevelLog
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 汽车系列
        /// </summary>
        public string VehicleSeries { get; set; }
    }
}

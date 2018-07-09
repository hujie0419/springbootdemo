using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class WheelAdapterConfigQuery
    {
        /// <summary>
        /// TID
        /// </summary>
        public string TIDCriterion { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandCriterion { get; set; }
        /// <summary>
        /// 车系(Vehicle)
        /// </summary>
        public string VehicleCriterion { get; set; }
        /// <summary>
        /// 排量
        /// </summary>
        public string PaiLiangCriterion { get; set; }
        /// <summary>
        /// 生产年份
        /// </summary>
        public string YearCriterion { get; set; }
        /// <summary>
        /// 车款(TID)
        /// </summary>
        public string SalesNameCriterion { get; set; }
        /// <summary>
        /// 页面序号（1开始）
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示数据条数
        /// </summary>
        public int PageDataQuantity { get; set; }
        /// <summary>
        /// 查询出的总条数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 查询方式(0根据车型,1根据TID)
        /// </summary>
        public int QueryWay { get; set; }
        /// <summary>
        /// 轮毂信息是否已配置(1已配置,2未配置)
        /// </summary>
        public int? IsInfoSpecified { get; set; }
    }
    public class VehicleTypeInfo: WheelAdapterConfigWithTid
    {
        /// <summary>
        /// 主键 
        /// </summary>
        //public int PKId { get; set; }
        /// <summary>
        /// TID
        /// </summary>
        //public string TID { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 车系
        /// </summary>
        public string Vehicle { get; set; }
        /// <summary>
        /// 排量
        /// </summary>
        public string PaiLiang { get; set; }
        /// <summary>
        /// 生产年份
        /// </summary>
        public string Nian { get; set; }
        /// <summary>
        /// 车款
        /// </summary>
        public string SalesName { get; set; }
        
    }
    public class WheelAdapterConfigWithTid
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// TID
        /// </summary>
        public string TID { get; set; }
        /// <summary>
        /// PCD
        /// </summary>
        public string PCD { get; set; }
        /// <summary>
        /// 轮毂偏距
        /// </summary>
        public double? ET { get; set; }
        /// <summary>
        /// 最小轮毂偏距
        /// </summary>
        public double? MinET { get; set; }
        /// <summary>
        /// 最大轮毂偏距
        /// </summary>
        public double? MaxET { get; set; }
        /// <summary>
        /// CB
        /// </summary>
        public double? CB { get; set; }
        /// <summary>
        /// 螺栓螺母规格
        /// </summary>
        public string BoltNutSpec { get; set; }
        /// <summary>
        /// 螺栓螺母(B螺栓，N螺母)
        /// </summary>
        public string BoltNut { get; set; }
        /// <summary>
        /// 最小轮毂尺寸
        /// </summary>
        public double? MinWheelSize { get; set; }
        /// <summary>
        /// 最大轮毂尺寸
        /// </summary>
        public double? MaxWheelSize { get; set; }
        /// <summary>
        /// 最小轮毂宽度
        /// </summary>
        public double? MinWheelWidth { get; set; }
        /// <summary>
        /// 最大轮毂宽度
        /// </summary>
        public double? MaxWheelWidth { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
    }
    public class WheelAdapterConfigLog
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 对应WheelAdpterConfigWithTid的TID
        /// </summary>
        public string TID { get; set; }
        /// <summary>
        /// 操作类型(0创建，1修改)
        /// </summary>
        public int OperateType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string Operator { get; set; }
    }
    public class Str
    {
        public string str { get; set; }
        public string str1 { get; set; }
        public string str2 { get; set; }
    }
}

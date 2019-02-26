using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode
{

    /// <summary>
    /// 业务类型
    /// </summary>
    public enum BusinessType
    {
        None = 0,

        /// <summary>
        /// 保养通用流程
        /// </summary>
        GeneralBaoYang = 10001,

        /// <summary>
        /// 保养套餐
        /// </summary>
        BaoYangPackage = 20001,

        /// <summary>
        /// 喷漆通用流程
        /// </summary>
        GeneralPaint = 10002,

        /// <summary>
        /// 喷漆套餐
        /// </summary>
        PaintPackage = 20002,

        /// <summary>
        /// 年检通用流程
        /// </summary>
        GeneralAnnualInspection = 10003,

        /// <summary>
        /// 年检套餐
        /// </summary>
        AnnualInspectionPackage = 20003,
    }

    /// <summary>
    /// 结算方式
    /// </summary>
    public enum SettlementMethod
    {
        None = 0,
        BatchPreSettled = 10001,
        SinglePreSettled = 10002,
        ByPeriod = 10003,
    }

    /// <summary>
    /// 周期类型
    /// </summary>
    public enum CycleType
    {
        Day = 1,
        Week = 7,
        Month = 30,
        Year = 365,
    }

    /// <summary>
    /// 生成方式
    /// </summary>
    public enum GenerateType
    {
        None = 0,

        /// <summary>
        /// 接口生成
        /// </summary>
        Interface = 1,

        /// <summary>
        /// 运营配置
        /// </summary>
        Batch = 2,
    }

    public enum AudioStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Model
{
    public class UploadFileTask
    {
        public long PKID { get; set; }

        public string BatchCode { get; set; }

        public string FilePath { get; set; }

        public FileType Type { get; set; }

        public FileStatus Status { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public enum FileType
    {
        /// <summary>
        /// 大客户喷漆套餐,
        /// </summary>
        VipPaintPackage,
        /// <summary>
        /// 大客户保养套餐
        /// </summary>
        VipBaoYangPackage,
        /// <summary>
        /// 全车件导入
        /// </summary>
        VehicleLiYangId,
        /// <summary>
        /// LiYangId和LiYangLevelId对应关系
        /// </summary>
        LiYangId_LevelIdMap
    }
    public enum FileStatus
    {
        /// <summary>
        /// 准备
        /// </summary>
        Wait,
        /// <summary>
        /// 失败
        /// </summary>
        Failed,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel,
        /// <summary>
        /// 正在进行
        /// </summary>
        Runing,
        /// <summary>
        /// 已加载
        /// </summary>
        Loaded,
        /// <summary>
        /// 已修复
        /// </summary>
        Repaired,
        /// <summary>
        /// 待修复
        /// </summary>
        WaitingForRepair,
    }
}

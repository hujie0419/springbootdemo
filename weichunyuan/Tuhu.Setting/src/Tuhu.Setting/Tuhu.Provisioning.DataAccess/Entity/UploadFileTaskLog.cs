using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class UploadFileTaskLog
    {
        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string Type { get; set; }

        public string BatchCode { get; set; }

        public string Status { get; set; }

        public string CreateUser { get; set; }
    }

    public enum FileType
    {
        /// <summary>
        /// 大客户保养套餐
        /// </summary>
        VipBaoYangPackage,
        /// <summary>
        /// 大客户喷漆套餐
        /// </summary>
        VipPaintPackage
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

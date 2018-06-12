using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.TireSecurityCode
{
    public class TireSecurityCodeConfig
    {
        public int PKID { get; set; }
        public Guid CodeID { get; set; }
        public string SecurityCode { get; set; }
        public string UCode { get; set; }
        public string FCode { get; set; }
        public string BarCode { get; set; }
        public bool DataIntegrity { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
        public string BatchNum { get; set; }
    }

    public class InputBarCode
    {
        public string SecurityCode { get; set; }
        public string BarCode { get; set; }
        public string BarCodeBatchNum { get; set; }
    }

    public class TireSecurityCodeConfigQuery
    {
        public string SecurityCodeCriterion { get; set; }
        public string UCodeCriterion { get; set; }
        public string FCodeCriterion { get; set; }
        public string BarCodeCriterion { get; set; }
        public string GuidCriterion { get; set; }
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
    }

    public class UploadSecurityCodeLog
    {
        public string UploadFileName { get; set; }
        public string UploadFileAddress { get; set; }
        public string SuccessFileName { get; set; }
        public string SuccessFileAddress { get; set; }
        public string FailFileName { get; set; }
        public string FailFileAddress { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class UploadBarCodeLog
    {
        public string UploadFileName { get; set; }
        public string UploadFileAddress { get; set; }
        public string FailFileName { get; set; }
        public string FailFileAddress { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class LogSearchQuery
    {
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
    }
}

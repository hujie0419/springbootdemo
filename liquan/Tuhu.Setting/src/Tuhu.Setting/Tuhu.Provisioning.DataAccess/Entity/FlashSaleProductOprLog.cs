using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSaleProductOprLog
    {
        public string PKid { get; set; }
        public string LogType { get; set; }
        public string LogId { get; set; }
        public string BeforeValue { get; set; }
        public string AfterValue { get; set; }
        public string OperateUser { get; set; }
        public string Operation { get; set; }
        public DateTime CreateDateTime { get; set; }

        public string Remark { get; set; }
    }
}

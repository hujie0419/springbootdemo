using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class ImportBankMrActivityUsersRecordModel
    {
        public string OperateUser { get; set; }
        public string BatchCode { get; set; }
        public string ImportRoundType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(BatchCode))
                {
                    var RoundType = BatchCode.Split('|').LastOrDefault();
                    if (!string.IsNullOrWhiteSpace(RoundType))
                    {
                        switch (RoundType.Trim())
                        {
                            case "AllRound": return "全部";
                            case "CurrentRound": return "当前";
                            case "NextRound": return "下一场次";
                        }
                    }
                }
                return "未知";
            }
        }

        public IEnumerable<ImportRoundTime> RoundTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeString
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string Note { get; set; }

    }
    public class ImportRoundTime
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
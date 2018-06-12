using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode
{
    public class SearchRedemptionConfigRequest
    {
        public string GenerateType { get; set; }

        public string SettlementMethod { get; set; }

        public int CooperateId { get; set; }

        public int CodeTypeConfigId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int SettlementMethodType
        {
            get
            {
                switch (SettlementMethod)
                {
                    case "None":
                        return 1;//查询没有结算方式
                    case "BatchPreSettled":
                        return 2;//查询大结算方式
                    default:
                        return 0;//查询全部
                }
            }
        }
    }
}

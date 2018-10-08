using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.MMS.Web.Domain.UserFilter
{
    public class UserFilterRuleJobDetail
    {
        public UserFilterRuleJobDetail()
        {
            IsEffective = true;
        }
        public int PKID { get; set; }
        public int JobId { get; set; }
        public string TableName { get; set; }
        public string SearchKey { get; set; }
        public string SearchValue { get; set; }
        public JoinType JoinType { get; set; }
        public string BasicAttribute { get; set; }
        public string SecondAttribute { get; set; }
        public string BatchID { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEffective { get; set; }
        public CompareType CompareType { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}

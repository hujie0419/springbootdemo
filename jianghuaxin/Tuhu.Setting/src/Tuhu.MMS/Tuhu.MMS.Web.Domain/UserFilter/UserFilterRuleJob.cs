using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.MMS.Web.Domain.UserFilter
{
    public class UserFilterRuleJob
    {
        public int PKID { get; set; }
        public string JobName { get; set; }
        public string CreateUser { get; set; }
        public string ModifyUser { get; set; }
        public string Description { get; set; }
        public bool IsPreview { get; set; }
        public bool IsSubmit { get; set; }
        public JobStatus JobStatus { get; set; }
        public PreviewStatus PreviewStatus { get; set; }
        public int ResultCount { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public DateTime LastRunDateTime { get; set; }
        public string QuerySql { get; set; }
        public string ResultTables { get; set; }
        public string ResultSql { get; set; }
    }
}

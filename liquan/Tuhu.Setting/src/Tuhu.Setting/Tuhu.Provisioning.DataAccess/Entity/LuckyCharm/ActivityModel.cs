using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.LuckyCharm
{
    public class LuckyCharmActivityModel
    {
        public int PKID { get; set; }
        public int ActivityType { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivitySlug { get; set; }
        public string ActivityDes { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class LuckyCharmUserModel
    {
        public int PKID { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }


    public class LuckyCharmCondition : BaseQueryModel
    {
        public string AreaName { get; set; }
        public string Phone { get; set; }
        public int PKID { get; set; }
    }

    public class PageLuckyCharmUserModel
    {
        public int Total { get; set; }
        public List<LuckyCharmUserModel> Users { get; set; }

    }

    public class PageLuckyCharmActivityModel
    {
        public int Total { get; set; }
        public List<LuckyCharmActivityModel> Activitys { get; set; }

    }
}

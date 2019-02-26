using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    public class LuckyCharmActivityInfoResponse
    {
        public int PKID { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivitySlug { get; set; }
        public string ActivityDes { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class PageLuckyCharmUserResponse
    {
        public List<LuckyCharmUserInfoRespone> Users { get; set; }
        public int Total { get; set; }
    }

    public class LuckyCharmUserInfoRespone
    {
        public int PKID { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public ActivityStatusEnum status { get; set; }
    }

    public class PageLuckyCharmActivityResponse
    {
        public List<LuckyCharmActivityInfoResponse> Activitys { get; set; }
        public int Total { get; set; }
    }
}

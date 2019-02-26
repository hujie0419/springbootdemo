using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SemCampaignModel
    {
        public int PKID { get; set; }
        public string Source { get; set; }
        public string Medium { get; set; }
        public string Campaign { get; set; }
        public string Group { get; set; }
        public string Url { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }


    }
    public class AppDownloadCount
    {
        public int PKID { get; set; }
        public string ArticleTitle { get; set; }
        public string Channel { get; set; }
        public string OtherChannel { get; set; }
        public string AppUrl { get; set; }
        public string IsDeleted { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Deletor { get; set; }
        public DateTime? DeleteDateTime { get; set; }
        public string DownloadCount { get; set; }
    }

    public class AppDownloadHistory
    {
        public string Machine { get; set; }
        public string IP { get; set; }
        public string DownloadTime { get; set; }
    }
}

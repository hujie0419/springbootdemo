using System;
using Tuhu.Component.Common.Models;
namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class tbl_app_Versions
    {
        public int PKID { set; get; }
        public int? VersionCode { set; get; }
        public int MustUpdate { set; get; }
        public string Version_Number { set; get; }
        public DateTime CreateTime { set; get; }
        public DateTime LastUpdateTime { set; get; }
        public string Download { set; get; }
        public int Download_Number { set; get; }
        public string UpdateConnent { set; get; }
    }
    public class Versions : BaseModel
    {
        public string Version_number { set; get; }
        public int? VersionCode { set; get; }
        public bool MustUpdate { set; get; }
        public string CreateTime { set; get; }
        public string Download { set; get; }
        public string UpdateConnent { set; get; }
        public string Size { get; set; }
    }
}
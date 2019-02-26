using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class ActivityBuild
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ActivityUrl { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string BgImageUrl { get; set; }
        public string SBgImageUrl { get; set; }
        public string BgColor { get; set; }
        public string TireBrand { get; set; }
        public int? ActivityType { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DataParames { get; set; }
        public string BigActivityHome { get; set; }
        public string ActivityHome { get; set; }
        public int? MenuType { get; set; }
        public string ActivityMenu { get; set; }
        public string SelKeyName { get; set; }
        public List<string> SelKeyNameList
        {
            get
            {
                var result = new List<string>();
                if (!string.IsNullOrEmpty(SelKeyName))
                {
                    result = SelKeyName
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?
                        .Select(p => p.Trim())?
                        .ToList() 
                        ?? new List<string>();
                }
                return result;
            }
        }
        public string SelKeyImage { get; set; }
        public string HashKey { get; set; }
    }
}

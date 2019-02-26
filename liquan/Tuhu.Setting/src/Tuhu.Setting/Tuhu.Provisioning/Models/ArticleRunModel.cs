using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class SearchModel
    {
        public string PKID { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string status { get; set; }
        public int Type { get; set; }

        public int Category { get; set; }
    }

    public class SearchImgModel
    {
        public string Content { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string status { get; set; }
    }
}
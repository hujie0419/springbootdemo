using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class CommentReportModel
    {
        public SortedDictionary<string, int> TousuReportItems { get; set; }

        public int NotVerifiedCount { get; set; }

        public int NotPassCount { get; set; }

        public int PassCount { get; set; }

        public int OnlyRankedCount { get; set; }

        public int PhoneCount { get; set; }

        public int WebCount { get; set; }

        public int TotalCount
        {
            get
            {
                return NotVerifiedCount + NotPassCount + PassCount + OnlyRankedCount;
            }
        }
    }
}
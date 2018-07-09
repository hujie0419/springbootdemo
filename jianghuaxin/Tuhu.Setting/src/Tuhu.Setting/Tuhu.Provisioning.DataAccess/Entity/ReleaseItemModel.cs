using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ReleaseItemModel
    {
        public int PKID { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string Developer { get; set; }

        public string Tester { get; set; }

        public string Checker { get; set; }

        public DateTime? ReleaseTime { get; set; }

        public int IsValid { get; set; }

        public short Status { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Reason { get; set; }
    }

    public class TuhuReleaseModel
    {
        public int PKID { get; set; }

        public string Name { get; set; }

        public string Person { get; set; }

        public string Description { get; set; }

        public bool IsValid { get; set; }

    }
}

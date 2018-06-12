using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class CompanyUserSmsRecord
    {
        public int PKID { get; set; }

        public string Type { get; set; }

        public string BatchCode { get; set; }

        public int SmsTemplateId { get; set; }

        public string SmsMsg { get; set; }

        public DateTime SentTime { get; set; }

        public string Status { get; set; }

        public string CreatedUser { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}

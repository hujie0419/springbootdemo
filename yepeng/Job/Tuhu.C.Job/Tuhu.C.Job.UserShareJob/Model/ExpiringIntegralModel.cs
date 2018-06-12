using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.C.Job.UserShareJob.Model
{
    public class ExpiringIntegralModel
    {
        public Guid IntegralID { get; set; }
    }
    public class UserExpiringPointModel
    {
        public Guid UserId { get; set; }
        public int Integral { get; set; }
        public Guid IntegralId { get; set; }
    }
}

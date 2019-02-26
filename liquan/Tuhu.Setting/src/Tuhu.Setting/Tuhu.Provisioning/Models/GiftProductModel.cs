using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class GiftProductModel
    {
        public string Pid { get; set; }
        public int? Stock { get; set; }

        public int Status { get; set; }
    }
}
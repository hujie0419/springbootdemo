using System;
using System.Collections.Generic;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Provisioning.Models
{
   
    public class VueDiscountModel
    {
        public decimal Condition { get; set; }

        public int DiscountRate { get; set; }

        public string Tip { get; set; }
    }


}
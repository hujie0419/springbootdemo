using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class TousuTypeItem
    {
        public string DicType { get; set; }
        public string DicValue { get; set; }
        public string DicText { get; set; }
        public int TypeLevel { get; set; }
        public bool IsDelete { get; set; }
    }
}
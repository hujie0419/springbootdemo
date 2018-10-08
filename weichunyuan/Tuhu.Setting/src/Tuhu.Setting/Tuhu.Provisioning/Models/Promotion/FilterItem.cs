using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class FilterItem
    {
        public string Title { get; set; }
        public string ElementName { get; set; }
        public string ElementValue { get; set; }
        public string[,] Attributes { get; set; }
    }
}
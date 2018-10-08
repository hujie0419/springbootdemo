using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Product.Models;

namespace Tuhu.Provisioning.Models.CarProductModel
{ 
    public class ForIviewCascaderModel
    {
        public string value { get; set; }
        public string label { get; set; }
        public List<ForIviewCascaderModel> children { get; set; }
    }
}
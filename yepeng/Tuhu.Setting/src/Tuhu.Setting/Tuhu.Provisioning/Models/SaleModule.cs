using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class SaleModule
    {
        public int id { get; set; }
        public string mname { get; set; }
        public int mstatus { get; set; }
        public string banner { get; set; }

        public int dispbanner { get; set; }
        public int sort { get; set; }
        public DateTime createtime { get; set; }

        public DateTime updatetime { get; set; }

        public string grp { get; set; }

        public string device { get; set; }

        public DateTime stime { get; set; }

        public DateTime etime { get; set; }

        public int parentid { get; set; }

        private string _state;
        public string state { get { return this._state; } set {this._state = value; } }
       
    }


}
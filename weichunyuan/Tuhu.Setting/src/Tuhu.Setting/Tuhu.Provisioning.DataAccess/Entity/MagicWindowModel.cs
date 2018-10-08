using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MagicWindowModel : BaseModel
    {

        public MagicWindowModel() { }
        public MagicWindowModel(DataRow row) : base(row) { }
        public int PKID { get; set; }
        public string Url { get; set; }
        public bool IsEnable { get; set; }
        public string CreateTime { get; set; }
        public int Total { get; set; }

    }
}

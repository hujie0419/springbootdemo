using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CommentTousuType: BaseModel
    {
        public CommentTousuType() { }
        public CommentTousuType(DataRow row):base(row){ }
        public string DicType { get; set; }
        public string DicValue { get; set; }
        public int TypeLevel { get; set; }
        public string DicText { get; set; }
    }
}

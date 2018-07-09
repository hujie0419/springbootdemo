using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CommentReportItem : BaseModel
    {
        public CommentReportItem() { }
        public CommentReportItem(DataRow row) : base(row) { }
        public string DicText { get; set; }
        public int CommentStatus { get; set; }
        public int CommentChannel { get; set; }
        public int Count { get; set; }

    }
}

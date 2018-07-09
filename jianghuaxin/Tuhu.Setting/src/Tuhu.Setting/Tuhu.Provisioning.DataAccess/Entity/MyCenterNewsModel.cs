using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MyCenterNewsModel
    {
        #region MyCenterNewsModel

        public int PKID { get; set; }

        public string UserObjectID { get; set; }

        public string News { get; set; }

        public string Type { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Title { get; set; }

        public string HeadImage { get; set; }

        public string OrderID { get; set; }

        public bool isdelete { get; set; }

        public int ShopId { get; set; }

        public string IOSKey { get; set; }

        public string IOSValue { get; set; }

        public string androidKey { get; set; }

        public string androidValue { get; set; }

        public DateTime BeginShowTime { get; set; }

        #endregion
    }
}

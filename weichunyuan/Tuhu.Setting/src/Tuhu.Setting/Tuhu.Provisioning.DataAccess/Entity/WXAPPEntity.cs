using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class WXAPPAccessTokenEntity
    {
        public string access_token { get; set; }

        public string expires_in { get; set; }

        public string errcode { get; set; }

        public string errmsg { get; set; }


    }


    public class QCodeEntity
    {
        /// <summary>
        /// 小程序的appId
        /// </summary>
        public int WXAPPType { get; set; }

        public int Type { get; set; }

        public bool auto_color { get; set; }

        public ColorEntity line_color { get; set; }

        public string path { get; set; }

        public int width { get; set; }
    }

   public class ColorEntity
    {
        public int r { get; set; }

        public int g { get; set; }

        public int b { get; set; }

    }

}

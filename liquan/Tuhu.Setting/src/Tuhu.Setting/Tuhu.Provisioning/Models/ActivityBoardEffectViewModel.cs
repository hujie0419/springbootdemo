using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.Models
{
    public class ActivityBoardEffectViewModel
    {
        //所有平台
        public BIActivityPageModel AllPlatforms { get; set; }

        //IOS平台
        public List<BIActivityPageModel> IOSPlatforms { get; set; }

        //安卓平台
        public List<BIActivityPageModel> AndroidPlatforms { get; set; }

        //网站平台
        public List<BIActivityPageModel> WebPlatforms { get; set; }

        //微信平台
        public List<BIActivityPageModel> WeiXinPlatforms { get; set; }
    }
}
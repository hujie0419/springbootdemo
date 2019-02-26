using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Activity;

namespace Tuhu.Service.Activity.Models.Requests.Activity
{
   public  class ActivityPageModuleProductRequest: ActivityBaseModel
    {
        public ActivityPageConfigType Type { get; set; }
        public Dictionary<string, List<ActivityPageProductModel>> DicActivityPageProductModel { get; set; }
    }

    public class ActivityPageProductModel
    {
        public  Guid? ActivityId { get; set; }

        public string Pid { get; set; }

        public  bool IsImage { get; set; }
    }

    public class ActivityPageModuleRequest : ActivityBaseModel
    {
        public List<string> Groups { get; set; }
    }

    public class ActivityPageModuleMenuRequest : ActivityBaseModel
    {
        public List<int>FkActivityContentIds { get; set; }
    }

}

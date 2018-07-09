using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.C.Job.Initialization.Model
{
    public class OrderSimpleInfoModel
    {
        public int? InstallShopId { get; set; }
        public string DeliveryStatus { get; set; }
        public string InstallStatus { get; set; }
        public Guid? UserId { get; set; }
    }


    public class UserCarInfoModel
    {
        public string u_cartype_pid_vid { get; set; }
        public Guid? u_user_id { get; set; }
        public Guid CarId { get; set; }
    }


    public class WXTaskInfoModel
    {
        public Guid UserId { get; set; }
        public string BindType { get; set; }
    }


    public class BindWxInfoModel
    {
        public Guid? UserId { get; set; }
        public string UnionId { get; set; }
        public string AuthSource { get; set; }
        public string BindingStatus { get; set; }
    }
}

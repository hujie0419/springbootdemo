using System.Data;
using Tuhu.Models;

namespace Tuhu.C.Job.Models
{
    public class InsureCarModel : BaseModel
    {
        public string ExpireDay { set; get; }
        public string UserID { set; get; }
        public string C_u_InsureExpireDate { set; get; }
        public string u_carno { set; get; }
        public string u_mobile_number { set; get; }

    }
}

using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShopEditOprLog
    {
        public int PKID { get; set; }
        public int ShopID { get; set; }
        public string Author { get; set; }
        public string ObjectType { get; set; }
        public int ObjectID { get; set; }
        public string BeforeValue { get; set; }
        public string AfterValue { get; set; }
        public DateTime ChangeDatetime { get; set; }
        public string IPAddress { get; set; }
        public string Operation { get; set; }
        public string Remark { get; set; }

        public string EmployeeName { get; set; }
    }
}

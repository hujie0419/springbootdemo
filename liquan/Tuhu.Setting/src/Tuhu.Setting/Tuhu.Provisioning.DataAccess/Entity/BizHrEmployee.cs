using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizHrEmployee
    {
        public int PKID { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string EmailAddress { get; set; }
        public int WareHouseId { get; set; }
        public string WareHouseName { get; set; }
        public bool IsActive { get; set; }

        public List<WareHouseEmployee> WareHouseEmployees { get; private set; }

        public BizHrEmployee()
        {
            this.WareHouseEmployees = new List<WareHouseEmployee>();
        }
    }

    public class WareHouseEmployee
    {
        public string EmployeeName { get; set; }
        public bool IsActive { get; set; }
        public string EmailAddress { get; set; }
    }
}

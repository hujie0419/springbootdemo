namespace Tuhu.Provisioning.DataAccess.Entity
{
     public class HrEmploryee
    {

        #region Model

        /// <summary>
        ///
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 雇员姓名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Extention { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱唯一的
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RolesName { get; set; }

        public int PurcharId { get; set; }

        public string VendorName { get; set; }

        public int Num { get; set; }

        public string Name { get; set; }

        public decimal? TotalPrice { get; set; }

        public string PurcharRemark { get; set; }






        #endregion Model

    }
}

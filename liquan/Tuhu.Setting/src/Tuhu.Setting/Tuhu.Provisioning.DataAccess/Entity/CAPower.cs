namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CAPower
    {
        public int PKID { get; set; }

        public int ParentID { get; set; }

        public string LinkName { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Remark { get; set; }

        public string ParametersName { get; set; }

        public string ParametersValue { get; set; }

        public int? ORDERBY { get; set; }

        public string Type { get; set; }

        public bool IsActive { get; set; }

        public string UserNos { get; set; }

        public string BtnKey { get; set; }

        public byte BtnType { get; set; }

        public byte CgKey { get; set; }

        public string Module { get; set; }
    }
}

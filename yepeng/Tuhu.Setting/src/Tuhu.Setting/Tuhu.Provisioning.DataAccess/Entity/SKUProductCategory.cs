namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SKUProductCategory
    {
        public int Oid { get; set; }

        public int ParentOid { get; set; }

        public string CategoryName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string NodeNo { get; set; }

        public int DescendantProductCount { get; set; }
    }
}

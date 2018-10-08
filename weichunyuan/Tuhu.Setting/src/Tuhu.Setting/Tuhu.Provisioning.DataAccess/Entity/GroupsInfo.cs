namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class GroupsInfo
    {
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public int ChildId { get; set; }
        public string ChildName { get; set; }
    }
}

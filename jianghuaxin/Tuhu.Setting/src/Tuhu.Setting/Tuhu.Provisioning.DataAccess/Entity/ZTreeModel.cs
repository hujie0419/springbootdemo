using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 标签菜单
    /// </summary>
    [Serializable]
    public class ZTreeModel
    {
        public int id { get; set; }
        public int pId { get; set; }
        public string name { get; set; }
        public bool isChecked { get; set; }
        public bool open { get; set; }
        public bool chkDisabled { get; set; }
        public string CategoryName { get; set; }
        public string NodeNo { get; set; }
    }
}
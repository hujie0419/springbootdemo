using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary> 
    /// CategoryTag:实体类 
    /// </summary>  
    [Serializable]
    public class CategoryTagModel
    {
        #region HD_CategoryTag
        public int id { get; set; }
        public int parentId { get; set; }
        public bool isParent { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string describe { get; set; }
        public string image { get; set; }
        public bool disable { get; set; }
        public int sort { get; set; }
        public int type { get; set; }
        public int topicsId { get; set; }
        public bool isHomePage { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public string nickName { get; set; }
        public int showNum { get; set; }
        public bool isShowList { get; set; }
        #endregion
    }
}
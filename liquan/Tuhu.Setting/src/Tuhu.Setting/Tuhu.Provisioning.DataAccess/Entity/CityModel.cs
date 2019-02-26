using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 城市模型
    /// </summary>
    [Serializable]
    public class CityModel
    {
        public int PKID { get; set; }
        public string RegionName { get; set; }
        public int ParentID { get; set; }
    }
}

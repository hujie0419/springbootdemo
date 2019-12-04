using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 区域信息
    /// </summary>
    public class RegionEntity
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public int PKID {get;set;}
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get;set;}
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNO { get;set;}
        /// <summary>
        /// 区域等级，省市区3级
        /// </summary>
        public int Layer { get;set;}
        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentID { get;set;}
        /// <summary>
        /// 省ID
        /// </summary>
        public int ProvinceID { get;set;}
        /// <summary>
        /// 省名称
        /// </summary>
        public string ProvinceName { get;set;}
        /// <summary>
        /// 市ID
        /// </summary>
        public int CityID { get;set;}
        /// <summary>
        /// 市名称
        /// </summary>
        public string CityName { get;set;}
        /// <summary>
        /// 区/县ID
        /// </summary>
        public int AreaID { get;set;}
        /// <summary>
        /// 区/县名称
        /// </summary>
        public string AreaName { get;set;}
        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get;set;}
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 最后更新人
        /// </summary>
        public string UpdateUser { get; set; }
    }
}

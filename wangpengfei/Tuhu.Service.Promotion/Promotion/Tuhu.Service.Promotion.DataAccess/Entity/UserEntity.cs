using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 人员信息
    /// </summary>
    public partial class UserEntity
    {
        /// <summary>
        /// 主键自增列
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 省ID
        /// </summary>
        public int ProvinceID { get; set; }
        /// <summary>
        /// 市ID
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        /// 区ID
        /// </summary>
        public int AreaID { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }
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


    /// <summary>
    /// 自定义参数
    /// </summary>
    public partial class UserEntity
    {
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
    }
}

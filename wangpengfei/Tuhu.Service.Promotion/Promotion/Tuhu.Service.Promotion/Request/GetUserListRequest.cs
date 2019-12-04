using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
   public class GetUserListRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 页长
        /// </summary>
        public int PageSize { get; set; }
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
        /// 详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public int ProvinceID { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public int AreaID { get; set; }
        /// <summary>
        /// 省(只读)
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市(只读)
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区(只读)
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 省市区区(只读)
        /// </summary>
        public string Area { get { return this.ProvinceName + this.CityName + this.AreaName; } }
        /// <summary>
        /// 是否只查询人员数据
        /// </summary>
        public bool IsOnlyUser { get; set; }
    }
}

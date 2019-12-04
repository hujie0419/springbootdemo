using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 活动申请信息
    /// </summary>
    public class UserActivityApplyEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public int PKID {get;set;}
        /// <summary>
        /// 人员ID
        /// </summary>
        public int UserID { get;set;}
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get;set;}
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get;set;}
        /// <summary>
        /// 通过时间
        /// </summary>
        public DateTime PassTime { get;set;}
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get;set;}
        /// <summary>
        /// 逻辑删除
        /// </summary>
        public int IsDeleted { get;set;}
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
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class UserPermissionActivity: BaseModel
    {
        public UserPermissionActivity() : base() { }
        public UserPermissionActivity(DataRow row) : base(row) { }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
		
	
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
		/// <summary>
		/// 活动类型  null或0表示限时抢购 1表示闪购
		/// </summary>
		public int ActiveType { get; set; }
        /// <summary>
        /// 活动产品
        /// </summary>
        public IEnumerable<UserPermissionActivityProduct> Products { get; set; }

    

    }
}

using System;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    public class ActivityPageWhiteListModel
    {
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 用户白名单状态
        /// </summary>
        public int Status { get; set; }


        public Guid UserId { get; set; }
    }
}

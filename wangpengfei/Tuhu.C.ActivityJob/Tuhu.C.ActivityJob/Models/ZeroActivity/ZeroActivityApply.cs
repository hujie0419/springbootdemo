using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.ZeroActivity
{
    public class ZeroActivityApply
    {
        /// <summary>
        /// Activity.dbo.tbl_ZeroActivity_Apply 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string UserMobileNumber { get; set; }

    }
}

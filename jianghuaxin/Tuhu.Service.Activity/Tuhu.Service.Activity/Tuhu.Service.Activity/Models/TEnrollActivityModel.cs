using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class TEnrollActivityModel:BaseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 报名用户id
        /// </summary>
        public int EnrollUserId { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public int ActivityId { get; set; }

       
    }
}

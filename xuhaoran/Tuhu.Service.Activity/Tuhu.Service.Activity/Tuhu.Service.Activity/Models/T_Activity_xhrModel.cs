using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class T_Activity_xhrModel : BaseModel
    {

        /// <summary>活动ID</summary>
        [Column("ActivityID")]
        public int ActivityId { get; set; }
        /// <summary>活动标题</summary>
        public string Title { get; set; }
        /// <summary>活动内容</summary>
        public string ActivityContent { get; set; }
        /// <summary>开始时间</summary>
        public DateTime StartTime { get; set; }
        /// <summary>结束时间</summary>
        public DateTime EndTime { get; set; }
        /// <summary>备注</summary>
        public string Remark { get; set; }
        /// <summary>图片地址</summary>
        public string Picture { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }
        /// <summary>创建人</summary>
        public string CreateUser { get; set; }
        /// <summary>编辑时间</summary>
        public DateTime EditTime { get; set; }
        /// <summary>编辑人</summary>
        public string EditUser { get; set; }
        /// <summary>活动状态</summary>
        public int ActStatus { get; set; }
    }
}

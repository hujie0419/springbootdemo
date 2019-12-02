using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class T_ArearModel : BaseModel
    {
        /// <summary>区域</summary>
        [Column("Id")]
        public int AreaId { get; set; }
        /// <summary>区域名称</summary>
        public string ArearName { get; set; }
    }
}

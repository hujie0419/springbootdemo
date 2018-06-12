using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class QuestionAnsRequestModel
    {

     /// <summary>
     /// 用户userId
     /// </summary>
        public Guid userId { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public int pkid { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string resOptions { get; set; }

        /// <summary>
        /// HASH值
        /// </summary>
        public string hashKey { get; set; }

    }
}

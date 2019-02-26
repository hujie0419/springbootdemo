using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Areas.FeedBack.Models
{
    public class QuestionTypeModel
    {
        /// <summary>
        /// 问题类型Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 默认描述
        /// </summary>
        public string Describtion { get; set; }
        //类型名称

        public string TypeName { get; set; }
    }
}
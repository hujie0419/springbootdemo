using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Areas.FeedBack.Models
{
    public class FeedbackInfoRequest
    {
        //问题类型ID
        public int TypeId { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string UserPhone { get; set; }
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string FeedbackContent { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNum { get; set; }
        /// <summary>
        /// 手机型号
        /// </summary>
        public string PhoneModels { get; set; }
        /// <summary>
        /// 网络环境
        /// </summary>
        public string NetworkEnvironment { get; set; }
        /// <summary>
        ///图片
        /// </summary>
        public List<string> Images { get; set; }
    }

   
}
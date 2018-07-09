using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models.Push
{
    public class PushViewModel
    {
        [Required(ErrorMessage = "标题不能为空")]
        [Display(Name = "标题")]
        public string Title { get; set; }
        [Required(ErrorMessage = "内容不能为空")]
        [Display(Name = "内容")]
        public string Body { get; set; }

        [Required(ErrorMessage = "消息描述不能为空")]
        [Display(Name = "消息描述")]
        public string Description { get; set; }
        /// <summary>
        /// 推送类型 单播组播广播
        /// </summary> 
        public int PushType { get; set; }
        [Required]
        public string AppMsgType { get; set; }
       
        [Required(ErrorMessage = "发送目标不能为空")]
        [Display(Name = "发送目标")]
        public string Target { get; set; }
        public string BigImage { get; set; }
        public string AndroidExtra { get; set; }
        public string IOSExtra { get; set; }
        public string RichTextImage { get; set; }
        public DateTime Sendtime { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime AppExpireTime { get; set; }
        public DateTime CreateTime { get; set; }


    }
}
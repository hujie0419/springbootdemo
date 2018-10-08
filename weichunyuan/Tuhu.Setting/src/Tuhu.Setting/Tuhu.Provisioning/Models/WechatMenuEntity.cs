using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class WechatMenuListEntity
    {
        public string name { get; set; }
        /// <summary>
        /// 菜单的响应动作类型，view表示网页类型，click表示点击类型，miniprogram表示小程序类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 微信按钮键
        /// </summary>
        public string key { get; set; }
        public string url { get; set; }
        public string appid { get; set; }

        public string pagepath { get; set; }

        public List<WechatMenuEntity> sub_button { get; set; }

    }

    public class WechatMenuEntity
    {
        public string type { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public List<WechatMenuEntity> sub_button { get; set; }

        public string appid { get; set; }

        public string pagepath { get; set; }
        /// <summary>
        /// 微信按钮键
        /// </summary>
        public string key { get; set; }
    }
}
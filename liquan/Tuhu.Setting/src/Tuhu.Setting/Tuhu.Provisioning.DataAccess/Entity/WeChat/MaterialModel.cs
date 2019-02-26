using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MaterialModel
    {
        /// <summary>
        /// 图文标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图片/视频/语音的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 素材ID
        /// </summary>
        public string MediaID{ get; set; }
        /// <summary>
        /// 素材类型
        /// </summary>
        public string MediaType { get; set; }
        /// <summary>
        /// 小程序id
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 图片/视频/语音的URL
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public int PKID { get; set; }

        public int IsDeleted { get; set; }


    }
}
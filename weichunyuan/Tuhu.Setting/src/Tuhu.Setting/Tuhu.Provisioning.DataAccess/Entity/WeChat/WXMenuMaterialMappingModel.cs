using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class WXMenuMaterialMappingModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public long ObjectID { get; set; }
        /// <summary>
        /// 业务类型，click=点击菜单回复、keyword=关键字回复
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 微信原始ID
        /// </summary>
        public string OriginalID { get; set; }
        /// <summary>
        /// 微信菜单键
        /// </summary>
        public string ButtonKey { get; set; }
        /// <summary>
        /// 素材ID
        /// </summary>
        public long MaterialID { get; set; }
        /// <summary>
        /// 素材类型，image=图片、video=视频、voice=语音、news=图文、text=文字
        /// </summary>
        public string MediaType { get; set; }
        /// <summary>
        /// 微信素材ID
        /// </summary>
        public string MediaID { get; set; }
        /// <summary>
        /// 素材标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 素材图片路径
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 回复方式，1=回复全部、2=随机回复一条
        /// </summary>
        public int ReplyWay { get; set; }
        /// <summary>
        /// 是否需要延迟发送
        /// </summary>
        public bool IsDelaySend { get; set; }
        /// <summary>
        /// 间隔发送的小时
        /// </summary>
        public int IntervalHours { get; set; }
        /// <summary>
        /// 间隔发送的分钟
        /// </summary>
        public int IntervalMinutes { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; }
    }
}

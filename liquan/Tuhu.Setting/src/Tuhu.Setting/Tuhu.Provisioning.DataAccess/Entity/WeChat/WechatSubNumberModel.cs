using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Mapping;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class WechatSubNumberModel: WechartReplyBase
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 父菜单ID
        /// </summary>
        public int? ParentPKID { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单类型，view、click
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 网页URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 小程序ID
        /// </summary>
        public string Appid { get; set; }
        /// <summary>
        /// 小程序页面路径
        /// </summary>
        public string Pagepath { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? OrderBy { get; set; }
        /// <summary>
        /// 公众号原始ID
        /// </summary>
        public string OriginalID { get; set; }
        /// <summary>
        /// 微信按钮键
        /// </summary>
        public string ButtonKey { get; set; }
        
    }

    public class WechartReplyBase
    {
        /// <summary>
        /// 文字回复内容
        /// </summary>
        public List<WXMaterialTextModel> MaterialTextList { get; set; }
        /// <summary>
        /// 延迟文字回复内容
        /// </summary>
        public List<WXMaterialTextModel> DelayMaterialTextList { get; set; }
        /// <summary>
        /// 回复素材信息
        /// </summary>
        public List<MaterialModel> MaterialList { get; set; }
        /// <summary>
        /// 延迟回复素材信息
        /// </summary>
        public List<MaterialModel> DelayMaterialList { get; set; }
        /// <summary>
        /// 是否需要延迟发送
        /// </summary>
        public bool IsDelaySend { get; set; }
        /// <summary>
        /// 延迟时间
        /// </summary>
        public string IntervalTime { get; set; }
        /// <summary>
        /// 回复方式
        /// </summary>
        public int ReplyWay { get; set; }
        /// <summary>
        /// 延迟回复方式
        /// </summary>
        public int DelayReplyWay { get; set; }
    }
}

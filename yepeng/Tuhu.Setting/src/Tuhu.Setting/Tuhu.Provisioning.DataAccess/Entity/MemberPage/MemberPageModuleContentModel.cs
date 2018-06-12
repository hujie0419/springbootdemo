using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MemberPage
{
    public class MemberPageModuleContentModel
    {
        /// <summary>
        /// 主键标识
        /// </summary>
        public long PKID { get; set;}
        /// <summary>
        /// 内容名称
        /// </summary>
        public string ContentName { get; set; }
        /// <summary>
        /// 配置标识
        /// </summary>
        public long MemberPageID { get; set; }
        /// <summary>
        /// 模块标识
        /// </summary>
        public long MemberPageModuleID { get; set; }
        /// <summary>
        /// 展示类型，1=图文导航；2数字导航；3=图片广告；4=标题栏；5=底部栏
        /// </summary>
        public int ShowType { get; set; }
        /// <summary>
        /// 数据类型，1=普通；2=购物车；3=优惠券；4=收藏夹；5=浏览记录；6=积分；7=爱车档案
        /// </summary>
        public int DataType { get; set; }
        /// <summary>
        /// 支持渠道
        /// </summary>
        public string SupportedChannels { get; set; }
        /// <summary>
        /// 图片/图标路径
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 数字颜色
        /// </summary>
        public string NumColor { get; set; }
        /// <summary>
        /// 是否启用角标，ShowType=1（图文导航）时显示角标，ShowType=2（数字导航）时显示红点
        /// </summary>
        public bool IsEnableCornerMark { get; set; }
        /// <summary>
        /// 标题文案
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标题文案颜色
        /// </summary>
        public string TitleColor { get; set; }
        /// <summary>
        /// 描述文案
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 描述文案颜色
        /// </summary>
        public string DescriptionColor { get; set; }
        /// <summary>
        /// 跳转链接/跳转按钮/底部栏提示文案，跳转按钮在ModuleType=8（一行一列）时显示
        /// </summary>
        public string ButtonText { get; set; }
        /// <summary>
        /// 跳转链接/跳转按钮/底部栏提示文案颜色
        /// </summary>
        public string ButtonTextColor { get; set; }
        /// <summary>
        /// 跳转按钮颜色
        /// </summary>
        public string ButtonColor { get; set; }
        /// <summary>
        /// 背景色，除ShowType=3（图片广告）外都可有背景色
        /// </summary>
        public string BgColor { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayIndex { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 开始版本
        /// </summary>
        public string StartVersion { get; set; }
        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersion { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 渠道信息
        /// </summary>
        public List<MemberPageChannelModel> ChannelList { get; set; }
    }
}

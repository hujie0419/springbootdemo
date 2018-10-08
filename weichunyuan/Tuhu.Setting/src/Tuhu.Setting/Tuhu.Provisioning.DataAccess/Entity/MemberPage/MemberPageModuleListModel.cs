using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MemberPage
{
    public class MemberPageModuleListModel
    {
        /// <summary>
        /// 配置主表标识
        /// </summary>
        public long MemberPageID { get; set; }
        /// <summary>
        /// 模块标识
        /// </summary>
        public long MemberPageModuleID { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块类型，1=个人信息；2=订单；3=猜你喜欢；4=车型信息；5=会员推荐任务；6=标题栏；7=底部栏；8=一行一列；9=一行两列；10=一行四列
        /// </summary>
        public int ModuleType { get; set; }
        /// <summary>
        /// 上边距
        /// </summary>
        public int MarginTop { get; set; }
        /// <summary>
        /// 模块显示顺序
        /// </summary>
        public int ModuleDisplayIndex { get; set; }
        /// <summary>
        /// 内容标识
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 内容名称
        /// </summary>
        public string ContentName { get; set; }
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
    }
}

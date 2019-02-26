using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MemberPage
{
    public class MemberPageModuleModel
    {
        /// <summary>
        /// 主键标识
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 配置主表标识
        /// </summary>
        public long MemberPageID { get; set; }
        /// <summary>
        /// 页面编码，member=个人中心，more=更多
        /// </summary>
        public string PageCode { get; set; }
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
        /// 显示顺序
        /// </summary>
        public int DisplayIndex { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
    }
}

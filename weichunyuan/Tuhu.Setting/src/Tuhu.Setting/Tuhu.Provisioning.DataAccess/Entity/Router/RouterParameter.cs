using System;

namespace Tuhu.Provisioning.DataAccess.Entity.Router
{
    /// <summary>
    /// 待拼接的参数类
    /// </summary>
    public class RouterParameter
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String Discription{ get; set; }

        /// <summary>
        /// 类型
        /// 1.选择
        /// 2.文本
        /// 3.下拉框
        /// </summary>
        public int Kind { get; set; }

        /// <summary>
        /// 状态
        /// 1.选中
        /// 2.未选
        /// </summary>
        public int State { get; set; }

        /// <summary>
        ///跳转链接
        /// 用于发现文章
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// 链接类型
        /// </summary>
        public int LinkKind { get; set; }
    }
}

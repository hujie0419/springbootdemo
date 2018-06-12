using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShopAppBanner
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 位置或类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 主题名称
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 大标题
        /// </summary>
        public string MainTitle { get; set; }
        /// <summary>
        /// 小标题
        /// </summary>
        public string Contents { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 动作指令 1:内嵌webview中打开 2:唤起某app的某窗口 3;浏览器中打开 
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 有效期开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 有效期结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BgColor { get; set; }
        /// <summary>
        /// 图片类型
        /// </summary>
        public string ImgType { get; set; }
        /// <summary>
        /// 1x图片资源地址
        /// </summary>
        public string ImgUrl1 { get; set; }
        /// <summary>
        /// 2x图片资源地址
        /// </summary>
        public string ImgUrl2 { get; set; }
        /// <summary>
        /// 3x图片资源地址
        /// </summary>
        public string ImgUrl3 { get; set; }
        /// <summary>
        /// 4x图片资源地址
        /// </summary>
        public string ImgUrl4 { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 发布人 （操作人）
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 最后编辑时间 (更新时间)
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string PushType { get; set; }

    }
}

using System;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{

    public class ArticleComment : BaseModel
    {
        public ArticleComment() { }
        public ArticleComment(DataRow row) : base(row) { }
        /// <summary>评论主键</summary>
        public long ID { get; set; }
        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>ArticleID </summary>
        public int PKID { get; set; }

        /// <summary>评论层级ID</summary>
        public int TopID { get; set; }

        /// <summary>文章类目 </summary>
        public string Category { get; set; }
        /// <summary>文章标题 </summary>
        public string Title { get; set; }
        /// <summary>用户手机号 </summary>
        public string PhoneNum { get; set; }
        /// <summary>用户ID </summary>
        public string UserID { get; set; }
        /// <summary>用户头像 </summary>
        public string UserHead { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public string UserGrade { get; set; }
        /// <summary>评论内容 </summary>
        public string CommentContent { get; set; }
        /// <summary>评论时间 </summary>
        public DateTime CommentTime { get; set; }
        /// <summary>
        /// 审核状态 
        ///		0：待审核
        ///		1：审核不通过
        ///		2：审核通过
        /// </summary>
        public int AuditStatus { get; set; }
        /// <summary>审核时间 </summary>
        public DateTime AuditTime { get; set; }
        /// <summary>
        /// 评论排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        ///用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 评论类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 点赞数 
        /// </summary>
        public int Praise { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 评论图片
        /// </summary>
        public string CommentImage { get; set; }

        /// <summary>
        /// 上级类型
        /// </summary>
        public int PKIDType { get; set; }

        /// <summary>
        /// 通知用户信息
        /// </summary>
        public string UserInfos { get; set; }

        /// <summary>
        /// 信息评论类型
        /// </summary>
        public int PType { get; set; }
    }
}

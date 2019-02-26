using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    /// <summary>
    /// 文章评论表
    /// </summary>
    [Table("tbl_Comment")]
    public class Comment
    {
        public Comment()
        {
            this.AuditStatus =Convert.ToInt16(CommentStatus.WaitCheck);
            this.CommentTime = DateTime.Now;
        }
        /// <summary>
        /// 评论ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PKID { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [MaxLength(2000)]
        public string CommentContent { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CommentTime { get; set; }

        /// <summary>
        /// 评论状态
        /// </summary>

        public int AuditStatus { get; set; }

        /// <summary>
        /// 评论人Id
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// 回复评论ID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 评论文章Id
        /// </summary>
        public int Praise { get; set; }


        /// <summary>
        /// 回复评论
        /// </summary>       
        [NotMapped]
        public  Comment ReplyComment { get; set; }


        [NotMapped]
        public  Article CommentArticle { get; set; }

        [NotMapped]
        public virtual CommentUser User { get; set; }

    }



    public enum CommentStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        WaitCheck=0,
        /// <summary>
        /// 审核通过
        /// </summary>
        Pass=2,
        /// <summary>
        /// 审核不通过
        /// </summary>
        Illegal=1,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted=3
    }

    public enum TempStatus
    {

        /// <summary>
        /// 全部
        /// </summary>
        ALL,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitCheck,
        /// <summary>
        /// 已审核
        /// </summary>
        HaveChecked
    }

}

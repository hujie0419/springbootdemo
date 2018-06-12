using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework.Extensions;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class ArticleCommentModel : BaseModel
    {
        /// <summary>/// 主键编号/// </summary>
        public int ID { get; set; }
        /// <summary>/// 图文关联编号/// </summary>
        public int PKID { get; set; }
        /// <summary>/// 图文目录编号 1：汽车百科 2：驾驶技巧 3：保养秘诀 4：必备车品/// </summary>
        public string Category { get; set; }
        /// <summary>/// 图文标题/// </summary>
        public string Title { get; set; }
        /// <summary>/// 用户手机号/// </summary>
        public string PhoneNum { get; set; }
        /// <summary>/// 用户ID/// </summary>
        public string UserID { get; set; }
        /// <summary>/// 用户头像/// </summary>
        public string UserHead { get; set; }
        /// <summary>/// 评论内容/// </summary>
        public string CommentContent { get; set; }
        /// <summary>/// 我的评论内容/// </summary>
        public string MyCommentContent { get; set; }
        /// <summary>/// 评论时间/// </summary>
        public DateTime CommentTime { get; set; }


        /// <summary>
        /// 评论操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }

        /// <summary>/// 评论时间(一次性)/// </summary>
        public string CommentTimeForOne { get; set; }
        /// <summary>/// 审核状态 0：待审核 1：审核不通过 2：审核通过/// </summary>
        public int AuditStatus { get; set; }
        /// <summary>/// 审核时间/// </summary>
        public DateTime AuditTime { get; set; }
        /// <summary>/// 用户昵称/// </summary>
        public string UserName { get; set; }
        /// <summary>/// 用户等级/// </summary>
        public string UserGrade { get; set; }
        /// <summary>/// 真实姓名/// </summary>
        public string RealName { get; set; }
        /// <summary>/// 父级ID/// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 对问题(专题内)的一级回答
        /// </summary>
        public string TopId { get; set; }
        public string ParentName { get; set; }

        /// <summary>/// 性别/// </summary>
        public string Sex { get; set; }
        public int floor { get; set; }
        public int CommentNum { get; set; }

        public int CommentAll { get; set; }

        public int IsPraise { get; set; }
        public IEnumerable<CommentPraise> Item { get; set; }
        public int PraiseNum { get; set; }
        public ArticleCommentModel() : base() { }
        public ArticleCommentModel(System.Data.DataRow row) : base(row)
        {
            if (row.HasValue("CreatorInfo"))
            {
                var createInfo = row["CreatorInfo"];
                if (createInfo != null && string.IsNullOrEmpty(createInfo.ToString()) == false)
                {
                    var createInfoDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(createInfo.ToString());
                    UserHead = createInfoDic["UserHead"];
                    UserName = createInfoDic["UserName"];
                    UserGrade = createInfoDic["UserGrade"];
                }
            }

            if (string.IsNullOrEmpty(UserHead))
            {
              UserHead=  Component.Activity.BusinessData.UserHeadHelper.GetUserDefaultHead(UserGrade);
            }

        }

        /// <summary>
        /// 评论的类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///   评论内容的类型 文章1/问题3/专题2
        /// </summary>
        public int ArticleType { get; set; }
        /// <summary>
        /// 该条评论(点赞)是否已读
        /// </summary>
        public int IsRead { get; set; }
    }



    public class CommentPraise : BaseModel
    {

        public int PKID { get; set; }
        public int Id { get; set; }

        public string UserId { get; set; }

        public string TargetUserId { get; set; }
        public int CommentId { get; set; }

        public string UserHead { get; set; }

        public string Sex { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public string PhoneNum { get; set; }

        public string UserGrade { get; set; }
        public DateTime? CreateTime { get; set; }
        /// <summary>评论点赞状态 1 : 0/// </summary>
        public int VoteState { get; set; }

        public CommentPraise() : base() { }
        public CommentPraise(System.Data.DataRow row) : base(row) {
            if (string.IsNullOrEmpty(UserHead))
            {
                UserHead = Component.Activity.BusinessData.UserHeadHelper.GetUserDefaultHead("");
            }
        }

    }

}
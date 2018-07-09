using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.MyCenterNews;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class ArticleCommentController : Controller
    {
        private readonly string ObjectType = "ArtC";
        private readonly IArticleCommentManager manager = new ArticleCommentManager();
        private static readonly MyCenterNewsManager InvokeMyCenterNewsManager = new MyCenterNewsManager();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime? CommentTime, string Category, string Title,string CommentContent,string PhoneNum, int? AuditStatus, int Index = 1)
        {
            int _PageSize = 20;
            var _List = manager.SelectBy(_PageSize, Index, CommentTime.GetValueOrDefault(), Category, Title, CommentContent, PhoneNum, AuditStatus, Index);
            if (_List != null && _List.Count > 0)
            {
                return Json(new
                {
                    ArticleCommentList = _List.Select(p => new
                    {
                        p.ID,
                        p.PKID,
                        p.ParentID,
                        p.Category,
                        p.Title,
                        p.CommentContent,
                        p.PhoneNum,
                        CommentTime = p.CommentTime.ToString("yyyy-MM-dd HH:mm:00"),
                        AuditTime = p.AuditTime.ToString("yyyy-MM-dd HH:mm:00"),
                        p.AuditStatus,
                        p.Sort,
                        p.UserName,
                        p.Type,
                        p.Praise,
                        p.CommentImage,
                        p.PType,
                        p.TopID,
                        p.UserID
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ArticleCommentList = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetByPKID(int PKID,string Type)
        {
            var _List = manager.GetByPKID(PKID,Type).ToList();
            if (_List != null && _List.Count > 0)
            {
                return Json(new
                {
                    ArticleCommentList = _List.Select(p => new
                    {
                        p.ID,
                        p.PKID,
                        p.ParentID,
                        p.Category,
                        p.Title,
                        p.CommentContent,
                        p.PhoneNum,
                        CommentTime = p.CommentTime.ToString("yyyy-MM-dd HH:mm:00"),
                        AuditTime = p.AuditTime.ToString("yyyy-MM-dd HH:mm:00"),
                        p.AuditStatus,
                        p.Sort,
                        p.UserName,
                        p.Type,
                        p.Praise,
                        p.CommentImage,
                        p.PType,
                        p.TopID,
                        p.UserID
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ArticleCommentList = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string IDs,string Type,string Status, string PKIDs)
        {
            IEnumerable<int> ids = JsonConvert.DeserializeObject<IEnumerable<int>>(IDs);
            if (ids != null && ids.Count() > 0)
            {
                bool result = manager.DeleteBatch(ids);
                if (result)
                {
                    WriteOperatorLog(ids, "删除评论");
                    foreach (int id in ids)
                    {
                        MQMessageClient.DeleteMessageQueue(2, id.ToString());
                    }
                    if (!string.IsNullOrEmpty(Type) && Convert.ToInt32(Type) == 10&&Status=="已通过")
                    {
                        IEnumerable<int> pkids = JsonConvert.DeserializeObject<IEnumerable<int>>(PKIDs);
                        foreach (var pkid in pkids)
                        {
                            //晒图评论数+1
                            manager.UpdateShImgCommentCount(pkid, "-1");
                        }
                    }
                }
                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult Pass(string IDs,string UserID,string Type,string PKIDs)
        {
            IEnumerable<int> ids = JsonConvert.DeserializeObject<IEnumerable<int>>(IDs);
            if (ids != null && ids.Count() > 0)
            {
                bool result = manager.PassBatch(ids);
                if (result)
                {
                    //InsertMyCenterNewsAsync(ids);
                    WriteOperatorLog(ids, "评论通过");
                    foreach (int id in ids)
                    {
                        MQMessageClient.InsertMessageQueue(2, id.ToString(), UserID);   
                    }
                    if (!string.IsNullOrEmpty(Type) && Convert.ToInt32(Type) == 10)
                    {
                        IEnumerable<int> pkids = JsonConvert.DeserializeObject<IEnumerable<int>>(PKIDs);
                        foreach (var pkid in pkids)
                        {
                            //晒图评论数+1
                            manager.UpdateShImgCommentCount(pkid, "+1");
                        }
                    }
                }
                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult UnPass(string IDs, string Type, string PKIDs)
        {
            IEnumerable<int> ids = JsonConvert.DeserializeObject<IEnumerable<int>>(IDs);
            if (ids != null && ids.Count() > 0)
            {
                bool result = manager.UnPassBatch(ids);
                if (result)
                {
                    WriteOperatorLog(ids, "评论不通过");
                    foreach (int id in ids)
                    {
                        MQMessageClient.DeleteMessageQueue(2, id.ToString());
                    }

                    if (!string.IsNullOrEmpty(Type) && Convert.ToInt32(Type) == 10)
                    {
                        IEnumerable<int> pkids = JsonConvert.DeserializeObject<IEnumerable<int>>(PKIDs);
                        foreach (var pkid in pkids)
                        {
                            //晒图评论数-1
                            manager.UpdateShImgCommentCount(pkid, "-1");
                        }
                    }
                }
                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult UpdateSortBatch(string IDs, int sort)
        {
            IEnumerable<int> ids = JsonConvert.DeserializeObject<IEnumerable<int>>(IDs);
            if (ids != null && ids.Count() > 0)
            {
                bool result = manager.UpdateSortBatch(ids, sort);
                if (result)
                {
                    if (sort == 0)
                    {
                        WriteOperatorLog(ids, "评论取消置顶");
                    }
                    else
                    {
                        WriteOperatorLog(ids, "评论置顶");
                    }

                }
                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }

        private void WriteOperatorLog(IEnumerable<int> ids, string operation)
        {
            foreach (int id in ids)
            {
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, id, operation);
            }
        }

        /// <summary>
        /// 异步插入审核通知
        /// </summary>
        public void InsertMyCenterNewsAsync(IEnumerable<int> IDs)
        {
            try
            {
                Guid _UserObjectID = Guid.Empty;
                IEnumerable<ArticleComment> _ArticleComment = manager.GetByIDs(IDs);
                bool IsInsertNews = false,  //是否可以插入推送
                    IsQuestionReply = false; //是否为问题或提问的回复
                string _IOSKey = "", _IOSValue = "", _AndroidKey = "", _AndroidValue = "";

                foreach (var item in _ArticleComment)
                {
                    #region app key value 转换

                    if (item.PType == 0 && item.ParentID > 0)  //文章回复，只推送文章评论的回复
                    {
                        IsInsertNews = true;

                        _AndroidKey = "cn.TuHu.Activity.Found.DiscoveryH5Activity";
                        _AndroidValue = "[{'PKID':" + item.PKID + ",'Category':'" + item.Category + "','Title':'" + item.Title + "','keyboard':1,'AddClick':false}]";
                        _IOSKey = "THDiscoverCommentVC";
                        _IOSValue = "{ \"pkid\":" + item.PKID + ", \"categoryName\":\"" + item.Category + " \", \"mytitle\": \"" + item.Title + "\"}";
                    }
                    else if ((item.PType == 1 && item.ParentID > 0) || (item.PType == 2 && (item.ParentID > 0 && item.TopID > 0)))  //专题相关，排除专题提问本身
                    {
                        IsInsertNews = true;

                        if (item.ParentID > 0 && item.TopID > 0) { IsQuestionReply = true; } //专题问题评论的回复

                        _AndroidKey = "cn.TuHu.Activity.NewFound.Found.DiscoveryCommentResListAtivity";
                        _AndroidValue = "[{'answerId':" + item.ID + ",'questionType':4}]";
                        _IOSKey = "Tuhu.THAnswerVC";
                        _IOSValue = "{ \"answerID\":" + item.ID + ", \"type\":\"4\"}";
                    }
                    else if (item.PType == 2 && item.ParentID == 0 && item.TopID == 0)  //说说相关
                    {
                        //TODO: 暂无说说评论无推送
                    }
                    else if (item.PType == 3) //全局问题相关
                    {
                        IsInsertNews = true;

                        if (item.ParentID > 0 && item.TopID > 0) { IsQuestionReply = true; } //全局问题评论回复

                        _AndroidKey = "cn.TuHu.Activity.NewFound.Found.DiscoveryCommentResListAtivity";
                        _AndroidValue = "[{'answerId':" + item.ID + ",'questionType':3}]";
                        _IOSKey = "Tuhu.THAnswerVC";
                        _IOSValue = "{ \"answerID\":" + item.ID + ", \"type\":\"3\"}";
                    }
                    #endregion

                    if (IsInsertNews && !string.IsNullOrEmpty(item.UserInfos))
                    {
                        string[] UserInfosArr = item.UserInfos.Split('|');

                        if (UserInfosArr != null) { Guid.TryParse(UserInfosArr[1], out _UserObjectID); }

                        MyCenterNewsModel model = new MyCenterNewsModel()
                        {
                            UserObjectID = _UserObjectID.ToString("D"),
                            News = " ",
                            Type = "3私信",
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            Title = UserNameForPhone(item.UserName) + (item.PType == 0 ? " 回复了你的评论!" : (IsQuestionReply ? " 回复了你的发言!" : " 回答了你的提问!")),
                            HeadImage = item.UserHead,
                            isdelete = false,
                            IOSKey = _IOSKey,
                            IOSValue = _IOSValue,
                            androidKey = _AndroidKey,
                            androidValue = _AndroidValue
                        };
                        bool result = InvokeMyCenterNewsManager.Insert(model);
                    }
                }
            }
            catch { }
        }

        private string UserNameForPhone(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                if (userName.StartsWith("1") && userName.Length == 11)
                {
                    userName = userName.Substring(0, 3) + "****" + userName.Substring(7, 4);
                }
                return userName;
            }
            return userName ?? "";
        }
    }
}
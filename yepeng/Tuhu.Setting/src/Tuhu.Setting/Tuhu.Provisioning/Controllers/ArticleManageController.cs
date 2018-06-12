using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.FileUpload;
using swc = System.Web.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
//using Tuhu.WebSite.Component.SystemFramework.Log;
using Tuhu.Provisioning.Models;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;
using Tuhu.Provisioning.Business.Discovery;
using Newtonsoft.Json.Linq;
using System.Net;
using Tuhu.Service.Utility.Request;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.CommonServices;
using System.Configuration;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Tuhu.Provisioning.Controllers
{
    //[ExceptionLog]
    public class ArticleManageController : AsyncController
    {

        private readonly string ObjectType = "ArticleTblNew";

        // GET: ArticleManage
        public async Task<ActionResult> Index(int pageIndex = 1, string status = "All", int type = 5)
        {
            ArticleStatus articleStatus = (ArticleStatus)Enum.Parse(typeof(ArticleStatus), status);
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            //取文章
            var articles = await ArticleBll.SearchArticle(articleStatus, pager, null, null, type: type) ?? new List<Article>();
            ViewData["ArticleStatus"] = articleStatus;
            ViewData["PagerModel"] = pager;
            ViewBag.Type = type;
            return View(articles);
        }
        public async Task<ActionResult> Search(SearchModel model, int pageIndex = 1, int type = 5, string OperationType = "")
        {
            ArticleStatus articleStatus = (ArticleStatus)Enum.Parse(typeof(ArticleStatus), model.status);
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex
            };
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(model.StartDate))
            {
                startDate = DateTime.Parse(model.StartDate);
            }
            if (!string.IsNullOrEmpty(model.EndDate))
            {
                endDate = DateTime.Parse(model.EndDate);
            }
            if (String.Equals(OperationType, "导出"))
            {
                var workBook = new XSSFWorkbook();
                var sheet = workBook.CreateSheet();
                var row = sheet.CreateRow(0);
                var cell = null as ICell;
                var cellNum = 0;

                #region 参数拼装
                row.CreateCell(cellNum++).SetCellValue("文章ID");
                row.CreateCell(cellNum++).SetCellValue("文章标题");
                row.CreateCell(cellNum++).SetCellValue("文章阅读量");
                row.CreateCell(cellNum++).SetCellValue("文章评论量");
                row.CreateCell(cellNum++).SetCellValue("文章分享数");
                row.CreateCell(cellNum++).SetCellValue("文章收藏数");
                row.CreateCell(cellNum++).SetCellValue("发布时间");
                row.CreateCell(cellNum++).SetCellValue("作者");

                cellNum = 0;

                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);

                var articles = await ArticleBll.SearchArticle(articleStatus, pager, startDate, endDate, model.PKID, model.Title, type);

                if (articles != null && articles.Any())
                {
                    for (var i = 0; i < articles.Count(); i++)
                    {
                        cellNum = 0;
                        NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].PKID);
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].BigTitle);
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].ClickCount??0);
                        rowtemp.CreateCell(cellNum++).SetCellValue(ArticleBll.SelectCommentCount(articles[i].PKID));
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].ShareCountNum??0);
                        rowtemp.CreateCell(cellNum++).SetCellValue(ArticleBll.SelectVoteByArticleId(articles[i].PKID));
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].PublishDateTime.ToString());
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].CoverTag);
                    }
                }
                #endregion

                var ms = new MemoryStream();
                workBook.Write(ms);
                return File(ms.ToArray(), "application/x-xls", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
            }
            else
            {
                //取文章
                var articles = await ArticleBll.SearchArticle(articleStatus, pager, startDate, endDate, model.PKID, model.Title, type);
                ViewData["ArticleStatus"] = articleStatus;
                ViewData["PagerModel"] = pager;
                ViewData["SearchModel"] = model;
                ViewBag.Type = type;
                return View("Index", articles);
            }
        }
        public async Task<ActionResult> ArticleDetail(int id, string userId = null)
        {
            var articleModel = await ArticleBll.GetArticleDetailById(id, userId);
            if (articleModel == null)
                return HttpNotFound();
            if (!string.IsNullOrEmpty(articleModel.CategoryTags))
            {
                try
                {
                    ViewData["TagList"] = JsonConvert.DeserializeObject<List<JObject>>(articleModel.CategoryTags).Select(x => new Category() { Id = x.Value<int>("key"), Name = x.Value<string>("value"), Disable = x.Value<string>("isShow") == "1" ? true : false }).ToList();
                }
                catch
                {
                    ViewData["TagList"] = null;
                }
            }
            //相关阅读
            //var relatedList = await ArticleBll.GetRelateArticleByArticleId(id, new PagerModel() { CurrentPage = 1, PageSize = 5 });
            //ViewData["RelatedArticles"] = relatedList;

            return PartialView(articleModel);
        }

        #region 删除文章
        public JsonResult DeleteArticleByPkid(int id)
        {
            var result = ArticleBll.DeleteArticleByPkid(id);
            if (result)
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, id, "删除文章");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 发表文章

        public async Task<ActionResult> Publish()
        {
            try
            {
                var tags = await CategoryBll.GetAllCategory(false, true);
                ViewData["AllTags"] = tags;
                ViewData["AllAuthor"] = ArticleBll.SelectAll();
            }
            catch (Exception ex)
            {
                ViewData["AllTags"] = new List<Category>();
                ViewData["AllAuthor"] = new List<CoverAuthor>();
                //WebLog.LogException(ex);
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Publish(Article model, string CreateType)
        {

            if (CreateType == "存草稿")
            {
                model.Status = ArticleStatus.Saved.ToString();
                model.IsShow = 0;
                model.PublishDateTime = model.PublishDateTime ?? DateTime.Now;
            }
            else//发表。定时发表
            {
                model.Status = ArticleStatus.Published.ToString();
                model.IsShow = 1;
                if (CreateType != "定时发表")
                    model.PublishDateTime = DateTime.Now;
            }
            model.CreateDateTime = DateTime.Now;
            model.LastUpdateDateTime = DateTime.Now;
            model.IsTopMost = false;
            model.CategoryTags = CategoriesJson(model.CategoryTags);
            model.Content = model.ContentHtml;
            model.ContentUrl = string.Empty;
            model.IsDescribe = true;
            //无图模式
            if (model.CoverMode == CoverMode.NoPicMode.ToString())
            {
                model.Image = string.Empty;
                model.SmallImage = string.Empty;
                //model.ShowImages = "";
                //model.ShowType = 1;
            }
            //单图模式
            else if (model.CoverMode == CoverMode.OnePicSmallMode.ToString() || model.CoverMode == CoverMode.OnePicBigMode.ToString() || model.CoverMode == CoverMode.TopBigPicMode.ToString())
            {
                string firstImg = model.CoverImage.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).First();
                model.Image = firstImg;
                model.SmallImage = firstImg;
                model.ShowImages = GetShowImages(firstImg);
                model.ShowType = 1;
            }
            //三图模式
            else if (model.CoverMode == CoverMode.ThreePicMode.ToString() || model.CoverMode == CoverMode.BigPicLeftMode.ToString())
            {
                string[] coverImages = model.CoverImage.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                model.Image = coverImages.First();
                model.SmallImage = coverImages.First();
                model.ShowImages = GetShowImages(coverImages);
                model.ShowType = 3;
            }
            model.BigTitle = model.SmallTitle;
            if (model.IsShowFaxian == true)
                model.Type = 5;
            else
                model.Type = 99;
            var result = await ArticleBll.AddArticle(model);
            if (result != null)
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, result.PKID, "新增文章，文章状态：" + result.Status);

            return Redirect("/ArticleManage/Index");
        }

        #endregion

        #region 编辑文章
        public async Task<ActionResult> Modify(int id, string source = "Published")
        {
            var articleModel = await ArticleBll.GetArticleDetailById(id);
            try
            {
                ViewData["AllTags"] = await CategoryBll.GetAllCategory(false, true);
                ViewData["Source"] = source;
                ViewData["AllAuthor"] = ArticleBll.IsExistByName(articleModel.CoverTag) ? ArticleBll.SelectAll() : null;
            }
            catch (Exception ex)
            {
                ViewData["AllTags"] = new List<Category>();
                ViewData["AllAuthor"] = new List<CoverAuthor>();
                //WebLog.LogException(ex);
            }
            return View(articleModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Modify(Article model, string CreateType, bool? IsNowPublish = null, string ReferrerUrl = "")
        {
            if (IsNowPublish != null && IsNowPublish.Value)//立即发表
            {
                model.PublishDateTime = DateTime.Now;
            }

            if (CreateType == "存草稿")
            {
                model.Status = ArticleStatus.Saved.ToString();
                model.IsShow = 0;
            }
            else
            {
                model.Status = ArticleStatus.Published.ToString();
                model.IsShow = 1;
            }
            model.LastUpdateDateTime = DateTime.Now;
            model.CategoryTags = CategoriesJson(model.CategoryTags);
            model.Content = model.ContentHtml;
            model.BigTitle = model.SmallTitle;
            #region 图片处理
            //无图模式
            if (model.CoverMode == CoverMode.NoPicMode.ToString())
            {
                model.Image = string.Empty;
                model.SmallImage = string.Empty;
                //model.ShowImages = "";
                //model.ShowType = 1;
            }
            //单图模式
            else if (model.CoverMode == CoverMode.OnePicSmallMode.ToString() || model.CoverMode == CoverMode.OnePicBigMode.ToString() || model.CoverMode == CoverMode.TopBigPicMode.ToString())
            {
                string firstImg = model.CoverImage.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).First();
                model.Image = firstImg;
                model.SmallImage = firstImg;
                model.ShowImages = GetShowImages(firstImg);
                model.ShowType = 1;
            }
            //三图模式
            else if (model.CoverMode == CoverMode.ThreePicMode.ToString() || model.CoverMode == CoverMode.BigPicLeftMode.ToString())
            {
                string[] coverImages = model.CoverImage.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                model.Image = coverImages.First();
                model.SmallImage = coverImages.First();
                model.ShowImages = GetShowImages(coverImages);
                model.ShowType = 3;
            }
            #endregion
            if (model.IsShowFaxian == true)
                model.Type = 5;
            else
                model.Type = 99;
            var result = await ArticleBll.UpdateArticle(model);

            RefreshArticleCache(model.PKID);//刷文章的缓存

            if (!string.IsNullOrEmpty(ReferrerUrl))
                return Redirect(ReferrerUrl);

            if (result != null)
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, result.PKID, "修改文章，文章状态：" + result.Status);

            return Redirect("/ArticleManage/Index");
        }
        #endregion

        //撤回&恢复
        public async Task<ActionResult> ReturnBack(int pkid, string op)
        {
            var result = await ArticleBll.UpdateArticleStatus(pkid, op == "back" ? ArticleStatus.Withdrew : ArticleStatus.Published);
            RefreshTouTiaoListCache();//刷新头条列表缓存
            RefreshArticleCache(pkid);
            if (result != null)
            {
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, pkid, op == "back" ? "文章撤回" : "文章恢复");
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        //置顶&取消置顶
        public ActionResult TopMost(int pkid, string op)
        {
            var result = ArticleBll.UpdateArticleTopMost(pkid, op);
            RefreshTouTiaoListCache();//刷新头条列表缓存
            if (result)
            {
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, pkid, op == "top" ? "文章置顶" : "取消文章置顶");
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }
        public ActionResult LogList(string objectType, string objectID)
        {
            var list = LoggerManager.SelectOprLogByParams(objectType, objectID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CompareDateTime(string PublishTime)
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var pubTime = Convert.ToDateTime(PublishTime);
            var dateTime = Convert.ToDateTime(now);
            if (pubTime > dateTime)
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }


        #region 评论管理

        public async Task<ActionResult> CommentManage(int pageIndex = 1, string status = "ALL")
        {
            PagerModel pager = new PagerModel()
            {
                PageSize = 20,
                CurrentPage = pageIndex
            };
            List<Comment> result = null;
            if (status == TempStatus.ALL.ToString())//全部
            {
                result = await CommentBll.GetCommentListBy(s => s.AuditStatus != Convert.ToInt16(CommentStatus.Deleted), s => s.CommentTime, pager);
            }
            else if (status == TempStatus.WaitCheck.ToString())//待审核
            {
                result = await CommentBll.GetCommentListBy(s => s.AuditStatus == Convert.ToInt16(CommentStatus.WaitCheck), s => s.CommentTime, pager);
            }
            else if (status == TempStatus.HaveChecked.ToString())//已审核
            {
                result = await CommentBll.GetCommentListBy(s => s.AuditStatus == Convert.ToInt16(CommentStatus.Pass) || s.AuditStatus == Convert.ToInt16(CommentStatus.Illegal),
                    s => s.CommentTime, pager);
            }
            else
            {
                result = new List<Comment>();
            }
            var commentBLL = new CommentBll();
            if (result.Count > 0)
            {
                result.ForEach(u =>
                {
                    u.User = commentBLL.GetCommentUser(u.UserId.ToString());
                });
            }
            ViewData["CommentStatus"] = status;
            ViewData["PagerModel"] = pager;
            return View(result);
        }

        public async Task<ActionResult> CommentSearch(SearchModel model, int pageIndex = 1)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex
            };
            List<Comment> result = null;
            if (model.status == TempStatus.ALL.ToString())//全部
            {
                result = await CommentBll.GetCommentListSearch(s => s.AuditStatus != Convert.ToInt16(CommentStatus.Deleted), s => s.CommentTime, pager, model.Title);
            }
            else if (model.status == TempStatus.WaitCheck.ToString())//待审核
            {
                result = await CommentBll.GetCommentListSearch(s => s.AuditStatus == Convert.ToInt16(CommentStatus.WaitCheck), s => s.CommentTime, pager, model.Title);
            }
            else if (model.status == TempStatus.HaveChecked.ToString())//已审核
            {
                result = await CommentBll.GetCommentListSearch(s => (s.AuditStatus == Convert.ToInt16(CommentStatus.Pass) || s.AuditStatus == Convert.ToInt16(CommentStatus.Illegal)), s => s.CommentTime, pager, model.Title);
            }
            else
            {
                result = new List<Comment>();
            }
            var commentBLL = new CommentBll();
            if (result.Count > 0)
            {
                result.ForEach(u =>
                {
                    u.User = commentBLL.GetCommentUser(u.UserId.ToString());
                });
            }
            ViewData["CommentStatus"] = model.status;
            ViewData["PagerModel"] = pager;
            ViewData["SearchModel"] = model;
            return View("CommentManage", result);

        }

        public async Task<ActionResult> UpdateStatus(int cid, int status, string aid = "")
        {
            var commentModel = await CommentBll.GetCommentById(cid);
            if (commentModel == null)
                return Content("-1");
            commentModel.AuditStatus = status;
            int result = await CommentBll.Modify(commentModel, "Comment_Status");
            if (result > 0)
            {
                //if (status == CommentStatus.Pass.ToString()&&!string.IsNullOrEmpty(aid))//审核通过
                //{
                //    //评论数+1
                //    await ArticleBll.UpdateArticleCommentCount(int.Parse(aid));
                //}
                return Content("ok");
            }
            else
                return Content("fail");
        }

        #endregion

        #region 标签管理

        [HttpPost]
        public async Task<ActionResult> AddTag(Category model,string type="FaXian")
        {
            var result = false;
            if (type.Trim() == "FaXian")
            {
                model.Disable = false;
                var tagModel = await CategoryBll.AddCategory(model);
                if (tagModel != null) result = true;
            }
            else if (type.Trim() == "YouXuan")
            {
                result = CategoryBll.AddYouXuanCategory(model); ;
            }
            if (result)
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTag(Category model, string type = "FaXian")
        {
            var result = false;
            if (type.Trim() == "FaXian")
            {
                var tagModel = await CategoryBll.UpdateCategory(model);
                if (tagModel != null) result = true;
            }
            else if (type.Trim() == "YouXuan")
            {
                result = CategoryBll.UpdateYouXuanCategory(model); ;
            }

            if (result)
                return Content("ok");
            else
                return Content("error");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteTag(int cateId, string type = "FaXian")
        {
            var result = false;
            if (type == "FaXian")
            {
                var data = await CategoryBll.UpdateCategoryStatus(cateId, true);
                if (data > 0) result = true;
            }
            else if (type == "YouXuan")
            {
                result = CategoryBll.UpdateYouXuanCategoryStatus(cateId, true);
            }
            if (result)
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> RecoveryTag(int cateId, string type = "FaXian")
        {
            var result = false;
            if (type == "FaXian")
            {
                var data = await CategoryBll.UpdateCategoryStatus(cateId, false);
                if (data > 0) result = true;
            }
            else if (type == "YouXuan")
            {
                result = CategoryBll.UpdateYouXuanCategoryStatus(cateId, false);
            }
            if (result)
            {
                return Content("ok");
            }
            else
            {
                return Content("error");
            }
        }


        [HttpPost]
        public async Task<ActionResult> GetTagById(int id, string type = "FaXian")
        {
            var dic = new Dictionary<string, object>();
            Category tagModel = null;
            try
            {
                if (type.Trim() == "FaXian")
                {
                    tagModel = await CategoryBll.GetCategoryDetailById(id);
                }
                else if (type.Trim() == "YouXuan")
                {
                    tagModel = CategoryBll.GetYouXuanCategoryById(id);
                }
                if (tagModel != null)
                {
                    dic.Add("Data", tagModel);
                    dic.Add("Code", "1");
                }
                else
                {
                    dic.Add("Error", "null");
                    dic.Add("Code", "0");
                }
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Error", ex.Message);
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> TagList(string type = "FaXian")
        {
            List<Category> result = new List<Category>();
            ViewBag.Type = type;
            if (type.Trim() == "FaXian")
            {
                result = await CategoryBll.GetAllCategory(null, true);
            }
            else if (type.Trim() == "YouXuan")
            {
                result = CategoryBll.GetYouXuanCategoryList();
            }

            return View(result);
        }

        public async Task<ActionResult> ChildTagList(int id, string type = "FaXian")
        {
            Category parentModel = null;
            List<Category> result = new List<Category>();
            ViewBag.Type = type;
            if (type.Trim() == "FaXian")
            {
                parentModel = await CategoryBll.GetCategoryDetailById(id);
                result = await CategoryBll.GetChildrenCategoryByCategoryId(id);
            }
            else if (type.Trim() == "YouXuan")
            {
                parentModel = CategoryBll.GetYouXuanCategoryById(id);
                result = CategoryBll.GetYouXuanChildCategoryById(id);
            }

            ViewData["ParentModel"] = parentModel;
            return View(result);
        }
        #endregion

        #region 作者管理

        //作者管理
        public ActionResult AuthorList(int pageIndex = 1)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            var authorList = ArticleBll.SelectAuthorList(pager, "");
            ViewData["PagerModel"] = pager;
            return View(authorList);
        }
        [HttpPost]
        public ActionResult AddAuthor(CoverAuthor model)
        {
            model.IsDelete = false;
            bool isExist = ArticleBll.IsExistByName(model.AuthorName);
            if (isExist)
            {
                return Content("exist");
            }
            else
            {
                bool b = ArticleBll.AddAuthor(model);
                return b ? Content("ok") : Content("error");
            }
        }

        [HttpPost]
        public ActionResult UpdateAuthor(CoverAuthor model, string OldName)
        {
            bool isExist = ArticleBll.IsExistByName2(OldName, model.AuthorName);
            if (isExist)
            {
                return Content("exist");
            }
            else
            {
                bool b = ArticleBll.UpdateAuthor(model);
                return b ? Content("ok") : Content("error");
            }
        }
        public ActionResult DeleteAuthor(int AuthorID)
        {
            bool b = ArticleBll.DeleteByPKID(AuthorID);
            return b ? Content("ok") : Content("error");
        }
        #endregion


        #region 图片上传
        /// <summary>
        /// 图片上传地址
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="identifying">标识</param>
        /// <param name="urlStr">地址</param>
        /// <returns></returns>
        public ActionResult ImageUploadToAli(string from)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            //最大文件大小 5M
            int maxSize = 5000000;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                if (from == "article")
                {
                    if (Imgfile.InputStream == null || Imgfile.InputStream.Length > maxSize)
                    {
                        return Json(new
                        {
                            error = 1,
                            message = "图片大小超过5M限制"
                        }, "text/html");
                    }
                }
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);

                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        string dirName = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"];

                        var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            _BImage = result.Result;
                            //_SImage= ImageHelper.GetImageUrl(result.Result, 100);
                        }
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            string imgUrl = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _BImage;

            if (from == "article")
            {
                return Json(new
                {
                    error = 0,
                    url = imgUrl
                }, "text/html");
            }
            return Json(new
            {
                BImage = imgUrl,
                SImage = imgUrl,
                Msg = ex == null ? "上传成功" : ex.Message
            }, "text/html");
        }

        /// <summary>
        /// 字节流转换成图片
        /// </summary>
        /// <param name="byt">要转换的字节流</param>
        /// <returns>转换得到的Image对象</returns>
        public static Dictionary<string, object> BytToImg(byte[] byt)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(byt))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        Dictionary<string, object> dicToImg = new Dictionary<string, object>();
                        dicToImg.Add("Width", img.Width);
                        dicToImg.Add("Height", img.Height);
                        return dicToImg;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 预览
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> PreView(Article model, bool isList = true)
        {
            string fileUrl = "";
            try
            {
                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                model.PublishDateTime = DateTime.Now;
                model.Status = ArticleStatus.Saved.ToString();
                StringBuilder sbHtml = new StringBuilder();
                List<Category> tagList = JsonConvert.DeserializeObject<List<JObject>>(model.CategoryTags)
                        .Select(x => new Category()
                        {
                            Id = x.Value<int>("key"),
                            Name = x.Value<string>("value"),
                            Disable = x.Value<string>("isShow") == "1" ? true : false
                        }).ToList();
                foreach (var item in tagList)
                {
                    if (item.Disable)
                    {
                        sbHtml.AppendFormat("<span class='tag-item' data-itemId='{0}'>{1}</span>", item.Id, item.Name);
                    }
                }
                string guid = Guid.NewGuid().ToString();
                string tempPath = Server.MapPath(@"\Content\HtmlFile\Template.html");
                string tempHtml = System.IO.File.ReadAllText(tempPath);

                if (model.CoverMode != CoverMode.NoPicMode.ToString() && !string.IsNullOrEmpty(model.CoverImage))
                {
                    string titleHtml = @"<img src='{0}' alt='{1}'><div class='bannerCover'><img src='http://resource.tuhu.cn/Content/images/cover.png' ></div><p class='content title'>{2}</p>";
                    string topImage = model.CoverImage.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).First();
                    tempHtml = tempHtml.Replace("{{TitleClass}}", "banner").Replace("{{TitleContent}}", string.Format(titleHtml, topImage, model.SmallTitle, model.SmallTitle));
                }
                else
                {
                    tempHtml = tempHtml.Replace("{{TitleClass}}", "theme").Replace("{{TitleContent}}", model.SmallTitle);
                }
                string resultHtml = tempHtml.Replace("{{Title}}", model.SmallTitle)
                                            .Replace("{{Author}}", model.CoverTag)
                                            .Replace("{{CreateDateTime}}", model.CreateDateTime.ToString("MM-dd HH:mm"))
                                            .Replace("{{BriefContent}}", model.Brief)
                                            .Replace(" {{ArticleContent}}", model.ContentHtml)
                                            .Replace("{{TagContent}}", sbHtml.ToString());
                //.Replace("{{CoverTag}}", model.CoverTag)
                //.Replace("{{CreateDateTime}}", model.CreateDateTime.ToString("MM-dd HH:mm"))
                //.Replace(" {{ArticleContent}}", model.Content)
                //.Replace("{{TagContent}}", sbHtml.ToString())
                //.Replace("{{FileGuid}}", guid);

                //string filePath = Server.MapPath(string.Concat(@"\HtmlFile\temp\", guid, ".html"));
                #region 文件上传

                byte[] array = System.Text.Encoding.UTF8.GetBytes(resultHtml);

                //路径  /activity/ActivityHtml/;
                var ArticleFilePath = swc.WebConfigurationManager.AppSettings["UploadDoMain_news"];
                //地址  http://file.tuhu.test
                var ArticleAddress = swc.WebConfigurationManager.AppSettings["DoMain_news"];

                ArticleFilePath = ArticleFilePath + "Temp/preview_" + guid + "_" + model.PKID + ".html";

                var client = new WcfClinet<IFileUpload>();
                var result = client.InvokeWcfClinet(w => w.UploadFile(ArticleFilePath, array));
                fileUrl = ArticleAddress + ArticleFilePath;

                #endregion
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return Content(fileUrl);
        }

        public async Task<ActionResult> ListPreView(int id)
        {
            var articleModel = await ArticleBll.GetArticleDetailById(id);
            if (articleModel != null)
            {
                return await PreView(articleModel, true);
            }
            else
            {
                return Content("http://wx.tuhu.cn/");
            }
        }


        //删除预览文件
        public ActionResult DeleteViewFile(string fileGuid)
        {
            string filePath = Server.MapPath(string.Concat(@"\HtmlFile\temp\", fileGuid, ".html"));
            //判断文件是不是存在
            if (System.IO.File.Exists(filePath))
            {
                //如果存在则删除
                System.IO.File.Delete(filePath);
            }
            return Content("ok");
        }
        #endregion

        #region Utils
        /// <summary>
        /// 处理标签字符串
        /// </summary>
        /// <param name="cateStr">^2|汽车百科^11|东风日产</param>
        /// <returns></returns>
        private static string CategoriesJson(string cateStr)
        {
            List<object> tagList = new List<object>();
            if (!string.IsNullOrEmpty(cateStr))
            {
                string[] tagObjs = cateStr.Split('^');// ^2|汽车百科^11|东风日产
                foreach (string tag in tagObjs)
                {
                    if (!string.IsNullOrEmpty(tag))
                    {
                        var t = tag.Split('|');
                        tagList.Add(new { key = t[0], value = t[1], isShow = "1" });
                    }
                }
            }
            return JsonConvert.SerializeObject(tagList); ;
        }

        private static string GetShowImages(params string[] images)
        {
            List<Object> imgObj = new List<object>();
            foreach (string img in images)
            {
                imgObj.Add(new
                {
                    BImage = img,
                    SImage = img
                });
            }
            return JsonConvert.SerializeObject(imgObj);
        }

        #region 刷新缓存

        /// <summary>
        /// 刷某一篇文章的缓存
        /// </summary>
        /// <param name="articleId">文章Id</param>
        private void RefreshArticleCache(int articleId)
        {
            try
            {
                string domain = Request.Url.Host.Contains("tuhu.cn") ? "tuhu.cn" : "tuhu.test";
                string api = string.Format("http://faxian.{0}/Article/RefreshCache?name=ArticleDetail&key=DetailById/{1}", domain, articleId);
                using (WebClient client = new WebClient())
                {
                    string result = client.DownloadString(api);
                    RefreshResult res = JsonConvert.DeserializeObject<RefreshResult>(result);
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 刷头条列表文章的缓存（只刷一页）
        /// </summary>
        /// <param name="tagId">标签的Id</param>
        private void RefreshTouTiaoListCache(string tagId = "")
        {
            try
            {
                string domain = Request.Url.Host.Contains("tuhu.cn") ? "tuhu.cn" : "tuhu.test";
                string api = string.Format("http://faxian.{0}/Article/RefreshCache?name=TouTiao_List_Cache&key=ArticleList/1/20/{1}", domain, tagId);
                using (WebClient client = new WebClient())
                {
                    string result = client.DownloadString(api);
                    RefreshResult res = JsonConvert.DeserializeObject<RefreshResult>(result);
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 刷新专题列表
        /// </summary>
        private void RefreshSpecialColumnList()
        {
            try
            {
                string domain = Request.Url.Host.Contains("tuhu.cn") ? "tuhu.cn" : "tuhu.test";
                string api1 = string.Format("http://faxian.{0}/Article/RefreshCache?name=TouTiao_List_Cache&key=SpecialColumnList/1/20", domain);
                string api2 = string.Format("http://faxian.{0}/Article/RefreshCache?name=ZhuanTi_List_Cache&key=SpecialColumnList/1/20", domain);
                using (WebClient client = new WebClient())
                {
                    client.DownloadString(api1);
                    client.DownloadString(api2);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        public ActionResult ExecuteQueryForUpdate(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return Content("sql语句不能为空");
            return Content(ShareImageBLL.ExecuteSqlForUpdate(sql));
        }

        public async Task<ActionResult> SyncdataFromTblComment(string timestamp, string top = "200", string where = "")
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            if (timestamp != now)
                return Content("参数不正确");
            string reqUrl = @"https://www.tuhu.cn/Community/GetDataBySql.aspx?sql=";
            string sql = "SELECT TOP {0} * FROM Gungnir..tbl_Comment(NOLOCK) WHERE CommentType=3 AND CommentStatus=2  ";
            sql = string.Format(sql, top);
            if (!string.IsNullOrEmpty(where)) sql += where;
            try
            {
                string data = "";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    data = client.DownloadString(reqUrl + sql);
                }
                var jsonData = JsonConvert.DeserializeObject<List<JObject>>(data).Select(s => new
                {
                    CommentId = s.Value<int>("CommentId"),
                    SingleTitle = s.Value<string>("SingleTitle"),
                    CommentUserName = s.Value<string>("CommentUserName"),
                    CommentContent = s.Value<string>("CommentContent"),
                    CommentImages = s.Value<string>("CommentImages"),
                    CommentStatus = s.Value<int>("CommentStatus"),
                    CommentProductId = s.Value<string>("CommentProductId"),
                    CommentOrderId = s.Value<int>("CommentOrderId"),
                    CreateTime = s.Value<DateTime>("CreateTime"),
                    UpdateTime = s.Value<DateTime>("UpdateTime"),
                    TotalPraise = s.Value<object>("TotalPraise"),
                    CommentExtAttr = s.Value<string>("CommentExtAttr"),
                    ShopType = s.Value<string>("ShopType"),
                });
                Random rand = new Random();
                foreach (var item in jsonData)
                {
                    Article model = new Article();
                    model.SmallTitle = item.SingleTitle;
                    model.BigTitle = item.SingleTitle;
                    model.Content = GetContentHtml(item.CommentContent, item.CommentImages, item.CommentProductId);
                    model.ContentHtml = model.Content;
                    model.SmallImage = string.Empty;
                    model.Image = string.Empty;
                    model.ContentUrl = string.Empty;
                    model.CoverMode = CoverMode.NoPicMode.ToString();
                    model.CoverImage = string.Empty;
                    model.Brief = string.Empty;
                    model.CreateDateTime = item.CreateTime;
                    model.IsShow = 1;
                    model.IsDescribe = true;
                    model.LastUpdateDateTime = item.UpdateTime;
                    model.PublishDateTime = model.CreateDateTime;
                    model.Status = ArticleStatus.Published.ToString();
                    model.ClickCount = rand.Next(1, 5001); ;
                    model.Vote = !string.IsNullOrEmpty(item.TotalPraise.ToString()) ? Convert.ToInt32(item.TotalPraise) : 0;
                    model.ReadCountNum = 0;
                    model.ShareCountNum = 0;
                    model.CommentCountNum = 0;
                    model.CoverTag = GetCoverAuthor(item.CommentExtAttr, item.CommentUserName);
                    model.CategoryTags = "[{\"key\":\"11867\",\"value\":\"途虎众测\",\"isShow\":\"1\"}]";
                    model.IsTopMost = false;
                    model.Type = 5;
                    await ArticleBll.AddArticle(model);
                }
                return Content("数据全部更新成功");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> SyncdataUpdateArticleContent(string timestamp, string top = "200", string where = "")
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            if (timestamp != now)
                return Content("参数不正确");
            string reqUrl = @"https://www.tuhu.cn/Community/GetDataBySql.aspx?sql=";
            string sql = "SELECT TOP {0} * FROM Gungnir..tbl_Comment(NOLOCK) WHERE CommentType=3 AND CommentStatus=2  ";
            sql = string.Format(sql, top);
            if (!string.IsNullOrEmpty(where)) sql += where;
            try
            {
                string data = "";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    data = client.DownloadString(reqUrl + sql);
                }
                var jsonData = JsonConvert.DeserializeObject<List<JObject>>(data).Select(s => new
                {
                    CommentId = s.Value<int>("CommentId"),
                    SingleTitle = s.Value<string>("SingleTitle"),
                    CommentUserName = s.Value<string>("CommentUserName"),
                    CommentContent = s.Value<string>("CommentContent"),
                    CommentImages = s.Value<string>("CommentImages"),
                    CommentStatus = s.Value<int>("CommentStatus"),
                    CommentProductId = s.Value<string>("CommentProductId"),
                    CommentOrderId = s.Value<int>("CommentOrderId"),
                    CreateTime = s.Value<DateTime>("CreateTime"),
                    UpdateTime = s.Value<DateTime>("UpdateTime"),
                    TotalPraise = s.Value<object>("TotalPraise"),
                    CommentExtAttr = s.Value<string>("CommentExtAttr"),
                    ShopType = s.Value<string>("ShopType"),
                });
                foreach (var item in jsonData)
                {
                    var article = await ArticleBll.SelectArticleDetailByTitle(item.SingleTitle);
                    string content = GetContentHtml(item.CommentContent, item.CommentImages, item.CommentProductId);
                    ArticleBll.UpdateContentByPKID(article.PKID, content);
                }
                return Content("数据全部更新成功");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCoverAuthor(string CommentExtAttr, string userName)
        {
            if (string.IsNullOrEmpty(CommentExtAttr))
            {
                return "车主 " + (userName.IsCellphone() ? userName.Substring(0, 3) + "****" + userName.Substring(7, 4) : userName);
            }
            var jsonData = JsonConvert.DeserializeObject<JObject>(CommentExtAttr);
            string carTypeDes = jsonData.GetValue("CarTypeDes").ToString();
            if (!string.IsNullOrEmpty(carTypeDes))
            {
                string[] cars = carTypeDes.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                string chexing = cars.LastOrDefault();
                return chexing + "车主 " + (userName.IsCellphone() ? userName.Substring(0, 3) + "****" + userName.Substring(7, 4) : userName);
            }
            else
            {
                return "车主 " + (userName.IsCellphone() ? userName.Substring(0, 3) + "****" + userName.Substring(7, 4) : userName);
            }
        }
        public string GetContentHtml(string content, string images, string pids)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendFormat("<p>{0}</p>", content);
            string[] imgs = images.Split(';');
            if (imgs.Length > 0)
            {
                string imgHtml = "";
                foreach (string item in imgs)
                {
                    imgHtml += "<img src='" + item + "' alt='' />";
                }
                sbHtml.AppendFormat("<p>{0}</p>", imgHtml);
            }
            string[] proIds = pids.Split(';');
            string productHtml = GetProductHtml(proIds.FirstOrDefault());
            if (!string.IsNullOrEmpty(productHtml))
                sbHtml.AppendFormat("<p>{0}</p>", productHtml);
            return sbHtml.ToString();
        }
        public string GetProductHtml(string pid)
        {
            try
            {
                using (var client = new Tuhu.Service.Product.ProductClient())
                {
                    List<string> list = new List<string>();
                    list.Add(pid);
                    var proList = client.SelectSkuProductListByPids(list);
                    if (proList.Success && proList.Result.Count > 0)
                    {
                        var model = proList.Result.FirstOrDefault();
                        string name = model.DisplayName;
                        string imgUrl = "http://image.tuhu.cn" + model.Image + "@300w_300h_100Q.jpg";
                        decimal price = model.Price;
                        string[] splits = pid.Split('|');

                        string productHtml = "<div style=\"position: relative;padding:15px;margin: 20px 0;\" class=\"ke-divblock-tuhu\"><i style=\"position:absolute;width:15px;height:15px;border-style:solid;border-color:#d9d9d9;border-width:2px 0 0 2px;top:0;left:0;\"></i><i style= \"position:absolute;width:15px;height:15px;border-style:solid;border-color:#d9d9d9;border-width:0 2px 2px 0;right:0;bottom:0;\"></i>                                                <div style=\"border: 1px solid #eee;\"><img src=\"{0}\" data-ke-src=\"{0}\" style=\"display: block;width:100%;\" /><div style=\"width:94.6%;clear:both;display:table;background-color:#fafafa;padding:10px 15px;\" class=\"ke-divblock-delete\"><p style= \"color: #666;font-size: 14px;float:left;\">{1}<br /><span style= \"color: #df3448;font-size: 16px;\">&yen;{2}</span></p><a href=\"{3}\" data-ke-src=\"{3}\" style=\"float:right;width:75px;height:24px;border:1px solid #e74c3c;border-radius:5px;font-size:12px;color:#e74c3c;text-align:center;line-height:24px;margin-top: 3px;text-decoration: none;\">查看详情</a></div></div></div>";
                        //javascript:AllGoTo.InvokeApp('AP-HSC-YC-01','1','goods'); 
                        string productId = splits[0];
                        string vartiId = splits[1];
                        string type = productId.StartsWith("TR") ? "tire" : "goods";
                        string link = string.Format("javascript:AllGoTo.InvokeApp('{0}','{1}','{2}')", productId, vartiId, type);
                        return string.Format(productHtml, imgUrl, name, price.ToString("0.00"), link);
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        #region 晒图管理

        //晒图列表
        public ActionResult ShareImgManage(int pageIndex = 1, string status = "All")
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            string strWhere = string.Format("AND A.[status]='{0}' ", status);
            var shareList = ShareImageBLL.SelectShareList(pager, status != "All" ? strWhere : "");
            ViewData["DataStatus"] = status;
            ViewData["PagerModel"] = pager;

            return View(shareList);
        }

        //晒图搜索
        public ActionResult SearchShareImgs(SearchImgModel model, int pageIndex = 1)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            string strWhere = string.Format("AND A.content LIKE '%{0}%' ", model.Content);
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(model.StartDate))
            {
                startDate = DateTime.Parse(model.StartDate);
                strWhere += string.Format("AND A.createTime>'{0}' ", startDate);
            }
            if (!string.IsNullOrEmpty(model.EndDate))
            {
                endDate = DateTime.Parse(model.EndDate).AddDays(1);
                strWhere += string.Format("AND A.createTime<'{0}' ", endDate);
            }
            if (model.status != "All")
            {
                strWhere += string.Format("AND A.[status]='{0}' ", model.status);
            }
            var shareList = ShareImageBLL.SelectShareList(pager, strWhere);
            ViewData["DataStatus"] = model.status;
            ViewData["PagerModel"] = pager;
            ViewData["SearchModel"] = model;
            return View("ShareImgManage", shareList);
        }


        public ActionResult ShareImgEdit(int PKID)
        {
            var shareImageModel = ShareImageBLL.SelectShareDetailByPKID(PKID);
            return View(shareImageModel);
        }

        [HttpPost]
        public ActionResult ShareImgEdit(ShareImage model, string imgsInfo, string ReferrerUrl = "")
        {
            model.lastUpdateTime = DateTime.Now;

            string[] pics = imgsInfo.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            List<ImagesDetail> picDetails = new List<ImagesDetail>();

            foreach (string pic in pics)
            {
                string[] splits = pic.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                picDetails.Add(new ImagesDetail
                {
                    PKID = int.Parse(splits[0].Trim()),
                    isDelete = bool.Parse(splits[1].Trim())
                });
            }
            model.images = picDetails;
            ShareImageBLL.UpdateShareImages(model);
            if (!string.IsNullOrEmpty(ReferrerUrl))
                return Redirect(ReferrerUrl);
            return RedirectToAction("ShareImgManage");
        }

        //通过&不通过
        [HttpPost]
        public ActionResult CheckShareStatus(int PKID, bool isActive)
        {
            bool r = ShareImageBLL.UpdateStatus(PKID, isActive);
            if (r)
            {
                MQMessageClient.UpdateSTMessageQueue(11, PKID, isActive ? 1 : 0);
                return Content("ok");
            }
            else
                return Content("fail");
        }
        //加入黑名单
        [HttpPost]
        public ActionResult AddBlackList(UserBlackList model)
        {
            model.Operator = User.Identity.Name;
            model.CreateTime = DateTime.Now;
            model.State = true;
            bool r = UserBlackListBLL.AddBlackList(model);
            if (r)
                return Content("ok");
            else
                return Content("fail");
        }

        #endregion


        #region 专栏管理

        public ActionResult ColumnManage(int pageIndex = 1)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            List<SpecialColumn> columnList = SpecialColumnBLL.SelectColumnList(pager, "");
            ViewData["PagerModel"] = pager;
            return View(columnList);
        }

        public ActionResult AddColumn()
        {
            ViewData["ArticleTemp"] = ArticleBll.SelectArticleShow(100);
            return View();
        }

        [HttpPost]
        public ActionResult AddColumn(SpecialColumn model)
        {
            string[] articleIds = model.ArticleIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<ColumnArticle> articles = new List<ColumnArticle>();
            foreach (string id in articleIds)
            {
                ColumnArticle ca = new ColumnArticle();
                ca.PKID = int.Parse(id);
                articles.Add(ca);
            }
            model.Articles = articles;
            model.CreateTime = DateTime.Now;
            model.Creator = User.Identity.Name;

            bool r = SpecialColumnBLL.AddSpecialColumn(model);
            if (r)
                return RedirectToAction("ColumnManage");
            return Content("新增出错");
        }

        public ActionResult EditColumn(int id)
        {
            ViewData["ArticleTemp"] = ArticleBll.SelectArticleShow(100);
            SpecialColumn model = SpecialColumnBLL.SelectSpecialColumnByID(id);
            List<ColumnArticle> caList = SpecialColumnBLL.SelectArticleBySCID(model.ID);
            ViewData["ArticleHaved"] = caList.Count > 0 ? ArticleBll.SelectArticleByPKIDs(caList) : new List<ArticleTemp>();
            return View(model);
        }
        [HttpPost]
        public ActionResult EditColumn(SpecialColumn model)
        {
            string[] articleIds = model.ArticleIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<ColumnArticle> articles = new List<ColumnArticle>();
            foreach (string id in articleIds)
            {
                ColumnArticle ca = new ColumnArticle();
                ca.PKID = int.Parse(id);
                articles.Add(ca);
            }
            model.Articles = articles;
            bool r = SpecialColumnBLL.UpdateSpecialColumn(model);
            if (r)
                return RedirectToAction("ColumnManage");
            return Content("编辑出错");
        }

        //关键词过滤筛选
        public JsonResult GetArticleByWord(string word)
        {
            var list = ArticleBll.SelectArticleByWords(word);

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SetIsShow(int id, bool isShow)
        {
            bool r = SpecialColumnBLL.UpdateIsShow(id, isShow);
            RefreshSpecialColumnList();//刷新缓存
            return Content(r ? "ok" : "fail");
        }
        #endregion

        public JsonResult GetESArticleBykeyWord(string keyWord)
        {
            return Json(ArticleBll.GetESArticleBykeyWord(keyWord), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SyncArticleToES(bool isNew = false)
        {
            var result = await ArticleBll.SyncArticleToES(isNew);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetESArticleById(int id)
        {
            return Json(ArticleBll.GetESArticleById(id), JsonRequestBehavior.AllowGet);
        }

        #region 上传视频
        public JsonResult UploadVideo()
        {
            var file = Request.Files[0];
            var extension = Path.GetExtension(file.FileName);
            var stream = file.InputStream;
            var uploadDomain = "/videos/ArticleManager";
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var url = string.Empty;
            var frame = string.Empty;
            var result = FileUploadService.UploadVideo(buffer, extension, file.FileName, uploadDomain);
            if (result != null)
            {
                url = "https://img1.tuhu.org" + result?.Raw ?? string.Empty;
                frame = "https://img1.tuhu.org" + result.Frame ?? string.Empty;
            }
            return Json(new { url = url, frame = frame }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  优选文章
        public async Task<ActionResult> ArticleStatistic(string pkid, string title,
            DateTime? startDate, DateTime? endDate, int type = 9, int pageIndex = 1,
            string operationType = "")
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };
            if (String.Equals(operationType, "导出"))
            {
                var workBook = new XSSFWorkbook();
                var sheet = workBook.CreateSheet();
                var row = sheet.CreateRow(0);
                var cell = null as ICell;
                var cellNum = 0;

                #region 参数拼装
                row.CreateCell(cellNum++).SetCellValue("文章ID");
                row.CreateCell(cellNum++).SetCellValue("文章标题");
                row.CreateCell(cellNum++).SetCellValue("文章阅读量");
                row.CreateCell(cellNum++).SetCellValue("文章评论量");
                row.CreateCell(cellNum++).SetCellValue("文章分享数");
                row.CreateCell(cellNum++).SetCellValue("文章收藏数");
                row.CreateCell(cellNum++).SetCellValue("发布时间");
                row.CreateCell(cellNum++).SetCellValue("作者");

                cellNum = 0;

                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);
                sheet.SetColumnWidth(cellNum++, 18 * 256);

                var articles = await ArticleBll.SearchArticle(ArticleStatus.All, pager, startDate, endDate, pkid, title, type);

                if (articles != null && articles.Any())
                {
                    for (var i = 0; i < articles.Count(); i++)
                    {
                        cellNum = 0;
                        NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].PKID);
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].BigTitle);
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].ClickCount ?? 0);
                        rowtemp.CreateCell(cellNum++).SetCellValue(ArticleBll.SelectCommentCount(articles[i].PKID));
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].ShareCountNum ?? 0);
                        rowtemp.CreateCell(cellNum++).SetCellValue(ArticleBll.SelectVoteByArticleId(articles[i].PKID));
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].PublishDateTime.ToString());
                        rowtemp.CreateCell(cellNum++).SetCellValue(articles[i].CoverTag);
                    }
                }
                #endregion

                var ms = new MemoryStream();
                workBook.Write(ms);
                return File(ms.ToArray(), "application/x-xls", $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
            }
            else
            {
                var result = await ArticleBll.SearchArticle(ArticleStatus.All, pager,
                    startDate, endDate, pkid, title, type) ?? new List<Article>();
                ViewData["PagerModel"] = pager;
                ViewBag.Type = type;
                ViewBag.PKID = pkid;
                ViewBag.BigTitle = title;
                ViewBag.StartTime = startDate;
                ViewBag.EndTime = endDate;
                return View(result);
            }
        }

        public async Task<ActionResult> YouXuanArticle(int id = 0, string source = "Published")
        {
            YouXuanArticle result = new YouXuanArticle() { CoverConfig = new ArticleCoverConfig() };
            List<Category> categoryList = new List<Category>();
            List<CoverAuthor> authorList = new List<CoverAuthor>();
            categoryList = CategoryBll.GetYouXuanCategoryList();
            authorList = ArticleBll.SelectAll();
            ViewData["AllTags"] = categoryList;
            ViewData["AllAuthor"] = authorList;
            ViewData["Source"] = source;
            if (id > 0)
            {
                var data = await ArticleBll.GetArticleDetailById(id);
                var coverConfig = ArticleBll.SelectArticleCoverConfig(id)??new ArticleCoverConfig();
                result = new YouXuanArticle
                {
                    PKID = data.PKID,
                    Image = data.Image,
                    ShowImages = data.ShowImages,
                    ShowType = data.ShowType,
                    IsDescribe = data.IsDescribe,
                    SmallImage = data.SmallImage,
                    SmallTitle = data.SmallTitle,
                    BigTitle = data.BigTitle,
                    Brief = data.Brief,
                    Content = data.Content,
                    PublishDateTime = data.PublishDateTime,
                    CreateDateTime = data.CreateDateTime,
                    LastUpdateDateTime = data.LastUpdateDateTime,
                    ClickCount = data.ClickCount,
                    Category = data.Category,
                    CategoryTags = data.CategoryTags,
                    Type = data.Type,
                    IsShow = data.IsShow,
                    Status = data.Status,
                    CoverTag = data.CoverTag,
                    IsTopMost = data.IsTopMost,
                    ContentHtml = data.ContentHtml,
                    QRCodeImg = data.QRCodeImg,
                    CoverConfig = new ArticleCoverConfig()
                    {
                        PKID = coverConfig.PKID,
                        ArticleId = coverConfig.ArticleId,
                        CoverType = coverConfig.CoverType,
                        CoverImg = coverConfig.CoverImg,
                        CoverVideo = coverConfig.CoverVideo,
                        OtherImg = coverConfig.OtherImg,
                        Source = coverConfig.Source
                    }
                };
            }

            return View(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpsertYxArticle(YouXuanArticle model, string createType)
        {
            if (createType == "Saved")
            {
                model.Status = ArticleStatus.Saved.ToString();
                model.IsShow = 0;
                model.PublishDateTime = model.PublishDateTime ?? DateTime.Now;
            }
            else//发表。定时发表
            {
                model.Status = ArticleStatus.Published.ToString();
                model.IsShow = 1;
                if (model.PublishDateTime == null)
                    model.PublishDateTime = DateTime.Now;
            }
            model.CategoryTags = CategoriesJson(model.CategoryTags);
            model.BigTitle = model.SmallTitle;
            var result = ArticleBll.UpsertYxArticle(model, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectProductListByPid(string pid)
        {
            var result = ProductService.SelectSkuProductListByPids(new List<string> { pid });
            if (result != null && result.Any())
            {
                var data = result.FirstOrDefault();
                return Json(new
                {
                    status = true,
                    displayName = data.DisplayName,
                    des = data.ShuXing5,
                    price = data.Price,
                    imgUrl = ConfigurationManager.AppSettings["DoMain_image"].ToString() + data.Image,
                    orderQuantity = data.OrderQuantity
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 导购文章
        public async Task<ActionResult> DaoGouArticle(int id = 0, string source = "Published")
        {
            YouXuanArticle result = new YouXuanArticle() { CoverConfig = new ArticleCoverConfig() };
            ViewData["Source"] = source;
            if (id > 0)
            {
                var data = await ArticleBll.GetArticleDetailById(id);
                result = new YouXuanArticle
                {
                    PKID = data.PKID,
                    Image = data.Image,
                    ShowImages = data.ShowImages,
                    ShowType = data.ShowType,
                    IsDescribe = data.IsDescribe,
                    SmallImage = data.SmallImage,
                    SmallTitle = data.SmallTitle,
                    BigTitle = data.BigTitle,
                    Brief = data.Brief,
                    Content = data.Content,
                    PublishDateTime = data.PublishDateTime,
                    CreateDateTime = data.CreateDateTime,
                    LastUpdateDateTime = data.LastUpdateDateTime,
                    ClickCount = data.ClickCount,
                    Category = data.Category,
                    CategoryTags = data.CategoryTags,
                    Type = data.Type,
                    IsShow = data.IsShow,
                    Status = data.Status,
                    CoverTag = data.CoverTag,
                    IsTopMost = data.IsTopMost,
                    ContentHtml = data.ContentHtml,
                    QRCodeImg = data.QRCodeImg,
                    CoverMode=data.CoverMode,
                    CoverImage=data.CoverImage
                };
            }

            return View(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpsertDaoGouArticle(YouXuanArticle model, string createType)
        {
            if (createType == "Saved")
            {
                model.Status = ArticleStatus.Saved.ToString();
                model.IsShow = 0;
                model.PublishDateTime = model.PublishDateTime ?? DateTime.Now;
            }
            else//发表。定时发表
            {
                model.Status = ArticleStatus.Published.ToString();
                model.IsShow = 1;
                if (model.PublishDateTime == null)
                    model.PublishDateTime = DateTime.Now;
            }
            model.BigTitle = model.SmallTitle;
            var result = ArticleBll.UpsertDaoGouArticle(model, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}   
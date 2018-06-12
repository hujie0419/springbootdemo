using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.WebSite.Component.Activity.Common.Cache;
using Tuhu.WebSite.Component.Activity.BusinessData;
using Tuhu.WebSite.Component.Activity.Business;
using Tuhu.WebSite.Component.SystemFramework.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.WebSite.Web.Activity.BusinessFacade;
using Tuhu.WebSite.Web.Activity.Common;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;
using System.Collections;
using System.Web.Caching;
using Tuhu.Nosql;
using System.Configuration;
using Tuhu.MessageQueue;
using Tuhu.WebSite.Component.Discovery.BusinessData;
using Tuhu.WebSite.Component.Discovery.Business;

namespace Tuhu.WebSite.Web.Activity.Controllers
{

    public partial class DiscoveryController : Controller
    {
        static readonly string defaultExchageName = "direct.defaultExchage";

        //发现文章更新消息队列
        static readonly string DiscoveryArticleNotificationQueueName = "notification.DiscoveryArticleModify";
        static readonly RabbitMQProducer DiscoveryArticleRechargeProduceer;

        //发现文章更新消息队列
        static readonly string DiscoveryArticleInitializationNotificationQueueName = "notification.discoveryArticleInitialization";
        static readonly RabbitMQProducer DiscoveryInitializationRechargeProduceer;
        static DiscoveryController()
        {
            try
            {
                DiscoveryArticleRechargeProduceer = RabbitMQClient.DefaultClient.CreateProducer(defaultExchageName);
                DiscoveryArticleRechargeProduceer.ExchangeDeclare(defaultExchageName);
                DiscoveryArticleRechargeProduceer.QueueBind(DiscoveryArticleNotificationQueueName, defaultExchageName);

                DiscoveryInitializationRechargeProduceer = RabbitMQClient.DefaultClient.CreateProducer(defaultExchageName);
                DiscoveryInitializationRechargeProduceer.ExchangeDeclare(defaultExchageName);
                DiscoveryInitializationRechargeProduceer.QueueBind(DiscoveryArticleInitializationNotificationQueueName, defaultExchageName);
            }
            catch
            {

            }
        }
        /// <summary>
        /// 发现列表
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="category"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectDiscoveryByCategory(string userId, int pageNumber = 1, int category = 1)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询发现首页数据
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectDiscoveryHomeData(string userId, int pageIndex = 1, int pageSize = 10, string vehicleId = "")
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询发现首页数据
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectDiscoveryHomeDataNew(string userId, int pageIndex = 1, int pageSize = 10, string vehicleId = "")
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发现首页查询
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="vehicleId">车型Id</param>
        /// <param name="sortType">排序规则</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectDiscoveryHomeDataVersion1(string userId, int pageIndex = 1, int pageSize = 10, string vehicleId = "", int sortType = 0)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 发现首页查询
        /// </summary>
        /// <returns></returns>
        //[OutputCache(Duration = 180, VaryByParam = "*")]
        [Obsolete]
        public  ActionResult SelectDiscoveryHomeDataVersion2()
        {
            var dic = new Dictionary<string, object>() {["Code"] = "0", ["Message"] = "请升级到最新版本"};
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询未读消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectUnReadMessage(string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送已读消息通知
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newIds"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult UpdateDiscoveryNewsStatus(string userId, string newsIds)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询推荐达人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectRecommandExpert(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发现首页，随机推荐标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectRandomRecomandTopic(string userId, string vehicleId = null)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 查询推荐的标签/专题
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectRecommandTopic(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var recommandTopicList = await DiscoverBLL.SelectRecommandTopicForUserId(userId, pager);
                dic.Add("Code", "1");
                dic.Add("TotalPage", pager.TotalPage);
                dic.Add("TotalCount", pager.TotalItem);
                dic.Add("RecommandTopic", recommandTopicList);
            }
            catch (Exception)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "查询失败");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询所有的问题，并根据指定的排序规则排序
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="vehicleId">车型Id</param>
        /// <param name="sortType">排序规则</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectDiscoveryAllQuestionsVersion1(int pageIndex = 1, int pageSize = 10, int sortType = 0, string userId = null, string vehicleId = null)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询所有的问题，并根据指定的排序规则排序
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortType"></param>
        /// <param name="userId"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectDiscoveryAllQuestionsVersion2(int pageIndex = 1, int pageSize = 10, int sortType = 0, string userId = null, string vehicleId = null)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据类别查询相关的文章并按照指定的排序规则进行排序(文章、口碑、车品文章查询)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="categoryId">类别Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="vehicleId">车型Id</param>
        /// <param name="order">排序规则</param>
        /// <returns></returns>
        public async Task<ActionResult> SelectArticleByCategoryVersion1(int categoryId, string vehicleId = null, int pageIndex = 1, int pageSize = 10, int sortType = 0)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var articlePage = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                //根据categoryId查询文章、专题
                var articles = await DistributedCacheHelper.SelectArticleByCategoryVersion1(categoryId, articlePage, sortType, vehicleId);
                var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                articles.ForEach(article =>
                {
                    article.ArticleShowMode = articleShowMode;
                    article.ContentUrl = (articleShowMode == "New" || article.Type == 5) ? DomainConfig.FaXian + "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + article.PKID : article.ContentUrl;
                });
                dic.Add("Code", "1");
                dic.Add("Articles", articles);
                dic.Add("TotalCount", articlePage.TotalItem);
                dic.Add("TotalPage", articlePage.TotalPage);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException("根据类别查询相关的文章并按照指定的排序规则进行排序(文章、口碑、车品文章查询)", ex.InnerException);
                dic.Clear();
                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 根据类别查询相关的文章并按照指定的排序规则进行排序(文章、口碑、车品文章查询)
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="categoryId">类别Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="vehicleId">车型Id</param>
        /// <param name="order">排序规则</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectArticleByCategoryVersion2(string userId, int categoryId = 1, string vehicleId = null, int pageIndex = 1, int pageSize = 10, int sortType = 0)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 取出发现板块中的父级标签
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SelectCategorysForDiscover()
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var parentCategory = await DistributedCacheHelper.SelectCategorysForDiscover();
                parentCategory.Insert(0, new RecommandTopic() { id = -1, name = "推荐" });
                dic.Add("Code", "1");
                dic.Add("CategoryList", parentCategory);
            }
            catch (Exception)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "查询异常");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据用户和父级标签，查询二级标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectCategoryByParentId(string userId, int parentCategoryId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var categoryList = await DistributedCacheHelper.SelectCategoryByParentId(userId, parentCategoryId);
                if (parentCategoryId != -1)
                {
                    var currentCategory = categoryList.FirstOrDefault(c => c.id == parentCategoryId);
                    categoryList.Remove(currentCategory);
                    currentCategory.image = string.IsNullOrEmpty(currentCategory.image) ? "http://image.tuhu.cn/Images/Logo/" +
                        Chs2py.Pinyin(currentCategory.name.Substring(4).Trim()).ToLower() + ".png" : currentCategory.image;
                    categoryList.Insert(0, currentCategory);
                }
                dic.Add("Code", "1");
                dic.Add("CategoryList", categoryList);
            }
            catch (Exception ex)
            {
                WebLog.LogException("根据用户和父级标签，查询二级标签", ex);
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "查询异常");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 关注/取消关注标签
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <param name="isAttention"></param>
        /// <returns></returns>
        public async Task<ActionResult> AttentionCategory(int categoryId, string userId, int isAttention, int categoryType = 1)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                int result;
                //专题
                if (categoryType == 2)
                {
                    bool Status;
                    result = ArticleSystem.AddUserReviewOfArticles(userId, categoryId, 1, isAttention.ToString(), out Status);
                }
                else
                {
                    result = await DiscoverBLL.AttentionCategory(categoryId, userId, isAttention, categoryType == 1 ? "标签" : "专题");


                    //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(string.Format("http://t.tuhu.cn/red?EventID=attentioncatgory&useridentity={0}&Data1={1};{2}", userId, categoryId, isAttention));

                    if (DomainConfig.ApiSite == "http://api-unit.tuhu.cn" || DomainConfig.ApiSite == "http://api.tuhu.cn")
                    {
                        try
                        {
                            var httpClient = new System.Net.Http.HttpClient();
                            Dictionary<string, string> parameters = new Dictionary<string, string> { { "EventID", "attentioncatgory" }, { "useridentity", categoryId.ToString() }, { "Data1", categoryId.ToString() + ";" + isAttention.ToString() } };
                            System.Net.Http.HttpContent content = new System.Net.Http.FormUrlEncodedContent(parameters);
                            System.Net.Http.HttpResponseMessage response = await httpClient.GetAsync(string.Format("http://t.tuhu.cn/red?EventID=attentioncatgory&useridentity={0}&Data1={1};{2}", userId, categoryId, isAttention));
                        }
                        catch (Exception ex)
                        {
                            WebLog.LogException("发送BI埋点", ex);
                        }
                    }


                }

                if (result > 0)
                {
                    dic.Add("Code", "1");
                    dic.Add("IsAtention", isAttention);
                }
                else
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "操作失败");
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "操作失败");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 关注多个标签
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="userId"></param>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult AttentionMultipleCategory(string categoryIds, string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询标签的详情
        /// </summary>
        /// <param name="caregoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectCategoryDetailById(int categoryId, string userId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var categoryDetail = await DiscoverBLL.SelectCategoryDetailById(categoryId, userId);
                var recommandCategoryPager = new PagerModel { CurrentPage = 1, PageSize = 10 };
                var recommandCategory = await DiscoverBLL.SelectRecommandCategoryByCategoryId(categoryId, userId, recommandCategoryPager);

                string parentName = categoryDetail == null || categoryDetail.ParentName == null ? null : categoryDetail.ParentName.Length > 4 ? categoryDetail.ParentName.Substring(4) : categoryDetail.ParentName;
                if (string.IsNullOrEmpty(categoryDetail.Image) && string.IsNullOrEmpty(categoryDetail.ParentName) == false)
                {
                    categoryDetail.Image = ("http://image.tuhu.cn/Images/Logo/" + Chs2py.Pinyin(parentName.Trim()).ToLower() + ".png");
                }
                else if (string.IsNullOrEmpty(categoryDetail.Image))
                {
                    categoryDetail.Image = "http://image.tuhu.cn/Images/Logo/" + Chs2py.Pinyin(categoryDetail.Header.Substring(4).Trim()).ToLower() + ".png";
                }

                var deviceId = Request.Headers.Get("DeviceID");
                var pageStyle = deviceId == null ? 1 : await DiscoverBLL.SelectDiscoveryPageStyle(deviceId, userId);
                categoryDetail.RecommandCategoryList = recommandCategory;
                dic.Add("Code", "1");
                dic.Add("Data", categoryDetail);
                dic.Add("PageStyle", pageStyle);
            }
            catch (Exception ex)
            {
                WebLog.LogException(string.Empty, ex);
                dic.Clear();
                dic.Add("Code", "0");
                var innerException = ex.InnerException == null ? "无" : ex.InnerException.Message;
                dic.Add("Messages", ex.Message + "---" + innerException);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询标签的动态
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectCategoryDynamic(int categoryId, string userId, int pageIndex = 1, int pageSize = 10, int isLoadedFinish = 0)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                //当前标签已经加载完毕，则加载推荐的标签
                if (isLoadedFinish == 1)
                {
                    var recommandCategoryPager = new PagerModel() { CurrentPage = pageIndex, PageSize = 1 };
                    var recommandCategory = (await DiscoverBLL.SelectRecommandCategoryByCategoryId(categoryId, userId, recommandCategoryPager)).FirstOrDefault();

                    if (recommandCategory == null)
                    {
                        dic.Add("Code", "0");
                        //dic.Add("Message", "该标签不存在");
                        return Json(dic, JsonRequestBehavior.AllowGet);
                    }
                    var recomandCategoryDynamicPager = new PagerModel { CurrentPage = 1, PageSize = 10 };
                    var recommandCategoryDynamicList = await DiscoverBLL.SelectCategoryDynamic(recommandCategory.id, recomandCategoryDynamicPager);

                    var tempDynamicList = new ArrayList();
                    recommandCategoryDynamicList.ForEach(d =>
                    {
                        tempDynamicList.Add(new
                        {
                            Question = d.Question,
                            QuestionId = d.QuestionId,
                            UserName = d.Answerer,
                            UserHead = d.AnswererHeader,
                            CommentContent = d.Answer,
                            ID = d.AnswerId,
                            UserID = d.AnswererId,
                            UserGrade = d.UserGrade,
                            Vehicle = d.Vehicle,
                            Praise = d.Praise,
                            UserIdentity = d.UserIdentity,
                            UserIdentityName = d.UserIdentityName,
                            CommentImage = string.IsNullOrEmpty(d.CommentImage) == false ? d.CommentImage.Split(';')[0] : string.Empty
                        });
                    });
                    dic.Add("Code", "1");
                    dic.Add("Data", new
                    {
                        RecommandTagTotalCount = recommandCategoryPager.TotalItem,
                        RecommandTagTotalPage = recommandCategoryPager.TotalPage,
                        RecommandTag = new
                        {
                            Header = new
                            {
                                Id = recommandCategory.id,
                                Header = recommandCategory.name,
                                IsAttention = recommandCategory.IsAttention,
                                AttentionCount = recommandCategory.AttentionCount,
                                Description = recommandCategory.describe,
                                Image = recommandCategory.image
                            },
                            DynamicList = tempDynamicList
                        }
                    });

                }
                else
                {
                    var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                    var dynamicList = await DiscoverBLL.SelectCategoryDynamic(categoryId, pager);
                    var tempDynamicList = new ArrayList();
                    dynamicList.ForEach(d =>
                    {
                        tempDynamicList.Add(new
                        {
                            Question = d.Question,
                            QuestionId = d.QuestionId,
                            UserName = d.Answerer,
                            UserHead = d.AnswererHeader,
                            CommentContent = d.Answer,
                            ID = d.AnswerId,
                            UserID = d.AnswererId,
                            UserGrade = d.UserGrade,
                            Vehicle = d.Vehicle,
                            Praise = d.Praise,
                            UserIdentity = d.UserIdentity,
                            UserIdentityName = d.UserIdentityName,
                            CommentImage = string.IsNullOrEmpty(d.CommentImage) == false ? d.CommentImage.Split(';')[0] : string.Empty
                        });
                    });
                    dic.Add("Code", "1");
                    //判断是否加载完毕当前标签的动态，如果加载完毕则加载推荐标签的内容
                    if (dynamicList.Count < 10)
                    {
                        var recommandCategoryPager = new PagerModel { CurrentPage = 1, PageSize = 1 };
                        var recommandCategory = (await DiscoverBLL.SelectRecommandCategoryByCategoryId(categoryId, userId, recommandCategoryPager)).FirstOrDefault();
                        if (recommandCategory != null)
                        {
                            var recomandCategoryDynamicPager = new PagerModel { CurrentPage = 1, PageSize = 10 };
                            var recommandCategoryDynamicList = await DiscoverBLL.SelectCategoryDynamic(recommandCategory.id, recomandCategoryDynamicPager);


                            var recommandCategoryTempDynamicList = new ArrayList();
                            recommandCategoryDynamicList.ForEach(d =>
                            {
                                recommandCategoryTempDynamicList.Add(new
                                {
                                    Question = d.Question,
                                    QuestionId = d.QuestionId,
                                    UserName = d.Answerer,
                                    UserHead = d.AnswererHeader,
                                    CommentContent = d.Answer,
                                    ID = d.AnswerId,
                                    UserID = d.AnswererId,
                                    UserGrade = d.UserGrade,
                                    Vehicle = d.Vehicle,
                                    Praise = d.Praise,
                                    UserIdentity = d.UserIdentity,
                                    UserIdentityName = d.UserIdentityName,
                                    CommentImage = string.IsNullOrEmpty(d.CommentImage) == false ? d.CommentImage.Split(';')[0] : string.Empty
                                });
                            });


                            dic.Add("Data", new
                            {
                                Source = tempDynamicList,
                                TotalPage = pager.TotalPage,
                                TotalItem = pager.TotalItem,
                                IsLoadedFinish = dynamicList.Count == 10 ? 0 : 1,
                                RecommandTagTotalCount = recommandCategoryPager.TotalItem,
                                RecommandTagTotalPage = recommandCategoryPager.TotalPage,
                                RecommandTag = new
                                {
                                    Header = new
                                    {
                                        Id = recommandCategory.id,
                                        Header = recommandCategory.name,
                                        IsAttention = recommandCategory.IsAttention,
                                        AttentionCount = recommandCategory.AttentionCount,
                                        Description = recommandCategory.describe,
                                        Image = recommandCategory.image
                                    },
                                    DynamicList = recommandCategoryTempDynamicList
                                }
                            });
                        }
                        else
                        {
                            dic.Add("Data", new
                            {
                                Source = tempDynamicList,
                                TotalPage = pager.TotalPage,
                                TotalItem = pager.TotalItem,
                                IsLoadedFinish = 0
                            });
                        }
                    }
                    else
                    {
                        dic.Add("Data", new
                        {
                            Source = tempDynamicList,
                            TotalPage = pager.TotalPage,
                            TotalItem = pager.TotalItem,
                            IsLoadedFinish = dynamicList.Count == 10 ? 0 : 1
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "查询失败");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据标签，推荐标签
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectRecommandCategoryByCategoryId(int categoryId, string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询标签的问题
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectAllQuestionByCategoryId(int categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询专题列表
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public async Task<ActionResult> SelectSubjects(int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询Web站点发现首页的标签
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SelectCategoryTagsForWeb()
        {
            var dic = new Dictionary<string, object>();
            try
            {
                //查询标签
                var categroyTags = await DistributedCacheHelper.DiscoveryHomeCategoryTagForWebFromCache();
                dic.Add("CategoryTags", categroyTags.Select(c => new { Id = c.id, Name = c.name }));
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                dic.Clear();
                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 是否有新消息(点赞、回复和回答)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult IsHasNewMessageForDiscovery(string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否有新消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult IsHasNewMessageForDiscoveryNew(string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询热门标签
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数据数量</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectPopularCategoryTag(int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据标签类型查询文章、专题、问题
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectArticleByCategory(int categoryId, string userId, int pageIndex = 1, int pageSize = 10, int sortType = 1)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据标签查询文章
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectArticleByCategoryVersion3(int categoryId, string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var versionNumber = Request.Headers.Get("VersionCode");
                var version = Request.Headers.Get("version");

                var articlePage = new PagerModel() { CurrentPage = pageIndex, PageSize = pageSize };
                //根据categoryId查询文章
                var articles = DiscoverBLL.SelectHomeArticle(articlePage, categoryId, false);
                var tempArticles = new ArrayList();
                var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                articles.ForEach(a =>
                {
                    tempArticles.Add(new
                    {
                        PKID = a.PKID,
                        ArticleShowMode = articleShowMode,
                        SmallTitle = a.SmallTitle,
                        BigTitle = a.BigTitle,
                        ClickCount = a.ClickCount,
                        Vote = a.Vote,
                        CommentTimes = a.CommentTimes,
                        AnnotationTime = a.AnnotationTime,
                        ShowImages = a.ShowImages,
                        ShowType = a.ShowType,
                        ContentUrl = (articleShowMode == "New" || a.Type == 5) ? DomainConfig.FaXian + "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + a.PKID : a.ContentUrl,
                        Content = a.Content,
                        VoteState = a.VoteState,
                        Brief = a.Brief,
                        Source = a.Source,
                        SmallImage = a.SmallImage,
                        IsDescribe = a.IsDescribe,
                        PublishNewDateTime = a.PublishNewDateTime
                    });
                });
                dic.Add("Code", "1");
                dic.Add("Data", new { Source = tempArticles, TotalPage = articlePage.TotalPage, TotalCount = articlePage.TotalItem });
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Error", ex.Message);
                dic.Add("Code", "0");
            }

            //浏览数
            ArticleSystem.AddReadingRecord(userId, 3, "0", categoryId);
            ArticleSystem.UpdateArticleClick(categoryId);

            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询热门问答
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数据的数量</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectPopularAnswers(int categoryiId = 1, string vehicleId = "", string userId = null, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> SelectPopularAnswersNew(int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var page = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var result=await DistributedCacheHelper.SelectPopularAnswersNew(page);
                if (result != null)
                {
                    dic["Code"] = "1";
                    dic["PopularAnswers"] = result;
                }
                else
                {
                    dic["Code"] = "0";
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic["Message"]= "查询失败";
                dic["Code"] = "0";
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 查询待回答问题
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示数据的数量</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectToAnswerQuestions(int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 发现首页，提交问题
        /// </summary>
        /// <param name="userId">问题提交者</param>
        /// <param name="questionContent">问题内容</param>
        /// <param name="vehicle">车型名称</param>
        /// <param name="vehicleId">车型编号</param>
        /// <param name="vehicleImage">车型图片</param>
        /// <param name="commentImage">评论图片</param>
        /// <returns></returns>

        public async Task<ActionResult> SubmitDiscoveryQuestion(string userId, string questionContent, string vehicle, string vehicleId, string vehicleImage, string commentImage,string userHead, string userGrade, string userName, string realName, string sex,string topicid, int categoryId = 0,int questionType= 3,int UserIdentity=0)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var comment = new DiscoveryCommentModel
                {
                    CommentContent = questionContent,
                    Vehicle = !string.IsNullOrWhiteSpace(vehicle) ? vehicle : null,
                    UserID = userId,
                    VehicleId = !string.IsNullOrWhiteSpace(vehicleId) ? vehicleId : null,
                    VehicleImage = vehicleImage,
                    CategoryTags = "[{\"key\":" + vehicleId + "\",value\":" + vehicle + ",\"isShow\":\"1\"}]",
                    CommentImage = commentImage,
                    UserHead = userHead,
                    UserGrade = userGrade,
                    UserName = userName,
                    RealName = realName,
                    Sex = sex,
                    Topicid = topicid,
                    CategoryId = categoryId
                };

                //Guid guidUserId;
                //if (!Guid.TryParse(comment.UserID, out guidUserId))
                //{
                //    dic.Add("Code", "0");
                //    return Json(dic);
                //}
                //过滤非法关键字
                if (UserIdentity <= 0)
                {
                    if (HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/ValidateCommentContent",
                    new Dictionary<string, string> { { "userId", comment.UserID }, { "content", questionContent } }) == "false")
                    {
                        dic.Add("Code", "0");
                        dic.Add("Message", "存在非法词汇");
                        return Json(dic);
                    }

                    Guid userIdGuid;
                    if (!Guid.TryParse(comment.UserID, out userIdGuid))
                    {
                        dic.Add("Code", "0");
                        return Json(dic);
                    }

                    comment.UserID = userIdGuid.ToString("B");
                }
                
                comment.AuditStatus = 1;
                //格式：22:00-08:00
                string timeLine = await DistributedCacheHelper.SelectDictionaryValueByKey("tbl_Question_Audit_Duration");
                if (!string.IsNullOrEmpty(timeLine) && IsLegalTime(DateTime.Now, timeLine))
                {
                    comment.AuditStatus = 0;
                }
                if (string.IsNullOrWhiteSpace(userId))
                {
                    dic.Add("Code", "0");
                    return Json(dic);
                }
                var createInfo = new
                {
                    comment.UserHead,
                    comment.UserName,
                    comment.UserGrade,
                    comment.RealName,
                    comment.Sex,
                    comment.Topicid
                };
                var result = await DiscoverBLL.AddDiscoveryQuestion(comment, JsonConvert.SerializeObject(createInfo),questionType == 0 ? 3 : questionType);
                if (result > 0)
                {
                    var isShowSettingDialog = await IsShowSettingHeadImageDialog(userId);
                    dic.Add("AuditStatus", comment.AuditStatus);
                    dic.Add("IsShowSettingDialog", isShowSettingDialog);
                    dic.Add("Message", "提交成功");
                    if (comment.AuditStatus == 1 && UserIdentity <= 0)
                    {
                        DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                        {
                            Type = 4,
                            ActileId = result,
                            UserId = Guid.Parse(userId).ToString("B"),
                            Vote = 1
                        });

                        DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                        {
                            Type = 1,
                            ActileId = result,
                            UserId = Guid.Parse(userId).ToString("B"),
                            Vote = 1
                        });

                        if (!string.IsNullOrWhiteSpace(vehicleId) || categoryId > 0)
                        {

                            DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                            {
                                Type = 7,
                                ActileId = result,
                                UserId = Guid.Parse(userId).ToString("B"),
                                Vote = 1
                            });
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    dic["Code"]="0";
                    dic["Message"] = "提交失败";
                    return Json(dic);
                }
                   
                dic.Add("QuestionId", result);
                dic.Add("Code", "1");
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Add("Code", "0");
                dic.Add("Error", ex.Message);
            }
            return Json(dic);
        }

        /// <summary>
        /// 判断是否显示设置头像的Dialog
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<string> IsShowSettingHeadImageDialog(string userId)
        {
            try
            {

                using (var userClient = new Tuhu.Service.UserAccount.UserAccountClient())
                {
                    var userInfo = await userClient.GetUserByIdAsync(new Guid(userId));
                    if (userInfo.Result != null)
                    {
                        var userHeadImage = userInfo.Result.Profile.HeadUrl;
                        if (string.IsNullOrEmpty(userHeadImage))
                        {
                            return "1";
                        }
                        else
                            return "0";
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException("查询用户信息出错", ex);
                return "0";
            }
        }




        /// <summary>
        /// 专题详页
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult FetchSubjectDetail(string userId, int articleId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 专题详情页问题或说说列表
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="type"> 1 问题 2说说</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectSubjectListType(int subjectId, int type, string userId, int pageNumber = 1)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 专题动态
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectSubjectDynamic(int subjectId, int pageNumber = 1)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 问题详情页
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="questionType">问题类型 1为专题问题 2为全局问题</param>
        /// <returns></returns>
        public async Task<ActionResult> SelectQuestionDetail(int questionId, string userId, int pageNumber = 1, int questionType = 1)
        {
            #region 专题问题详情
            if (questionType == 1)
            {
                var dic = new Dictionary<string, object>();
                try
                {
                    var page = new PagerModel(pageNumber, 10);
                    if (page.CurrentPage == 1)
                    {
                        var questionDetail = DiscoverBLL.FetchQuestionDetail(questionId, userId);
                        dic.Add("QuestionDetail", questionDetail);
                    }
                    var data = new DiscoverySubjectListCacheModel()
                    {
                        Source = DiscoverBLL.SelectSubjectSecondList(questionId, userId, page),
                        TotalPage = page.TotalPage,
                    };
                    dic.Add("Data", data);
                    dic.Add("Code", "1");
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                    dic.Add("Message", "查询失败");
                    dic.Add("Code", "0");
                }
                //浏览数
                ArticleSystem.AddReadingRecord(userId, 3, "0", questionId);
                ArticleSystem.UpdateArticleClick(questionId);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            #endregion
            #region 全局问题详情
            //全局问题
            else
            {
                var dic = new Dictionary<string, object>();
                try
                {
                    var page = new PagerModel(pageNumber, 10);
                    //查询问题详情
                    var questionDetail = await DiscoverBLL.SelectGlobalQuestionDetail(questionId, userId);

                    var versionNumber = Request.Headers.Get("VersionCode");
                    var version = Request.Headers.Get("version");

                    //仅仅请求首次查询详情
                    if (pageNumber == 1)
                    {


                        if ((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "48") < 0)
                            || (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.3") < 0)))
                        {
                            if (string.IsNullOrEmpty(questionDetail.CommentImage) == false)
                            {
                                var questionImage = questionDetail.CommentImage.Split(';');
                                questionDetail.CommentImage = questionImage[0];
                            }
                        }


                        dic.Add("QuestionDetail", questionDetail.PKID > 0 ? questionDetail : null);
                        //dic.Add("Message","该问题已被删除");
                    }

                    if (questionDetail.PKID > 0)
                    {
                        var answers = await DiscoverBLL.SelectGlobalQuestionAnswerList(questionId, userId, page);

                        if ((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "48") < 0)
                           || (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.3") < 0)))
                        {
                            answers.ForEach(a =>
                            {
                                if (string.IsNullOrEmpty(a.CommentImage) == false)
                                {
                                    var answerImage = a.CommentImage.Split(';');
                                    a.CommentImage = answerImage[0];
                                }
                            });
                        }
                        //查询全局问题的回答详情
                        var data = new DiscoverySubjectListCacheModel()
                        {
                            Source = answers,
                            TotalPage = page.TotalPage,
                        };
                        dic.Add("Data", data);
                    }
                    else
                        dic.Add("Data", null);

                    dic.Add("Code", "1");

                    //浏览数
                    ArticleSystem.AddReadingRecord(userId, 3, "0", questionId);
                    ArticleSystem.UpdateArticleClick(questionId);
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                    dic.Clear();
                    dic.Add("Code", "0");
                    dic.Add("Message", "查询失败");
                }
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            #endregion
        }

        /// <summary>
        /// 设置最佳回答
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<ActionResult> MarkBestAnswer(string userName, string userHead, int commentId, int questionId, string answererUserId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var result = await DiscoverBLL.MarkBestAnswer(commentId, questionId);
                if (result > 0)
                {
                    userName = GetUserName(userName, userName, string.Empty);
                    await InsertPraiseNotice(userName, commentId, 3, userHead, answererUserId, "把你的回答设为了最佳！");
                    dic.Add("Code", "1");
                }
                else
                    dic.Add("Code", "0");
            }
            catch (Exception ex)
            {
                WebLog.LogException("设置最佳回答", ex);
                dic["Code"] = "0";
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询回答详细
        /// </summary>
        /// <param name="answerId">回答ID</param>
        /// <param name="userId">用户Id</param>
        /// <param name="questionType">问题类型 1为专题问题 3为全局问题</param>
        /// <returns></returns>
        public async Task<ActionResult> FetchAnswerDetail(int answerId, string userId, int pageSize = 10, int pageIndex = 1, int questionType = 1)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                DiscoveryAnswerModel questionDetail = null;
                //专题内的问题
                if (questionType == 1)
                {
                    questionDetail = DiscoverBLL.FetchAnswerDetail(answerId, userId);
                    dic.Add("AnswerDetail", questionDetail);
                    dic.Add("Code", "1");
                }
                //全局问题
                else if (questionType == 3 || questionType == 5 || questionType == 2)
                {
                    var page = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                    var answerDetail = DiscoverBLL.DiscoveryGlobalQuestionAnswerDetail(answerId, userId, page).ToList();
                    if (pageIndex == 1)
                    {
                        var currentAnswer = answerDetail.FirstOrDefault();
                        if (currentAnswer == null)
                        {
                            dic.Add("AnswerDetail", answerDetail);
                            dic.Add("Total", page.TotalItem);
                            dic.Add("TotalPage", page.TotalPage);
                            dic.Add("Code", "1");
                            return Json(dic, JsonRequestBehavior.AllowGet);
                        }
                        if (currentAnswer.UserIdentity == 0)
                        {
                            try
                            {
                                var userJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/User/SelectUserInfoForDiscovery",
                                       new Dictionary<string, string> { { "userId", currentAnswer.UserID } });
                                var userInfo = JsonConvert.DeserializeObject<UserInfoForDiscovery>(userJson);
                                if (currentAnswer != null && userInfo != null && userInfo.Data != null)
                                    currentAnswer.Signature = userInfo.Data.UserSign;
                            }
                            catch (Exception)
                            {
                            }

                        }
                    }


                    var versionNumber = Request.Headers.Get("VersionCode");
                    var version = Request.Headers.Get("version");
                    if ((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "48") < 0)
                     || (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.3") < 0)))
                    {
                        answerDetail.ForEach(a =>
                        {
                            if (string.IsNullOrEmpty(a.CommentImage) == false)
                            {
                                var commentImage = a.CommentImage.Split(';');
                                a.CommentImage = commentImage[0];
                            }
                        });
                    }

                    dic.Add("AnswerDetail", answerDetail);
                    dic.Add("Total", page.TotalItem);
                    dic.Add("TotalPage", page.TotalPage);
                    dic.Add("Code", "1");
                }
                //专题的问题新版本 TODO 分页
                else if (questionType == 4)
                {
                    var page = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                    var answerDetail = DiscoverBLL.FetchAnswerDetailNew(answerId, userId, page);
                    dic.Add("AnswerDetail", answerDetail.Any(a => a.Type == 2) ? answerDetail : null);
                    dic.Add("Total", page.TotalItem);
                    dic.Add("TotalPage", page.TotalPage);
                    dic.Add("Code", "1");
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException("回答详情页", ex);
                dic.Clear();
                dic.Add("Error", ex.InnerException.Message);
                dic.Add("Message", "查询失败");
                dic.Add("Code", "0");
            }


            //浏览数
            ArticleSystem.AddReadingRecord(userId, 5, "0", answerId);
            ArticleSystem.UpdateArticleClick(answerId);

            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提问或发表说说以及回复
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<ActionResult> SubmintDiscoveryComment(DiscoveryCommentModel comment,int? apiVersion=0)
        {
            var dic = new Dictionary<string, object>();
            var versionNumber = Request.Headers.Get("VersionCode");
            var version = Request.Headers.Get("version");
            if (string.IsNullOrWhiteSpace(versionNumber) && string.IsNullOrWhiteSpace(versionNumber) && (!apiVersion.HasValue || apiVersion.Value < 4))
            {
                dic.Add("Code", "0");
                dic.Add("Message", "请升级");
                return Json(dic);
            }
            try
            {
                if (string.IsNullOrWhiteSpace(comment.UserID))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "非法用户");
                    return Json(dic);
                }
                Guid userId = Guid.Empty;
                Guid.TryParse(comment.UserID, out userId);
                if (userId == Guid.Empty && comment.UserIdentity < 1)
                {
                    comment.UserIdentity = 2;
                }

                comment.Type = comment.Type == 0 ? 3 : comment.Type;
                comment.ParentID = (comment.ParentID > 0 && comment.ParentID != comment.PKID) ? comment.ParentID : null;
                comment.UserID = userId == Guid.Empty ? comment.UserID : userId.ToString("B");
                comment.UserName = GetUserName(comment.UserName, comment.RealName, comment.PhoneNum, comment.Sex);
                comment.AuditStatus = 2;
                //关键字过滤 GOTO
                //过滤非法关键字
                if (comment.UserIdentity <= 0)
                {
                    try
                    {
                        //格式：22:00-08:00
                        string timeLine = await DistributedCacheHelper.SelectDictionaryValueByKey("tbl_Question_Audit_Duration");
                        if (!string.IsNullOrEmpty(timeLine) && IsLegalTime(DateTime.Now, timeLine))
                        {
                            comment.AuditStatus = 0;
                        }
                        if (HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/ValidateCommentContent",
                    new Dictionary<string, string> { { "userId", comment.UserID }, { "content", comment.CommentContent } }) == "false")
                        {
                            dic.Add("Code", "0");
                            dic.Add("Message", "存在非法词汇");
                            return Json(dic);
                        }
                    }
                    catch (Exception ex)
                    {
                        WebLog.LogException("验证非法字符出错", ex);
                    }
                }


                var result =await DiscoverBLL.AddDiscoveryComment(comment);
                var commentId = result.Item1;
                var parentUserId = result.Item2;
                if (commentId > 0)
                {
                    dic.Add("Message", "提交成功");
                    bool isForum = false;
                    //仅针对普通用户判断
                    if (comment.UserIdentity < 1)
                    {
                        
                        var isShowSettingDialog = await IsShowSettingHeadImageDialog(comment.UserID);
                        dic.Add("IsShowSettingDialog", isShowSettingDialog);
                    }
                    else if (comment.AuditStatus == 2 && comment.UserIdentity > 0)
                    {
                        var questionDetail = await DiscoverBLL.SelectArticleCreateInfoById(comment.PKID);
                        if (!string.IsNullOrWhiteSpace(questionDetail?.Topicid) && comment.AuditStatus == 2 && comment.TopicCommentSouceId == 0)
                        {
                            isForum = true;
                            int TopicCommentSouceId = 0;
                            if (comment.ParentID.HasValue)
                            {
                                TopicCommentSouceId = await DiscoverBLL.SelectArticleCommentTopicCommentSouceIdById(comment.ParentID.Value);
                            }
                            await HttpClientHelper.ForumSynchronousData(comment.UserHead, comment.PhoneNum,
                                comment.UserName, comment.UserID, questionDetail.Topicid, comment.CommentContent,
                                comment.CommentImage, comment.UserIdentity, commentId, TopicCommentSouceId);

                        }
                    }


                    //当回答某个问题的时候，进行系统消息通知
                    if (!isForum && ((comment.Type == 2 && comment.ParentID > 0) || comment.Type == 3))
                    {
                        var userName = GetUserName(comment.UserName, comment.RealName, comment.PhoneNum);

                        var userHeade = string.IsNullOrEmpty(comment.UserHead) ? GetDefaultUserHeadByUserGrade(comment.UserHead) :
                                   (comment.UserHead.Contains("http") ? comment.UserHead : DomainConfig.ImageSite + comment.UserHead);
                        try
                        {
                            await UtilityService.PushArticleMessage(parentUserId, 685, comment.PKID, userName);
                            //await InsertPraiseNotice(userName, commentId, comment.Type, userHeade, parentUserId);
                        }
                        catch (Exception ex)
                        {

                            WebLog.LogException("回答通知出错", ex);
                        }
                    }
                    //if (comment.Type == 3 && comment.AuditStatus == 2&& comment.UserIdentity <= 0)
                    //{
                    //    DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                    //    {
                    //        Type = 3,
                    //        ActileId = commentId,
                    //        UserId = Guid.Parse(comment.UserID).ToString("B"),
                    //        Vote = 1
                    //    });
                    //    DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                    //    {
                    //        Type = 6,
                    //        ActileId = commentId,
                    //        UserId = Guid.Parse(comment.UserID).ToString("B"),
                    //        Vote = 1
                    //    });
                    //    Article articleModel = null;
                    //    using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
                    //    {
                    //        var r = await reclient.GetOrSetAsync("DetailById/" + comment.PKID, () => ArticleBll.GetArticleDetailById(comment.PKID), TimeSpan.FromHours(1));
                    //        if (r.Value != null)
                    //            articleModel = r.Value;
                    //    }
                    //    if (articleModel != null && !string.IsNullOrWhiteSpace(articleModel.CategoryTags))
                    //    {
                    //        DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                    //        {
                    //            Type = 8,
                    //            ActileId = commentId,
                    //            UserId = Guid.Parse(comment.UserID).ToString("B"),
                    //            Vote = 1
                    //        });
                    //    }
                    //}
                    dic.Add("CommentId", result.Item1);
                    dic.Add("Code", "1");
                }
                else
                {
                    dic.Clear();
                    dic.Add("Code", "0");
                    dic.Add("Message", "提交失败");
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Error", ex.Message);
                dic.Add("Code", "0");
                dic.Add("Message", "提交失败");
            }
            return Json(dic);
        }

        /// <summary>
        /// 新增点赞通知
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="commentId"></param>
        /// <param name="parentUserId"></param>
        /// <returns></returns>
        private async Task InsertPraiseNotice(string title, int commentId, int questionType, string userHead, string parentUserId, string news = "回答了你的问题")
        {
            //查询用户最近的一次回答通知
            //var lastPushDateTime = await DiscoverBLL.GetLastPushRecord(parentUserId, 2);

            //查询当前用户的个人信息
            //var userResultJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/User/GetUsersInfoForAttention",
            // new Dictionary<string, string> { { "userIds", string.Join(",", parentUserId) } });
            //var userInfo = JsonConvert.DeserializeObject<List<MyAttentionUserList>>(userResultJson).FirstOrDefault();
            var userResultJson = await HttpClientHelper.SelectUserInfo(string.Join(",", parentUserId));
            var userInfo = userResultJson.FirstOrDefault();
            #region 被回答者的信息
            var phoneNumber = string.Empty;
            if (userInfo != null)
            {
                phoneNumber = userInfo.PhoneNumber;
            }
            else
            {
                WebLog.Logger.Info("推送查询用户信息失败用户ID"+parentUserId);
            }
            #endregion

            var androidActivity = string.Empty;
            var androidValue = string.Empty;
            var iosActivity = string.Empty;
            var iosValue = string.Empty;



            //对专题问题的回答
            if (questionType == 2)
            {
                androidActivity = "cn.TuHu.Activity.NewFound.Found.DiscoveryCommentResListAtivity";
                androidValue = "[{'answerId':" + commentId + ",'questionType':4}]";
                iosActivity = "Tuhu.THAnswerVC";
                iosValue = "{ \"answerID\":" + commentId + ", \"type\":\"4\"}";
            }
            //对全局问题的回答
            else if (questionType == 3)
            {
                androidActivity = "cn.TuHu.Activity.NewFound.Found.DiscoveryCommentResListAtivity";
                androidValue = "[{'answerId':" + commentId + ",'questionType':3}]";
                iosActivity = "Tuhu.THAnswerVC";
                iosValue = "{ \"answerID\":" + commentId + ", \"type\":\"3\"}";
            }

            //保证相同的推送，每天只向用户推送一次
            //if (lastPushDateTime.HasValue && lastPushDateTime.Value.Date != DateTime.Now.Date)
            //{
            HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertDiscoveryNoticeAndPush",
      new Dictionary<string, string> {
                                    { "userHead",userHead },
                                    { "userName",title},
                                    { "phoneNumber",phoneNumber},
                                    { "userId", parentUserId },
                                    { "id", commentId.ToString() },
                                    { "news",news},
                                  { "androidKey1","answerId"},
                                  { "androidKey2","questionType"},
                                    { "androidActivity",androidActivity },
                                    { "iosActivity",iosActivity },
                                     { "androidParameter",androidValue },
                                     { "iosParameter",iosValue },
                                     { "commentId",commentId.ToString() },
                                     { "questionType",questionType == 2 ? "4" :"3"},
      });

            //await DiscoverBLL.InsertPushRecord(parentUserId, 2);
            //}
            //只向系统消息中插入消息
            //        else
            //        {
            //            await HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertPraiseNotice",
            //new Dictionary<string, string> {
            //                                { "userHead",userHead },
            //                                { "userName",title},
            //                                { "phoneNumber",phoneNumber},
            //                                { "userId", parentUserId },
            //                                { "id", commentId.ToString() },
            //                                { "news",news},
            //                                 { "androidKey",androidRedictKey },
            //                                { "iosKey",iosRedictKey },
            //                                 { "androidValue",androidRedictValue },
            //                                 { "iosValue",iosRedictValue }
            //});
            //        }
        }

        /// <summary>
        /// 查询可被邀请回答问题的用户列表
        /// </summary>
        /// <param name="questionId">问题ID</param>
        /// <param name="questionUserId">提问者UserId</param>
        /// <param name="userId">被邀请回答者UserId</param>
        /// <returns></returns>
        public async Task<ActionResult> SelectBestAnswerUser(int questionId, string questionUserId = "", string userId = "")
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var questionStatus = await DiscoverBLL.SelectQuestionStatus(questionId);
                if (questionStatus == 1)
                {
                    var bestUserList = await DiscoverBLL.SelectBestAnswerUser(questionId);
                    bestUserList = bestUserList.Where(user => user.UserId.ToLower() != userId.ToLower() && user.UserId.ToLower() != questionUserId.ToLower()).ToList();
                    if (bestUserList.Any())
                    {
                        var userList =await HttpClientHelper.SelectUserInfo(string.Join(",", bestUserList.Select(u => "{"+u.UserId+"}").ToList()));
                        //var userList = JsonConvert.DeserializeObject<List<MyAttentionUserList>>(userResultJson);
                        Uri uriResult;
                        
                        bestUserList.ForEach(u =>
                        {
                            var tempUser = userList.FirstOrDefault(tu => tu.UserId .ToLower() == "{" + u.UserId + "}".ToLower());
                            if (tempUser != null)
                            {
                                u.UserName = UserNameHelper.GetUserName(tempUser.UserName, tempUser.UserName, tempUser.PhoneNumber, tempUser.UserGrade);
                                u.UserHead = Uri.TryCreate(tempUser.UserHead, UriKind.Absolute, out uriResult) == false && string.IsNullOrEmpty(tempUser.UserHead) == false ?
                                                                                                                                        DomainConfig.ImageSite + tempUser.UserHead :
                                                                                                                                        (string.IsNullOrEmpty(tempUser.UserHead) == false ? tempUser.UserHead : GetDefaultUserHeadByUserGrade(tempUser.UserGrade));
                                u.Vehicle = tempUser.Vehicle;
                            }
                            u.UserId = Guid.Parse(u.UserId).ToString("B");
                        });
                    }

                    dic.Add("Code", "1");
                    dic.Add("Users", bestUserList);
                }
                else
                {
                    dic.Add("ArticleStatus", questionStatus);
                    dic.Add("Code", "0");
                    dic.Add("Message", "该问题尚未审核通过！");
                }

            }
            catch (Exception ex)
            {
                WebLog.LogException("查询可被邀请回答问题的用户列表", ex);
                dic.Clear();
                dic["Code"] = "0";
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 邀请用户回答问题
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<ActionResult> InviteUserAnswerQuestion(int questionId, string inviteUserName, string inviteUserHead, string invitedUserId,string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除回答
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult DeleteCommentVersion1(int commentId, int questionId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        [Obsolete]
        public ActionResult DeleteComment(int commentId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新回答
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="content"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult UpdateComment(int commentId, string content, string image = null)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除问题
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="questionType"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult DeleteQuestion(int questionId, int questionType)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑问题
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="content"></param>
        /// <param name="image"></param>
        /// <param name="questionType"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult UpdateQuestion(int questionId, string content, int questionType, string image = "")
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  说说点赞(数据库type=4) 专题内的问题回答点赞(type=4) 全局问题回答点赞(type=4) 标签内文章点赞(type=1)
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="userId"></param>
        /// <param name="vote">-1:反对  1:感谢点赞</param>
        /// <param name="operationTypes">操作类型 1点赞 2关注</param>
        /// <returns></returns>                                                                                                         
        //[HttpPost]
        public async Task<ActionResult> PraiseForComment(int commentId, string userId, int vote,string operationTypes, string pkid = null, string userHead = null, string userName = null, string userGrade = null, string realName = null, string sex = null, string targetUserId = null)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (!string.IsNullOrWhiteSpace(operationTypes) && operationTypes == "2")
                {
                    var result=await DiscoverBLL.InsertFollowRecord(commentId, Guid.Parse(userId).ToString("B"), Convert.ToBoolean(vote));
                    if (result > 0)
                    {
                        DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                        {
                            Type = 1,
                            ActileId = commentId,
                            UserId = Guid.Parse(userId).ToString("B"),
                            Vote = Convert.ToBoolean(vote)
                        });
                    }
                    dic.Add("Code","1");
                    dic.Add("State", result > 0 ? vote : 0);
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                var createInfo = new
                {
                    UserHead = userHead,
                    UserName = userName,
                    UserGrade = userGrade,
                    RealName = realName,
                    Sex = sex
                };


                int articleId;
                //返回当前被点赞内容的类型
                // 2说说 3全局问题的回答 4专题问题的回答
                var operationResult = DiscoverBLL.PraiseForComment(commentId, userId, vote, out articleId, JsonConvert.SerializeObject(createInfo));
                var operationType = operationResult.Item1;


                if (operationType > 0 && operationResult.Item2 == 1)
                {
                    if (string.IsNullOrEmpty(pkid) && articleId > 0)
                        pkid = articleId.ToString();
                    //点赞通知
                    if (vote == 1 && string.IsNullOrEmpty(targetUserId) == false && operationType > 1)
                    {
                        //安卓请求
                        var androidType = Request.Headers.Get("VersionCode");
                        //IOS请求
                        var iosType = Request.Headers.Get("version");


                        //查询当前用户的个人信息
                        var userResultJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/User/GetUsersInfoForAttention",
                                                         new Dictionary<string, string> { { "userIds", string.Join(",", new string[] { userId, targetUserId }) } });

                        var userList = JsonConvert.DeserializeObject<List<MyAttentionUserList>>(userResultJson);
                        var userInfo = userList.Where(u => u.UserId == userId).FirstOrDefault();

                        var targetUserInfo = userList.Where(u => u.UserId == targetUserId).FirstOrDefault();
                        var phoneNumber = string.Empty;

                        if (userInfo != null)
                        {
                            userHead = userInfo.UserHead;
                            userGrade = userInfo.UserGrade;
                            userName = userInfo.UserName;
                        }

                        if (targetUserInfo != null)
                        {
                            phoneNumber = targetUserInfo.PhoneNumber;
                        }
                        await UtilityService.PushArticleMessage(targetUserId, 673, articleId, userName);
                        //var androidRedictKey = string.Empty;
                        //var androidRedictValue = string.Empty;
                        //var iosRedictKey = string.Empty;
                        //var iosRedictValue = string.Empty;
                        //var praiseType = string.Empty;
                        //var news = string.Empty;
                        //var title = string.IsNullOrEmpty(userName) ? GetUserName(userName, realName, "") : userName;
                        ////专题问题的回答
                        //if (operationType == 4)
                        //{
                        //    praiseType = "回答";
                        //    androidRedictKey = "cn.TuHu.Activity.NewFound.Found.DiscoveryCommentResListAtivity";
                        //    iosRedictKey = "Tuhu.THAnswerVC";
                        //    androidRedictValue = "[{'answerId':" + commentId + ",'questionType':4}]";
                        //    iosRedictValue = "{\"answerID\":" + commentId + ",\"type\":\"4\"}";
                        //    news = "感谢了你的回答!";

                        //}
                        ////全局问题的回答
                        //else if (operationType == 3)
                        //{
                        //    praiseType = "回答";
                        //    androidRedictKey = "cn.TuHu.Activity.NewFound.Found.DiscoveryCommentResListAtivity";
                        //    iosRedictKey = "Tuhu.THAnswerVC";
                        //    androidRedictValue = "[{'answerId':" + commentId + ",'questionType':3}]";
                        //    iosRedictValue = "{\"answerID\":" + commentId + ",\"type\":\"3\"}";
                        //    news = "感谢了你的回答!";
                        //}
                        //else if (operationType == 2)
                        //{
                        //    praiseType = "说说";
                        //    androidRedictKey = "cn.TuHu.Activity.NewFound.Special.SpecialActivity";
                        //    iosRedictKey = "THDiscoverSubjectVC";
                        //    androidRedictValue = "[{'articleId':" + pkid + ",'currentIndex':'2'}]";
                        //    iosRedictValue = "{\"PKID\":\"" + pkid + "\",\"message\":1}";
                        //    news = "赞了你的说说!";
                        //}



                        //HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertDiscoveryNoticeAndPush",
                        //    new Dictionary<string, string> {
                        //        { "userHead",string.IsNullOrEmpty(userHead) ? GetDefaultUserHeadByUserGrade(userGrade) : DomainConfig.ImageSite + userHead},
                        //        { "userName",title},
                        //        { "phoneNumber",phoneNumber},
                        //        { "userId", targetUserId },
                        //        { "id", commentId.ToString() },
                        //        { "news",news},
                        //        { "androidKey1","answerId"},
                        //        { "androidKey2","questionType"},
                        //        { "androidActivity",androidRedictKey },
                        //        { "iosActivity",iosRedictKey },
                        //         { "androidParameter",androidRedictValue },
                        //        { "commentId",commentId.ToString()},
                        //         { "iosParameter",iosRedictValue },
                        //          { "questionType","3"},
                        //    });
                    }
                }
                if (!string.IsNullOrWhiteSpace(operationTypes) && operationTypes == "1")
                {
                    var Vote = operationResult.Item2 == 1 ? true : false;
                    DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                    {
                        Type = 2,
                        ActileId = commentId,
                        UserId = Guid.Parse(userId).ToString("B"),
                        Vote = Vote
                    });
                }
                
                dic.Add("Code", "1");
                dic.Add("State", operationResult.Item2);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Message", "操作失败");
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 关注问题
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="questionId">问题Id</param>
        /// <param name="vote">点赞/取消点赞</param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult VoteQuestion(string userId, int questionId, int vote)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 我的提问
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult SelectMyQuestions(string userId, string targetUserId = null, int pageNumber = 1, int questionType = 1)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                targetUserId = targetUserId == "" ? null : targetUserId;
                var page = new PagerModel(pageNumber, 10);
                var questions = DiscoverBLL.SelectMyQuestions(userId, targetUserId, page, questionType);
                dic.Add("Questions", questions);
                dic.Add("TotalPage", page.TotalPage);
                dic.Add("QuestionCount", page.TotalItem);
                dic.Add("Code", "1");
            }
            catch (Exception ex)
            {
                dic.Add("Error", ex.Message);
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        [Obsolete]
        public ActionResult GetMyQuestionCout(string userId, int questionType = 1)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询发现个人首页信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyHomePageForDiscovery(string userId, string targetUserId = null, int userIdentity = 0, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                targetUserId = targetUserId == "" ? null : targetUserId;
                int isVote = 0;
                //是否关注了目标用户
                if (string.IsNullOrEmpty(targetUserId) == false)
                    isVote = await DiscoverBLL.IsAttentionTargetUser(userId, targetUserId);
                //userId = string.IsNullOrEmpty(targetUserId) == false ? targetUserId : userId;

                //查询员工或者技师的个人信息
                if (userIdentity > 0)
                {
                    //技师的回答数量，我的回答列表(2条)
                    var myAnswerPage = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                    if (pageIndex == 1)
                    {
                        var myAnswers = await DiscoverBLL.SelectMyAnswer(userId, targetUserId, myAnswerPage);
                        var myAnswersCount = myAnswerPage.TotalItem;

                        //关注技师的人
                        var attentionToMeUserCount = await DiscoverBLL.SelectAttentionToMeUserCount(targetUserId);
                        //技师收到的感谢
                        var myPraiseCount = await DiscoverBLL.SelectMyPraiseCount(targetUserId);

                        var discoveryHomeData = new
                        {
                            IsVote = isVote,
                            MyAnswerCount = myAnswersCount,
                            MyAnswer = myAnswers,
                            AttentionToMeUserCount = attentionToMeUserCount,
                            MyPraiseCount = myPraiseCount
                        };
                        dic.Add("Data", discoveryHomeData);
                        dic.Add("TotalPage", myAnswerPage.TotalPage);
                        dic.Add("TotalCount", myAnswerPage.TotalItem);
                    }
                    else
                    {
                        var myAnswers = await DiscoverBLL.SelectMyAnswer(userId, targetUserId, myAnswerPage);
                        dic.Add("Data", new { MyAnswer = myAnswers });
                    }

                    dic.Add("Code", "1");
                }

                else
                {
                    var versionNumber = Request.Headers.Get("VersionCode");
                    var version = Request.Headers.Get("version");

                    //我关注的人数量
                    var myAttentionUserCount = await DiscoverBLL.SelectMyAttentionUserCount(string.IsNullOrEmpty(targetUserId) ? userId : targetUserId);
                    //关注我的人
                    var attentionToMeUserCount = await DiscoverBLL.SelectAttentionToMeUserCount(string.IsNullOrEmpty(targetUserId) ? userId : targetUserId);
                    //收到的感谢
                    var myPraiseCount = await DiscoverBLL.SelectMyPraiseCount(string.IsNullOrEmpty(targetUserId) ? userId : targetUserId);
                    //我的提问数量
                    //var myQuestionsCount= DiscoverBLL.GetMyQuestionCount(userId, 3);
                    //我的提问列表(显示2条)
                    var myQuestionPage = new PagerModel { CurrentPage = 1, PageSize = 2 };
                    var myQuestions = DiscoverBLL.SelectMyQuestions(userId, targetUserId, myQuestionPage, 3);
                    //我的回答数量，我的回答列表(2条)
                    var myAnswerPage = new PagerModel { CurrentPage = 1, PageSize = 2 };
                    var myAnswers = await DiscoverBLL.SelectMyAnswer(userId, targetUserId, myAnswerPage);
                    var myAnswersCount = myAnswerPage.TotalItem;
                    //我关注的问题
                    var myAttentionQuestionPage = new PagerModel { CurrentPage = 1, PageSize = 2 };
                    var myAttentionQuestion = await DiscoverBLL.SelectMyAttentionQuestions(userId, targetUserId, myAttentionQuestionPage);
                    //我关注的专题
                    var myAttentionSubjectPage = new PagerModel { CurrentPage = 1, PageSize = 2 };
                    var myAttentionSubject = await DiscoverBLL.SelectMyAttentionSubjects(string.IsNullOrEmpty(targetUserId) ? userId : targetUserId, myAttentionSubjectPage);
                    //我关注的标签
                    var myAttentionCategoryPage = new PagerModel { CurrentPage = 1, PageSize = 2 };
                    var myAttentionCategory = await DiscoverBLL.SelectMyAttentionCategorys(string.IsNullOrEmpty(targetUserId) ? userId : targetUserId, myAttentionCategoryPage);
                    //我关注的文章
                    var myAttentionArticlePage = new PagerModel { CurrentPage = 1, PageSize = 2 };
                    var myAttentionArticle = await DiscoverBLL.SelectMyAttentionArticles(string.IsNullOrEmpty(targetUserId) ? userId : targetUserId, myAttentionArticlePage);

                    //if (((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "50") >= 0) ||
                    //    (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.5") >= 0))))
                    //{
                    //    myAttentionArticle.ForEach(article =>
                    //    {
                    //        article.ContentUrl = DiscoverySite + "/Article/Detail?Id=" + article.RelatedArticleId;
                    //    });
                    //}
                    //我提交的晒图
                    var mySubmitShareImages = await DiscoverBLL.SelectMyShareImages(string.IsNullOrEmpty(targetUserId),string.IsNullOrEmpty(targetUserId) ? userId : targetUserId, new PagerModel(1,3));
                    var mySubmitShareImagesCount = await DiscoverBLL.SelectMyShareImagesCount(string.IsNullOrEmpty(targetUserId), string.IsNullOrEmpty(targetUserId) ? userId : targetUserId);
                    var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                    myAttentionArticle.ForEach(article =>
                    {
                        article.ArticleShowMode = articleShowMode;
                        article.ContentUrl = (articleShowMode == "New" || article.Type == 5) ? DomainConfig.FaXian + "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + article.PKID : article.ContentUrl;
                    });

                    var discoveryHomeData = new
                    {
                        IsVote = isVote,
                        MyAttentionUserCount = myAttentionUserCount,
                        AttentionToMeUserCount = attentionToMeUserCount,
                        MyPraiseCount = myPraiseCount,
                        MyQuestionCount = myQuestionPage.TotalItem,
                        MyQuestion = myQuestions,
                        MyAnswerCount = myAnswersCount,
                        MyAnswer = myAnswers,
                        MyAttentionQuestionCount = myAttentionQuestionPage.TotalItem,
                        MyAttentionQuestion = myAttentionQuestion,

                        MyAtentionSubjectCount = myAttentionSubjectPage.TotalItem,
                        MyAttentionSubject = myAttentionSubject,
                        MyAttentionCategoryCount = myAttentionCategoryPage.TotalItem,
                        MyAttentionCategory = myAttentionCategory.Select(category =>
                        new MyAttentionCategory
                        {
                            PKID = category.PKID,
                            Content = category.Content,
                            SmallImage = category.SmallImage,
                            SmallTitle = category.SmallTitle
                        }),
                        MyAttentionArticleCount = myAttentionArticlePage.TotalItem,
                        MyAttentionArticle = myAttentionArticle,
                        mySubmitShareImages,
                        mySubmitShareImagesCount
                    };
                    dic.Add("Code", "1");
                    dic.Add("Data", discoveryHomeData);
                }


                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException("查询发现个人首页信息", ex);
                dic.Clear();
                dic.Add("Code", "0");
                var innerException = ex.InnerException == null ? "无" : ex.InnerException.Message;
                dic.Add("Messages", ex.Message + innerException);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 查询技师个人信息
        /// </summary>
        /// <param name="technicianId"></param>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectTechnicianInfo(int technicianId, int userIdentity)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                TechnicianModel techInfo = new TechnicianModel();
                //员工
                if (userIdentity == 1)
                {
                    //查询员工信息
                    var resultJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/SelectShopEmployeeById",
                           new Dictionary<string, string> { { "employeeId", technicianId.ToString() } });
                    techInfo = JsonConvert.DeserializeObject<TechnicianModel>(resultJson);
                    //techInfo.UserHead = string.IsNullOrEmpty(techInfo.UserHead) ? "http://resource.tuhu.cn/Image/Product/jinlaohu.png" : techInfo.UserHead;
                }
                //技师
                else if (userIdentity == 2)
                {
                    var resultJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/SelectTechnicianById",
                           new Dictionary<string, string> { { "techId", technicianId.ToString() } });
                    techInfo = JsonConvert.DeserializeObject<TechnicianModel>(resultJson);
                    //techInfo.UserHead = "http://resource.tuhu.cn/Image/Product/jinlaohu.png";
                }

                //查询员工资质
                var certificateInfo = HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/SelectCertificateByTechnicianId",
                       new Dictionary<string, string> { { "techId", technicianId.ToString() }, { "userIdentity", userIdentity.ToString() } });
                techInfo.UserIdentityName = certificateInfo == "\"\"" ? "门店技师" : certificateInfo;

                //查询擅长的品牌

                dic.Add("Data", techInfo);
                dic.Add("Code", "1");
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "查询失败");
                WebLog.LogException(ex);
            }

            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询车主回复消息数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectReceiveMessageCount(string userId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var count = await DiscoverBLL.SelectReceiveMessageCount(userId);
                dic.Add("MessageCount", count);
                dic.Add("Code", "1");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Code", "查询车主回复消息数量失败");
                WebLog.LogException(ex);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改回复消息为已读
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<ActionResult> ReadReplyMessage(int commentId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var result = await DiscoverBLL.ReadReplyMessage(commentId);
                if (result == 1)
                    dic.Add("Code", "1");
                else
                    dic.Add("Code", "0");
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "操作失败");
                WebLog.LogException(ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        //// <summary>
        /// 查询技师的推荐问题
        /// </summary>
        /// <param name="userId">技师ID</param>
        /// <param name="vehicleIds">擅长的车型</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据的数量</param>
        /// <returns></returns>
        public async Task<ActionResult> SelectRecomandQuestion(string userId, string vehicleIds, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var list = await DiscoverBLL.SelectRecomandQuestion(userId, vehicleIds, pager);
                var questionList = new ArrayList();

                list.ForEach(q =>
                {
                    questionList.Add(new
                    {
                        Content = q.CommentContent,
                        Id = q.Id,
                        PkId = q.PKID,
                        UserId = q.UserId,
                        UserName = q.UserName,
                        UserHead = q.UserHead.StartsWith("http") ? q.UserHead : q.UserHead.GetImageUrl(),
                        Images = q.Image,
                        QuestionType = q.QuestionType,
                        CommentTimes = q.CommentTimes,
                        Vehicle = q.Vehicle,
                        VehicleId = q.VehicleId
                    });
                });
                dic.Add("Data", new { Questions = questionList, Count = pager.TotalItem, TotalPage = pager.TotalPage });
                dic.Add("Code", "1");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Error", ex.InnerException.Message);
                dic.Add("Message", "查询失败");
                dic.Add("Code", "0");
                WebLog.LogException(ex);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询车主的回复
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectOwnerReply(string userId, int? useridentity, int pageIndex = 1, int PageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = PageSize };
                var result = await DiscoverBLL.SelectOwnerReply(userId, pager, useridentity);
                var list = result.Item2;
                if (list != null && list.Any())
                {
                    list.ForEach(x =>
                    {
                        if (x.QuestionType == 0)
                            x.QuestionType = 3;

                        if (!string.IsNullOrWhiteSpace(x.UserHead2) && !x.UserHead2.StartsWith("http"))
                            x.UserHead2 = x.UserHead2.GetImageUrl();
                    });
                }
                
                dic.Add("Data", new { Replys = list, Count = pager.TotalItem, TotalPage = pager.TotalPage, UnReadCount = result.Item1 });
                dic.Add("Code", "1");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Message", "查询失败");
                dic.Add("Code", "0");
                WebLog.LogException(ex);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询用户的被感谢数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyPraiseCount(string userId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                //收到的感谢
                var myPraiseCount = await DiscoverBLL.SelectMyPraiseCount(userId);
                dic.Add("Code", "1");
                dic.Add("PraiseCount", myPraiseCount);
            }
            catch (Exception)
            {
                dic.Clear();
                dic.Add("Message", "查询失败");
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询我关注的人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelctMyAttentionUser(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object> {{"Code", "1"}, {"TotalCount", "0"}, {"TotalPage", "0"}, {"Data", new {MyAttentionUser = new List<MyAttentionUserList> {}}}};
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询关注我的人的列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelctMyAttentionMeUser(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {

                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var userIds = await DiscoverBLL.SelctAttentionMeUser(userId, pager);



                dic.Add("Code", "1");

                dic.Add("TotalCount", pager.TotalItem);
                dic.Add("TotalPage", pager.TotalPage);

                if (userIds != null && userIds.Any())
                {

                    //请求api.tuhu.cn 接口，返回关注的用户信息
                    var resultJson = await HttpClientHelper.SelectUserInfo(string.Join(",", userIds.Select(u => u.Item1)));
                    if (resultJson!=null&& resultJson.Count>0)
                    {

                        //反序列化数据
                        //var result = JsonConvert.DeserializeObject<List<MyAttentionUserList>>(resultJson);
                        resultJson.ForEach(u =>
                        {
                            u.IsAttention = !userIds.Any(x => x.Item1.Equals(u.UserId, StringComparison.OrdinalIgnoreCase)) ? 0 : userIds.FirstOrDefault(uu => uu.Item1.Equals(u.UserId, StringComparison.OrdinalIgnoreCase)).Item2;
                            u.UserHead = string.IsNullOrEmpty(u.UserHead) ? GetDefaultUserHeadByUserGrade(u.UserGrade) : DomainConfig.ImageSite + u.UserHead;
                            u.UserName = GetUserName(u.UserName, u.UserName, "");
                        });
                        dic.Add("Data", new { MyAttentionUser = resultJson });
                    }
                    else
                    {
                        dic.Add("Data", new { MyAttentionUser = new List<MyAttentionUserList> { } });
                    }
                    
                }
                else
                {
                    dic.Add("Data", new { MyAttentionUser = new List<MyAttentionUserList> { } });
                }
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Error", ex.Message);
                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询我的回答点赞列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyPraiseForAnswer(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var page = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var result = await DiscoverBLL.SelectMyPraiseForAnswer(userId, page);
                dic.Add("Code", "1");
                dic.Add("Data", new { MyAnswerPraises = result });
                dic.Add("TotalCount", page.TotalItem);
                dic.Add("TotalPage", page.TotalPage);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Message", "查询失败");
                dic.Add("Code", "0");
                WebLog.LogException(ex);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> SelectCommentCountByVehicle(string VehicleId)
        {
            var dic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(VehicleId))
            {
                dic.Add("Code", "0");
                dic.Add("Message", "参数错误");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            var Count=await DiscoverBLL.SelectCommentCountByVehicle(VehicleId);
            dic.Add("Code", "1");
            dic.Add("Count", Count);
            return Json(dic, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 关注人
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="attentionUserId">被关注用户Id</param>
        /// <param name="isAttention">是否关注 1为关注 0为不关注</param>
        /// <returns></returns>
        public async Task<ActionResult> ConcernUser(string userId, string attentionUserId, int isAttention, int userIdentity = 0)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(attentionUserId))
                {
                    dic.Add("Code", "0");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(userId) == false && userId == attentionUserId)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "哎呦，自己就不要关注自己啦！");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }

                var result = await DiscoverBLL.ConcernUser(userId, attentionUserId, isAttention, userIdentity);
                if (result.Item1 > 0)
                {
                    dic.Add("Code", "1");
                    dic.Add("State", isAttention);
                    dic.Add("AttentionToMeUserCount", result.Item2);

                    var versionNumber = Request.Headers.Get("VersionCode");

                    if (isAttention == 1)
                    {
                        var userResultJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/User/GetUsersInfoForAttention",
                                                         new Dictionary<string, string> { { "userIds", string.Join(",", userId) } });

                        var userInfo = JsonConvert.DeserializeObject<List<MyAttentionUserList>>(userResultJson).FirstOrDefault();
                        if (userInfo != null)
                        {

                            var title = string.IsNullOrEmpty(userInfo.UserName) ? GetUserName(userInfo.UserName, userInfo.UserName, "") : userInfo.UserName;
                            HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertPraiseNotice",
            new Dictionary<string, string> {
                                      { "userHead",string.IsNullOrEmpty(userInfo.UserHead) ? GetDefaultUserHeadByUserGrade(userInfo.UserGrade) :  DomainConfig.ImageSite + userInfo.UserHead},
                                { "userName",title},
                                { "phoneNumber",userInfo.PhoneNumber},
                                { "userId", attentionUserId },
                                { "id", attentionUserId.ToString() },
                                { "news","关注了你"},

                                { "androidKey","cn.TuHu.Activity.Found.PersonalPage.FoundDividerItem" },
                                { "iosKey", "Tuhu.THDiscoverFollowListVC"},
                                 { "androidValue","[{'intotype':'man_concerning_me','userId':'"+attentionUserId+"','isOther':false,'num':2}]" },
                                 { "iosValue", "{\"userId\":\"" + attentionUserId + "\",\"titleName\":\"关注我的人\"}"  }  });
                        }
                    }
                }
                else
                {

                    dic.Add("Code", "0");
                }

                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException("关注、取消关注用户", ex);
                dic.Clear();

                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询我关注的问题
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyAttentionQuestion(string userId, string targetUserId = null, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                targetUserId = targetUserId == "" ? null : targetUserId;
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var result = await DiscoverBLL.SelectMyAttentionQuestions(userId, targetUserId, pager);
                dic.Add("Code", "1");
                dic.Add("Data", new { AttentionQuestions = result });
                dic.Add("TotalCount", pager.TotalItem);
                dic.Add("TotalPage", pager.TotalPage);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询我的回答
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyAnswer(string userId, int RelatedVoteType = 0, string targetUserId = null, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                targetUserId = targetUserId == "" ? null : targetUserId;
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var result = await DiscoverBLL.SelectMyAnswer(userId, targetUserId, pager, RelatedVoteType);
                dic.Add("Code", "1");
                dic.Add("Data", new { Answers = result });
                dic.Add("TotalCount", pager.TotalItem);
                dic.Add("TotalPage", pager.TotalPage);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Message", "查询异常");
                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询我关注的专题
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectMyAttentionSubject(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询我关注的标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyAttentionCategory(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var result = await DiscoverBLL.SelectMyAttentionCategorys(userId, pager);

                dic.Add("Code", "1");
                dic.Add("Data", new { Category = result });
                dic.Add("TotalCount", pager.TotalItem);
                dic.Add("TotalPage", pager.TotalPage);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询我关注的文章
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectMyAttentionArticle(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                var result = await DiscoverBLL.SelectMyAttentionArticles(userId, pager);

                var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();

                result.ForEach(article =>
                {
                    article.ArticleShowMode = articleShowMode;
                    article.ContentUrl = (articleShowMode == "New" || article.Type == 5) ? DomainConfig.FaXian + "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + article.PKID : article.ContentUrl;
                });
                dic.Add("Code", "1");
                dic.Add("Data", new { Articles = result });
                dic.Add("TotalCount", pager.TotalItem);
                dic.Add("TotalPage", pager.TotalPage);
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                dic.Clear();
                dic.Add("Code", "0");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 转译显示用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="realName"></param>
        /// <param name="phone"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public string GetUserName(string userName, string realName, string phone, string sex = "")
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                if (userName.StartsWith("1") && userName.Length == 11)
                {
                    userName = userName.Substring(0, 3) + "****" + userName.Substring(7, 4);
                }

                return userName;
            }
            else if (!string.IsNullOrWhiteSpace(realName))
            {
                if (realName.StartsWith("1") && realName.Length == 11)
                {
                    realName = realName.Substring(0, 3) + "****" + realName.Substring(7, 4);
                }
                else
                {
                    if (sex == "男")
                    {
                        realName = realName.Substring(0, 1) + "先生";
                    }
                    else if (sex == "女")
                    {
                        realName = realName.Substring(0, 1) + "小姐";
                    }
                    else
                    {
                        realName = realName.Substring(0, 1) + "先生";
                    }
                }
                return realName;
            }
            else if (!string.IsNullOrWhiteSpace(phone))
            {
                return phone.Substring(0, 3) + "****" + phone.Substring(7, 4);
            }
            else
            {
                return "途虎用户";
            }
        }

        private static string GetDefaultUserHeadByUserGrade(string userGrade)
        {
            string userHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
            switch (userGrade)
            {
                case "银卡会员":
                case "V1":
                    userHead = "http://resource.tuhu.cn/Image/Product/bulaohu.png";
                    break;
                case "金卡会员":
                case "V2":
                    userHead = "http://resource.tuhu.cn/Image/Product/mulaohu.png";
                    break;
                case "白金卡会员":
                case "V3":
                    userHead = "http://resource.tuhu.cn/Image/Product/tielaohu.png";
                    break;
                case "黑金卡会员":
                case "V4":
                    userHead = "http://resource.tuhu.cn/Image/Product/tonglaohu.png";
                    break;
                default:
                    userHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                    break;
            }
            return userHead;
        }
        

        /// <summary>
        /// 判断一个时间是否位于指定的时间段内
        /// </summary>
        /// <param name="duration">时间区间字符串,格式：22:30-08:00</param>
        /// <returns></returns>
        public static bool IsLegalTime(DateTime dt, string duration)
        {
            string[] splits = duration.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            string start = splits[0].Replace(":", "").Substring(0, 4) + "00";
            string end = splits[1].Replace(":", "").Substring(0, 4) + "00";
            string time_intervals = "000000-000000";
            if (Convert.ToInt32(start.Substring(0, 2)) > Convert.ToInt32(end.Substring(0, 2)))
            {
                time_intervals = start + "-235959;000000-" + end;
            }
            else
            {
                time_intervals = start + "-" + end;
            }
            //当前时间
            int time_now = dt.Hour * 10000 + dt.Minute * 100 + dt.Second;
            //查看各个时间区间
            string[] time_interval = time_intervals.Split(';');
            foreach (string time in time_interval)
            {
                //空数据直接跳过
                if (string.IsNullOrWhiteSpace(time))
                {
                    continue;
                }
                //一段时间格式：六个数字-六个数字
                if (!System.Text.RegularExpressions.Regex.IsMatch(time, "^[0-9]{6}-[0-9]{6}$"))
                {
                    return false;
                }
                string timea = time.Substring(0, 6);
                string timeb = time.Substring(7, 6);
                int time_a, time_b;
                //尝试转化为整数
                if (!int.TryParse(timea, out time_a))
                {
                    return false;
                }
                if (!int.TryParse(timeb, out time_b))
                {
                    return false;
                }
                //如果当前时间不小于初始时间，不大于结束时间，返回true
                if (time_a <= time_now && time_now <= time_b)
                {
                    return true;
                }
            }
            //不在任何一个区间范围内，返回false
            return false;
        }
    }

    public static class DistributedCacheHelper
    {
        public static List<string> CacheKeys = new List<string>();

        private static readonly string _cachePrefix = (string)ConfigurationManager.AppSettings["CacheKeyProfix"];

        private static readonly string _DiscoveryHomeCategoryTag = "DiscoveryHomeCategoryTags";
        private static readonly string _DiscoveryHomeTopBanner = "DiscoveryHomeTopBanner";
        private static readonly string _DiscoveryHomeCache = "DiscoveryCache";
        private static readonly string _DiscoveryHomeCacheKey1 = "DiscoveryCacheKey1";
        private static readonly string _DiscoveryAboutMyCarArticle = "DiscoveryAboutMyCarArticleCache";

        //发现板块中的车型父级标签
        private static readonly string _DiscoveryParentCategoryList = "DiscoveryParentCategoryList";

        //发现板块中的车型二级标签
        private static readonly string _DiscoverySecondLevelCategoryList = "DiscoverySecondLevelCategoryList";

        //官网发现标签缓存
        private static readonly string _DiscoveryHomeCategoryTagForWeb = "DiscoveryHomeCategoryTagForWeb";

        private static readonly string _DiscoverySubjectDynamicCache = "DiscoverySubjectDynamicCache";
        //最热门问答
        private static readonly string _DiscoveryPopularAnswersCache = "DiscoveryPopularAnswers";
        //待回答问题
        private static readonly string _DiscoveryToAnswerQuestions = "DiscoveryToAnswerQuestions";


        public static string PopularSource = string.Empty;

        private static readonly string _key = "/";
        //5分钟缓存
        private static readonly TimeSpan defaultCacheTime = new TimeSpan(0, 5, 0);
        public async static Task<List<CategoryTagModel>> SelectHomeCategoryTagsFromCache()
        {
            var categoryTags = GetCacheDataFromCache<List<CategoryTagModel>>(_DiscoveryHomeCategoryTag);
            if (categoryTags == null)
            {
                categoryTags = await DiscoverBLL.SelectHomeCategoryTags();
                SetCacheData(_DiscoveryHomeCategoryTag, categoryTags, DateTime.Now.AddDays(30));
            }
            return categoryTags;
        }


        private static string DiscoverySite = System.Configuration.ConfigurationManager.AppSettings["DiscoverySite"];


        public static DiscoverySubjectDynamicCacheModel SelectSubjectDynamicFromCache(int subjectId, PagerModel page)
        {
            var cacheKey = string.Concat(_DiscoverySubjectDynamicCache, _key, subjectId, _key, page.CurrentPage);
            //var data = GetCacheDataFromCache<DiscoverySubjectDynamicCacheModel>(cacheKey);
            //if (data != null)
            //    return data;
            var cacheData = new DiscoverySubjectDynamicCacheModel();
            cacheData.Source = DiscoverBLL.SelectSubjectDynamic(subjectId, page);
            cacheData.TotalPage = page.TotalPage;
            SetCacheData(cacheKey, cacheData, defaultCacheTime);
            return cacheData;
        }

        /// <summary>
        /// 从缓存中读取关于我的车型的问答
        /// </summary>
        /// <param name="page"></param>
        /// <param name="vehicleId"></param>
        /// <param name="discoveryAboutMyCarArticleCacheKey"></param>
        /// <param name="cacheData"></param>
        /// <returns></returns>
        private static async Task SelectAboutMyCarQuestionFromCache(PagerModel page, string vehicleId, string discoveryAboutMyCarArticleCacheKey, DiscoveryArticleCacheModel cacheData, int sortType = 0, string userId = null, int isHome = 1)
        {
            //车型编号不为空
            if (string.IsNullOrEmpty(vehicleId) == false && page.CurrentPage == 1)
            {
                //与我的车型相关的问答
                cacheData.ArticleAboutMyCar = (await SelectAboutMyCarQuestions(page, vehicleId, sortType, userId, isHome)).Item1;
            }
        }


        /// <summary>
        /// 从缓存中取出发现板块中的父级标签
        /// </summary>
        /// <returns></returns>
        public async static Task<List<RecommandTopic>> SelectCategorysForDiscover()
        {
            var parentCategory = GetCacheDataFromCache<List<RecommandTopic>>(_DiscoveryParentCategoryList);
            if (parentCategory != null)
                return parentCategory;
            parentCategory = await DiscoverBLL.SelectCategorysForDiscover();

            SetCacheData<List<RecommandTopic>>(_DiscoveryParentCategoryList, parentCategory, defaultCacheTime);
            return parentCategory;
        }

        /// <summary>
        /// 根据用户和父级标签，查询二级标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public async static Task<List<CategoryTagModel>> SelectCategoryByParentId(string userId, int parentCategoryId)
        {
            var secondeLevelCategory = await DiscoverBLL.SelectCategoryByParentId(userId, parentCategoryId);
            return secondeLevelCategory;
        }


        /// <summary>
        /// 查询首页文章
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static List<DiscoveryModel> SelectHomeArticle(PagerModel page, int categoryId = 1, bool isExpectQuestion = false, int sortType = 0)
        {
            var articles = DiscoverBLL.SelectHomeArticle(page, categoryId, isExpectQuestion, sortType);
            if (articles != null && articles.Any())
            {
                articles.ForEach(article =>
                {
                    //专题
                    if (article.Type == 2)
                    {
                        var pageItem = new PagerModel(1, 10);
                        var item = SelectSubjectDynamicFromCache(article.PKID, pageItem);
                        if (item != null && item.Source != null && item.Source.Count > 0)
                            article.SubjectContent = item.Source.FirstOrDefault();
                    }
                });
                return articles.Where(a => a.Type != 2 || a.SubjectContent != null).ToList();
            }
            return new List<DiscoveryModel>();
        }



        /// <summary>
        /// 从缓存中查询官网发现的标签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CategoryTagModel>> DiscoveryHomeCategoryTagForWebFromCache()
        {
            var categories = GetCacheDataFromCache<List<CategoryTagModel>>(_DiscoveryHomeCategoryTagForWeb);
            if (categories != null && categories.Any())
                return categories;
            categories = await DiscoverBLL.SelectDiscoveryHomeCategoryForWeb();
            SetCacheData(_DiscoveryHomeCategoryTagForWeb, categories, defaultCacheTime);
            return categories;
        }

        /// <summary>
        /// 查询专题列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public static async Task<List<DiscoveryModel>> SelectSubjects(PagerModel page, int sortType = 0)
        {
            var subjects = await DiscoverBLL.SelectSubjects(page, sortType);
            return subjects;
        }
        /// <summary>
        /// 查询标签的别名
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async static Task<string> SelectCategoryNickName(string categoryName)
        {
            return await DiscoverBLL.SelectCategoryNickName(categoryName);
        }

        /// <summary>
        /// 分页查询最热门问答
        /// </summary>
        /// <returns></returns>
        public async static Task<Tuple<string, List<DiscoveryModel>>> SelectPopularAnswers(PagerModel page, int categoryId = 1, int isHomePage = 0, int sortType = 0, string userId = null, string vehicleId = null)
        {
            //var cacheKey = string.Concat(_DiscoveryPopularAnswersCache, _key, categoryId, _key, isHomePage, _key, sortType, _key, page.CurrentPage);
            //var questions = GetCacheDataFromCache<List<DiscoveryModel>>(cacheKey);
            //if (questions != null && questions.Any())
            //{
            //    var popularNickName = GetCacheDataFromCache<string>(_DiscoveryPopularAnswersCache);
            //    var totalItem = GetCacheDataFromCache<int>(string.Concat(_DiscoveryPopularAnswersCache, "TotalPage"));
            //    page.TotalItem = totalItem;
            //    return new Tuple<string, List<DiscoveryModel>>(popularNickName, questions);
            //}
            //var popularData = await DiscoverBLL.SelectPopularAnswers(page, isHomePage, categoryId, sortType, userId, vehicleId);
            //if (popularData == null)
            //    return null;
            //SetCacheData<string>(_DiscoveryPopularAnswersCache, popularData.Item1, defaultCacheTime);
            //SetCacheData<List<DiscoveryModel>>(cacheKey, popularData.Item2, defaultCacheTime);
            //SetCacheData<int>(string.Concat(_DiscoveryPopularAnswersCache, "TotalItem"), page.TotalItem, defaultCacheTime);
            var popularData = new Tuple<string, List<DiscoveryModel>>(null,null);
            using (var reclient = CacheHelper.CreateCacheClient("PopularAnswers"))
            {
                var result = await reclient.GetOrSetAsync($"List2/{categoryId}/{isHomePage}/{sortType}/{page.CurrentPage}/{page.PageSize}/{userId}/{vehicleId}", () => DiscoverBLL.SelectPopularAnswers(page, isHomePage, categoryId, sortType, userId, vehicleId), TimeSpan.FromHours(1));
                if (result.Value != null)
                {
                    var r = result.Value;
                    popularData = new Tuple<string, List<DiscoveryModel>>(r.Item1, r.Item2);
                    page.TotalItem = r.Item3;


                }
                   
            }
            return popularData;
        }

        /// <summary>
        /// 分页查询最热门问答新
        /// </summary>
        /// <returns></returns>
        public async static Task<List<DiscoveryModel>> SelectPopularAnswersNew(PagerModel page)
        {


            using (var reclient = CacheHelper.CreateCacheClient("PopularAnswers"))
            {
                var result =
                    await
                        reclient.GetOrSetAsync($"ListTEST/{page.CurrentPage}/{page.PageSize}",
                            () => DiscoverBLL.SelectPopularAnswers(page), TimeSpan.FromMinutes(1));

                return result.Value?.ToList();

            }
        }


        /// <summary>
        /// 查询关于我的车型的问答
        /// </summary>
        /// <param name="page"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async static Task<Tuple<List<DiscoveryModel>, int>> SelectAboutMyCarQuestions(PagerModel page, string vehicleId, int sortType = 0, string userId = null, int isHome = 1)
        {
            var cacheKey = string.Concat(_DiscoveryAboutMyCarArticle, _key, isHome, _key, vehicleId, _key, sortType, _key, page.CurrentPage);
            var questions = GetCacheDataFromCache<List<DiscoveryModel>>(cacheKey);
            if (questions != null && questions.Any())
            {
                var vehicleCategoryId = GetCacheDataFromCache<int>(string.Concat(_DiscoveryAboutMyCarArticle, _key, vehicleId));
                return new Tuple<List<DiscoveryModel>, int>(questions, vehicleCategoryId);
            }
            //与我的车型相关的问答
            var articleAboutMyCar = await DiscoverBLL.GetQuestionAboutMyCar(vehicleId, page, sortType, userId, isHome);

            SetCacheData<int>(string.Concat(_DiscoveryAboutMyCarArticle, _key, vehicleId), articleAboutMyCar.Item2, defaultCacheTime);
            SetCacheData<List<DiscoveryModel>>(cacheKey, articleAboutMyCar.Item1, defaultCacheTime);

            return articleAboutMyCar;
        }
        
        /// <summary>
        ///分页 查询待回答问题列表
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async static Task<Tuple<string, List<DiscoveryModel>>> SelectToAnswerQuestions(PagerModel page, int isHomePage = 0, int sortType = 0)
        {
            var cacheKey = string.Concat(_DiscoveryToAnswerQuestions, _key, sortType, isHomePage, page.CurrentPage);
            var pageCacheKey = string.Concat(_DiscoveryToAnswerQuestions, _key, "pageCount");
            var data = GetCacheDataFromCache<List<DiscoveryModel>>(cacheKey);
            var totalItem = GetCacheDataFromCache<int>(pageCacheKey);
            var categoryNickName = await SelectCategoryNickName("待回答");
            if (data != null && data.Any())
            {
                page.TotalItem = totalItem;
                return new Tuple<string, List<DiscoveryModel>>(categoryNickName, data);
            }


            var toAnswerQuestionsData = await DiscoverBLL.SelectToAnswerQuestions(page, isHomePage, sortType);
            if (toAnswerQuestionsData == null)
                return null;
            SetCacheData(cacheKey, toAnswerQuestionsData.Item2, new TimeSpan(0, 5, 0));
            SetCacheData(pageCacheKey, page.TotalItem, defaultCacheTime);
            return toAnswerQuestionsData;
        }



        private static T GetCacheDataFromCache<T>(string cacheKey)
        {
            var chcheData = EnyimMemcachedContext.MemcachedContext.Get<string>(cacheKey);
            if (chcheData == null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(chcheData);
        }
        private static bool SetCacheData<T>(string cacheKey, T t, TimeSpan ts)
        {
            if (t == null)
                return false;
            return EnyimMemcachedContext.MemcachedContext.Set(cacheKey, JsonConvert.SerializeObject(t), ts);
        }
        private static bool SetCacheData<T>(string cacheKey, T t, DateTime dt)
        {
            if (t == null)
                return false;
            return EnyimMemcachedContext.MemcachedContext.Set(cacheKey, JsonConvert.SerializeObject(t), dt);
        }



        /// <summary>
        /// 发现--根据类型查询文章
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="page"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public static async Task<List<DiscoveryModel>> SelectArticleByCategoryVersion1(int categoryId, PagerModel page, int sortType = 0, string vehilceId = null)
        {
            var articleCacheKey = string.Concat(_DiscoveryHomeCacheKey1, _key, categoryId, _key, sortType, page.CurrentPage);
            var discoveries = GetCacheDataFromCache<DiscoveryModelList>(articleCacheKey);
            if (discoveries != null)
            {
                page.TotalItem = discoveries.TotalItem;
                return discoveries.list;
            }
            DiscoveryModelList discoveryDatas = new DiscoveryModelList
            {
                list = await DiscoverBLL.SelectArticleByCategoryVersion1(categoryId, page, sortType, vehilceId),
                TotalItem = page.TotalItem
            };
            var articleShowMode = await SelectArticleShowMode();


            var result = SetCacheData(articleCacheKey, discoveryDatas, defaultCacheTime);
            return discoveryDatas.list;
        }


        /// <summary>
        /// 查询文章详情显示模式
        /// </summary>
        /// <returns></returns>
        public static async Task<string> SelectArticleShowMode()
        {
            var cacheKey = "DiscoveryShowModel";
            var articleShowMode = GetCacheDataFromCache<string>(cacheKey);
            if (articleShowMode == null)
            {
                articleShowMode = await DiscoverBLL.SelectArticleShowMode();
                SetCacheData(cacheKey, articleShowMode, defaultCacheTime);
            }
            return articleShowMode;
        }

        /// <summary>
        /// 设置文章详情页显示模式
        /// </summary>
        /// <param name="articleShowMode"></param>
        /// <returns></returns>
        public static async Task SetArticleShowMode(string articleShowMode)
        {
            var cacheKey = "DiscoveryShowModel";
            await DiscoverBLL.SetArticleShowMode(articleShowMode);
            SetCacheData(cacheKey, articleShowMode, defaultCacheTime);
        }

        public static async Task<string> SelectDictionaryValueByKey(string key)
        {
            var cacheKey = "SelectDictionaryValueByKey";
            var dictValue = GetCacheDataFromCache<string>(cacheKey);
            if (dictValue == null)
            {
                dictValue = await DiscoverBLL.GetDictionaryValueByKey(key);
                SetCacheData(cacheKey, dictValue, defaultCacheTime);
            }
            return dictValue;
        }

       
    }

    public class MyAttentionUserList
    {
        //public int Code { get; set; }
        //public List<MyAttentionUser> Data { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserHead { get; set; }
        public string VehicleImageUrl { get; set; }

        public int IsAttention { get; set; }
        public string Vehicle { get; set; }

        public string UserGrade { get; set; }
        public int UserIdentity { get; set; }

        public string UserIdentityName { get { if (UserIdentity == 2) { return "门店技师"; } else return string.Empty; } }
        public string PhoneNumber { get; set; }
    }

    public class UserInfoForDiscovery
    {
        public UserInfoData Data { get; set; }
    }

    public class UserInfoData
    {
        public string UserName { get; set; }
        public string UserHeader { get; set; }
        public string UserSign { get; set; }
    }
    public class DiscoveryModelList
    {
        public List<DiscoveryModel> list { get; set; }
        public int TotalItem { get; set; }
    }

    /// <summary>
    /// 技师个人信息
    /// </summary>
    public class TechnicianModel
    {
        public string UserName { get; set; }
        public string UserHead { get; set; }
        public string Brands { get; set; }
        public string UserIdentityName { get; set; }
    }

}
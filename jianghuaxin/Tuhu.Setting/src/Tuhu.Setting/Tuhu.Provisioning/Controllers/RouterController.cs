using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.OperationCategory;
using Tuhu.Provisioning.Business.ProductInfomationManagement;
using Tuhu.Provisioning.Business.Router;

namespace Tuhu.Provisioning.Controllers
{
    public class RouterController : Controller
    {
        

        // GET: Router
        //测试页面
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        ///     美容页面
        /// </summary>
        public ActionResult Beauty(string routerMainLinkDiscription, string linkUrl, string linkId, string noLinkId,int linkKind )
        {
            ViewBag.linkId = linkId;
            ViewBag.noLinkId = noLinkId;
            ViewBag.linkKind = linkKind;
            var valid = false; //标志用于验证 是否应该解析
            string latter = null;
            var routerManager = new RouterManager();
            var content = routerManager.GetMainLink(routerMainLinkDiscription, linkKind).Content;
            if (linkUrl != "" && linkKind == 1)
                //解析APP链接 参数拼接格式：/webView?url=xxx
                try
                {
                    var array = linkUrl.Split('?');
                    var cont = array[0];
                    var url = array[1].Split('=');
                    if (linkUrl.Length > linkUrl.IndexOf("=", StringComparison.Ordinal) + 1)
                        latter = linkUrl.Substring(linkUrl.IndexOf("=", StringComparison.Ordinal) + 1);
                    if (url[0] == "url" && cont == content)
                        valid = true;
                    
                }
                catch (Exception)
                {
                    // ignored
                }
            if (linkUrl != "" && linkKind == 2)
            //解析小程序链接 参数拼接格式：/pages /...
            {
                var cont ="/"+ linkUrl.Split('/')[1];
                if (linkKind == 2 && cont == content)
                {
                    latter = linkUrl.Substring(6);
                    valid = true;
                }
            }
            if (valid)
                return View(routerManager.GetParameterState(routerMainLinkDiscription,
                    latter,linkKind)); //进入解析结果页面 
            return View(routerManager.GetParameterList(routerMainLinkDiscription,linkKind)); //进入空白页面
        }

        /// <summary>
        ///     保养页面
        /// </summary>
        public ActionResult Maintenance(string routerMainLinkDiscription, string linkUrl, string linkId,
            string noLinkId, int linkKind)
        {
            ViewBag.linkId = linkId;
            ViewBag.noLinkId = noLinkId;
            ViewBag.linkKind = linkKind;

            var valid = false; //标志是否解析
            var routerManager = new RouterManager();
            var content = routerManager.GetMainLink(routerMainLinkDiscription,linkKind).Content;

            var list = new List<string>();
            var type="";
            string[] aid;
            var aId = "";
            if (linkKind == 1)
            {
                type = "type";
                aId = "aid";
            }
            if (linkKind == 2)
            {
                type = "baoyangtypes";
                aId = "actid";
            }
            if (linkUrl != "")

                //解析格式APP/maintenance? type=xxx;xxx;xxx&aid=xxx&produceIds=xx,xx&productActivityId=xx&IsTuhuRecommend=x 
                //小程序 /pages/keep_list2/keep_list2?baoyangtypes=xxx&actid=xxxx
                try
                {
                    var array = linkUrl.Split('?');
                    var arrayLatter = array[1].Split('&');
                    ViewBag.paraArray = array[1]; //paraArray 是参数以&分隔的字符串

                    var typeString = arrayLatter[0].Split('=');
                    ViewBag.typeArray = typeString[1]; //typeArray 是种类以;分隔的字符串

                    if (typeString[0] == type && array[0] == content)
                        valid = true;

                    if (arrayLatter.Length >= 2)
                    {
                        aid = arrayLatter[1].Split('=');
                        if (aid[0] == aId)
                        {
                            ViewBag.aid = aid[1];
                            for (var i = 2; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }
                        else
                        {
                            for (var i = 1; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

            if (valid)
                return View(routerManager.GetParameterStateList(routerMainLinkDiscription, list, linkKind)); //进入解析页面
            return View(routerManager.GetParameterList(routerMainLinkDiscription, linkKind)); //进入空白页面
        }


        /// <summary>
        ///     根据发现文章ID查找跳转链接
        /// </summary>
        public ActionResult GetFindUrl(int id)
        {
            try
            {
                var manager = new ArticleManager();
                var articleModel = manager.GetByPKID(id);
                if (articleModel != null)
                {
                    var s = articleModel.ContentUrl;
                    return Json(s);
                }
                return Json(0);
            }
            catch
            {
                return Json(0);
            }
        }

        /// <summary>
        ///     发现文章页面
        /// </summary>
        public ActionResult Find(string routerMainLinkDiscription, string linkUrl, string linkId, string noLinkId, int linkKind)
        {
            ViewBag.linkId = linkId;
            ViewBag.noLinkId = noLinkId;
            ViewBag.linkKind = linkKind;

            var valid = false; //标志是否解析
            var routerManager = new RouterManager();
            var content = routerManager.GetMainLink(routerMainLinkDiscription, linkKind).Content;
            var articleId = "";

            if (linkUrl != "")
                try
                {
                    var array = linkUrl.Split('?');
                    var url = array[1].Split('=');
                    var manager = new ArticleManager();

                    if (url[0] == "url" && array[0] == content && linkKind == 1)
                    {
                        valid = true;
                        var articleModel =
                            manager.GetByUrl(linkUrl.Substring(linkUrl.IndexOf("=", StringComparison.Ordinal) + 1));
                        if (articleModel != null)
                            articleId = articleModel.PKID.ToString();
                    }
                    if (array[1].Split('&')[0].Split('=')[0] == "id" && array[0] == content && linkKind == 2)
                    {
                        valid = true;
                        articleId = array[1].Split('&')[0].Split('=')[1];
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            if (valid)
                return View(routerManager.GetParameterState(routerMainLinkDiscription, articleId, linkKind)); //进入解析页面
            return View(routerManager.GetParameterList(routerMainLinkDiscription, linkKind)); //进入空白页面
        }

        /// <summary>
        ///     商品详情-根据id获取名称，前端验证
        /// </summary>
        public ActionResult GetProductNameById(string id)
        {
            var manager = new ProductInfomationManager();
            var name = manager.GetNameById(id);
            if (name != null)
                return Json(name);
            return Json(0);
        }

        /// <summary>
        ///     商品详情-根据id获取链接，后台验证
        /// </summary>
        public string GetProductNameByIdString(string id)
        {
            var manager = new ProductInfomationManager();
            var name = manager.GetNameById(id);
            return name;
        }

        /// <summary>
        ///     商品详情页面
        /// </summary>
        public ActionResult ProductDetail(string routerMainLinkDiscription, string linkUrl, string linkId,
            string noLinkId, int linkKind)
        {
            ViewBag.linkId = linkId;
            ViewBag.noLinkId = noLinkId;
            ViewBag.linkKind = linkKind;
            var routerManager = new RouterManager();
            if (linkUrl == "" && linkUrl.IndexOf("pid=", StringComparison.Ordinal) > 0)
                return View(routerManager.GetParameterList(routerMainLinkDiscription, linkKind));
            var array = linkUrl.Split('?');
            var list = new List<string>();
            var validState = true;

            var pId = "";
            var vId = "";
            var aId = "";
            if (linkKind == 1) //APP前缀判断 
            {
                switch (array[0])
                {
                    case "/tire/item":
                        ViewBag.state = 1;
                        break;
                    case "/wheelRim/item":
                        ViewBag.state = 2;
                        break;
                    case "/accessory/item":
                        ViewBag.state = 3;
                        break;
                    default:
                        validState = false; //若主链接不符合则不进入下面的判断
                        break;
                }
                pId = "pid";
                vId = "vid";
                aId = "aid";
            }
            else if (linkKind == 2) //小程序前缀判断
            { 
                switch (array[0])
                {
                    case "/pages/tireDetail/tireDetail":
                        ViewBag.state = 1;
                        break;
                    case "/pages/hubs/detail":
                        ViewBag.state = 2;
                        break;
                    case "/pages/chepin/detail":
                        ViewBag.state = 3;
                        break;
                    default:
                        validState = false; //若主链接不符合则不进入下面的判断
                        break;
                }
                pId = "productid";
                vId = "variantid";
                aId = "vid";
            }
            if (validState)
                try
                {
                    ViewBag.paraArray = array[1]; //paraArray 是参数以&分隔的字符串
                    var arrayLatter = array[1].Split('&');
                    var pid = arrayLatter[0].Split('=');

                    if (arrayLatter.Length == 1 && pid[0] == pId) //？pid=xxx的格式
                    {
                        ViewBag.pid = pid[1];
                        ViewBag.pName = GetProductNameByIdString(pid[1]);

                        for (var i = 2; i < arrayLatter.Length; i++)
                            list.Add(arrayLatter[i]);
                    }

                    else if (arrayLatter.Length >= 2 && pid[0] == pId)
                    {
                        var vid = arrayLatter[1].Split('=');
                        if (vid[0] == vId) //？pid=xxx|x&vid=xxx的格式
                        {
                            ViewBag.pid = pid[1] + "|" + vid[1];
                            ViewBag.pName = GetProductNameByIdString(pid[1] + "|" + vid[1]);

                            if (arrayLatter.Length >= 3)
                            {
                                var aid = arrayLatter[2].Split('=');
                                if (aid[0] == aId)
                                    ViewBag.aid = aid[1]; //？pid=xxx|x&vid=xxx&aid=xx的格式
                            }

                            for (var i = 3; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }

                        else if (vid[0] == aId) //？pid=xxx&aid=xx的格式
                        {
                            ViewBag.aid = vid[1];
                            ViewBag.pid = pid[1];
                            ViewBag.pName = GetProductNameByIdString(pid[1]);

                            for (var i = 2; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }
                        else//？pid=xxx&...&...的格式
                        {
                            ViewBag.pid = pid[1];
                            ViewBag.pName = GetProductNameByIdString(pid[1]);

                            for (var i = 2; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            return View(routerManager.GetParameterStateList(routerMainLinkDiscription, list, linkKind));
        }

        /// <summary>
        ///     H5页面
        /// </summary>
        public ActionResult HFive(string routerMainLinkDiscription, string linkUrl, string linkId, string noLinkId, int linkKind)
        {
            ViewBag.linkId = linkId;
            ViewBag.noLinkId = noLinkId;
            ViewBag.linkKind = linkKind;
            var valid = false; //标志是否解析
            var routerManager = new RouterManager();
            var content = routerManager.GetMainLink(routerMainLinkDiscription, linkKind).Content;
            var list = new List<string>();

            if (linkUrl != "")
            {
                var array = linkUrl.Split('?');
                if (content == array[0])
                    valid = true;

                if(linkKind==1&& valid)
                    try
                {
                    ViewBag.paraArray = array[1]; //paraArray 是参数以&分隔的字符串

                    var arrayLatter = array[1].Split('&');
                    var url = arrayLatter[0].Split('=');
                    ViewBag.url = url[1];

                    if (arrayLatter.Length >= 2)
                    {
                        var title = arrayLatter[1].Split('=');
                        if (title[0] == "title")
                        {
                            ViewBag.Pagetitle = title[1];
                            for (var i = 2; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }
                        else
                        {
                            for (var i = 1; i < arrayLatter.Length; i++)
                                list.Add(arrayLatter[i]);
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                if (linkKind == 2&& valid)
                {
                    
                    var arrayLatter = array[1].Split('&');
                    var hash = arrayLatter[0].Split('=');
                    ViewBag.hash = hash[1];
                }
            }

            if (valid)
                return View(routerManager.GetParameterStateList(routerMainLinkDiscription, list, linkKind)); //进入解析页面
            return View(routerManager.GetParameterList(routerMainLinkDiscription, linkKind)); //进入空白页面
        }

        /// <summary>
        ///     其他页面
        /// </summary>
        public ActionResult Other(string routerMainLinkDiscription, string linkUrl, string linkId, string noLinkId, int linkKind)
        {
            ViewBag.linkId = linkId;
            ViewBag.noLinkId = noLinkId;
            ViewBag.linkKind = linkKind;
            CreateTree();

            var routerManager = new RouterManager();
            if (linkUrl == "")
                return View(routerManager.GetParameterList(routerMainLinkDiscription, linkKind));


            var value = routerManager.GetParameterForOther(routerMainLinkDiscription, linkUrl, linkKind);
            var list1 = value as List<string>;
            if (list1 != null)
            {
                try
                {
                    var list = list1;
                    ViewBag.state = list[0];
                    ViewBag.value = list[1];
                }
                catch (Exception)
                {
                    // ignored
                }

                return View(routerManager.GetParameterList(routerMainLinkDiscription, linkKind));
            }
            return View(value);
        }

        /// <summary>
        ///     获取所有主链接，设置下拉框
        /// </summary>
        public ActionResult GetMainLinkList(int linkKind)
        {
            var routerManager = new RouterManager();
            return Json(routerManager.GetMainLinkList(linkKind));
        }

        /// <summary>
        ///     初始化商品类目树
        /// </summary>
        public void CreateTree(int id = 0)
        {
            ViewBag.id = id;
            var manager = new OperationCategoryManager();
            var cateItems = manager.SelectProductCategories();
            var productList = manager.SelectOperationCategoryProducts(id);

            if (productList != null && productList.Count() > 0)
                foreach (var pitem in productList)
                foreach (var citem in cateItems)
                    if (pitem.CorrelId.Equals(citem.id))
                    {
                        citem.open = true;
                        citem.isChecked = true;
                    }

            ViewBag.CategoryTagManager = JsonConvert.SerializeObject(cateItems).Replace("isChecked", "checked");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning
{
    #region 分页


    public static class PageExtension
    {
        /// <summary>生成分页Html</summary>
        /// <param name="helper">对象</param>
        /// <param name="pager">分页数据</param>
        /// <param name="func">Url生成函数</param>
        /// <returns>分页Html</returns>
        public static MvcHtmlString Pager(this HtmlHelper helper, PagerModel pager, Func<int, string> func)
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"pager\">");
            if (pager.CurrentPage < 2)
                sb.Append("<a class=\"disabled first-child\">&lt;&lt; 上一页</a>");
            else
                sb.Append("<a class=\"first-child\" href=\"" + func(pager.CurrentPage - 1) + "\">&lt;&lt; 上一页</a>");
            if (pager.TotalPage < 12)
            {
                for (var index = 1; index <= pager.TotalPage; index++)
                {
                    if (index == pager.CurrentPage)
                        sb.Append("<a class=\"current\">" + index + "</a>");
                    else
                        sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(index), index);
                }
            }
            else
            {
                if (pager.CurrentPage < 8)
                {
                    for (var index = 1; index <= 8; index++)
                    {
                        if (index == pager.CurrentPage)
                            sb.Append("<a class=\"current\">" + index + "</a>");
                        else
                            sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(index), index);
                    }
                    sb.Append("<span>...</span>");
                    sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(pager.TotalPage - 1), pager.TotalPage - 1);
                    sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(pager.TotalPage), pager.TotalPage);
                }
                else if (pager.CurrentPage > pager.TotalPage - 7)
                {
                    sb.Append("<a href=\"" + func(1) + "\">1</a>");
                    sb.Append("<a href=\"" + func(2) + "\">2</a>");
                    sb.Append("<span>...</span>");
                    for (var index = pager.TotalPage - 7; index <= pager.TotalPage; index++)
                    {
                        if (index == pager.CurrentPage)
                            sb.Append("<a class=\"current\">" + index + "</a>");
                        else
                            sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(index), index);
                    }
                }
                else
                {
                    sb.Append("<a href=\"" + func(1) + "\">1</a>");
                    sb.Append("<a href=\"" + func(2) + "\">2</a>");
                    sb.Append("<span>...</span>");
                    for (var index = pager.CurrentPage - 2; index <= pager.CurrentPage + 2; index++)
                    {
                        if (index == pager.CurrentPage)
                            sb.Append("<a class=\"current\">" + index + "</a>");
                        else
                            sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(index), index);
                    }
                    sb.Append("<span>...</span>");
                    sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(pager.TotalPage - 1), pager.TotalPage - 1);
                    sb.AppendFormat("<a href=\"{0}\">{1}</a>", func(pager.TotalPage), pager.TotalPage);
                }
            }
            if (pager.CurrentPage >= pager.TotalPage)
                sb.Append("<a class=\"disabled last-child\">下一页 &gt;&gt;</a>");
            else
                sb.Append("<a class=\"last-child\" href=\"" + func(pager.CurrentPage + 1) + "\">下一页 &gt;&gt;</a>");
            sb.Append("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }
        /// <summary>生成分页Html</summary>
        /// <param name="helper">对象</param>
        /// <param name="pager">分页数据</param>
        /// <param name="hideIfLessTowPage">小于两页是否不生成</param>
        /// <param name="func">Url生成函数</param>
        /// <returns>分页Html</returns>
        public static MvcHtmlString Pager(this HtmlHelper helper, PagerModel pager, bool hideIfLessTowPage, Func<int, string> func)
        {
            if (hideIfLessTowPage == true && pager.TotalPage < 2)
                return null;
            return Pager(helper, pager, func);
        }
    }

    #endregion

    #region DomainConfig

    public static class DomainConfig
    {
        private static Dictionary<string, string> _domains;
        private static string _topDomain;
        private static Dictionary<string, XmlElement> _nodeCollection;

        public static Action OnConfigChanged { get; set; }
        static DomainConfig()
        {
            ConfigDomain();
            OnConfigChanged += ConfigDomain;
        }
        /// <summary>
        /// 获得配置节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>配置节点</returns>
        public static XmlElement GetConfigNode(string name)
        {
            XmlElement ele;
            _nodeCollection.TryGetValue(name, out ele);
            return ele;
        }
        private static void ConfigDomain()
        {
            var doc = new XmlDocument();
            doc.Load(HostingEnvironment.MapPath("~/Site.config") ?? "Site.config");

            var nodeCollection = new Dictionary<string, XmlElement>();
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                    nodeCollection[node.Name] = node as XmlElement;
            }
            _nodeCollection = nodeCollection;

            var domains = new Dictionary<string, string>();

            var configNode = GetConfigNode("domainConfig");
            foreach (XmlNode node in configNode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element && node.Attributes["name"] != null)
                {

                    domains[node.Attributes["name"].Value] = node.Attributes["site"].Value;
                }

            }

            _domains = domains;
            _topDomain = configNode.Attributes["topDomain"].Value;
        }

        /// <summary>顶级域名</summary>
        public static string TopDomain => _topDomain;

     

        /// <summary>图片站点(CDN，只支持图片)</summary>
        public static string ImageSite => GetSiteByName("ImageSite");


        /// <summary>通过站点名称获得域名</summary>
        /// <param name="name">站点名称</param>
        /// <returns>域名</returns>
        public static string GetSiteByName(string name)
        {
            string site;
            _domains.TryGetValue(name, out site);
            return site;
        }

        /// <summary>
        /// 解释数据库中的图片地址为真实的地址（目前不需要实现）
        /// </summary>
        /// <param name="imageUrl">数据库中的图片地址</param>
        /// <returns>真实的地址</returns>
        public static string ResolveImageUrl(string imageUrl)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
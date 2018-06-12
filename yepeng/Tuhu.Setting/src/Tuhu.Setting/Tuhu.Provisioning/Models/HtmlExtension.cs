using System;
using System.Text;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.Models
{
    public static class HtmlExtension
    {
        #region Pager
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
        #endregion
    }
}
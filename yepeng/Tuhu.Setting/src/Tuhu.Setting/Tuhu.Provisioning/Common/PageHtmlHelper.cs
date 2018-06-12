using System.Text;

namespace Tuhu.Provisioning.Common
{
    public class PageHtmlHelper
    {
        public static string GetPager(string hrefPrefix, int pageNo, int pageNum)
        {
            StringBuilder sb = new StringBuilder();
            if (pageNo.Equals(1))
            {
                sb.Append("<span>上一页</span>");
                sb.Append(GetPageInfo(pageNo, pageNum, hrefPrefix));
                sb.AppendFormat("<a class='pageon' href='{0}'>下一页</a>", string.Format(hrefPrefix, pageNo + 1));
            }
            else if (pageNo.Equals(pageNum))
            {
                sb.AppendFormat("<a class='pageon' href='{0}'>上一页</a>", string.Format(hrefPrefix, pageNo - 1));
                sb.Append(GetPageInfo(pageNo, pageNum, hrefPrefix));
                sb.Append("<span>下一页</span>");
            }
            else
            {
                sb.AppendFormat("<a class='pageon' href='{0}'>上一页</a>", string.Format(hrefPrefix, pageNo - 1));
                sb.Append(GetPageInfo(pageNo, pageNum, hrefPrefix));
                sb.AppendFormat("<a class='pageon' href='{0}'>下一页</a>", string.Format(hrefPrefix, pageNo + 1));
            }
            return sb.ToString();
        }

        private static string GetPageInfo(int pageNo, int pageNum, string hrefPrefix)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pageNum; i++)
            {
                if ((i + 1).Equals(pageNo))
                {
                    sb.AppendFormat("<strong>{0}</strong>", pageNo);
                }
                else
                {
                    sb.AppendFormat("<a href='{0}'>{1}</a>", string.Format(hrefPrefix, i + 1), i + 1);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 生成列表页分页代码
        /// </summary>
        /// <param name="pagerPrefixFormat"></param>
        /// <param name="paramReplace"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public static string GetListPager(string pagerPrefixFormat, int pageNo, int pageNum)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"page-nav\">");
            if (pageNo.Equals(1))
            {
                sb.Append(GetListPageInfo(pageNo, pageNum, pagerPrefixFormat));
                sb.AppendFormat("<li><a href=\"{0}\" title=\"下一页\">下一页</a></li>", string.Format(pagerPrefixFormat, (pageNo + 1).ToString()));
                sb.AppendFormat("<li><a href=\"{0}\" title=\"尾页\">尾页</a></li>", string.Format(pagerPrefixFormat, pageNum.ToString()));
            }
            else if (pageNo.Equals(pageNum))
            {
                sb.AppendFormat("<li><a href=\"{0}\" title=\"上一页\">上一页</a></li>", string.Format(pagerPrefixFormat, (pageNo - 1).ToString()));
                sb.AppendFormat("<li><a href=\"{0}\" title=\"首页\">首页</a></li>", string.Format(pagerPrefixFormat, "1"));
                sb.Append(GetListPageInfo(pageNo, pageNum, pagerPrefixFormat));
            }
            else
            {
                if ((pageNo - 1).Equals(1))
                {
                    sb.AppendFormat("<li><a href=\"{0}\" title=\"上一页\">上一页</a></li>", string.Format(pagerPrefixFormat, string.Empty));
                    sb.AppendFormat("<li><a href=\"{0}\" title=\"首页\">首页</a></li>", string.Format(pagerPrefixFormat, "1"));
                }
                else
                {
                    sb.AppendFormat("<li><a href=\"{0}\" title=\"上一页\">上一页</a></li>", string.Format(pagerPrefixFormat, (pageNo - 1).ToString()));
                    sb.AppendFormat("<li><a href=\"{0}\" title=\"首页\">首页</a></li>", string.Format(pagerPrefixFormat, "1"));
                }
                sb.Append(GetListPageInfo(pageNo, pageNum, pagerPrefixFormat));
                sb.AppendFormat("<li><a href=\"{0}\" title=\"下一页\">下一页</a></li>", string.Format(pagerPrefixFormat, (pageNo + 1).ToString()));
                sb.AppendFormat("<li><a href=\"{0}\" title=\"尾页\">尾页</a></li>", string.Format(pagerPrefixFormat, pageNum.ToString()));
            }
            sb.AppendFormat("<li><span>共{0}页</span></li>", pageNum.ToString());
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string GetListPageInfo(int pageNo, int pageNum, string pagerPrefixFormat)
        {
            StringBuilder sb = new StringBuilder();
            if (pageNum <= 10)
            {
                for (int i = 0; i < pageNum; i++)
                {
                    if ((i + 1).Equals(pageNo))
                    {
                        sb.AppendFormat("<li class=\"curr-page\"><a class=\"selected\">{0}</a></li>", i + 1);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", string.Format(pagerPrefixFormat, i == 0 ? "1" : ((i + 1).ToString())), (i + 1).ToString());
                    }
                }
            }
            else
            {
                int startIndex = 0;
                int endIndex = 0;
                if (pageNo <= 6)
                {
                    startIndex = 1;
                    endIndex = 10;
                }
                else if ((pageNum - pageNo) < 5)
                {
                    startIndex = pageNum - 9;
                    endIndex = pageNum;
                }
                else
                {
                    startIndex = pageNo - 4;
                    endIndex = pageNo + 4;
                }

                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (i.Equals(pageNo))
                    {
                        sb.AppendFormat("<li class=\"curr-page\"><a class=\"selected\">{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", string.Format(pagerPrefixFormat, i.ToString()), i.ToString());
                    }
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 详情页面异步分页
        /// </summary>
        /// <param name="pagerPrefixFormat"></param>
        /// <param name="paramReplace"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public static string GetPagerAsyn(int pageNo, int pageNum, string extraStrFormat)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"page-nav\">");
            if (pageNo.Equals(1))
            {
                sb.Append(GetListPageInfoAsyn(pageNo, pageNum, extraStrFormat));
                sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"下一页\">下一页</a></li>",
                    string.Format(extraStrFormat, (pageNo + 1).ToString()));
                sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"尾页\">尾页</a></li>",
                    string.Format(extraStrFormat, pageNum.ToString()));
            }
            else if (pageNo.Equals(pageNum))
            {
                sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"上一页\">上一页</a></li>", string.Format(extraStrFormat, (pageNo - 1).ToString()));
                sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"首页\">首页</a></li>", string.Format(extraStrFormat, "1"));
                sb.Append(GetListPageInfoAsyn(pageNo, pageNum, extraStrFormat));
            }
            else
            {
                if ((pageNo - 1).Equals(1))
                {
                    sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"上一页\">上一页</a></li>", string.Format(extraStrFormat, string.Empty));
                    sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"首页\">首页</a></li>", string.Format(extraStrFormat, "1"));
                }
                else
                {
                    sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"上一页\">上一页</a></li>", string.Format(extraStrFormat, (pageNo - 1).ToString()));
                    sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"首页\">首页</a></li>", string.Format(extraStrFormat, "1"));
                }
                sb.Append(GetListPageInfoAsyn(pageNo, pageNum, extraStrFormat));
                sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"下一页\">下一页</a></li>", string.Format(extraStrFormat, (pageNo + 1).ToString()));
                sb.AppendFormat("<li><a href=\"javascript:void(0)\" {0} title=\"尾页\">尾页</a></li>", string.Format(extraStrFormat, pageNum.ToString()));
            }
            sb.AppendFormat("<li><span>共{0}页</span></li>", pageNum.ToString());
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string GetListPageInfoAsyn(int pageNo, int pageNum, string extraStrFormat)
        {
            StringBuilder sb = new StringBuilder();
            if (pageNum <= 10)
            {
                for (int i = 0; i < pageNum; i++)
                {
                    if ((i + 1).Equals(pageNo))
                    {
                        sb.AppendFormat("<li class=\"curr-page\"><a class=\"\">{0}</a></li>", i + 1);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a href=\"javascript:void(0)\" {1}>{0}</a></li>", (i + 1).ToString(),
                            string.Format(extraStrFormat, i == 0 ? "1" : ((i + 1).ToString())));
                    }
                }
            }
            else
            {
                int startIndex = 0;
                int endIndex = 0;
                if (pageNo <= 6)
                {
                    startIndex = 1;
                    endIndex = 10;
                }
                else if ((pageNum - pageNo) < 5)
                {
                    startIndex = pageNum - 9;
                    endIndex = pageNum;
                }
                else
                {
                    startIndex = pageNo - 4;
                    endIndex = pageNo + 4;
                }

                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (i.Equals(pageNo))
                    {
                        sb.AppendFormat("<li class=\"curr-page\"><a class=\"\">{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a href=\"javascript:void(0)\" {1}>{0}</a></li>", i.ToString(),
                            string.Format(extraStrFormat, i.ToString()));
                    }
                }
            }
            return sb.ToString();
        }
    }
}
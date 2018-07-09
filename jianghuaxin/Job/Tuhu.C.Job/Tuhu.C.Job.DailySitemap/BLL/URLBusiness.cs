using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.DailySitemap.DAL;

namespace Tuhu.C.Job.DailySitemap.BLL
{
    class URLBusiness
    {

        public static bool InsertDailyURLToDB(bool isFull)
        {
            bool insertsuccess = false;           
            var articleresult = InsertArticleURL(isFull);          
            if (articleresult)
            {
                insertsuccess = true;
            }
            return insertsuccess;
        }

        public static bool InsertArticleURL(bool isFull = false)
        {
            List<string> articleUrlList = new List<string>();
            IEnumerable<int> articleIds = BaseDataDAL.GetArticleID(isFull);
            if (articleIds != null)
            {
                foreach (var article in articleIds.Where(q => q > 0))
                {
                    articleUrlList.Add(string.Format("http://www.tuhu.cn/Community/detail/{0}.aspx", article));
                }
            }
            if (isFull)
            {
                InsertURLToDB(new List<string> { "http://www.tuhu.cn/Community/","http://www.tuhu.cn/Community/Discovery.aspx", "http://www.tuhu.cn/Community/Discovery.aspx?tagId=1", "http://www.tuhu.cn/Community/Discovery.aspx?tagId=1344",
                "http://www.tuhu.cn/Community/Discovery.aspx?tagId=6","http://www.tuhu.cn/Community/Discovery.aspx?tagId=21","http://www.tuhu.cn/Community/Discovery.aspx?tagId=61","http://www.tuhu.cn/Community/Discovery.aspx?tagId=4"}, "DiscoveryList");
            }
            return InsertURLToDB(articleUrlList, "DiscoveryDetail");
        }


        public static bool InsertURLToDB(List<string> urlList, string type)
        {
            bool isInsert = true;
            try
            {
                if (urlList != null && urlList.Any())
                {
                    foreach (var url in urlList.Where(q => !string.IsNullOrWhiteSpace(q)))
                    {
                        BaseDataDAL.InsertURL(url, type);
                    }
                }
            }
            catch (Exception ex)
            {
                isInsert = false;
                DailySitemapJob.Logger.Error(ex);
            }
            return isInsert;
        }

    }
}

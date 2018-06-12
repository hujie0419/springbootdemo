using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.DailySitemap.DAL
{
    class BaseDataSqlText
    {
        public static string SqlTextAllArticleIDs = "SELECT PKID FROM tbl_Article(NOLOCK)";
        public static string sqlTextPartialArticleIDs = "SELECT PKID FROM Marketing..tbl_Article WHERE DATEDIFF(dd,PublishDateTime,GETDATE())=6 and IsShow=1";
        public static string sqlTextInsertUrl = @"INSERT INTO  [dbo].[tbl_SiteMap]([URL],[Type],[DataCreate_Time],[DataUpdate_Time]) VALUES
(
@URL, @Type, @DataCreate_Time,@DataUpdate_Time
)";
    }
}

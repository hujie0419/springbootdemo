using System.Web;
using System.Web.Mvc;
using Tuhu.MMS.Web.Filters;

namespace Tuhu.MMS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ThreadIdentityActionFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}

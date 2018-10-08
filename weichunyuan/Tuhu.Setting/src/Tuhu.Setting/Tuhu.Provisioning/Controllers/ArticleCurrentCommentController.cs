using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;

namespace Tuhu.Provisioning.Controllers
{
    public class ArticleCurrentCommentController : Controller
    {
        private readonly IArticleManager manager = new ArticleManager();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(int? PageIndex)
        {
            int _PageSize = 30;
            var _List = manager.SelectBy(_PageSize, PageIndex);
            if (_List != null && _List.Count > 0)
            {
                return Json(new
                {
                    ArticleList = _List.Select(p => new
                    {
                        //文章阅读时间
                        CurrentOperateTime = p.CurrentOperateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        //文章阅读次数
                        CurrentClickCount = p.CurrentClickCount
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ArticleList = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

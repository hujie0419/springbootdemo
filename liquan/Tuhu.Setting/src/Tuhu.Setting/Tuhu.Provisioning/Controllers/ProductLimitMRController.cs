using System.Web.Mvc;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductLimitMRController : ProductLimitController
    {
        /// <summary>
        /// 获取类目树
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpGet]
        public JsonResult GetCatrgoryTree(string category)
        {
            var list = new ProductLimitController().GetCatrgoryList(category);
            return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
        }
    }
}
using System.Web.Mvc;

using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.ProductLimit;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductLimitBaoYangController : ProductLimitController
    {

        readonly ProductLimitManager _manager = new ProductLimitManager();

        /// <summary>
        /// 根据分类名称和级别获取限购实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpGet]
        public JsonResult FetchCategoryLimitCount(string name, string code, int level)
        {
            var model = new ProductLimitCountEntity
            {
                CategoryName = name,
                CategoryCode = code,
                CategoryLevel = level
            };
            var result = _manager.FetchCategoryLimitCount(model);
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.SearchWordConvertMapConfig;

namespace Tuhu.Provisioning.Controllers
{
    public class SearchWordConvertConfigController : Controller
    {
        private readonly SearchWordFactory _factory = new SearchWordFactory();
        private readonly ISearchWordConvertMgr _iswcmgr = new SearchWordConvertMgr();

        public static readonly Dictionary<SearchWordConfigType, string> SearchWordConfigType = new Dictionary<SearchWordConfigType, string>
        {
            { DataAccess.Entity.SearchWordConfigType.Config, "同义词配置表" },
            { DataAccess.Entity.SearchWordConfigType.NewWord, "途虎词典配置表" },
            { DataAccess.Entity.SearchWordConfigType.VehicleType, "二级车型配置表" }
        };
        public ActionResult Index(string msg)
        {
            ViewBag.error = msg;
            ViewBag.configType = SearchWordConfigType;
            return View();
        }

        /// <summary>
        /// 上传Excel文件处理
        /// </summary>
        /// <param name="filebase"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileImport(HttpPostedFileBase filebase)
        {
            var configType = (SearchWordConfigType)Enum.Parse(typeof(SearchWordConfigType), Request.Form["configType"]);
            var fileName = "最新配置-" + SearchWordConfigType[configType] + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(999) + ".xls";

            var handler = _factory.Create(configType);
            var existConfigList = _iswcmgr.GetAllSearchWord(configType);

            byte[] bytes;
            var result = handler.Import(Request.Files["files"], existConfigList, out bytes);

            switch (result)
            {
                case -1:
                    return File(bytes, "application/vnd.ms-excel", fileName);
                case -100:
                    return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "文件不能为空" });
                case -101:
                    return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "文件扩展名不能为空" });
                case -102:
                    return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "文件类型不对，只能导入xls和xlsx格式的文件" });
                case -103:
                    return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "上传文件超过10M，不能上传" });
                default:
                    return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "导入成功" });
            }

        }

        /// <summary>
        /// 下载当前最新配置
        /// </summary>
        /// <returns></returns>
        public ActionResult DownLoadFile()
        {
            var configType = (SearchWordConfigType)Enum.Parse(typeof(SearchWordConfigType), Request.QueryString["configType"]);

            var handler = _factory.Create(configType);

            var sourceList = _iswcmgr.GetAllSearchWord(configType);
            byte[] file = handler.Export(sourceList);

            var fileName = "最新配置-" + SearchWordConfigType[configType] + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(999) + ".xls";

            return File(file, "application/vnd.ms-excel", fileName);
        }
    }
}

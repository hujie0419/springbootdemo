using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.WebSite.Web.Api
{

    public class APPController : Controller
    {
        private static readonly Lazy<APPManager> lazyAPPManager = new Lazy<APPManager>();

        private APPManager APPManager
        {
            get { return lazyAPPManager.Value; }
        }


        /// <summary>
        /// 管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 历史版本
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryVersion(int? PageIndex = 1)
        {
            PagerModel pagerModel = new PagerModel();
            pagerModel.CurrentPage = PageIndex.Value;
            pagerModel.PageSize = 10;
            return View(new ListModel<tbl_app_Versions>() { Pager = pagerModel, Source = APPManager.GetAppVersions(pagerModel) });
        }
        /// <summary>
        /// 更新历史版本的一些记录
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult Update(int? PKID)
        {
            if (PKID != null)
            {
                int p = Convert.ToInt32(PKID);
                return View(APPManager.load2(p));
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Update2(tbl_app_Versions tb)
        {
            //tbl_app_Versions ts = new tbl_app_Versions();
            if (tb.UpdateConnent.ToString().Length == 0)
            {
                return Content(@"<script>alert('修改内容不能为空');</script>");
            }
            else
            {
            }

            if (tb.Version_Number == "" || tb.Version_Number == null)
            { return Content(@"<script>alert('版本编号不能为空');location.href='Update'</script>"); }
            else if (tb.VersionCode == null)
            {
                return Content(@"<script>alert('版本标识不能为空');location.href='Update'</script>");
            }
            else
            {
                APPManager.upDate(tb);
                return RedirectToAction("HistoryVersion");
            }

        }
        /// <summary>
        /// 上传app展示页
        /// </summary>
        /// <returns></returns>
        public ActionResult upload()
        {

            return View();
        }
        /// <summary>
        /// 上传App
        /// </summary>
        /// <param name="Version_Number">版本编号3.3.0</param>
        /// <param name="VersionCode">版本标识16</param>
        /// <param name="UpdateConnent">更新内容</param>
        /// <returns></returns>
        public ActionResult upload1(string Version_Number, string VersionCode, string UpdateConnent, string mustUpdate)
        {
            if (Request.Files.Count > 0)
            {
                string[] a = Request.Files[0].FileName.Split('.');
                if (a.Length > 1 && a[a.Length - 1] == "apk")
                {
                    if (Request.Files[0].ContentLength > 0 & Request.Files[0].ContentLength < 200 * 1024 * 1024)
                    {
                        var apk = Request.Files[0];
                        Regex rx = new Regex("[\u4e00-\u9fa5]+");

                        if (rx.IsMatch(Version_Number, 0))
                        {
                            return Content(@"<script>alert('版本编号只能为数字');location.href='upload'</script>");

                        }
                        else if (rx.IsMatch(VersionCode, 0))
                        {
                            return Content(@"<script>alert('版本标识只能为数字');location.href='upload'</script>");
                        }
                        else
                        {
                            //最新版
                            //string path = "/Download/TuHu_android.apk";
                            string filename = Path.GetFileName(Request.Files[0].FileName);
                            //apk.SaveAs(Server.MapPath(path));
                            string size = (apk.ContentLength / 1.0 / 1024 / 1024).ToString("0.0") + "M";
                            //历史版本
                            string historyPath = "/History/TuHu_android_" + Version_Number + ".apk";
                            //apk.SaveAs(Server.MapPath(historyPath));
                            if (apk.SaveAsRemoteFile(historyPath) > 0)
                            {
                                APPManager.upload(Version_Number, historyPath, UpdateConnent, size, VersionCode, mustUpdate);
                                return Json("提交成功");
                            }
                            else
                            {
                                return Json("文件保存失败");
                            }
                        }
                    }
                    else
                    {
                        return Content(@"<script>alert('上传文件不能大于200M');</script>");
                    }
                }
                else
                {
                    return Content(@"<script>alert('上传必须为apk文件');</script>");
                }
            }
            else
            {
                return Content(@"<script>alert('上传不能为空');</script>");
            }
        }
    }
}

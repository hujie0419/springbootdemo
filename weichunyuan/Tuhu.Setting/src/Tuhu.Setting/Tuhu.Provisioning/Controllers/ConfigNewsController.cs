using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ThBiz.Business.OprLogManagement;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ConfigNewsController : Controller
    {

        private readonly Lazy<LoggerManager> lazy = new Lazy<LoggerManager>();

        private LoggerManager LoggerManager
        {
            get { return lazy.Value; }

        }
        // GET: ConfigNews
        string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        [PowerManage]
        public ActionResult Index()
        {
            return View(DALTuhuNews.GetTuhuNews(new SqlConnection(strConn), 1, 0));
        }
        [PowerManage]
        public ActionResult Add(string N_id)
        {
            if (!string.IsNullOrWhiteSpace(N_id))
            {
                return View(DALTuhuNews.SelectNewsbyID(new SqlConnection(strConn), Convert.ToInt32(N_id)));
            }
            else
            {
                return View(new tbl_TuhuNews() { IssueTime = DateTime.Now });
            }

        }
        [PowerManage]
        [HttpPost]
        public ActionResult Add(tbl_TuhuNews tn)
        {
            var userName = HttpContext.User.Identity.Name;
            string strGUID = System.Guid.NewGuid().ToString();
            tn.NewsGuid = new Guid(strGUID);
            int result = (int)DALTuhuNews.InsertTuhuNews(new SqlConnection(strConn), tn);
            tbl_TuhuNews Tuhunews = DALTuhuNews.SelectNewsbyID(new SqlConnection(strConn));
            if (result > 0)
            {

                var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                {
                    ObjectID = result,
                    ObjectType = "TuhuNews",
                    Author = userName,
                    Operation = "添加途虎新闻"
                };
                new Tuhu.Provisioning.Business.OprLogManagement.OprLogManager().AddOprLog(oprLog);
            }
            return Json(result);
        }
        [PowerManage]
        [HttpPost]
        public ActionResult DeleteNews(int N_Id)
        {
            var userName = HttpContext.User.Identity.Name;
            int result = DALTuhuNews.DeleteTuhuNews(new SqlConnection(strConn), N_Id);
            if (result > 0)
            {
                try
                {
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = N_Id,
                        ObjectType = "TuhuNews",
                        Author = userName,
                        Operation = "删除途虎新闻"
                    };
                    new Tuhu.Provisioning.Business.OprLogManagement.OprLogManager().AddOprLog(oprLog);
                }
                catch (Exception)
                {


                }


            }
            return Json(result);
        }
        [PowerManage]
        [HttpPost]
        public ActionResult UpdateNews(tbl_TuhuNews tn)
        {
            var userName = HttpContext.User.Identity.Name;

            int result = DALTuhuNews.UpdateTuhuNews(new SqlConnection(strConn), tn);
            if (result > 0)
            {
                try
                {

                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = tn.Id,
                        ObjectType = "TuhuNews",
                        Author = userName,
                        Operation = "修改途虎新闻"
                    };
                    new Tuhu.Provisioning.Business.OprLogManagement.OprLogManager().AddOprLog(oprLog);
                }
                catch (Exception)
                {


                }

            }
            return Json(result);
        }


        public ActionResult SelectOprLogByCommentID(int Oid)
        {
            var result = LoggerManager.SelectOprLogByParams("TuhuNews", Oid.ToString());
            return result.Count() > 0 && result != null
               ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
               : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }
    }
}
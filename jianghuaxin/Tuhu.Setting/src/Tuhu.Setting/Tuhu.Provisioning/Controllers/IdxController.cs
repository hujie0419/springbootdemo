using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Idx;

namespace Tuhu.Provisioning.Controllers
{

    public class IdxController : Controller
    {
        private Lazy<IdxManager> lazyManger = new Lazy<IdxManager>();
        private static IAutoSuppliesManager manager = new AutoSuppliesManager();
        private IdxManager IdxManager
        {
            get { return this.lazyManger.Value; }
        }
        [PowerManage]
        public ActionResult Edit()
        {
            readMsg();
            return View();
        }

        public ActionResult Prod(string apptype, string mid)
        {
            ViewBag.aid = apptype;
            ViewBag.mid = mid;
            return View();
        }

        private void readMsg()
        {
            var msg = Session["msg"];
            if (msg != null)
            {
                ViewBag.msg = msg.ToString();
            }
            else
            {
                ViewBag.msg = string.Empty;
            }
            Session["msg"] = string.Empty;
        }
        public ActionResult Save(string a)
        {
            if ("1".Equals(a))
            {
                if (IdxManager.Insert_tal_newappsetdata(smAppModel, Request.Form))
                {
                    writeMsg("添加成功");
                }
                else
                {
                    writeMsg("添加失败 " + SqlAdapter.ErrorMsg);
                }
            }
            else
            {
                if (IdxManager.Update_tal_newappsetdata(smAppModel, Request.Form))
                {
                    writeMsg("更新成功");
                }
                else
                {
                    writeMsg("更新失败 " + SqlAdapter.ErrorMsg);
                }
            }
            return Redirect("/idx/edit");
        }

        private void writeMsg(string s)
        {
            Session["msg"] = HttpUtility.HtmlEncode(s);
        }

        public ActionResult Modules()
        {
            var md = IdxManager.Get_tal_newappsetdata(smAppModel);
            if (md.IsEmpty)
            {
                return Content(error("模块不存在"));
            }
            var s = md.Serialize();

            return Content(s);
        }

        public ActionResult ModuleProds(string mid)
        {
            var md = IdxManager.Get_tbl_AdProduct(smp, mid);
            if (md.IsEmpty)
            {
                return Content(error("该模块没有产品"));
            }
            var s = md.Serialize(true);
            return Content(s);
        }

        /// <summary>
        /// 根据PID获取产品相关信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult GetProductInfo(string pid)
        {
            try
            {
                if (string.IsNullOrEmpty(pid))
                    return Content("-1");
                var resultData = manager.GetProductInfoByPID(pid);
                if (resultData != null)
                    return Content(JsonConvert.SerializeObject(resultData, new DataTableConverter()));
                else
                    return Content("-1");
            }
            catch (Exception)
            {
                return Content("-1");
            }
        }

        public ActionResult Remove(string id)
        {
            if (IdxManager.Delete_tal_newappsetdata(smAppModel, id))
            {
                writeMsg("删除成功");
            }
            else
            {
                writeMsg("删除失败");
            }
            return Redirect("/idx/edit");
        }

        public ActionResult RemoveProd(string id, string mid, string apptype)
        {
            if (IdxManager.Delete_tbl_AdProduct(smp, mid, id))
            {
                writeMsg("删除成功");
            }
            else
            {
                writeMsg("删除失败 " + HttpUtility.HtmlEncode(SqlAdapter.LastError.Message));
            }
            return Redirect("/idx/prod?mid=" + mid + "&apptype=" + apptype);
        }

        [HttpPost]
        public ActionResult SaveProd(string mid, string id, string a, string apptype)
        {
            if ("1".Equals(a))
            {             
                if (IdxManager.Insert_tbl_AdProduct(smp,Request.Form))
                {
                    writeMsg("添加成功");
                }
                else
                {
                    writeMsg("添加失败" + SqlAdapter.LastError != null ? "" : SqlAdapter.LastError.Message);
                }
            }
            else
            {               
                if (IdxManager.Update_tbl_AdProduct(smp, Request.Form, id))
                {
                    writeMsg("更新成功");
                }
                else
                {
                    writeMsg("更新失败" + SqlAdapter.LastError != null ? "" : SqlAdapter.LastError.Message);
                }
            }
            return Redirect("/idx/prod?mid=" + mid + "&apptype=" + apptype);
        }

        #region Constructor

        private SqlSchema smAppModel, smp;
        public IdxController()
        {
            smAppModel = new SqlSchema().Bind("id", SqlDbType.BigInt).Bind("apptype", SqlDbType.SmallInt).Bind("modelname", SqlDbType.NVarChar, 50).Bind("modelfloor", SqlDbType.SmallInt)
                .Bind("showorder", SqlDbType.Int).Bind("icoimgurl", SqlDbType.NVarChar, 200).Bind("jumph5url", SqlDbType.NVarChar, 200).Bind("showstatic", SqlDbType.SmallInt)
                .Bind("starttime", SqlDbType.DateTime).Bind("overtime", SqlDbType.DateTime).Bind("cpshowtype", SqlDbType.SmallInt).Bind("cpshowbanner", SqlDbType.NVarChar, 200)
                .Bind("appoperateval", SqlDbType.NVarChar, 100).Bind("operatetypeval", SqlDbType.NVarChar, 100).Bind("pronumberval", SqlDbType.NVarChar, 50).Bind("keyvaluelenth", SqlDbType.NVarChar, 500)
                .Bind("umengtongji", SqlDbType.NVarChar, 50).Bind("createtime", SqlDbType.DateTime).Bind("updatetime", SqlDbType.DateTime).Bind("Version", SqlDbType.SmallInt).Bind("edittime",SqlDbType.DateTime);
            smp = new SqlSchema().Bind("advertiseid", SqlDbType.Int).Bind("pid", SqlDbType.VarChar, 256)
                .Bind("position", SqlDbType.TinyInt).Bind("state", SqlDbType.TinyInt).Bind("createdatetime", SqlDbType.DateTime)
                .Bind("lastupdatedatetime", SqlDbType.DateTime).Bind("promotionprice", SqlDbType.Money).Bind("promotionnum", SqlDbType.Int)
                .Bind("new_modelid", SqlDbType.Int);
        }

        private string error(string msg)
        {
            var r = new Dictionary<string, string>();
            r["error"] = "true";
            r["Msg"] = msg;
            return r.ToJson();
        }
        #endregion
    }
}

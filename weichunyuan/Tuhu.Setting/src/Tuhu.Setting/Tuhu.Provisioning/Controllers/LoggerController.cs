using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;
using System.Linq;

namespace Tuhu.Provisioning.Controllers
{
    public class LoggerController : Controller
    {
        public ActionResult List(string objectType, string startDT = "", string endDT = "")
        {
            if (startDT == "" || endDT == "")
            {
                startDT = DateTime.Now.ToString("yyyy-MM-dd");
                endDT = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
            LoggerManager manger = new LoggerManager();
            List<ConfigHistory> list = manger.GetList(objectType, startDT, endDT);
            return View(list);
        }


        public ActionResult ListBoostrap(string objectType, string startDT = "", string endDT = "")
        {
            if (startDT == "" || endDT == "")
            {
                startDT = DateTime.Now.ToString("yyyy-MM-dd");
                endDT = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
               
            }
            ViewBag.StartDate = startDT;
            ViewBag.EndDate = endDT;
            LoggerManager manger = new LoggerManager();
            List<ConfigHistory> list = manger.GetList(objectType, startDT, endDT);
            return View(list);
        }

        public ActionResult LogList(int id,string type)
        {
            var result = LoggerManager.SelectOprLogByParams(type, id.ToString());
            return View(result);
        }


        public ActionResult ListLoger()
        {
            return View();
        }

        public ActionResult GetListLoger(string objectType, string startDT = "", string endDT = "")
        {
            if (startDT == "" || endDT == "")
            {
                startDT = DateTime.Now.ToString("yyyy-MM-dd");
                endDT = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
            LoggerManager manger = new LoggerManager();
            List<ConfigHistory> list = manger.GetList(objectType, startDT, endDT);
            return Content(JsonConvert.SerializeObject(list.Select(o=>new {
                o.PKID,
                o.Author,
                ChangeDatetime = o.ChangeDatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                o.Operation
            })));
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThBiz.Business.Power;

namespace Tuhu.Provisioning.Controllers
{
    public class AuthController: Controller
    {
        public JsonResult GetBasicInformantion()
        {
            //string name = User.Identity.Name;
            string name = "devteam@tuhu.work";
            return Json(new { success = !string.IsNullOrEmpty(name), user = new { name } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HasPower(string path)
        {
            bool hasPower = false;

            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Json(new {success = true, result = false}, JsonRequestBehavior.AllowGet);
            }

            string username = User.Identity.Name;           

            if (!string.IsNullOrEmpty(path))
            {
                var data = path.Split('/').Where(o => !string.IsNullOrEmpty(o)).ToList();
                if (data.Count == 1)
                {
                    data.Add("Index");
                }

                if(data.Count == 2)
                {
                    string controller = data[0];
                    string action = data[1];
                    byte issupper = 0;
                    if (System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(username))
                    {
                        issupper = 1;
                    }
                    //listPower = new PowerManage().GetBusPower(userNo, issupper, IwSystem); 
                    var listPower = new PowerManage().GetBusPower(username, issupper, "OperateSys");
                    string message = string.Empty;
                    hasPower = PowerHandle.PowerValidServer(listPower, username, controller, action, string.Empty, string.Empty, true, out message);
                }
            }

            return Json(new { success = true, result = hasPower }, JsonRequestBehavior.AllowGet);
        }
    }
}
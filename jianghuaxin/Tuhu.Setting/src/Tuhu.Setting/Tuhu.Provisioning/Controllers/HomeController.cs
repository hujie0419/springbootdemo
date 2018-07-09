using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ThBiz.Business.Power;
using ThBiz.Common.Configurations;
using ThBiz.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using BusPowerManage = ThBiz.Business.Power.PowerManage;
using EnOrDe = Tuhu.Component.Framework.EnOrDeHelper;
using IndexConfigManager = Tuhu.Provisioning.Business.Setting.IndexConfigManager;
using IndexModuleConfig = Tuhu.Provisioning.DataAccess.Entity.IndexModuleConfig;
using IndexModuleItem = Tuhu.Provisioning.DataAccess.Entity.IndexModuleItem;
namespace Tuhu.Provisioning.Controllers
{


    public class HomeController : Controller
    {
        private const string keyStr = "search12031136";
        //
        // GET: /Home
        //[PowerManage]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CardIndex()
        {
            return View();
        }

        public ActionResult Module(int moduleId)
        {
            ViewBag.moduleId = moduleId;
            return View();
        }

        private static List<ActionPower> GetAllPower(string userno)
        {
            ActionPowerList apList = new ActionPowerList();
            if (!string.IsNullOrEmpty(userno))
            {
                byte issupper = 0;
                if (System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(userno))
                {
                    issupper = 1;
                }
                apList.ListPower = new PowerManage().GetBusPower(userno, issupper);
            }
            return apList.ListPower;
        }

        public static IndexModuleConfig FilterItems(IndexModuleConfig moudle, List<ActionPower> powers)
        {
            var result = new IndexModuleConfig();
            result.PKID = moudle.PKID;
            result.ModuleName = moudle.ModuleName;
            result.Items = new List<IndexModuleItem>();
            foreach (var item in moudle.Items)
            {
                var entry = powers.Where(config => config.ParentID != 0 && config.Controller.Equals(item.Controller) && config.Action.Equals(item.Action)).FirstOrDefault();
                if (entry == null)
                {
                    item.LinkUrl = $"/{item.Controller}/{item.Action}";
                    result.Items.Add(item);
                }
                else if (!entry.LinkName.Contains("NotDisPlay") && entry.IsActive)
                {
                    string childDomain = ModuleDomainConfig.GetDomainByModule(entry.LinkName);
                    if (string.IsNullOrEmpty(childDomain))
                    {
                        var list = powers.FirstOrDefault(p => p.PKID == entry.ParentID);
                        string domain = !string.IsNullOrWhiteSpace(list?.LinkName) ? ModuleDomainConfig.GetDomainByModule(list?.LinkName) : string.Empty;
                        childDomain = domain;
                    }
                    string md5 = "Info=" + entry.PKID.ToString();
                    string parameters = EnOrDe.GetMd5(md5 + EnOrDe.GetMd5(keyStr, System.Text.Encoding.UTF8), System.Text.Encoding.UTF8);
                    if (string.IsNullOrEmpty(entry.ParametersName))
                    {
                        item.LinkUrl = $"{childDomain}/{entry.Controller}/{entry.Action}?KEY={parameters}";
                        result.Items.Add(item);
                    }
                    else
                    {
                        int i = 0;
                        foreach (string name in entry.ParametersName.Split('.'))
                        {
                            if (string.IsNullOrEmpty(entry.ParametersValue))
                            {
                                continue;
                            }
                            else
                            {
                                if (entry.ParametersValue.Split(',')[i].ToLower().IndexOf("yyyy", StringComparison.Ordinal) == 0)
                                {
                                    parameters = parameters + "&" + name + "=" + DateTime.Now.ToString(entry.ParametersValue.Split(',')[i]);
                                }
                                else
                                {
                                    parameters = parameters + "&" + name + "=" + entry.ParametersValue.Split(',')[i];
                                }
                            }
                            i += 1;
                        }

                        item.LinkUrl = $"{childDomain}/{entry.Controller}/{entry.Action}?KEY={parameters}";
                        result.Items.Add(item);
                    }
                }
            }
            return result;
        }
        
        public static List<IndexModuleItem> GetAvailableIndexEntryByMoudleId(string userno, int moudleId)
        {
            var allModules = IndexConfigManager.GetAllIndexModulesFormCache();
            var moudle = allModules.FirstOrDefault(m => m.PKID == moudleId);
            var allPower = GetAllPower(userno);
            var result = FilterItems(moudle, allPower);
            return result.Items;
        }
        public static List<IndexModuleConfig> GetAvaliableIndexModule(string userno)
        {
            var moduleList = IndexConfigManager.GetAllIndexModules();
            var powerList = GetAllPower(userno);
            for(var index = 0; index < moduleList.Count; ++index)
            {
                var itemList = IndexConfigManager.GetIndexItems(moduleList.ElementAt(index).PKID);
                if(itemList == null || itemList.Count == 0)
                {
                    moduleList.RemoveAt(index);
                    --index;
                    continue;
                }
                bool existAvaliableItem = false;
                foreach(var item in itemList)
                {
                    if (powerList.Where(power => power.Action.Equals(item.Action) && power.Controller.Equals(item.Controller) && (!power.IsActive || power.LinkName.Contains("NotDisPlay"))).Count() == 0)
                    {
                        existAvaliableItem = true;
                        break;
                    }
                }
                if (!existAvaliableItem)
                {
                    moduleList.RemoveAt(index);
                    --index;
                    continue;
                }
            }
            return moduleList;
        }


        [HttpPost]
        public ActionResult IsSupperUser()
        {
            MJsonResult jsonObj = new MJsonResult { Status = false, Msg = "" };
            if (System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(User.Identity.Name + "|"))
                jsonObj.Status = true;
            return Json(jsonObj);
        }
        [HttpPost]
        public ActionResult GetBtnPower(string Info, string BtnKey, string Key = "")
        {
            MJsonResult jsonObj = new MJsonResult { Status = false, Msg = "" };
            //if (Session["CaPowerList"] == null)
            //{
            //    byte isSupper = 0;
            //    if (System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(User.Identity.Name + "|"))
            //        isSupper = 1;
            //    Session["CaPowerList"] = new BusPowerManage().GetBusPower(User.Identity.Name, isSupper);
            //}
            byte isSupper = (byte)(System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(User.Identity.Name) ? 1 : 0);
            List<ActionPower> list = new BusPowerManage().GetBusPower(User.Identity.Name, isSupper);//(List<ActionPower>)Session["CaPowerList"];
            if (list == null || list.Count == 0)
            {
                jsonObj.Status = true;
                jsonObj.Msg = "[]";
                return Json(jsonObj);
            }
            var infos = Info.Split('_');
            var tmpList = list.Where(ap => ap.Controller.ToLower() == infos[0].ToLower() && ap.Action.ToLower() == infos[1].ToLower()).ToList();
            if (tmpList == null || tmpList.Count == 0)
            {
                jsonObj.Status = true;
                jsonObj.Msg = "[]";
                return Json(jsonObj);
            }
            int id = 0;
            foreach (ActionPower ap in tmpList)
            {
                if (string.IsNullOrEmpty(Key))
                {
                    id = tmpList[0].PKID;
                    break;
                }
                else
                {
                    if (EnOrDe.GetMd5("Info=" + ap.PKID.ToString() + EnOrDe.GetMd5(keyStr, Encoding.UTF8), Encoding.UTF8).Equals(Key))
                    {
                        id = ap.PKID;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (string.IsNullOrEmpty(BtnKey))
            {
                list = list.Where(ap => ap.ParentID == id && !string.IsNullOrEmpty(ap.BtnKey)).ToList();
            }
            else
            {
                list = list.Where(ap => ap.ParentID == id && ap.BtnKey == BtnKey).ToList();
            }
            if (list == null || list.Count == 0)
            {
                jsonObj.Status = true;
                jsonObj.Msg = "[]";
                return Json(jsonObj);
            }
            jsonObj.Status = true;
            string tmp = "";
            foreach (ActionPower ap in list)
            {
                tmp += "{BtnKey:\"" + ap.BtnKey + "\",BtnType:\"" + ap.BtnType + "\"},";
            }
            tmp = "[" + tmp.TrimEnd(',') + "]";
            jsonObj.Msg = tmp;
            return Json(jsonObj);
        }

        public ActionResult ErrorPage(string error)
        {
            //return "<span style=\"color:red\">" + error + "</span> <a href=\"/Account/LogOn\">进入登录页</a>";
            return Redirect(ThBiz.Common.Configurations.YewuDomainConfig.YewuSite + "/Account/LogOn");
        }

        public async System.Threading.Tasks.Task<ActionResult> GetBrandName()
        {
            //
            Dictionary<object, object> dic = new Dictionary<object, object>();
            string vehicleId = null;
            string tid = null;
            var request = new Service.Product.Request.SelectPropertyValuesRequest { PropertyName = "CP_Brand" };
            request.AddParameter("Category", "Tires");
            request.AddParameter("OnSale", "1");
            request.AddParameter("stockout", "0");
            if (!string.IsNullOrWhiteSpace(vehicleId))
                request.AddParameter("VehicleMatch", vehicleId);
            if (!string.IsNullOrWhiteSpace(tid))
                request.AddParameter("adpterTid", tid);

            using (var client = new Service.Product.ProductSearchClient())
            {
                var result = await client.SelectPropertyValuesAsync(request);

                result.ThrowIfException(true);
                dic.Add("TiresBrand", result?.Result?.Keys);
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(dic));
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> GetTireBrand()
        {
            //
            Dictionary<object, object> dic = new Dictionary<object, object>();
            string vehicleId = null;
            string tid = null;
            var request = new Service.Product.Request.SelectPropertyValuesRequest { PropertyName = "CP_Brand" };
            request.AddParameter("Category", "Tires");
            request.AddParameter("OnSale", "1");
            request.AddParameter("stockout", "0");
            if (!string.IsNullOrWhiteSpace(vehicleId))
                request.AddParameter("VehicleMatch", vehicleId);
            if (!string.IsNullOrWhiteSpace(tid))
                request.AddParameter("adpterTid", tid);

            using (var client = new Service.Product.ProductSearchClient())
            {
                var result = await client.SelectPropertyValuesAsync(request);

                result.ThrowIfException(true);
                dic.Add("TiresBrand", result?.Result?.Keys.Select(_=>new {
                    BrandName=_
                }));
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(dic));
            }
        }
    }
}

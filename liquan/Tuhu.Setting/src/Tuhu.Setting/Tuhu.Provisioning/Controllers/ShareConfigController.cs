using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.ShareConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareConfigController : Controller
    {
        private readonly Lazy<ShareConfigManager> lazy = new Lazy<ShareConfigManager>();

        private ShareConfigManager ShareConfigManager
        {
            get { return lazy.Value; }
        }
        // GET: ShareConfig
        public async Task<ActionResult> Index(ShareConfigQuery query)
        {
            query.PageIndex = query.PageIndex == 0 ? 1 : query.PageIndex;
            var datas = ShareConfigManager.SelectShareConfig(query);
            ViewBag.query = query;
            return View(datas ?? new List<ShareConfigSource>());
        }

        public ActionResult HistoryModule(int id)
        {
            ViewBag.Title = "历史";
            List<ShareConfigLog> scm = ShareConfigManager.SelectShareConfigLogById(id);
            return View(scm);
        }

        public JsonResult GetShareSConfigByJumpId(int jumpId)
        {
            List<ShareSupervisionConfig> ssc = ShareConfigManager.SelectShareSConfigByJumpId(jumpId);
            return Json(ssc);
        }

        public ActionResult TemplateEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewBag.Title = "新增";
                ShareConfigSource scs = new ShareConfigSource()
                {
                    PKId = 0,
                    Location = null,
                    Description = null,
                    Status = 1,
                };
                return View(scs);
            }
            else if (id > 0)
            {
                ViewBag.Title = "修改";
                ShareConfigQuery query = new ShareConfigQuery
                {
                    IdCriterion = id,
                    PageIndex = 1,
                };
                var datas = ShareConfigManager.SelectShareConfig(query);
                ShareConfigSource scs = datas.FirstOrDefault() ?? new ShareConfigSource();
                return View(scs);
            }
            else
            {
                ViewBag.Title = "复制";
                ShareConfigQuery query = new ShareConfigQuery
                {
                    IdCriterion = -id,
                    PageIndex = 1,
                };
                var datas = ShareConfigManager.SelectShareConfig(query);
                ShareConfigSource scs = datas.FirstOrDefault() ?? new ShareConfigSource();
                scs.PKId = id;
                return View(scs);
            }
        }
        public int CheckLocationExist(string location, string specialParam = null)
        {
            int pkid = ShareConfigManager.SelectPKIdByLocation(location, string.IsNullOrEmpty(specialParam) ? null : specialParam);
            return pkid;
        }
        public ActionResult SaveShareConfig(ShareConfigSource scs)
        {
            if (scs.PKId == 0)
            {
                scs.CreateDateTime = DateTime.Now;
                scs.UpdateDateTime = DateTime.Now;
                scs.Creator = ThreadIdentity.Operator.Name;
                int pkid = ShareConfigManager.InsertShareConfig(scs);
                scs.PKId = pkid;
                //写操作记录
                ShareConfigLog scl = new ShareConfigLog()
                {
                    ConfigId = scs.PKId,
                    Operator = ThreadIdentity.Operator.Name,
                    OperateType = 0,
                    CreateDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now
                };
                ShareConfigManager.InsertShareConfigLog(scl);
            }
            else if (scs.PKId > 0)
            {
                scs.UpdateDateTime = DateTime.Now;
                string oldlocation = ShareConfigManager.SelectLocationByPKId(scs.PKId);
                string newlocation = scs.Location;
                ShareConfigManager.UpdateShareConfig(scs);
                //写操作记录
                ShareConfigLog scl = new ShareConfigLog()
                {
                    ConfigId = scs.PKId,
                    Operator = ThreadIdentity.Operator.Name,
                    OperateType = 1,
                    CreateDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now
                };
                ShareConfigManager.InsertShareConfigLog(scl);
            }
            else
            {
                scs.CreateDateTime = DateTime.Now;
                scs.UpdateDateTime = DateTime.Now;
                scs.Creator = ThreadIdentity.Operator.Name;
                int pkid = ShareConfigManager.InsertShareConfig(scs);
                scs.PKId = pkid;
                //写操作记录
                ShareConfigLog scl = new ShareConfigLog()
                {
                    ConfigId = scs.PKId,
                    Operator = ThreadIdentity.Operator.Name,
                    OperateType = 0,
                    CreateDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now
                };
                ShareConfigManager.InsertShareConfigLog(scl);
            }

            return Json(scs.PKId);
        }
        public async Task<ActionResult> ShareSConfigEditModule(string location, int jumpid, int id = 0)
        {
            //ViewBag.MiniGramList = await ShareConfigManager.SelectWxConfigsAsync();
            if (id == 0)
            {
                ViewBag.Title = "创建分享渠道";
                ShareSupervisionConfig ssc = new ShareSupervisionConfig()
                {
                    PKId = 0,
                    Location = location,
                    ShareScene = 0,
                    Status = 1,
                    ShareType = 0,
                    SceneSequence = 0,
                    JumpId = jumpid,
                };
                return View(ssc);
            }
            else if (id < 0)
            {
                ViewBag.Title = "复制分享渠道";
                ShareSupervisionConfig ssc = ShareConfigManager.SelectShareSConfigById(-id);
                ssc.PKId = 0;
                return View(ssc);
            }
            else
            {
                ViewBag.Title = "修改分享渠道";
                ShareSupervisionConfig ssc = ShareConfigManager.SelectShareSConfigById(id);
                return View(ssc);
            }
        }
        public JsonResult CheckShareSConfig(int pkid, int jumpId, int sharescene)
        {
            bool flag = true;
            List<ShareSupervisionConfig> ssc = ShareConfigManager.SelectShareSConfigByJumpId(jumpId);
            if (pkid == 0)
            {
                for (int i = 0; i < ssc.Count; i++)
                {
                    if (ssc[i].ShareScene == sharescene)
                    {
                        flag = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ssc.Count; i++)
                {
                    if (ssc[i].PKId != pkid && ssc[i].ShareScene == sharescene)
                    {
                        flag = false;
                    }
                }
            }
            return Json(flag);
        }
        public ActionResult SaveShareSConfig(ShareSupervisionConfig ssc)
        {
            if (ssc.PKId == 0)
            {
                ssc.CreateDateTime = DateTime.Now;
                ssc.UpdateDateTime = DateTime.Now;
                ssc.Creator = ThreadIdentity.Operator.Name;
                bool flag = ShareConfigManager.InsertShareSConfig(ssc);
            }
            else
            {
                ssc.UpdateDateTime = DateTime.Now;
                bool flag = ShareConfigManager.UpdateShareSConfig(ssc);
            }
            ShareConfigLog scl = new ShareConfigLog()
            {
                ConfigId = ssc.JumpId,
                Operator = ThreadIdentity.Operator.Name,
                OperateType = 1,
                CreateDateTime = DateTime.Now,
                LastUpdateDateTime = DateTime.Now
            };
            bool flaglog = ShareConfigManager.InsertShareConfigLog(scl);
            return Redirect("TemplateEdit?id=" + ssc.JumpId);
        }
        public bool DelShareSConfig(int id, int configid)
        {
            ShareConfigLog scl = new ShareConfigLog()
            {
                ConfigId = configid,
                Operator = ThreadIdentity.Operator.Name,
                OperateType = 1,
                CreateDateTime = DateTime.Now,
                LastUpdateDateTime = DateTime.Now
            };
            bool flaglog = ShareConfigManager.InsertShareConfigLog(scl);
            return ShareConfigManager.DeleteShareSConfigById(id);
        }
        public bool DelShareConfig(string location, string specialParam)
        {
            //删除相关操作记录
            int pkid = ShareConfigManager.SelectPKIdByLocation(location, string.IsNullOrEmpty(specialParam) ? null : specialParam);
            if (pkid == 0)
                return false;
            return ShareConfigManager.DeleteShareConfigByLocation(location, string.IsNullOrEmpty(specialParam) ? null : specialParam, pkid);
        }
        public bool CopyShareSConfigs(string location, int id, int newId)
        {
            List<ShareSupervisionConfig> sscList = ShareConfigManager.SelectShareSConfigByJumpId(id);
            return ShareConfigManager.InsertShareSConfigs(sscList, location, ThreadIdentity.Operator.Name, newId);
        }
        public JsonResult CacheUpdate()
        {
            try
            {
                using (var client = new ConfigClient())
                {
                    var res = client.RefreshShareSupervisionCache();
                    res.ThrowIfException(true);
                    if (res.Success && res.Result)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(-1);
                    }
                }
            }
            catch (Exception exp)
            {
                Component.Framework.Extension.WebLog.LogException(exp);
                return Json(-1);
            }
        }
    }
}
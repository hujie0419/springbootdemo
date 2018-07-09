using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.ThirdPartyCouponConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdPartyCouponConfigController : Controller
    {


        public ActionResult Index(int pageNum = 1, int pageSize = 50)
        {
            var models = ThirdPartyCouponConfigManager.SelecThirdPartyCouponConfigModels(pageNum, pageSize);
            var listModels = new ListModel<ThirdPartyCouponConfigModel>
            {
                Pager = new PagerModel(pageNum, pageSize)
                {
                    TotalItem = models.Select(r => r.TotalCount).FirstOrDefault()
                },
                Source = models
            };
            return View(listModels);
        }

        public ActionResult SelectThirdPartyCouponConfigsByChannelAndPatch(string thirdPartyChannel, string thirdPartyCouponPatch)
        {
            return Json(ThirdPartyCouponConfigManager.SelectThirdPartyCouponConfigsByChannelAndPatch(thirdPartyChannel, thirdPartyCouponPatch));
        }

        public ActionResult ThirdPartyCouponConfigDel(int id)
        {
            var before = ThirdPartyCouponConfigManager.SelecThirdPartyCouponConfigModelById(id);
            if (ThirdPartyCouponConfigManager.DeleteThirdPartyCouponConfig(id))
            {
                new OprLogManager().AddOprLog(new OprLog()
                {
                    Author = HttpContext.User.Identity.Name,
                    BeforeValue = JsonConvert.SerializeObject(before),
                    AfterValue = "",
                    ChangeDatetime = DateTime.Now,
                    ObjectID = id,
                    ObjectType = "Third",
                    Operation = "删除第三方优惠券配置",
                    HostName = Request.UserHostName
                });
                return Json(new { Status = 0 });
            }
            else
                return Json(new { Status = -1 });
        }

        public ActionResult ThirdPartyCouponEdit(int id)
        {
            var model = ThirdPartyCouponConfigManager.SelecThirdPartyCouponConfigModelById(id);
            return View(model ?? new ThirdPartyCouponConfigModel());

        }
        public ActionResult SaveThirdPartyCouponConfig(ThirdPartyCouponConfigModel model)
        {
            //try
            //{
            if (model.PKID > 0)
            {
                if (new ThirdPartyCouponConfigManager().UpdateThirdPartyCouponConfig(model) > 0)
                {
                    new OprLogManager().AddOprLog(new OprLog()
                    {
                        Author = HttpContext.User.Identity.Name,
                        BeforeValue = JsonConvert.SerializeObject(model),
                        AfterValue = "",
                        ChangeDatetime = DateTime.Now,
                        ObjectID = model.PKID,
                        ObjectType = "Third",
                        Operation = "修改第三方优惠券配置",
                        HostName = Request.UserHostName
                    });
                    return Json(new { Status = 1 });
                }
                else
                {
                    return Json(new { Status = -1 });
                }
            }
            else
            {
                var result = new ThirdPartyCouponConfigManager().InsertThirdPartyCouponConfig(model);
                if (result > 0)
                {
                    new OprLogManager().AddOprLog(new OprLog()
                    {
                        Author = HttpContext.User.Identity.Name,
                        BeforeValue = JsonConvert.SerializeObject(model),
                        AfterValue = "",
                        ChangeDatetime = DateTime.Now,
                        ObjectID = result,
                        ObjectType = "Third",
                        Operation = "新增第三方优惠券配置",
                        HostName = Request.UserHostName
                    });
                    return Json(new { Status = 1 });
                }
                else
                {
                    return Json(new { Status = -1 });
                }

            }
            // }
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);
            //}
        }

        public ActionResult SelectGetCouponRulesByGetRuleId(Guid getRuleId)
        {
            return Json(ThirdPartyCouponConfigManager.SelectGetCouponRulesByGetRuleId(getRuleId));
        }


        public ActionResult ViewThirdPartyCouponConfigLog(int pkid)
        {
            var content = "";
            var result = LoggerManager.SelectOprLogByParams("Third", pkid.ToString());
            var configHistories = result as ConfigHistory[] ?? result.ToArray();
            if (result != null && configHistories.Any())
            {
                content = configHistories.Aggregate(content, (current, h) => current + ("<span style='display:block;margin-bottom:5px;'>操作人：" + h.Author + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;操作：" + h.Operation + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;操作时间：" + h.ChangeDatetime + "</span>"));
            }
            else
                content = "暂无记录";
            return Content(content);
        }
    }
}

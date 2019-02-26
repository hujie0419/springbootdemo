using System;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Collections.Generic;
using System.Data;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Business.ServiceProxy;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 会员生活权益
    /// </summary>
    public class UserLivingRightsController : Controller
    {
        /// <summary>
        /// 会员生活权益首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 会员权益列表数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetDataByPage(UserPermissionRequest request)
        {
            var returnValue = new List<MemberShipLivingRightsModel>();
            if (request == null)
            {
                return Json(new { code = -1, msg = "无参数信息", count = 0, data = returnValue }, JsonRequestBehavior.AllowGet);
            }
            if (request.Page <= 0)
            {
                request.Page = 1;
            }
            if (request.Limit <= 0)
            {
                request.Limit = 50;
            }
            var memberBll = new MemberService();
            var requestModel = new MemberShipLivingRightsModel { ChannelName = request.ChannelName };
            var count = DALMembershipLivingRights.QueryPageCount(requestModel);
            returnValue = DALMembershipLivingRights.QueryPageList(requestModel, request.Page, request.Limit);
            return Json(new { code = 0, msg = "加载成功", count = count, data = returnValue.OrderByDescending(t => t.PKID) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除会员权益
        /// </summary>
        /// <param name="id">权益Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string ids)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return Json(new { data = 0, msg = "未能获取数据" });
                }

                var operLogManager = new Business.OprLogManagement.OprLogManager();
                var result = 0;
                foreach (var strId in ids.Split(','))
                {
                    if (string.IsNullOrWhiteSpace(strId))
                    {
                        continue;
                    }
                    var id = int.Parse(strId);
                    result += DALMembershipLivingRights.Delete(id);
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = id,
                        ObjectType = "UserLivingRights",
                        Author = HttpContext.User.Identity.Name,
                        Operation = "删除UserLivingRights配置"
                    };
                    operLogManager.AddOprLogAsync(oprLog);
                }
                return Json(new { data = result, msg = "删除成功" });
            }
            catch (Exception e)
            {
                return Json(new { data = -1, msg = "删除异常：" + e.ToString() });
            }
        }

        /// <summary>
        /// 新增编辑页面数据加载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                ViewBag.Title = "添加会员生活权益信息";
                return View(new MemberShipLivingRightsModel());
            }
            else
            {
                ViewBag.Title = "编辑会员生活权益信息";
                var model = DALMembershipLivingRights.GetModelById(id);
                if (model == null)
                {
                    model = new MemberShipLivingRightsModel();
                }
                return View(model);
            }
        }

        /// <summary>
        /// 编辑操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(MemberShipLivingRightsModel model)
        {
            if (model == null)
            {
                return Json(new { result = 0, msg = "无法获取数据对象" });
            }
            var result = 0;

            model.LastUpdateBy = HttpContext.User.Identity.Name;
            if (model.PKID <= 0)
            {
                model.LastUpdateBy = model.LastUpdateBy;
                result = DALMembershipLivingRights.Add(model);
            }
            else
            {
                model.LastUpdateDateTime = DateTime.Now;
                result = DALMembershipLivingRights.Update(model);
            }
            var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
            {
                ObjectID = model.PKID,
                ObjectType = "UserLivingRights",
                Author = HttpContext.User.Identity.Name,
                Operation = model.PKID > 0 ? "更新" : "新增" + "UserLivingRights配置"
            };
            var operLogManager = new Business.OprLogManagement.OprLogManager();
            operLogManager.AddOprLogAsync(oprLog);
            return Json(new { result = result, msg = "操作成功" });
        }
    }
}
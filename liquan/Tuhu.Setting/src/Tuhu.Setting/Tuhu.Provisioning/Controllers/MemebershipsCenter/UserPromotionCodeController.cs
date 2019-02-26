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
    public class UserPromotionCodeController : Controller
    {
        /// <summary>
        /// 权益奖励首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var memberBll = new MemberService();
            var memberGradeList = memberBll.GetMembershipsGradeList();
            ViewBag.MemberGrade = memberGradeList;
            return View();
        }

        /// <summary>
        /// 权益奖励列表数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetDataByPage(UserPermissionRequest request)
        {
            var returnValue = new List<UserPromotionCodeModel>();
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
            var requestModel = new UserPromotionCodeModel
            {
                MembershipsGradeId = request.MembershipsGradeId <= 0 ? 1 : request.MembershipsGradeId,
                Name = request.Name
            };
            var count = DALUserPromotionCode.QueryPageCount(requestModel);
            returnValue = DALUserPromotionCode.QueryPageList(requestModel, request.Page, request.Limit);
            return Json(new { code = 0, msg = "加载成功", count = count, data = returnValue.OrderByDescending(t => t.Id) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除权益奖励
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
                    result += DALUserPromotionCode.Delete(id);
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = id,
                        ObjectType = "UserPromotionCode",
                        Author = HttpContext.User.Identity.Name,
                        Operation = "删除UserPromotionCode配置"
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
        public ActionResult Edit(int id,int membershipsGradeId,string memberGradeName)
        {
            if (membershipsGradeId > 0)
            {
                var queryModel = new UserPermissionModel() { MembershipsGradeId = membershipsGradeId };
                //获取对应的生活权益(当前可观的时间范围内，相同等级下的会员权益不会超过100条)
                var userPermissionList = DALUserPermission.QueryPageList(queryModel, 1, 100);
                ViewBag.UserPermissionList = userPermissionList;
            }
            if (ViewBag.UserPermissionList == null)
            {
                ViewBag.UserPermissionList = new List<UserPermissionModel>();
            }
            ViewBag.MembershipsGradeId = membershipsGradeId;
            ViewBag.MemberGradeName = memberGradeName;
            if (id <= 0)
            {
                ViewBag.Title = "添加权益奖励信息";
               
                return View(new UserPromotionCodeModel());
            }
            else
            {
                ViewBag.Title = "编辑权益奖励信息";
                var model = DALUserPromotionCode.GetModelById(id);
                if (model == null)
                {
                    model = new UserPromotionCodeModel();
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
        public JsonResult Save(UserPromotionCodeModel model)
        {
            if (model == null)
            {
                return Json(new { result = 0, msg = "无法获取数据对象" });
            }
            var result = 0;

            model.LastUpdateBy = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(model.RewardValue))
            {
                model.RewardValue = "0";
            }
            if (string.IsNullOrWhiteSpace(model.RewardId))
            {
                model.RewardId = "0";
            }
            if (model.Id <= 0)
            {
                model.LastUpdateBy = model.LastUpdateBy;
                result = DALUserPromotionCode.Add(model);
            }
            else
            {
                model.LastUpdateDateTime = DateTime.Now;
                result = DALUserPromotionCode.Update(model);
            }
            var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
            {
                ObjectID = model.Id,
                ObjectType = "UserPromotionCode",
                Author = HttpContext.User.Identity.Name,
                Operation = model.Id > 0 ? "更新" : "新增" + "UserPromotionCode配置"
            };
            var operLogManager = new Business.OprLogManagement.OprLogManager();
            operLogManager.AddOprLogAsync(oprLog);
            return Json(new { result = result, msg = "操作成功" });
        }
    }
}
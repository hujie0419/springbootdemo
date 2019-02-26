using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CouponActivityConfigV2;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class CouponActivityConfigV2Controller : Controller
    {
        // GET: CouponActivityConfigV2
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取蓄电池/加油卡浮动配置列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCouponActivityConfigs(CouponActivityConfigPageRequestModel request)
        {
            var result = new BaoYangResultEntity<Tuple<int, List<CouponActivityConfigV2Model>>>()
            { Status = false };
            if (request == null)
            {
                result.Msg = "参数验证失败";
                return Json(result);
            }
            if (request.PageIndex <= 0)
            {
                request.PageIndex = 1;
            }
            if (request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            var manager = new CouponActivityConfigManagerV2();
            result = manager.GetCouponActivityConfigs(request);
            return Json(result);
        }
        /// <summary>
        /// 保存蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveCouponActivityConfig(CouponActivityConfigV2Model request)
        {
            BaoYangResultEntity<bool> result = new BaoYangResultEntity<bool>() { Status = false };
            if (request == null ||
                string.IsNullOrWhiteSpace(request.ActivityImage) ||
                string.IsNullOrWhiteSpace(request.LayerImage) ||
                string.IsNullOrWhiteSpace(request.ActivityName) ||
                request.Type <= 0
                )
            {
                result.Status = false;
                result.Msg = "参数验证失败";
                return Json(result);
            }
            var manager = new CouponActivityConfigManagerV2();
            if (request.Channels != null && request.Channels.Any())
            {
                request.Channels = request.Channels.Distinct().ToList();
                request.Channel = string.Join(",", request.Channels);
            }
             
            var resultValidation = manager.Validation(request);
            if (resultValidation.Status)
            {
                if (request.Id > 0)
                {
                    result = manager.UpdateCouponActivityConfig(request, User.Identity.Name);
                }
                else
                {
                    result = manager.InsertCouponActivityConfig(request, User.Identity.Name);
                }
            }
            else
            {
                result = resultValidation;
            }
            return Json(result);

        }
        /// <summary>
        /// 获取单个蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCouponActivityConfig(int id)
        {
            var result = new BaoYangResultEntity<CouponActivityConfigV2Model>() { Status = false };
            if (id <= 0)
            {
                result.Msg = "参数验证失败";
                result.Status = false;
                return Json(result);
            }
            var manager = new CouponActivityConfigManagerV2();
            result = manager.GetCouponActivityConfig(id);
            return Json(result);
        }
        /// <summary>
        /// 删除蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DeleteChannelConfigsByConfigId(int id)
        {
            var result = new BaoYangResultEntity<bool>() { Status = false };
            if (id <= 0)
            {
                result.Msg = "参数验证失败";
                result.Status = false;
                return Json(result);
            }
            var manager = new CouponActivityConfigManagerV2();
            result = await manager.DeleteChannelConfigsByConfigId(id, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 清除蓄电池/加油卡浮动配置缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> RemoveCouponActivityConfigCache(int id)
        {
            var result = new BaoYangResultEntity<bool>() { Status = false };
            var manager = new CouponActivityConfigManagerV2();
            result = await manager.RemoveCouponActivityConfigCache(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
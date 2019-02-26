using System;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Collections.Generic;
using System.Data;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 会员签到控制器
    /// </summary>
    public class UserDailyCheckInController : Controller
    {

        #region  签到配置信息
        /// <summary>
        /// 获取所有签到配置信息
        /// </summary>
        /// <returns></returns>
        [PowerManage]
        public JsonResult GetData()
        {
            var returnValue = new List<UserDailyCheckInConfigModel>();
            returnValue = DalUserDailyCheckInConfig.SearchAllList();
            return Json(new { code = 0, msg = "加载成功", data = returnValue.OrderBy(t => t.ContinuousDays) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public async System.Threading.Tasks.Task<JsonResult> SaveAsync(List<UserDailyCheckInConfigModel> dataList)
        {
            if (dataList == null || dataList.Count <= 0)
            {
                return Json(new { result = false, msg = "暂无获取数据" });
            }
            var result = false;
            var  updateUser= HttpContext.User.Identity.Name;
            DalUserDailyCheckInConfig.Delete(updateUser);
            foreach (var dataModel in dataList)
            {
                dataModel.LastUpdateBy = updateUser;
                result = DalUserDailyCheckInConfig.Create(dataModel);
            }
            if (result)
            {
                var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                {
                    ObjectID = 0,
                    ObjectType = "UserLivingRights",
                    Author = HttpContext.User.Identity.Name,
                    Operation = "更新UserDailyCheckInConfig配置信息"
                };
                var operLogManager = new Business.OprLogManagement.OprLogManager();
                operLogManager.AddOprLogAsync(oprLog);

                //更新member服务中缓存
               await new Business.ServiceProxy.MemberService().RefreshUserDailyCheckInConfigCacheAsync();
                return Json(new { result = result, msg = "操作成功" });
            }
            return Json(new { result = false, msg = "操作失败" });
        }
    }
}
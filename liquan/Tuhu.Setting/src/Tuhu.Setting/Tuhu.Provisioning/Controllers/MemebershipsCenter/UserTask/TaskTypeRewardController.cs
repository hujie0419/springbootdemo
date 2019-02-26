using System;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Collections.Generic;
using System.Data;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Controllers
{
    public class TaskTypeRewardController : Controller
    {
        /// <summary>
        /// 获取所有任务类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTaskType()
        {
            var returnValue = new List<TaskTypeModel>();
            returnValue = DalTaskType.SearchAllTaskType();
            return Json(new { code = 0, msg = "加载成功", data = returnValue.OrderBy(t => t.SortIndex) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有任务奖励信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [PowerManage]
        public JsonResult GetTaskTypeReward()
        {
            var returnValue = new List<TaskTypeRewardModel>();
            returnValue = DalTaskTypeReward.SearchAllList();
            return Json(new { code = 0, msg = "加载成功", data = returnValue }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除会员权益
        /// </summary>
        /// <param name="id">权益Id</param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public JsonResult DeleteTaskTypeReward(int id)
        {
            try
            {
                if (id<=0)
                {
                    return Json(new { data = 0, msg = "未能获取数据" });
                }

                var operLogManager = new Business.OprLogManagement.OprLogManager();
                var result =DalTaskTypeReward.Delete(id);
                if (result)
                {
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = id,
                        ObjectType = "TaskTypeReward",
                        Author = HttpContext.User.Identity.Name,
                        Operation = "删除TaskTypeReward配置"
                    };
                    operLogManager.AddOprLogAsync(oprLog);
                    return Json(new { data = 1, msg = "删除成功" });
                }
                return Json(new { data = 0, msg = "删除失败" });
            }
            catch (Exception e)
            {
                return Json(new { data = -1, msg = "删除异常：" + e.ToString() });
            }
        }
    }

}
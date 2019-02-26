using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class ExchangeCenterConfigController : Controller
    {
        private static readonly Lazy<ExchangeCenterConfigManager> lazy = new Lazy<ExchangeCenterConfigManager>();

        private ExchangeCenterConfigManager ExchangeCenterConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        /// <summary>
        /// 首页数据加载
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var memberBll = new MemberService();
            var memberGradeList = memberBll.GetMembershipsGradeList();
            ViewBag.MemberGrade = memberGradeList;
            ViewBag.PostionData = GetPostionData();
            return View();
        }

        /// <summary>
        /// 获取配置位数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> GetPostionData()
        {
            var dic = new Dictionary<string, string>()
            {
                { "2","换好物"},
                { "3","拼手气"},
                { "4","兑神券"},
            };
            return dic;
        }
        /// <summary>
        /// 积分兑换配置详情页面展示数据列表
        /// </summary>
        /// <param name="SearchWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult List(ExchangeCenterConfigRequest request)
        {
            int count = 0;
            var searchModel = new ExchangeCenterConfig()
            {
                PostionCode=request.PostionCode,
                UserRank=request.UserRank,
                CouponName=request.CouponName,
                SearchStatus = request.Status
            };
            var lists = DALExchangeCenterConfig.GetExchangeCenterConfigList(searchModel, request.PageSize, request.PageIndex, out count);
            var memberBll = new MemberService();
            var memberGradeList = memberBll.GetMembershipsGradeList();
            var positionList = GetPostionData();
            foreach (var item in lists)
            {
                if (string.IsNullOrWhiteSpace(item.UserRank))
                {
                    continue;
                }
                foreach (var np in positionList)
                {
                    if (np.Key == item.PostionCode)
                    {
                        item.Postion = np.Value;
                        break;
                    }
                }
                item.GradeName = memberGradeList.Find(t => t.GradeCode.ToLower() == item.UserRank.ToLower())?.GradeName;
            }
            var list = new OutData<List<ExchangeCenterConfig>, int>(lists, count);
            var pager = new PagerModel(request.PageIndex,request.PageSize)
            {
                TotalItem = count
            };

         
         

            return View(new ListModel<ExchangeCenterConfig>(list.ReturnValue, pager));
        }

        /// <summary>
        /// 刷新积分兑换缓存
        /// </summary>
        /// <returns></returns>
        public JsonResult RefreshCache()
        {
            using (var client = new MemberMallClient())
            {
               var response =  client.RefreshMemberMallConfigs();
                response.ThrowIfException(true);
            }
            return Json(new {Code = 1});
        }

        /// <summary>
        /// 编辑界面数据绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0,string type="")
        {
            var memberBll = new MemberService();
            var memberGradeList = memberBll.GetMembershipsGradeList();
            ViewBag.MemberGrade = memberGradeList;
            ViewBag.PostionData = GetPostionData();

            ExchangeCenterConfig model = new ExchangeCenterConfig();
            if (id <= 0)
            {
                model.EndTime = DateTime.Now.AddDays(7);
                model.Status = true;//新增默认启动
            }
            else
            {
                model = ExchangeCenterConfigManager.GetExchangeCenterConfig(id);
            }
            if(!string.IsNullOrWhiteSpace(type) && type == "copy")
            {
                model.Id = 0;
            }
            return View(model);
        }

        /// <summary>
        /// 编辑积分兑换请求方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ExchangeCenterConfig model)
        {
            //Version sVersion = new Version();
            //if (Version.TryParse(model.StartVersion, out sVersion))
            //{
            //    model.StartVersion = sVersion.ToString();
            //}
            //else
            //{
            //    return Json(false);
            //}
            //Version eVersion = new Version();
            //if (Version.TryParse(model.EndVersion, out eVersion))
            //{
            //    model.EndVersion = eVersion.ToString();
            //}
            //else
            //{
            //    return Json(false);
            //}
            #region  向后兼容处理
            switch (model.PostionCode.Trim())
            {
                //换好物
                case "2":
                    model.Postion = "会员商城";
                    model.ExchangeCenterType = false;
                    break;
                //拼手气
                case "3":
                    model.Postion = "会员商城";
                    model.ExchangeCenterType = true;
                    break;
                //兑神券
                case "4":
                    model.Postion = "精品通用券";
                    model.ExchangeCenterType = false;
                    break;
            }
            #endregion

            var result = 0;
            if (model.Id > 0)
            {
                result = ExchangeCenterConfigManager.UpdateExchangeCenterConfig(model);
            }
            else
            {
                result = ExchangeCenterConfigManager.InsertExchangeCenterConfig(model);
            }
            #region 记录操作日志
            var oprLog = new DataAccess.Entity.CommonConfigLogModel
            {
                ObjectId = (model.Id > 0 ? model.Id : result).ToString(), //新增时候存储新增的Id值
                ObjectType = "ExchangeCenterConfig",
                Creator = HttpContext.User.Identity.Name,
                Remark = (model.Id > 0 ? "更新" : "新增") + "积分兑换配置",
            };
            new Tuhu.Provisioning.Business.ConfigLog.CommonConfigLogManager().AddCommonConfigLogInfo(oprLog);
            #endregion
            return Json(result > 0);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var result = ExchangeCenterConfigManager.DeleteExchangeCenterConfig(id);

            #region 记录操作日志
            var oprLog = new DataAccess.Entity.CommonConfigLogModel
            {
                ObjectId = id.ToString(), 
                ObjectType = "ExchangeCenterConfig",
                Creator = HttpContext.User.Identity.Name,
                Remark = "删除积分兑换配置" + (result ? "成功" : "失败")
            };
            new Tuhu.Provisioning.Business.ConfigLog.CommonConfigLogManager().AddCommonConfigLogInfo(oprLog);
            #endregion

            return Json(result);
        }

        /// <summary>
        /// 获取操作页面视图
        /// </summary>
        /// <returns></returns>
        public ActionResult OperatorHisitoryLog(int id)
        {
            var pagination = new Pagination() {
                page=1,
                records=20,
                rows=20,
            };
            var result = new Business.ConfigLog.CommonConfigLogManager().GetCommonConfigLogList(
                pagination,id.ToString(), "ExchangeCenterConfig");
            return View(result);
        }
       
    }
}

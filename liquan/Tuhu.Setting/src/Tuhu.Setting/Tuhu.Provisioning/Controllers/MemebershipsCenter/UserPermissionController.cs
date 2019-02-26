using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.UserPermission;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Collections.Generic;
using Tuhu.Service.Member.Request;

using System.Data;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Business.ServiceProxy;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Controllers
{
    public class UserPermissionController : Controller
    {
        #region 会员特权

        public ActionResult Index()
        {
            var memberBll = new MemberService();
            var memberGradeList = memberBll.GetMembershipsGradeList();
            ViewBag.MemberGrade = memberGradeList;
            return View();
        }

        /// <summary>
        /// 会员权益列表数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetUserPermissionByPage(UserPermissionRequest request)
        {
            var returnValue = new List<UserPermissionViewModel>();
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
            var requestModel = new UserPermissionModel {
                Name = request.PermissionName,
               MembershipsGradeId=request.MembershipsGradeId
            };
            var count = DALUserPermission.QueryPageCount(requestModel);
            var dataResult = DALUserPermission.QueryPageList(requestModel, request.Page, request.Limit);
            var memberGradeList = await memberBll.GetMembershipsGradeListAsync();
            
            if (memberGradeList != null && memberGradeList.Count>0 && dataResult!=null && dataResult.Count>0)
            {
                foreach (var userPermission in dataResult)
                {
                    var viewModel = ObjectMapper.ConvertTo<UserPermissionModel,UserPermissionViewModel>(userPermission);
                    var gradeModel = memberGradeList.Find(t => t.PKID == userPermission.MembershipsGradeId);
                    viewModel.LastUpdateDateTime = userPermission.LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    viewModel.CreateDatetime = userPermission.CreateDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                    if (gradeModel != null)
                    {
                        viewModel.MembershipsGradeName = gradeModel.GradeName;
                    }
                    returnValue.Add(viewModel);
                }
            }
            return Json(new { code = 0, msg = "加载成功", count = count, data = returnValue.OrderByDescending(t=>t.Id) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除会员权益
        /// </summary>
        /// <param name="id">权益Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteById(string ids)
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
                    result += DALUserPermission.Delete(id);
                    var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = id,
                        ObjectType = "UserPermission",
                        Author = HttpContext.User.Identity.Name,
                        Operation = "删除UserPermission配置"
                    };
                    operLogManager.AddOprLogAsync(oprLog);
                }
                return Json(new { data = result, msg = "删除成功" });
            }
            catch (Exception e)
            {
                return Json(new { data = -1, msg ="删除异常："+ e.ToString() });
            }
        }

        /// <summary>
        /// 新增编辑页面数据加载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id,int memberGradeId=0)
        {
            var memberBll = new MemberService();
            var memberGradeList =  memberBll.GetMembershipsGradeList();
            ViewBag.MemberGrade = memberGradeList;
            ViewBag.CheckCycle = CheckCycleDllList();
            ViewBag.PermissionType = PermissionTypeDllList();
            if (id <= 0)
            {
                ViewBag.Title = "添加用户权益信息";
                return View(new UserPermissionModel() { MembershipsGradeId=memberGradeId});
            }
            else
            {
                ViewBag.Title = "编辑用户权益信息";
                var model = DALUserPermission.GetModelById(id);
                if (model == null)
                {
                    model = new UserPermissionModel();
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
        public JsonResult Edit(UserPermissionModel model)
        {
            if (model == null)
            {
                return Json(new {result=0,msg="无法获取数据对象" });
            }
            var result = 0;
            if (string.IsNullOrWhiteSpace(model.EndVersion))
            {
                model.EndVersion="9.9.9";
            }
            model.LastUpdateBy = HttpContext.User.Identity.Name;
            if (model.Id <= 0)
            {
                model.CreateBy = model.LastUpdateBy;
                result = DALUserPermission.Add(model);
            }
            else
            {
                model.LastUpdateDateTime = DateTime.Now;
                result = DALUserPermission.Update(model);
            }
            var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog
            {
                ObjectID = model.Id,
                ObjectType = "UserPermission",
                Author = HttpContext.User.Identity.Name,
                Operation = model.Id > 0 ? "更新" : "新增" + "UserLivingRights配置"
            };
            var operLogManager = new Business.OprLogManagement.OprLogManager();
            return Json(new { result = result, msg = "操作成功" });
        }

        /// <summary>
        ///获取权益类型下拉列表
        /// </summary>
        /// <returns></returns>
        public List<DropDownListViewModel> PermissionTypeDllList()
        {
            var list = new List<DropDownListViewModel>();
            list.Add(new DropDownListViewModel() { Id="0",Name="会员商城"});
            list.Add(new DropDownListViewModel() { Id = "1", Name = "月度福利" });
            list.Add(new DropDownListViewModel() { Id = "2", Name = "升级福利" });
            list.Add(new DropDownListViewModel() { Id = "5", Name = "生日特权" });
            list.Add(new DropDownListViewModel() { Id = "6", Name = "更多福利" });
            return list;
        }

        /// <summary>
        ///获取计算周期数据
        /// </summary>
        /// <returns></returns>
        public List<DropDownListViewModel> CheckCycleDllList()
        {
            var list = new List<DropDownListViewModel>();
            list.Add(new DropDownListViewModel() { Id = "", Name = "只能领取一次" });
            list.Add(new DropDownListViewModel() { Id = "1_d", Name = "每【天】可以领取一次" });
            list.Add(new DropDownListViewModel() { Id = "1_M", Name = "每【月】可领取一次" });
            list.Add(new DropDownListViewModel() { Id = "1_NM", Name = "每【自然月】可领取一次" });
            list.Add(new DropDownListViewModel() { Id = "1_Y", Name = "每【年】可领取一次" });
            return list;
        }
       
        #endregion


        #region 特价商品
        public ActionResult SaleProductList(string activityID = "8312FE9D-6DCE-4CD5-87D5-99CE064336A3")
        {
            if (activityID != "")
            {
                IEnumerable<UserPermissionActivityProduct> list = UserPermissionManager.GetActivityProductList(activityID);
                return View(list);
            }
            else
            {
                return View();
            }

        }

        public ActionResult SaleProductEdit(string id = "")
        {
            if (id == "")
            {
                return View(new UserPermissionActivityProduct());
            }
            else
            {
                UserPermissionActivityProduct model = UserPermissionManager.GetActivityProduct(id);
                if (model == null)
                    return View(new UserPermissionActivityProduct());
                return View(model);
            }

        }

        public ActionResult AddActivityProduct(UserPermissionActivityProduct model)
        {
            if (model.PID == "")
            {
                return JavaScript("{\"status\":0}");
            }
            else
            {
                if (UserPermissionManager.AddByActivityProduct(model))
                {
                    return JavaScript("{\"status\":1}");
                }
                else
                {
                    return JavaScript("{\"status\":0}");
                }
            }
        }

        public ActionResult DeleteActivityProduct(string activityID, string pkid)
        {
            if (activityID == "" || pkid == "")
                return JavaScript("{\"status\":0}");
            else
            {
                if (UserPermissionManager.DeleteByActivityProduct(activityID, pkid))
                    return JavaScript("{\"status\":1}");
                else
                    return JavaScript("{\"status\":0}");
            }
        }


        #endregion


        #region 运费折扣
        public ActionResult Freight()
        {
            List<tbl_UserTransportation> list = UserPermissionManager.GetUseTransMoney();

            return View(list);
        }

        public ActionResult SaveTrans(string list)
        {
            if (string.IsNullOrEmpty(list))
            {
                return JavaScript("{\"status\":0}");
            }
            List<tbl_UserTransportation> models = JsonConvert.DeserializeObject<List<tbl_UserTransportation>>(list);
            if (UserPermissionManager.SaveTransMoney(models))
            {
                return JavaScript("{\"status\":1}");
            }
            else
            {
                return JavaScript("{\"status\":0}");
            }
        }


        #endregion

        #region 升级任务
        public ActionResult TaskList(string appType = "1")
        {

            return View(UserPermissionManager.GetTaskList(appType));
        }

        public ActionResult TaskEdit(string appType = "1", string id = "0")
        {
            if (id == "0")
            {
                return View(new tbl_UserTask() { APPType = Convert.ToInt32(appType) });
            }
            else
            {
                tbl_UserTask model = UserPermissionManager.GetTask(id);
                return View(model);
            }
        }


        public ActionResult SaveTask(tbl_UserTask model)
        {
            if (UserPermissionManager.EditTask(model))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }

        public ActionResult DeleteTask(string id)
        {
            if (UserPermissionManager.DeleteTask(id))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }

        #endregion


        #region  会员优惠券
        public ActionResult PromotionList(string userRank = "LV1")
        {

            return View(UserPermissionManager.GetPromotionList(userRank));
        }

        public ActionResult PromotionEdit(string id = "0", string userRank = "LV1")
        {
            if (id == "0")
                return View(new tbl_UserPromotionCode() { UserRank = userRank });
            else
            {
                tbl_UserPromotionCode model = UserPermissionManager.GetPromotion(id);
                return View(model);
            }
        }


        public ActionResult SavePromotion(tbl_UserPromotionCode model)
        {
            if (UserPermissionManager.EditPromotion(model))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }

        public ActionResult DeletePromotion(string id)
        {
            if (UserPermissionManager.DeletePromotion(id))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }
        #endregion


        public ActionResult Grade()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ImportGrade()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (!file.FileName.Contains(".xlsx") || !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("会员等级导入", true);
                    List<VipMobileRequest> list = new List<VipMobileRequest>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new VipMobileRequest()
                        {
                            Mobile = dr["Mobile"]?.ToString(),
                            Pkid = dr["Pkid"] != DBNull.Value ? Convert.ToInt32(dr["Pkid"]) : 0,
                        });
                    }

                    IEnumerable<VipMobileRequest> vipList = list as IEnumerable<VipMobileRequest>;

                    if (vipList.Any())
                    {
                        if (vipList.Count() >= 500)
                        {
                            return Json(new { Status = -6, Error = "单次不能超出500条数据操作！" }, "text/html");
                        }
                        using (var client = new Tuhu.Service.Member.UserClient())
                        {
                            var result = client.BatchCreateVipAuthorization(vipList, "Tuhu", "Import");
                            if (result.Success)
                            {
                                return Json(new { Status = 1, Result = result.Result[1] }, "text/html");
                            }
                            else
                            {
                                return Json(new { Status = -5, Error = "服务器错误！" }, "text/html");
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Status = -2, Error = "导入数据为空！" }, "text/html");
                    }
                }
                return Json(new { Status = -3, Error = "未知错误！" }, "text/html");
            }
            catch (Exception em)
            {
                return Json(new { Status = -4, Error = em }, "text/html");
                throw em;
            }
        }
    }
}

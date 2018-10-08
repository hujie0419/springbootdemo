using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Models;
using Tuhu.Provisioning.Areas.ActivityPage.Model;
using Tuhu.Provisioning.Areas.ActivityPage.Models;
using Tuhu.Service;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.ActivityPage;
using Tuhu.Service.ActivityPage.Request.SetRequest;
using Tuhu.Service.ActivityPage.Response.SetResponse;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Areas.ActivityPage.Controllers
{
    public class ActivityPageController : Controller
    {
        private readonly string imgUrl = "https://img1.tuhu.org/";

        private static readonly ILog logger = LoggerFactory.GetLogger("ActivityPageController");

        #region 活动信息

        /// <summary>
        /// 活动配置信息查询
        /// </summary>
        /// <param name="activityInfoSettingRequest">请求实体</param>
        /// <returns></returns>
        public async Task<JsonResult> GetActivityInfo(GetActivityInfoSettingRequest activityInfoSettingRequest)
        {
            var activityInfoSettingResponse = new GetActivityInfoSettingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var activityInfoResult = await activityPageClient.GetActivityInfoAsync(activityInfoSettingRequest);

                    if (!activityInfoResult.Success)
                    {
                        activityInfoSettingResponse.ResponseCode = "0005";
                        activityInfoSettingResponse.ResponseMessage = activityInfoResult.ErrorMessage;
                    }
                    else
                    {
                        activityInfoSettingResponse = activityInfoResult.Result;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityInfo");
                activityInfoSettingResponse.ResponseCode = "0009";
                activityInfoSettingResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = activityInfoSettingResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 活动基本信息添加，更新
        /// </summary>
        /// <param name="activityInfoRequest">请求实体</param>
        /// <returns></returns>

        public async Task<JsonResult> SaveActivityInfo(ActivityInfoRequest activityInfoRequest)
        {
            activityInfoRequest.CurrentUser = User.Identity.Name;
            var activityInfoResponse = new SaveActivityInfoResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var activityInfoResult = await activityPageClient.SaveActivityInfoAsync(activityInfoRequest);

                    if (!activityInfoResult.Success)
                    {
                        activityInfoResponse.ResponseCode = "0005";
                        activityInfoResponse.ResponseMessage = activityInfoResult.ErrorMessage;
                    }
                    else
                    {
                        activityInfoResponse = activityInfoResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveActivityInfo");
                activityInfoResponse.ResponseCode = "0009";
                activityInfoResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = activityInfoResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 活动页面基本配置信息添加、更新
        /// </summary>
        /// <param name="activityConfigRequest">请求实体</param>
        /// <returns></returns>

        public async Task<JsonResult> SaveActivityConfig(ActivityConfigRequest activityConfigRequest)
        {
            activityConfigRequest.CurrentUser = User.Identity.Name;
            var activityConfigResponse = new SaveActivityConfigResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveActivityConfigAsync(activityConfigRequest);

                    if (!result.Success)
                    {
                        activityConfigResponse.ResponseCode = "0005";
                        activityConfigResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        activityConfigResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveActivityConfig");
                activityConfigResponse.ResponseCode = "0009";
                activityConfigResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = activityConfigResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  活动展示要求配置信息新增、更新
        /// </summary>
        /// <param name="activityDispalyRequest">请求实体</param>
        /// <returns></returns>

        public async Task<JsonResult> SaveActivityDispaly(ActivityDispalyRequest activityDispalyRequest)
        {
            activityDispalyRequest.CurrentUser = User.Identity.Name;
            var activityDispalyResponse = new SaveActivityDispalyResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveActivityDispalyAsync(activityDispalyRequest);

                    if (!result.Success)
                    {
                        activityDispalyResponse.ResponseCode = "0005";
                        activityDispalyResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        activityDispalyResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveActivityDispaly");
                activityDispalyResponse.ResponseCode = "0009";
                activityDispalyResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = activityDispalyResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取活动基本信息
        /// </summary>
        /// <param name="getActivityListRequest"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetActivityList(GetActivityListRequest getActivityListRequest)
        {
            var getActivityListResponse = new GetActivityListResponse();
            if (getActivityListRequest.PageIndex == 0)
            {
                getActivityListRequest.PageIndex = 1;
            }
            if (getActivityListRequest.PageSize == 0)
            {
                getActivityListRequest.PageSize = 20;
            }

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var activityInfoResult = await activityPageClient.GetActivityListAsync(getActivityListRequest);

                    if (!activityInfoResult.Success)
                    {
                        getActivityListResponse.ResponseCode = "0005";
                        getActivityListResponse.ResponseMessage = activityInfoResult.ErrorMessage;
                    }
                    else
                    {
                        getActivityListResponse = activityInfoResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityInfo");
                getActivityListResponse.ResponseCode = "0009";
                getActivityListResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getActivityListResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 活动复制接口
        /// </summary>
        /// <param name="addMinuteFigureRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> ActivityCopy(ActivityCopyRequest activityCopyRequest)
        {
            activityCopyRequest.CurrentUser = User.Identity.Name;
            var activityCopyResponse = new ActivityCopyResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.ActivityCopyAsync(activityCopyRequest);

                    if (!result.Success)
                    {
                        activityCopyResponse.ResponseCode = "0005";
                        activityCopyResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        activityCopyResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ActivityCopy");
                activityCopyResponse.ResponseCode = "0009";
                activityCopyResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = activityCopyResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 活动删除接口
        /// </summary>
        /// <param name="deleteActivityRequest"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteActivity(DeleteActivityRequest deleteActivityRequest)
        {
            deleteActivityRequest.CurrentUser = User.Identity.Name;
            var deleteActivityResponse = new DeleteActivityResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.DeleteActivityAsync(deleteActivityRequest);

                    if (!result.Success)
                    {
                        deleteActivityResponse.ResponseCode = "0005";
                        deleteActivityResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        deleteActivityResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteActivity");
                deleteActivityResponse.ResponseCode = "0009";
                deleteActivityResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = deleteActivityResponse }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 悬浮窗保存
        /// </summary>
        /// <param name="floatingInfoRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> EditFloatingInfo(FloatingInfoRequest floatingInfoRequest)
        {
            floatingInfoRequest.CurrentUser = User.Identity.Name;
            var saveFloatingInfoResponse = new SaveFloatingInfoResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveFloatingInfoAsync(floatingInfoRequest);

                    if (!result.Success)
                    {
                        saveFloatingInfoResponse.ResponseCode = "0005";
                        saveFloatingInfoResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveFloatingInfoResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveFloatingInfo");
                saveFloatingInfoResponse.ResponseCode = "0009";
                saveFloatingInfoResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveFloatingInfoResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 导航信息保存
        /// </summary>
        /// <param name="editNavigationInfoRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> EditNavigationInfo(EditNavigationInfoRequest editNavigationInfoRequest)
        {
            editNavigationInfoRequest.CurrentUser = User.Identity.Name;
            var editNavigationInfoResponse = new EditNavigationInfoResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.EditNavigationInfoAsync(editNavigationInfoRequest);

                    if (!result.Success)
                    {
                        editNavigationInfoResponse.ResponseCode = "0005";
                        editNavigationInfoResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        editNavigationInfoResponse = result.Result;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditNavigationInfo");
                editNavigationInfoResponse.ResponseCode = "0009";
                editNavigationInfoResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = editNavigationInfoResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 导航设置保存
        /// </summary>
        /// <param name="navigationSettingRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> EditNavigationSetting(NavigationSettingRequest navigationSettingRequest)
        {
            navigationSettingRequest.CurrentUser = User.Identity.Name;
            var saveNavigationSettingResponse = new SaveNavigationSettingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.EditNavigationSettingAsync(navigationSettingRequest);

                    if (!result.Success)
                    {
                        saveNavigationSettingResponse.ResponseCode = "0005";
                        saveNavigationSettingResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveNavigationSettingResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditNavigationSetting");
                saveNavigationSettingResponse.ResponseCode = "0009";
                saveNavigationSettingResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveNavigationSettingResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 顶部适配栏编辑
        /// </summary>
        /// <param name="adaptationColumnInfoRequest"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveAdaptationColumnInfo(AdaptationColumnInfoRequest adaptationColumnInfoRequest)
        {
            adaptationColumnInfoRequest.CurrentUser = User.Identity.Name;
            var adaptationColumnInfoResponse = new SaveAdaptationColumnResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveAdaptationColumnInfoAsync(adaptationColumnInfoRequest);

                    if (!result.Success)
                    {
                        adaptationColumnInfoResponse.ResponseCode = "0005";
                        adaptationColumnInfoResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        adaptationColumnInfoResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditAdaptationColumnInfo");
                adaptationColumnInfoResponse.ResponseCode = "0009";
                adaptationColumnInfoResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = adaptationColumnInfoResponse }, JsonRequestBehavior.AllowGet);

        }

        #endregion


        #region 模块信息

        /// <summary>
        /// 基本模块查询
        /// </summary>
        /// <param name="getActivityModuleBasisRequest">请求实体</param>
        /// <returns></returns>
        public async Task<JsonResult> GetActivityModuleBasis(GetActivityModuleBasisRequest getActivityModuleBasisRequest)
        {
            var activityModuleBasisResponse = new GetActivityModuleBasisResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetActivityModuleBasisAsync(getActivityModuleBasisRequest);

                    if (!result.Success)
                    {
                        activityModuleBasisResponse.ResponseCode = "0005";
                        activityModuleBasisResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        activityModuleBasisResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityModuleBasis");
                activityModuleBasisResponse.ResponseCode = "0009";
                activityModuleBasisResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = activityModuleBasisResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 添加模块基础信息
        /// </summary>
        /// <param name="addModuleInfoRequest"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddModuleInfo(AddModuleInfoRequest addModuleInfoRequest)
        {
            addModuleInfoRequest.CurrentUser = User.Identity.Name;
            var addModuleInfoResponse = new AddModuleInfoResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.AddModuleInfoAsync(addModuleInfoRequest);

                    if (!result.Success)
                    {
                        addModuleInfoResponse.ResponseCode = "0005";
                        addModuleInfoResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        addModuleInfoResponse = result.Result;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "AddModuleInfo");
                addModuleInfoResponse.ResponseCode = "0009";
                addModuleInfoResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = addModuleInfoResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 模块排序
        /// </summary>
        /// <param name="editModuleSortRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> EditModuleSort(EditModuleSortRequest editModuleSortRequest)
        {
            editModuleSortRequest.CurrentUser = User.Identity.Name;
            var editModuleSortResponse = new EditModuleSortResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.EditModuleSortAsync(editModuleSortRequest);

                    if (!result.Success)
                    {
                        editModuleSortResponse.ResponseCode = "0005";
                        editModuleSortResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        editModuleSortResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditModuleSort");
                editModuleSortResponse.ResponseCode = "0009";
                editModuleSortResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = editModuleSortResponse }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 模块删除接口
        /// </summary>
        /// <param name="editModuleSortRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> DeleteActivityModule(DeleteActivityModuleRequest deleteActivityModuleRequest)
        {
            deleteActivityModuleRequest.CurrentUser = User.Identity.Name;
            var deleteActivityModuleResponse = new DeleteActivityModuleResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.DeleteActivityModuleAsync(deleteActivityModuleRequest);

                    if (!result.Success)
                    {
                        deleteActivityModuleResponse.ResponseCode = "0005";
                        deleteActivityModuleResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        deleteActivityModuleResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteActivityModule");
                deleteActivityModuleResponse.ResponseCode = "0009";
                deleteActivityModuleResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = deleteActivityModuleResponse }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 头图模块

        /// <summary>
        /// 新增分车型头图
        /// </summary>
        /// <param name="addMinuteFigureRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> AddMinuteFigure(AddMinuteFigureRequest addMinuteFigureRequest)
        {
            addMinuteFigureRequest.CurrentUser = User.Identity.Name;
            var minuteFigureResponse = new AddMinuteFigureResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.AddMinuteFigureAsync(addMinuteFigureRequest);

                    if (!result.Success)
                    {
                        minuteFigureResponse.ResponseCode = "0005";
                        minuteFigureResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        minuteFigureResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "AddMinuteFigure");
                minuteFigureResponse.ResponseCode = "0009";
                minuteFigureResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = minuteFigureResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 分车型头图删除
        /// </summary>
        /// <param name="deleteMinuteFigureRequest"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteMinuteFigure(DeleteMinuteFigureRequest deleteMinuteFigureRequest)
        {
            deleteMinuteFigureRequest.CurrentUser = User.Identity.Name;
            var deleteMinuteFigureResponse = new DeleteMinuteFigureResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.DeleteMinuteFigureAsync(deleteMinuteFigureRequest);


                    if (!result.Success)
                    {
                        deleteMinuteFigureResponse.ResponseCode = "0005";
                        deleteMinuteFigureResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        deleteMinuteFigureResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteMinuteFigure");
                deleteMinuteFigureResponse.ResponseCode = "0009";
                deleteMinuteFigureResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = deleteMinuteFigureResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存通用头图
        /// </summary>
        /// <param name="saveGeneralFigureRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> SaveGeneralFigure(SaveGeneralFigureRequest saveGeneralFigureRequest)
        {
            saveGeneralFigureRequest.CurrentUser = User.Identity.Name;
            var saveGeneralFigureResponse = new SaveGeneralFigureResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveGeneralFigureAsync(saveGeneralFigureRequest);

                    if (!result.Success)
                    {
                        saveGeneralFigureResponse.ResponseCode = "0005";
                        saveGeneralFigureResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveGeneralFigureResponse = result.Result;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveGeneralFigure");
                saveGeneralFigureResponse.ResponseCode = "0009";
                saveGeneralFigureResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveGeneralFigureResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分车型头图默认保存接口
        /// </summary>
        /// <param name="saveDefaultMinuteFigureRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> SaveDefaultMinuteFigure(SaveDefaultMinuteFigureRequest saveDefaultMinuteFigureRequest)
        {
            saveDefaultMinuteFigureRequest.CurrentUser = User.Identity.Name;
            var saveDefaultMinuteFigureResponse = new SaveDefaultMinuteFigureResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveDefaultMinuteFigureAsync(saveDefaultMinuteFigureRequest);

                    if (!result.Success)
                    {
                        saveDefaultMinuteFigureResponse.ResponseCode = "0005";
                        saveDefaultMinuteFigureResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveDefaultMinuteFigureResponse = result.Result;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveDefaultMinuteFigure");
                saveDefaultMinuteFigureResponse.ResponseCode = "0009";
                saveDefaultMinuteFigureResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveDefaultMinuteFigureResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分车型头图编辑
        /// </summary>
        /// <param name="editMinuteFigureRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> EditMinuteFigure(EditMinuteFigureRequest editMinuteFigureRequest)
        {
            editMinuteFigureRequest.CurrentUser = User.Identity.Name;
            var editMinuteFigureResponse = new EditMinuteFigureResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.EditMinuteFigureAsync(editMinuteFigureRequest);

                    if (!result.Success)
                    {
                        editMinuteFigureResponse.ResponseCode = "0005";
                        editMinuteFigureResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        editMinuteFigureResponse = result.Result;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditMinuteFigure");
                editMinuteFigureResponse.ResponseCode = "0009";
                editMinuteFigureResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = editMinuteFigureResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分车型头图查询
        /// </summary>
        /// <param name="getMinuteFigureListRequest"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetMinuteFigureList(GetMinuteFigureListRequest getMinuteFigureListRequest)
        {
            var getMinuteFigureListResponse = new GetMinuteFigureListResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetMinuteFigureListAsync(getMinuteFigureListRequest);

                    if (!result.Success)
                    {
                        getMinuteFigureListResponse.ResponseCode = "0005";
                        getMinuteFigureListResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getMinuteFigureListResponse = result.Result;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetMinuteFigureList");
                getMinuteFigureListResponse.ResponseCode = "00095";
                getMinuteFigureListResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getMinuteFigureListResponse }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 图片链接

        /// <summary>
        /// 图片/连接1-4列保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<JsonResult> SaveImgageLinkColumns(SaveImgageLinkColumnsRequest request)
        {
            request.CurrentUser = User.Identity.Name;
            var response = new SaveImgageLinkColumnsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveImgageLinkColumnsAsync(request);

                    if (!result.Success)
                    {
                        response.ResponseCode = "0005";
                        response.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        response = result.Result;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveImgageLinkColumns");
                response.ResponseCode = "0009";
                response.ResponseMessage = "程序异常";
            }
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 图片/连接1-4列查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetImgageLinkColumns(GetImgageLinkColumnsRequest request)
        {
            var response = new GetImgageLinkColumnsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetImgageLinkColumnsAsync(request);

                    if (!result.Success)
                    {
                        response.ResponseCode = "0005";
                        response.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        response = result.Result;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetImgageLinkColumns");
                response.ResponseCode = "0009";
                response.ResponseMessage = "程序异常";
            }
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 图片/连接 1图3产品保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<JsonResult> SaveImgageLinkProducts(SaveImgageLinkProductsReqeust request)
        {
            request.CurrentUser = User.Identity.Name;
            var response = new SaveImgageLinkProductsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveImgageLinkProductsAsync(request);


                    if (!result.Success)
                    {
                        response.ResponseCode = "0005";
                        response.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        response = result.Result;

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveImgageLinkProducts");
                response.ResponseCode = "0009";
                response.ResponseMessage = "程序异常";
            }
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 图片/连接 1图3产品查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetImgageLinkProducts(GetImgageLinkProductsReqeust request)
        {
            var response = new GetImgageLinkProductsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetImgageLinkProductsAsync(request);

                    if (!result.Success)
                    {
                        response.ResponseCode = "0005";
                        response.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        response = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetImgageLinkProducts");
                response.ResponseCode = "0009";
                response.ResponseMessage = "程序异常";
            }
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 商品模块

        /// <summary>
        /// 车型品牌查询
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetAllVehicles()
        {
            var allVehiclesResponse = new GetAllVehiclesResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetAllVehiclesAsync();

                    if (!result.Success)
                    {
                        allVehiclesResponse.ResponseCode = "0005";
                        allVehiclesResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        allVehiclesResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetAllVehicles");
                allVehiclesResponse.ResponseCode = "0009";
                allVehiclesResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = allVehiclesResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///  普通商品数据源查询接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetGeneralProducts(GetGeneralProductsRequest getGeneralProductsRequest)
        {
            var getGeneralProductsResponse = new GetGeneralProductsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetGeneralProductsAsync(getGeneralProductsRequest);

                    if (!result.Success)
                    {
                        getGeneralProductsResponse.ResponseCode = "0005";
                        getGeneralProductsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getGeneralProductsResponse = result.Result;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetGeneralProducts");
                getGeneralProductsResponse.ResponseCode = "0009";
                getGeneralProductsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getGeneralProductsResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///  已关联商品数据查询接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetGeneralProductAssociations(GetGeneralProductAssReqeust getGeneralProductAssReqeust)
        {
            var getGeneralProductAssResponse = new GetGeneralProductAssResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetGeneralProductAssociationsAsync(getGeneralProductAssReqeust);

                    if (!result.Success)
                    {
                        getGeneralProductAssResponse.ResponseCode = "0005";
                        getGeneralProductAssResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getGeneralProductAssResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetGeneralProductAssociations");
                getGeneralProductAssResponse.ResponseCode = "0009";
                getGeneralProductAssResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getGeneralProductAssResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///  关联商品保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveGeneralProductAssociations(SaveGeneralProductAssRequest saveGeneralProductAssRequest)
        {
            saveGeneralProductAssRequest.CurrentUser = User.Identity.Name;
            var saveGeneralProductAssResponse = new SaveGeneralProductAssResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveGeneralProductAssociationsAsync(saveGeneralProductAssRequest);

                    if (!result.Success)
                    {
                        saveGeneralProductAssResponse.ResponseCode = "0005";
                        saveGeneralProductAssResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveGeneralProductAssResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveGeneralProductAssociations");
                saveGeneralProductAssResponse.ResponseCode = "0009";
                saveGeneralProductAssResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveGeneralProductAssResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  编辑已关联商品组号接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> EditGeneralProductSortGroupId(EditGeneralProductSortRequest editGeneralProductSortRequest)
        {
            editGeneralProductSortRequest.CurrentUser = User.Identity.Name;
            var editGeneralProductSortResponse = new EditGeneralProductSortResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.EditGeneralProductSortGroupIdAsync(editGeneralProductSortRequest);

                    if (!result.Success)
                    {
                        editGeneralProductSortResponse.ResponseCode = "0005";
                        editGeneralProductSortResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        editGeneralProductSortResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditGeneralProductSortGroupId");
                editGeneralProductSortResponse.ResponseCode = "0009";
                editGeneralProductSortResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = editGeneralProductSortResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 普通商品配置保存
        /// </summary>
        /// <param name="saveGeneralProductSettingRequest"></param>
        /// <returns></returns>

        public async Task<JsonResult> SaveGeneralProductSetting(SaveGeneralProductSettingRequest saveGeneralProductSettingRequest)
        {
            saveGeneralProductSettingRequest.CurrentUser = User.Identity.Name;
            var saveGeneralProductSettingResponse = new SaveGeneralProductSettingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveGeneralProductSettingAsync(saveGeneralProductSettingRequest);

                    if (!result.Success)
                    {
                        saveGeneralProductSettingResponse.ResponseCode = "0005";
                        saveGeneralProductSettingResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveGeneralProductSettingResponse = result.Result;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveGeneralProductSetting");
                saveGeneralProductSettingResponse.ResponseCode = "0009";
                saveGeneralProductSettingResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveGeneralProductSettingResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///字典查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetDictList(GetDictListReqeust request)
        {
            var response = new GetDictListResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetDictListAsync(request);

                    if (!result.Success)
                    {
                        response.ResponseCode = "0005";
                        response.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        response = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetDictList");

                response.ResponseCode = "0009";
                response.ResponseMessage = "程序异常";
            }
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///系统商品保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<JsonResult> SavePushProductSetting(SavePushProductSettingReqeust request)
        {
            request.CurrentUser=User.Identity.Name;
            var response = new SavePushProductSettingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SavePushProductSettingAsync(request);

                    if (!result.Success)
                    {
                        response.ResponseCode = "0005";
                        response.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        response = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SavePushProductSetting");
                response.ResponseCode = "0009";
                response.ResponseMessage = "程序异常";
            }
            return Json(new { data = response }, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> FileUpload(UploadRequest request)
        {
            var fileContent = default(byte[]);
            var fileUploadResponse = new Model.FileUploadResponse();
            if (this.HttpContext.Request.Files.Count > 0)
            {
                using (var fs = this.HttpContext.Request.Files[0].InputStream)
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        ms.Flush();
                        fileContent = ms.ToArray();
                    }
                }
                using (FileUploadClient fileUploadClient = new FileUploadClient())
                {
                    if (request.FileType == FileType.Image.ToString())
                    {
                        if (request.Extension == ".zip")
                        {
                            var fileUploadRequest = new FileUploadRequest
                            {
                                Contents = fileContent,
                                Extension = request.Extension
                            };
                            var result = await fileUploadClient.UploadFileAsync(fileUploadRequest);
                            fileUploadResponse.ResponseLinkUrl = result.Success ? imgUrl + result.Result : "";
                        }
                        else
                        {
                            ImageUploadRequest imageUploadRequest = new ImageUploadRequest
                            {
                                Contents = fileContent
                            };
                            var result = await fileUploadClient.UploadImageAsync(imageUploadRequest);
                            fileUploadResponse.ResponseLinkUrl = result.Success ? imgUrl + result.Result : "";
                        }


                    }

                    if (request.FileType == FileType.Video.ToString())
                    {
                        FileUploadRequest fileUploadRequest = new FileUploadRequest
                        {
                            Contents = fileContent,
                            Extension = request.Extension
                        };
                        var result = await fileUploadClient.UploadVideoAsync(fileUploadRequest);
                        fileUploadResponse.ResponseLinkUrl = result.Success ? imgUrl + result.Result?.Raw : "";
                    }

                    if (string.IsNullOrEmpty(fileUploadResponse.ResponseLinkUrl))
                    {
                        fileUploadResponse.ResponseMessage = "上传失败";
                        fileUploadResponse.ResponseCode = "0001";
                    }
                    else
                    {
                        fileUploadResponse.ResponseMessage = "上传成功";
                        fileUploadResponse.ResponseCode = "0000";
                    }
                }
            }
            return Json(new { data = fileUploadResponse }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        ///  秒杀商品活动场次查询接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSpikeSessions(GetSpikeSessionsRequest getSpikeSessionsRequest)
        {
            var getSpikeSessionsResponse = new GetSpikeSessionsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetSpikeSessionsAsync(getSpikeSessionsRequest);

                    if (!result.Success)
                    {
                        getSpikeSessionsResponse.ResponseCode = "0005";
                        getSpikeSessionsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getSpikeSessionsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetSpikeSessions");
                getSpikeSessionsResponse.ResponseCode = "0009";
                getSpikeSessionsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getSpikeSessionsResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  保存秒杀场次信息接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveSpikeSessions(SaveSpikeSessionsRequest saveSpikeSessionsRequest)
        {
            saveSpikeSessionsRequest.CurrentUser = User.Identity.Name;
            var saveSpikeSessionsResponse = new SaveSpikeSessionsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveSpikeSessionsAsync(saveSpikeSessionsRequest);

                    if (!result.Success)
                    {
                        saveSpikeSessionsResponse.ResponseCode = "0005";
                        saveSpikeSessionsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveSpikeSessionsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveSpikeSessions");
                saveSpikeSessionsResponse.ResponseCode = "0009";
                saveSpikeSessionsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveSpikeSessionsResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  拼团商品查询接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetGroupProducts(GetGroupProductsRequest getGroupProductsRequest)
        {
            var getGroupProductsResponse = new GetGroupProductsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetGroupProductsAsync(getGroupProductsRequest);

                    if (!result.Success)
                    {
                        getGroupProductsResponse.ResponseCode = "0005";
                        getGroupProductsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getGroupProductsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetGroupProducts");

                getGroupProductsResponse.ResponseCode = "0009";
                getGroupProductsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getGroupProductsResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  拼团保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveGroupProducts(SaveGroupProductsRequest saveGroupProductsRequest)
        {
            saveGroupProductsRequest.CurrentUser = User.Identity.Name;
            var saveGroupProductsResponse = new SaveGroupProductsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveGroupProductsAsync(saveGroupProductsRequest);

                    if (!result.Success)
                    {
                        saveGroupProductsResponse.ResponseCode = "0005";
                        saveGroupProductsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveGroupProductsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveGroupProducts");
                saveGroupProductsResponse.ResponseCode = "0009";
                saveGroupProductsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveGroupProductsResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  根据商品类目查询品牌集合接口 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetProductBrandsByCategory(GetProductBrandsByCategoryRequest getProductBrandsByCategoryRequest)
        {
            var getProductBrandsByCategoryResponse = new GetProductBrandsByCategoryResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetProductBrandsByCategoryAsync(getProductBrandsByCategoryRequest);

                    if (!result.Success)
                    {
                        getProductBrandsByCategoryResponse.ResponseCode = "0005";
                        getProductBrandsByCategoryResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getProductBrandsByCategoryResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetProductBrandsByCategory");
                getProductBrandsByCategoryResponse.ResponseCode = "0009";
                getProductBrandsByCategoryResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getProductBrandsByCategoryResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  促销价格查询 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPromotionProductPrice(GetPromotionProductPriceRequest getPromotionProductPriceRequest)
        {
            var getPromotionProductPriceResponse = new GetPromotionProductPriceResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetPromotionProductPriceAsync(getPromotionProductPriceRequest);


                    if (!result.Success)
                    {
                        getPromotionProductPriceResponse.ResponseCode = "0005";
                        getPromotionProductPriceResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getPromotionProductPriceResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetPromotionProductPrice");
                getPromotionProductPriceResponse.ResponseCode = "0009";
                getPromotionProductPriceResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getPromotionProductPriceResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  商品价格查询 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetProductPrice(GetProductPriceRequest getProductPriceRequest)
        {
            var getProductPriceResponse = new GetProductPriceResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetProductPriceAsync(getProductPriceRequest);

                    if (!result.Success)
                    {
                        getProductPriceResponse.ResponseCode = "0005";
                        getProductPriceResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getProductPriceResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetProductPrice");
                getProductPriceResponse.ResponseCode = "0009";
                getProductPriceResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getProductPriceResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  PID 查询拼团信息集合
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetProductGroupInfoByIds(GetProductGroupInfoByIdsRequest getProductGroupInfoByIdsRequest)
        {
            var getProductGroupInfoByIdsResponse = new GetProductGroupInfoByIdsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetProductGroupInfoByIdsAsync(getProductGroupInfoByIdsRequest);

                    if (!result.Success)
                    {
                        getProductGroupInfoByIdsResponse.ResponseCode = "0005";
                        getProductGroupInfoByIdsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getProductGroupInfoByIdsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetProductGroupInfoByIds");
                getProductGroupInfoByIdsResponse.ResponseCode = "0009";
                getProductGroupInfoByIdsResponse.ResponseMessage = "程序异常";

            }
            return Json(new { data = getProductGroupInfoByIdsResponse }, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region 抽奖活动

        /// <summary>
        ///  抽奖活动保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveSweepstakes(SaveSweepstakesRequest saveSweepstakesRequest)
        {
            saveSweepstakesRequest.CurrentUser = User.Identity.Name;
            var saveSweepstakesResponse = new SaveSweepstakesResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveSweepstakesAsync(saveSweepstakesRequest);

                    if (!result.Success)
                    {
                        saveSweepstakesResponse.ResponseCode = "0005";
                        saveSweepstakesResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveSweepstakesResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveSweepstakes");
                saveSweepstakesResponse.ResponseCode = "0009";
                saveSweepstakesResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveSweepstakesResponse }, JsonRequestBehavior.AllowGet);

        }

        #endregion


        #region 优惠券

        /// <summary>
        ///  滑动优惠券保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveSlidingCoupon(SaveSlidingCouponRequest saveSlidingCouponRequest)
        {
            saveSlidingCouponRequest.CurrentUser = User.Identity.Name;
            var saveSlidingCouponResponse = new SaveSlidingCouponResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveSlidingCouponAsync(saveSlidingCouponRequest);

                    if (!result.Success)
                    {
                        saveSlidingCouponResponse.ResponseCode = "0005";
                        saveSlidingCouponResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveSlidingCouponResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveSlidingCoupon");
                saveSlidingCouponResponse.ResponseCode = "0009";
                saveSlidingCouponResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveSlidingCouponResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  滑动优惠券查询接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSlidingCoupon(GetSlidingCouponRequest getSlidingCouponRequest)
        {
            var getSlidingCouponResponse = new GetSlidingCouponResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetSlidingCouponAsync(getSlidingCouponRequest);
                    if (!result.Success)
                    {
                        getSlidingCouponResponse.ResponseCode = "0005";
                        getSlidingCouponResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getSlidingCouponResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetSlidingCoupon");
                getSlidingCouponResponse.ResponseCode = "0009";
                getSlidingCouponResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getSlidingCouponResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  优惠券ID验证 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> IsProductId(IsProductIdRequest isProductIdRequest)
        {
            isProductIdRequest.CurrentUser = User.Identity.Name;
            var isProductIdResponse = new IsProductIdResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.IsProductIdAsync(isProductIdRequest);

                    if (!result.Success)
                    {
                        isProductIdResponse.ResponseCode = "0005";
                        isProductIdResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        isProductIdResponse = result.Result;
                    }


                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "IsProductId");
                isProductIdResponse.ResponseCode = "0009";
                isProductIdResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = isProductIdResponse }, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        ///  保养服务数据源 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetMaintenanceServices()
        {
            var getMaintenanceServicesResponse = new GetMaintenanceServicesResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetMaintenanceServicesAsync();

                    if (!result.Success)
                    {
                        getMaintenanceServicesResponse.ResponseCode = "0005";
                        getMaintenanceServicesResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getMaintenanceServicesResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetMaintenanceServices");
                getMaintenanceServicesResponse.ResponseCode = "0009";
                getMaintenanceServicesResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getMaintenanceServicesResponse }, JsonRequestBehavior.AllowGet);

        }
        #endregion


        #region 特殊模块

        /// <summary>
        ///  特殊模块保养定价保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveMaintenancePricing(SaveMaintenancePricingRequest saveMaintenancePricingRequest)
        {
            saveMaintenancePricingRequest.CurrentUser = User.Identity.Name;
            var saveMaintenancePricingResponse = new SaveMaintenancePricingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveMaintenancePricingAsync(saveMaintenancePricingRequest);

                    if (!result.Success)
                    {
                        saveMaintenancePricingResponse.ResponseCode = "0005";
                        saveMaintenancePricingResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveMaintenancePricingResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveMaintenancePricing");
                saveMaintenancePricingResponse.ResponseCode = "0009";
                saveMaintenancePricingResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveMaintenancePricingResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///  特殊模块倒计时保存接口 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveCountdown(SaveCountdownRequest saveCountdownRequest)
        {
            saveCountdownRequest.CurrentUser = User.Identity.Name;
            var saveCountdownResponse = new SaveCountdownResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveCountdownAsync(saveCountdownRequest);

                    if (!result.Success)
                    {
                        saveCountdownResponse.ResponseCode = "0005";
                        saveCountdownResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveCountdownResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveCountdown");
                saveCountdownResponse.ResponseCode = "0009";
                saveCountdownResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveCountdownResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  轮播文字保存接口 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveWritingts(SaveWritingtsRequest saveWritingtsRequest)
        {
            saveWritingtsRequest.CurrentUser = User.Identity.Name;
            var saveWritingtsResponse = new SaveWritingtsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveWritingtsAsync(saveWritingtsRequest);

                    if (!result.Success)
                    {
                        saveWritingtsResponse.ResponseCode = "0005";
                        saveWritingtsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveWritingtsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveWritingts");
                saveWritingtsResponse.ResponseCode = "0009";
                saveWritingtsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveWritingtsResponse }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///  轮播文字查询接口 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetWritingts(GetWritingtsRequest getWritingtsRequest)
        {
            var getWritingtsResponse = new GetWritingtsResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetWritingtsAsync(getWritingtsRequest);

                    if (!result.Success)
                    {
                        getWritingtsResponse.ResponseCode = "0005";
                        getWritingtsResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getWritingtsResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetWritingts");
                getWritingtsResponse.ResponseCode = "0009";
                getWritingtsResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getWritingtsResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  底部Tab模块保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveBottomTabSetting(SaveBottomTabSettingRequest saveBottomTabSettingRequest)
        {
            saveBottomTabSettingRequest.CurrentUser = User.Identity.Name;
            var saveBottomTabSettingResponse = new SaveBottomTabSettingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveBottomTabSettingAsync(saveBottomTabSettingRequest);

                    if (!result.Success)
                    {
                        saveBottomTabSettingResponse.ResponseCode = "0005";
                        saveBottomTabSettingResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveBottomTabSettingResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveBottomTabSetting");
                saveBottomTabSettingResponse.ResponseCode = "0009";
                saveBottomTabSettingResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveBottomTabSettingResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  底部Tab查询接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetBottomTabSetting(GetBottomTabConfigRequest getBottomTabConfigRequest)
        {
            var getBottomTabConfigResponse = new GetBottomTabConfigResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.GetBottomTabSettingAsync(getBottomTabConfigRequest);

                    if (!result.Success)
                    {
                        getBottomTabConfigResponse.ResponseCode = "0005";
                        getBottomTabConfigResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        getBottomTabConfigResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetBottomTabSetting");
                getBottomTabConfigResponse.ResponseCode = "0009";
                getBottomTabConfigResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = getBottomTabConfigResponse }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        ///  视频模块配置保存接口
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveVideoSetting(SaveVideoSettingRequest saveVideoSettingRequest)
        {
            saveVideoSettingRequest.CurrentUser = User.Identity.Name;
            var saveVideoSettingResponse = new SaveVideoSettingResponse();

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var result = await activityPageClient.SaveVideoSettingAsync(saveVideoSettingRequest);

                    if (!result.Success)
                    {
                        saveVideoSettingResponse.ResponseCode = "0005";
                        saveVideoSettingResponse.ResponseMessage = result.ErrorMessage;
                    }
                    else
                    {
                        saveVideoSettingResponse = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SaveVideoSetting");
                saveVideoSettingResponse.ResponseCode = "0009";
                saveVideoSettingResponse.ResponseMessage = "程序异常";
            }
            return Json(new { data = saveVideoSettingResponse }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region 日志

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="referId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[HttpGet]
        public async Task<JsonResult> GetOperationLogList(string referId,int pageIndex,int pageSize)
        {
            var logResponse = new LogResponse();
            try
            {
                using (var activityLogClient=new SalePromotionActivityLogClient())
                {
                   var result=await activityLogClient.GetOperationLogListAsync(referId, pageIndex, pageSize);
                    if (result.Success)
                    {
                        logResponse.LogPageModel = result.Result.Source.ToList();
                        logResponse.ResponseCode = "0000";
                        if (result.Result.Pager?.Total != null)
                            logResponse.TotalCount = (int) result.Result.Pager?.Total;
                    }
                    else
                    {
                        logResponse.ResponseCode = "0005";
                        logResponse.ResponseMessage = result.ErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetOperationLogList");
                logResponse.ResponseCode = "0009";
                logResponse.ResponseMessage = "程序异常";
            }

            return Json(new { data = logResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取日志详情
        /// </summary>
        /// <param name="FPKID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetOperationLogDetailList(string FPKID)
        {
            var logDetailResponse = new LogDetailResponse();
            try
            {
                using (var activityLogClient = new SalePromotionActivityLogClient())
                {
                    var result = await activityLogClient.GetOperationLogDetailListAsync(FPKID);
                    if (result.Success)
                    {
                        logDetailResponse.LogPageModel = result.Result.ToList();
                        logDetailResponse.ResponseCode = "0000";
                    }
                    else
                    {
                        logDetailResponse.ResponseCode = "0005";
                        logDetailResponse.ResponseMessage = result.ErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetOperationLogDetailList");
                logDetailResponse.ResponseCode = "0009";
                logDetailResponse.ResponseMessage = "程序异常";
            }

            return Json(new{data= logDetailResponse },JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
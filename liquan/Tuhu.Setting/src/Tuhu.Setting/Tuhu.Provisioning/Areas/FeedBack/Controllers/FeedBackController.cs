using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RabbitMQ.Client.Framing.Impl;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Areas.FeedBack.Models;
using Tuhu.Provisioning.Areas.FeedBack.Services;
using Tuhu.Provisioning.Business.Feedback;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.Feedback;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using Exception = System.Exception;

namespace Tuhu.Provisioning.Areas.FeedBack.Controllers
{
    public class FeedBackController : Controller
    {
        private readonly string imgUrl = "https://img1.tuhu.org/";
        private  readonly QuestionTypeBLL  questionTypeBll=new QuestionTypeBLL();
        private readonly  FeedbackBLL  feedbackBll = new FeedbackBLL();
        private static readonly ILog logger = LoggerFactory.GetLogger("FeedBackController");

        #region 问题类型相关

        /// <summary>
        /// 添加(更新)问题类型
        /// </summary>
        /// <param name="typeName">问题类型</param>
        /// <returns></returns>
        //[PowerManage(IwSystem = "OperateSys")]
        [System.Web.Http.HttpPost]
        public async Task<ActionResult> CreateQuestionType([FromBody]QuestionTypeModel questionTypeModel)
        {
            int result = 0;
            string message=String.Empty;
            try
            {
                var currentUser = User.Identity.Name;
                if (questionTypeModel.Id == 0)
                {
                    result = await questionTypeBll.AddQuestionType(questionTypeModel.TypeName, questionTypeModel.Describtion, currentUser);
                    if (result == -1)
                        message = "已存在此类型，不能重复添加";
                }
                else
                {
                    result = await questionTypeBll.UpdateQuestionType(questionTypeModel.TypeName, questionTypeModel.Describtion, currentUser, questionTypeModel.Id);
                }
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "CreateQuestionType");
            }

            return Json(new {Id = result,Message=message}, JsonRequestBehavior.AllowGet);
        }
  
        /// <summary>
        /// 删除问题类型
        /// </summary>
        /// <param name="Id">问题类型ID</param>
        /// <returns>大于0删除成功，则失败</returns>
        //[PowerManage(IwSystem = "OperateSys")]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> DeleteQuestionType(int id)
        {
            int result = 0;
            try
            {
                result = await questionTypeBll.DeleteQuestionType(id);
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "DeleteQuestionType");
            }

            return Json(new { Id = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取问题类型
        /// </summary>
        /// <returns></returns>
        //[PowerManage(IwSystem = "OperateSys")]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetQuestionTypeList()
        {
            var result = new List<QuestionTypeEntity>();
            try
            {
                result = questionTypeBll.GetQuestionTypeList().ToList();
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "GetQuestionTypeList");
            }

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 意见反馈相关
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public ActionResult GetVersionNumList()
        {
            var result=new List<string>();
            try
            {
                result= feedbackBll.GetVersionNumList();
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "GetVersionNumList");
            }

            return Json(new {data= result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取手机型号
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public ActionResult GetPhoneModelList()
        {
            var result = new List<string>();
            try
            {
                result= feedbackBll.GetPhoneModelList();
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "CreateQuestionType");
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取网络环境
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public ActionResult GetNetworkEnvironmentList()
        {
            var result = new List<string>();
            try
            {
                result= feedbackBll.GetNetworkEnvironmentList();
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "GetNetworkEnvironmentList");
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加意见反馈内容
        /// </summary>
        /// <returns></returns>

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> AddFeedbackInfo(FeedbackInfoRequest fdInforequest)
        {
            var result=0;
            string messageStr = string.Empty;
            try
            {
                if (fdInforequest.TypeId == 0)
                    messageStr = "问题类型不能为空";
                else
                    result =(await feedbackBll.AddFeedbackInfo(fdInforequest.TypeId, fdInforequest.UserPhone, fdInforequest.FeedbackContent, fdInforequest.VersionNum, fdInforequest.PhoneModels, fdInforequest.NetworkEnvironment, fdInforequest.Images))>0?1:0;
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "AddFeedbackInfo");
                messageStr = e.Message;
            }
            return Json(new { data = result, message = messageStr }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据条件查询反馈信息
        /// </summary>
        /// <param name="typeId">问题类型Id</param>
        /// <param name="userPhone">用户电话</param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public ActionResult GetFeedbackEntityByCondition(int typeId, string userPhone)
        {
            var result = new FeedbackBLLModel();
            try
            {
                result = feedbackBll.GetFeedbackEntityByTypeIdAndUser(typeId, userPhone);
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "GetFeedbackEntityByTypeIdAndUser");
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分页获取反馈信息
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="typeId">问题类型</param>
        /// <param name="flag">时间标识</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="phoneModel">手机类型</param>
        /// <param name="networkEnvir">网络环境</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [System.Web.Mvc.HttpGet]
        public  ActionResult GetFeedbackListByCondition(int pageIndex,int pageSize,string typeIds,int flag,string versionNum,string phoneModel,string networkEnvir,DateTime? startTime,DateTime? endTime)
        {
            string messageStr = string.Empty;
            PageResult result = new PageResult();
            try
            {
                var queryResult =feedbackBll.GetFeedbackListByCondition(pageIndex,pageSize, typeIds,flag, versionNum, phoneModel, networkEnvir, startTime, endTime);
                result.total = queryResult.total;
                result.FeedbackList = queryResult.FeedbackList.Select(s =>
                    new FeedbackBLLModel
                    {
                        Id=s.Id,
                        FeedbackContent = s.FeedbackContent,
                        NetworkEnvironment = s.NetworkEnvironment,
                        PhoneModels = s.PhoneModels,
                        TypeName = s.TypeName,
                        CreateTime=s.CreateTime.ToString(),
                        UserPhone = s.UserPhone,
                        VersionNum = s.VersionNum,
                        IsCustomerServer = s.IsCustomerServer,
                        FeedbackImgs =!string.IsNullOrEmpty(s.ImgUrl)? (s.ImgUrl.Split(',').Select(x=>x).ToList()):new List<string>()
                    }
                );
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e, "AddFeedbackInfo");
            }
            return Json(new { data = result, message = messageStr }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportToExcel(string typeIds, int flag, string versionNum, string phoneModel, string networkEnvir, DateTime? startTime, DateTime? endTime)
        {
                var queryResult = feedbackBll.GetFeedbackListByCondition(0,0,typeIds, flag, versionNum, phoneModel, networkEnvir, startTime, endTime,1);
                var data =new List<FeedbackBLLModel>();
                if (queryResult.FeedbackList != null && queryResult.FeedbackList.Any())
                {
                    data = queryResult.FeedbackList.Select(s => new FeedbackBLLModel
                        {
                            Id = s.Id,
                            FeedbackContent = s.FeedbackContent,
                            NetworkEnvironment = s.NetworkEnvironment,
                            PhoneModels = s.PhoneModels,
                            TypeName = s.TypeName,
                            CreateTime = s.CreateTime.ToString(),
                            UserPhone = s.UserPhone,
                            VersionNum = s.VersionNum
                        }
                    ).ToList();
                }

                XSSFWorkbook wk = new XSSFWorkbook();
                ISheet tb = wk.CreateSheet("意见反馈");
                //创建表头
                IRow headerRow = tb.CreateRow(0);
                var num = 0;
                headerRow.CreateCell(num++).SetCellValue("序号");
                headerRow.CreateCell(num++).SetCellValue("问题类型");
                headerRow.CreateCell(num++).SetCellValue("日期");
                headerRow.CreateCell(num++).SetCellValue("手机号");
                headerRow.CreateCell(num++).SetCellValue("反馈内容");
                headerRow.CreateCell(num++).SetCellValue("版本号");
                headerRow.CreateCell(num++).SetCellValue("手机型号");
                headerRow.CreateCell(num++).SetCellValue("网络环境");
                //创建数据行
                 for (int i = 0; i < data.Count(); i++)
                    {
                        var row = tb.CreateRow(i + 1);
                        for (int j = 0; j < 8; j++)
                        {
                            var cell = row.CreateCell(j);
                            switch (j)
                            {
                                case 0:
                                    cell.SetCellValue(i+1);
                                    break;
                                case 1:
                                    cell.SetCellValue(data[i].TypeName);
                                    break;
                                case 2:
                                    cell.SetCellValue(data[i].CreateTime);
                                    break;
                                case 3:
                                    cell.SetCellValue(data[i].UserPhone);
                                    break;
                                case 4:
                                    cell.SetCellValue(data[i].FeedbackContent);
                                    break;
                                case 5:
                                    cell.SetCellValue(data[i].VersionNum);
                                    break;
                                case 6:
                                    cell.SetCellValue(data[i].PhoneModels);
                                    break;
                                case 7:
                                    cell.SetCellValue(data[i].NetworkEnvironment);
                                    break;
                            }
                        }
                    }
                var ms = new MemoryStream();
                wk.Write(ms);
                return File(ms.ToArray(), "application/x-xls", "意见反馈.xlsx");
            
        }

        /// <summary>
        /// 下载Excel文件
        /// </summary>
        /// <param name="download"></param>
        /// <returns></returns>
        public FileResult GetExcelFile(string download)
        {
            FileStream fs = (FileStream)Session[download];
            Session.Remove(download);
            return File(fs, "application/vnd.ms-excel", download);
        }

        /// <summary>
        /// 客服介入
        /// </summary>
        /// <param name="remark">反馈内容</param>
        /// <param name="phone">手机号</param>
        /// <param name="appVersion">版本号</param>
        /// <param name="phoneModel">手机型号</param>
        /// <param name="id">反馈id</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public async Task<JsonResult> CreateTaskNeiBuWithDistinctAsync(string remark,string phone, string appVersion,string phoneModel,int id)
        {
            var currentUser = User.Identity.Name;
            var taskId=await OrderTaskService.CreateTaskNeiBuCommonAsync(remark,phone, currentUser);
            int result = 0;
            if (taskId != 0)
            {
               var commentId= await OrderTaskService.CreateUmengCommentsAsync(taskId, remark,phone, currentUser,appVersion,phoneModel);
                if (commentId != 0)
                    result= await feedbackBll.UpdateIsCustomerServer(id);
            }
            return   Json(new { data =result});
        }

       
        #endregion


    }
}
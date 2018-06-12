using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.WebSite.Component.Discovery.Business;
using Tuhu.WebSite.Component.Discovery.BusinessData;
using Tuhu.WebSite.Component.SystemFramework.Log;
using Tuhu.WebSite.Web.Activity.Models;

namespace Tuhu.WebSite.Web.Activity.Filters
{
    /// <summary>
    /// 用于收集用户行为信息
    /// </summary>
    public class  CollectUserActionAttribute : ActionFilterAttribute, IActionFilter
    {
        private UserOperationEnum _operationOption;
        private UserOperation _operation;
        private IDictionary<string, object> _actionParams;
        /// <summary>
        /// 构造操作类型
        /// </summary>
        /// <param name="operationOption"></param>
        public CollectUserActionAttribute(UserOperationEnum operationOption) : base()
        {
            this._operationOption = operationOption;
        }
        /// <summary>
        /// 操作成功后记录用户行为
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                base.OnActionExecuted(filterContext);
                ActionResult result = filterContext.Result;
                if (IsActionResultSuccess(result))
                {
                    ActionResultExcuteFactory factory = new ActionResultExcuteFactory(this._operationOption, this._actionParams);
                    this._operation = factory.GetUserOperation();
                    MqHelper.Send(MqHelper.ARTICLE_STATISTICS_QUEUE, new Dictionary<string, object>()
                    {
                        {"Operation", _operation.Operation},
                        {"ArticleId", _operation.ArticleId}
                    });
                    //ArticleBll.InsertUserOperation(this._operation).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this._actionParams = filterContext.ActionParameters;
        }
        #region 判断是否成功执行
        /// <summary>
        /// 判断是否成功执行
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool IsActionResultSuccess(ActionResult result)
        {
            bool isSuccess = false;
            switch (this._operationOption)
            {
                case UserOperationEnum.Comment:
                    var dic= (Dictionary<string, object>)(((JsonResult)result).Data);
                    isSuccess = dic["Code"].ToString() == "1";
                    break;
                case UserOperationEnum.DeleteComment:
                    var md = (CommentViewModel)(((JsonResult)result).Data);
                    isSuccess = md.success;
                    break;
                case UserOperationEnum.Favorite:
                    var model = (FavoriteViewModel)(((JsonResult)result).Data);
                    isSuccess = model.success;
                    break;
                case UserOperationEnum.Read:
                    isSuccess = true;
                    break;
                case UserOperationEnum.Share:
                    isSuccess = true;
                    break;
                default: break;
            }
            return isSuccess;
        }
        #endregion

    }
    #region 设置记录数据
    /// <summary>
    /// 设置记录数据
    /// </summary>
    public class ActionResultExcuteFactory
    {
        private UserOperationEnum _operationOption;
        private IDictionary<string, object> actionParams;
        public ActionResultExcuteFactory(UserOperationEnum operationOption, IDictionary<string, object> actionParams)
        {
            this._operationOption = operationOption;
            this.actionParams = actionParams;
        }
        public UserOperation GetUserOperation()
        {
            UserOperation op = new UserOperation();
            switch (this._operationOption)
            {
                case UserOperationEnum.Read:
                case UserOperationEnum.Share:
                case UserOperationEnum.Favorite:
                    op.ArticleId = Convert.ToInt32(this.actionParams["Id"]);
                    op.CreateDateTime = DateTime.Now;
                    op.DeviceId = this.actionParams.ContainsKey("DeviceId") && !string.IsNullOrEmpty((string)this.actionParams["DeviceId"])
                                                    ? Guid.Parse(this.actionParams["DeviceId"].ToString()) : Guid.Empty;
                    op.Operation = this._operationOption.ToString();
                    op.UserId = this.actionParams.ContainsKey("userId") && !string.IsNullOrEmpty(this.actionParams["userId"]?.ToString())
                                                    ? Guid.Parse(this.actionParams["userId"].ToString()) : Guid.Empty;
                    break;
                case UserOperationEnum.Comment:
                    ArticleCommentModel comment = this.actionParams["acm"] as ArticleCommentModel;
                    op.ArticleId = comment.PKID;
                    op.CreateDateTime = DateTime.Now;
                    op.DeviceId = Guid.Empty;
                    op.Operation = this._operationOption.ToString();
                    op.UserId = Guid.Parse(comment.UserID);
                    break;
            }
            return op;
        }
    }
    #endregion
}
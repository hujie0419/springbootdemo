using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.C.Job.ThirdPart.Model;
using Tuhu.C.Job.ThirdPart.Model.Enum;
using Tuhu.C.Job.ThirdPart.Model.GFCard;
using Tuhu.C.Job.ThirdPart.Proxy;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Member.Request;

namespace Tuhu.C.Job.ThirdPart.Manager
{
    /// <summary>
    /// 发券任务执行者
    /// </summary>
    public class GFPromotionTaskExecutor
    {
        private GFDal _gfDal = GFDal.GetInstance();
        private static readonly string _promotionchannel = "GFBankCardJob";
        private GFBankPromotionTask _task;
        private const int _promotionCount = 1;
        private const int _redemptionCodeCount = 1;
        private static readonly Guid _redemptionCodeConfig = new Guid("8af70ce9-c9ea-42b7-8bf5-cffe44ef5290");
        public GFPromotionTaskExecutor(GFBankPromotionTask task)
        {
            this._task = task;
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteTask()
        {
            if (_task == null)
                return;
            switch (_task.BusinessType)
            {
                case nameof(GFBusinessType.Activate):
                    await ExecuteActivateTask();
                    break;
                case nameof(GFBusinessType.Cancel):
                    await ExecuteCancelTask();
                    break;
            }
        }
        /// <summary>
        /// 执行卡号激活任务
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteActivateTask()
        {
            switch (_task.Status)
            {
                case nameof(GFTaskStatus.Created):
                case nameof(GFTaskStatus.Failed):
                    var createPromotionResult = await CreatePromotion();
                    await _gfDal.UpdateGFBankPromotionTaskStatus(_task.PKID,
                        createPromotionResult ? nameof(GFTaskStatus.Success) : nameof(GFTaskStatus.Failed));
                    break;
            }
        }
        /// <summary>
        /// 执行卡号注销任务
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteCancelTask()
        {
            switch (_task.Status)
            {
                case nameof(GFTaskStatus.Created):
                case nameof(GFTaskStatus.Failed):
                    var invalidatedPromotionResult = await InvalidatedPromotion();
                    await _gfDal.UpdateGFBankPromotionTaskStatus(_task.PKID,
                        invalidatedPromotionResult ? nameof(GFTaskStatus.Success) : nameof(GFTaskStatus.Failed));
                    break;
            }
        }
        /// <summary>
        /// 发放优惠券
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreatePromotion()
        {
            var result = false;
            var createPromotionModel = Enumerable.Range(0, _promotionCount).Select(i => new CreatePromotionModel
            {
                Author = _task.Mobile,
                Channel = "GFBankCardJob",
                GetRuleGUID = _task.RuleGuid,
                UserID = _task.UserId,
                Operation = "广发联名卡用户发券",
                Issuer = "GFBankCard",
            });
            var gantPromotionResult = await MemberServiceProxy.CreatePromotions(createPromotionModel);
            if (gantPromotionResult != null && gantPromotionResult.IsSuccess && gantPromotionResult.promotionIds != null && gantPromotionResult.promotionIds.Any())
            {
                await _gfDal.UpdateGFBankPromotionTaskPromotionIds(_task.PKID, string.Join(",", gantPromotionResult.promotionIds));          
                result = true;
            }

            return result;
        }
        /// <summary>
        /// 作废优惠券
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InvalidatedPromotion()
        {
            var result = true;
            var userCreatePromotionTask = await _gfDal.SelectGFBankPromotionTaskByMobile(_task.Mobile);
            var taskPromotionIds = userCreatePromotionTask.Where(s => string.Equals(s.BusinessType, nameof(GFBusinessType.Activate)))
                .FirstOrDefault(s => s.RuleGuid == _task.RuleGuid)?.PromotionIds;
            if (!string.IsNullOrWhiteSpace(taskPromotionIds))
            {
                var promotionIdList = taskPromotionIds.Split(',');
                var success = true;
                foreach (var promotionId in promotionIdList)
                {
                    var promotion = await MemberServiceProxy.FetchPromotionByPromotionCode(promotionId);
                    if (promotion.Status == 0)
                    {
                        var updateRequest = new UpdateUserPromotionCodeStatusRequest()
                        {
                            UserID = _task.UserId,
                            PromotionCodeId = Convert.ToInt32(promotionId),
                            Status = 2,
                            Channel = _promotionchannel,
                            OperationAuthor = "GuangFa"
                        };
                        var updateResult = await MemberServiceProxy.UpdateUserPromotionCodeStatus(updateRequest);
                        if (!updateResult)
                        {
                            success = false;
                            JobLogger.GFLogger.Warn($"作废优惠券失败，promotionId:{promotionId},task:{this.ToString()}");
                            break;
                        }
                    }
                    else if (promotion.Status == 1)
                    {
                        JobLogger.GFLogger.Warn($"广发需要作废的优惠券已经使用,promotionId:{promotionId}");
                    }
                }
                result = success;
            }

            return result;
        }

        public override string ToString() => $"UserId:{_task.UserId},Mobile:{_task.Mobile},RuleGuid:{_task.RuleGuid},PromotionIds:{_task.PromotionIds},BusinessType:{_task.BusinessType},Status:{_task.Status},SourceFileName:{_task.SourceFileName}";
    }
}

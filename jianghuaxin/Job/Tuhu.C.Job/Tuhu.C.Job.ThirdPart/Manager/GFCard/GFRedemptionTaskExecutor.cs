using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.C.Job.ThirdPart.Model.Enum;
using Tuhu.C.Job.ThirdPart.Model.GFCard;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;
using Tuhu.Service.Member.Models;

namespace Tuhu.C.Job.ThirdPart.Manager.GFCard
{
    class GFRedemptionTaskExecutor
    {
        private GFDal _gfDal = GFDal.GetInstance();
        private GFBankRedemptionCodeTask _task;
        private const int _redemptionCodeCount = 1;
        private static readonly Guid _redemptionCodeConfig = new Guid("8af70ce9-c9ea-42b7-8bf5-cffe44ef5290");
        public GFRedemptionTaskExecutor(GFBankRedemptionCodeTask task)
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
                    var redemptionCodeResult = await CreateRedemptionCode();
                    await _gfDal.UpdateGFBankRedemptionCodeTaskStatus(_task.PKID,
                        redemptionCodeResult ? nameof(GFTaskStatus.Success) : nameof(GFTaskStatus.Failed));
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
                    var invalidateRedemptionCodeResult = await InvalidateRedemptionCode();
                    await _gfDal.UpdateGFBankRedemptionCodeTaskStatus(_task.PKID,
                        invalidateRedemptionCodeResult ? nameof(GFTaskStatus.Success) : nameof(GFTaskStatus.Failed));
                    break;
            }
        }
        /// <summary>
        /// 发放兑换码
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreateRedemptionCode()
        {
            var result = false;
            var generateCodeResult = new Service.GroupBuying.Models.ResultModel.ResultModel<RedemptionResult>() { IsSuccess = false };
            if (string.IsNullOrEmpty(_task.RedemptionCode))
            {
                generateCodeResult = await Proxy.GroupBuyingServiceProxy.GenerateUSRedemptionCode(new GenerateUSRedemptionCodeRequest()
                {
                    ConfigId = _redemptionCodeConfig,
                    UserId = _task.UserId,
                    Quantity = _redemptionCodeCount
                });
                if (generateCodeResult.IsSuccess)
                    await _gfDal.UpdateGFBankRedemptionCodeTaskRedemptionCode(_task.PKID, generateCodeResult.Data.RedemptionCode);
            }
            else
            {
                var redemptionResult = await Proxy.GroupBuyingServiceProxy.GetRedemptionCodeDetail(_task.RedemptionCode);
                if (redemptionResult != null)
                {
                    generateCodeResult.Data = redemptionResult;
                    generateCodeResult.IsSuccess = true;
                }
            }
            if (generateCodeResult.IsSuccess && generateCodeResult.Data?.RedeemMrCodeItem != null)
            {
                if (string.Equals(generateCodeResult.Data.Status, "2Redeemed"))
                {
                    return true;
                }
                var reedemRedemptionRequest = new ReedemRedemptionRequest()
                {
                    RedemptionCode = generateCodeResult.Data.RedemptionCode,
                    UserId = _task.UserId,
                    RedeemMrCodeItem = generateCodeResult.Data.RedeemMrCodeItem.Select(s => new RedemptionRequestItem()
                    {
                        PKID = s.PKID,
                        IsChoosed = true
                    })
                };
                var redeemResult = await Proxy.GroupBuyingServiceProxy.RedeemRedemptionCodeByChoice(reedemRedemptionRequest);
                if (redeemResult.Success)
                {
                    result = true;
                }
            }

            return result;
        }
        /// <summary>
        /// 作废兑换码
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InvalidateRedemptionCode()
        {
            var result = true;
            var activeTask = await _gfDal.SelectGFBankRedemptionCodeTaskByMobile(_task.Mobile, nameof(GFBusinessType.Activate));
            var redemptionCode = activeTask?.FirstOrDefault()?.RedemptionCode;
            if (!string.IsNullOrEmpty(redemptionCode))
            {
                var invalidateResult = await Proxy.GroupBuyingServiceProxy.InvalidateRedemptionCode(new InvalidateRedemptionCodeRequest()
                {
                    Codes = new List<string>() { redemptionCode },
                    Remark = "广发联名卡注销作废"
                });
                if (invalidateResult != null && invalidateResult.Success)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public override string ToString() => $"UserId:{_task.UserId},Mobile:{_task.Mobile},RedemptionCode:{_task.RedemptionCode},BusinessType:{_task.BusinessType},Status:{_task.Status},SourceFileName:{_task.SourceFileName}";
    }
}

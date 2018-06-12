using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.C.Job.ThirdPart.Model;
using Tuhu.C.Job.ThirdPart.Model.Enum;
using Tuhu.C.Job.ThirdPart.Model.GFCard;
using Tuhu.C.Job.ThirdPart.Proxy;
using Tuhu.Service.Member.Models;

namespace Tuhu.C.Job.ThirdPart.Manager
{
    /// <summary>
    /// 任务创建者
    /// </summary>
    public class GFTaskCreator
    {
        private string _mobile;
        private string _userName;
        private string _cardLevel;
        private string _businessType;
        private string _sourceFileName;
        private Guid _userId;
        private static readonly string _promotionRules = ConfigurationManager.AppSettings["GFPromotionRuleGuids"];
        private GFDal _gfDal = GFDal.GetInstance();
        public GFTaskCreator(string mobile, string userName, string cardLevel, string businessType, string sourceFileName)
        {
            this._mobile = mobile;
            this._userName = userName;
            this._cardLevel = cardLevel;
            this._businessType = GFModelBll.ConvertToTuhuBusinessType(businessType);
            this._sourceFileName = sourceFileName;
        }
        /// <summary>
        /// 创建广发记录和发券任务
        /// </summary>
        /// <returns></returns>
        public async Task CreateCardRecordAndTask()
        {
            var user = await UserAccountServiceProxy.GetOrCreateUser(this._mobile);
            if (user != null)
            {
                _userId = user.UserId;
                var record = new GFBankCardRecord
                {
                    UserId = _userId,
                    Mobile = _mobile,
                    UserName = _userName,
                    CardLevel = _cardLevel,
                    BusinessType = _businessType,
                    SourceFileName = _sourceFileName
                };
                var insertResult = await _gfDal.InsertGFBankCardRecord(record);
                var userPromotionTasks = await _gfDal.SelectGFBankPromotionTaskByMobile(_mobile);
                var lastPromotionTask = userPromotionTasks.FirstOrDefault();
                if (lastPromotionTask == null || !string.Equals(lastPromotionTask.BusinessType, _businessType))
                    await CreatePromotionTask();

                var userRedemptionCodeTasks = await _gfDal.SelectGFBankRedemptionCodeTask(_mobile);
                var lastRedemptionTask = userRedemptionCodeTasks.FirstOrDefault();
                if (lastRedemptionTask == null || !string.Equals(lastRedemptionTask.BusinessType, _businessType))
                    await CreateRedemptionCodeTask();
            }
            else
            {
                JobLogger.GFLogger.Error($"根据广发数据创建用户失败,{this.ToString}");
            }
        }
        /// <summary>
        /// 创建发券或作废券任务，BusinessType是任务类型
        /// </summary>
        /// <returns></returns>
        private async Task CreatePromotionTask()
        {
            var ruleGuids = _promotionRules.Split(',').Select(s => new Guid(s)).Distinct();
            foreach (var rule in ruleGuids)
            {
                var insertTaskResult = await _gfDal.InsertGFBankPromotionTask(new GFBankPromotionTask()
                {
                    UserId = _userId,
                    Mobile = _mobile,
                    RuleGuid = rule,
                    BusinessType = _businessType,
                    Status = nameof(GFTaskStatus.Created),
                    SourceFileName = _sourceFileName
                });
                if (!insertTaskResult)
                {
                    JobLogger.GFLogger.Error($"创建发券任务失败,{this.ToString}");
                }
            }
        }
        /// <summary>
        /// 创建兑换码任务
        /// </summary>
        /// <returns></returns>
        private async Task CreateRedemptionCodeTask()
        {
            var insertTaskResult = await _gfDal.InsertGFBankRedemptionCodeTask(new GFBankRedemptionCodeTask()
            {
                UserId = _userId,
                Mobile = _mobile,
                BusinessType = _businessType,
                Status = nameof(GFTaskStatus.Created),
                SourceFileName = _sourceFileName
            });
            if (!insertTaskResult)
            {
                JobLogger.GFLogger.Error($"创建兑换码任务失败,{this.ToString}");
            }
        }

        private new string ToString => $"userId:{_userId},mobile:{_mobile},userName:{_userName},cardLevel:{_cardLevel},businessType:{_businessType},sourceFileName:{_sourceFileName}";

    }
}

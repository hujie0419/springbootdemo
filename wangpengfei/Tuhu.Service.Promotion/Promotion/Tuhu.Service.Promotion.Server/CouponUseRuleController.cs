using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Manager;
using Tuhu.Service.Promotion.Server.Manager.IManager;

namespace Tuhu.Service.Promotion.Server
{
    /// <summary>
    /// 使用规则 - 服务
    /// </summary>
    public class CouponUseRuleController : CouponUseRuleService
    {
        public ICouponGetRuleManager _CouponGetRuleManager;
        private readonly ILogger _logger;
        public CouponUseRuleController(ICouponManager ICouponManager,
            ILogger<CouponUseRuleController> Logger,
            ICouponGetRuleManager ICouponGetRuleManager
            )
        {
            _logger = Logger;
            _CouponGetRuleManager = ICouponGetRuleManager;
        }

        public override ValueTask<OperationResult<CouponModel>> GetRuleByIDAsync([FromBody] int ID)
        {
            throw new NotImplementedException();
        }
    }
}

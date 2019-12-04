using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.IntegrationTest
{
    public class CouponGetRuleTest
    {
        private readonly ICouponGetRuleClient _client;

        public CouponGetRuleTest(ICouponGetRuleClient client) => _client = client;

        [Fact]
        public async Task GetCouponRuleListAsync()
        {
            List<Guid> request = new  List<Guid>()
            {
                new Guid("2CA32BEB-0D4D-4C75-BA10-EA2B6EDEC4C8")
            };
            var result = await _client.GetCouponGetRuleListAsync(request).ConfigureAwait(false);

            result.ThrowIfException(true);
        }


        [Fact]
        public async Task CreateCouponGetRuleAuditAsync()
        {
            CreateCouponGetRuleAuditRequest request = new CreateCouponGetRuleAuditRequest()
            {
                GetCouponRulePKID=1
            };
            var result = await _client.CreateCouponGetRuleAuditAsync(request).ConfigureAwait(false);

            result.ThrowIfException(true);
        }


        [Fact]
        public async Task GetCouponGetRuleAuditorAsync()
        {
            GetCouponGetRuleAuditorRequest request = new GetCouponGetRuleAuditorRequest()
            {
               
            };
            var result = await _client.GetCouponGetRuleAuditorAsync(1,1).ConfigureAwait(false);

            result.ThrowIfException(true);
        }


        


        [Fact]
        public async Task UpdateCouponGetRuleAuditAsync()
        {
            UpdateCouponGetRuleAuditRequest request = new UpdateCouponGetRuleAuditRequest()
            {
            };
            var result = await _client.UpdateCouponGetRuleAuditAsync(request).ConfigureAwait(false);

            result.ThrowIfException(true);
        }


        [Fact]
        public async Task GetPromotionBusinessLineConfigListAsync()
        {
            var result = await _client.GetPromotionBusinessLineConfigListAsync().ConfigureAwait(false);

            result.ThrowIfException(true);
        }
    }
}

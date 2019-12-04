using System;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.Request;
using Xunit;

namespace Tuhu.Service.Promotion.IntegrationTest
{
    public class PromotionTest
    {
        private readonly IPromotionClient _client;

        public PromotionTest(IPromotionClient client) => _client = client;

        public Guid TestUserID = Guid.Parse("FEEF7C6D-B5B6-41C8-8108-76596514A510");


        [Fact]
        public async Task GetCouponByUserIDAsync()
        {
            GetCouponByUserIDRequest request = new GetCouponByUserIDRequest()
            {
                UserID = TestUserID,
                IsHistory = 0,
            };
            var result = await _client.GetCouponByUserIDAsync(request).ConfigureAwait(false);
            result.ThrowIfException(true);
        }
    }
}

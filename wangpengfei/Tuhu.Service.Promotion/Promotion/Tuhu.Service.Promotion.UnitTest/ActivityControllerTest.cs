using System.Threading.Tasks;
using Xunit;
using System.ComponentModel;
using Tuhu.Service.Promotion.Server;
using Tuhu.Service.Promotion.Server.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Tuhu.Service.Promotion.UnitTest
{
    public class ActivityControllerTest
    {
        private readonly IActivityClient _client;
        public ActivityControllerTest(IActivityClient client)
        {
            _client = client;
        }
        [Fact]
        [Description("获取活动信息")]
        public async Task GetActivityTest()
        {
            var result = await _client.GetActivityInfoAsync(1).ConfigureAwait(false);
            result.ThrowIfException(true);

            Assert.NotNull(result.Result);
        }

    }
}

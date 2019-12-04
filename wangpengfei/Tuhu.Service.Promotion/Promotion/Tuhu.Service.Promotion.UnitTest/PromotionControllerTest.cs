using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess;
using Tuhu.Service.Promotion.Server;
using Xunit;

namespace Tuhu.Service.Promotion.UnitTest
{
    public class PromotionControllerTest
    {
        //[Fact]
        //public async Task DemoMethod()
        //{
        //    var moq = new Mock<IPromotionRepository>();

        //    moq.Setup(o => o.DemoMethodAsync(1, It.IsAny<CancellationToken>()))
        //        .Returns(new ValueTask<bool>(true));

        //    var result = await new PromotionController()
        //    {
        //        ControllerContext = Mock.Of<ControllerContext>(ctx => ctx.HttpContext == Mock.Of<HttpContext>())
        //    }.DemoMethodAsync(1, moq.Object, "2");

        //    Assert.NotNull(result);

        //    result.ThrowIfException(true);

        //    Assert.Equal(2, result.Result.Count);
        //    Assert.Equal("1", result.Result[0]);
        //    Assert.Equal("2", result.Result[1]);

        //    moq.Verify(p => p.DemoMethodAsync(1, It.IsAny<CancellationToken>()));
        //    moq.VerifyNoOtherCalls();
        //}
    }
}

using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.Models;
using Xunit;

namespace Tuhu.C.ThirdParty.SyncProductPrice.Test
{
    public class ProductsMappingTest
    {
        [Fact]
        public async Task SaveProductMappingTest()
        {
            var result = await Products.SaveProductMapping(new ProductPriceMappingModel
            {
                Pid = "TR-GT-WINGRO|6",
                ItemId = 11414278129,
                SkuId = 25714302097,
                Title = "佳通汽车轮胎途虎品质包安装 WINGRO",
                ShopCode = "5京东服务店"
            });
            Assert.True(result > 0);
        }

        [Theory]
        [InlineData("5京东服务店")]
        public async Task QueryProductMappingsTest(string shopCode)
        {
            var mappings = (await Products.QueryProductMappings(shopCode)).ToArray();
            Assert.NotNull(mappings);
            Assert.NotEmpty(mappings);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.Models;
using Xunit;

namespace Tuhu.C.ThirdParty.SyncProductPrice.Test
{
    public class LianjiaTest
    {
        [Fact]
        public async Task SaveTest()
        {
            var result = await LianjiaXiaoqu.SaveXiaoquInfoAsync(new List<LianjiaXiaoquModel>());
            Assert.Equal(0, result);
            result = await LianjiaXiaoqu.SaveXiaoquInfoAsync(new List<LianjiaXiaoquModel>
            {
                new LianjiaXiaoquModel
                {
                    XiaoquId = 5011102207057,
                    LinkUrl = "https://sh.lianjia.com/xiaoqu/5011102207057/",
                    Name = "上海康城",
                    City = "上海",
                    District = "闵行",
                    Address = "(闵行莘庄)莘松路958弄（大浪湾道01-61号、山林道04-96号、江山道01-23号、瀑布湾道21-96号、维园道01-45号、康城道51-82）",
                    Price = 36738,
                    Age = "2001年建成",
                    BuildingType = "板楼",
                    WuyeFee = "1.5元/平米/月",
                    WuyeCompany = "盛付物业",
                    Developer = "闵行房地产",
                    BuildingNum = "288栋",
                    HouseNum = "12141户"
                }
            });
            Assert.True(result > 0);

            var abc = await LianjiaXiaoqu.SaveXiaoquInfoAsync(new LianjiaXiaoquModel
            {
                XiaoquId = 5011102207057,
                LinkUrl = "https://sh.lianjia.com/xiaoqu/5011102207057/",
                Name = "上海康城",
                City = "上海",
                District = "闵行",
                Address = "(闵行莘庄)莘松路958弄（大浪湾道01-61号、山林道04-96号、江山道01-23号、瀑布湾道21-96号、维园道01-45号、康城道51-82）",
                Price = 36738,
                Age = "2001年建成",
                BuildingType = "板楼",
                WuyeFee = "1.5元/平米/月",
                WuyeCompany = "盛付物业",
                Developer = "闵行房地产",
                BuildingNum = "288栋",
                HouseNum = "12141户"
            }
            );
            Assert.True(abc);
            var aa = await LianjiaXiaoqu.SaveXiaoquInfoAsync(new LianjiaXiaoquModel
            {
                XiaoquId = 1,
                LinkUrl = "https://sh.lianjia.com/xiaoqu/5011102207057/",
                Name = "上海康城",
                City = "上海",
                District = "闵行",
                Address = "(闵行莘庄)莘松路958弄（大浪湾道01-61号、山林道04-96号、江山道01-23号、瀑布湾道21-96号、维园道01-45号、康城道51-82）",
                Price = 36738,
                Age = "2001年建成",
                BuildingType = "板楼",
                WuyeFee = "1.5元/平米/月",
                WuyeCompany = "盛付物业",
                Developer = "闵行房地产",
                BuildingNum = "288栋",
                HouseNum = "12141户"
            }
            );
            Assert.True(aa);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Activity.DataAccess;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class LuckyCharmUnitTest
    {
        [TestMethod]
        public void PageActivityUserTest()
        {
            //var condition = new PageLuckyCharmUserRequest() { AreaName = "上海市闵行区", PageIndex = 1, PageSize = 10 };
            var condition = new PageLuckyCharmUserRequest() { CheckState = 0, PageIndex = 1, PageSize = 10 };
            //var results = DalLuckyCharm.IsExistUserByPKID(2);
            //Assert.AreEqual(results.Result, true);
            //return;++
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            using (var client = new LuckyCharmClient())
            {
                for (int i = 0; i < 10; i++)
                {
                    stopwatch.Start();
                    var result = client.GetLuckyCharmActivity(3);
                    stopwatch.Stop();
                    Console.WriteLine($"第{i}次耗时：{stopwatch.ElapsedMilliseconds}");
                    Assert.IsTrue(result.Success);
                }
            }
        }
    }
}

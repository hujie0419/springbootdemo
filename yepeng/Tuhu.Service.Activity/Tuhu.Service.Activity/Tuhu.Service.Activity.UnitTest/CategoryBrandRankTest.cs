using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class CategoryBrandRankTest
    {
        [TestMethod]
        public void SelectAllCategoryBrandByDate()
        {
            using (var client = new CategoryBrandRankClient())
            {
                var result = client.SelectAllCategoryBrandByDate(DateTime.Now);
                result.ThrowIfException(true);
                Assert.IsNotNull(result.Result);
                Assert.AreEqual(result.Result.Any(), true);
            }
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tuhu.Service.Activity.UnitTest
{
    /// <summary>
    /// SexAnnualVoteTest 的摘要说明
    /// </summary>
    [TestClass]
    public class SexAnnualVoteTest
    {

        [TestMethod]
        public void AddShopSignUp()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.AddShopSignUp(new Models.ShopVoteModel()
                {
                    ShopId = 1,
                    Area = "50平方米",
                    CreateDate = "2016年",
                    Description = "介绍",
                    ImageUrls = Newtonsoft.Json.JsonConvert.SerializeObject(new string[] {"1", "2"}),
                    EmployeeCount = 2
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
            }
        }
        [TestMethod]
        public void AddEmployeeSignUp()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.AddEmployeeSignUp(new Models.ShopEmployeeVoteModel()
                {
                    Age = 1,
                    City = "上海",
                    Description = "介绍",
                    EmployeeId = 1,
                    ExpertiseModels = "路虎",
                    ExpertiseProjects = "轮胎",
                    Hobby = "没有",
                    ImageUrls = Newtonsoft.Json.JsonConvert.SerializeObject(new string[] {"1", "2"}),
                    Name = "张三",
                    ShopId = 1,
                    VideoUrl = "",
                    YearsEmployed = 1
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
            }
        }

        [TestMethod]
        public void CheckShopSignUp()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.CheckShopSignUp(1);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
                result = client.CheckShopSignUp(0);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, false);
            }
        }

        [TestMethod]
        public void CheckEmployeeSignUp()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.CheckEmployeeSignUp(1,1);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
                result = client.CheckEmployeeSignUp(1,0);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, false);
            }
        }

        [TestMethod]
        public void SelectShopRanking()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.SelectShopRanking(new Models.Requests.SexAnnualVoteQueryRequest()
                {
                    PageIndex = 1,
                    PageSize = 10
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Source.Any(),true);
                result = client.SelectShopRanking(new Models.Requests.SexAnnualVoteQueryRequest()
                {
                    PageIndex = 1,
                    PageSize = 10,
                    ProvinceId=1
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Source.Any(), true);

                result = client.SelectShopRanking(new Models.Requests.SexAnnualVoteQueryRequest()
                {
                    PageIndex = 1,
                    PageSize = 10,
                    ProvinceId = 1,
                    Keywords="22"
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Source.Any(), true);
            }
        }
        [TestMethod]
        public void SelectShopEmployeeRanking()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.SelectShopEmployeeRanking(new Models.Requests.SexAnnualVoteQueryRequest()
                {
                    PageIndex=1,
                    PageSize=10
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Source.Any(), true);
                result = client.SelectShopEmployeeRanking(new Models.Requests.SexAnnualVoteQueryRequest()
                {
                    PageIndex = 1,
                    PageSize = 10,
                    ProvinceId=1
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Source.Any(), true);
                result = client.SelectShopEmployeeRanking(new Models.Requests.SexAnnualVoteQueryRequest()
                {
                    PageIndex = 1,
                    PageSize = 10,
                    ProvinceId = 1,
                    Keywords = "22"
                });
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Source.Any(), true);
            }
        }
        [TestMethod]
        public void FetchShopDetail()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.FetchShopDetail(21159);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result == null, false);
            }
        }

        [TestMethod]
        public void FetchShopEmployeeDetail()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.FetchShopEmployeeDetail(11727, 10319);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result == null, false);
            }
        }
        [TestMethod]
        public void AddShopVote()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.AddShopVote(Guid.Parse("{607084d4-36fc-4f37-94b7-b68c77921f60}"), 1);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
            }
        }
        [TestMethod]
        public void SelectShopVoteRecord()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.SelectShopVoteRecord(Guid.Parse("{607084d4-36fc-4f37-94b7-b68c77921f60}"),
                    DateTime.Now, DateTime.Now);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Any(), true);
            }
        }

        [TestMethod]
        public void AddShopEmployeeVote()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.AddShopEmployeeVote(Guid.Parse("{607084d4-36fc-4f37-94b7-b68c77921f60}"), 1, 1);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
            }
        }

        [TestMethod]
        public void SelectShopEmployeeVoteRecord()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.SelectShopEmployeeVoteRecord(Guid.Parse("{607084d4-36fc-4f37-94b7-b68c77921f60}"),
                    DateTime.Now, DateTime.Now);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Any(), true);
            }
        }
        [TestMethod]
        public void AddShareShopVote()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.AddShareShopVote(Guid.Parse("{607084d4-36fc-4f37-94b7-b68c77921f60}"), 1);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
            }
        }
        [TestMethod]
        public void AddShareShopEmployeeVote()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result =
                    client.AddShareShopEmployeeVote(Guid.Parse("{208592FF-19BE-41A4-AF18-4FBFD8D3281A}"), 1, 1);
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result, true);
            }
        }
        [TestMethod]
        public void GetShopRegion()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.GetShopRegion();
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Count > 0, true);
            }
        }

        [TestMethod]
        public void GetShopEmployeeRegionAsync()
        {
            using (var client = new SexAnnualVoteClient())
            {
                var result = client.GetShopEmployeeRegion();
                result.ThrowIfException(true);
                Assert.AreEqual(result.Result.Count > 0, true);
            }
        }
    }
}

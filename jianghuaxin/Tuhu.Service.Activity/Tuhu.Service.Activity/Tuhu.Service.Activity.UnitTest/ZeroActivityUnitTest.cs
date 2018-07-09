using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models;
using System.Linq;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class ZeroActivityUnitTest
    {
        [TestMethod]
        public void SelectUnfinishedZeroActivityForHomepage()
        {
            using (var client = new ZeroActivityClient())
            {

                var result1 = client.SelectUnfinishedZeroActivitiesForHomepage(false);

                var result = client.SelectUnfinishedZeroActivitiesForHomepage(true);

                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectFinishedZeroActivityForHomepage()
        {
            using (var client = new ZeroActivityClient())
            {
                for (var i = 1; i <= 10; ++i)
                {
                    var result = client.SelectFinishedZeroActivitiesForHomepage(i);

                    Assert.IsNotNull(result.Result);
                }
            }
        }

        [TestMethod]
        public void FetchZeroActivityDetail()
        {
            using (var client = new ZeroActivityClient())
            {
                for (var i = 109; i >= 90; --i)
                {
                    var result = client.FetchZeroActivityDetail(i);

                    Assert.IsNotNull(result.Result);
                }
            }
        }

        [TestMethod]
        public void HasZeroActivityApplicationSubmitted()
        {
            using (var client = new ZeroActivityClient())
            {
                var result = client.HasZeroActivityApplicationSubmitted(new Guid("28876C74-1349-43A1-958B-1935550C1992"), 105);

                Assert.IsTrue(result.Result);
            }
        }

        [TestMethod]
        public void HasZeroActivityReminderSubmitted()
        {
            using (var client = new ZeroActivityClient())
            {
                var result = client.HasZeroActivityReminderSubmitted(new Guid("4f79e798-502c-1891-afe5-156b452bf4b2"), 106);

                Assert.IsTrue(result.Result);
            }
        }

        [TestMethod]
        public void SelectChosenUserReports()
        {
            using (var client = new ZeroActivityClient())
            {
                var result = client.SelectChosenUserReports(101);

                Assert.IsNotNull(result.Result);
                Assert.IsNotNull(result.Result.FirstOrDefault());
                Assert.AreEqual(result.Result.First().ReportTitle, "europa");
            }
        }

        [TestMethod]
        public void FetchTestReportDetail()
        {
            using (var client = new ZeroActivityClient())
            {
                var result = client.FetchTestReportDetail(3209062);

                Assert.IsNotNull(result.Result);
                Assert.AreEqual(result.Result.ReportTitle, "europa");
                Assert.AreEqual(result.Result.TestReportExtenstionAttribute.CarID, new Guid("{e7f91d52-94af-4167-ada2-929ea5cca3bc}"));
                Assert.AreEqual(result.Result.TestReportExtenstionAttribute.TestEnvironment.DriveSituation, "拥堵");
                Assert.AreEqual(result.Result.TestReportExtenstionAttribute.TestUserInfo.Age, 25);
            }
        }

        [TestMethod]
        public void SelectMyZeroActivityApplications()
        {
            using (var client = new ZeroActivityClient())
            {
                var result = client.SelectMyApplications(new Guid("4F79E798-502C-1891-AFE5-156B452BF4B2"), 1);

                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SubmitZeroActivityApplication()
        {
            try
            {

                using (var client = new ZeroActivityClient())
                {
                    var result = client.SubmitZeroActivityApplication(new ZeroActivityRequest { Period = 116, UserId = new Guid("6FD36988-A872-4C8C-9827-BA945B998C2E"),UserName="kpl", ProvinceID = 1, CityID = 43, CarID = Guid.NewGuid(), ApplicationReason = "我wantceshi" });

                    Assert.IsTrue(result.Result);
                }
            }
            catch (Exception ex)
            {
                var a = ex;
            }
        }

        [TestMethod]
        public void SubmitZeroActivityReminder()
        {
            using (var client = new ZeroActivityClient())
            {
                var result = client.SubmitZeroActivityReminder(new Guid("d67fd5c0-7412-45b2-bb30-742e246b3fcf"), 31);

                Assert.IsTrue(result.Result);
            }
        }
    }
}

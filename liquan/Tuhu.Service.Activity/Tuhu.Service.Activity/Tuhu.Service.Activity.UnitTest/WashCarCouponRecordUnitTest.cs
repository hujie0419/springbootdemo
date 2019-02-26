using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models;
using System.Linq;
using Tuhu.Service.Activity.Models.WashCarCoupon;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class WashCarCouponRecordUnitTest
    {
        [TestMethod]
        public void CreateWashCarCouponAsync()
        {
            using (var client = new WashCarCouponClient())
            {
                WashCarCouponRecordModel request = new WashCarCouponRecordModel()
                {
                    UserID = Guid.Parse("FEEF7C6D-B5B6-41C8-8108-76596514A511"),
                    CarID = Guid.Empty,
                    CarNo = "贵CV1000",
                    VehicleID = "VE-GM-S07BTHRV",
                    Vehicle = "凯越HRV-上海通用别克",
                    PaiLiang = "1.6L",
                    Nian = "2013",
                    Tid = "Tid",
                    PromotionCodeID = 10,
                };
                var result = client.CreateWashCarCoupon(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetWashCarCouponListByUseridsAsync()
        {
            using (var client = new WashCarCouponClient())
            {
                GetWashCarCouponListByUseridsRequest request = new GetWashCarCouponListByUseridsRequest()
                {
                    UserID = Guid.Parse("FEEF7C6D-B5B6-41C8-8108-76596514A510"),
                };
                var result = client.GetWashCarCouponListByUserids(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetWashCarCouponListByPromotionCodeIDAsync()
        {
            using (var client = new WashCarCouponClient())
            {
                GetWashCarCouponInfoByPromotionCodeIDRequest request = new GetWashCarCouponInfoByPromotionCodeIDRequest()
                {
                    PromotionCodeID = 10
                };
                var result = client.GetWashCarCouponInfoByPromotionCodeID(request);
                Assert.IsNotNull(result.Result);
            }
        }

    }
}

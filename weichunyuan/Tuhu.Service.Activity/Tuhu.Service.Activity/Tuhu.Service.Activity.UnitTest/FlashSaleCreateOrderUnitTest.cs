using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Order.Request;
//using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class FlashSaleCreateOrderUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var client = new FlashSaleCreateOrderClient())
            {
                var request =
                    JsonConvert.DeserializeObject<CreateOrderRequest>(
                        "{\"BookDateTime\":\"2018-05-15T00:00:00\",\"BookPeriod\":\"15:00\",\"OrderChannel\":\"WXAPP_PT\",\"OrderDatetime\":\"2018-05-14T16:37:00\",\"OrderId\":0,\"OrderType\":\"60拼团年卡\",\"Status\":\"0NewPingTuan\",\"SumNum\":1,\"Items\":[{\"Category\":\"XNQT\",\"Cost\":0,\"MarkedPrice\":99999,\"ListPrice\":0,\"Name\":\"途虎VIP卡\",\"Num\":1,\"OrderId\":0,\"Pid\":\"AP-HSC-YC-401|1\",\"Price\":45,\"ProductType\":0,\"PurchaseStatus\":0,\"UsePromotionCode\":false,\"ActivityId\":\"84e05c5b-ec4a-4f7f-a113-fe0ecc03f524\",\"IsVerifyActivity\":false,\"NodeNo\":\"558128.880028.615882.615883\",\"IsDaiFa\":false}],\"DeviceID\":\"cfba6c15-5b26-da30-7340-b0905b525ad4\",\"Delivery\":{\"Address\":{\"Pkid\":0,\"ConsigneeName\":\"途虎测试\",\"Cellphone\":\"18801794316\",\"Telphone\":\"18801794316\",\"ProvinceId\":1,\"Province\":\"上海市\",\"CityId\":45,\"City\":\"闵行区\",\"CountyId\":45,\"County\":\"闵行区\",\"Address\":\"闵虹路166弄城开中心1号楼9楼途虎养车前台\",\"CreateDateTime\":\"0001-01-01T00:00:00\",\"LastUpdateDateTime\":\"0001-01-01T00:00:00\",\"UserCreated\":0},\"DeliveryStatus\":\"1NotStarted\",\"InstallType\":\"3NoInstall\"},\"Payment\":{\"PayMothed\":\"fXianShangZhiFu\",\"PayStatus\":\"1Waiting\",\"PaymentType\":\"5Special\",\"PlatformDiscount\":0},\"Money\":{\"ShippingMoney\":0,\"SumDisMoney\":99954,\"SumMarkedMoney\":99999,\"SumMoney\":45},\"Car\":{\"Pkid\":0,\"VehicleId\":\"VE-GM-S07BO\",\"Vehicle\":\"君威-上海通用别克\",\"Brand\":\"B - 别克\",\"PaiLiang\":\"3.0L\",\"Nian\":\"2004\",\"SalesName\":\"2002款 3.0L 自动 旗舰版\",\"LiYangId\":\"\",\"Tid\":\"102154\",\"PlateNumber\":\"\",\"CreateDateTime\":\"0001-01-01T00:00:00\",\"LastUpdateDateTime\":\"0001-01-01T00:00:00\",\"Distance\":0,\"UserCreated\":0},\"Customer\":{\"UserId\":\"21fa116e-5aaf-4dd8-9ee0-c2d93cb026c6\",\"UserName\":\"途虎测试\",\"UserTel\":\"13671714524\"}}");
                var result = client.FlashSaleCreateOrder(request);
                Assert.IsNotNull(result);
                            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (var client = new FlashSaleCreateOrderClient())
            {
                var request =
                    JsonConvert.DeserializeObject<CreateOrderRequest>(
                        "{\"BookDateTime\":\"2018-01-31T00:00:00\",\"BookPeriod\":\"15:00\",\"OrderChannel\":\"WXAPP_PT\",\"OrderDatetime\":\"2018-01-31T11:17:00\",\"OrderId\":0,\"OrderType\":\"50拼团抽奖\",\"Status\":\"0NewPingTuan\",\"SumNum\":1,\"Items\":[{\"Category\":\"PTCJ\",\"Cost\":0.0,\"MarkedPrice\":2999.0000,\"ListPrice\":0.0,\"Name\":\"vivo X20 全面屏手机 全网通 4GB+64GB 移动联通电信4G手机 星耀红 标准版\",\"Num\":1,\"OrderId\":0,\"Pid\":\"XU-PTCJ-HY|2\",\"Price\":0.01,\"ProductType\":0,\"PurchaseStatus\":0,\"UsePromotionCode\":false,\"ActivityId\":\"8448b8a4-cae7-468e-9d4d-72f4171925e7\",\"IsVerifyActivity\":false,\"NodeNo\":\"558128.594531.594544.594545\",\"IsDaiFa\":false}],\"DeviceID\":\"f13629d1-392e-23c0-7192-5d00f74d41a5\",\"Delivery\":{\"Address\":{\"Pkid\":0,\"ConsigneeName\":\"途虎测试\",\"Cellphone\":\"15895659394\",\"Telphone\":\"15895659394\",\"ProvinceId\":723,\"Province\":\"江苏省\",\"CityId\":4,\"City\":\"南京市\",\"CountyId\":725,\"County\":\"玄武区\",\"Address\":\"测试地址测试地址测试地址测试地址测试地址测试地址1234\",\"CreateDateTime\":\"0001-01-01T00:00:00\",\"LastUpdateDateTime\":\"0001-01-01T00:00:00\",\"UserCreated\":0},\"DeliveryStatus\":\"1NotStarted\",\"InstallType\":\"3NoInstall\"},\"Payment\":{\"PayMothed\":\"fXianShangZhiFu\",\"PayStatus\":\"1Waiting\",\"PaymentType\":\"5Special\"},\"Money\":{\"ShippingMoney\":0.0,\"SumDisMoney\":2998.9900,\"SumMarkedMoney\":2999.0000,\"SumMoney\":0.01},\"Customer\":{\"UserId\":\"fc338d4b-4a2f-4948-81f1-da7a46221379\",\"UserName\":\"途虎测试\",\"UserTel\":\"15895659394\"}}");
                var result = client.FlashSaleCreateOrder(request);
                Assert.IsNotNull(result);

            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            using (var client = new FlashSaleCreateOrderClient())
            {
                var request =
                    JsonConvert.DeserializeObject<CreateOrderRequest>(
                        "{\"BookDateTime\":\"2018-03-27T00:00:00\",\"BookPeriod\":\"15:00\",\"OrderChannel\":\"WXAPP_PT\",\"OrderDatetime\":\"2018-03-27T11:04:00\",\"OrderId\":0,\"OrderType\":\"1普通\",\"Status\":\"0NewPingTuan\",\"SumNum\":1,\"Items\":[{\"Category\":\"Camp\",\"Cost\":0.0,\"MarkedPrice\":59.0000,\"ListPrice\":0.0,\"Name\":\"德国TAWA 户外野营绑绳帆布吊床 室内宿舍单人秋千寝室吊床【彩虹】1421  \",\"Num\":1,\"OrderId\":0,\"Pid\":\"AP-TAWA-DC|1\",\"Price\":9.00,\"ProductType\":4,\"PurchaseStatus\":0,\"UsePromotionCode\":false,\"ActivityId\":\"028dd4e1-1ca5-4d92-b706-a4229d5ace67\",\"IsVerifyActivity\":false,\"NodeNo\":\"28349.28392.30833.160907.160908\",\"IsDaiFa\":true}],\"Delivery\":{\"Address\":{\"Pkid\":0,\"ConsigneeName\":\"途虎测试\",\"Cellphone\":\"15000183754\",\"Telphone\":\"15000183754\",\"ProvinceId\":1,\"Province\":\"上海市\",\"CityId\":1,\"City\":\"上海市\",\"CountyId\":35,\"County\":\"黄浦区\",\"Address\":\"途虎\",\"CreateDateTime\":\"0001-01-01T00:00:00\",\"LastUpdateDateTime\":\"0001-01-01T00:00:00\",\"UserCreated\":0},\"DeliveryStatus\":\"1NotStarted\",\"InstallType\":\"3NoInstall\"},\"Payment\":{\"PayMothed\":\"fXianShangZhiFu\",\"PayStatus\":\"1Waiting\",\"PaymentType\":\"5Special\",\"PlatformDiscount\":0.0},\"Money\":{\"ShippingMoney\":0.0,\"SumDisMoney\":50.0000,\"SumMarkedMoney\":59.0000,\"SumMoney\":9.00},\"Car\":{\"Pkid\":0,\"VehicleId\":\"VE-CROSSPOLOSJDZ\",\"Vehicle\":\"CrossPOLO-上海大众\",\"Brand\":\"D - 大众\",\"PaiLiang\":\"1.6L\",\"Nian\":\"2017\",\"SalesName\":\"\",\"LiYangId\":\"\",\"Tid\":\"\",\"PlateNumber\":\"\",\"CreateDateTime\":\"0001-01-01T00:00:00\",\"LastUpdateDateTime\":\"0001-01-01T00:00:00\",\"Distance\":0,\"UserCreated\":0},\"Customer\":{\"UserId\":\"7f73364b-0307-4245-9e95-b5489fe54eef\",\"UserName\":\"途虎测试\",\"UserTel\":\"15000183754\"}}"

                    );
                var result = client.FlashSaleCreateOrder(request);
                Assert.IsNotNull(result);

            }
        }

        [TestMethod]
        public void TestMethod4()
        {
            using (var client = new FlashSaleCreateOrderClient())
            {
                var result = client.FetchActivityProductPrice(new ActivityPriceRequest
                {
                    GroupId = Guid.Empty,
                    UserId = new Guid("e025b1d3-271e-6977-59b3-7fb41dca4cc5"),
                    Items = new List<ActivityPriceItem>
                    {
                        new ActivityPriceItem
                        {
                            ActicityId = new Guid("ba775df4-8d09-4ef9-9bd2-3085956a9c95"),
                            PID = "XU-PTCJ-XX|1"
                        }
                    }
                });
                Assert.IsNull(result.Exception);
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Requests.Activity;

namespace Tuhu.Service.TuboAllianceService.UnitTest
{
    [TestClass]
    public class TuboAllianceUnitTest
    {
        #region CPS

        /// <summary>
        /// 佣金商品列表查询
        /// </summary>
        [TestMethod]
        public void GetCommissionProductListTest()
        {
            using (var client = new TuboAllianceClient())
            {
                var model = new GetCommissionProductListRequest();
                model.IsEnable = 1;
                model.pageIndex = 1;
                model.pageSize = 10;

                var result = client.GetCommissionProductListAsync(model);
                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        /// 佣金商品详情查询
        /// </summary>
        [TestMethod]
        public void GetCommissionProductDetatilsTest()
        {
            using (var client = new TuboAllianceClient())
            {
                var model = new GetCommissionProductDetatilsRequest();
                model.CpsId = new Guid("052e3b8d-5b39-4c5b-8f57-78b7bdffd35a");
                model.PID = "GF-MOBIL-GIFT|18";

                var result = client.GetCommissionProductDetatilsAsync(model);
                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        /// 佣金订单商品记录创建接口
        /// </summary>
        [TestMethod]
        public void CreateOrderItemRecordTest()
        {
            using (var client = new TuboAllianceClient())
            {
                var model = new CreateOrderItemRecordRequest();
                model.OrderId = "123497220";

                var orderItemList = new List<OrderItemRecord>();

                var orderItem = new OrderItemRecord();
                orderItem.CpsId = new Guid("72CF04E7-E99D-4441-80EC-BADCF8DDF953");
                orderItem.DarenID = new Guid("00004371-77D8-4D26-A57E-E46426587BEA");
                orderItem.Pid = "TR-CT-C2-MC5|8";
                orderItem.Number = 1;
                orderItemList.Add(orderItem);

                var orderItem2 = new OrderItemRecord();
                orderItem2.CpsId = new Guid("052E3B8D-5B39-4C5B-8F57-78B7BDFFD35A");
                orderItem2.DarenID = new Guid("00004371-77D8-4D26-A57E-E46426587BEA");
                orderItem2.Pid = "GF-MOBIL-GIFT|18";
                orderItem2.Number = 1;
                orderItemList.Add(orderItem2);

                //var orderItem3 = new OrderItemRecord();
                //orderItem3.CpsId = new Guid("dbcd2b00-e7e9-4d16-8fca-d92736ed48f7");
                //orderItem3.DarenID = new Guid("00010D2E-F4CD-430A-BEB0-AE6674F6EA46");
                //orderItem3.Pid = "LF-GiGi-G-1069|6"; ;
                //orderItem3.Number = 1;
                //orderItemList.Add(orderItem3);

                model.OrderItem = orderItemList;

                var result = client.CreateOrderItemRecordAsync(model);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 订单商品返佣接口
        /// </summary>
        [TestMethod]
        public void CommodityRebateTest()
        {
            using (var client = new TuboAllianceClient())
            {
                var model = new CommodityRebateRequest();
                model.OrderId = "123502852";
                var result = client.CommodityRebateAsync(model);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 订单商品退佣接口
        /// </summary>
        [TestMethod]
        public void CommodityDeductionTest()
        {
            using (var client = new TuboAllianceClient())
            {
                var model = new CommodityDeductionRequest();
                model.OrderId = "123502862";
                var result = client.CommodityDeductionAsync(model);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// CPS支付流水修改状态接口
        /// </summary>
        [TestMethod]
        public void CpsUpdateRunningTest()
        {
            using (var client = new TuboAllianceClient())
            {
                var model = new CpsUpdateRunningRequest();
                model.OutBizNo = "";
                model.TransactionNo = "";
                model.Status = "";
                model.Reason = "";
                var result = client.CpsUpdateRunningAsync(model);
                Assert.IsNotNull(result.Result);
            }
        }

        #endregion
    }
}

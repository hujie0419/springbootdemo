using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class CarFriendsGroupUnitTest
    {

        /// <summary>
        /// 获取车友群列表
        /// </summary>
        [TestMethod]
        public void GetCarFriendsGroupList()
        {
            using(var client=new CarFriendsGroupClient())
            {
                var request = new GetCarFriendsGroupListRequest();
                //var vehicleList = new List<string>();
                //vehicleList.Add("奥迪A3");
                //request.VehicleList = vehicleList;
                //request.SearchVehicleKey = "A8L";
                //request.IsRecommend = true;
                var result=client.GetCarFriendsGroupListAsync(request);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 获取所有热门车型
        /// </summary>
        [TestMethod]
        public void GetRecommendVehicleList()
        {
            using(var client=new CarFriendsGroupClient())
            {
                var result = client.GetRecommendVehicleList();
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 根据pkid获取车友群
        /// </summary>
        [TestMethod]
        public void GetCarFriendsGroupModel()
        {
            using (var client = new CarFriendsGroupClient())
            {
                int pkid = 1;
                var result = client.GetCarFriendsGroupModel(pkid);
                Assert.IsNotNull(result.Result);
            }
        }
        

        /// <summary>
        /// 根据pkid获取途虎管理员信息
        /// </summary>
        [TestMethod]
        public void GetCarFriendsAdministratorsModel()
        {
            using (var client = new CarFriendsGroupClient())
            {
                int pkid = 0;
                var result = client.GetCarFriendsAdministratorsModel(pkid);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 车友群小程序推送车友群或群主信息
        /// </summary>
        [TestMethod]
        public void CarFriendsGroupPushInfo()
        {
            using (var client = new CarFriendsGroupClient())
            {
                var request = new GetCarFriendsGroupPushInfoRequest();
                request.PKID = 1;
                request.InfoType = 1;
                request.UserId = new Guid("3517219E-71FB-4E6E-9E73-92D3413E6223");
                request.OpenId = "o7BMN5MqAXb_3r_47kXAJBhzGVc0";
                var result = client.CarFriendsGroupPushInfo(request);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 调用MQ延迟推送
        /// </summary>
        [TestMethod]
        public void CarFriendsGroupMqDelayPush()
        {
            using (var client = new CarFriendsGroupClient())
            {
                var request = new GetCarFriendsGroupPushInfoRequest();
                request.PKID = 1;
                request.InfoType = 0;
                request.UserId = new Guid("3517219E-71FB-4E6E-9E73-92D3413E6223");
                request.OpenId = "o7BMN5MqAXb_3r_47kXAJBhzGVc0";
                var result = client.CarFriendsGroupMqDelayPush(request);
                Assert.IsNotNull(result.Result);
            }
        }
    }
}

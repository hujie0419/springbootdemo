using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Tuhu;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class BigBrandUnitTest
    {
        [TestMethod]
        public void GetBigBrand()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.GetBigBrand("8513920B");
                result.ThrowIfException(true);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject( result.Result));
            }
        }

        [TestMethod]
        public void UpdateBigBrand()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.UpdateBigBrand("A2E4E6A9");
                result.ThrowIfException(true);
                Console.WriteLine(result.Result?"成功":"失败");
            }
        }

        [TestMethod]
        public void GetPacketTest()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.GetPacket(Guid.Empty, "dfsdfsd", "H5", "1818BFD0", "", "http://wx.tuhu.cn", "oPD5Ns0lDuMghgvyTZwL1q9EADc8");
               
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject( result.Result));
            }
        }

        [TestMethod]
        public void ShareAddTest()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.ShareAddOne(Guid.Parse("{0052aa43-5b14-4fb8-ac8b-c5f78fcb2d64}"), "dfsdfsd", "H5", "A2E4E6A9", "18037108212", "http://wx.tuhu.cn");

                Console.WriteLine(result.Exception);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        [TestMethod]
        public async Task GetSelectPacketTest()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = await client.SelectCanPackerAsync(Guid.Empty, "dfsdfsd", "H5", "1818BFD0", "", "http://wx.tuhu.cn", "oPD5Ns51t9l2ZfETpXmHVYVbxGfs");
                result.ThrowIfException(true);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        [TestMethod]
        public void SelectShareList()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.SelectShareList(Guid.Parse("{0052aa43-5b14-4fb8-ac8b-c5f78fcb2d64}"),"1818BFD0", 3);
                result.ThrowIfException(true);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        /// <summary>
        /// 推荐有礼测试
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetSelectPacketTestByRecord()
        {

            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                //var result = client.AddBigBrandTimes(Guid.Parse("2088993c-e721-4686-9f25-fd257ee9d4d3"), "", DateTime.Now.ToString("yyyyMMddHHmmss"), "DCF3DF88", "", "GroupByingAddBigBrandTimesConsumer", 1);
                var result = await client.SelectCanPackerAsync(Guid.Parse("2088993c-e721-4686-9f25-fd257ee9d4d3"), "dfsdfsd", "H5", "DCF3DF88", "18037108212", "http://wx.tuhu.cn");
                result.ThrowIfException(true);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result, Newtonsoft.Json.Formatting.Indented));
            }
        }

        [TestMethod]
        public void GetPackRealTest()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.GetPacket(Guid.Empty, "dfsdfsd", "H5", "1818BFD0", "18037108212", "http://wx.tuhu.cn", "oPD5Ns0lDuMghgvyTZwL1q9EADc8");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

       

        [TestMethod]
        public void IsNullSelectRealPack()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.IsNULLBigBrandRealByAddress("AEE792AF", Guid.Parse("16c2921e-adf2-433c-b693-92d5bb6a1c3d"), "18037108212", "dfsdfsd", "H5");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        [TestMethod]
        public void UpdateRealPackAddress()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.UpdateBigBrandRealLog("AEE792AF", Guid.Parse("5010BDED-8492-4F5D-8016-DA28ED34A623"),"上海市闵行区莲花南路与江淮路交叉口向东2000米路西*********",Guid.Parse("CC5CC7F6-43F3-47B5-BA73-B0008E55846F"), "18037108212", "dfsdfsd", "H5","姓名");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        [TestMethod]
        public void ShareAddByOrder()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.ShareAddByOrder(Guid.Parse("2088993c-e721-4686-9f25-fd257ee9d4d3"), "", "拼团增加抽奖次数", "DCF3DF88", "", "GroupByingAddBigBrandTimesConsumer", 1);
                result.ThrowIfException(true);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }
        [TestMethod]
        public void AddBigBrandTimes()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.AddBigBrandTimes(Guid.Parse("2088993c-e721-4686-9f25-fd257ee9d4d3"), "", DateTime.Now.ToString("yyyyMMddHHmmss"), "DCF3DF88", "", "GroupByingAddBigBrandTimesConsumer", 1);
                result.ThrowIfException(true);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        /// <summary>
        /// 获取用户答题试卷
        /// </summary>
        [TestMethod]
        public void GetQuestionListByUser()
        {
            using (var client = new BigBrandClient())
            {
                var result = client.GetQuestionList(Guid.Parse("{63d05ec6-4a39-49db-b830-b374390fca5f}"), "A9FD4677");
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }

        [TestMethod]
        public void UpdateQuestion_Test()
        {
            using (var client = new BigBrandClient())
            {
                var result = client.GetQuestionList(Guid.Parse("{63d05ec6-4a39-49db-b830-b374390fca5f}"), "A9FD4677");
                //Guid.Parse("{63d05ec6-4a39-49db-b830-b374390fca5f}"), result.Result[4].PKID, "A"
                var results = client.SetAnswerRes( new Models.Requests.QuestionAnsRequestModel() {
                     hashKey= "A9FD4677",
                     pkid= result.Result[3].PKID,
                     userId= Guid.Parse("{63d05ec6-4a39-49db-b830-b374390fca5f}"),
                     resOptions="C"
                });
                Console.WriteLine(JsonConvert.SerializeObject(results.Result));
            }
        }

        [TestMethod]
        public void GetPackQuest_Test()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.GetAnswerPacket(Guid.Parse("{0013eeed-2c14-43ff-a859-c2dc71c65def}"), "dfsdfsd", "H5", "7543DFC0", "18037108212", "http://wx.tuhu.cn");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Result));
            }
        }

        #region 分享红包组
        /// <summary>
        /// 获取红包组信息
        /// </summary>
        [TestMethod]
        public void FilghtGroupsList_Test()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {

               var result =  client.GetFightGroupsPacketsList(null,Guid.Parse("{63d457cd-8b83-4901-893a-98a6656dd35f}"));
                result.ThrowIfException(true);
                Assert.IsNotNull(result.Result);
                if (result.Result != null)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(result.Result, new JsonSerializerSettings() { Formatting = Formatting.Indented }));

                }
            }
        }

        /// <summary>
        /// 创建红包组
        /// </summary>
        [TestMethod]
        public void InserFilghtGroups_Test()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                Guid userId = Guid.Parse("{63d457cd-8b83-4901-893a-98a6656dd35f}");
                var result = client.InsertFightGroupsPacket(userId);
                result.ThrowIfException(true);
                Assert.IsNotNull(result.Result);
                if (result.Result != null)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(result.Result,new JsonSerializerSettings() { Formatting= Formatting.Indented }));
                   
                }
            }
        }

        /// <summary>
        /// 更新红包用户
        /// </summary>
        [TestMethod]
        public void UpdateFilghtGroupsByUserId_Test()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.UpdateFightGroupsPacketByUserId(new Models.Requests.FightGroupsPacketsUpdateRequest()
                {
                    FightGroupsIdentity = Guid.Parse("d7244c17-c16c-400c-b84c-0814a001f02c"),
                    PKID= 418,
                    UserId=Guid.Parse("{009d858b-bda0-4fed-9655-2ce34b704cc9}")
                });
                 result = client.UpdateFightGroupsPacketByUserId(new Models.Requests.FightGroupsPacketsUpdateRequest()
                {
                    FightGroupsIdentity = Guid.Parse("d7244c17-c16c-400c-b84c-0814a001f02c"),
                    PKID = 419,
                    UserId = Guid.Parse("{00a076fa-1d78-4860-9b33-d4e6af057977}")
                });
                 result = client.UpdateFightGroupsPacketByUserId(new Models.Requests.FightGroupsPacketsUpdateRequest()
                {
                    FightGroupsIdentity = Guid.Parse("d7244c17-c16c-400c-b84c-0814a001f02c"),
                    PKID = 420,
                    UserId = Guid.Parse("{00a475c9-7d24-48f8-b151-e379b8677613}")
                });

                result.ThrowIfException(true);
                Assert.AreEqual(result.Result,true);
            }
        }

        /// <summary>
        /// 创建优惠券组
        /// </summary>
        [TestMethod]
        public void CreateFilghtGroupsPromotion_Test()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.CreateFightGroupsPacketByPromotion(Guid.Parse("d7244c17-c16c-400c-b84c-0814a001f02c"));
                result.ThrowIfException(true);
                Console.WriteLine(result.Result.Msg);
                Assert.AreEqual(result?.Result.IsSuccess, true,result.Result.Msg);
            }
        }

        #endregion


        public class User
        {
           public Guid UserID { get; set; }
        }


        #region 完整测试
        /// <summary>
        /// 获取没有抽过奖励的用户UserId
        /// </summary>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        private List<User> GetUserIds(int pkid,int number)
        {
            if (number <= 0)
                number = 1;

            string sql = @"	SELECT TOP "+number+@" UserID FROM  Tuhu_profiles.dbo.UserObject WITH (NOLOCK) WHERE  IsActive=1 AND UserID NOT IN (

        SELECT UserId FROM SystemLog.dbo.BigBrandRewardLog WITH (NOLOCK) WHERE FKBigBrandPkid=@PKID AND ChanceType=1)";
           
            using (var db = DbHelper.CreateDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@PKID",pkid);
                return db.ExecuteSelect<User>(cmd)?.ToList();
               
            }
               
        }

        [TestMethod]
        public void Finish()
        {
            string hashKey = "A2E4E6A9";
            int Total = 0;//总人数
            int TotalPack = 0;//抽奖总数
            int TotalShare = 0;//分享总数

            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var bigBrand = client.GetBigBrand(hashKey);
                Assert.IsNotNull(bigBrand.Result, "hashKey不存在");
                List<User> userIds = GetUserIds(bigBrand.Result.PKID, 20);
                Total = userIds.Count;
                if (userIds != null)
                    foreach (var item in userIds)
                    {
                        var userId = item.UserID;
                        var selectResult = client.SelectCanPacker(userId, "deviceId", "测试", bigBrand.Result.HashKeyValue, "18037108212", "Tuhu.Service.Activity.Server.UnitTest.Finish");
                        selectResult.ThrowIfException(true);
                        if (selectResult.Success)
                        {
                            if (selectResult.Result.Code == 1)
                            {
                                for (int i = 0; i < selectResult.Result.Times; i++)
                                {
                                    var result = client.GetPacket(userId, "deviceId", "测试", bigBrand.Result.HashKeyValue, "18037108212", "Tuhu.Service.Activity.Server.UnitTest.Finish");
                                    result.ThrowIfException(true);
                                    if (result.Result.TimeCount <= 0 && selectResult.Result.IsShare == false)
                                    {
                                        TotalPack++;
                                        var shareResult = client.ShareAddOne(userId, "deviceId", "测试", bigBrand.Result.HashKeyValue, "18037108212", "Tuhu.Service.Activity.Server.UnitTest.Finish");
                                        if (shareResult.Result)
                                        {
                                            TotalShare++;
                                          var selectResult1 = client.SelectCanPacker(userId, "deviceId", "测试", bigBrand.Result.HashKeyValue, "18037108212", "Tuhu.Service.Activity.Server.UnitTest.Finish");
                                            #region
                                            for (int n = 0; n < selectResult1.Result.Times; n++)
                                            {
                                                var result1 = client.GetPacket(userId, "deviceId", "测试", bigBrand.Result.HashKeyValue, "18037108212", "Tuhu.Service.Activity.Server.UnitTest.Finish");
                                                result1.ThrowIfException(true);
                                                if (result1.Result.Code == 1)
                                                    TotalPack++;
                                            }
                                            #endregion
                                        }

                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{userId} 不能抽奖的原因:{selectResult.Result.Msg}");
                            }
                        }


                    }

                Console.WriteLine($"参加抽奖的总数：{Total}");
                Console.WriteLine($"抽中奖励的总数:{TotalPack}");
                Console.WriteLine($"分享加一次抽奖的总数:{TotalShare}");
            }


        }

       
        #endregion

    }
}

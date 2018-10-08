using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.Business;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class LuckyWheelController : Controller
    {
        //
        // GET: /LuckyWheel/

        public ActionResult List()
        {
            List<LuckyWheel> list = LuckyWheelManager.GetTableList("");
            return View(list);
        }


        public ActionResult WheelList(string id = "")
        {
            Guid guid;
            if (Guid.TryParse(id, out guid))
            {
                LuckyWheel model = LuckyWheelManager.GetEntity(guid.ToString());
                return View(model);
            }
            else
                return View(new LuckyWheel());
        }


        public ActionResult Row(string json = "")
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                return View(JsonConvert.DeserializeObject<LuckyWheelDeatil>(json));
            }
            else
                return View(new LuckyWheelDeatil());
        }


        public ActionResult Edit(string json = "")
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                return View(JsonConvert.DeserializeObject<LuckyWheelDeatil>(json));
            }
            else
                return View(new LuckyWheelDeatil() { GetDescription = "", GoDescription = "" });
        }


        public ActionResult Save(string json)
        {
            JObject jObj = new JObject();
            if (!string.IsNullOrWhiteSpace(json))
            {
                LuckyWheel wheel = JsonConvert.DeserializeObject<LuckyWheel>(json);
                if (wheel != null)
                {
                    wheel.UpdateDate = DateTime.Now;
                    wheel.CreatorUser = User.Identity.Name;
                    wheel.UpdateUser = User.Identity.Name;
                    bool result = LuckyWheelManager.Save(wheel);
                    if (result)
                    {
                        using (var client = new Tuhu.Service.Activity.ActivityClient())
                        {
                            var clientReuslt = client.RefreshLuckWheelCache(wheel.ID.Trim());
                            clientReuslt.ThrowIfException(true);
                        }
                    }
                    jObj.Add("result", result == true ? 1 : 0);
                }
            }
            else
            {
                jObj.Add("result", 0);
            }

            return Json(jObj.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            JObject json = new JObject();
            json.Add("result", LuckyWheelManager.Delete(id) == true ? 1 : 0);
            return Json(json.ToString());
        }

        private static object obj = new Object();

       
        public void TestGetPack()
        {

            string url = "http://wx.tuhu.dev/LuckyWheel/GetPacket";
           
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(@"data source=172.16.20.1\dev;Initial Catalog=Gungnir;User ID=gungnirreader;Password=itsme999"))
            {
                try
                {
                    conn.Open();
                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SELECT DISTINCT u_user_id FROM Tuhu_profiles.dbo.UserObject (NOLOCK)", conn);
                    System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    System.Data.DataSet ds = new System.Data.DataSet();
                    da.Fill(ds);
                    
                    for (int i=0;i<ds.Tables[0].Rows.Count;i++)
                    {
                        string user = ds.Tables[0].Rows[i][0].ToString();
                        Console.WriteLine("读取的用户UserID:" + user);
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        IEnumerable<KeyValuePair<string, string>> form = new List<KeyValuePair<string, string>>
                    {
                         new KeyValuePair<string, string>("UserID",user),
                         new KeyValuePair<string, string>("LuckyWheel","5948b149-c27f-457b-bb70-7ed629ee99d6"),
                         new KeyValuePair<string, string>("deviceId",""),
                         new KeyValuePair<string, string>("Channal","auto")
                    };
                        System.Net.Http.HttpContent content = new FormUrlEncodedContent(form);

                        var result = client.PostAsync(url, content).Result;

                        if (result.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("请求失败:" + result.StatusCode);
                        }
                        else
                        {
                            string responseContent = result.Content.ReadAsStringAsync().Result;
                           // JObject json = JObject.Parse(responseContent);
                            //if (json["Code"].ToString() == "1")
                            //{
                            //    Console.WriteLine("领取成功,收到的券：" + json["GetDescription"] + " 券ID: " + json["Coupon"]);
                            //}
                            Console.WriteLine("返回的数据:" + responseContent);
                            Console.WriteLine("----------------------------------------------------------------");
                            Console.WriteLine();
                        }
                        if (i == 10000)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
               
            }
        }


    }
}

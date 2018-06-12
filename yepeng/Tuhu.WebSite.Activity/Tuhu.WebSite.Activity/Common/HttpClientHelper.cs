using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Nosql;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.WebSite.Component.Activity.Business;
using Tuhu.WebSite.Component.Discovery.Business;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity.Common
{
    public class HttpClientHelper
    {
        public static HttpClient client;
        public static HttpClientHandler clientHandler;
        static HttpClientHelper()
        {   System.Net.ServicePointManager.DefaultConnectionLimit = Int16.MaxValue;
            //clientHandler = new HttpClientHandler();
            //client = new HttpClient();
        }
        public  static string AcccessApi(HttpRequestBase httpRequest, string apiUrl, Dictionary<string, string> parameters)
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            HttpWebRequest Request = WebRequest.Create("https://api.tuhu.cn" + apiUrl) as HttpWebRequest;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Timeout = 30000;
            Request.KeepAlive = true;

            List<string> Parameter = new List<string>(parameters.Count);
            foreach (KeyValuePair<string, string> item in parameters)
            {
                Parameter.Add(String.Concat(item.Key, "=", item.Value));
            }
            string PostData = String.Join("&", Parameter.ToArray());

            byte[] PostBytes = Encoding.UTF8.GetBytes(PostData);
            Request.ContentLength = PostBytes.Length;
            System.IO.Stream RequestStream = Request.GetRequestStream();
            RequestStream.Write(PostBytes, 0, PostBytes.Length);
            System.IO.Stream ResponseStream = Request.GetResponse().GetResponseStream();
            System.IO.StreamReader ResponseStreamReader = new System.IO.StreamReader(ResponseStream, Encoding.UTF8);
            return ResponseStreamReader.ReadToEnd();
            
            //using (var webClient = new WebClient())
            //{
               
            //    string postString = string.Join("&", parameters.Select(kv => kv.Key + "=\"" + kv.Value + "\""));
            //    byte[] postData = Encoding.UTF8.GetBytes(postString);
            //    string url = "http://api.tuhu.cn"+ apiUrl;
            //    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //    byte[] responseData = webClient.UploadData(url, "POST", postData);
            //    string srcString = Encoding.UTF8.GetString(responseData);
            //    return srcString;
            //}
        }
        public static async Task<List<Controllers.MyAttentionUserList>> SelectUserInfo(string userIds)
        {
            using (var dbherple = new Component.SystemFramework.SqlDbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["GungnirUser"].ConnectionString))
            {
                List<Dictionary<string, string>> userInfos = new List<Dictionary<string, string>>();
                var userArr = userIds.Split(',');
                StringBuilder userBuilder = new StringBuilder();
                Array.ForEach(userArr, uid =>
                {
                    uid = "'" + uid + "',";
                    userBuilder.Append(uid);
                });
                userIds = userBuilder.ToString();
                userIds = userIds.Substring(0, userIds.Length - 1);
                var dt = await dbherple.ExecuteDataTableAsync(@"SELECT    u_user_id ,
                                                                            u_pref5 as NickName,
                                                                            UOI.UserGrade,
                                                                            u_Imagefile,
                                                                u_mobile_number,
                                                                            ISNULL((SELECT TOP 1
                                                                                            [Vehicle]
                                                                                     FROM[Tuhu_profiles].[dbo].[CarObject] WITH(NOLOCK)
                                                                                     WHERE  u_user_id = UO.u_user_id
                                                                                            AND IsDefaultCar = 1
                                                                                   ), '') AS VEHICLE
                                                                  FROM[Tuhu_profiles].[dbo].[UserObject] AS UO WITH(NOLOCK)
                                                                            LEFT JOIN[Tuhu_profiles].dbo.UserObjectInfo UOI WITH(NOLOCK) ON UO.UserID = UOI.UserID
                                                                                                                                    WHERE UO.u_user_id IN(" + userIds + ")");

                return dt.Rows.OfType<DataRow>().Select(row => new Controllers.MyAttentionUserList()
                {
                    Vehicle = (string)row["vehicle"],
                    UserName = Controllers.ArticleController.GetUserName(row["NickName"] == DBNull.Value? (string)row["u_mobile_number"] : row["NickName"].ToString()),
                    UserHead = row["u_Imagefile"].ToString(),
                    VehicleImageUrl = string.Concat(DomainConfig.ImageSite, ImageHelper.GetLogoUrlByName((string)row["vehicle"])),
                    UserId = row["u_user_id"].ToString().Trim(),
                    PhoneNumber = (string)row["u_mobile_number"],
                    UserGrade = row["UserGrade"].ToString()
                }).ToList();
            }

        }

        public static  UserObjectModel SelectUserInfoByUserId(string userId)
        {
            Guid uid;
            if (!Guid.TryParse(userId, out uid))
            {
                return null;
            }
            using (var reclient = CacheHelper.CreateCacheClient("ArticleUserInfo"))
            {
                var result = reclient.GetOrSet($"InfoByUserId/{userId}", () => SelectUserInfoByUserIdAsync(userId), TimeSpan.FromHours(1));
                return result.Value;
            }
        }

        public static  UserObjectModel SelectUserInfoByUserIdAsync(string userId)
        {
            
            using (var userClient = new UserClient())
            {
                var user =  userClient.FetchUserByUserId(userId);
                user.ThrowIfException(true);
                return user.Result;
            }
        }

        public static async Task<bool> ForumSynchronousData(string userHeard,string phone,string userName,string userId,string forumId,string conent,string commentImage,int userIdentity,int commentId,int topicCommentSouceId)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            //var sign = SecurityHelper.Hash($"lietome@2017!{timestamp}", Encoding.UTF8, true);
            var sign = SecurityHelper.GetHashAlgorithm(HashType.Md5).ComputeHash($"lietome@2017!{timestamp}", Encoding.UTF8, true);
            using (var client = new HttpClient())
            {

                try
                {
                    var result =await client.PostAsFormAsync<string>($"https://hushuoapi.tuhu.cn/v1/replies/sync?timestamp={timestamp}&sign={sign}",
                    new SortedDictionary<string, string>
                    {
                        ["avatar"] = userHeard,
                        ["mobile"] = phone,
                        ["tuhu_name"] = userName,
                        ["tuhu_userid"] = userId,
                        ["topic_id"] = forumId,
                        ["body"] = conent,
                        ["image_urls"] = commentImage,
                        ["tuhu_user_type"] = userIdentity.ToString(),
                        ["jishi_comment_id"] = commentId.ToString(),
                        ["source_id"] = topicCommentSouceId.ToString()
                    });
                    var jObj=Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(result);
                    if (jObj.Value<int>("code") == 1)
                    {
                        var replyId = jObj.Value<int>("replyId");
                        if (replyId > 0)
                        {
                            await DiscoverBLL.InsertTopicCommentSouceIdById(commentId, replyId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                }
                return true;
            }
        }
    }


}

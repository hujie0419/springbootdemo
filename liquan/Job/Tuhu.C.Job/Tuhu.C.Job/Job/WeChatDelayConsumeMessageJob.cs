using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class WeChatDelayConsumeMessageJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WeChatDelayConsumeMessageJob));
        private static string WeiXinApi = "https://wx.tuhu.cn";
        private const string URL_CUSTOM_SEND = "https://api.weixin.qq.com";//发送客服消息的地址
        public static Dictionary<string, int> Flatform = new Dictionary<string, int>()
        {
            ["gh_14d1902086bc"] = 0, //途虎养车
            ["gh_155524e3c1f5"] = 9, //新汽车志
            ["gh_f45d2fdb7813"] = 10 //养车开发版
        };
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("启动任务");
                var dt = GetDelayConsumeMessageData();
                Logger.Info($"获取到 {dt.Rows.Count} 条数据");

                foreach (DataRow row in dt.Rows)
                {
                    var token = GetWeiXinToken(row.GetValue<string>("OriginalID"));
                    if (!string.IsNullOrEmpty(token))
                    {
                        var type = row.GetValue<string>("MediaType");
                        if (type == "news") //如果是图文消息，转换为mpnews
                            type = "mpnews";
                        dynamic jsonObj = new ExpandoObject();
                        jsonObj.touser = row.GetValue<string>("OpenID");
                        jsonObj.msgtype = type;
                        if (type == "text")
                        {
                            ((IDictionary<string, object>) jsonObj).Add(type, new
                            {
                                content = row.GetValue<string>("Content")
                            });
                        }
                        else
                        {
                            ((IDictionary<string, object>)jsonObj).Add(type, new
                            {
                                media_id = row.GetValue<string>("MediaID")
                            });
                        }
                        if (!PushConsumeMessage(token, jsonObj))
                        {
                            Logger.Info($"{row.GetValue<int>("PKID")} 发送客服消息失败");
                        }
                        UpdateDelayConsumeMessage(row.GetValue<int>("PKID"));
                    }
                    else
                    {
                        Logger.Info("token is empty");
                    }
                }

                Logger.Info("任务结束");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        string GetWeiXinToken(string originalId)
        {
            int platform = -1;
            Flatform.TryGetValue(originalId, out platform);
            var client = new RestSharp.RestClient(WeiXinApi);
            var request = new RestSharp.RestRequest("packet/test?platform="+ platform, RestSharp.Method.GET);
            request.AddHeader("access_token", "access_token");
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                try
                {
                    var model = JsonConvert.DeserializeObject<WeixinApiResponse>(response.Content);
                    if (model != null)
                    {
                        return model.access_token;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("GetWeiXinToken", e);
                }

            }
            return string.Empty;
        }


        /// <summary>
        /// 获取需要延迟发送的人和素材
        /// </summary>
        /// <returns></returns>
        public DataTable GetDelayConsumeMessageData()
        {
            using (var cmd =
                new SqlCommand(
                    @"SELECT L.PKID,L.OpenID,M.MediaID,M.OriginalID,M.MediaType,T.Content,L.SendTime FROM Configuration..WXClickMenuLog AS L WITH(NOLOCK)
                        JOIN Configuration..WXMenuMaterialMapping M WITH(NOLOCK) ON L.ObjectID=M.ObjectID AND L.ObjectType=m.ObjectType
                        LEFT JOIN Configuration..WXMaterialText AS T WITH(NOLOCK) ON M.MaterialID=T.PKID AND T.IsDeleted=0
                        WHERE L.SendTime<=@SendTime AND L.IsDeleted=0 AND M.IsDeleted=0 AND M.IsDelaySend=1 AND L.IsSend=0"))
            {
                cmd.Parameters.AddWithValue("@SendTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:59"));
                return DbHelper.ExecuteQuery(true, cmd, dt => dt);
            }
        }

        public void UpdateDelayConsumeMessage(int pkid)
        {
            using (var cmd =
                new SqlCommand("UPDATE Configuration..WXClickMenuLog WITH(ROWLOCK) SET IsSend=1 WHERE PKID=@PKID"))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public bool PushConsumeMessage(string token,dynamic jsonObject)
        {
            var client = new RestSharp.RestClient(URL_CUSTOM_SEND);
            var request = new RestSharp.RestRequest($"cgi-bin/message/custom/send?access_token={token}", RestSharp.Method.POST);
            request.AddJsonBody(jsonObject);
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                try
                {
                    var model = JsonConvert.DeserializeObject<ConsumeMessageResponse>(response.Content);
                    if (model.errcode == 0) return true;
                    Logger.Info(
                        $"PushConsumeMessage Faild errorcode = {model.errcode} errormsg={model.errmsg} body={JsonConvert.SerializeObject(jsonObject)}");
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error("PushConsumeMessage Error", e);
                }
            }
            return false;
        }


        public class WeixinApiResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public class ConsumeMessageResponse
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }
        }
    }
}

using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.C.ActivityJob.Common;
using Tuhu.C.ActivityJob.Models.Monitor;
using Newtonsoft.Json;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 活动服务代理
    /// </summary>
    public class ActivcityServiceProxy
    {
        private static readonly HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("http://localhost:8081/") };
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AMapServiceProxy));


        /// <summary>
        /// 根据区域自动通过活动申请
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> AutoPassUserActivityApply()
        {
            bool nextFlag = true;
            var getDataPath = "/Promotion/Activity/GetAutoPassUserActivityApplyPKIDs";
            var batchPassPath = "/Promotion/Activity/BatchPassUserActivityApplyByPKIDs";
            try
            {
                HttpClient.DefaultRequestHeaders.Add("RequestID", "799385b17308425a9d02e2237fc57501");
                HttpClient.DefaultRequestHeaders.Add("RemoteName", "Tuhu.Service.Promotion.Server");
                HttpClient.DefaultRequestHeaders.Add("RemoteEndpoint", "TH201969521");

                int minPKID = 0;
                while (nextFlag)
                {
                    var getDataRequestModel = new GetAutoPassUserActivityApplyRequest()
                    {
                        AreaIDs = "7",
                        minPKID = minPKID,
                        CurrentPage = 1,
                        PageSize = 50
                    };
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(getDataRequestModel));
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var getDataResponse = await HttpClient.PostAsync(getDataPath, content).ConfigureAwait(false);
                    if (getDataResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var responseError = getDataResponse.Content.ReadAsStringAsync();
                        Logger.Error($"获取根据区域自动通过活动申请数据失败：{responseError}");
                    }
                    var getDataResult = (await getDataResponse.Content.ReadAsAMapAsync<GetAutoPassUserActivityApplyResponse>());
                    if (getDataResult == null || getDataResult?.Result.Count == 0)
                    {
                        nextFlag = false;
                        break;
                    }
                    if (getDataResult?.Result.Count < 50)
                    {
                        nextFlag = false;
                    }
                    string jsonData = JsonConvert.SerializeObject(getDataResult.Result);
                    HttpContent httpContent = new StringContent(jsonData.Replace(",","],["));
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var batchPassResponse = await HttpClient.PostAsync(batchPassPath, httpContent).ConfigureAwait(false);
                    if (batchPassResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var responseError = getDataResponse.Content.ReadAsStringAsync();
                        Logger.Error($"批量通过活动申请调接口失败：{responseError}");
                    }
                    var result = (await batchPassResponse.Content.ReadAsAMapAsync<BatchPassUserActivityApplyResponse>());
                    if(!result.Result)
                    {
                        Logger.Warn($"批量通过活动申请更新失败,参数：{jsonData}");
                    }
                    minPKID = getDataResult.Result.Max();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"根据区域自动通过活动申请异常", ex);
                return false;
            }

        }
    }

    public class GetAutoPassUserActivityApplyResponse
    {
        public bool Success { get; set; }
        public List<int> Result { get; set; }
    }

    public class BatchPassUserActivityApplyResponse
    {
        public bool Success { get; set; }
        public bool Result { get; set; }
    }

    public class GetAutoPassUserActivityApplyRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 页长
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 区域ID，逗号隔开
        /// </summary>
        public string AreaIDs { get; set; }
        /// <summary>
        /// 提高查询速度，传入上一次查询的最大PKID未本次最小PKID
        /// </summary>
        public int minPKID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using Tuhu.Service.Config;
using Tuhu.Service.Product;
using Tuhu.Service.UserAccount;
using Tuhu.Service.Vehicle;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class ProductCommentToTopicJob : IJob
    {
        private string TopicAddUrl = ConfigurationManager.AppSettings["TopicAddUrl"];
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductCommentToTopicJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info($"ProductCommentToTopicJob 开始执行导入");

                var dataMap = context.JobDetail.JobDataMap;

                var type = dataMap.GetInt("Type");
                switch (type)
                {
                    case 1:
                        ImportProductComment();
                        break;
                    case 2:
                        ImportOfficialComment();
                        break;
                    case 3:
                        ImportAdditionComment();
                        break;
                }

                Logger.Info($"ProductCommentToTopicJob 导入完毕");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        void ImportProductComment()
        {
            int minPkid = int.MaxValue;
            int maxPkid = int.MaxValue;
            using (var client = new ConfigClient())
            {
                var response = client.GetOrSetRuntimeSwitch("ProductCommentToTopicJob");
                if (response.Success && (response.Result?.Value ?? false))
                {
                    var pkidArr = response.Result.Description.Split(',');
                    if (pkidArr.Length == 2)
                    {
                        int.TryParse(pkidArr[0], out minPkid);
                        int.TryParse(pkidArr[1], out maxPkid);
                    }
                }
            }
            int current = 0;
            int pageSize = 500;
            int pageIndex = 1;
            int totalCount = GetCommentsCount(minPkid, maxPkid);
            int pageTotal = (totalCount - 1) / pageSize + 1;
            while (pageIndex <= pageTotal && totalCount > 0)
            {
                pageIndex++;

                var comments = GetComments(minPkid, maxPkid, pageSize);
                Logger.Info(
                    $"ImportProductComment {current}/{totalCount} minPkid={minPkid} 总共获取到 {comments.Rows.Count}条数据");
                if (comments.Rows.Count == 0) break;
                int tempMinPkid = minPkid;
                minPkid = comments.Rows[comments.Rows.Count - 1].GetValue<int>("CommentId");
                Logger.Info(
                    $"ImportProductComment {current}/{totalCount} minPkid={tempMinPkid} maxPkid={minPkid} 开始导入");
                Stopwatch watcher = new Stopwatch();
                watcher.Start();
                var userIds = new List<Guid>(comments.Rows.Count);
                var pids = new List<string>(comments.Rows.Count);
                foreach (DataRow row in comments.Rows)
                {
                    userIds.Add(row.GetValue<Guid>("CommentUserId"));
                    pids.Add(row.GetValue<string>("CommentProductId"));
                }
                var mobilesDic =
                    new Dictionary<Guid, string>(comments.Rows.Count);
                //批量获取用户手机号
                using (var client = new UserAccountClient())
                {
                    var response = client.GetUsersByIds(userIds);
                    if (response.Success)
                    {
                        foreach (var item in response.Result)
                        {
                            mobilesDic[item.UserId] = item.MobileNumber;
                        }
                    }
                }

                //批量获取产品数据
                var productDic =
                    new Dictionary<string, Service.Product.Models.New.SkuProductDetailModel>(comments.Rows.Count);
                using (var client = new ProductClient())
                {
                    var response = client.SelectSkuProductListByPids(pids);
                    if (response.Success && response.Result.Any())
                    {
                        foreach (var item in response.Result)
                        {
                            productDic[item.Pid] = item;
                        }
                    }
                }
                //批量获取默认车型数据
                var vehicleDt = GetDefaultVehicleByUserIds(userIds);
                var vehicleDic = new Dictionary<Guid, string>(comments.Rows.Count);
                foreach (DataRow row in vehicleDt.Rows)
                {
                    vehicleDic[row.GetValue<Guid>("UserId")] = row.GetValue<string>("VehicleId");
                }

                Parallel.ForEach(comments.Rows.OfType<DataRow>(),
                    new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                    item =>
                    {
                        AddTopic(item, mobilesDic, productDic, vehicleDic);
                    });
                watcher.Stop();
                current += pageSize;
                Logger.Info(
                    $"ImportProductComment {current}/{totalCount} minPkid={tempMinPkid} maxPkid={minPkid} 导入完毕 用时{watcher.ElapsedMilliseconds}");


            }
        }

        void ImportOfficialComment()
        {
            int minPkid = int.MaxValue;
            int maxPkid = int.MaxValue;
            using (var client = new ConfigClient())
            {
                var response = client.GetOrSetRuntimeSwitch("ProductCommentToTopicJob");
                if (response.Success && (response.Result?.Value ?? false))
                {
                    var pkidArr = response.Result.Description.Split(',');
                    if (pkidArr.Length == 2)
                    {
                        int.TryParse(pkidArr[0], out minPkid);
                        int.TryParse(pkidArr[1], out maxPkid);
                    }
                }
            }
            int current = 0;
            int pageSize = 500;
            int pageIndex = 1;
            int totalCount = GetOfficialCommentsCount(minPkid, maxPkid);
            int pageTotal = (totalCount - 1) / pageSize + 1;
            while (pageIndex <= pageTotal)
            {
                pageIndex++;

                var comments = GetOfficialComments(minPkid, maxPkid, pageSize);
                Logger.Info(
                    $"ImportOfficialComment {current}/{totalCount} minPkid={minPkid} 总共获取到 {comments.Rows.Count}条数据");
                if (comments.Rows.Count == 0) break;
                int tempMinPkid = minPkid;
                minPkid = comments.Rows[comments.Rows.Count - 1].GetValue<int>("CommentId");
                Logger.Info(
                    $"ImportOfficialComment {current}/{totalCount} minPkid={tempMinPkid} maxPkid={minPkid} 开始导入");
                Stopwatch watcher = new Stopwatch();
                watcher.Start();
                var userIds = new List<Guid>(comments.Rows.Count);
                foreach (DataRow row in comments.Rows)
                {
                    userIds.Add(row.GetValue<Guid>("CommentUserId"));
                }
                var mobilesDic =
                    new Dictionary<Guid, string>(comments.Rows.Count);
                //批量获取用户手机号
                using (var client = new UserAccountClient())
                {
                    var response = client.GetUsersByIds(userIds);
                    if (response.Success)
                    {
                        foreach (var item in response.Result)
                        {
                            mobilesDic[item.UserId] = item.MobileNumber;
                        }
                    }
                }

                Parallel.ForEach(comments.Rows.OfType<DataRow>(),
                    new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                    item =>
                    {
                        AddReply(Guid.Empty, string.Empty,
                            item.GetValue<int>("CommentId"), item.GetValue<string>("OfficialReply"),
                            string.Empty, 1,
                            item.GetValue<string>("CreateTime"), item.GetValue<string>("UpdateTime"), item.GetValue<int>("CommentId"), mobilesDic);
                    });
                watcher.Stop();
                current += pageSize;
                Logger.Info(
                    $"ImportOfficialComment {current}/{totalCount} minPkid={tempMinPkid} maxPkid={minPkid} 导入完毕 用时{watcher.ElapsedMilliseconds}");
            }
        }

        void ImportAdditionComment()
        {
            int minPkid = int.MaxValue;
            int maxPkid = int.MaxValue;
            using (var client = new ConfigClient())
            {
                var response = client.GetOrSetRuntimeSwitch("ProductCommentToTopicJob");
                if (response.Success && (response.Result?.Value ?? false))
                {
                    var pkidArr = response.Result.Description.Split(',');
                    if (pkidArr.Length == 2)
                    {
                        int.TryParse(pkidArr[0], out minPkid);
                        int.TryParse(pkidArr[1], out maxPkid);
                    }
                }
            }
            int current = 0;
            int pageSize = 500;
            int pageIndex = 1;
            int totalCount = GetAdditionCommentsCount(minPkid, maxPkid);
            int pageTotal = (totalCount - 1) / pageSize + 1;
            while (pageIndex <= pageTotal)
            {
                pageIndex++;

                var comments = GetAdditionComments(minPkid, maxPkid, pageSize);
                Logger.Info(
                    $"ImportAdditionComment {current}/{totalCount} minPkid={minPkid} 总共获取到 {comments.Rows.Count}条数据");
                if (comments.Rows.Count == 0) break;
                int tempMinPkid = minPkid;
                minPkid = comments.Rows[comments.Rows.Count - 1].GetValue<int>("CommentId");
                Logger.Info(
                    $"ImportAdditionComment {current}/{totalCount} minPkid={tempMinPkid} maxPkid={minPkid} 开始导入");
                Stopwatch watcher = new Stopwatch();
                watcher.Start();
                var userIds = new List<Guid>(comments.Rows.Count);
                foreach (DataRow row in comments.Rows)
                {
                    userIds.Add(row.GetValue<Guid>("CommentUserId"));
                }
                var mobilesDic =
                    new Dictionary<Guid, string>(comments.Rows.Count);
                //批量获取用户手机号
                using (var client = new UserAccountClient())
                {
                    var response = client.GetUsersByIds(userIds);
                    if (response.Success)
                    {
                        foreach (var item in response.Result)
                        {
                            mobilesDic[item.UserId] = item.MobileNumber;
                        }
                    }
                }

                Parallel.ForEach(comments.Rows.OfType<DataRow>(),
                    new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                    item =>
                    {
                        AddReply(item.GetValue<Guid>("CommentUserId"), item.GetValue<string>("CommentUserName"),
                            item.GetValue<int>("CommentId"), item.GetValue<string>("AdditionCommentContent"),
                            item.GetValue<string>("AdditionCommentImages"), item.GetValue<int>("AdditionCommentStatus"),
                            item.GetValue<string>("CreateTime"), item.GetValue<string>("UpdateTime"), item.GetValue<int>("AdditionCommentId"), mobilesDic, false);
                    });
                watcher.Stop();
                current += pageSize;
                Logger.Info(
                    $"ImportAdditionComment {current}/{totalCount} minPkid={tempMinPkid} maxPkid={minPkid} 导入完毕 用时{watcher.ElapsedMilliseconds}");
            }
        }

        public DataTable GetComments(int minPkid, int maxPkid, int pageSize)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP {
                            pageSize
                        } * FROM Gungnir..tbl_Comment WITH(NOLOCK) WHERE CommentId>={minPkid} AND CommentId<={maxPkid}  AND ParentComment IS NULL AND CommentContent<>N'暂无评价' ORDER BY CommentId ASC;"))
            {
                return DbHelper.ExecuteQuery(cmd, dt => dt);
            }
        }

        public DataTable GetCommentByOrderId(int orderId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP 1 * FROM Gungnir..tbl_Comment WITH(NOLOCK) WHERE CommentOrderId={orderId}  AND ParentComment IS NULL AND TopicId>0;"))
            {
                return DbHelper.ExecuteQuery(cmd, dt => dt);
            }
        }

        public int GetCommentsCount(int minPkid, int maxPkid)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT COUNT(1) FROM Gungnir..tbl_Comment WITH(NOLOCK) WHERE CommentId>={
                            minPkid
                        } AND CommentId<={maxPkid} AND ParentComment IS NULL AND CommentContent<>N'暂无评价'"))
            {
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }

        public DataTable GetOfficialComments(int minPkid, int maxPkid, int pageSize)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP {
                            pageSize
                        } * FROM Gungnir..tbl_Comment WITH(NOLOCK) WHERE CommentId>={minPkid} AND CommentId<={maxPkid}  AND ParentComment IS NULL AND CommentContent<>N'暂无评价' AND OfficialReply IS NOT NULL AND TopicId>0 AND ReplyId IS NULL ORDER BY CommentId ASC;"))
            {
                return DbHelper.ExecuteQuery(cmd, dt => dt);
            }
        }

        public int GetOfficialCommentsCount(int minPkid, int maxPkid)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT COUNT(1) FROM Gungnir..tbl_Comment WITH(NOLOCK) WHERE CommentId>={
                            minPkid
                        } AND CommentId<={maxPkid} AND ParentComment IS NULL AND OfficialReply IS NOT NULL AND TopicId>0 AND ReplyId IS NULL AND CommentContent<>N'暂无评价' "))
            {
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }

        public DataTable GetAdditionComments(int minPkid, int maxPkid, int pageSize)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP {
                            pageSize
                        } R.*,C.CommentUserId,C.CommentUserName FROM Gungnir..tbl_AdditionComment  AS R WITH(NOLOCK) 
                        JOIN Gungnir..tbl_Comment AS C WITH(NOLOCK) ON R.CommentId=C.CommentId WHERE R.AdditionCommentId>{minPkid} AND R.AdditionCommentId<={maxPkid} AND R.AdditionCommentContent<>N'暂无评价' AND C.TopicId>0 AND R.ReplyId IS NULL AND R.IsDel=0 ORDER BY CommentId ASC;"))
            {
                return DbHelper.ExecuteQuery(cmd, dt => dt);
            }
        }

        public int GetAdditionCommentsCount(int minPkid, int maxPkid)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT COUNT(1) FROM Gungnir..tbl_AdditionComment AS R WITH(NOLOCK)
                        JOIN Gungnir..tbl_Comment AS C WITH(NOLOCK) ON R.CommentId=C.CommentId WHERE R.AdditionCommentId>={
                            minPkid
                        } AND R.AdditionCommentId<={maxPkid} AND R.AdditionCommentContent<>N'暂无评价' AND C.TopicId>0 AND R.ReplyId IS NULL AND R.IsDel=0"))
            {
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }
        public DataTable GetDefaultVehicleByUserIds(IEnumerable<Guid> userIds)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT UserId,u_cartype_pid_vid AS VehicleId FROM Tuhu_profiles..CarObject WITH(NOLOCK) WHERE IsDeleted=0 AND IsDefaultCar=1 AND UserId IN(SELECT Item FROM Tuhu_profiles..SplitString(@UserIds,',',1))")
            )
            {
                cmd.Parameters.AddWithValue("@UserIds", string.Join(",", userIds));
                return DbHelper.ExecuteQuery(true, cmd, dt => dt);
            }
        }

        public void AddTopic(DataRow row, Dictionary<Guid, string> mobilesDic, Dictionary<string, Service.Product.Models.New.SkuProductDetailModel> productDic, Dictionary<Guid, string> vehicleDic)
        {
            int commentId = row.GetValue<int>("CommentId");
            try
            {
                string commentProductId = row.GetValue<string>("CommentProductId");
                //如果查出来的平轮产品id不存在，不再继续导入
                if (string.IsNullOrEmpty(commentProductId)) return;

                int commentType = row.GetValue<int>("CommentType");
                string commentContent = row.GetValue<string>("CommentContent");
                string commentCreatedAt = row.GetValue<string>("CreateTime");
                string commentUpdatedAt = row.GetValue<string>("UpdateTime");
                int commentStatus = row.GetValue<int>("CommentStatus");
                if (commentStatus == 2) //审核通过的导入的状态为正常，其他的都为屏蔽状态
                {
                    commentStatus = 1;
                }
                else
                {
                    commentStatus = 0;
                }

                string brand = string.Empty;
                string categoryName = string.Empty;
                string CP_Tire_Pattern = string.Empty;
                if (productDic.TryGetValue(commentProductId,
                    out Service.Product.Models.New.SkuProductDetailModel model))
                {
                    brand = model.Brand;
                    categoryName = model.Category;
                    CP_Tire_Pattern = model.Pattern;
                }
                Guid userId = row.GetValue<Guid>("CommentUserId");
                string userName = row.GetValue<string>("CommentUserName");
                mobilesDic.TryGetValue(userId, out string userMobile);

                var commentImgs = row.GetValue<string>("CommentImages");

                var commentTitle = string.Empty;
                var commentR1 = row.GetValue<int>("CommentR1");
                if (commentProductId.StartsWith("TR-")) //轮胎
                {
                    commentTitle = $"【开箱测评】{brand} {CP_Tire_Pattern} {commentR1}星";
                }
                else
                {
                    commentTitle = $"【开箱测评】{brand}{categoryName}（底级） {commentR1}星";
                }
                string vehicleId = string.Empty;

                var orderComments = GetCommentByOrderId(row.GetValue<int>("CommentOrderId"));
                //一个订单下的评论只有一个同步到车型板块
                if (orderComments.Rows.Count == 0)
                {
                    vehicleDic.TryGetValue(userId, out vehicleId);
                }

                IRestClient restClient = new RestClient(TopicAddUrl);
                IRestRequest restRequest = new RestRequest("v1/addProductComment", Method.POST);
                restRequest.AddParameter("userId", userId);
                restRequest.AddParameter("userMobile", userMobile);
                restRequest.AddParameter("userName", userName);
                restRequest.AddParameter("vehicleId", vehicleId);
                restRequest.AddParameter("commentId", commentId);
                restRequest.AddParameter("commentType", commentType);
                restRequest.AddParameter("commentTitle", commentTitle);
                restRequest.AddParameter("commentContent", commentContent);
                restRequest.AddParameter("commentImgs", commentImgs);
                restRequest.AddParameter("commentCreatedAt", commentCreatedAt);
                restRequest.AddParameter("commentUpdatedAt", commentUpdatedAt);
                restRequest.AddParameter("commentStatus", commentStatus);
                restRequest.AddParameter("commentProductId", commentProductId);
                Stopwatch watcher = new Stopwatch();
                watcher.Start();
                var restResponse = restClient.Execute(restRequest);
                watcher.Stop();
                Logger.Info($"评论 {commentId} 导入用时{watcher.ElapsedMilliseconds}");
                if (restResponse.IsSuccessful)
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<AddTopicResponse>(restResponse.Content);
                        if (data.code != 1)
                        {
                            if (!string.IsNullOrEmpty(data.data.msg))
                                Logger.Warn($@"评论 commentId={commentId}导入失败 {
                                        new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                                            .Replace(
                                                data.data.msg,
                                                x => string.Empty +
                                                     Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)))
                                    }");
                            else
                            {
                                Logger.Warn($@"评论 commentId={commentId}导入失败 {restResponse.Content}");
                            }
                        }
                        else
                        {
                            UpdateCommentTopicId(commentId, data.data.topicId);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"评论 commentId={commentId}导入出现异常 {restResponse.Content}", e);
                    }
                }
                else
                {
                    Logger.Error($"评论 commentId={commentId}导入出现异常  {restResponse.Content}", restResponse.ErrorException);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"评论 commentId={commentId}导入出现异常", e);
            }
        }

        public void AddReply(Guid userId, string userName, int commentId, string content, string commentImgs, int commentStatus, string commentCreatedAt, string commentUpdatedAt, int replyId, Dictionary<Guid, string> mobilesDic, bool isOfficial = true)
        {
            try
            {
                if (commentStatus == 2) //审核通过的导入的状态为正常，其他的都为屏蔽状态
                {
                    commentStatus = 1;
                }
                else
                {
                    commentStatus = 0;
                }
                mobilesDic.TryGetValue(userId, out string userMobile);


                IRestClient restClient = new RestClient(TopicAddUrl);
                IRestRequest restRequest = new RestRequest("v1/commentReply", Method.POST);
                restRequest.AddParameter("userId", userId == Guid.Empty ? "" : userId.ToString());
                restRequest.AddParameter("userMobile", userMobile);
                restRequest.AddParameter("userName", userName);
                restRequest.AddParameter("commentId", commentId);
                restRequest.AddParameter("replyBody", content);
                restRequest.AddParameter("replyImgs", commentImgs);
                restRequest.AddParameter("commentStatus", commentStatus);
                restRequest.AddParameter("commentCreatedAt", commentCreatedAt);
                restRequest.AddParameter("commentUpdatedAt", commentUpdatedAt);
                restRequest.AddParameter("replyId", replyId);
                Stopwatch watcher = new Stopwatch();
                watcher.Start();
                var restResponse = restClient.Execute(restRequest);
                watcher.Stop();
                Logger.Info($"追评 {replyId} 导入用时{watcher.ElapsedMilliseconds}");
                if (restResponse.IsSuccessful)
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<AddTopicResponse>(restResponse.Content);
                        if (data.code != 1)
                        {
                            if (!string.IsNullOrEmpty(data.data.msg))
                                Logger.Warn($@"追评 commentId={replyId}导入失败 {
                                        new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                                            .Replace(
                                                data.data.msg,
                                                x => string.Empty +
                                                     Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)))
                                    }");
                            else
                            {
                                Logger.Warn($@"追评 commentId={replyId}导入失败 {restResponse.Content}");
                            }
                        }
                        else
                        {
                            if (isOfficial)
                            {
                                UpdateCommentReplyId(replyId, data.data.replyId);
                            }
                            else
                            {
                                UpdateAdditionCommentReplyId(replyId, data.data.replyId);
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"追评 commentId={replyId}导入出现异常  {restResponse.Content}", e);
                    }
                }
                else
                {
                    Logger.Error($"追评 commentId={replyId}导入出现异常 {restResponse.Content}", restResponse.ErrorException);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"追评 commentId={replyId}导入出现异常", e);
            }
        }

        public void UpdateCommentTopicId(int commentId, int topicId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"UPDATE Gungnir..tbl_Comment WITH(ROWLOCK) SET TopicId={topicId} WHERE CommentId={commentId}"))
            {
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public void UpdateCommentReplyId(int commentId, int replyId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"UPDATE Gungnir..tbl_Comment WITH(ROWLOCK) SET ReplyId={replyId} WHERE CommentId={commentId}"))
            {
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public void UpdateAdditionCommentReplyId(int additionCommentId, int replyId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"UPDATE Gungnir..tbl_AdditionComment WITH(ROWLOCK) SET ReplyId={replyId} WHERE AdditionCommentId={additionCommentId}"))
            {
                DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }

    public class AddTopicResponse
    {
        public int code { get; set; }
        public AddTopicResponseData data { get; set; }
    }

    public class AddTopicResponseData
    {
        public int topicId { get; set; }
        public string msg { get; set; }
        public int replyId { get; set; }
        public string status { get; set; }
    }
}

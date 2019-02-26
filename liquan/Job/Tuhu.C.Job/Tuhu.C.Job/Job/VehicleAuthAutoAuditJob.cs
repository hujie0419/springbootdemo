using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Request;

namespace Tuhu.C.Job.Job
{

    /// <summary>
    /// 认证车型申诉自动审核Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class VehicleAuthAutoAuditJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VehicleAuthAutoAuditJob));
        private static readonly string Author = "tuhusystem@tuhu.cn";
        public static readonly string merchantID = ConfigurationManager.AppSettings["MinSHMID"] ?? "10084";
        public static readonly string account = ConfigurationManager.AppSettings["MinSHAccount"] ?? "tuhu1";
        public static readonly string key = ConfigurationManager.AppSettings["MinSHKey"] ?? "b46615f5";
        public static readonly string url = ConfigurationManager.AppSettings["MinSHUrl"] ?? "http://api.minsh.cn/ocr/v1/vehicleLicenseRecognize";

        public static readonly string UserCardUrl= ConfigurationManager.AppSettings["MinSHUserCardUrl"] ?? "http://api.minsh.cn/ocr/v2/idRecognize";

        private static int ThreadCount = 1;

        private static int GongDan_TaskId = 0;//创建工单的taskid

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("开始执行 VehicleAuthAutoAuditJob ");
                GongDan_TaskId = 0;
                var dataMap = context.JobDetail.JobDataMap;
                ThreadCount = dataMap.GetInt("ThreadCount");
                if (ThreadCount < 1) ThreadCount = 1;
                if (ThreadCount > 20) ThreadCount = 20;

                //var openSixAnnual = dataMap.GetBoolean("OpenSixAnnual");
                //if (openSixAnnual)
                //{
                //    AuditSixAnnual();
                //    Logger.Info($"VehicleAuthAutoAuditJob AuditSixAnnual Success");
                //}
                var openAppealAudit = dataMap.GetBoolean("OpenAppealAudit");
                if (openAppealAudit)
                {
                    Audit();
                    Logger.Info($"VehicleAuthAutoAuditJob openAppealAudit Success");
                }

                Logger.Info($"VehicleAuthAutoAuditJob Success");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 审核六周年的
        /// </summary>
        void AuditSixAnnual()
        {
            int pkid = 0;
            int pageSize = 100;
            while (true)
            {

                var data = GetVehicleAuth(pkid, pageSize);
                if (!data.Any()) break;
                List<Task> tasks = new List<Task>();
                foreach (var item in data)
                {
                    pkid = item.PKID;
                    //不是待审核状态，不做自动审核, 已经审核过的也不做自动审核
                    if (item.Status != 0||string.IsNullOrEmpty(item.Vehicle_license_img)|| ExistsAudit(item.CarId)) continue;
                    //真正的方法体
                    Action<CarInfo> action = (obj) =>
                    {
                        var bytes = LoadImage(obj.Vehicle_license_img);
                        if (bytes == null) return;
                        var response = ExecuteORC<OCRResponse>(bytes,url).GetAwaiter().GetResult();
                        if (response == null) return;
                        if (response.plateNumber == obj.CarNo && response.vin == obj.ClassNo)
                        {
                            //认证通过

                            if (UpdateVehicleAuth(new Service.Vehicle.Request.VehicleTypeCertificationRequest()
                            {
                                CarId=obj.CarId,
                                Status=1
                            }))
                            {
                                InsertLog(obj.CarId, Author,
                                    $"6周年系统审核成功 OCR调用结果 为 {JsonConvert.SerializeObject(response)}", "1", "0");
                            }
                        }
                        else
                        {
                            //认证不通过，记录审核过的日志
                            InsertLog(obj.CarId, Author,
                                $"6周年系统审核失败 不做任何操作 OCR调用结果 为 {JsonConvert.SerializeObject(response)}", "0", "0");
                        }
                    };

                    if (ThreadCount > 1)
                    {
                        tasks.Add(Task.Factory.StartNew((obj) =>
                        {
                            action.Invoke(obj as CarInfo);
                        }, item));
                        if (tasks.Count >= ThreadCount)
                        {
                            Task.WaitAll(tasks.ToArray());
                            tasks.Clear();
                        }
                    }
                    else //如果是单线程 就直接在主线程上运行即可
                    {
                        action.Invoke(item);
                    }
                }
                if (tasks.Count > 0)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
                if (data.Count() < pageSize) break;
                System.Threading.Thread.Sleep(1000);
            }
        }

        bool UpdateVehicleAuth(Service.Vehicle.Request.VehicleTypeCertificationRequest request)
        {
            using (var client = new Tuhu.Service.Vehicle.VehicleClient())
            {
                var response = client.UpdateVehicleTypeCertificationStatus(request);
                return response.Result;
            }
        }

        bool UpdateAppeal(Guid carId, int status)
        {
            using (var helper = DbHelper.CreateDbHelper())
            {
                using (var cmd = helper.CreateCommand(@"UPDATE Tuhu_profiles..VehicleAuthAppeal WITH(ROWLOCK)
                                                        SET Status=@Status
                                                        WHERE CarID=@CarId"))
                {

                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@CarId", carId));
                    return helper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        bool InsertLog(Guid carId, string author, string description, string newValue, string oldValue)
        {
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = helper.CreateCommand(@"INSERT INTO Tuhu_log..VehicleTypeCertificationAuditLog
                                                   ([CarId]
                                                   ,[Author]
                                                   ,[Description]
                                                   ,[CreateTime]
                                                   ,[NewValue]
                                                   ,[OldValue])
                                             VALUES (@CarId,@Author,@Description,GETDATE(),@NewValue,@OldValue)"))
                {
                    cmd.Parameters.Add(new SqlParameter("@CarId", carId));
                    cmd.Parameters.Add(new SqlParameter("@Author", author));
                    cmd.Parameters.Add(new SqlParameter("@Description", description));
                    cmd.Parameters.Add(new SqlParameter("@NewValue", newValue));
                    cmd.Parameters.Add(new SqlParameter("@OldValue", oldValue));
                    return helper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        /// <summary>
        /// 审核1.8申诉的
        /// </summary>
        void Audit()
        {
            int pkid = 0;
            int pageSize = 100;
            while (true)
            {

                var data = GetAppeal(pkid, pageSize);
                if (!data.Any()) break;
                List<Task> tasks = new List<Task>();
                foreach (var item in data)
                {
                    pkid = item.PKID;
                    //不是待审核状态，不做自动审核, 已经审核过的也不做自动审核
                    if (item.Status != 0 || string.IsNullOrEmpty(item.Vehicle_license_img) || string.IsNullOrEmpty(item.User_IdCard_img) || ExistsAudit(item.CarId)) continue;
                    //真正的方法体
                    Action<CarInfo> action = (obj) =>
                    {
                        var bytes = LoadImage(obj.Vehicle_license_img);
                        if (bytes == null) return;
                        var response = ExecuteORC<OCRResponse>(bytes, url).GetAwaiter().GetResult();
                        if (response == null) return;

                        var cardBytes = LoadImage(obj.User_IdCard_img);
                        if (cardBytes == null) return;
                        var cardResponse = ExecuteORC<OCRUserCardResponse>(cardBytes, UserCardUrl).GetAwaiter().GetResult();
                        if (cardResponse == null) return;

                        if (response.plateNumber == obj.CarNo && response.vin == obj.ClassNo &&!string.IsNullOrWhiteSpace(response.owner)&&!string.IsNullOrWhiteSpace(cardResponse.name)&&
                            response.owner == cardResponse.name) //增加了姓名验证
                        {
                            //认证通过

                            if (UpdateVehicleAuth(new Service.Vehicle.Request.VehicleTypeCertificationRequest()
                            {
                                CarId = obj.CarId,
                                CarNo = obj.CarNo,
                                EngineNo = obj.Engineno,
                                Status = 1,
                                User_IdCard_img = obj.User_IdCard_img,
                                Vehicle_license_img = obj.Vehicle_license_img,
                                VinCode = obj.ClassNo,
                                Registrationtime = obj.car_Registrationtime
                            }))
                            {
                                UpdateAppeal(obj.CarId, 1);
                                InsertLog(obj.CarId, Author,
                                    $@"1.8系统审核成功 OCR调用结果 为 {JsonConvert.SerializeObject(response)} /n 
                                    OCR身份证调用结果{JsonConvert.SerializeObject(cardResponse)}", "1", "0");
                            }
                        }
                        else
                        {
                            if (GongDan_TaskId == 0)
                            {
                                GongDan_TaskId = AddYewuTask();//审核失败，给业务系统创建工单，让人工审核
                            }
                            string taskDes = GongDan_TaskId > 0 ? $"工单任务id为: {GongDan_TaskId}" : "";
                            //认证不通过，记录审核过的日志
                            InsertLog(obj.CarId, Author,
                                $@"1.8系统审核失败 不做任何操作 OCR调用结果 为 {JsonConvert.SerializeObject(response)} /n 
                                OCR身份证调用结果{JsonConvert.SerializeObject(cardResponse)} {taskDes}", "0", "0");
                        }
                    };

                    if (ThreadCount > 1)
                    {
                        tasks.Add(Task.Factory.StartNew((obj) =>
                        {
                            action.Invoke(obj as CarInfo);
                        }, item));
                        if (tasks.Count >= ThreadCount)
                        {
                            Task.WaitAll(tasks.ToArray());
                            tasks.Clear();
                        }
                    }
                    else //如果是单线程 就直接在主线程上运行即可
                    {
                        action.Invoke(item);
                    }
                }
                if (tasks.Count > 0)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
                if (data.Count() < pageSize) break;
                System.Threading.Thread.Sleep(1000);
            }
        }

        IEnumerable<CarInfo> GetVehicleAuth(int pkid, int pageSize)
        {
            var date = new DateTime(2017, 10, 20);
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = $@"SELECT TOP {
                        pageSize
                    }V.PKID,V.CarID AS CarId,C.UserId,C.u_carno AS CarNo,C.VinCode AS ClassNo,V.Vehicle_license_img,C.Engineno,ISNULL(V.[Status],-1) AS Status FROM 
                                                        Tuhu_profiles..VehicleTypeCertificationInfo AS V WITH(NOLOCK) INNER JOIN Tuhu_profiles..CarObject AS C WITH(NOLOCK)
                                                        ON V.CarID=C.CarID 
                                                        WHERE V.Status=0";
                if (pkid > 0)
                {
                    sql += " AND PKID<@PKID ";
                }
                if (DateTime.Now > date)
                {
                    sql+= " AND V.CreateDateTime>=@CreateTime ";
                }
                using (var cmd = helper.CreateCommand($@"{sql} ORDER BY PKID DESC"))
                {
                    if (DateTime.Now > date)//19号上线的，所以第一次执行的时候把所有的,之后只执行最近两天的
                    {
                        cmd.Parameters.Add(new SqlParameter("@CreateTime", DateTime.Now.AddDays(-1).Date.ToShortDateString()));
                    }

                    if (pkid > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                    }
                    return helper.ExecuteSelect<CarInfo>(cmd);

                }
            }
        }

        /// <summary>
        /// 从申诉表获取待审核的数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<CarInfo> GetAppeal(int pkid,int pageSize)
        {
            var date = new DateTime(2017, 11, 1);
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = $@"SELECT TOP {pageSize} PKID,CarID AS CarId,UserId,CarNo,ClassNo,Vehicle_license_img,User_IdCard_img,Engineno,Status,car_Registrationtime FROM 
                                                        Tuhu_profiles..VehicleAuthAppeal WITH(NOLOCK)
                                                        WHERE Status=0";
                if (pkid > 0)
                {
                    sql += " AND PKID<@PKID";
                }
                if (DateTime.Now > date)
                {
                    sql += " AND CreateTime>=@CreateTime ";
                }
                using (var cmd = helper.CreateCommand($@"{sql} ORDER BY PKID DESC"))
                {
                    //上线的时候要执行所有的

                    if (DateTime.Now > date)//30 号上线的，所以第一次执行的时候把所有的,之后只执行最近两天的
                    {
                        cmd.Parameters.Add(new SqlParameter("@CreateTime", DateTime.Now.AddDays(-1).Date.ToShortDateString()));
                    }
                    if (pkid > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                    }
                    return helper.ExecuteSelect<CarInfo>(cmd);
                }
            }
        }

        int AddYewuTask()
        {
            var inputItem = new CreateTaskNeiBuCommonInput()
            {
                OrderId = 0, //订单号
                Creator = "tuhusystem@tuhu.cn", //提交者名称
                Remark = "", //备注信息
                Res = "", //没有可以默认为“备用字段”
                Title = "车型认证", //调用工单标题已固定，不能修改
                //SecondTitle = "工单任务2",//调用工单标题已固定，不能修改
                //ThirdTitle = "工单任务3",//调用工单标题已固定，不能修改
                Priority = 3 //调用工单权限已固定，不能修改
            };
            using (var client = new OrderTaskClient())
            {
                var result = client.CreateTaskNeiBuCommon(inputItem);
                if (result.Success)
                {
                    return result.Result;
                }
            }
            return 0;
        }

        #region OCR相关
        /// <summary>
        /// 验证这个车型是否已经审核过
        /// </summary>
        /// <param name="carId"></param>
        /// <returns></returns>
        bool ExistsAudit(Guid carId) {
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd =
                    helper.CreateCommand(
                        @"SELECT TOP 1 1 FROM Tuhu_Log..VehicleTypeCertificationAuditLog WITH(NOLOCK) WHERE CarId=@CarId AND CreateTime>=@CreateTime")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@CarId", carId));
                    cmd.Parameters.Add(new SqlParameter("CreateTime", DateTime.Now.Date.ToShortDateString()));
                    var obj = helper.ExecuteScalar(cmd);
                    return (int?) obj > 0;
                }
            }
        }

        /// <summary>
        /// 识别行驶证
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        async Task<T> ExecuteORC<T>(byte[] file,string requestUrl)
        {
            if (file == null || file.Length <= 0) { throw new Exception("文件大小异常"); }
            if (file.Length > 2097152) { throw new Exception("文件不允许超过2M"); }
            var httpClient = new HttpClient();
            try
            {
                httpClient.DefaultRequestHeaders.Add("merchantID", merchantID);
                httpClient.DefaultRequestHeaders.Add("account", getAccount());
                httpClient.DefaultRequestHeaders.Add("timeStamp", getTimeStamp());
                HttpContent content = new ByteArrayContent(file);
                var data = await httpClient.PostAsync(requestUrl, content);
                if (data.IsSuccessStatusCode)
                {
                    var result = await data.Content.ReadAsStringAsync();
                    var resultdata = decryptForDES(result, key);
                    return JsonConvert.DeserializeObject<T>(resultdata);
                }
                else
                {

                    var result = JsonConvert.DeserializeObject<HttpResult>(await data.Content.ReadAsStringAsync());
                    Logger.Error($"{result.Message}");
                    return default(T);
                }
            }
            catch (Exception e)
            {

                Logger.Error($"行驶证图片识别失败: {e.Message}");
                return default(T);

            }
        }
        private static string getAccount()
        {
            try
            {
                return encryptForDES(account, key);
            }
            catch
            { throw; }
        }

        private static string getTimeStamp()
        {
            try
            {
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000; //除10000调整为13位
                return encryptForDES(t.ToString(), key);
            }
            catch
            {
                throw;
            }

        }

        private static string encryptForDES(string message, string key)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(message);
                des.Key = UTF8Encoding.UTF8.GetBytes(key);
                des.IV = UTF8Encoding.UTF8.GetBytes(key);
                des.Mode = CipherMode.ECB;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        private static string decryptForDES(string message, string key)
        {
            byte[] inputByteArray = Convert.FromBase64String(message);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = UTF8Encoding.UTF8.GetBytes(key);
                des.IV = UTF8Encoding.UTF8.GetBytes(key);
                des.Mode = CipherMode.ECB;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        byte[] LoadImage(string url)
        {
            try
            {
                WebClient client = new WebClient();
                return client.DownloadData(url);
            }
            catch (Exception e)
            {
                Logger.Error($"{url} download error");
                return null;
            }
        }

        #endregion
    }
    class CarInfo {
        public int PKID { get; set; }
        public Guid CarId{ get; set; }
        public Guid UserId { get; set; }
        public string CarNo{ get; set; }
        public string ClassNo{ get; set; }
        public string Vehicle_license_img{ get; set; }
        public string User_IdCard_img { get; set; }
        public string Engineno{ get; set; }
        public int Status{ get; set; }
        public DateTime? car_Registrationtime { get; set; }
    }

    class OCRUserCardResponse{
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string birthday { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idNumber { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string people { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 签发机关
        /// </summary>
        public string issueAuthority { get; set; }
        /// <summary>
        /// 有效期限
        /// </summary>
        public string validity { get; set; }
    }

    class OCRResponse {
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string engineNumber { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string issueDate { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        public string plateNumber { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string registerDate { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string type { get; set; }
        public string usage { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string vin { get; set; }
    }
    /// <summary>
    /// http请求错误返回对象MODEL
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { set; get; }
    }
}

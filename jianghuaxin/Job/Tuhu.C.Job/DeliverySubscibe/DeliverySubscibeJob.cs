using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using Tuhu.Component.Order.BusinessFacade;

namespace Tuhu.Yewu.WinService.JobSchedulerService.DeliverySubscibe
{
	/// <summary>快递100订阅快递信息</summary>
	[DisallowConcurrentExecution]
	//[PersistJobDataAfterExecution]
	public class DeliverySubscibeJob : IInterruptableJob
	{
		private static readonly WebClient WebClient = new WebClient();

		private static readonly ILog Logger = LogManager.GetLogger(typeof(DeliverySubscibeJob));
		private static readonly ILog SubscibeLog = LogManager.GetLogger("DeliverySubscibeLogger");

		private bool _isStop = false;

		public void Execute(IJobExecutionContext context)
		{
			lock (Logger)
			{
				var jobName = JsonConvert.SerializeObject(context.JobDetail.Key);

				Logger.Info("启动任务");

			    var source = DeliveryHelper.SelectSubscibeDeliveryModels();

				Logger.InfoFormat("获得{0}个需要订阅的快递", source.Count());

				foreach (var delivery in source)
				{
					if (!_isStop)
						lock (this)
							if (!_isStop)
							{
								var model = new SubscibeModel(delivery);
                                try
                                {
                                    SubscibeLog.Info(new SubscibeLogModel(model, Subscibe(model)));
                                    if (model.SubscibeResult)
                                    {
                                        DeliveryHelper.SubscribeDelivery(delivery.DeliveryCompany, delivery.DeliveryCode);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex.Message, ex);
                                    SubscibeLog.Error(new SubscibeLogModel(model, null), ex);
                                }
								
								continue;
							}
					Logger.Info("任务被终止");
					break;
				}

				Logger.Info("结束任务");
			}
		}

		private string Subscibe(SubscibeModel model)
		{
			model.SubscibeBody.Parameters.Salt = DeliverySystem.GetKuaidi100Salt(model.SubscibeBody.DeliveryCompany, model.SubscibeBody.DeliveryCode).ToString();
			var json = model.SubscibeBody.ToString();

			var postVars = new NameValueCollection();

			postVars.Add("schema", "json");
			postVars.Add("param", json);

			var responseBuffer = WebClient.UploadValues(ConfigurationManager.AppSettings["DeliverySubscibeJob:SubscibeUrl"], "POST", postVars);

			var responseString = Encoding.UTF8.GetString(responseBuffer);
			var responseResult = JsonConvert.DeserializeObject<ResponseResultModel>(responseString);

			model.SubscibeResult = responseResult.Result;

			if (!model.SubscibeResult && responseResult.ReturnCode != 501)
				Logger.Info("快递订阅失败，请求内容：" + JsonConvert.SerializeObject(model) + "；响应内容：" + responseString);

			return responseString;
		}

		public void Interrupt()
		{
			lock (this)
			{
				_isStop = true;
			}
		}
	}
}

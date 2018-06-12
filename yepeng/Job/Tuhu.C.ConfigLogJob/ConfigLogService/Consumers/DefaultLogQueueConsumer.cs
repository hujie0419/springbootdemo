using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.MessageQueue;
using Tuhu.Service.ConfigLog;

namespace ConfigLogService.Consumers
{
    public class DefaultLogQueueConsumer : MessageConsumerService
    {
        private static readonly string ExchageName = ConfigurationManager.AppSettings["defaultExchageName"];
        private static readonly string LogQueue = "message.configlog.default";

        public override RabbitMQConsumer BeginConsume(string name, RabbitMQClient client)
        {
            var consumer = client.CreateConsumer(name);

            consumer.ExchangeDeclare(ExchageName);

            consumer.QueueBind(LogQueue, ExchageName);

            consumer.SubscribeAsync(LogQueue, InsertLogQueue);

            return consumer;
        }

        public async Task<bool> InsertLogQueue(IMessage message)
        {
            try
            {
                var logInfo = message.JsonDeserialize<IDictionary<string, string>>();
                using (var client = new ConfigLogClient())
                {
                    var isInsertSuccess = await client.ConsumeDefaultLogAsync(logInfo);
                    if (!isInsertSuccess.Result)
                    {
                        Logger.Error($"添加日志失败, data:{JsonConvert.SerializeObject(logInfo)}", isInsertSuccess.Exception);
                        isInsertSuccess.ThrowIfException(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex);
            }

            return true;
        }
    }
}

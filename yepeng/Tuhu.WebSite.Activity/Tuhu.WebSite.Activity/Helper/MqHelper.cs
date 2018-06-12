using System;
using System.Collections.Concurrent;
using Tuhu.MessageQueue;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity
{
    public static class MqHelper
    {
        private const string DEFAULT_EXCHAGE = "direct.defaultExchage";

        private static readonly ConcurrentDictionary<string, RabbitMQProducer> ProducerMap = new ConcurrentDictionary<string, RabbitMQProducer>();

        /// <summary>
        /// 发现统计数据
        /// </summary>
        public const string ARTICLE_STATISTICS_QUEUE = "ArticleStatisticsRecharge";

        static MqHelper()
        {

        }

        public static void Send(string queue, object msg)
        {
            try
            {
                var producer = GetProducer(queue);
                if (producer == null)
                {
                    throw new Exception($"消息队列空, 队列名:{queue}");
                }
                producer.Send(queue, msg);
            }
            catch (Exception ex)
            {
                WebLog.Logger.Error($"MqHelper.Send 错误. 队列名: {queue}", ex);
            }
        }

        private static RabbitMQProducer CreateProducer(string queue)
        {
            var producer = RabbitMQClient.DefaultClient.CreateProducer(DEFAULT_EXCHAGE);
            if (producer == null)
            {
                //TODO: 可考虑重新初始化
                throw new Exception($"消息队列初始化失败. 队列名:{queue}");
            }

            if (ProducerMap.TryAdd(queue, producer))
            {
                producer.ExchangeDeclare(DEFAULT_EXCHAGE);
                producer.QueueBind(queue, DEFAULT_EXCHAGE);
            }

            return producer;
        }

        private static RabbitMQProducer GetProducer(string queue)
        {
            return ProducerMap.GetOrAdd(queue, CreateProducer);
        }
    }
}
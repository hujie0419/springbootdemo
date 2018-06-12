using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.MessageQueue;
using Tuhu.ZooKeeper;

namespace ConfigLogService
{
    partial class HostService : MasterSlaveService
    {
        private readonly ICollection<MessageConsumerService> _services;

        public HostService()
        {
            InitializeComponent();

            Logger = LogManager.GetLogger(typeof (HostService));

            _services = new List<MessageConsumerService>();
        }

        public override ILog Logger { get; }

        public override void StartService()
        {
            var consumers = (ConfigurationManager.GetSection("messageConsumer") as MessageConsumerSection).MessageConsumers
                 .OfType<MessageConsumerConfigurationElement>()
                 .Select(c => new KeyValuePair<Type, string>(Type.GetType(c.Type), c.Name))
                 .Where(c => typeof(MessageConsumerService).IsAssignableFrom(c.Key))
                 .ToList();

            consumers.AddRange(Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(MessageConsumerService).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new KeyValuePair<Type, string>(t, t.Name)));

            Logger.Info(consumers.Count + "个消费者");

            foreach (var consumer in consumers)
            {
                try
                {
                    var service = Activator.CreateInstance(consumer.Key) as MessageConsumerService;

                    if (service == null)
                        Logger.Warn(consumer.Value + "没有继承MessageConsumerService");
                    else
                    {
                        service.Register(consumer.Value, RabbitMQClient.DefaultClient);

                        _services.Add(service);
                    }
                }
                catch (TypeInitializationException ex)
                {
                    Logger.FatalException("类型初始化异常", ex.InnerException ?? ex);
                }
                catch (Exception ex)
                {
                    Logger.FatalException("启动消费都失败", ex);
                }
            }

            Logger.Info("服务启动");
        }

        public override void StopService()
        {
            Logger.Info("服务停止");

            log4net.LogManager.Shutdown();
            try
            {
                Task.WaitAll(_services.Select(service => Task.Run(new Action(service.Dispose))).ToArray());
            }
            catch { }

            RabbitMQClient.DefaultClient.Dispose();
        }

        protected override string GetMessage(string status)
        {
            var clientInfo = Client.ClientInfo;

            return clientInfo.Ip + "的作业调度服务" + status;
        }

#if DEBUG
        public void StartIt()
        {
            this.OnStart(null);
        }
        public void StopIt()
        {
            this.OnStop();
        }
        public void PauseIt()
        {
            this.OnPause();
        }
        public void ContinueIt()
        {
            this.OnContinue();
        }
#endif
    }

    /// <summary>队列消费服务</summary>
    public abstract class MessageConsumerService : IDisposable
    {
        /// <summary></summary>
        public ILog Logger { get; protected set; }
        /// <summary></summary>
        public RabbitMQConsumer Consumer { get; private set; }

        /// <summary></summary>
        public void Register(string name, RabbitMQClient client)
        {
            Logger = LogManager.GetLogger(name);

            Consumer = BeginConsume(name, client);

            if (Consumer != null)
                Consumer.OnError += ex => Logger.Error(ex.Message, ex);
        }

        /// <summary></summary>
        public abstract RabbitMQConsumer BeginConsume(string name, RabbitMQClient client);

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //释放托管资源，比如将对象设置为null
            }

            //释放非托管资源
            if (Consumer != null)
                Consumer.Dispose();

            _disposed = true;
        }

        ~MessageConsumerService()
        {
            Dispose(false);
        }
        #endregion
    }
}
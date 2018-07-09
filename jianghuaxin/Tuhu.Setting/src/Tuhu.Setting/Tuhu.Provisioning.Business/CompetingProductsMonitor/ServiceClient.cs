using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.CompetingProductsMonitorManager
{
    public class ServiceClient<T> : ClientBase<T>, IDisposable where T : class
    {
        public ServiceClient()
        {
            //read config
        }

        public ServiceClient(Binding binding, EndpointAddress endpoint) : base(binding, endpoint)
        {
            //manual config
        }

        public ServiceClient(string url, bool http = true) :
            this(http ? new BasicHttpBinding()
            {
                MaxBufferSize = Convert.ToInt32(ConfigurationManager.AppSettings["WCF.MaxBufferSize"] ?? "2147483647"),
                MaxReceivedMessageSize = Convert.ToInt32(ConfigurationManager.AppSettings["WCF.MaxBufferSize"] ?? "2147483647"),
                ReceiveTimeout = TimeSpan.FromSeconds(Convert.ToInt32((ConfigurationManager.AppSettings["WCF.ReceiveTimeoutSecond"] ?? "20"))),
                SendTimeout = TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["WCF.SendTimeoutSecond"] ?? "20"))
            } : throw new Exception("暂不支持"), new EndpointAddress(url))
        {
            //with default config
        }

        private T _ins = null;

        private readonly object _create_proxy_lock = new object();

        [Obsolete("无法代理channel，方法没有实现")]
        public bool UseProxy { get; set; } = false;

        /// <summary>
        /// 接口实例
        /// </summary>
        public T Instance
        {
            get
            {

                return this.Channel;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
    }
}

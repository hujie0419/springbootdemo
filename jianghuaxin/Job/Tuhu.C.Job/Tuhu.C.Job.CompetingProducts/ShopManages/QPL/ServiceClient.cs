using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CompetingProducts.ShopManages
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
                //MaxBufferSize = (ConfigurationManager.AppSettings["WCF.MaxBufferSize"] ?? "2147483647").ToInt(null),
                //MaxReceivedMessageSize = (ConfigurationManager.AppSettings["WCF.MaxBufferSize"] ?? "2147483647").ToInt(null),
                //ReceiveTimeout = TimeSpan.FromSeconds((ConfigurationManager.AppSettings["WCF.ReceiveTimeoutSecond"] ?? "20").ToInt(null)),
                //SendTimeout = TimeSpan.FromSeconds((ConfigurationManager.AppSettings["WCF.SendTimeoutSecond"] ?? "20").ToInt(null))
                MaxBufferSize = Convert.ToInt32(ConfigurationManager.AppSettings["WCF.MaxBufferSize"] ?? "2147483647"),
                MaxReceivedMessageSize = Convert.ToInt32(ConfigurationManager.AppSettings["WCF.MaxBufferSize"] ?? "2147483647"),
                ReceiveTimeout = TimeSpan.FromSeconds(Convert.ToInt32((ConfigurationManager.AppSettings["WCF.ReceiveTimeoutSecond"] ?? "20"))),
                SendTimeout = TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["WCF.SendTimeoutSecond"] ?? "20"))
            } : throw new Exception("暂不支持"), new EndpointAddress(url))
        {
            //with default config
        }

        //class LogProxy : IInterceptor
        //{
        //    public void Intercept(IInvocation invocation)
        //    {
        //        invocation.Proceed();

        //        var valueType = invocation.ReturnValue;
        //    }
        //}

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
                //if (this.UseProxy)
                //{
                //    if (this._ins == null)
                //    {
                //        lock (this._create_proxy_lock)
                //        {
                //            if (this._ins == null)
                //            {
                //                var p = new ProxyGenerator();
                //                var classToProxy = this.Channel.GetType();
                //                var proxy_ins = (T)p.CreateClassProxy(classToProxy, new LogProxy());

                //                this._ins = proxy_ins;
                //            }
                //        }
                //    }
                //    return this._ins;
                //}
                //else
                //{
                return this.Channel;
                //}
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            //this.SafeClose_();
            this.Close();
        }
    }
}

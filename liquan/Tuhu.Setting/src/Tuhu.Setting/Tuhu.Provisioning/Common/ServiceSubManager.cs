
using System;
using ServiceBridge.core;
using ServiceBridge.distributed.zookeeper.ServiceManager;
using ServiceBridge.rpc;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ServiceBridge.extension;

namespace Tuhu.Provisioning.Common
{

    

    public class ServiceSubManager : IDisposable
    {
        public static readonly ServiceSubManager Instance = new ServiceSubManager();

        public readonly Lazy_<ServiceBridge.distributed.zookeeper.ServiceManager.ServiceSubscribe> _lz = new Lazy_<ServiceBridge.distributed.zookeeper.ServiceManager.ServiceSubscribe>(() =>
        new ServiceBridge.distributed.zookeeper.ServiceManager.ServiceSubscribe(ConfigurationManager.ConnectionStrings["ZK"]?.ConnectionString ?? throw new Exception("请配置zookeeper")))
            .WhenDispose((ref ServiceBridge.distributed.zookeeper.ServiceManager.ServiceSubscribe x) => x.Dispose());

        public void Dispose()
        {
            if (this._lz.IsValueCreated)
            {
                this._lz.Dispose();
            }
        }
    }

    /// <typeparam name="T"></typeparam>
    public class Auto_Service_Client_<T> : ServiceClient<T> where T : class
    {
        public Auto_Service_Client_() :
            base(ServiceSubManager.Instance._lz.Value.ResolveSvc<T>() ?? throw new Exception($"{typeof(T)}服务已下线"))
        {
            //this.Endpoint.EndpointBehaviors.Add(new MyEndPointBehavior<MyMessageInspector>());
        }

        [Obsolete("理论上只有单元测试才会用到这个构造函数")]
        public Auto_Service_Client_(string url) :
            base(ServiceSubManager.Instance._lz.Value.ResolveSvc<T>() ?? throw new Exception($"{typeof(T)}服务已下线"))
        { }
    }

    public static class XServiceExtension
    {
        public static List<ContractModel> ScanSvcContractModel(this Assembly ass, string base_dir)
        {
            var host_port = ConfigurationManager.AppSettings["host_port"] ??
               throw new Exception("host_port");

            var base_url = ConfigurationManager.AppSettings["base_url"] ?? string.Empty;
            var host = string.IsNullOrEmpty(base_url) ? Dns.GetHostName() : base_url;

            ContractModel ExtractContract(Type t)
            {
                var file_name = $"{t.Name}.svc";
                //base_url = base_url.TrimEnd('/', '\\');
                var url = $"http://{host}:{host_port}/{file_name}";
                var file_path = Path.Combine(base_dir, file_name);

                if (!File.Exists(file_path))
                    throw new FileNotFoundException(file_path);
                var contract = t.GetInterfaces().ToList();
                if (!contract.Any())
                    throw new Exception($"{t.FullName}必须至少实现一个接口");

                return new ContractModel(contract.First(), url);
            }

            var services = ass.GetTypes()
                .Where(x => x.IsNormalClass())
                .Where(x => x.GetCustomAttributes<ServiceContractImplAttribute>().Any());

            return services.Select(x => ExtractContract(x)).ToList();
        }

        public static ServiceRegister ServiceRegister(Assembly ass, string base_dir)
        {
            var zookeeper_constring =
                ConfigurationManager.AppSettings["zookeeper_constring"] ??
                ConfigurationManager.ConnectionStrings["ZK"]?.ConnectionString ??
                throw new Exception("zookeeper_constring");

            var data = ass.ScanSvcContractModel(base_dir);
            return new ServiceRegister(zookeeper_constring, () => data);
        }
    }

    public class ThEpcCatalogServiceClient : Auto_Service_Client_<QPL.WebService.TuHu.Service.ITuHuTaskService>
    {


    }
}
<%@ WebHandler Language="C#" Class="Tuhu.Service.Hosting.Monitor" %>

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace Tuhu.Service.Hosting
{
    /// <summary></summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IMonitorClient : IMonitorService, ITuhuServiceClient
    {
        /// <summary>获取短网址</summary>
        [OperationContract(Name = "Monitor", Action = TuhuSerivce.TuhuSerivceNamespace + "/Hosting/Monitor", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Hosting/MonitorResponse")]
        Task<OperationResult<int>> MonitorAsync(int port);
    }

    public class Monitor : HttpTaskAsyncHandler
    {
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            using (var client = new TuhuServiceClient<IMonitorClient>())
            {
                client.Endpoint.Address = GetLocalhost(client.Endpoint.Address);

                var result = await client.InvokeAsync(_ => _.MonitorAsync(client.Endpoint.Address.Uri.Port));

                result.ThrowIfException();

                context.Response.Write(result.Result);
            }
        }

        private static EndpointAddress GetLocalhost(EndpointAddress address)
        {
            var ub = new UriBuilder(address.Uri) { Host = "127.0.0.1" };
            return new EndpointAddress(ub.Uri, address.Identity, address.Headers);
        }
    }
}
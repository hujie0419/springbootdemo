using System.ServiceModel;

namespace Tuhu.Service.Hosting
{
    /// <summary></summary>
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
    public interface IMonitorService
    {
        /// <summary>获取短网址</summary>
        [OperationContract(Name = "Monitor", Action = TuhuSerivce.TuhuSerivceNamespace + "/Hosting/Monitor", ReplyAction = TuhuSerivce.TuhuSerivceNamespace + "/Hosting/MonitorResponse")]
        OperationResult<int> Monitor(int port);
    }

    public class MonitorService : IMonitorService
    {
        public OperationResult<int> Monitor(int port)
        {
            return OperationResult.FromResult(port);
        }
    }
}
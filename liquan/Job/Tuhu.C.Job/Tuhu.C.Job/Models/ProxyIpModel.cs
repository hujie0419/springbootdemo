
using System;
using Tuhu.Models;

namespace Tuhu.C.Job.Models
{
    public class ProxyIpModel : BaseModel, IComparable, IComparable<ProxyIpModel>
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Position { get; set; }
        public int Speed { get; set; }

        public int CompareTo(object obj) => CompareTo(obj as ProxyIpModel);

        public int CompareTo(ProxyIpModel other) => string.Compare($"{Ip}:{Port}", $"{other.Ip}:{other.Port}", StringComparison.Ordinal);
    }
}

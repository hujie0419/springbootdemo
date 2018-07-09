using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class RefreshCachePrefixRequest
    {
        public string Prefix { get; set; }
        public string ClientName { get; set; }
        public TimeSpan Expiration { get; set; }

        public string Key { get; set; }
    }
}

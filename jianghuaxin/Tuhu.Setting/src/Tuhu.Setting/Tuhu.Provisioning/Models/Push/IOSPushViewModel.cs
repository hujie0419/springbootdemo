using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Provisioning.Models.Push
{
    public class IOSPushViewModel
    {
        public int PKID { get; set; }
        public int BatchID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content { get; set; }
        public DateTime? PushTime { get; set; }
        public string ExpireTime { get; set; }

        public DeviceType DeviceType { get; set; }
        public int Soundtype { get; set; }
        public int Bdagetype { get; set; }
        public int Bdage { get; set; }
        public string AppActivity { get; set; }
        public int MessageType { get; set; }
        public IEnumerable<ExtraKey> ExtraKey { get; set; }
        public int MessageNavigationTypeId { get; set; }
        public MessageBoxShowType MessageBoxShowType { get; set; }

    }

    public class ExtraKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
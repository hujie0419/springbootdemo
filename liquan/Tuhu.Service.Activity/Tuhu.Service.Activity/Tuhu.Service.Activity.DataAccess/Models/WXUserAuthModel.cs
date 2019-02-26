using System;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class WXUserAuthModel
    {
        public int PKID { get; set; }

        public Guid? UserId { get; set; }

        public string Channel { get; set; }

        public string OpenId { get; set; }

        public string UnionId { get; set; }

        public string AuthSource { get; set; }

        public string Source { get; set; }

        public string BindingStatus { get; set; }

        public string AuthorizationStatus { get; set; }

        public string MetaData { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime? PushTime { get; set; }

        public string PushStatus { get; set; }

        public bool? IsSuccess { get; set; }

        public string MsgID { get; set; }
    }
}

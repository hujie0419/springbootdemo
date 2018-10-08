using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.DownloadApp
{
    public class DownloadAppException : TuhuBizException
    {
        public DownloadAppException()
        { }
        public DownloadAppException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public DownloadAppException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public DownloadAppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

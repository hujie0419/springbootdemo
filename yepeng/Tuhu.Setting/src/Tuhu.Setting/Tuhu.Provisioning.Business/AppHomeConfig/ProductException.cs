using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.AppHomeConfig
{
    public class ProductException : TuhuBizException
    {
         public ProductException()
        { }
        public ProductException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public ProductException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public ProductException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

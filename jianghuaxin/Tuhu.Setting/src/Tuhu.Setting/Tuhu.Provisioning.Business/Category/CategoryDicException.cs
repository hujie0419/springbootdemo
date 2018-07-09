using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Category
{
    public class CategoryDicException : TuhuBizException
    {
        public CategoryDicException()
        { }
        public CategoryDicException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public CategoryDicException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public CategoryDicException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

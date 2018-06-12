using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.ProductInfomationManagement
{
    public class ProductInfomationException : TuhuBizException
	{

		public ProductInfomationException()
		{ }

		public ProductInfomationException(int errorCode, string message)
			: base(errorCode, message)
		{ }

		public ProductInfomationException(int errorCode, string message, Exception innerException)
			: base(errorCode, message, innerException)
		{ }

		protected ProductInfomationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}


using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace Tuhu.Yewu.WinService.JobSchedulerService.DeliverySubscibe.Log
{
	internal class DeliveryCompanyConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			var model = loggingEvent.MessageObject as SubscibeLogModel;
			if (model != null)
			{
				writer.Write(model.DeliveryCompany);
			}
		}
	}

	internal class DeliveryCodeConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			var model = loggingEvent.MessageObject as SubscibeLogModel;
			if (model != null)
			{
				writer.Write(model.DeliveryCode);
			}
		}
	}

	internal class SubscibeResultConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			var model = loggingEvent.MessageObject as SubscibeLogModel;
			if (model != null)
			{
				writer.Write(model.SubscibeResult);
			}
		}
	}

	internal class ResponseContentConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			var model = loggingEvent.MessageObject as SubscibeLogModel;
			if (model != null)
			{
				writer.Write(model.ResponseContent);
			}
		}
	}
}

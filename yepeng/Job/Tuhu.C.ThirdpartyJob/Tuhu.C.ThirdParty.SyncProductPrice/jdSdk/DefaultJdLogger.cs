using System;
using System.Diagnostics;
using System.IO;
using Common.Logging;

namespace JdSdk
{
    /// <summary>
    /// 日志打点的简单实现。
    /// </summary>
    public class DefaultJdLogger : IJdLogger
	{
		private static readonly ILog _logger = LogManager.GetLogger(typeof(DefaultJdLogger));

		#region IJdLogger 成员

		public void Error(string message)
		{
			_logger.Error(message);
		}

		public void Warn(string message)
		{
			_logger.Warn(message);
		}

		public void Info(string message)
		{
			_logger.Info(message);
		}

		#endregion
    }
}

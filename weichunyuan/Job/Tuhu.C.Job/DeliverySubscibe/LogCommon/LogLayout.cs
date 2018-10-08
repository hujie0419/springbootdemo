using System;
using System.Collections;
using log4net.Layout;
using log4net.Util;

namespace Tuhu.Yewu.WinService.JobSchedulerService.DeliverySubscibe.Log
{
	public class LogLayout : PatternLayout
	{
		private static readonly Hashtable SGlobalRulesRegistry;
		static LogLayout()
		{
			SGlobalRulesRegistry = new Hashtable(4);

			SGlobalRulesRegistry.Add("DeliveryCompany", typeof(DeliveryCompanyConverter));
			SGlobalRulesRegistry.Add("DeliveryCode", typeof(DeliveryCodeConverter));
			SGlobalRulesRegistry.Add("SubscibeResult", typeof(SubscibeResultConverter));
			SGlobalRulesRegistry.Add("ResponseContent", typeof(ResponseContentConverter));
		}

		public LogLayout() : this(null) { }
		public LogLayout(string pattern)
			: base(pattern) { }

		protected override PatternParser CreatePatternParser(string pattern)
		{
			var parser = base.CreatePatternParser(pattern);

			foreach (DictionaryEntry entry in SGlobalRulesRegistry)
			{
				var info = new ConverterInfo
				{
					Name = (string)entry.Key,
					Type = (Type)entry.Value
				};
				parser.PatternConverters[entry.Key] = info;
			}

			return parser;
		}
	}
}

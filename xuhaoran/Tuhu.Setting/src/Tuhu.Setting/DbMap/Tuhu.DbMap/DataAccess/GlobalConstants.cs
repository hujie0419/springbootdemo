using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.DbMap.DataAccess
{
	public class GlobalConstants
	{
		internal static readonly Dictionary<string, string> DataBaseAccount = new Dictionary<string, string>()
		{
			["Activity"] = "Gungnir",
			["BaoYang"] = "Gungnir",
			["BaoYangSuggest"] = "Gungnir",
			["Configuration"] = "Gungnir",
			["Gungnir"] = "Gungnir",
			["Gungnir_Finance"] = "Gungnir",
			["Gungnir_History"] = "Gungnir",
			["KuaiXiu"] = "Gungnir",
			["ReportingDB"] = "Gungnir",
			["SystemLog"] = "Gungnir",
			["ThTmpDB"] = "Gungnir",
			["Tuhu_blade"] = "Gungnir",
			["Tuhu_crm"] = "Gungnir",
			["Tuhu_comment"]= "Gungnir",
			["Tuhu_oa"] = "Gungnir",
			["Tuhu_order"] = "Gungnir",
			["Tuhu_productcatalog"] = "Gungnir",
			["Tuhu_profiles"] = "Gungnir",
			["Monitor"] = "WmsReadOnly",
			["TMS"] = "WmsReadOnly",
			["WMS"] = "WmsReadOnly",
			["Cache"] = "Tuhu_log",
			["Marketing"] = "Tuhu_log",
			["SystemLog"] = "Tuhu_log",
			["Tuhu_bi"] = "Tuhu_log",
		    ["Tuhu_shop"] = "Tuhu_shop",
            ["Tuhu_huodong"] = "Tuhu_log",
			["Tuhu_log"] = "Tuhu_log",
			["Tuhu_log_History"] = "Tuhu_log",
			["Tuhu_notification"] = "Tuhu_log",
			["Tuhu_notification_history"] = "Tuhu_log",
			["Tuhu_oauth"] = "Tuhu_log",
			["Gungnir_Tbd"] = "Tuhu_Groupon",
			["Tuhu_tech"] = "Tuhu_Groupon",
			["Tuhu_wiki"] = "Tuhu_Groupon",
			["Tuhu_groupon"] = "Tuhu_Groupon",
			["Tuhu_finance"] = "ThirdParty",
			["Tuhu_thirdparty"] = "ThirdParty",
			["Tuhu_usertrack"] = "ThirdParty"
		};
	}
}
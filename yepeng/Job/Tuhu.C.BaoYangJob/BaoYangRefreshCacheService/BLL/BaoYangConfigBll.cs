using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BaoYangRefreshCacheService.DAL;
using BaoYangRefreshCacheService.Model;
using Common.Logging;
using System.Xml.Linq;

namespace BaoYangRefreshCacheService.BLL
{
    public static class BaoYangConfigBll
    {
        public static string GetConfigByConfigName(string configName)
        {
            string configXml = null;
            try
            {
                configXml = BaoYangConfigDal.GetConfigByConfigName(configName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return configXml;
        }
        /// <summary>
        /// 保养辅料
        /// </summary>
        /// <returns></returns>
        public static BaoYangAccessoryConfig GetPartAccessoryConfig()
        {
            const string configName = "BaoYangPartAccessoryConfig";
            BaoYangAccessoryConfig result = null;
            try
            {
                var configXml = GetConfigByConfigName(configName);
                using (var xmlReader = new StringReader(configXml))
                {
                    result = new XmlSerializer(typeof(BaoYangAccessoryConfig)).Deserialize(xmlReader) as BaoYangAccessoryConfig;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 适配配件
        /// </summary>
        /// <returns></returns>
        public static BaoYangAdaptationConfig GetBaoYangAdaptationConfig()
        {
            const string configName = "BaoYangAdaptation";
            BaoYangAdaptationConfig result = null;
            try
            {
                var configXml = GetConfigByConfigName(configName);
                using (var xmlReader = new StringReader(configXml))
                {
                    result = new XmlSerializer(typeof(BaoYangAdaptationConfig)).Deserialize(xmlReader) as BaoYangAdaptationConfig;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 获取适配覆盖率的配置
        /// </summary>
        /// <returns></returns>
        public static BaoYangKpiReportConfig GetBaoYangKpiReportConfig()
        {
            const string configName = "BaoYangKpiReportConfig";
            var xml = GetConfigByConfigName(configName);
            using (var xmlReader = new StringReader(xml))
            {
                var doc = XDocument.Load(xmlReader);
                var result = doc.Element("BaoYangKpiReportConfig").Element("CoverageRatio").Elements("Item").Select(node => new CoverageRatioConfigItem
                {
                    DisplayName = node.Attribute("displayName")?.Value,
                    Group = node.Attribute("group")?.Value,

                    Names = node.Attribute("names")?.Value
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(x => x.Trim())?.ToList() ?? new List<string>(),

                    Actions = node.Attribute("actions")?.Value
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(x => x.Trim())?.ToList() ?? new List<string>(),

                    RefType = node.Attribute("refType")?.Value
                }).ToList();

                var result2 = doc.Element("BaoYangKpiReportConfig").Element("TireCoverageRatio").Elements("Item").Select(node => new CoverageRatioConfigItem
                {
                    DisplayName = node.Attribute("displayName")?.Value,
                    Group = node.Attribute("group")?.Value,

                    Names = node.Attribute("names")?.Value
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(x => x.Trim())?.ToList() ?? new List<string>(),

                    Actions = node.Attribute("actions")?.Value
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(x => x.Trim())?.ToList() ?? new List<string>(),

                    RefType = node.Attribute("refType")?.Value
                }).ToList();

                var result3 = doc.Element("BaoYangKpiReportConfig").Element("AdaptationChanged").Elements("Item").Select(node => new CoverageRatioConfigItem
                {
                    DisplayName = node.Attribute("displayName")?.Value,
                    Group = node.Attribute("group")?.Value,

                    Names = node.Attribute("names")?.Value
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(x => x.Trim())?.ToList() ?? new List<string>(),

                    Actions = node.Attribute("actions")?.Value
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(x => x.Trim())?.ToList() ?? new List<string>(),

                    RefType = node.Attribute("refType")?.Value
                }).ToList();
                return new BaoYangKpiReportConfig { CoverageRatios = result, TireCoverageRatios = result2, AdaptationChangeds = result3 };
            }
        }
    }
}

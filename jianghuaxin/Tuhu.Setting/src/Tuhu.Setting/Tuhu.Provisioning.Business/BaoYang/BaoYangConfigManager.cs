using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.BaoYang
{
    public class BaoYangConfigManager
    {
        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbScopeManagerBY;

        public BaoYangConfigManager()
        {
            this.connectionManager =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            this.dbScopeManagerBY = new DBScopeManager(this.connectionManager);
        }

        public BaoYangRecommendConfig GetRecommendConfig()
        {
            string xml = GetConfigXml("BaoYangRecommendConfig");

            BaoYangRecommendConfig result = XmlHelper.Deserialize<BaoYangRecommendConfig>(xml);

            return result;
        }

        public string GetConfigXml(string configName)
        {
            var result = string.Empty;

            result = dbScopeManagerBY.Execute(conn => DalBaoYangConfig.SelectConfig(conn, configName));

            return result;
        }

        public EmailPersonsConfig GetEmailPersonsConfig()
        {
            string xml = GetConfigXml("EmailPersonsConfig");

            EmailPersonsConfig result = XmlHelper.Deserialize<EmailPersonsConfig>(xml);

            return result;
        }

        public BaoYangYearCardConfigs GetBaoYangYearCardConfig()
        {
            BaoYangYearCardConfigs result = new BaoYangYearCardConfigs();
            List<BaoYangYearCardConfig> data = new List<BaoYangYearCardConfig>();
            string xml = GetConfigXml("BaoYangYearCardConfig");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList nodes = doc.DocumentElement?.SelectNodes("BaoYangYearCardConfig");
            if (nodes != null)
            {
                foreach (XmlElement node in nodes)
                {
                    BaoYangYearCardConfig item = new BaoYangYearCardConfig();
                    var attr = node.GetElementsByTagName("Property")[0].Attributes;
                    if (attr != null)
                    {
                        item.Name = attr["name"].Value;
                        item.Type = attr["type"].Value;

                    }
                    item.Content = new List<YearCardContent>();
                    foreach (XmlElement con in node.GetElementsByTagName("Item"))
                    {
                        YearCardContent content = new YearCardContent
                        {
                            Name = con.HasAttribute("name") ? con.Attributes["name"].Value : string.Empty,
                            Category = con.HasAttribute("category") ? con.Attributes["category"].Value : string.Empty,
                            Type = con.HasAttribute("type") ? con.Attributes["type"].Value : string.Empty
                        };
                        item.Content.Add(content);
                    }
                    data.Add(item);
                }
            }
            result.BaoYangYearCardConfig = data;
            return result;
        }

        public List<string> ViscosityList()
        {
            const string configName = "BaoYangPartAccessoryConfig";
            var xml = GetConfigXml(configName);
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nodes = doc.DocumentElement.SelectSingleNode("PartAccessories").SelectNodes("BaoYangAccessory");
            string value = string.Empty;
            foreach(XmlElement item in nodes)
            {
                var node = item.SelectSingleNode("Viscosity");
                if (node != null)
                {
                    value = node.InnerText;
                }
            }
            var result = (value?.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList()) ?? new List<string>();
            return result;
        }
    }
}

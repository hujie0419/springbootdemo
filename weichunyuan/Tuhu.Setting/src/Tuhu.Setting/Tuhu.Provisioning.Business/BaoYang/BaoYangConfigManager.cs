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
using Tuhu.Provisioning.DataAccess.Entity.Config;

namespace Tuhu.Provisioning.Business.BaoYang
{
    public class BaoYangConfigManager
    {
        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbScopeManagerBY;
        private readonly IDBScopeManager dbBaoYangScopeManager;

        public BaoYangConfigManager()
        {
            this.connectionManager =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            this.dbScopeManagerBY = new DBScopeManager(this.connectionManager);
            this.dbBaoYangScopeManager = new DBScopeManager(new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang"].ConnectionString));
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
        /// <summary>
        /// 获取产品推荐备注配置信息列表
        /// </summary>
        /// <returns></returns>
        public List<ProductPriorityDescribeConfig> GetProductPriorityDescribeConfigs()
        {
            var result = new List<ProductPriorityDescribeConfig>();
            string xml = GetConfigXml("ProductPriorityDescribeConfig");
            if (!string.IsNullOrWhiteSpace(xml))
            {
                result = XmlHelper.Deserialize<ProductPriorityDescribeConfigs>(xml).Configs;
            }
            if (result == null || !result.Any())
            {
                result = new List<ProductPriorityDescribeConfig>();
            }
            return result;
        }
        /// <summary>
        /// 修改产品推荐备注配置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateProductPriorityDescribeConfig(List<ProductPriorityDescribeConfig> configs)
        {
            var result = false;
            ProductPriorityDescribeConfigs describeConfigs = new ProductPriorityDescribeConfigs() { Configs = configs };
            var configXml = XmlHelper.Serialize(describeConfigs);
            if (!string.IsNullOrWhiteSpace(configXml))
            {
               result = dbBaoYangScopeManager.Execute(conn => DalBaoYangConfig.UpdateConfig(conn, configXml, "ProductPriorityDescribeConfig"));
            }   
            return result;
        }
        /// <summary>
        /// 保存产品推荐备注信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveProductPriorityDescribeConfig(ProductPriorityDescribeConfig model)
        {
            var result = false;
            var configs = GetProductPriorityDescribeConfigs();
            if (configs == null)
            {
                configs = new List<ProductPriorityDescribeConfig>();
            }
            var config = configs.FirstOrDefault(x => x.PartName == model.PartName);
            if (config != null)
            {
                config.Value = model.Value;
                result = UpdateProductPriorityDescribeConfig(configs);
            }
            else
            {
                configs.Add(model);
                if (!IsExistsProductPriorityDescribeConfig("ProductPriorityDescribeConfig"))
                {
                     
                    result = InsertProductPriorityDescribeConfig(configs);
                }
                else
                { 
                    result = UpdateProductPriorityDescribeConfig(configs);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加产品推荐配置表
        /// </summary>
        /// <returns></returns>
        public bool InsertProductPriorityDescribeConfig(List<ProductPriorityDescribeConfig> configs)
        {
            ProductPriorityDescribeConfigs describeConfigs = new ProductPriorityDescribeConfigs() { Configs = configs };
            var configXml = XmlHelper.Serialize(describeConfigs);
            return dbBaoYangScopeManager.Execute(conn => DalBaoYangConfig.InsertConfig(conn, configXml, "ProductPriorityDescribeConfig"));
        }
        /// <summary>
        /// 判断是否有产品推荐描述的配置项
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public bool IsExistsProductPriorityDescribeConfig(string configName) => dbScopeManagerBY.Execute(conn => DalBaoYangConfig.IsExistsConfig(conn, configName));

        /// <summary>
        /// 获取优先级Map
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetPrioritySettingMapConfig()
        {
            var result = new Dictionary<string, string>();
            var xml = GetConfigXml("GlobalConfig");
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            var nodes = document.SelectNodes("/BaoYangConfig/TypeNameMap/PrioritySetting/Item");

            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null)
                    {
                        result.Add(node.Attributes["type"].Value, node.Attributes["partName"].Value);
                    }
                }
            }
            return result;
        }

    }
}

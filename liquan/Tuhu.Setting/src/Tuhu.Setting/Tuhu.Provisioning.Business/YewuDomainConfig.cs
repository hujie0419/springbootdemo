using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tuhu.Provisioning.Business
{
    public static class YewuDomainConfig
    {
        private static Dictionary<string, string> domainsDic;
        private static string topDomain;

        static YewuDomainConfig()
        {
            ConfigDomain();
            YewuSiteConfig.OnConfigChanged += ConfigDomain;
        }

        private static void ConfigDomain()
        {
            var domains = new Dictionary<string, string>();

            var configNode = YewuSiteConfig.GetConfigNode("domainConfig");
            foreach (XmlNode node in configNode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element && node.Attributes["name"] != null)
                {

                    domains[node.Attributes["name"].Value] = node.Attributes["site"].Value;
                }
            }

            domainsDic = domains;
            topDomain = configNode.Attributes["topDomain"].Value;
        }
        public static string GetSiteByName(string name)
        {
            string site = null;
            domainsDic.TryGetValue(name, out site);
            return site;
        }
        public static string FileSite
        {
            get { return GetSiteByName("FileSite"); }
        }
    }
}

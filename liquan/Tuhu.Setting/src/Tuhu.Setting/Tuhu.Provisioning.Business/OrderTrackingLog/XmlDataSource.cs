using System;
using System.Collections.Generic;
using System.Xml;

namespace Tuhu.Provisioning.Business.OrderTrackingLog
{
    public static class XmlDataSource
    {
        public static Dictionary<string, string> NodeCollection;
        static XmlDataSource()
        {
            LoadXmlToDictionary();
            FileWatch.OnConfigChanged += LoadXmlToDictionary;
        }
        private static void LoadXmlToDictionary()
        {
            var xmlModel = new XmlDocument();
            xmlModel.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "XML", "OrderTrackingLogMapping.xml"));
            var nodeCollection = new Dictionary<string, string>();
            XmlNodeList xxList = xmlModel.GetElementsByTagName("Exp");
            foreach (XmlNode node in xxList)
            {
                if (node.Attributes != null && (node.NodeType == XmlNodeType.Element && node.Attributes["Key"] != null))
                {
                    nodeCollection[node.Attributes["Key"].Value] = node.Attributes["Description"].Value;
                }
            }
            NodeCollection = nodeCollection;

        }
    }
}

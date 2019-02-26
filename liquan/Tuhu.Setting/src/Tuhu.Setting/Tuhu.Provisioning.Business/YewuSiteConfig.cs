using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using System.Xml;

namespace Tuhu.Provisioning.Business
{
    public static class YewuSiteConfig
    {
        private static Dictionary<string, XmlElement> _nodeCollection;

        private static Timer m_timer = new Timer(ReadSiteConfig);

        public static Action OnConfigChanged { get; set; }

        static YewuSiteConfig()
        {
            ReadSiteConfig(null);

            var fsw = new FileSystemWatcher(HostingEnvironment.MapPath("~/"));

            fsw.Filter = "Site.config";
            fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;

            fsw.Changed += new FileSystemEventHandler(fsw_Changed);

            fsw.EnableRaisingEvents = true;
        }

        private static void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            m_timer.Change(200, -1);
        }

        private static void ReadSiteConfig(object state)
        {
            var doc = new XmlDocument();
            doc.Load(HostingEnvironment.MapPath("~/Site.config"));

            var nodeCollection = new Dictionary<string, XmlElement>();
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                    nodeCollection[node.Name] = node as XmlElement;
            }
            _nodeCollection = nodeCollection;

            var temp = OnConfigChanged;
            if (temp != null)
                temp();
        }

        /// <summary>
        /// 获得配置节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>配置节点</returns>
        public static XmlElement GetConfigNode(string name)
        {
            XmlElement ele = null;
            _nodeCollection.TryGetValue(name, out ele);
            return ele;
        }

    }
}

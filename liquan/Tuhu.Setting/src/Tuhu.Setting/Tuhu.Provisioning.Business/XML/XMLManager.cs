using System.Collections.Generic;
using System.Web;
using System.Xml;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.XML
{
    public class XMLManager
    {
        /// <summary>
        /// Get XML node
        /// </summary>
        /// <param name="XMLFileName"></param>
        /// <param name="personNodeName"></param>
        /// <returns></returns>
        public List<Dictionary> GetXML(string XMLFileName, string personNodeName)
        {
            List<Dictionary> dics = new List<Dictionary>();
            var path = HttpContext.Current.Server.MapPath("/XML/" + XMLFileName + ".xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList personNodes = rootElem.GetElementsByTagName(personNodeName); //获取person子节点集合  
            foreach (XmlNode node in personNodes)
            {
                var dictionary = new Dictionary()
                {
                    DicKey = ((XmlElement)node).GetAttribute("name"),
                    DicValue = ((XmlElement)node).GetAttribute("value")
                };
                dics.Add(dictionary);
            }
            return dics;
        }
    }
}

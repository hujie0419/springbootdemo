using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tuhu.Provisioning.DataAccess.Entity.Config
{
    public class ProductPriorityDescribeConfig
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [XmlElement("PartName")]
        public string PartName { get; set; }
        /// <summary>
        /// 配置值
        /// </summary>
        [XmlElement("Value")]
        public string Value { get; set; }
    }

    [XmlRoot("ProductPriorityDescribeConfigs")]
    public class ProductPriorityDescribeConfigs
    {

        [XmlElement("ProductPriorityDescribeConfig")]
        public List<ProductPriorityDescribeConfig> Configs { get; set; }
    }
}

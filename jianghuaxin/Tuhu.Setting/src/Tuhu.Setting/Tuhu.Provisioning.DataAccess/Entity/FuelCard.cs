using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [XmlType]
    public class FuelCardConfig
    {
        [XmlArray("CardTypes")]
        public List<CardType> CardTypes { get; set; }
    }

    [XmlType]
    public class CardType
    {
        [XmlElement("TypeName")]
        public string TypeName { get; set; }

        [XmlArray("FuelCards")]
        public List<FuelCard> FuelCards { get; set; }

        [XmlElement("AnnounceMent")]
        public AnnounceMent AnnounceMent { get; set; }
    }

    [XmlType]
    public class FuelCard
    {
        [XmlAttribute("Pid")]
        public string Pid { get; set; }

        [XmlIgnore]
        public decimal Value { get; set; }

        [XmlIgnore]
        public decimal Price { get; set; }

        [XmlAttribute("IsActive")]
        public bool IsActive { get; set; }

        [XmlAttribute("VenderPKID")]
        public int VenderPKID { get; set; }

        [XmlAttribute("VenderID")]
        public string VenderID { get; set; }

        [XmlAttribute("VenderName")]
        public string VenderName { get; set; }

        [XmlAttribute("VenderShortName")]
        public string VenderShortName { get; set; }
    }

    [XmlType]
    public class AnnounceMent
    {
        [XmlAttribute("Text")]
        public string Text { get; set; }

        [XmlAttribute("Url")]
        public string Url { get; set; }
    }
}

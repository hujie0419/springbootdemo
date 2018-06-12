using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Tuhu.C.Job.PushProduct.Model
{
    [XmlRoot(ElementName = "urlset")]
    public class ProductDetailNodeList
    {
        [XmlElement("url")]
        public List<ProductDetailNode> productDetailNodeList { get; set; }
    }


    [Serializable]
    public class ProductDetailNode
    {
        [XmlIgnore]
        public string CDataLoc { get; set; }

        [XmlElement("loc")]
        public XmlCDataSection LocCDATA
        {
            get
            {
                return new System.Xml.XmlDocument().CreateCDataSection(CDataLoc);
            }
            set
            {
                CDataLoc = value.Value;
            }
        }


        [XmlElement("data")]
        public ProductDetail product { get; set; }
    }


    [Serializable]
    public class ImageValue
    {

        [XmlAttribute("index")]
        public int Index { get; set; }

        [XmlIgnore]
        public string CDataImg { get; set; }

       // [XmlElement("img")]
        public XmlCDataSection ImgCDATA
        {
            get
            {
                return new System.Xml.XmlDocument().CreateCDataSection(CDataImg);
            }
            set
            {
                CDataImg = value.Value;
            }
        }



    }

    [Serializable]
    public class ProductDetail
    {
        [XmlIgnore]
        public string CDataName { get; set; }

        [XmlIgnore]
        public string CDataOuterID { get; set; }


        [XmlIgnore]
        public string CDataSellerName { get; set; }

        [XmlIgnore]
        public string CDataSellerSiteUrl { get; set; }

        [XmlIgnore]
        public string CDataLogo = "https://img5-tuhu-cn.alikunlun.com/Home/Image/D7787D8D722B649D6937187C5BADC612.png@100Q.png";

        [XmlIgnore]
        public string CDataTitle { get; set; }

        [XmlIgnore]
        public string CDataImage { get; set; }


       

        [XmlIgnore]
        public string CDataBrand { get; set; }

        [XmlIgnore]
        public string CDataTargetUrl { get; set; }

        [XmlIgnore]
        public string CDataCategory { get; set; }

        [XmlIgnore]
        public string CDataSubCategory { get; set; }

        [XmlIgnore]
        public string CDataThirdCategory { get; set; }

        [XmlIgnore]
        public string CDataFourthCategory { get; set; }

        [XmlIgnore]
        public string CDataCategoryUrl { get; set; }

        [XmlIgnore]
        public string CDataSubCategoryUrl { get; set; }

      

        [XmlIgnore]
        public string CDataThirdCategoryUrl { get; set; }

        [XmlIgnore]
        public string CDataFourthCategoryUrl { get; set; }

        [XmlIgnore]
        public string CDataModel { get; set; }

        [XmlIgnore]
        public string CDataServices { get; set; }

        [XmlIgnore]
        public string CDataTags { get; set; }

        [XmlIgnore]
        public string CDataAvailability { get; set; }
             

        [XmlElement("name")]
        public XmlCDataSection NameCDATA
        {
            get
            {
                return new System.Xml.XmlDocument().CreateCDataSection(CDataName);
            }
            set
            {
                CDataName = value.Value;
            }
        }


        [XmlElement("outerID")]
        public XmlCDataSection OuterIDCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataOuterID);
            }
            set
            {
                CDataOuterID = value.Value;
            }
        }


        [XmlElement("sellerName")]
        public XmlCDataSection SellerNameCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataSellerName);
            }
            set
            {
                CDataSellerName = value.Value;
            }
        }

        [XmlElement("sellerSiteUrl")]
        public XmlCDataSection SellerSiteUrlCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataSellerSiteUrl);
            }
            set
            {
                CDataSellerSiteUrl = value.Value;
            }
        }


        [XmlElement("logo")]
        public XmlCDataSection LogoCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataLogo);
            }
            set
            {
                CDataLogo = value.Value;
            }
        }


        [XmlElement("title")]
        public XmlCDataSection TitleCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataTitle);
            }
            set
            {
                CDataTitle = value.Value;
            }
        }


        [XmlElement("image")]
        public XmlCDataSection ImageCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataImage);
            }
            set
            {
                CDataImage = value.Value;
            }
        }


        [XmlArray("moreImages")]
        [XmlArrayItem("img")]
        public List<ImageValue> ImageValues { get; set; }


        [XmlElement("price")]
        public string CDataPrice { get; set; }


        [XmlElement("brand")]
        public XmlCDataSection BrandCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataBrand);
            }
            set
            {
                CDataBrand = value.Value;
            }
        }


        [XmlElement("targetUrl")]
        public XmlCDataSection TargetUrlCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataTargetUrl);
            }
            set
            {
                CDataTargetUrl = value.Value;
            }
        }

        [XmlElement("category")]
        public XmlCDataSection CategoryCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataCategory);
            }
            set
            {
                CDataCategory = value.Value;
            }
        }



        [XmlElement("subCategory")]
        public XmlCDataSection SubCategoryCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataSubCategory);
            }
            set
            {
                CDataSubCategory = value.Value;
            }
        }


        [XmlElement("thirdCategory")]
        public XmlCDataSection ThirdCategoryCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataThirdCategory);
            }
            set
            {
                CDataThirdCategory = value.Value;
            }
        }

        [XmlElement("categoryUrl")]
        public XmlCDataSection CategoryUrlCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataCategoryUrl);
            }
            set
            {
                CDataCategoryUrl = value.Value;
            }
        }

        [XmlElement("subCategoryUrl")]
        public XmlCDataSection SubCategoryUrlCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataSubCategoryUrl);
            }
            set
            {
                CDataSubCategoryUrl = value.Value;
            }
        }


        [XmlElement("thirdCategoryUrl")]
        public XmlCDataSection ThirdCategoryUrlCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataThirdCategoryUrl);
            }
            set
            {
                CDataThirdCategoryUrl = value.Value;
            }
        }


       

        [XmlElement("tags")]
        public XmlCDataSection TagsCDATA
        {
            get
            {
                return new XmlDocument().CreateCDataSection(CDataTags);
            }
            set
            {
                CDataTags = value.Value;
            }
        }
        [XmlElement("availability")]
        public int Availability { get; set; }

        [XmlElement("bought")]
        public int Bought { get; set; }

        [XmlElement("choice")]
        public ChoicePropertyList choiceProperties { get; set; }
    }

    [Serializable]
    public class SpecifyProperty
    {
        [XmlIgnore]
        public string CDataKey { get; set; }

        [XmlElement("key")]
        public XmlCDataSection KeyCDATA
        {
            get
            {
                return new System.Xml.XmlDocument().CreateCDataSection(CDataKey);
            }
            set
            {
                CDataKey = value.Value;
            }
        }


        [XmlIgnore]
        public string CDataValue { get; set; }

        [XmlElement("value")]
        public XmlCDataSection ValueCDATA
        {
            get
            {
                return new System.Xml.XmlDocument().CreateCDataSection(CDataValue);
            }
            set
            {
                CDataValue = value.Value;
            }
        }
    }



    [Serializable]
    public class ChoicePropertyList
    {
        [XmlElement("attribute")]
        public List<SpecifyProperty> SpecifyPropertyList { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.New;
using Tuhu.C.Job.PushProduct.DAL;
using Tuhu.C.Job.PushProduct.Model;

namespace Tuhu.C.Job.PushProduct.BLL
{
    public class ProductBusiness
    {

        public static List<SkuProductDetailModel> GetProductDetailModelList()
        {
            IEnumerable<string> pids = BaseDataDAL.GetPIDList();

            List<SkuProductDetailModel> productdetailList = new List<SkuProductDetailModel>();

            if (pids != null && pids.Any())
            {
                using (var client = new ProductClient())
                {
                    int count = pids.Count() / 100;
                    List<string> pidList = new List<string>();
                    for (int i = 0; i <= count; i++)
                    {
                        pidList = pids.Skip(i * 100).Take(100).ToList();

                        var response = client.SelectSkuProductListByPids(pidList);
                        if (response != null && response.Result != null)
                        {
                            productdetailList.AddRange(response.Result);
                        }
                    }


                }
            }
            return productdetailList;
        }

        public static void GenerateProductFile()
        {
            List<SkuProductDetailModel> productdetailList = GetProductDetailModelList();
            ProductDetailNodeList nodelist = new ProductDetailNodeList()
            {
                productDetailNodeList = new List<ProductDetailNode>()
            };

            if (productdetailList != null && productdetailList.Any())
            {
                foreach (var productdetail in productdetailList.Where(q => q != null))
                {
                    ProductDetail detail = new ProductDetail();
                    if (productdetail.RootCategoryName == "hub")
                    {
                        detail = CreateHubProductDetail(productdetail);
                        nodelist.productDetailNodeList.Add(new ProductDetailNode()
                        {
                            CDataLoc = detail.CDataTargetUrl,
                            product = detail
                        });
                    }
                    //AutoProduct
                    else if (productdetail.RootCategoryName == "Tires")
                    {
                        detail = CreateTireProductDetail(productdetail);
                        nodelist.productDetailNodeList.Add(new ProductDetailNode()
                        {
                            CDataLoc = detail.CDataTargetUrl,
                            product = detail
                        });
                    }
                    else if (productdetail.RootCategoryName == "AutoProduct")
                    {
                        detail = CreateVehicleProductDetail(productdetail);
                        nodelist.productDetailNodeList.Add(new ProductDetailNode()
                        {
                            CDataLoc = detail.CDataTargetUrl,
                            product = detail
                        });
                    }


                }
            }





            string filename = string.Format("TuhuProducts_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd"));
            //
            XmlSerializer serializer = new XmlSerializer(typeof(ProductDetailNodeList));
            string str = string.Empty;
            using (StreamWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, nodelist);
            }
            using (StreamReader reader = new StreamReader(filename))
            {
                str = reader.ReadToEnd();
                str = str.Replace("<ImgCDATA>", "");
                str = str.Replace("</ImgCDATA>", "");

            }
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(str);
                writer.Flush();
            }

            using (var smtp = new SmtpClient())
            using (var mail = new MailMessage())
            {
                mail.Subject = "TuhuProductsFile";

                foreach (var a in ConfigurationManager.AppSettings["SEOFileJob:To"].Split(';'))
                {
                    mail.To.Add(a);
                }

                mail.Attachments.Add(new Attachment(filename));

                smtp.Send(mail);
            }
        }

        public static ProductDetail CreateHubProductDetail(SkuProductDetailModel productHubdetail)
        {

            SpecifyProperty property8 = new SpecifyProperty()
            {
                CDataKey = "移动地址",
                CDataValue = string.Format("https://wx.tuhu.cn/Products/Details?pid={0}&source=wxshantou", !string.IsNullOrWhiteSpace(productHubdetail.VariantID) ? productHubdetail.ProductId + "&vid=" + productHubdetail.VariantID : productHubdetail.ProductId)
            };

            SpecifyProperty property1 = new SpecifyProperty()
            {
                CDataKey = "尺寸",
                CDataValue = productHubdetail.ProductSize
            };
            SpecifyProperty property2 = new SpecifyProperty()
            {
                CDataKey = "中心孔距",
                CDataValue = productHubdetail.CB
            };
            SpecifyProperty property3 = new SpecifyProperty()
            {
                CDataKey = "偏距",
                CDataValue = productHubdetail.ET
            };
            SpecifyProperty property4 = new SpecifyProperty()
            {
                CDataKey = "孔数",
                CDataValue = productHubdetail.H
            };
            SpecifyProperty property5 = new SpecifyProperty()
            {
                CDataKey = "孔距",
                CDataValue = productHubdetail.PCD
            };
            SpecifyProperty property6 = new SpecifyProperty()
            {
                CDataKey = "宽度",
                CDataValue = productHubdetail.Width
            };
            SpecifyProperty property7 = new SpecifyProperty()
            {
                CDataKey = "颜色",
                CDataValue = productHubdetail.ProductColor
            };

            ChoicePropertyList propertyList = new ChoicePropertyList()
            {
                SpecifyPropertyList = new List<SpecifyProperty>() { property8, string.IsNullOrWhiteSpace(property1.CDataValue) ? null : property1, string.IsNullOrWhiteSpace(property2.CDataValue) ? null : property2, string.IsNullOrWhiteSpace(property3.CDataValue) ? null : property3, string.IsNullOrWhiteSpace(property4.CDataValue) ? null : property4, string.IsNullOrWhiteSpace(property5.CDataValue) ? null : property5, string.IsNullOrWhiteSpace(property6.CDataValue) ? null : property6, string.IsNullOrWhiteSpace(property7.CDataValue) ? null : property7 }
            };

            List<string> imageUrls = new List<string>();
            if (productHubdetail.ImageUrls != null && productHubdetail.ImageUrls.Any())
            {
                foreach (var imageUrl in productHubdetail.ImageUrls)
                {
                    imageUrls.Add("https://img3.tuhu.org" + imageUrl + "@484w_300h_100Q.jpg");
                    imageUrls.Add("https://img3.tuhu.org" + imageUrl + "@400w_400h_100Q.jpg");
                }
            }



            ProductDetail detail = new ProductDetail()
            {
                CDataName = productHubdetail.DisplayName,
                CDataOuterID = productHubdetail.Pid,
                CDataSellerName = "途虎",
                CDataSellerSiteUrl = "https://www.tuhu.cn/",
                CDataTitle = productHubdetail.ShuXing5,
                CDataImage = imageUrls[0],
                // CDataMoreImages = "<img index=\"1\">https://img3.tuhu.org/Images/Products/1607/47B48818B4002255.jpg@100w_100h_100Q.jpg</img><img index=\"2\">https://img3.tuhu.org/Images/Products/1607/ADCAA3A068A026D0.jpg@100w_100h_100Q.jpg</img>",
                ImageValues = imageUrls.Skip(1).Select(s => new ImageValue()
                {
                    Index = imageUrls.IndexOf(s),
                    CDataImg = s
                }).ToList(),
                CDataPrice = productHubdetail.Price.ToString(),
                CDataBrand = productHubdetail.Brand,
                CDataTargetUrl = "http://item.tuhu.cn/Products/" + productHubdetail.ProductId + (!string.IsNullOrWhiteSpace(productHubdetail.VariantID) ? "/" + productHubdetail.VariantID + ".html" : ".html") + "?source=pcshantou",
                CDataCategory = "轮毂",
                CDataSubCategory = productHubdetail.Category.Replace("hub", "寸") + "轮毂",
                CDataCategoryUrl = "http://item.tuhu.cn/hub.html",
                CDataSubCategoryUrl = "http://item.tuhu.cn/List/" + productHubdetail.Category + "/",
                Availability = 1,
                Bought = productHubdetail.SalesQuantity,
                choiceProperties = propertyList
            };
            return detail;
        }


        public static ProductDetail CreateTireProductDetail(SkuProductDetailModel productTiredetail)
        {
            List<ProductCategory> productCategories = null;
            SpecifyProperty property25 = new SpecifyProperty()
            {
                CDataKey = "移动地址",
                CDataValue = string.Format("https://wx.tuhu.cn/Products/Details?pid={0}&source=wxshantou", !string.IsNullOrWhiteSpace(productTiredetail.VariantID) ? productTiredetail.ProductId + "&vid=" + productTiredetail.VariantID : productTiredetail.ProductId)
            };

            SpecifyProperty property21 = new SpecifyProperty()
            {
                CDataKey = "产品规格",
                CDataValue = productTiredetail.Size.Width + "/" + productTiredetail.Size.AspectRatio + "R" + productTiredetail.Size.Rim
            };
            SpecifyProperty property22 = new SpecifyProperty()
            {
                CDataKey = "速度级别",
                CDataValue = productTiredetail.SpeedRating
            };
            SpecifyProperty property23 = new SpecifyProperty()
            {
                CDataKey = "轮胎花纹",
                CDataValue = productTiredetail.Pattern
            };
            SpecifyProperty property24 = new SpecifyProperty()
            {
                CDataKey = "重量",
                CDataValue = productTiredetail.Weight
            };

            ChoicePropertyList propertyList2 = new ChoicePropertyList()
            {
                SpecifyPropertyList = new List<SpecifyProperty>() { property25, string.IsNullOrWhiteSpace(property21.CDataValue) ? null : property21, string.IsNullOrWhiteSpace(property22.CDataValue) ? null : property22, string.IsNullOrWhiteSpace(property23.CDataValue) ? null : property23, string.IsNullOrWhiteSpace(property24.CDataValue) ? null : property24 }
            };


            List<string> imageUrls = new List<string>();
            if (productTiredetail.ImageUrls != null && productTiredetail.ImageUrls.Any())
            {
                foreach (var imageUrl in productTiredetail.ImageUrls)
                {
                    imageUrls.Add("https://img3.tuhu.org" + imageUrl + "@484w_300h_100Q.jpg");
                    imageUrls.Add("https://img3.tuhu.org" + imageUrl + "@400w_400h_100Q.jpg");
                }
            }

            if (productTiredetail.NodeNo.Split(new char[] { '.' }).Count() >= 2)
            {
                string oids = productTiredetail.NodeNo.Replace('.', ',');
                productCategories = BaseDataDAL.GetProductCategoryByOid(oids).ToList();
                productCategories = productCategories.OrderByDescending(s => s.DescendantProductCount).ToList();


            }

            ProductDetail detail = new ProductDetail()
            {
                CDataName = productTiredetail.DisplayName,
                CDataOuterID = productTiredetail.Pid,
                CDataSellerName = "途虎",
                CDataSellerSiteUrl = "https://www.tuhu.cn/",
                CDataTitle = productTiredetail.ShuXing5,
                CDataImage = imageUrls[0],
                // CDataMoreImages = "<img index=\"1\">https://img3.tuhu.org/Images/Products/1607/47B48818B4002255.jpg@100w_100h_100Q.jpg</img><img index=\"2\">https://img3.tuhu.org/Images/Products/1607/ADCAA3A068A026D0.jpg@100w_100h_100Q.jpg</img>",
                // ImgValues = images,
                ImageValues = imageUrls.Skip(1).Select(s => new ImageValue()
                {
                    Index = imageUrls.IndexOf(s),
                    CDataImg = s
                }).ToList(),
                CDataPrice = productTiredetail.Price.ToString(),
                CDataBrand = productTiredetail.Brand,
                CDataTargetUrl = "http://item.tuhu.cn/Products/" + productTiredetail.ProductId + (!string.IsNullOrWhiteSpace(productTiredetail.VariantID) ? "/" + productTiredetail.VariantID + ".html" : ".html") + "?source=pcshantou",
                CDataCategory = "轮胎",
                CDataSubCategory = productCategories[1] != null ? productCategories[1].DisplayName : "越野胎",
                CDataCategoryUrl = "http://item.tuhu.cn/Tires.html",
                CDataSubCategoryUrl = "http://item.tuhu.cn/List/" + productCategories[1] != null ? productCategories[1].CategoryName : "TireCross" + "/",
                CDataTags = productTiredetail.CP_Tab,
                Availability = 1,
                Bought = productTiredetail.SalesQuantity,
                choiceProperties = propertyList2
            };
            return detail;
        }


        public static ProductDetail CreateVehicleProductDetail(SkuProductDetailModel productVehicledetail)
        {
            List<ProductCategory> productCategories = null;

            SpecifyProperty property35 = new SpecifyProperty()
            {
                CDataKey = "移动地址",
                CDataValue = string.Format("https://wx.tuhu.cn/Products/Details?pid={0}&source=wxshantou", !string.IsNullOrWhiteSpace(productVehicledetail.VariantID) ? productVehicledetail.ProductId + "&vid=" + productVehicledetail.VariantID : productVehicledetail.ProductId)
            };

            SpecifyProperty property31 = new SpecifyProperty()
            {
                CDataKey = "重量",
                CDataValue = productVehicledetail.Weight
            };
            SpecifyProperty property32 = new SpecifyProperty()
            {
                CDataKey = "颜色",
                CDataValue = productVehicledetail.Color
            };

            ChoicePropertyList propertyList3 = new ChoicePropertyList()
            {
                SpecifyPropertyList = new List<SpecifyProperty>() { property35, string.IsNullOrWhiteSpace(property31.CDataValue) ? null : property31, string.IsNullOrWhiteSpace(property32.CDataValue) ? null : property32 }
            };





            List<string> imageUrls = new List<string>();
            if (productVehicledetail.ImageUrls != null && productVehicledetail.ImageUrls.Any())
            {
                foreach (var imageUrl in productVehicledetail.ImageUrls)
                {
                    imageUrls.Add("https://img3.tuhu.org" + imageUrl + "@484w_300h_100Q.jpg");
                    imageUrls.Add("https://img3.tuhu.org" + imageUrl + "@400w_400h_100Q.jpg");
                }
            }

            if (productVehicledetail.NodeNo.Split(new char[] { '.' }).Count() >= 2)
            {
                string oids = productVehicledetail.NodeNo.Replace('.', ',');
                productCategories = BaseDataDAL.GetProductCategoryByOid(oids).ToList();
                productCategories = productCategories.OrderByDescending(s => s.DescendantProductCount).ToList();


            }

            ProductDetail detail = new ProductDetail()
            {
                CDataName = productVehicledetail.DisplayName,
                CDataOuterID = productVehicledetail.Pid,
                CDataSellerName = "途虎",
                CDataSellerSiteUrl = "https://www.tuhu.cn/",
                CDataTitle = productVehicledetail.ShuXing5,
                CDataImage = imageUrls[0],
                //CDataMoreImages = "<img index=\"1\">https://img3.tuhu.org/Images/Products/TW-DASUN-DSXL_02.jpg@100w_100h_100Q.jpg</img><img index=\"2\">https://img3.tuhu.org/Images/Products/TW-DASUN-DSXL_03.jpg@100w_100h_100Q.jpg</img><img index=\"3\">https://img3.tuhu.org/Images/Products/TW-DASUN-DSXL_04.jpg@100w_100h_100Q.jpg</img><img index=\"4\">https://img3.tuhu.org/Images/Products/TW-DASUN-DSXL_05.jpg@100w_100h_100Q.jpg</img>",
                ImageValues = imageUrls.Skip(1).Select(s => new ImageValue()
                {
                    Index = imageUrls.IndexOf(s),
                    CDataImg = s
                }).ToList(),
                CDataPrice = productVehicledetail.Price.ToString(),
                CDataBrand = productVehicledetail.Brand,
                CDataTargetUrl = "http://item.tuhu.cn/Products/" + productVehicledetail.ProductId + (!string.IsNullOrWhiteSpace(productVehicledetail.VariantID) ? "/" + productVehicledetail.VariantID + ".html" : ".html") + "?source=pcshantou",
                CDataCategory = "车品",
                CDataSubCategory = productCategories[1].DisplayName,
                CDataThirdCategory = productCategories[2].DisplayName,
                CDataCategoryUrl = "http://item.tuhu.cn/List/AutoProduct/",
                CDataSubCategoryUrl = "http://item.tuhu.cn/List/" + productCategories[1].CategoryName + "/",
                CDataThirdCategoryUrl = "http://item.tuhu.cn/List/" + productCategories[2].CategoryName + "/",
                Availability = 1,
                Bought = productVehicledetail.SalesQuantity,
                choiceProperties = propertyList3
            };
            return detail;
        }

    }
}

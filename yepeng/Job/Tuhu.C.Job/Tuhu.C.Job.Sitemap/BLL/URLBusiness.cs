using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;
using Tuhu.Service.Vehicle;
using Tuhu.Service.Vehicle.Model;
using Tuhu.C.Job.Sitemap.DAL;
using Tuhu.C.Job.Sitemap.Models;

namespace Tuhu.C.Job.Sitemap.BLL
{
    class URLBusiness
    {

        public static List<string> hotTiresizes = new List<string> { "195/65R15", "205/55R16", "195/55R15", "185/60R14", "235/55R17", "205/55R16", "185/60R14", "195/60R15", "215/70R15", "185/65R14", "195/60R14", "175/65R14", "215/55R16", "185/65R15", "215/60R16", "205/65R15", "225/60R16", "205/50R16", "205/60R15", "215/65R16" };
        public void DoWithSitemapFile()
        {
            if (!File.Exists(@"www.tuhu.cn/sitemap/sitemap.xml"))
            {
                File.Create(@"www.tuhu.cn/sitemap/sitemap.xml");
                using (FileStream filestream = new FileStream(@"www.tuhu.cn/sitemap/sitemap.xml", FileMode.CreateNew))
                {
                    using (StreamWriter writer = new StreamWriter(filestream))
                    {
                        // writer.WriteLine(@"<?xml version="1.0" encoding="utf-8"?>");
                        //  writer.WriteLine("<sitemapindex>");

                    }
                }
            }
        }

        public static bool CreateFiles()
        {
            bool flag = true;
            try
            {
                CreateBaoYangFile();
                CreateShopFile();
            }
            catch (Exception ex)
            {
                flag = false;
                WeeklySitemapJob.Logger.Error(ex);
            }
            return flag;
        }

        public static void CreateBaoYangFile()
        {
            List<string> baoyangUrlList = new List<string>();
            IEnumerable<VehicleInfo> Vehicles = BaseDataDAL.GetVehicleInfo(true);
            if (Vehicles.Any())
            {
                WeeklySitemapJob.Logger.Info("有" + Vehicles.Count() + "个保养地址需要收录");
            }

            if (Vehicles != null && Vehicles.Any())
            {
                foreach (var vehicle in Vehicles.Where(q => q != null))
                {
                    for (int i = vehicle.StartYear; i <= vehicle.EndYear; i++)
                    {
                        string title = vehicle.Brand + vehicle.VehicleSeries + "系列" + vehicle.PaiLiang + i + "年保养";
                        baoyangUrlList.Add(string.Format("<li><a target=\"_blank\" title=\"{3}\" href=\"http://by.tuhu.cn/baoyang/{0}/pl{1}-n{2}.html\">{3}</a></li>", vehicle.VehicleID, vehicle.PaiLiang, i, title));
                    }
                }
            }
            Stream baoyangstream = GenerateStreamFromString(baoyangUrlList);

            Attachment fileattachment = new Attachment(baoyangstream, "SEOBaoYang.html");
            SendMail("SEOBaoYangHtml", ConfigurationManager.AppSettings["SEOFileJob:To"], fileattachment);

        }

        public static void SendMail(string subject, string to, Attachment attachment)
        {
            using (var smtp = new SmtpClient())
            using (var mail = new MailMessage())
            {
                mail.Subject = subject;

                foreach (var a in to.Split(';'))
                {
                    mail.To.Add(a);
                }

                mail.Attachments.Add(attachment);

                smtp.Send(mail);
            }
        }

        public static void CreateShopFile()
        {
            List<string> shopUrlList = new List<string>();
            IEnumerable<ShopInfo> shops = BaseDataDAL.GetShopListInfo(true);
            IEnumerable<ShopInfo> detailshops = BaseDataDAL.GetShopDetailInfo(true);
            if (shops.Any())
            {
                WeeklySitemapJob.Logger.Info("有" + shops.Count() + "个门店地址需要收录");
            }
            if (shops != null && shops.Any())
            {
                foreach (var shop in shops.Where(q => q != null))
                {
                    string title = shop.RegionName + "门店";
                    shopUrlList.Add(string.Format("<li><a target=\"_blank\" title=\"{2}\" href=\"http://www.tuhu.cn/shops/{0}{1}.aspx\">{2}</a></li>", shop.RegionNamePinYin, shop.RegionId, title));
                }
            }
            //if (detailshops != null && detailshops.Any())
            //{
            //    foreach (var shop in detailshops.Where(q => q != null))
            //    {
            //        //string title = shop.ShopName + "门店";
            //        shopUrlList.Add(string.Format("<li><a target=\"_blank\" title=\"{3}\" href=\"http://www.tuhu.cn/shop/{0}{1}/{2}.aspx\">{3}</a></li>", shop.RegionNamePinYin, shop.RegionId, shop.ShopId,shop.ShopName));
            //    }
            //}
            Stream shopstream = GenerateStreamFromString(shopUrlList);
            Attachment fileattachment = new Attachment(shopstream, "SEOShop.html");
            SendMail("SEOShopHTML", ConfigurationManager.AppSettings["SEOFileJob:To"], fileattachment);
        }

        public static MemoryStream GenerateStreamFromString(List<string> strList)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            foreach (var str in strList)
            {
                writer.WriteLine(str);
                writer.Flush();
            }
            stream.Position = 0;
            return stream;
        }

        public static MemoryStream GenerateStreamFromString(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static bool InsertWeeklyURLToDB(bool isFull)
        {
            bool insertsuccess = false;
            InsertPaintAndInsuranceURL(isFull);
            var tireresult = InsertTireURL(isFull);
            var tirebrandresult = InsertTireBrandURL(isFull);
            var productresult = InsertProductURL(isFull);
            var shoplistresult = InsertShopListURL(isFull);
            var shopdetailresult = InsertShopDetailURL(isFull);
            var baoyangresult = InsertBaoYangURL(isFull);
            var articleresult = InsertArticleURL(isFull);
            var hubresult = InsertHubURL(isFull);
            if (tireresult && tirebrandresult && productresult && shoplistresult && shopdetailresult && baoyangresult && articleresult && hubresult)
            {
                insertsuccess = true;
            }
            return insertsuccess;
        }

        public static bool GenerateSitemapFile(bool isFull)
        {
            bool flag = true;
            try
            {
                List<SitemapNode> sitemapNodeList = new List<SitemapNode>();
                IEnumerable<UrlNode> urlNodes = BaseDataDAL.GetUrls(isFull);
                if (urlNodes != null && urlNodes.Any())
                {
                    foreach (var urlNode in urlNodes)
                    {
                        sitemapNodeList.Add(new SitemapNode()
                        {
                            Url = urlNode.URL,
                            Frequency = SitemapFrequency.Weekly,
                            Priority = 2,
                            LastModified = DateTime.Now
                        });

                    }

                }
                WeeklySitemapJob.Logger.Info("有" + sitemapNodeList.Count() + "个URL需要导入文件");
                int count = sitemapNodeList.Count() / 20000;
                List<string> filelist = new List<string>();
                for (int i = 0; i <= count; i++)
                {

                    var sitemapList = sitemapNodeList.Skip(i * 20000).Take(20000);
                    string sitemapstr = GetSitemapDocument(sitemapList);
                    MemoryStream baoyangstream = GenerateStreamFromString(sitemapstr);
                    string filename = string.Format("Sitemap_{0}_{1}.xml", DateTime.Now.ToString("yyyy-MM-dd"), i);
                    using (FileStream filestream = new FileStream(filename, FileMode.Create))
                    {
                        baoyangstream.WriteTo(filestream);
                        filelist.Add(filename);
                    }
                }
                AddFileToZip("sitemap.zip", filelist);
                Attachment fileattachment = new Attachment("sitemap.zip");
                SendMail("SitemapFiles", ConfigurationManager.AppSettings["SEOFileJob:To"], fileattachment);
            }
            catch (Exception ex)
            {
                flag = false;
                WeeklySitemapJob.Logger.Error(ex);
            }
            return flag;
        }


        private static void AddFileToZip(string zipFilename, List<string> filesToAdd, CompressionOption compression = CompressionOption.Normal)
        {
            using (Package zip = Package.Open(zipFilename, FileMode.OpenOrCreate))
            {
                foreach (var file in filesToAdd)
                {
                    string destFilename = file;
                    Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                    if (zip.PartExists(uri))
                    {
                        zip.DeletePart(uri);
                    }

                    PackagePart part = zip.CreatePart(uri, "", compression);
                    using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream dest = part.GetStream())
                        {
                            fileStream.CopyTo(dest);
                        }
                    }
                }

            }
        }


        public static void InsertPaintAndInsuranceURL(bool isFull)
        {
            //bool insertSuccess = false;
            if (isFull)
            {
                InsertURLToDB(new List<string> { "https://www.tuhu.cn/" }, "Home");
                InsertURLToDB(new List<string> { "http://www.tuhu.cn/Spages/CheXian.aspx" }, "Chexian");
                InsertURLToDB(new List<string> { "https://item.tuhu.cn/Activity/Act/PenQi" }, "PenQi");
                InsertURLToDB(new List<string> { "http://www.tuhu.cn/ChePin/" }, "ProductList");
            }
            else
            {
                WeeklySitemapJob.Logger.Info("官网,车险和喷漆地址已导入");
            }
        }

        public static bool InsertArticleURL(bool isFull = false)
        {
            List<string> articleUrlList = new List<string>();
            IEnumerable<int> articleIds = BaseDataDAL.GetArticleID(isFull);
            if (articleIds != null)
            {
                foreach (var article in articleIds.Where(q => q > 0))
                {
                    articleUrlList.Add(string.Format("http://www.tuhu.cn/Community/detail/{0}.aspx", article));
                }
            }
            if (isFull)
            {
                InsertURLToDB(new List<string> { "http://www.tuhu.cn/Community/","http://www.tuhu.cn/Community/Discovery.aspx", "http://www.tuhu.cn/Community/Discovery.aspx?tagId=1", "http://www.tuhu.cn/Community/Discovery.aspx?tagId=1344",
                "http://www.tuhu.cn/Community/Discovery.aspx?tagId=6","http://www.tuhu.cn/Community/Discovery.aspx?tagId=21","http://www.tuhu.cn/Community/Discovery.aspx?tagId=61","http://www.tuhu.cn/Community/Discovery.aspx?tagId=4"}, "DiscoveryList");
            }
            WeeklySitemapJob.Logger.Info("有" + articleUrlList.Count() + "需要导入库");
            return InsertURLToDB(articleUrlList, "DiscoveryDetail");
        }

        public static bool InsertProductURL(bool isFull = false)
        {
            List<string> productUrlList = new List<string>();
            IEnumerable<Product> products = BaseDataDAL.GetProducts(isFull);
            if (products != null && products.Any())
            {
                foreach (var product in products.Where(q => q != null))
                {
                    productUrlList.Add(string.Format("http://item.tuhu.cn/Products{0}{1}.html", !string.IsNullOrWhiteSpace(product.ProductID) ? "/" + product.ProductID : string.Empty, !string.IsNullOrWhiteSpace(product.VariantID) ? "/" + product.VariantID : string.Empty));
                }
            }
            WeeklySitemapJob.Logger.Info("有" + productUrlList.Count() + "需要导入库");
            return InsertURLToDB(productUrlList, "Product");
        }

        public static bool InsertShopListURL(bool isFull = false)
        {
            List<string> shopUrlList = new List<string>();
            IEnumerable<ShopInfo> shops = BaseDataDAL.GetShopListInfo(isFull);
            if (shops != null && shops.Any())
            {
                foreach (var shop in shops.Where(q => q != null))
                {
                    shopUrlList.Add(string.Format("http://www.tuhu.cn/shops/{0}{1}.aspx", shop.RegionNamePinYin, shop.RegionId));
                }
            }
            WeeklySitemapJob.Logger.Info("有" + shopUrlList.Count() + "需要导入库");
            return InsertURLToDB(shopUrlList, "ShopList");
        }

        public static bool InsertShopDetailURL(bool isFull = false)
        {
            List<string> shopdetailUrlList = new List<string>();
            IEnumerable<ShopInfo> shops = BaseDataDAL.GetShopDetailInfo(isFull);
            if (shops != null && shops.Any())
            {
                foreach (var shop in shops.Where(q => q != null))
                {
                    shopdetailUrlList.Add(string.Format("http://www.tuhu.cn/shop/{0}{1}/{2}.aspx", shop.RegionNamePinYin, shop.RegionId, shop.ShopId));
                }
            }
            WeeklySitemapJob.Logger.Info("有" + shopdetailUrlList.Count() + "需要导入库");
            return InsertURLToDB(shopdetailUrlList, "ShopDetail");
        }
        public static bool InsertBaoYangURL(bool isFull = false)
        {
            List<string> baoyangUrlList = new List<string>();
            IEnumerable<VehicleInfo> Vehicles = BaseDataDAL.GetVehicleInfo(isFull);
            if (Vehicles != null && Vehicles.Any())
            {
                foreach (var vehicle in Vehicles.Where(q => q != null))
                {
                    for (int i = vehicle.StartYear; i <= vehicle.EndYear; i++)
                    {
                        baoyangUrlList.Add(string.Format("http://by.tuhu.cn/baoyang/{0}/pl{1}-n{2}.html", vehicle.VehicleID, vehicle.PaiLiang, i));
                    }

                }
            }
            WeeklySitemapJob.Logger.Info("有" + baoyangUrlList.Count() + "需要导入库");
            return InsertURLToDB(baoyangUrlList, "BaoYang");
        }


        public static bool InsertDianPingURL(bool isFull = false)
        {
            List<string> dianpingUrlList = new List<string>();
            IEnumerable<VehicleInfo> Vehicles = BaseDataDAL.GetVehicleInfo(isFull);
            if (Vehicles != null && Vehicles.Any())
            {
                foreach (var vehicle in Vehicles.Where(q => q != null))
                {
                    for (int i = vehicle.StartYear; i <= vehicle.EndYear; i++)
                    {
                        dianpingUrlList.Add(string.Format("http://www.tuhu.cn/spages/Battery.aspx?pid={0}&n={1}&pl={2}", vehicle.VehicleID, i, vehicle.PaiLiang));
                    }

                }
            }
            return InsertURLToDB(dianpingUrlList, "DianPing");
        }

        public static bool InsertTireURL(bool isFull = false)
        {
            List<string> tireUrlList = new List<string>();
            List<VehicleBrand> vehicles = new List<VehicleBrand>();
            using (var vehicleclient = new VehicleClient())
            {
                var response = vehicleclient.GetAllVehicles();
                if (response != null && response.Result != null)
                {
                    vehicles = response.Result.ToList();
                }
            }
            if (vehicles != null && vehicles.Any())
            {
                foreach (var vehicle in vehicles.Where(q => q != null))
                {
                    foreach (var tiresize in hotTiresizes)
                    {
                        var count = BaseDataDAL.GetTireCount(vehicle.VehicleId, tiresize, isFull);
                        int pagecount = count % 20 > 0 ? count / 20 + 1 : count / 20;
                        for (int i = 1; i <= pagecount; i++)
                        {
                            string w = tiresize.Substring(0, tiresize.IndexOf('/'));
                            string a = tiresize.Substring(tiresize.IndexOf('/') + 1, tiresize.IndexOf('R') - tiresize.IndexOf('/') - 1);
                            string r = tiresize.Substring(tiresize.IndexOf('R') + 1);
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/{0}/a{1}-r{2}-w{3}-v{4}.html?oe=", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/{0}/a{1}-r{2}-w{3}-v{4}.html?oe=#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/{0}/a{1}-r{2}-w{3}-v{4}-o1.html?oe=#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/{0}/a{1}-r{2}-w{3}-v{4}-o3.html?oe=#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/{0}/a{1}-r{2}-w{3}-v{4}-o6.html?oe=#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                        }
                        //  tireUrlList.Add(string.Format("http://www.tuhu.cn/spages/Battery.aspx?pid={0}&n={1}&pl={2}", vehicle.VehicleID, i, vehicle.PaiLiang));
                    }

                }
            }
            WeeklySitemapJob.Logger.Info("有" + tireUrlList.Count() + "需要导入库");
            return InsertURLToDB(tireUrlList, "TireList");
        }


        public static bool GenerateTireUrlFiles()
        {
            bool flag = true;
            try
            {

                List<string> tireUrls = new List<string>();


                List<string> filelist = new List<string>();
                tireUrls.AddRange(GetTireURLs(true));
                tireUrls.AddRange(GetTireBrandURLs(true));
                int Urlcount = tireUrls.Count();
                WeeklySitemapJob.Logger.Info("有" + Urlcount + "地址需要上传");
                int sequence = Urlcount / 2000;
                WeeklySitemapJob.Logger.Info("有" + sequence + 1 + "个文件需要生成");
                for (int i = 0; i <= sequence; i++)
                {
                    var tempurls = tireUrls.Skip(i * 2000).Take(2000).ToList();
                    MemoryStream stream = GenerateStreamFromString(tempurls);
                    string filename = string.Format("{0}.txt", i);
                    using (FileStream filestream = new FileStream(filename, FileMode.Create))
                    {
                        stream.WriteTo(filestream);
                        filelist.Add(filename);
                    }
                }
                AddFileToZip("TireUrl.zip", filelist);
                Attachment fileattachment = new Attachment("TireUrl.zip");
                SendMail("TireUrlFiles", ConfigurationManager.AppSettings["SEOFileJob:To"], fileattachment);
            }
            catch (Exception ex)
            {
                flag = false;
                WeeklySitemapJob.Logger.Error(ex);
            }
            return flag;
        }

        public static List<string> GetTireURLs(bool isFull = false)
        {
            List<string> tireUrlList = new List<string>();
            List<VehicleBrand> vehicles = new List<VehicleBrand>();
            using (var vehicleclient = new VehicleClient())
            {
                var response = vehicleclient.GetAllVehicles();
                if (response != null && response.Result != null)
                {
                    vehicles = response.Result.ToList();
                }
            }
            if (vehicles != null && vehicles.Any())
            {
                WeeklySitemapJob.Logger.Info("有" + vehicles.Count() + "个车型适配轮胎");
                foreach (var vehicle in vehicles.Where(q => q != null))
                {
                    foreach (var tiresize in hotTiresizes)
                    {
                        var count = BaseDataDAL.GetTireCount(vehicle.VehicleId, tiresize, isFull);
                        int pagecount = count % 20 > 0 ? count / 20 + 1 : count / 20;
                        for (int i = 1; i <= pagecount; i++)
                        {
                            string w = tiresize.Substring(0, tiresize.IndexOf('/'));
                            string a = tiresize.Substring(tiresize.IndexOf('/') + 1, tiresize.IndexOf('R') - tiresize.IndexOf('/') - 1);
                            string r = tiresize.Substring(tiresize.IndexOf('R') + 1);
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}.html", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}.html#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}-o1.html", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}-o3.html", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}-o6.html", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}-o1.html#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}-o3.html#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/{0}/au1-a{1}-r{2}-w{3}-v{4}-o6.html#Products", i, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                        }
                    }

                }
            }
            return tireUrlList;
        }


        public static List<string> GetTireBrandURLs(bool isFull = false)
        {
            List<string> tireUrlList = new List<string>();
            List<VehicleBrand> vehicles = new List<VehicleBrand>();
            using (var vehicleclient = new VehicleClient())
            {
                var response = vehicleclient.GetAllVehicles();
                if (response != null && response.Result != null)
                {
                    vehicles = response.Result.ToList();
                }
            }
            IEnumerable<TireInfo> TireBrandInfos = BaseDataDAL.GetTireInfo(isFull);
            WeeklySitemapJob.Logger.Info("有" + TireBrandInfos.Count() + "个品牌适配轮胎");
            if (vehicles != null && vehicles.Any())
            {
                foreach (var vehicle in vehicles.Where(q => q != null))
                {
                    foreach (var tiresize in hotTiresizes)
                    {
                        foreach (var brand in TireBrandInfos.Where(q => q != null))
                        {

                            // var count = BaseDataDAL.GetTireCount(vehicle.VehicleID, tiresize);
                            // int pagecount = count % 20 > 0 ? count / 20 + 1 : count / 20;

                            string w = tiresize.Substring(0, tiresize.IndexOf('/'));
                            string a = tiresize.Substring(tiresize.IndexOf('/') + 1, tiresize.IndexOf('R') - tiresize.IndexOf('/') - 1);
                            string r = tiresize.Substring(tiresize.IndexOf('R') + 1);
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}.html", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}.html#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}-o1.html", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}-o3.html", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}-o6.html", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}-o1.html#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}-o3.html#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("https://item.tuhu.cn/Tires/1/au1-b{0}-a{1}-r{2}-w{3}-v{4}-o6.html#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                        }

                    }

                }
            }
            return tireUrlList;
        }

        public static bool InsertHubURL(bool isFull = false)
        {
            List<string> HubUrlList = new List<string>();
            List<VehicleBrand> vehicles = new List<VehicleBrand>();
            using (var vehicleclient = new VehicleClient())
            {
                var response = vehicleclient.GetAllVehicles();
                if (response != null && response.Result != null)
                {
                    vehicles = response.Result.ToList();
                }
            }
            List<string> hubUrlList = new List<string>();
            if (vehicles != null && vehicles.Any())
            {
                foreach (var vehicle in vehicles)
                {
                    List<string> sizes = new List<string>();
                    List<string> tires = vehicle.Tires.Split(';').ToList();
                    if (tires.Any())
                    {
                        foreach (var tire in tires.Where(q => !string.IsNullOrWhiteSpace(q)))
                        {
                            int i = tire.IndexOf('R');
                            if (i >= 0)
                            {
                                string size = tire.Substring(i + 1);
                                if (!sizes.Contains(size))
                                {
                                    sizes.Add(size);
                                    HubUrlList.Add(string.Format("http://item.tuhu.cn/hub/1/r{0}-v{1}.html", size, vehicle.VehicleId.Replace("-", "_")));
                                    HubUrlList.Add(string.Format("http://item.tuhu.cn/hub/1/r{0}-v{1}.html#Products", size, vehicle.VehicleId.Replace("-", "_")));
                                    HubUrlList.Add(string.Format("http://item.tuhu.cn/hub/1/r{0}-v{1}-o1.html#Products", size, vehicle.VehicleId.Replace("-", "_")));
                                    HubUrlList.Add(string.Format("http://item.tuhu.cn/hub/1/r{0}-v{1}-o3.html#Products", size, vehicle.VehicleId.Replace("-", "_")));
                                }
                            }
                        }
                    }
                }
            }
            WeeklySitemapJob.Logger.Info("有" + HubUrlList.Count() + "需要导入库");
            return InsertURLToDB(HubUrlList, "HubList");
        }

        public static bool InsertTireBrandURL(bool isFull = false)
        {
            List<string> tireUrlList = new List<string>();
            List<VehicleBrand> vehicles = new List<VehicleBrand>();
            using (var vehicleclient = new VehicleClient())
            {
                var response = vehicleclient.GetAllVehicles();
                if (response != null && response.Result != null)
                {
                    vehicles = response.Result.ToList();
                }
            }
            IEnumerable<TireInfo> TireBrandInfos = BaseDataDAL.GetTireInfo(isFull);
            if (vehicles != null && vehicles.Any())
            {
                foreach (var vehicle in vehicles.Where(q => q != null))
                {
                    foreach (var tiresize in hotTiresizes)
                    {
                        foreach (var brand in TireBrandInfos.Where(q => q != null))
                        {
                            // var count = BaseDataDAL.GetTireCount(vehicle.VehicleID, tiresize);
                            // int pagecount = count % 20 > 0 ? count / 20 + 1 : count / 20;

                            string w = tiresize.Substring(0, tiresize.IndexOf('/'));
                            string a = tiresize.Substring(tiresize.IndexOf('/') + 1, tiresize.IndexOf('R') - tiresize.IndexOf('/') - 1);
                            string r = tiresize.Substring(tiresize.IndexOf('R') + 1);
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/1/b{0}-a{1}-r{2}-w{3}-v{4}.html?oe=", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/1/b{0}-a{1}-r{2}-w{3}-v{4}.html?oe=#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/1/b{0}-a{1}-r{2}-w{3}-v{4}-o1.html?oe=#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/1/b{0}-a{1}-r{2}-w{3}-v{4}-o3.html?oe=#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                            tireUrlList.Add(string.Format("http://item.tuhu.cn/Tires/1/b{0}-a{1}-r{2}-w{3}-v{4}-o6.html?oe=#Products", brand.BrandID, a, r, w, vehicle.VehicleId.Replace("-", "_")));
                        }
                        //  tireUrlList.Add(string.Format("http://www.tuhu.cn/spages/Battery.aspx?pid={0}&n={1}&pl={2}", vehicle.VehicleID, i, vehicle.PaiLiang));
                    }

                }
            }
            WeeklySitemapJob.Logger.Info("有" + tireUrlList.Count() + "需要导入库");
            return InsertURLToDB(tireUrlList, "TireBrandList");
        }




        public static bool InsertURLToDB(List<string> urlList, string type)
        {
            bool isInsert = true;
            try
            {
                if (urlList != null && urlList.Any())
                {
                    foreach (var url in urlList.Where(q => !string.IsNullOrWhiteSpace(q)))
                    {
                        BaseDataDAL.InsertURL(url, type);
                    }
                }
            }
            catch (Exception ex)
            {
                isInsert = false;
                WeeklySitemapJob.Logger.Error(ex);
            }
            return isInsert;
        }


        public static string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

    }
}

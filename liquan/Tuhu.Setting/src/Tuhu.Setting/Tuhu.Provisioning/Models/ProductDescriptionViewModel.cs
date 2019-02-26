using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class ProductDescriptionViewModel
    {
        public int ModuleID { get; set; }

        public string ModuleName { get; set; }

        public int ModuleOrder { get; set; }

        public bool IsAdvertise { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string IOSHandleKey { get; set; }

        public string IOSSpecialKey { get; set; }

        public string AndroidHandleKey { get; set; }

        public string AndroidSpecialKey { get; set; }

        public string PCUrl { get; set; }

        public string H5Url { get; set; }

        public string BigImageUrl { get; set; }

        public string SmallImageUrl { get; set; }

        public string ContentHtml { get; set; }

        public int PlatfromType { get; set; }

        public ProductDescriptionConfig ProductConfigDetails { get; set; }

        public class ProductDescriptionConfig
        {
            public List<string> Categories { get; set; }
            public string CategoryName { get; set; }
            public int AddOrDel { get; set; }
            public List<string> Pids { get; set; }
            public List<ProductDescriptionBrand> BrandDetails { get; set; }
        }

        public class ProductDescriptionBrand
        {
            public string BrandCategoryName { get; set; }
            public List<string> Brands { get; set; }
        }

        public List<ProductDescriptionViewModel> productDescriptionList { get; set; }

        public static List<ProductDescriptionViewModel> ConvertToList(List<ProductDescriptionModel> models)
        {
            List<ProductDescriptionViewModel> result = new List<ProductDescriptionViewModel>();
            var source = PromotionManager.SelectProductCategoryCategoryNameAndDisplayName().ToList();

            if (models != null && models.Count() > 0)
            {
                foreach (var item in models)
                {
                    ProductDescriptionViewModel viewModel = new ProductDescriptionViewModel();
                    ProductDescriptionConfig configDetails = new ProductDescriptionConfig();
                    ProductDescriptionBrand brandItem = new ProductDescriptionBrand();
                    configDetails.Categories = new List<string>();
                    configDetails.Pids = new List<string>();
                    configDetails.BrandDetails = new List<ProductDescriptionBrand>();
                    brandItem.Brands = new List<string>();

                    if (result.Where(o => o.ModuleID.Equals(item.ModuleID)).Count() > 0)
                    {
                        foreach (var configItem in result)
                        {
                            if (configItem.ModuleID.Equals(item.ModuleID))
                            {
                                configItem.ProductConfigDetails = IsExcistProductDescriptionItem(item, configItem.ProductConfigDetails);

                                if (item.IsAdvert)
                                {
                                    if (configItem.ModuleID.Equals(item.ModuleID) && configItem.PlatfromType != item.ShowPlatform)
                                    {
                                        switch (item.ShowPlatform)
                                        {
                                            case 1:
                                                configItem.PCUrl = item.URL;
                                                break;
                                            case 3:
                                                configItem.IOSHandleKey = item.AppHandleKey;
                                                configItem.IOSSpecialKey = item.AppSpecialKey;
                                                break;
                                            case 4:
                                                configItem.AndroidHandleKey = item.AppHandleKey;
                                                configItem.AndroidSpecialKey = item.AppSpecialKey;
                                                break;
                                            case 5:
                                                configItem.H5Url = item.URL;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        viewModel.ModuleID = item.ModuleID;
                        viewModel.ModuleName = item.ModuleName;
                        viewModel.PlatfromType = item.ShowPlatform;
                        viewModel.ContentHtml = item.ModuleContent;
                        viewModel.ModuleOrder = item.ModuleOrder;
                        viewModel.IsAdvertise = item.IsAdvert;
                        viewModel.StartDateTime = item.StartDateTime;
                        viewModel.EndDateTime = item.EndDateTime;
                        if (item.IsAdvert)
                        {
                            viewModel.SmallImageUrl = item.SmallImageUrl;
                            viewModel.BigImageUrl = item.BigImageUrl;

                            switch (item.ShowPlatform)
                            {
                                case 1:
                                    viewModel.PCUrl = item.URL;
                                    break;
                                case 3:
                                    viewModel.IOSHandleKey = item.AppHandleKey;
                                    viewModel.IOSSpecialKey = item.AppSpecialKey;
                                    break;
                                case 4:
                                    viewModel.AndroidHandleKey = item.AppHandleKey;
                                    viewModel.AndroidSpecialKey = item.AppSpecialKey;
                                    break;
                                case 5:
                                    viewModel.H5Url = item.URL;
                                    break;
                            }
                        }

                        configDetails.AddOrDel = item.AddOrDel;

                        if (!string.IsNullOrEmpty(item.CategoryName) && string.IsNullOrEmpty(item.Brand))
                        {
                            configDetails.Categories.Add(item.CategoryName);
                        }
                        if (!string.IsNullOrEmpty(item.PID))
                        {
                            configDetails.Pids.Add(item.PID);
                        }
                        if (!string.IsNullOrEmpty(item.Brand))
                        {
                            brandItem.BrandCategoryName = item.CategoryName;
                            brandItem.Brands.Add(item.Brand);
                            configDetails.BrandDetails.Add(brandItem);
                        }

                        viewModel.ProductConfigDetails = configDetails;

                        result.Add(viewModel);
                    }
                }
            }

            result = ShowProductCategoryName(result, source);

            return result;
        }


        public static List<ProductDescriptionViewModel> ShowProductCategoryName(List<ProductDescriptionViewModel> viewModel, List<Category> categories)
        {
            if (viewModel != null && categories != null && categories.Any())
            {
                foreach (var item in viewModel)
                {
                    if ((!string.IsNullOrEmpty(item.IOSHandleKey) && !string.IsNullOrEmpty(item.PCUrl)))
                    {
                        item.PlatfromType = 0;
                    }
                    if (!string.IsNullOrEmpty(item.IOSHandleKey) && string.IsNullOrEmpty(item.PCUrl))
                    {
                        item.PlatfromType = 2;
                    }
                    if (string.IsNullOrEmpty(item.IOSHandleKey) && !string.IsNullOrEmpty(item.PCUrl))
                    {
                        item.PlatfromType = 1;
                    }
                    if (item.ProductConfigDetails.Categories != null && item.ProductConfigDetails.Categories.Count() > 0)
                    {
                        var categoryList = string.Join(",", item.ProductConfigDetails.Categories).Split(',');

                        item.ProductConfigDetails.CategoryName = string.Empty;

                        foreach (var categoryItem in item.ProductConfigDetails.Categories)
                        {
                            if(categories.Where(s => String.Equals(s.CategoryName, categoryItem)).Count()>0)
                            {
                                item.ProductConfigDetails.CategoryName += categories.Where(s => String.Equals(s.CategoryName, categoryItem)).FirstOrDefault().DisplayName + "|";
                            }
                        }
                        item.ProductConfigDetails.CategoryName = item.ProductConfigDetails.CategoryName.TrimEnd('|');
                    }
                }
            }

            return viewModel;
        }

        public static ProductDescriptionConfig IsExcistProductDescriptionItem(ProductDescriptionModel item, ProductDescriptionConfig data)
        {
            ProductDescriptionConfig configDetails = new ProductDescriptionConfig();
            ProductDescriptionBrand brandItem = new ProductDescriptionBrand();

            if (item.AddOrDel > 0 && data.AddOrDel == 0)
            {
                data.AddOrDel = item.AddOrDel;
            }

            if (!string.IsNullOrEmpty(item.CategoryName) && string.IsNullOrEmpty(item.Brand) && !data.Categories.Contains(item.CategoryName))
            {
                data.Categories.Add(item.CategoryName);
            }

            if (!string.IsNullOrEmpty(item.PID) && !data.Pids.Contains(item.PID))
            {
                data.Pids.Add(item.PID);
            }

            if (!string.IsNullOrEmpty(item.Brand))
            {
                if (data.BrandDetails.Where(o => o.BrandCategoryName.Equals(item.CategoryName)).Count() > 0)
                {
                    foreach (var configItem in data.BrandDetails)
                    {
                        if (String.Equals(configItem.BrandCategoryName, item.CategoryName))
                        {
                            if (String.Equals(configItem.BrandCategoryName, item.CategoryName) && !configItem.Brands.Contains(item.Brand))
                            {
                                configItem.Brands.Add(item.Brand);
                            }
                        }

                    }
                }
                else
                {
                    brandItem.Brands = new List<string>();

                    brandItem.BrandCategoryName = item.CategoryName;
                    brandItem.Brands.Add(item.Brand);
                    data.BrandDetails.Add(brandItem);
                }
            }

            return data;
        }
    }
}
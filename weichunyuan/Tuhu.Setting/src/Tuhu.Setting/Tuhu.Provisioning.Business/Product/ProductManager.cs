﻿using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;

namespace Tuhu.Provisioning.Business.Product
{
    public class ProductManager
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ProductManager));
        private static readonly IConnectionManager tuhuProductcatalogReadConnectionManager =
           new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);
        private readonly IDBScopeManager TuhuProductcatalogReadDbScopeManager = null;
        public ProductManager()
        {
            TuhuProductcatalogReadDbScopeManager = new DBScopeManager(tuhuProductcatalogReadConnectionManager);
        }
        /// <summary>
        /// 根据类别查询品牌名称
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string[] GetProductPrioritySettingsBrand(string category)
        {
            try
            {
                return TuhuProductcatalogReadDbScopeManager.Execute(conn =>
                {
                    return DalProduct.GetProductBrand(conn, category);
                });
            }
            catch (Exception ex)
            {

                logger.Error(ex.Message, ex);
            }
            return null;
        }
        /// <summary>
        /// 根据品牌和类别查询系列产品
        /// </summary>
        /// <param name="category"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public string[] GetProductPrioritySettingsSeries(string category, string brand)
        {
            try
            {
                return TuhuProductcatalogReadDbScopeManager.Execute(conn =>
                {
                    if (category == "Oil")
                    {
                        return DalProduct.GetOilProductSeries(conn, category, brand);
                    }
                    return DalProduct.GetProductSeries(conn, category, brand);
                });
            }
            catch (Exception ex)
            {

                logger.Error(ex.Message, ex);
            }
            return null;

        } 
        /// <summary>
        /// 获取产品详细属性
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public Dictionary<string, ProductDetailModel> SelectProductDetail(List<string> pids)
        {
            var productDic = new Dictionary<string, ProductDetailModel>();
            using (var client = new ProductClient())
            {
                var serviceResult = client.SelectProductDetail(pids);
                if (!serviceResult.Success)
                {
                    serviceResult.ThrowIfException(true);
                }
                foreach (var item in serviceResult.Result)
                {
                    productDic.Add(item.Pid, item);
                }
            }
            return productDic;
        }
    }
}

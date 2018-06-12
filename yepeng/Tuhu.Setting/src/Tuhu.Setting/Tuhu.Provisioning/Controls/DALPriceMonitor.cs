using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Component.Common.Models;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controls
{
    public static class ProductMappingManager
    {
        public static IEnumerable<ProductMappingModel> List(string shopCode, string q, PagerModel pager)
        {
            using (var cmd = new SqlCommand())
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    cmd.CommandText = @"SELECT	@Total = COUNT(1)
FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM
LEFT JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
		ON PPM.Pid = CP.ProductID + '|' + CP.VariantID
		   AND CP.i_ClassType IN (2, 4)
LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
		ON C.#Catalog_Lang_Oid = CP.oid
WHERE	PPM.ThirdParty = 1
		AND (@ShopCode IS NULL
			 OR @ShopCode = PPM.ShopCode)
SELECT	PPM.ShopCode,
				PPM.Pid,
				C.DisplayName,
				PPM.ItemID,
				PPM.SkuID,
				PPM.Price,
				PPM.Promotion,
				PPM.Title,
				PPM.LastUpdateDateTime,
				PPM.ItemCode
		 FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM WITH (NOLOCK)
		 LEFT JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
				ON PPM.Pid = CP.ProductID + '|' + CP.VariantID
				   AND CP.i_ClassType IN (2, 4)
		 LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
				ON C.#Catalog_Lang_Oid = CP.oid
		 WHERE	PPM.ThirdParty = 1
				AND (@ShopCode IS NULL
					 OR @ShopCode = PPM.ShopCode)
ORDER BY PPM.CreateDateTime DESC
OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
                }
                else
                {
                    //var sb = new StringBuilder();

                    //var arr = q.Split(new[] { ' ', '　', ',', '，', ';', '；', '\\', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    //if (arr.Length > 1)
                    //    for (var index = 0; index < arr.Length; index++)
                    //    {
                    //        sb.AppendLine("AND C.DisplayName LIKE '%' + " + ("@Q" + index) + " + '%'");

                    //        cmd.Parameters.AddWithValue("@Q" + index, arr[index]);
                    //    }
                    //else
                    //{
                    //    sb.AppendLine("AND C.DisplayName LIKE '%' + @Q + '%'");
                    //}

                    cmd.CommandText = @"SELECT	@Total = COUNT(1)
FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM
LEFT JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
		ON PPM.Pid = CP.ProductID + '|' + CP.VariantID
		   AND CP.i_ClassType IN (2, 4)
LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
		ON C.#Catalog_Lang_Oid = CP.oid
WHERE	PPM.ThirdParty = 1
		AND (@ShopCode IS NULL
			 OR @ShopCode = PPM.ShopCode)
		AND (@Q IS NULL
			 OR PPM.Pid = @Q
			 OR PPM.Pid LIKE @Q + '|%'
			 OR CAST(PPM.ItemID AS VARCHAR(20)) = @Q
			 OR CAST(PPM.SkuID AS VARCHAR(20)) = @Q)
SELECT	PPM.ShopCode,
				PPM.Pid,
				C.DisplayName,
				PPM.ItemID,
				PPM.SkuID,
				PPM.Price,
				PPM.Promotion,
				PPM.Title,
				PPM.LastUpdateDateTime,
                PPM.ItemCode
		 FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM WITH (NOLOCK)
		 LEFT JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
				ON PPM.Pid = CP.ProductID + '|' + CP.VariantID
				   AND CP.i_ClassType IN (2, 4)
		 LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
				ON C.#Catalog_Lang_Oid = CP.oid
		 WHERE	PPM.ThirdParty = 1
				AND (@ShopCode IS NULL
					 OR @ShopCode = PPM.ShopCode)
				AND (@Q IS NULL
					 OR PPM.Pid = @Q
					 OR PPM.Pid LIKE @Q + '|%'
					 OR CAST(PPM.ItemID AS VARCHAR(20)) = @Q
					 OR CAST(PPM.SkuID AS VARCHAR(20)) = @Q)
ORDER BY PPM.CreateDateTime DESC
OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
                }

                cmd.Parameters.AddWithValue("@ShopCode", shopCode);
                cmd.Parameters.AddWithValue("@Q", q);
                cmd.Parameters.AddWithValue("@Total", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@PageNumber", pager.CurrentPage);
                cmd.Parameters.AddWithValue("@PageSize", pager.PageSize);

                var dt = Tuhu.Component.Common.DbHelper.ExecuteDataTable(cmd);
                //var dt = DbHelper.ExecuteSelect<ProductMappingModel>(true, cmd);
                pager.TotalItem = Convert.ToInt32(cmd.Parameters["@Total"].Value);


                return dt == null || dt.Rows.Count == 0 ? new ProductMappingModel[0] : dt.Rows.OfType<DataRow>().Select(row => new ProductMappingModel(row)).ToArray();
            }
        }

        public static int Insert(ProductMappingModel model)
        {
            if (model == null)
                return -2;

            using (var cmd = new SqlCommand())
            {
                switch (model.ShopCode)
                {
                    case "京东自营":
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM WHERE	PPM.ShopCode = @ShopCode AND PPM.SkuID = @SkuID";
                        cmd.Parameters.AddWithValue("@SkuID", model.SkuID);
                        break;
                    case "养车无忧":
                    case "康众官网":
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM WHERE	PPM.ShopCode = @ShopCode AND PPM.ItemCode = @ItemCode";
                        cmd.Parameters.AddWithValue("@ItemCode", model.ItemCode);
                        break;
                    case "汽配龙":
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM WHERE	ShopCode = @ShopCode AND PPM.Pid = @Pid";
                        cmd.Parameters.AddWithValue("@Pid", model.Pid);
                        break;
                    default:
                        cmd.CommandText = "SELECT	1 FROM	Tuhu_productcatalog..ProductPriceMapping AS PPM WHERE	PPM.ShopCode = @ShopCode AND PPM.ItemID = @ItemID";
                        cmd.Parameters.AddWithValue("@ItemID", model.ItemID);
                        break;
                }

                cmd.Parameters.AddWithValue("@ShopCode", model.ShopCode);
                try
                {
                    if (Tuhu.Component.Common.DbHelper.ExecuteScalar(cmd) != null)
                        return -1;
                }
                catch (Exception ex)
                {                  
                    throw ex;
                }
               
            }

            var displayName = Tuhu.Component.Common.DbHelper.ExecuteScalar(@"SELECT	C.DisplayName
FROM	Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH(NOLOCK)
JOIN	Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH(NOLOCK)
		ON C.#Catalog_Lang_Oid = CP.oid
WHERE	@Pid = CP.ProductID + '|' + CP.VariantID
		AND CP.i_ClassType IN (2, 4)",
                                     CommandType.Text,
                                     new SqlParameter("Pid", model.Pid));

            if (displayName == null)
                return -5;

            model.DisplayName = displayName.ToString();

            using (var cmd = new SqlCommand(@"INSERT	INTO Tuhu_productcatalog..ProductPriceMapping
		(ShopCode,
		 Pid,
		 ItemID,
		 SkuID,
		 Price,
		 Promotion,
		 Title,
		 ThirdParty,
		 ItemCode)
VALUES	(@ShopCode, -- ShopCode - nvarchar(20)
		 @Pid, -- Pid - nvarchar(200)
		 @ItemID, -- ItemID - bigint
		 @SkuID, -- SkuID - bigint
		 @Price, -- Price - money
		 0, -- Promotion - bit
		 @Title, -- Title - nvarchar(250)
		 1,
		 @ItemCode)"))
            {
                cmd.Parameters.AddWithValue("@ShopCode", model.ShopCode);
                cmd.Parameters.AddWithValue("@Pid", model.Pid.Trim());

                if (model.ShopCode == "养车无忧" || model.ShopCode == "康众官网")
                {
                    cmd.Parameters.AddWithValue("@ItemID", 0);
                    cmd.Parameters.AddWithValue("@SkuID", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ItemID", model.ItemID);
                    cmd.Parameters.AddWithValue("@SkuID", model.SkuID);
                }

                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                if (model.ShopCode == "养车无忧" || model.ShopCode == "康众官网")
                {
                    cmd.Parameters.AddWithValue("@ItemCode", model.ItemCode);
                }
                else if (model.ShopCode == "汽配龙")
                {
                    cmd.Parameters.AddWithValue("@ItemCode", model.Pid);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ItemCode", "");
                }
                try
                {
                    return Tuhu.Component.Common.DbHelper.ExecuteNonQuery(cmd);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }

        public static int Delete(string shopCode, string item)
        {
            using (var cmd = new SqlCommand("DELETE	Tuhu_productcatalog..ProductPriceMapping WHERE	ThirdParty = 1 AND ShopCode = @ShopCode AND "))
            {
                cmd.Parameters.AddWithValue("@ShopCode", shopCode);

                long itemId;
                if (long.TryParse(item, out itemId))
                {
                    cmd.CommandText += "(ItemID > 0 AND ItemID = @ItemID OR SkuID > 0 AND SkuID = @ItemID)";
                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                }
                else
                {
                    cmd.CommandText += "ItemCode = @itemCode";
                    cmd.Parameters.AddWithValue("@itemCode", item);
                }

                return Tuhu.Component.Common.DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }

}
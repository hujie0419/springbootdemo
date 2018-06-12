using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.Models;
using Tuhu.Service.Utility.Models;

namespace Tuhu.C.SyncProductPriceJob.DataAccess
{
    public class Products
    {
        public static async Task<bool> SavePrice(ProductPriceMappingModel model)
        {
            // 价格异常不更新
            if (model.Price <= 0 || model.Price >= 9999999)
            {
                return true;
            }
            DbParameter[] paramArray =
            {
                new SqlParameter("@ShopCode", model.ShopCode),
                new SqlParameter("@Pid", model.Pid?.Replace("▲", "").Replace("★", "") ?? ""),
                new SqlParameter("@ItemID", model.ItemId),
                new SqlParameter("@SkuID", model.SkuId),
                new SqlParameter("@Properties", model.Properties),
                new SqlParameter("@Price", model.Price),
                new SqlParameter("@Price2", model.Price2),
                new SqlParameter("@Promotion", 0),
                new SqlParameter("@Title", model.Title)
            };
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                if (await dbHelper.ExecuteNonQueryAsync(@"UPDATE	[Tuhu_productcatalog].[dbo].[ProductPriceMapping]
SET		Pid = @Pid,
		Properties = @Properties,
		Price = @Price,
		Price2 = @Price2,
		Promotion = @Promotion,
		Title = ISNULL(@Title, Title),
		LastUpdateDateTime = GETDATE()
WHERE	ShopCode = @ShopCode
		AND ItemID = @ItemID
		AND SkuID = @SkuID", CommandType.Text, paramArray) > 0)
                {
                    return true;
                }
                return await dbHelper.ExecuteNonQueryAsync(@"INSERT	INTO [Tuhu_productcatalog].[dbo].[ProductPriceMapping]
			(ShopCode,
			 Pid,
			 ItemID,
			 SkuID,
			 Properties,
			 Price,
			 Promotion,
			 Title
			)
   VALUES	(@ShopCode,	-- ShopCode - nvarchar(20)
			 @Pid,		-- Pid - nvarchar(200)
			 @ItemID,	-- ItemID - bigint
			 @SkuID,	-- SkuID - bigint
			 @Properties,
			 @Price,	-- Price - money
			 @Promotion,
			 ISNULL(@Title, '')
			)", CommandType.Text, paramArray) > 0;
            }
        }

        public static async Task<int> SaveProductMapping(ProductPriceMappingModel mapping)
        {
            if (mapping == null)
            {
                return 0;
            }
            DbParameter[] paramArray =
            {
                new SqlParameter("@ShopCode", mapping.ShopCode),
                new SqlParameter("@Pid", mapping.Pid?.Replace("▲", "").Replace("★", "") ?? ""),
                new SqlParameter("@ItemID", mapping.ItemId),
                new SqlParameter("@SkuID", mapping.SkuId),
                new SqlParameter("@Properties", mapping.Properties),
                new SqlParameter("@Title", mapping.Title)
            };
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                var result = await dbHelper.ExecuteNonQueryAsync(
                    @"UPDATE	[Tuhu_productcatalog].[dbo].[ProductMapping]
SET		Pid = @Pid,
		Properties = @Properties,
		Title = ISNULL(@Title, Title),
		LastUpdateDateTime = GETDATE()
WHERE	ShopCode = @ShopCode
		AND ItemID = @ItemID
		AND SkuID = @SkuID", CommandType.Text, paramArray);
                if (result > 0)
                {
                    return result;
                }
                return await dbHelper.ExecuteNonQueryAsync(@"INSERT	INTO [Tuhu_productcatalog].[dbo].[ProductMapping]
			(ShopCode,
			 Pid,
			 ItemID,
			 SkuID,
			 Properties,
			 Title
			)
   VALUES	(@ShopCode,	-- ShopCode - nvarchar(20)
			 @Pid,		-- Pid - nvarchar(200)
			 @ItemID,	-- ItemID - bigint
			 @SkuID,	-- SkuID - bigint
			 @Properties,
			 ISNULL(@Title, '')
			)", CommandType.Text, paramArray);
            }
        }

        public static Task<int> SaveProductMappings(IReadOnlyCollection<ProductPriceMappingModel> mappings) => Task.WhenAll(mappings.Select(SaveProductMapping)).ContinueWith(_ => _.Result.Sum());

        public static async Task<IEnumerable<ProductPriceMappingModel>> QueryProductMappings(string shopCode)
        {
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                return await dbHelper.ExecuteSelectAsync<ProductPriceMappingModel>(@"SELECT [ShopCode],
       [PID],
       [ItemId],
       [SkuId],
       [ItemCode],
       [Title],
       [Properties]
FROM [Tuhu_productcatalog].[dbo].[ProductMapping] WITH (NOLOCK)
WHERE [ShopCode] = @shopCode
    AND [LastUpdateDateTime] > DATEADD(DAY, -3, CAST(GETDATE() AS DATE));", CommandType.Text, new SqlParameter("@shopCode", shopCode));
            }
        }

        public static async Task<int> UpdatePriceByItemId(ItemPriceModel priceModel, string shopCode, long itemId)
        {
            if (priceModel.Price <= 0 || priceModel.Price >= 9999999)
            {
                return 0;
            }
            DbParameter[] parameters =
            {
                new SqlParameter("@Price", priceModel.Price),
                new SqlParameter("@Title", priceModel.Title),
                new SqlParameter("@ShopCode", shopCode),
                new SqlParameter("@ItemID", itemId)
            };
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                return await dbHelper.ExecuteNonQueryAsync(@"UPDATE	[Tuhu_productcatalog].[dbo].[ProductPriceMapping]
SET		Price = @Price,
		Title = ISNULL(@Title, Title),
		LastUpdateDateTime = GETDATE()
WHERE	ThirdParty IS NOT NULL
		AND ShopCode = @ShopCode
		AND ItemID = @ItemID", CommandType.Text, parameters);
            }
        }

        public static async Task<int> UpdatePriceByItemCode(ItemPriceModel priceModel, string shopCode, string itemCode)
        {
            if (priceModel.Price <= 0 || priceModel.Price >= 9999999)
            {
                return 0;
            }
            DbParameter[] parameters =
            {
                new SqlParameter("@Price", priceModel.Price),
                new SqlParameter("@Title", priceModel.Title),
                new SqlParameter("@ShopCode", shopCode),
                new SqlParameter("@ItemCode", itemCode)
            };
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                return await dbHelper.ExecuteNonQueryAsync(@"UPDATE  [Tuhu_productcatalog].[dbo].[ProductPriceMapping]
SET     Price = @Price ,
        Title = ISNULL(@Title, Title) ,
        LastUpdateDateTime = GETDATE()
WHERE   ThirdParty IS NOT NULL
        AND ShopCode = @ShopCode
        AND ItemCode = @ItemCode", CommandType.Text, parameters);
            }
        }

        public static async Task<int> UpdatePriceByPid(ItemPriceModel priceModel, string shopCode, string pid)
        {
            if (priceModel.Price <= 0 || priceModel.Price >= 9999999)
            {
                return 0;
            }
            DbParameter[] parameters =
            {
                new SqlParameter("@Price", priceModel.Price),
                new SqlParameter("@Title", priceModel.Title),
                new SqlParameter("@ShopCode", shopCode),
                new SqlParameter("@Pid", pid)
            };
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                return await dbHelper.ExecuteNonQueryAsync(@"UPDATE	[Tuhu_productcatalog].[dbo].[ProductPriceMapping]
SET		Price = @Price,
		Title = ISNULL(@Title, Title),
		LastUpdateDateTime = GETDATE()
WHERE	ThirdParty IS NOT NULL
		AND ShopCode = @ShopCode
		AND Pid = @Pid", CommandType.Text, parameters);
            }
        }

        public static async Task<int> UpdatePriceBySkuId(ItemPriceModel priceModel, string shopCode, long skuId)
        {
            if (priceModel.Price <= 0 || priceModel.Price >= 9999999)
            {
                return 0;
            }
            DbParameter[] parameters =
            {
                new SqlParameter("@Price", priceModel.Price),
                new SqlParameter("@Price2", priceModel.Price2),//京东plus价格
                new SqlParameter("@Title", priceModel.Title),
                new SqlParameter("@ShopCode", shopCode),
                new SqlParameter("@SkuID", skuId)
            };
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                return await dbHelper.ExecuteNonQueryAsync(@"UPDATE	[Tuhu_productcatalog].[dbo].[ProductPriceMapping]
SET		Price = @Price,
        Price2 = @Price2,
		Title = ISNULL(@Title, Title),
		LastUpdateDateTime = GETDATE()
WHERE	ThirdParty IS NOT NULL
		AND ShopCode = @ShopCode
		AND SkuID = @SkuID", CommandType.Text, parameters);
            }
        }

        public static int DeleteProduct()
        {
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                return dbHelper.ExecuteNonQuery(
                    "DELETE FROM [Tuhu_productcatalog].[dbo].[ProductPriceMapping] WHERE ThirdParty IS NULL AND LastUpdateDateTime < CAST(DATEADD(DAY, -5, GETDATE()) AS DATE)");
            }
        }

        public static async Task<DataTable> QueryThirdPartyProducts()
        {
            using (var cmd = new SqlCommand(@"SELECT ShopCode, Pid, ItemID, SkuID,ItemCode FROM [Tuhu_productcatalog].[dbo].[ProductPriceMapping] WITH(NOLOCK) WHERE ThirdParty IS NOT NULL"))
            {
                return await DbHelper.ExecuteQueryAsync(cmd, _ => _);
            }
        }

        /// <summary>
        /// 查询需要同步的产品（今天未同步成功的产品）
        /// </summary>
        /// <param name="isThirdParty">是否只取第三方数据，null:都要，true:仅第三方，false:自营门店</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ProductPriceMappingModel>> QueryProductsNeedSync(bool? isThirdParty)
        {
            using (var cmd = new SqlCommand($@"SELECT [ShopCode],
       [Pid],
       [ItemId],
       [SkuId],
       [ItemCode],
       [Title]
FROM [Tuhu_productcatalog].[dbo].[ProductPriceMapping] WITH (NOLOCK)
WHERE LastUpdateDateTime < CAST(GETDATE() AS DATE)
AND [LastUpdateDateTime] > DATEADD(DAY, -5, CAST(GETDATE() AS DATE))
{(isThirdParty == null ? "" : (isThirdParty.Value ? " AND [ThirdParty] IS NOT NULL" : " AND [ThirdParty] IS NULL"))}"))
            {
                return await DbHelper.ExecuteSelectAsync<ProductPriceMappingModel>(cmd);
            }
        }

        public static DataTable QueryHotProduct()
        {
            using (var cmd = new SqlCommand(@"WITH PM
AS (SELECT *
    FROM
    (
        SELECT *,
               ROW_NUMBER() OVER (PARTITION BY PPM.ShopCode,
                                               PPM.Pid
                                  ORDER BY CAST(PPM.LastUpdateDateTime AS DATE) DESC,
                                           PPM.Price
                                 ) AS RowNumber
        FROM [Tuhu_productcatalog].[dbo].[ProductPriceMapping] AS PPM WITH (NOLOCK)
    ) AS T
    WHERE T.RowNumber = 1)
SELECT [R].[ShopCode],
       [R].[ItemID],
       [R].[SkuID],
       [R].[Pid],
       [R].[DisplayName],
       [R].[Price],
       [R].[SitePrice],
       [R].[TmallPrice],
       [R].[TmallItemID]
FROM
(
    SELECT PPM.ShopCode,
           PPM.ItemID,
           PPM.SkuID,
           T.Pid,
           C.DisplayName,
           PPM.Price,
           CP.cy_list_price AS SitePrice,
           TM.Price AS TmallPrice,
           TM.ItemID AS TmallItemID,
           T.Qauntity,
           ROW_NUMBER() OVER (PARTITION BY PPM.ShopCode ORDER BY T.Qauntity DESC) AS RowNumber
    FROM
    (
        SELECT OL.PID AS Pid,
               COUNT(1) AS Qauntity
        FROM Gungnir.dbo.tbl_Order AS O WITH (NOLOCK)
            JOIN Gungnir.dbo.tbl_OrderList AS OL WITH (NOLOCK)
                ON O.PKID = OL.OrderID
        WHERE O.OrderDatetime > DATEADD(WEEK, -1, GETDATE())
              AND O.Status <> '7Canceled'
              AND O.Status <> '0New'
              AND OL.Deleted = 0
              AND OL.Price > 0
        GROUP BY OL.PID
    ) AS T
        JOIN PM AS PPM WITH (NOLOCK)
            ON PPM.Pid COLLATE Chinese_PRC_CI_AS = T.Pid
        JOIN [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] AS CP WITH (NOLOCK)
            ON T.Pid = CP.ProductID + '|' + CP.VariantID COLLATE Chinese_PRC_CI_AS
        JOIN  [Tuhu_productcatalog].[dbo].[CarPAR_zh-CN_Catalog] AS C
            ON C.#Catalog_Lang_Oid = CP.oid
        JOIN PM AS TM WITH (NOLOCK)
            ON TM.Pid COLLATE Chinese_PRC_CI_AS = T.Pid
    WHERE PPM.ShopCode IN ( N'京东自营', N'特维轮', N'麦轮胎官网', N'麦轮胎天猫' )
          AND TM.ShopCode = N'4天猫'
          AND CP.cy_list_price > 0
) AS R
WHERE R.RowNumber <= 100;"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteQuery(cmd, _ => _);
            }
        }
    }
}

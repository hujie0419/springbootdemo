using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Sitemap.DAL
{
    class BaseDataSqlText
    {
        public static string SqlTextAllArticleIDs= "SELECT PKID FROM tbl_Article(NOLOCK) WHERE IsShow=1";
        public static string sqlTextPartialArticleIDs="SELECT PKID FROM tbl_Article WHERE DATEDIFF(dd,PublishDateTime,GETDATE())<=6 and IsShow=1";
        public static string sqlTextProductIDs = "SELECT ProductID,VariantID FROM Tuhu_productcatalog..vw_Products(NOLOCK)  WHERE IsShow=1";
        public static string sqlTextPartialProductIDs = "SELECT ProductID,VariantID FROM Tuhu_productcatalog..vw_Products(NOLOCK) WHERE IsShow=1 AND DATEDIFF(dd,CreateDatetime,GETDATE())<=6";
        public static string sqlTextInsertUrl = @"INSERT INTO  [dbo].[tbl_SiteMap]([URL],[Type],[DataCreate_Time],[DataUpdate_Time]) VALUES
(
@URL, @Type, @DataCreate_Time,@DataUpdate_Time
)";

        public static string sqlTextAllUrl = "SELECT [URL],[Type],[DataCreate_Time],[DataUpdate_Time] FROM [dbo].[tbl_SiteMap]";
        public static string sqlTextPartialUrl = "SELECT [URL],[Type],[DataCreate_Time],[DataUpdate_Time] FROM [dbo].[tbl_SiteMap] WHERE DATEDIFF(dd,DataCreate_Time,GETDATE())<=6";
        public static string SqlTextVehicles= @"
 SELECT VehicleSeries,VehicleID,Brand,
        PaiLiang,
        MIN(ListedYear)AS StartYear,
        ISNULL(MAX(StopProductionYear), YEAR(GETDATE())) AS EndYear
FROM    tbl_Vehicle_Type_Timing(NOLOCK)
WHERE   ListedYear IS NOT NULL
        AND VehicleID IS NOT NULL
        AND PaiLiang IS NOT NULL
GROUP BY VehicleSeries ,
        Brand,
        VehicleID,
        PaiLiang";

        public static string SqlTextPartialVehicles = @"
 SELECT VehicleSeries,VehicleID,Brand,
        PaiLiang,
        MIN(ListedYear)AS StartYear,
        ISNULL(MAX(StopProductionYear), YEAR(GETDATE())) AS EndYear
FROM    tbl_Vehicle_Type_Timing(NOLOCK)
WHERE   ListedYear IS NOT NULL
        AND VehicleID IS NOT NULL
        AND PaiLiang IS NOT NULL 
        AND DATEDIFF(dd,UpdateTime,GETDATE())<=6 
GROUP BY VehicleSeries ,
        Brand,
        VehicleID,
        PaiLiang";


        public static string SqlTextShopList = @"SELECT PKID AS RegionId,RegionName,PinYin AS RegionNamePinYin FROM dbo.tbl_region AS r WITH(NOLOCK) WHERE r.IsActive=1";

        public static string SqlTextPartialShopList = @"SELECT PKID AS RegionId,RegionName,PinYin AS RegionNamePinYin FROM dbo.tbl_region AS r WITH(NOLOCK) WHERE r.IsActive=1 AND DATEDIFF(dd,r.LastUpdateTime,GETDATE())<=6";

        public static string SqlTextShops = @"SELECT s.PKID AS ShopId,s.SimpleName AS ShopName,r.PKID AS RegionId,r.pinyin AS RegionNamePinYin FROM
		shops AS s WITH(NOLOCK) INNER JOIN tbl_region  AS r WITH(NOLOCK)
		ON s.regionid=r.PKID 
        INNER JOIN ShopIsActiveOpreatLog AS sa WITH(NOLOCK)
        ON s.PKID=sa.ShopID
        WHERE s.IsActive=1";


        public static string SqlTextPartialShops= @"SELECT s.PKID AS ShopId,s.SimpleName AS ShopName,r.PKID AS RegionId,r.pinyin AS RegionNamePinYin FROM
		shops AS s WITH(NOLOCK) 
        INNER JOIN tbl_region  AS r WITH(NOLOCK)
		ON s.regionid=r.PKID 
        INNER JOIN ShopIsActiveOpreatLog AS sa WITH(NOLOCK)
        ON s.PKID=sa.ShopID                     
        WHERE s.IsActive=1 and DATEDIFF(dd,sa.OpreatDateTime,GETDATE())<=6 and sa.ActiveOrPassive = N'审核通过'";

        public static string SqlTextPartialHotVehicles = " SELECT  ProductID AS VehicleID,Brand,Vehicle AS Name  FROM tbl_Vehicle_Type(NOLOCK)  WHERE DATEDIFF(dd,LastUpdateTime,GETDATE())<=6";

        public static string SqlTextHotVehicles = " SELECT  ProductID AS VehicleID,Brand,Vehicle AS Name  FROM tbl_Vehicle_Type(NOLOCK)";

        public static string SqlTextTireBrand= @"SELECT DISTINCT(cp_brand) AS BrandName,brand.PKID AS BrandID FROM Tuhu_productcatalog..vw_products product
 INNER JOIN Tuhu_productcatalog..[CarPAR_CatalogBrands]
        brand
ON product.cp_brand=brand.[BrandName]
WHERE product.pid LIKE 'TR-%' AND product.onsale=1 AND product.IsShow=1";

        public static string SqlTextPartialTireBrand = @"SELECT DISTINCT(cp_brand) AS BrandName,brand.PKID AS BrandID FROM Tuhu_productcatalog..vw_products product
 INNER JOIN Tuhu_productcatalog..[CarPAR_CatalogBrands]
        brand
ON product.cp_brand=brand.[BrandName]
WHERE product.pid LIKE 'TR-%' AND product.onsale=1 AND product.IsShow=1 AND DATEDIFF(dd,brand.CreateDateTime,GETDATE())<=6";
    }
}

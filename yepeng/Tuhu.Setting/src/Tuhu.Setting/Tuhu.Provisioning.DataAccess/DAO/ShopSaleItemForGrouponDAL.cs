using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Configuration;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 数据访问-ShopSaleItemForGrouponDAL   
    /// </summary>
    public class ShopSaleItemForGrouponDAL
    {
        public static IEnumerable<ShopSaleItemForGrouponModel> SelectPages(SqlConnection connection, int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM (
                               SELECT ROW_NUMBER() OVER(ORDER BY PKID desc) AS 'RowNumber'
                               ,'TotalCount' = (SELECT COUNT(1) FROM Tuhu_Groupon.dbo.ShopSaleItemForGroupon WITH(NOLOCK) WHERE 1=1 {0})
                               ,* FROM Tuhu_Groupon.dbo.ShopSaleItemForGroupon WITH(NOLOCK) WHERE 1=1 {0}
                               ) AS tab1 
                               WHERE tab1.RowNumber between ((@pageIndex - 1)* @pageSize) + 1 AND @pageIndex * @pageSize ";

                if (!string.IsNullOrWhiteSpace(strWhere))
                    sql = string.Format(sql, strWhere);
                else
                    sql = string.Format(sql, "", "");

                return conn.Query<ShopSaleItemForGrouponModel>(sql, new { pageIndex = pageIndex, pageSize = pageSize });
            }
        }
        public static IEnumerable<ShopSaleItemForGrouponModel> SelectPagesNew(
            string shopName,
            string province,
            string city,
            string area,
            int shopType,
            string category,
            string proName,
            int sales,
            int isActive,
            out int count,
            int pageIndex = 1,
            int pageSize = 10)
        {
            pageSize = pageSize > 100 ? 10 : pageSize;
            string Sqlcon = System.Configuration.ConfigurationManager.ConnectionStrings["Tuhu_Groupon_GungnirReadOnly"].ConnectionString;
            using (var conn = new SqlConnection(Sqlcon))
            {
                string sql_count = @"SELECT COUNT(1) FROM Tuhu_Groupon.dbo.ShopSaleItemForGroupon AS SG WITH(NOLOCK) 
left join [Gungnir].[dbo].[Shops] AS SP WITH(NOLOCK) 
ON SG.ShopId=SP.PKID
WHERE 1=1 {0}";
                string sql = @"SELECT SG.* FROM Tuhu_Groupon.dbo.ShopSaleItemForGroupon AS SG WITH(NOLOCK) 
left join [Gungnir].[dbo].[Shops] AS SP WITH(NOLOCK) 
ON SG.ShopId=SP.PKID
WHERE 1=1 {0}
ORDER BY SG.PKID
OFFSET @Begin ROWS FETCH NEXT @PageSize ROWS ONLY";
                conn.Open();
                string whereStr = "";
                if (!string.IsNullOrEmpty(shopName))
                {
                    whereStr = " AND SG.ShopName LIKE @shopName";
                    shopName = shopName.Trim()+"%";
                }
                if (!string.IsNullOrEmpty(proName))
                {
                    whereStr = whereStr + " AND SG.ProductName LIKE @proName";
                    proName = proName.Trim()+"%";
                }
                if (!string.IsNullOrEmpty(category))
                {
                    whereStr = whereStr + " AND SG.CategoryName LIKE @category";
                    category = category.Trim()+"%";
                }
                if (sales > 0)
                    whereStr = whereStr + (sales == 1 ? " AND SG.IsSale=1" : " AND SG.IsSale=0");
                if (isActive > 0)
                    whereStr = whereStr + (isActive == 1 ? " AND SG.isActive=1" : " AND SG.isActive=0");
                if (shopType > 0)
                {
                    if (shopType == 1)//工厂店
                        whereStr = whereStr + " AND SP.ShopType&512=512";
                    else
                    {
                        switch (shopType)
                        {
                            case 2://快修店
                                whereStr = whereStr + " AND SP.shopbusinesstype=N'b快修店'";
                                break;
                            case 3://维修店
                                whereStr = whereStr + " AND SP.shopbusinesstype=N'c修理厂'";
                                break;
                            case 4://4S店
                                whereStr = whereStr + " AND SP.shopbusinesstype=N'a4S店'";
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(province))
                {
                    whereStr = whereStr + " AND SP.[Province]=@province";
                    if (!string.IsNullOrEmpty(city))
                    {
                        whereStr = whereStr + " AND SP.[City]=@city";
                        if (!string.IsNullOrEmpty(area))
                            whereStr = whereStr + " AND SP.[District]=@area";
                    }
                }
                count = Convert.ToInt32(conn.ExecuteScalar(string.Format(sql_count, whereStr),
                    new
                    {
                        Begin = (pageIndex - 1) * pageSize,
                        PageSize = pageSize,
                        shopName = shopName,
                        proName = proName,
                        category = category,
                        sales = sales,
                        province = province,
                        city = city,
                        area = area
                    }));
                if (count > 0)
                    return conn.Query<ShopSaleItemForGrouponModel>(string.Format(sql, whereStr),
                        new
                        {
                            Begin = (pageIndex - 1) * pageSize,
                            PageSize = pageSize,
                            shopName = shopName,
                            proName = proName,
                            category = category,
                            sales = sales,
                            province = province,
                            city = city,
                            area = area
                        });
                return null;

            }
        }
        public static ShopSaleItemForGrouponModel GetEntity(SqlConnection connection, int PKID)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM Tuhu_Groupon.dbo.ShopSaleItemForGroupon WITH(NOLOCK) WHERE PKID = @PKID ";
                return conn.Query<ShopSaleItemForGrouponModel>(sql, new { PKID = PKID })?.FirstOrDefault();
            }
        }

        public static bool Insert(SqlConnection connection, ShopSaleItemForGrouponModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                INSERT INTO Tuhu_Groupon.dbo.ShopSaleItemForGroupon
								(
									ShopID,
									ShopName,
									CategoryId,
									CategoryName,
									ProductID,
									ProductName,
									BrandID,
									BrandName,
									ShowName,
									Description,
									DayLimit,
									RetailPrice,
									IsSale,
									SalePrice,
									SaleLimit,
									GrouponStutas,
									CheckStatus,
									CheckMemo,
									RecommendationId,
									Recommendation,
									SaleStartDate,
									SaleEndDate,
									IsActive,
									IsDeleted,
									CreatedBy,
									CreatedTime,
									AuditBy,
									AuditTime,
									UpdatedBy,
									UpdatedTime,
									PID,
									AdaptiveCar,
									ApplyTime
								)
                                VALUES
                                (
									@ShopID,
									@ShopName,
									@CategoryId,
									@CategoryName,
									@ProductID,
									@ProductName,
									@BrandID,
									@BrandName,
									@ShowName,
									@Description,
									@DayLimit,
									@RetailPrice,
									@IsSale,
									@SalePrice,
									@SaleLimit,
									@GrouponStutas,
									@CheckStatus,
									@CheckMemo,
									@RecommendationId,
									@Recommendation,
									@SaleStartDate,
									@SaleEndDate,
									@IsActive,
									@IsDeleted,
									@CreatedBy,
									@CreatedTime,
									@AuditBy,
									@AuditTime,
									@UpdatedBy,
									@UpdatedTime,
									@PID,
									@AdaptiveCar,
									@ApplyTime
								)";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Update(SqlConnection connection, ShopSaleItemForGrouponModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE  Tuhu_Groupon.dbo.ShopSaleItemForGroupon
                                SET	ShopID = @ShopID,
									ShopName = @ShopName,
									CategoryId = @CategoryId,
									CategoryName = @CategoryName,
									ProductID = @ProductID,
									ProductName = @ProductName,
									BrandID = @BrandID,
									BrandName = @BrandName,
									ShowName = @ShowName,
									Description = @Description,
									DayLimit = @DayLimit,
									RetailPrice = @RetailPrice,
									IsSale = @IsSale,
									SalePrice = @SalePrice,
									SaleLimit = @SaleLimit,
									GrouponStutas = @GrouponStutas,
									CheckStatus = @CheckStatus,
									CheckMemo = @CheckMemo,
									RecommendationId = @RecommendationId,
									Recommendation = @Recommendation,
									SaleStartDate = @SaleStartDate,
									SaleEndDate = @SaleEndDate,
									IsActive = @IsActive,
									IsDeleted = @IsDeleted,
									CreatedBy = @CreatedBy,
									CreatedTime = @CreatedTime,
									AuditBy = @AuditBy,
									AuditTime = @AuditTime,
									UpdatedBy = @UpdatedBy,
									UpdatedTime = @UpdatedTime,
									PID = @PID,
									AdaptiveCar = @AdaptiveCar,
									ApplyTime = @ApplyTime
								WHERE PKID = @PKID ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Delete(SqlConnection connection, int PKID)
        {
            using (IDbConnection conn = connection)
            {
                string sql = " DELETE Tuhu_Groupon.dbo.ShopSaleItemForGroupon WHERE PKID = @PKID ";
                return conn.Execute(sql, new { PKID = PKID }) > 0;
            }
        }
    }
}

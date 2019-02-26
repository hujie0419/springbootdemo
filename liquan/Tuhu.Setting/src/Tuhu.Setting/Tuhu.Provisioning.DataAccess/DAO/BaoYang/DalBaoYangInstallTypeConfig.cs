using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.BaoYang;
using Dapper;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.DAO.BaoYang
{
    public class DalBaoYangInstallTypeConfig
    {
        public List<InstallTypeConfig> SelectInstallTypeConfig(SqlConnection connection)
        {
            const string sql = @"SELECT
                                  PKID,
                                  PackageType,
                                  ImageUrl,
                                  InstallType,
                                  InstallTypeName,
                                  BaoYangTypes,
                                  IsDefault,
                                  NeedAll,
                                  TextFormat
                                FROM BaoYang..Config_InstallTypeConfig WITH ( NOLOCK )";

            List<InstallTypeConfig> result = new List<InstallTypeConfig>();

            var data = connection.Query(sql, commandType: CommandType.Text);

            foreach (var item in data)
            {
                if (result.Any(o => string.Equals(o.PackageType, item.PackageType)))
                {
                    var config = result.First(o => string.Equals(o.PackageType, item.PackageType));
                    
                    if (!config.InstallTypes.Any(o=> string.Equals(o.Type, item.InstallType)))
                    {
                        config.InstallTypes.Add(new InstallTypeConfigItem()
                        {
                            Type = item.InstallType,
                            Name = item.InstallTypeName,
                            BaoYangTypeList = (item.BaoYangTypes as string).Split(',').ToList(),
                            IsDefault = item.IsDefault,
                            NeedAll = item.NeedAll,
                            TextFormat = item.TextFormat
                        });
                    }
                }
                else
                {
                    result.Add(new InstallTypeConfig()
                    {
                        PackageType = item.PackageType,
                        ImageUrl = item.ImageUrl,
                        InstallTypes = new List<InstallTypeConfigItem>()
                        {
                            new InstallTypeConfigItem()
                            {
                                Type = item.InstallType,
                                Name = item.InstallTypeName,
                                BaoYangTypeList = (item.BaoYangTypes as string).Split(',').ToList(),
                                IsDefault = item.IsDefault,
                                NeedAll = item.NeedAll,
                                TextFormat = item.TextFormat
                            }
                        }
                    });
                }
            }

            return result;
        }

        public bool Update(SqlConnection conn, string packageType, string installType,
            bool isDefault, string textFormat)
        {
            const string sql = @"UPDATE baoyang..Config_InstallTypeConfig
                                SET IsDefault = @IsDefault, TextFormat = @TextFormat, LastUpdateDateTime = GETDATE()
                                WHERE PackageType = @PackageType AND InstallType = @InstallType";

            int rows = conn.Execute(sql, new
            {
                PackageType = packageType,
                InstallType = installType,
                IsDefault = isDefault,
                TextFormat = textFormat
            }, commandType: CommandType.Text);

            return rows > 0;
        }

        public bool UpdateImage(SqlConnection conn, string packageType, string imageUrl)
        {
            const string sql = @"UPDATE baoyang..Config_InstallTypeConfig
                                SET ImageUrl = @ImageUrl, LastUpdateDateTime = GETDATE()
                                WHERE PackageType = @PackageType";

            int rows = conn.Execute(sql, new
            {
                PackageType = packageType,
                ImageUrl = imageUrl
            }, commandType: CommandType.Text);

            return rows > 0;
        }

        public List<InstallTypeVehicleConfig> SelectVehicleConfigs(SqlConnection conn, string packageType, string installType,
            string brand, string series, string vehicleId, string categories, int minPrice, int maxPrice, string brands,
            bool isConfig, int pageIndex, int pageSize)
        {
            const string sql = @"WITH brands AS (
                                  SELECT * FROM Gungnir.dbo.SplitString (@Brands, ',', 1)
                                ),
                                categories as (
                                  SELECT * FROM Gungnir.dbo.SplitString (@Categories, ',', 1)
                                )
                                SELECT
                                  vt.ProductID AS VehicleId,
                                  vt.Brand,
                                  vt.Vehicle AS Series,
                                  (CASE WHEN vc.VehicleId IS NOT NULL
                                    THEN 1
                                   ELSE 0 END) AS IsRecommend
                                FROM
                                  Gungnir..tbl_Vehicle_Type AS vt WITH ( NOLOCK )
                                   LEFT JOIN (SELECT VehicleId
                                                 FROM BaoYang..Config_InstallTypeVehicleConfig WITH ( NOLOCK )
                                                 where PackageType = @PackageType and InstallType = @InstallType
                                                ) AS vc ON vt.ProductID = vc.VehicleId
                                COLLATE Chinese_PRC_CI_AS
                                WHERE (@Brand = '' OR vt.Brand = @Brand)
                                      AND (@Series = '' OR vt.Vehicle = @Series)
                                      AND (@VehicleId = '' OR vt.ProductID = @VehicleId)
                                      AND vt.AvgPrice BETWEEN @MinPrice AND @MaxPrice
                                      AND (@Brands = '' OR exists(SELECT 1
                                                                  FROM brands
                                                                  WHERE item = vt.Brand))
                                      AND (@Categories = '' OR exists(SELECT 1
                                                                      FROM categories
                                                                      WHERE item = vt.BrandCategory))
                                      AND (@IsConfig = 0 OR vc.VehicleId IS NOT NULL)
                                ORDER BY vt.Brand, vt.Vehicle
                                OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

            return conn.Query<InstallTypeVehicleConfig>(sql, new
            {
                PackageType = packageType,
                InstallType = installType,
                Brand = brand,
                Series = series,
                VehicleId = vehicleId,
                Categories = categories,
                minPrice = minPrice,
                maxPrice = maxPrice,
                Brands = brands,
                IsConfig = isConfig,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public int SelectVehicleConfigsCount(SqlConnection conn, string packageType, string installType,
            string brand, string series, string vehicleId, string categories, int minPrice, int maxPrice, string brands, bool isConfig)
        {
            const string sql = @"WITH brands AS (
                                    SELECT * FROM Gungnir.dbo.SplitString (@Brands, ',', 1)
                                ),
                                categories as (
                                    SELECT * FROM Gungnir.dbo.SplitString (@Categories, ',', 1)
                                )
                                SELECT
                                  COUNT(1)
                                FROM
                                  Gungnir..tbl_Vehicle_Type AS vt WITH ( NOLOCK )
                                  LEFT JOIN (SELECT VehicleId
                                                 FROM BaoYang..Config_InstallTypeVehicleConfig WITH ( NOLOCK )
                                                 where PackageType = @PackageType and InstallType = @InstallType
                                                ) AS vc ON vt.ProductID = vc.VehicleId
                                COLLATE Chinese_PRC_CI_AS
                                WHERE (@Brand = '' OR vt.Brand = @Brand)
                                      AND (@Series = '' OR vt.Vehicle = @Series)
                                      AND (@VehicleId = '' OR vt.ProductID = @VehicleId)
                                      AND vt.AvgPrice BETWEEN @MinPrice AND @MaxPrice
                                      AND (@Brands = '' OR exists(SELECT 1
                                                                  FROM brands
                                                                  WHERE item = vt.Brand))
                                      AND (@Categories = '' OR exists(SELECT 1
                                                                      FROM categories
                                                                      WHERE item = vt.BrandCategory))
                                      AND (@IsConfig = 0 OR vc.VehicleId IS NOT NULL)";

            return conn.ExecuteScalar<int>(sql, new
            {
                PackageType = packageType,
                InstallType = installType,
                Brand = brand,
                Series = series,
                VehicleId = vehicleId,
                Categories = categories,
                minPrice = minPrice,
                maxPrice = maxPrice,
                Brands = brands,
                IsConfig = isConfig
            }, commandType: CommandType.Text);
        }

        public bool SelectVehicleConfig(SqlConnection conn, string packageType, string installType, string vehicleId)
        {
            const string sql = @"SELECT 1
                                FROM baoyang..Config_InstallTypeVehicleConfig WITH ( NOLOCK )
                                WHERE PackageType = @PackageType AND InstallType = @InstallType AND VehicleId = @VehicleId";

            return conn.ExecuteScalar<int>(sql, new
            {
                PackageType = packageType,
                InstallType = installType,
                VehicleId = vehicleId
            }, commandType: CommandType.Text) > 0;
        }

        public bool InsertVehicleConfig(SqlConnection conn, string packageType, string installType, string vehicleId)
        {
            const string sql = @"INSERT INTO baoyang..Config_InstallTypeVehicleConfig
                                (PackageType, InstallType, VehicleId)
                                VALUES
                                (@PackageType, @InstallType, @VehicleId)";

            return conn.Execute(sql, new
            {
                PackageType = packageType,
                InstallType = installType,
                VehicleId = vehicleId
            }, commandType: CommandType.Text) > 0;
        }

        public bool DeleteVehicleConfig(SqlConnection conn, string packageType, string installType, string vehicleId)
        {
            const string sql = @"DELETE baoyang..Config_InstallTypeVehicleConfig
                                WHERE PackageType = @PackageType AND InstallType = @InstallType AND VehicleId = @VehicleId";

            return conn.Execute(sql, new
            {
                PackageType = packageType,
                InstallType = installType,
                VehicleId = vehicleId
            }, commandType: CommandType.Text) > 0;
        }
    }
}

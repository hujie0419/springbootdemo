using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.Models;

namespace Tuhu.C.SyncProductPriceJob.DataAccess
{
    public static class LianjiaXiaoqu
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LianjiaXiaoqu));

        /// <summary>
        /// 批量保存小区信息
        /// </summary>
        /// <param name="xiaoquList">小区列表</param>
        /// <returns></returns>
        public static async Task<int> SaveXiaoquInfoAsync(List<LianjiaXiaoquModel> xiaoquList)
        {
            if (xiaoquList == null || xiaoquList.Count == 0)
            {
                return 0;
            }

            var totalCount = xiaoquList.Count;
            var city = xiaoquList[0].City;
            var district = xiaoquList[0].District;

            var existsXiaoqu = SelectXiaoquId(city, district);
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                try
                {
                    await dbHelper.BeginTransactionAsync();
                    if (existsXiaoqu.Count > 0)
                    {
                        foreach (var group in xiaoquList.Where(_ => existsXiaoqu.Contains(_.XiaoquId)).Split(100))
                        {
                            var sql = string.Join(Environment.NewLine, group.ToArray().Select(_ => $@"UPDATE [dbo].[LianjiaXiaoquInfo]
SET [Name] = N'{_.Name?.Replace("'", "''") ?? ""}',
    [Age] = N'{_.Age?.Replace("'", "''") ?? ""}',
    [LinkUrl] = N'{_.LinkUrl.Replace("'", "''")}',
    [BuildingType] = N'{_.BuildingType?.Replace("'", "''") ?? ""}',
    [WuyeFee] = N'{_.WuyeFee?.Replace("'", "''") ?? ""}',
    [WuyeCompany] = N'{_.WuyeCompany?.Replace("'", "''") ?? ""}',
    [Developer] = N'{_.Developer?.Replace("'", "''") ?? ""}',
    [BuildingNum] = N'{_.BuildingNum?.Replace("'", "''") ?? ""}',
    [HouseNum] = N'{_.HouseNum?.Replace("'", "''") ?? ""}',
    [Longtitude] = N'{_.Longtitude?.Replace("'", "''") ?? ""}',
    [Latitude] = N'{_.Latitude?.Replace("'", "''") ?? ""}',
    [Price] = {_.Price},
    [Address] = N'{_.Address?.Replace("'", "''") ?? ""}',
    [Remark] = N'{_.Remark?.Replace("'", "''")}',
    [Remark1] = N'{_.Remark1?.Replace("'", "''")}',
    [LastUpdateDateTime] = GETDATE()
WHERE [XiaoquId] = {_.XiaoquId};"));
                            await dbHelper.ExecuteNonQueryAsync(sql);
                        }
                        // remove 已经更新的小区数据
                        xiaoquList.RemoveAll(_ => existsXiaoqu.Contains(_.XiaoquId));
                    }
                    // 新增数据
                    if (xiaoquList.Count > 0)
                    {
                        var dataTable = new DataTable("LianjiaXiaoquInfo");
                        dataTable.Columns.AddRange(new[]
                        {
                            new DataColumn("XiaoquId"),
                            new DataColumn("LinkUrl"),
                            new DataColumn("Name"),
                            new DataColumn("City"),
                            new DataColumn("District"),
                            new DataColumn("Age"),
                            new DataColumn("BuildingType"),
                            new DataColumn("WuyeFee"),
                            new DataColumn("WuyeCompany"),
                            new DataColumn("Developer"),
                            new DataColumn("BuildingNum"),
                            new DataColumn("HouseNum"),
                            new DataColumn("Longtitude"),
                            new DataColumn("Latitude"),
                            new DataColumn("Price"),
                            new DataColumn("Address"),
                            new DataColumn("Remark"),
                            new DataColumn("Remark1"),
                            new DataColumn("CreateDateTime"),
                            new DataColumn("LastUpdateDateTime")
                        });
                        foreach (var model in xiaoquList)
                        {
                            dataTable.Rows.Add(model.XiaoquId,
                                model.LinkUrl,
                                model.Name,
                                model.City,
                                model.District,
                                model.Age ?? "",
                                model.BuildingType ?? "",
                                model.WuyeFee ?? "",
                                model.WuyeCompany ?? "",
                                model.Developer ?? "",
                                model.BuildingNum ?? "",
                                model.HouseNum ?? "",
                                model.Longtitude ?? "",
                                model.Latitude ?? "",
                                model.Price,
                                model.Address ?? "",
                                model.Remark,
                                model.Remark1,
                                DateTime.Now,
                                DateTime.Now);
                        }

                        await dbHelper.BulkCopyAsync(dataTable);
                    }
                    dbHelper.Commit();
                    Logger.Info($"{city}-{district} 小区数据同步成功{totalCount}条数据");
                    return totalCount;
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    Logger.Error($"保存链家数据出错,【{city}-{district}】", ex);
                    return -1;
                }
            }
        }

        /// <summary>
        /// 保存单个小区信息
        /// </summary>
        /// <param name="model">小区信息</param>
        /// <returns></returns>
        public static async Task<bool> SaveXiaoquInfoAsync(LianjiaXiaoquModel model)
        {
            if (model == null)
            {
                return false;
            }

            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                var result = await dbHelper.ExecuteNonQueryAsync(@"UPDATE [dbo].[LianjiaXiaoquInfo]
                    SET [Name] = @Name,
                    [Age] = @Age,
                    [LinkUrl] = @LinkUrl,
                    [BuildingType] = @BuildingType,
                    [WuyeFee] = @WuyeFee,
                    [WuyeCompany] = @WuyeCompany,
                    [Developer] = @Developer,
                    [BuildingNum] = @BuildingNum,
                    [HouseNum] = @HouseNum,
                    [Longtitude] = @Longtitude,
                    [Latitude] = @Latitude,
                    [Price] = @Price,
                    [Address] = @Address,
                    [Remark] = @Remark,
                    [Remark1] = @Remark1,
                    [LastUpdateDateTime] = GETDATE()
                WHERE [XiaoquId] = @XiaoquId;",
                    CommandType.Text,
                    new SqlParameter("@XiaoquId", model.XiaoquId),
                    new SqlParameter("@Name", model.Name.Trim()),
                    new SqlParameter("@Age", model.Age ?? ""),
                    new SqlParameter("@LinkUrl", model.LinkUrl.Trim()),
                    new SqlParameter("@BuildingType", model.BuildingType?.Trim() ?? ""),
                    new SqlParameter("@WuyeFee", model.WuyeFee?.Trim() ?? ""),
                    new SqlParameter("@WuyeCompany", model.WuyeCompany?.Trim() ?? ""),
                    new SqlParameter("@Developer", model.Developer ?? ""),
                    new SqlParameter("@BuildingNum", model.BuildingNum?.Trim() ?? ""),
                    new SqlParameter("@HouseNum", model.HouseNum?.Trim() ?? ""),
                    new SqlParameter("@Longtitude", model.Longtitude?.Trim() ?? ""),
                    new SqlParameter("@Latitude", model.Latitude?.Trim() ?? ""),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Address", model.Address?.Trim() ?? ""),
                    new SqlParameter("@Remark", model.Remark?.Trim()),
                    new SqlParameter("@Remark1", model.Remark1?.Trim())
                );
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    //
                    result = await dbHelper.ExecuteNonQueryAsync(@"INSERT INTO [Tuhu_log].[dbo].[LianjiaXiaoquInfo]
(
    [XiaoquId],
    [LinkUrl],
    [Name],
    [City],
    [District],
    [Age],
    [BuildingType],
    [WuyeFee],
    [WuyeCompany],
    [Developer],
    [BuildingNum],
    [HouseNum],
    [Longtitude],
    [Latitude],
    [Price],
    [Address],
    [Remark],
    [Remark1],
    [CreateDateTime],
    [LastUpdateDateTime]
)
VALUES
(   @XiaoquId,         -- XiaoquId - bigint
    @LinkUrl,       -- LinkUrl - nvarchar(200)
    @Name,       -- Name - nvarchar(50)
    @City,       -- City - nvarchar(50)
    @District,       -- District - nvarchar(50)
    @Age,       -- Age - nvarchar(100)
    @BuildingType,       -- BuildingType - nvarchar(100)
    @WuyeFee,       -- WuyeFee - nvarchar(100)
    @WuyeCompany,       -- WuyeCompany - nvarchar(100)
    @Developer,       -- Developer - nvarchar(50)
    @BuildingNum,       -- BuildingNum - nvarchar(50)
    @HouseNum,       -- HouseNum - nvarchar(50)
    @Longtitude,       -- Longtitude - nvarchar(50)
    @Latitude,       -- Latitude - nvarchar(50)
    @Price,      -- Price - money
    @Address,       -- Address - nvarchar(100)
    @Remark,       -- Remark - nvarchar(100)
    @Remark1,       -- Remark1 - nvarchar(100)
    GETDATE(), -- CreateDateTime - datetime
    GETDATE()  -- LastUpdateDateTime - datetime
    )",
                    CommandType.Text,
                    new SqlParameter("@XiaoquId", model.XiaoquId),
                    new SqlParameter("@Name", model.Name),
                    new SqlParameter("@City", model.City),
                    new SqlParameter("@District", model.District),
                    new SqlParameter("@Age", model.Age ?? ""),
                    new SqlParameter("@LinkUrl", model.LinkUrl),
                    new SqlParameter("@BuildingType", model.BuildingType ?? ""),
                    new SqlParameter("@WuyeFee", model.WuyeFee ?? ""),
                    new SqlParameter("@WuyeCompany", model.WuyeCompany ?? ""),
                    new SqlParameter("@Developer", model.Developer ?? ""),
                    new SqlParameter("@BuildingNum", model.BuildingNum ?? ""),
                    new SqlParameter("@HouseNum", model.HouseNum ?? ""),
                    new SqlParameter("@Longtitude", model.Longtitude ?? ""),
                    new SqlParameter("@Latitude", model.Latitude ?? ""),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Address", model.Address ?? ""),
                    new SqlParameter("@Remark", model.Remark?.Trim()),
                    new SqlParameter("@Remark1", model.Remark1?.Trim())
                );
                    return result > 0;
                }
            }
        }

        /// <summary>
        /// 根据小区id判断小区是否存在
        /// </summary>
        /// <param name="xiaoquId">xiaoquId</param>
        /// <returns></returns>
        public static async Task<bool> ExistXiaoquInfoAsync(long xiaoquId)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                var result = await dbHelper.ExecuteScalarAsync(@"SELECT 1
FROM [Tuhu_log].[dbo].[LianjiaXiaoquInfo] WITH (NOLOCK)
WHERE [XiaoQuId] = @id AND [Age]!=''", CommandType.Text, new SqlParameter("@id", xiaoquId)
                {
                    SqlDbType = SqlDbType.BigInt
                });
                return Convert.ToInt32(result) > 0;
            }
        }

        /// <summary>
        /// 根据城市和区域查询已存在小区id
        /// </summary>
        /// <param name="city">城市</param>
        /// <param name="district">区</param>
        /// <returns></returns>
        public static IReadOnlyList<long> SelectXiaoquId(string city, string district)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var command = dbhelper.CreateCommand(@"
SELECT [XiaoquId]
FROM [dbo].[LianjiaXiaoquInfo] WITH (NOLOCK)
WHERE [City] = @city
AND [District] = @district"))
                {
                    command.AddParameter("@city", city);
                    command.AddParameter("@district", district);
                    return dbhelper.ExecuteQuery(command, dt => dt.ToList<long>());
                }
            }
        }
    }
}

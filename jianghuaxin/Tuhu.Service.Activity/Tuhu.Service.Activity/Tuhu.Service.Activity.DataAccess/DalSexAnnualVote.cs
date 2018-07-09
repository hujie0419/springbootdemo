using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalSexAnnualVote
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalSexAnnualVote));

        public static async Task<int> InsertShopVoteAsync(ShopVoteModel model)
        {
            using (var cmd = new SqlCommand(@"INSERT Activity.dbo.tbl_ShopVote (
                            ShopId,ImageUrls,CreateDate,Area,EmployeeCount,Description,VideoUrl,CreateTime)
                            VALUES(@shopId,@imageUrls,@createDate,@area,@employeeCount,@description,@videoUrl,GETDATE())"))
            {
                cmd.Parameters.AddWithValue("@shopId", model.ShopId);
                cmd.Parameters.AddWithValue("@imageUrls", model.ImageUrls);
                cmd.Parameters.AddWithValue("@createDate", model.CreateDate);
                cmd.Parameters.AddWithValue("@area", model.Area);
                cmd.Parameters.AddWithValue("@employeeCount", model.EmployeeCount);
                cmd.Parameters.AddWithValue("@description", model.Description);
                cmd.Parameters.AddWithValue("@videoUrl", model.VideoUrl);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<int> InsertShopEmployeeVoteAsync(ShopEmployeeVoteModel model)
        {
            using (var cmd = new SqlCommand(@"INSERT Activity.dbo.tbl_ShopEmployeeVote (
                            ShopId,EmployeeId,ImageUrls,Name,Age,City,YearsEmployed,Hobby,ExpertiseModels,ExpertiseProjects,Description,VideoUrl,CreateTime)
                            VALUES(@shopId,@employeeId,@imageUrls,@name,@age,@city,@yearsEmployed,@hobby,@expertiseModels,@expertiseProjects,@description,@videoUrl,GETDATE())")
            )
            {
                cmd.Parameters.AddWithValue("@shopId", model.ShopId);
                cmd.Parameters.AddWithValue("@employeeId", model.EmployeeId);
                cmd.Parameters.AddWithValue("@imageUrls", model.ImageUrls);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@age", model.Age);
                cmd.Parameters.AddWithValue("@city", model.City);
                cmd.Parameters.AddWithValue("@yearsEmployed", model.YearsEmployed);
                cmd.Parameters.AddWithValue("@hobby", model.Hobby);
                cmd.Parameters.AddWithValue("@expertiseModels", model.ExpertiseModels);
                cmd.Parameters.AddWithValue("@expertiseProjects", model.ExpertiseProjects);
                cmd.Parameters.AddWithValue("@description", model.Description);
                cmd.Parameters.AddWithValue("@videoUrl", model.VideoUrl);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<ShopVoteModel> GetShopVoteByShopIdAsync(long shopId)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Activity.dbo.tbl_ShopVote WITH(NOLOCK) WHERE ShopId=@shopId"))
            {
                cmd.Parameters.AddWithValue("@shopId", shopId);

                return await DbHelper.ExecuteFetchAsync<ShopVoteModel>(cmd);
            }
        }

        public static async Task<ShopEmployeeVoteModel> GetShopEmployeeIdVoteByEmployeeIdAsync(long shopId,long employeeId)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Activity.dbo.tbl_ShopEmployeeVote WITH(NOLOCK) WHERE ShopId=@shopId AND EmployeeId=@employeeId"))
            {
                cmd.Parameters.AddWithValue("@shopId", shopId);
                cmd.Parameters.AddWithValue("@employeeId", employeeId);

                return await DbHelper.ExecuteFetchAsync<ShopEmployeeVoteModel>(cmd);
            }
        }


        public static async Task<PagedModel<ShopVoteBaseModel>> SelectShopRankingAsync(SexAnnualVoteQueryRequest query)
        {
           
            string wheresql = " WHERE 1=1 ";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<SqlParameter> countSqlParameters = new List<SqlParameter>();
            if (query.ShopId > 0)
            {
                wheresql += " AND PKID=@shopId";//搜索用的是PKID
                sqlParameters.Add(new SqlParameter("@shopId", query.ShopId));
                countSqlParameters.Add(new SqlParameter("@shopId", query.ShopId));
            }
            if (query.ProvinceId > 0)
            {
                wheresql += " AND ProvinceId=@provinceId ";
                sqlParameters.Add(new SqlParameter("@provinceId", query.ProvinceId));
                countSqlParameters.Add(new SqlParameter("@provinceId", query.ProvinceId));
            }
            if (query.CityId > 0)
            {
                wheresql += " AND CityId=@cityId ";
                sqlParameters.Add(new SqlParameter("@cityId", query.CityId));
                countSqlParameters.Add(new SqlParameter("@cityId", query.CityId));
            }
            if (!string.IsNullOrWhiteSpace(query.Keywords))
            {
                wheresql += " AND ShopName LIKE @keywords ";
                sqlParameters.Add(new SqlParameter("@keywords", $"%{query.Keywords}%"));
                countSqlParameters.Add(new SqlParameter("@keywords", $"%{query.Keywords}%"));
            }
            var pager = new PagedModel<ShopVoteBaseModel>();
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT * FROM (SELECT  
	                            ROW_NUMBER() OVER(ORDER BY VoteNumber DESC) AS Ranking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId ORDER BY VoteNumber DESC) AS ProvinceRanking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId, CityId ORDER BY VoteNumber DESC) AS CityRanking,
	                            VoteNumber,
	                            ShopName,
                                Image,
                                PKID,
	                            ShopId,
	                            ProvinceId,
	                            CityId,
	                            ImageUrls
                            FROM Activity..tbl_ShopVoteDetail WITH(NOLOCK)) AS T {wheresql} 
                            ORDER BY T.Ranking ASC
                            OFFSET {query.PageSize*(query.PageIndex-1)} ROWS
                            FETCH NEXT {query.PageSize} ROWS ONLY;"))
                {
                    cmd.Parameters.AddRange(sqlParameters.ToArray());
                    pager.Source = await helper.ExecuteSelectAsync<ShopVoteBaseModel>(cmd);
                }
                using (var cmd = helper.CreateCommand($@"SELECT COUNT(1) FROM Activity..tbl_ShopVoteDetail WITH(NOLOCK) {wheresql}"))
                {
                    cmd.Parameters.AddRange(countSqlParameters.ToArray());
                    pager.Pager = new PagerModel()
                    {
                        Total = (int) await helper.ExecuteScalarAsync(cmd),
                        PageSize = query.PageSize,
                        CurrentPage = query.PageIndex
                    };
                }
            }
            return pager;
        }

        public static async Task<PagedModel<ShopEmployeeVoteBaseModel>> SelectShopEmployeeRankingAsync(SexAnnualVoteQueryRequest query) {
            string wheresql = " WHERE 1=1 ";
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<SqlParameter> countSqlParameters = new List<SqlParameter>();
            if (query.ShopId > 0)
            {
                wheresql += " AND ShopId=@shopId";
                sqlParameters.Add(new SqlParameter("@shopId", query.ShopId));
                countSqlParameters.Add(new SqlParameter("@shopId", query.ShopId));
            }
            if (query.EmployeeId > 0)
            {
                wheresql += " AND PKID=@employeeId";//搜索用的是PKID
                sqlParameters.Add(new SqlParameter("@employeeId", query.EmployeeId));
                countSqlParameters.Add(new SqlParameter("@employeeId", query.EmployeeId));
            }
            if (query.ProvinceId > 0)
            {
                wheresql += " AND ProvinceId=@provinceId ";
                sqlParameters.Add(new SqlParameter("@provinceId", query.ProvinceId));
                countSqlParameters.Add(new SqlParameter("@provinceId", query.ProvinceId));
            }
            if (query.CityId > 0)
            {
                wheresql += " AND CityId=@cityId ";
                sqlParameters.Add(new SqlParameter("@cityId", query.CityId));
                countSqlParameters.Add(new SqlParameter("@cityId", query.CityId));
            }
            if (!string.IsNullOrWhiteSpace(query.Keywords))
            {
                wheresql += " AND Name LIKE @keywords";
                sqlParameters.Add(new SqlParameter("@keywords", $"%{query.Keywords}%"));
                countSqlParameters.Add(new SqlParameter("@keywords", $"%{query.Keywords}%"));
            }
            var pager = new PagedModel<ShopEmployeeVoteBaseModel>();
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT * FROM (SELECT  
	                            ROW_NUMBER() OVER(ORDER BY VoteNumber DESC) AS Ranking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId ORDER BY VoteNumber DESC) AS ProvinceRanking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId, CityId ORDER BY VoteNumber DESC) AS CityRanking,
	                            VoteNumber,
                                Name,
                                ShopName,
                                EmployeeAvatar,
                                PKID,
	                            ShopId,
                                EmployeeId,
	                            ProvinceId,
	                            CityId,
	                            ImageUrls
                            FROM Activity..tbl_ShopEmployeeVoteDetail WITH(NOLOCK)) AS T {wheresql} 
                            ORDER BY T.Ranking ASC
                            OFFSET {query.PageSize * (query.PageIndex - 1)} ROWS
                            FETCH NEXT {query.PageSize} ROWS ONLY;"))
                {
                    cmd.Parameters.AddRange(sqlParameters.ToArray());
                    pager.Source = await helper.ExecuteSelectAsync<ShopEmployeeVoteBaseModel>(cmd);
                }
                using (var cmd = helper.CreateCommand($@"SELECT COUNT(1) FROM Activity..tbl_ShopEmployeeVoteDetail WITH(NOLOCK) {wheresql}"))
                {
                    cmd.Parameters.AddRange(countSqlParameters.ToArray());
                    pager.Pager = new PagerModel()
                    {
                        Total = (int)await helper.ExecuteScalarAsync(cmd),
                        PageSize = query.PageSize,
                        CurrentPage = query.PageIndex
                    };
                }
            }
            return pager;
        }

        public static async Task<IEnumerable<ShopVoteBaseModel>> SelectShopRegionAsync() {
            using (var cmd = new SqlCommand(@"SELECT DISTINCT ProvinceId,CityId FROM Activity..tbl_ShopVoteDetail WITH(NOLOCK)"))
            {
                return await DbHelper.ExecuteSelectAsync<ShopVoteBaseModel>(cmd);
            }
        }

        public static async Task<IEnumerable<ShopVoteBaseModel>> SelectShopEmployeeRegionAsync()
        {
            using (var cmd = new SqlCommand(@"SELECT DISTINCT ProvinceId,CityId FROM Activity..tbl_ShopEmployeeVoteDetail WITH(NOLOCK)"))
            {
                return await DbHelper.ExecuteSelectAsync<ShopVoteBaseModel>(cmd);
            }
        }

        public static async Task<ShopVoteModel> FetchShopDetailAsync(long shopId)
        {
                using (var cmd = new SqlCommand(@"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY VoteNumber DESC) AS Ranking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId ORDER BY VoteNumber DESC) AS ProvinceRanking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId, CityId ORDER BY VoteNumber DESC) AS CityRanking, 
								D.* FROM Activity..tbl_ShopVoteDetail AS D
                                WITH(NOLOCK)) AS T WHERE T.ShopId=@shopId"))
                {
                    cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                    return await DbHelper.ExecuteFetchAsync<ShopVoteModel>(true,cmd);
                }
        }

        public static async Task<ShopEmployeeVoteModel> FetchShopEmployeeDetailAsync(long shopId, long employeeId)
        {
            var sql = @"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY VoteNumber DESC) AS Ranking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId ORDER BY VoteNumber DESC) AS ProvinceRanking,
	                            ROW_NUMBER() OVER(PARTITION BY ProvinceId, CityId ORDER BY VoteNumber DESC) AS CityRanking, 
								D.* FROM Activity..tbl_ShopEmployeeVoteDetail AS D
                                WITH(NOLOCK)) AS T WHERE T.EmployeeId=@employeeId";
            if (shopId > 0)
            {
                sql += " AND T.ShopId=@shopId";
            }
            using (var cmd = new SqlCommand(sql)
            )
            {
                cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                return await DbHelper.ExecuteFetchAsync<ShopEmployeeVoteModel>(true,cmd);
            }
        }

        public static async Task<IEnumerable<ShopEmployeVoteCommentModel>> SelectShopEmployeeCommentAsync(long shopId,
            long employeeId)
        {
            using (var cmd = new SqlCommand(@"SELECT isnull(ImageUrls, '') as ImageUrls, PKID, ShopId,EmployeeId,NickName,Avatar,Score,CreateTime,Content FROM Activity..tbl_ShopEmployeeComment WITH(NOLOCK) WHERE ShopId=@shopId AND EmployeeId=@employeeId"))
            {
                cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                return await DbHelper.ExecuteSelectAsync<ShopEmployeVoteCommentModel>(true, cmd);
            }
        }

        public static async Task<int> AddShopVoteAsync(Guid userId, long shopId)
        {
            using (var helper = DbHelper.CreateDbHelper())
            {
                await helper.BeginTransactionAsync();
                int result = 0;
                using (var cmd =
                    helper.CreateCommand(
                        @"INSERT Activity..tbl_ShopVoteRecord (UserId,ShopId,Share,CreateTime) VALUES(@userId,@shopId,0,GETDATE())")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@userId", userId));
                    cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                    result  = await helper.ExecuteNonQueryAsync(cmd);
                }
                if (result == 0) helper.Rollback();
                else
                {
                    using (var cmd =
                        helper.CreateCommand(
                            @"UPDATE Activity..tbl_ShopVoteDetail WITH(ROWLOCK) SET VoteNumber=ISNULL(VoteNumber,0)+1 WHERE ShopId=@shopId")
                    )
                    {
                        cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                        result = await helper.ExecuteNonQueryAsync(cmd);
                    }
                    if (result > 0)
                    {
                        helper.Commit();
                    }
                    else
                    {
                        helper.Rollback();
                    }
                }
                return result;
            }
        }

        public static async Task<IEnumerable<ShopVoteRecordModel>> SelectShopVoteRecordAsync(Guid userId,
            DateTime startDate, DateTime endDate)
        {
            using (var cmd =
                new SqlCommand(
                    "SELECT * FROM Activity..tbl_ShopVoteRecord WITH(NOLOCK) WHERE UserId=@userId AND CreateTime BETWEEN @startDate AND @endDate")
            )
            {
                cmd.Parameters.Add(new SqlParameter("@userId", userId));
                cmd.Parameters.Add(new SqlParameter("@startDate", startDate.ToString("yyyy-MM-dd")));
                cmd.Parameters.Add(new SqlParameter("@endDate", $"{endDate:yyyy-MM-dd} 23:59:59"));
                return await DbHelper.ExecuteSelectAsync<ShopVoteRecordModel>(cmd);
            }
        }

        public static async Task<int> AddShopEmployeeVoteAsync(Guid userId, long shopId, long employeeId)
        {
            using (var helper = DbHelper.CreateDbHelper())
            {
                await helper.BeginTransactionAsync();
                int result = 0;
                using (var cmd =
                    helper.CreateCommand(
                        @"INSERT Activity..tbl_ShopEmployeeVoteRecord (UserId,ShopId,EmployeeId,Share,CreateTime) VALUES(@userId,@shopId,@employeeId,0,GETDATE())")
                )
                {
                    cmd.Parameters.Add(new SqlParameter("@userId", userId));
                    cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                    cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                    result = await helper.ExecuteNonQueryAsync(cmd);
                }
                if (result == 0) helper.Rollback();
                else
                {
                    using (var cmd =
                        helper.CreateCommand(
                            @"UPDATE Activity..tbl_ShopEmployeeVoteDetail WITH(ROWLOCK) SET VoteNumber=ISNULL(VoteNumber,0)+1 WHERE ShopId=@shopId AND EmployeeId=@employeeId")
                    )
                    {
                        cmd.Parameters.Add(new SqlParameter("@shopId", shopId));
                        cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                        result = await helper.ExecuteNonQueryAsync(cmd);
                    }
                    if (result > 0)
                    {
                        helper.Commit();
                    }
                    else
                    {
                        helper.Rollback();
                    }
                }
                return result;
            }
        }

        public static async Task<IEnumerable<ShopEmployeeVoteRecordModel>> SelectShopEmployeeVoteRecordAsync(
            Guid userId, DateTime startDate, DateTime endDate)
        {
            using (var cmd = new SqlCommand(
                "SELECT * FROM Activity..tbl_ShopEmployeeVoteRecord WITH(NOLOCK) WHERE UserId=@userId AND CreateTime BETWEEN @startDate AND @endDate")
            )
            {
                cmd.Parameters.Add(new SqlParameter("@userId", userId));
                cmd.Parameters.Add(new SqlParameter("@startDate", startDate.ToString("yyyy-MM-dd")));
                cmd.Parameters.Add(new SqlParameter("@endDate", $"{endDate:yyyy-MM-dd} 23:59:59"));
                return await DbHelper.ExecuteSelectAsync<ShopEmployeeVoteRecordModel>(cmd);
            }
        }

        public static async Task<int> AddShareShopVoteAsync(Guid userId, long shopId)
        {
            using (var cmd = new SqlCommand(
                @"UPDATE Activity..tbl_ShopVoteRecord WITH(ROWLOCK) SET Share=1 WHERE PKID=(SELECT MIN(PKID) FROM Activity..tbl_ShopVoteRecord WITH(NOLOCK) WHERE UserId=@userId AND CreateTime BETWEEN @startDate AND @endDate)")
            )
            {
                cmd.Parameters.Add(new SqlParameter("@userId", userId));
                cmd.Parameters.Add(new SqlParameter("@startDate", DateTime.Now.ToString("yyyy-MM-dd")));
                cmd.Parameters.Add(new SqlParameter("@endDate", $"{DateTime.Now.ToString("yyyy-MM-dd")} 23:59:59"));
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<int> AddShareShopEmployeeVoteAsync(Guid userId, long shopId, long employeeId)
        {
            using (var cmd = new SqlCommand(
                @"UPDATE Activity..tbl_ShopEmployeeVoteRecord WITH(ROWLOCK) SET Share=1 WHERE PKID = (SELECT MIN(PKID) FROM Activity..tbl_ShopEmployeeVoteRecord WITH(NOLOCK) WHERE UserId=@userId AND CreateTime BETWEEN @startDate AND @endDate)")
            )
            {
                cmd.Parameters.Add(new SqlParameter("@userId", userId));
                cmd.Parameters.Add(new SqlParameter("@startDate", DateTime.Now.ToString("yyyy-MM-dd")));
                cmd.Parameters.Add(new SqlParameter("@endDate", $"{DateTime.Now.ToString("yyyy-MM-dd")} 23:59:59"));
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }
    }
}

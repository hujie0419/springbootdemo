using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using System.Linq;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.Request;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 活动
    /// </summary>
    public class ActivityRepository : IActivityRepository
    {
        private string DBName = "T_wpf_Activity";
        private readonly IDbHelperFactory _factory;

        public ActivityRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 获取所有活动列表数量
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> GetActivityListCountAsync(GetActivityListRequest request, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT count(1) 
                            FROM T_wpf_Activity  with (nolock)
                            WHERE IsDeleted=@IsDeleted
                        ;";
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sqlParaments.Add(new SqlParameter("@IsDeleted", false));
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    int count = (int)(await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return count;
                }
            }
        }


        /// <summary>
        /// 获取所有活动列表
        /// </summary
        /// <param name="Request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<ActivityEntity>> GetActivityListAsync(GetActivityListRequest request, CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"SELECT [PKID]
                                ,[Name]
                                ,[Description]
                                ,[StartTime]
                                ,[EndTime]
                                ,[Status]
                                ,[IsDeleted]
                                ,[CreateTime]
                                ,[CreateUser]
                                ,[UpdateTime]
                                ,[UpdateUser]  
                        FROM {DBName}  with (nolock)
                        WHERE IsDeleted=@IsDeleted
                    ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sqlParaments.Add(new SqlParameter("@IsDeleted", false));

            //分页
            {
                sql.Append("ORDER BY PKID DESC  OFFSET(@PageSize * (@CurrentPage - 1)) ROWS FETCH NEXT @PageSize ROWS ONLY ");
                sqlParaments.Add(new SqlParameter("@CurrentPage", request.CurrentPage));
                sqlParaments.Add(new SqlParameter("@PageSize", request.PageSize));
            }
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteSelectAsync<ActivityEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result?.ToList();
                }
            }   
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="ActivityID">活动ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<ActivityEntity> GetActivityInfoAsync(int ActivityID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                  ,[Name]
                                  ,[Description]
                                  ,[StartTime]
                                  ,[EndTime]
                                  ,[Status]
                                  ,[CreateTime]
                                  ,[CreateUser]
                                  ,[UpdateTime]
                                  ,[UpdateUser]
                            FROM {DBName}  with (nolock)
                            WHERE PKID=@ActivityID
                        ;";
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sqlParaments.Add(new SqlParameter("@ActivityID", ActivityID));
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteFetchAsync<ActivityEntity>(cmd, cancellationToken).ConfigureAwait(false))??new ActivityEntity();
                    return result;
                }
            }
        }
    }
}

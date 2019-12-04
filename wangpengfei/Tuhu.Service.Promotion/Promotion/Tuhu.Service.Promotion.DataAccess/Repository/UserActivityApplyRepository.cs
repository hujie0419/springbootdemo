using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using System.Linq;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.Request;
using System.Reflection;
using System.Text;
using Tuhu.Service.Promotion.DataAccess.QueryModel;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 活动申请
    /// </summary>
    public class UserActivityApplyRepository : IUserActivityApplyRepository
    {
        private string DBName = "T_wpf_UserActivityApply";
        private readonly IDbHelperFactory _factory;

        public UserActivityApplyRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 活动申请信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> GetUserActivityApplyListCountAsync(GetUserActivityApplyListQueryModel request,
            CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"SELECT COUNT(1)
                    FROM [T_wpf_UserActivityApply] ua with (nolock)
					  inner join T_wpf_User u with (nolock) on u.pkid=ua.UserId
					  inner join T_wpf_Activity a with (nolock) on a.pkid=ua.ActivityId
                    WHERE ua.IsDeleted=0
                        ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            if (request.UserId > 0)
            {
                sql.Append(" and ua.UserId = @UserId ");
                sqlParaments.Add(new SqlParameter("@UserId", request.UserId));
            }
            if (request.ActivityID > 0)
            {
                sql.Append(" and ua.ActivityID = @ActivityID ");
                sqlParaments.Add(new SqlParameter("@ActivityID", request.ActivityID));
            }
            if (request.Status > 0)
            {
                sql.Append(" and ua.Status = @Status ");
                sqlParaments.Add(new SqlParameter("@Status", request.Status));
            }
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    int count = (int)(await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return count;
                }
            }
        }

        /// <summary>
        /// 活动申请信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<UserActivityApplyEntity>> GetUserActivityApplyListAsync(GetUserActivityApplyListQueryModel request,
            CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"SELECT ua.[PKID]
                      ,ua.[UserID]
                      ,ua.[ActivityID]
                      ,ua.[ApplyTime]
                      ,ua.[PassTime]
                      ,ua.[Remark]
                      ,ua.[IsDeleted]
                      ,ua.[Status]
                      ,ua.[CreateTime]
                      ,ua.[CreateUser]
                      ,ua.[UpdateTime]
                      ,ua.[UpdateUser]
					  ,u.UserName
					  ,u.RealName
					  ,a.Name ActivityName
                    FROM [T_wpf_UserActivityApply] ua with (nolock)
					  inner join T_wpf_User u with (nolock) on u.PKID=ua.UserID
					  inner join T_wpf_Activity a with (nolock) on a.PKID=ua.ActivityID
                    WHERE ua.IsDeleted=0
                        ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            if (request.UserId > 0)
            {
                sql.Append(" and ua.UserID = @UserID ");
                sqlParaments.Add(new SqlParameter("@UserID", request.UserId));
            }
            if (request.ActivityID > 0)
            {
                sql.Append(" and ua.ActivityID = @ActivityID ");
                sqlParaments.Add(new SqlParameter("@ActivityID", request.ActivityID));
            }
            if (request.Status > 0)
            {
                sql.Append(" and ua.Status = @Status ");
                sqlParaments.Add(new SqlParameter("@Status", request.Status));
            }
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
                    var result = (await dbHelper.ExecuteSelectAsync<UserActivityApplyEntity>(cmd, cancellationToken).
                        ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 根据PKID自动通过活动申请
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> BatchPassUserActivityApplyByPKIDsAsync(List<int> PKIDs,
            CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"UPDATE T_wpf_UserActivityApply SET Status = 2
                                        ,PassTime = getdate()
                                        ,UpdateTime = getdate()
                            FROM T_wpf_UserActivityApply 
                            where PKID in (@PKIDs)
                            ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sqlParaments.Add(new SqlParameter("@PKIDs", string.Join(",", PKIDs)));
            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return true;
                }
            }
        }

        /// <summary>
        /// 获取可自动审核的活动申请数据
        /// </summary>
        /// <param name="AreaIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<int>> GetAutoPassUserActivityApplyPKIDsAsync(GetAutoPassUserActivityApplyPKIDsRequest request,
            CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"select ua.PKID 
                            FROM T_wpf_UserActivityApply ua
                            INNER JOIN T_wpf_User u on u.PKID=ua.UserId
                            INNER JOIN t_wpf_region r on r.pkid=u.areaID
                            WHERE ua.Status=1
                            AND ua.IsDeleted=0
                            ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sql.Append(" AND r.AreaID in (@AreaIDs) ");
            sqlParaments.Add(new SqlParameter("@AreaIDs", request.AreaIDs));

            sql.Append(" AND r.PKID > @minPKID ");
            sqlParaments.Add(new SqlParameter("@minPKID", request.minPKID));
            //分页
            {
                sql.Append("ORDER BY PKID DESC  OFFSET(@PageSize * (@CurrentPage - 1)) ROWS FETCH NEXT @PageSize ROWS ONLY ");
                sqlParaments.Add(new SqlParameter("@CurrentPage", request.CurrentPage));
                sqlParaments.Add(new SqlParameter("@PageSize", request.PageSize));
            }
            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteSelectAsync<UserActivityApplyEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList()?.Select(s => s.PKID).ToList();
                }
            }
        }

        /// <summary>
        /// 新增活动申请
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> CreateUserActivityApplyAsync(CreateUserActivityApplyRequest request,
            CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   ([UserID]
                                   ,[ActivityID]
                                   ,[ApplyTime]
                                   ,[Remark]
                                   ,[IsDeleted]
                                   ,[Status]
                                   ,[CreateTime]
                                   ,[CreateUser]
                                   ,[UpdateTime]
                                   ,[UpdateUser])
                             VALUES(@UserID
                                    ,@ActivityID
                                    ,getdate()
                                    ,@Remark
                                    ,0
                                    ,1
                                    ,getdate()
                                    ,@CreateUser
                                    ,getdate()
                                    ,@UpdateUser)
                            ";
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sqlParaments.Add(new SqlParameter("@UserID", request.UserId));
            sqlParaments.Add(new SqlParameter("@ActivityID", request.ActivityId));
            sqlParaments.Add(new SqlParameter("@Remark", request.Remark));
            sqlParaments.Add(new SqlParameter("@CreateUser", request.CreateUser));
            sqlParaments.Add(new SqlParameter("@UpdateUser", request.CreateUser));
            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return true;
                }
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> DeleteUserActivityApplyByPKIDAsync(int PKID,
            CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"UPDATE T_wpf_UserActivityApply SET IsDeleted = 1
                                        ,UpdateTime = getdate()
                            FROM T_wpf_UserActivityApply 
                            where PKID =@PKID
                            ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            sqlParaments.Add(new SqlParameter("@PKID", PKID));
            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return true;
                }
            }
        }
    }
}

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
using System;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 人员
    /// </summary>
    public class UserRepository:IUserRepository
    {
        private string DBName = "T_wpf_User";
        private readonly IDbHelperFactory _factory;

        public UserRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 人员信息查询数量
        /// </summary>
        /// <param name="request">查询条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> GetUserListCountAsync(GetUserListRequest request, CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@" select count(1) from (SELECT DISTINCT u.[PKID]
                          ,u.[UserName]
                          ,u.[PassWord]
                          ,u.[RealName]
                          ,u.[Phone]
                          ,u.[ProvinceID]
                          ,u.[CityID]
                          ,u.[AreaID]
                          ,u.[Address]
                          ,u.[IsDeleted]
                          ,u.[Status]
                          ,u.[CreateTime]
                          ,u.[CreateUser]
                          ,u.[UpdateTime]
                          ,u.[UpdateUser]
                          ,r.ProvinceName
                          ,r.CityName
                          ,r.AreaName
                      FROM {DBName} u with (nolock)
                            INNER JOIN t_wpf_region R with (nolock) on r.pkid=u.areaID
                        ");
            if (!request.IsOnlyUser)
            {
                sql.Append(" INNER JOIN T_wpf_UserActivityApply ua with (nolock) on u.pkid=ua.UserID and ua.IsDeleted=0 ");
            }
            sql.Append(" WHERE u.IsDeleted=0 ");
            #endregion

            //model转SqlParameter  后面可以封装起来
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            foreach (PropertyInfo info in request.GetType().GetProperties())
            {
                if(info.Name == "IsOnlyUser"|| info.Name == "PageSize" || info.Name == "CurrentPage")
                {
                    continue;
                }
                if (info.PropertyType.FullName == typeof(DateTime).FullName)
                {
                    DateTime pValue = (DateTime)info.GetValue(request, null);
                    if (pValue == null || pValue == DateTime.MinValue)
                    {
                        continue;
                    }
                }
                else if (info.PropertyType.FullName == typeof(Int32).FullName)
                {
                    int pValue = (int)info.GetValue(request, null);
                    if (pValue == 0)
                    {
                        continue;
                    }
                }
                else if (info.PropertyType.FullName == typeof(Boolean).FullName)
                {
                    Object pValue = info.GetValue(request, null);
                    if (pValue == null)
                    {
                        continue;
                    }
                }
                else if (info.PropertyType.FullName == typeof(String).FullName)
                {
                    object pValue = info.GetValue(request, null);
                    if (pValue == null || string.IsNullOrEmpty(pValue.ToString()))
                    {
                        continue;
                    }
                }
                sqlParaments.Add(new SqlParameter($"@{info.Name}", info.GetValue(request)));
                sql.Append($" and u.{info.Name}=@{info.Name}");
            }
            sql.Append($" ) tab");
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    if (sqlParaments.Any())
                    {
                        cmd.Parameters.AddRange(sqlParaments.ToArray());
                    }
                    int count = (int)(await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return count;
                }
            }
        }
        /// <summary>
        /// 人员信息查询
        /// </summary>
        /// <param name="request">查询条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<UserEntity>> GetUserListAsync(GetUserListRequest request, CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
            sql.Append($@"select * from ( SELECT DISTINCT u.[PKID]
                          ,u.[UserName]
                          ,u.[PassWord]
                          ,u.[RealName]
                          ,u.[Phone]
                          ,u.[ProvinceID]
                          ,u.[CityID]
                          ,u.[AreaID]
                          ,u.[Address]
                          ,u.[IsDeleted]
                          ,u.[Status]
                          ,u.[CreateTime]
                          ,u.[CreateUser]
                          ,u.[UpdateTime]
                          ,u.[UpdateUser]
                          ,r.ProvinceName
                          ,r.CityName
                          ,r.AreaName
                      FROM {DBName} u with (nolock)
                            INNER JOIN t_wpf_region R with (nolock) on r.pkid=u.areaID
                        ");
            if(!request.IsOnlyUser)
            {
                sql.Append(" INNER JOIN T_wpf_UserActivityApply ua with (nolock) on u.pkid=ua.UserID and ua.IsDeleted=0 ");
            }
            sql.Append(" WHERE u.IsDeleted=0 ");
            #endregion

            //model转SqlParameter  后面可以封装起来
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            foreach (PropertyInfo info in request.GetType().GetProperties())
            {
                if (info.Name == "IsOnlyUser" || info.Name == "PageSize" || info.Name == "CurrentPage")
                {
                    continue;
                }
                if (info.PropertyType.FullName == typeof(DateTime).FullName)
                {
                    DateTime pValue = (DateTime)info.GetValue(request, null);
                    if (pValue == null || pValue == DateTime.MinValue)
                    {
                        continue;
                    }
                }
                else if (info.PropertyType.FullName == typeof(Int32).FullName)
                {
                    int pValue = (int)info.GetValue(request, null);
                    if (pValue == 0)
                    {
                        continue;
                    }
                }
                else if (info.PropertyType.FullName == typeof(Boolean).FullName)
                {
                    Object pValue = info.GetValue(request, null);
                    if(pValue == null)
                    {
                        continue;
                    }
                }
                else if (info.PropertyType.FullName == typeof(String).FullName)
                {
                    object pValue = info.GetValue(request, null);
                    if(pValue == null||string.IsNullOrEmpty(pValue.ToString()))
                    {
                        continue;
                    }
                }
                sqlParaments.Add(new SqlParameter($"@{info.Name}", info.GetValue(request)));
                sql.Append($" and u.{info.Name}=@{info.Name}");
            }

            //分页
            {
                sql.Append(")tab ORDER BY PKID DESC  OFFSET(@PageSize * (@CurrentPage - 1)) ROWS FETCH NEXT @PageSize ROWS ONLY ");
                sqlParaments.Add(new SqlParameter("@CurrentPage", request.CurrentPage));
                sqlParaments.Add(new SqlParameter("@PageSize", request.PageSize));
            }
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    if (sqlParaments.Any())
                    {
                        cmd.Parameters.AddRange(sqlParaments.ToArray());
                    }
                    var result = (await dbHelper.ExecuteSelectAsync<UserEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 登录验证  Demo密码没做加密，线上可以机密或加密加盐
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> Login(string UserName,string PassWord, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT UserName
                      FROM {DBName} with (nolock)
                            WHERE UserName=@UserName AND PassWord=@PassWord
                    ;";
            #endregion
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@UserName", UserName));
                    cmd.Parameters.Add(new SqlParameter("@PassWord", PassWord));
                    var result = (await dbHelper.ExecuteSelectAsync<UserEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.Any();
                }
            }
        }
    }
}

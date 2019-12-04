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
    /// 区域
    /// </summary>
    public class RegionRepository : IRegionRepository
    {
        private string DBName = "T_wpf_Region";
        private readonly IDbHelperFactory _factory;

        public RegionRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 获取区域数据
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="IsALL"></param>
        /// <returns></returns>
        public async ValueTask<List<RegionEntity>> GetRegionListAsync(CancellationToken cancellationToken, bool IsALL = false)
        {
            #region sql
            StringBuilder sql = new StringBuilder();
             sql.Append( $@"SELECT [PKID]
                          ,[Name]
                          ,[SortNO]
                          ,[Layer]
                          ,[ParentId]
                          ,[Provinceid]
                          ,[ProvinceName]
                          ,[CityId]
                          ,[CityName]
                          ,[AreaId]
                          ,[AreaName]
                          ,[IsDeleted]
                          ,[Status]
                          ,[CreateTime]
                          ,[CreateUser]
                          ,[UpdateTime]
                          ,[UpdateUser]
                      FROM {DBName} with (nolock)
                            WHERE 1=1
                        ");
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();
            if(IsALL)
            {
                sql.Append(" and IsDeleted = @IsDeleted ");
                sqlParaments.Add(new SqlParameter("@IsDeleted", false));
            }
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray()); 
                    var result = (await dbHelper.ExecuteSelectAsync<RegionEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }
    }
}

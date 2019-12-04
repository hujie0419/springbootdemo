using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 待发
    /// </summary>
    public class PromotionSingleTaskUsersRepository : IPromotionSingleTaskUsersRepository
    {
        private string DBName = "Gungnir..tbl_PromotionSingleTaskUsers";
        private readonly IDbHelperFactory _factory;
        public PromotionSingleTaskUsersRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> CreateAsync(PromotionSingleTaskUsersEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   (PromotionSingleTaskUsersId
                                   ,PromotionTaskId
                                   ,UserCellPhone
                                   ,CeateTime
                                   ,OrderNo
                                    )
                             VALUES
                                   (
                                    @PromotionSingleTaskUsersId
                                   ,@PromotionTaskId
                                   ,@UserCellPhone
                                   ,getdate()
                                   ,@OrderNo
                                );
                                select SCOPE_IDENTITY();";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionSingleTaskUsersId", entity.PromotionSingleTaskUsersId));
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", entity.PromotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@UserCellPhone", entity.UserCellPhone));
                    cmd.Parameters.Add(new SqlParameter("@OrderNo", entity.OrderNo));

                    var temp = await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false);
                    return Convert.ToInt32(temp);

                }
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> DeleteAsync(int PKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"delete from {DBName}  where  PromotionSingleTaskUsersId = @PKID;";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", PKID));

                    return (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false)) > 0;
                }
            }
        }
    }
}

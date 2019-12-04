using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 优惠券日志
    /// </summary>
    public class PromotionOprLogRepository : IPromotionOprLogRepository
    {

        private string DBName = "Tuhu_log..PromotionOprLog";
        private readonly IDbHelperFactory _factory;
        public PromotionOprLogRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> CreateAsync(PromotionOprLogEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   (PromotionPKID
                                    ,Author
                                    ,Operation
                                    ,Referer
                                    ,Channel
                                    ,OprDateTime
                                    ,GetRuleID
                                    ,UserID
                                    ,DeviceID
                                    ,OperationDetail
                                    ,CouponType
		                            )
                                VALUES
                                    (@PromotionPKID
                                    ,@Author
                                    ,@Operation
                                    ,@Referer
                                    ,@Channel
                                    ,getdate()
                                    ,@GetRuleID
                                    ,@UserID
                                    ,@DeviceID
                                    ,@OperationDetail
                                    ,@CouponType
		                            );"; 
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Tuhu_log", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionPKID", entity.PromotionPKID));
                    cmd.Parameters.Add(new SqlParameter("@Author", entity.Author));
                    cmd.Parameters.Add(new SqlParameter("@Operation", entity.Operation));
                    cmd.Parameters.Add(new SqlParameter("@Referer", entity.Referer));
                    cmd.Parameters.Add(new SqlParameter("@Channel", entity.Channel));
                    cmd.Parameters.Add(new SqlParameter("@GetRuleID", entity.GetRuleID));
                    cmd.Parameters.Add(new SqlParameter("@UserID", entity.UserID));
                    cmd.Parameters.Add(new SqlParameter("@DeviceID", entity.DeviceID));
                    cmd.Parameters.Add(new SqlParameter("@OperationDetail", entity.OperationDetail));
                    cmd.Parameters.Add(new SqlParameter("@CouponType", entity.CouponType));

                    var temp = await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false);
                    return Convert.ToInt32(temp);

                }
            }
        }

        
        public ValueTask<PromotionOprLogEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> UpdateAsync(PromotionOprLogEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

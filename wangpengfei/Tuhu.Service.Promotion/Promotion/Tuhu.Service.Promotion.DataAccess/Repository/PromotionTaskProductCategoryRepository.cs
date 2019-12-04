using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using System.Linq;
using Tuhu.Service.Promotion.DataAccess.IRepository;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    public class PromotionTaskProductCategoryRepository : IPromotionTaskProductCategoryRepository
    {

        private string DBName = "Activity..tbl_PromotionTaskProductCategory";
        private readonly IDbHelperFactory _factory;


        public PromotionTaskProductCategoryRepository(IDbHelperFactory factory) => _factory = factory;


        /// <summary>
        /// 根据任务id 获取配置的 类目信息
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionTaskProductCategoryEntity>> GetByPromotionTaskIdsync(int promotionTaskId, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT pkid
                                  ,PromotionTaskId
                                  ,ProductCategoryId
                              FROM {DBName} with (nolock)
                              where PromotionTaskId = @PromotionTaskId 
                        ";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionTaskProductCategoryEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }

        }
    }
}

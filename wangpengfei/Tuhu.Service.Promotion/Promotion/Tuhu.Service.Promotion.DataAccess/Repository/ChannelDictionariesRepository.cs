using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using System.Linq;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 渠道仓储
    /// </summary>
    public class ChannelDictionariesRepository : IChannelDictionariesRepository
    {

        private string DBName = "Gungnir..tbl_ChannelDictionaries ";
        private readonly IDbHelperFactory _factory;


        public ChannelDictionariesRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        ///  获取所有渠道
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<ChannelDictionariesEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT PKID
                                  ,ChannelType
                                  ,ChannelKey
                                  ,ChannelValue
                              FROM {DBName} with (nolock)
                        ";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var result = (await dbHelper.ExecuteSelectAsync<ChannelDictionariesEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }
    }
}

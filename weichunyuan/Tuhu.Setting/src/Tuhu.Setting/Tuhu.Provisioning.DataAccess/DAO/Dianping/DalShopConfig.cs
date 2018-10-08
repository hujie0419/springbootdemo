using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Dianping;

namespace Tuhu.Provisioning.DataAccess.DAO.Dianping
{
    public class DalShopConfig
    {
        public List<DianpingShopConfig> SelectDianpingShopConfig(SqlConnection conn, string dianpingId, int tuhuShopId, SqlTransaction transaction = null)
        {
            const string sql = @"SELECT 
                                    PKID, 
                                    DianpingId, 
                                    DianpingName, 
                                    DianpingShopName, 
                                    TuhuShopId, 
                                    GroupStatus,
                                    CreateDateTime,
                                    LastUpdateDateTime
                                FROM DianpingShopConfig WITH ( NOLOCK )
                                WHERE DianpingId = @DianpingId OR TuhuShopId = @TuhuShopId";
            return conn.Query<DianpingShopConfig>(sql, new
            {
                DianpingId = dianpingId,
                TuhuShopId = tuhuShopId
            }, commandType: CommandType.Text, transaction: transaction).ToList();
        }

        public int Insert(SqlConnection conn, DianpingShopConfig shopConfig, SqlTransaction transaction = null)
        {
            const string sql = @"INSERT INTO DianpingShopConfig (DianpingId, DianpingName, DianpingShopName, TuhuShopId, GroupStatus)
VALUES (@DianpingId, @DianpingName, @DianpingShopName, @TuhuShopId, @GroupStatus)";
            return conn.Execute(sql, shopConfig, commandType: CommandType.Text, transaction: transaction);
        }

        public int Update(SqlConnection conn, DianpingShopConfig shopConfig)
        {
            const string sql = @"UPDATE DianpingShopConfig
                                SET
                                  DianpingName       = @DianpingName,
                                  DianpingShopName   = @DianpingShopName,
                                  TuhuShopId         = @TuhuShopId,
                                  GroupStatus        = @GroupStatus,
                                  LastUpdateDateTime = getdate()
                                WHERE DianpingId = @DianpingId";
            return conn.Execute(sql, shopConfig, commandType: CommandType.Text);
        }

        public List<DianpingShopConfig> SelectShopConfigs(SqlConnection conn, int pageIndex, int pageSize,
            string dianpingId, string dianpingName, string dianpingShopName, string tuhuShopId, int groupStatus,
            int linkStatus)
        {
            const string sql = @"SELECT
                                  config.PKID,
                                  config.DianpingId,
                                  config.DianpingName,
                                  config.DianpingShopName,
                                  config.TuhuShopId,
                                  config.GroupStatus,
                                  config.CreateDateTime,
                                  config.LastUpdateDateTime
                                FROM DianpingShopConfig AS config WITH ( NOLOCK )
                                  LEFT JOIN DianpingShopSession AS session WITH ( NOLOCK ) ON config.TuhuShopId = session.TuhuShopId
                                WHERE (@DianpingId = '' OR config.DianpingId = @DianpingId) AND
                                      (@DianpingName = '' OR config.DianpingName = @DianpingName) AND
                                      config.DianpingShopName LIKE '%' + @DianpingShopName + '%' AND
                                      (@TuhuShopId = '' OR config.TuhuShopId = @TuhuShopId) AND
                                      (@GroupStatus = -1 OR (config.GroupStatus = @GroupStatus)) AND
                                      (@LinkStatus = -1 OR (@LinkStatus = 0 AND session.PKID IS NULL) OR
                                      (@LinkStatus = 1 AND session.PKID IS NOT NULL) OR
                                      (@LinkStatus = 2 AND session.PKID IS NOT NULL AND 
                                      (session.RefreshToken IS NULL OR session.RefreshToken = '')))
                                ORDER BY config.LastUpdateDateTime DESC
                                  OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
            return conn.Query<DianpingShopConfig>(sql, new
            {
                DianpingId = dianpingId,
                DianpingName = dianpingName,
                DianpingShopName = dianpingShopName,
                TuhuShopId = tuhuShopId,
                GroupStatus = groupStatus,
                LinkStatus = linkStatus,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public int SelectShopConfigsCount(SqlConnection conn,
            string dianpingId, string dianpingName, string dianpingShopName, string tuhuShopId, 
            int groupStatus, int linkStatus)
        {
            const string sql = @"SELECT COUNT(1)
                                FROM DianpingShopConfig AS config WITH ( NOLOCK )
                                  LEFT JOIN DianpingShopSession AS session WITH ( NOLOCK ) ON config.TuhuShopId = session.TuhuShopId
                                WHERE (@DianpingId = '' OR config.DianpingId = @DianpingId) AND
                                      (@DianpingName = '' OR config.DianpingName = @DianpingName) AND
                                      config.DianpingShopName LIKE '%' + @DianpingShopName + '%' AND
                                      (@TuhuShopId = '' OR config.TuhuShopId = @TuhuShopId) AND
                                      (@GroupStatus = -1 OR (config.GroupStatus = @GroupStatus)) AND
                                      (@LinkStatus = -1 OR (@LinkStatus = 0 AND session.PKID IS NULL) OR
                                      (@LinkStatus = 1 AND session.PKID IS NOT NULL) OR
                                      (@LinkStatus = 2 AND session.PKID IS NOT NULL AND 
                                      (session.RefreshToken IS NULL OR session.RefreshToken = '')))";
            return conn.ExecuteScalar<int>(sql, new
            {
                DianpingId = dianpingId,
                DianpingName = dianpingName,
                DianpingShopName = dianpingShopName,
                TuhuShopId = tuhuShopId,
                GroupStatus = groupStatus,
                LinkStatus = linkStatus
            }, commandType: CommandType.Text);
        }

        public int Delete(SqlConnection conn, string dianpingId)
        {
            const string sql = @"DELETE FROM DianpingShopConfig
                                WHERE DianpingId = @DianpingId";
            return conn.Execute(sql, new { DianpingId = dianpingId }, commandType: CommandType.Text);
        }

        public List<DianpingShopSession> SelectShopSessions(SqlConnection conn, List<int> shopIds)
        {
            const string sql = @"WITH ids AS (
                                    SELECT *
                                    FROM SplitString(@TuhuShopIds, ',', 1)
                                )
                                SELECT
                                    PKID,
                                    TuhuShopId,
                                    Session,
                                    RefreshToken,
                                    Expires,
                                    CreateDateTime,
                                    LastUpdateDateTime
                                FROM DianpingShopSession WITH ( NOLOCK )
                                    JOIN ids ON ids.Item = TuhuShopId";
            return conn.Query<DianpingShopSession>(sql, new { TuhuShopIds = string.Join(",", shopIds) },
                commandType: CommandType.Text).ToList();
        }
    }
}

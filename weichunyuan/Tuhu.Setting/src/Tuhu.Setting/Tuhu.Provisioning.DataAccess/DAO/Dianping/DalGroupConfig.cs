using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Dianping;

namespace Tuhu.Provisioning.DataAccess.DAO.Dianping
{
    public class DalGroupConfig
    {
        public DianpingGroupConfig SelectDianpingGroupConfig(SqlConnection conn, string dianpingGroupId, SqlTransaction transaction = null)
        {
            const string sql = @"SELECT
                                    PKID,
                                    DianpingGroupId,
                                    DianpingBrand,
                                    DianpingTuanName,
                                    TuhuProductId,
                                    TuhuProductStatus,
                                    CreateDateTime,
                                    LastUpdateDateTime
                                FROM DianpingGroupConfig WITH ( NOLOCK )
                                WHERE DianpingGroupId = @DianpingGroupId";
            return conn.QueryFirstOrDefault<DianpingGroupConfig>(sql, new
            {
                DianpingGroupId = dianpingGroupId
            }, commandType: CommandType.Text, transaction: transaction);
        }

        public int Insert(SqlConnection conn, DianpingGroupConfig groupConfig, SqlTransaction transaction = null)
        {
            const string sql = @"INSERT INTO DianpingGroupConfig (DianpingGroupId, DianpingBrand, DianpingTuanName, TuhuProductId, TuhuProductStatus)
                                VALUES (@DianpingGroupId, @DianpingBrand, @DianpingTuanName, @TuhuProductId, @TuhuProductStatus)";
            return conn.Execute(sql, groupConfig, commandType: CommandType.Text, transaction: transaction );
        }

        public int Update(SqlConnection conn, DianpingGroupConfig groupConfig)
        {
            const string sql = @"UPDATE DianpingGroupConfig
                                SET DianpingBrand    = @DianpingBrand,
                                  DianpingTuanName   = @DianpingTuanName,
                                  TuhuProductId      = @TuhuProductId,
                                  TuhuProductStatus  = @TuhuProductStatus,
                                  LastUpdateDateTime = getdate()
                                WHERE DianpingGroupId = @DianpingGroupId";
            return conn.Execute(sql, groupConfig, commandType: CommandType.Text);
        }

        public List<DianpingGroupConfig> SelectGroupConfigs(SqlConnection conn, int pageIndex, int pageSize, 
            string dianpingId, string dianpingBrand, string dianpingName, string tuhuProductId, int status)
        {
            const string sql = @"SELECT
                                    PKID,
                                    DianpingGroupId,
                                    DianpingBrand,
                                    DianpingTuanName,
                                    TuhuProductId,
                                    TuhuProductStatus,
                                    CreateDateTime,
                                    LastUpdateDateTime
                                FROM DianpingGroupConfig WITH ( NOLOCK )
                                WHERE (@DianpingGroupId = '' OR DianpingGroupId = @DianpingGroupId) AND
                                    (@DianpingBrand = '' OR DianpingBrand = @DianpingBrand) AND
                                    DianpingTuanName LIKE '%' + @DianpingTuanName + '%' AND
                                    (@TuhuProductId = '' OR TuhuProductId = @TuhuProductId) AND
                                    (@TuhuProductStatus = -1 OR (TuhuProductStatus = @TuhuProductStatus))
                                ORDER BY LastUpdateDateTime DESC
                                OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
            return conn.Query<DianpingGroupConfig>(sql, new {
                DianpingGroupId = dianpingId,
                DianpingBrand = dianpingBrand,
                DianpingTuanName = dianpingName,
                TuhuProductId = tuhuProductId,
                TuhuProductStatus = status,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public int SelectGroupConfigsCount(SqlConnection conn, string dianpingId, string dianpingBrand,
            string dianpingName, string tuhuProductId, int status)
        {
            const string sql = @"SELECT COUNT(1)
                                FROM DianpingGroupConfig WITH ( NOLOCK )
                                WHERE (@DianpingGroupId = '' OR DianpingGroupId = @DianpingGroupId) AND
                                    (@DianpingBrand = '' OR DianpingBrand = @DianpingBrand) AND
                                    DianpingTuanName LIKE '%' + @DianpingTuanName + '%' AND
                                    (@TuhuProductId = '' OR TuhuProductId = @TuhuProductId) AND
                                    (@TuhuProductStatus = -1 OR (TuhuProductStatus = @TuhuProductStatus))";
            return conn.ExecuteScalar<int>(sql, new
            {
                DianpingGroupId = dianpingId,
                DianpingBrand = dianpingBrand,
                DianpingTuanName = dianpingName,
                TuhuProductId = tuhuProductId,
                TuhuProductStatus = status,
            }, commandType: CommandType.Text);
        }

        public int Delete(SqlConnection conn, string groupId)
        {
            const string sql = @"DELETE FROM DianpingGroupConfig
                                WHERE DianpingGroupId = @DianpingGroupId";
            return conn.Execute(sql, new { DianpingGroupId = groupId }, commandType: CommandType.Text);
        }
    }
}

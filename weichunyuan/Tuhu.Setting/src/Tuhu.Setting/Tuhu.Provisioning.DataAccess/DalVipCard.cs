using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess
{
    public class DalVipCard
    {
        #region Get数据
        public static IEnumerable<VipCardDetailModel> GetVipCardSaleList(int pageNum, int pageSize, int clientId)
        {
            var wherecondition = clientId==0? "WHERE  @ClientId>=0" : "WHERE    ClientId = @ClientId";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                 string sql = $@"
			       SELECT  a.ActivityId ,
                    a.ActivityName ,
                    a.Url ,
                    a.CreateDateTime ,
                    a.LastUpdateDateTime ,
                    b.* 
            FROM    Configuration..VipCardSaleConfig AS a WITH ( NOLOCK )
                    JOIN Configuration..VipCardSaleConfigDetail AS b WITH ( NOLOCK ) ON a.Pkid = b.VipCardId
               {wherecondition} ANd Status=1
               ORDER BY b.Pkid";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[]
                    {
                       
                        new SqlParameter("@ClientId", clientId),
                        new SqlParameter("@PageSize", pageSize),
                        new SqlParameter("@PageIndex", pageNum)
                    }).ConvertTo<VipCardDetailModel>();
            }
        }

        public static IEnumerable<VipCardDetailModel> GetAllClients()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                const string sql = @"
			       					SELECT DISTINCT(ClientId),CLientName FROM Configuration..VipClientBatchConfig WITH ( NOLOCK) ";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<VipCardDetailModel>();
            }
        }
        public static IEnumerable<VipCardDetailModel> GetBatchesByClientId(int clientId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                const string sql = @"
			       					SELECT * FROM Configuration..VipClientBatchConfig WITH ( NOLOCK) where ClientId=@ClientId ";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new DbParameter[]
                {
                    new SqlParameter("@ClientId", clientId),
                }).ConvertTo<VipCardDetailModel>();
            }
        }
        public static int GetVipCardIdByActivityId(string activityId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                const string sql = @"
			       					SELECT Pkid FROM Configuration..VipCardSaleConfig WITH ( NOLOCK) where ActivityId=@ActivityId ";
                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new DbParameter[]
                {
                    new SqlParameter("@ActivityId", activityId),
                }));
            }
        }
        public static IEnumerable<VipCardDetailModel> GetVipCardDetailsByActivityId(string activityId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                const string sql = @"
			       				    SELECT  ActivityName,A.PKid,B.*
                                        FROM    Configuration..VipCardSaleConfig
                                                AS A WITH ( NOLOCK )
                                                JOIN Configuration..VipCardSaleConfigDetail
                                                AS B WITH ( NOLOCK ) ON A.Pkid = B.VipCardId
                                        WHERE   ActivityId = @ActivityId And Status=1 ";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new DbParameter[]
                {
                    new SqlParameter("@ActivityId", activityId),
                }).ConvertTo<VipCardDetailModel>();
            }
        }
        public static IEnumerable<VipCardDetailModel> GetVipCardDetailsByActivityId(int vipCardId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                const string sql = @"
			       					SELECT * FROM Configuration..VipCardSaleConfigDetail WITH ( NOLOCK) where VipCardId=@VipCardId And status=1";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new DbParameter[]
                {
                    new SqlParameter("@VipCardId", vipCardId),
                }).ConvertTo<VipCardDetailModel>();
            }
        }
        #endregion
        #region 新增操作
        public static int InsertVipCardModel(VipCardModel model, SqlDbHelper dbHelper)
        {
            const string sql = @"
			       					INSERT INTO Configuration..VipCardSaleConfig 
										(ActivityId,
										ActivityName,
										Url
										)
										VALUES(
										@ActivityId,
                                        @ActivityName,
                                        @Url
										) SELECT @@IDENTITY";
            return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new DbParameter[]
            {
                    new SqlParameter("@ActivityId", model.ActivityId),
                     new SqlParameter("@ActivityName", model.ActivityName),
                    new SqlParameter("@Url", model.Url),
            }));
        }

        public static bool InsertVipCardDetailsModel(List<VipCardDetailModel> details, int vipCardId, SqlDbHelper dbHelper)
        {
            var flag = true;
            const string sql = @"
									INSERT INTO Configuration..VipCardSaleConfigDetail
									(
									ClientId,
									VipCardId,
									ClientName,
									BatchId,
									CardName,
									CardValue,
									SalePrice,
									UseRange,
									StartDate,
									EndDate,
									Stock,
									SaleOutQuantity,
									Status
									)
									VALUES
									(
									@ClientId,
									@VipCardId,
									@ClientName,
									@BatchId,
									@CardName,
									@CardValue,
									@SalePrice,
									@UseRange,
									@StartDate,
									@EndDate,
									@Stock,
									0,
									1
									) ";
            foreach (var model in details)
            {
                var singleResult = dbHelper.ExecuteNonQuery(sql, CommandType.Text, new DbParameter[]
                {
                    new SqlParameter("@ClientId", model.ClientId),
                    new SqlParameter("@VipCardId", vipCardId),
                    new SqlParameter("@ClientName", model.ClientName),
                    new SqlParameter("@BatchId", model.BatchId),
                    new SqlParameter("@CardName", model.CardName),
                    new SqlParameter("@CardValue", model.CardValue),
                    new SqlParameter("@SalePrice", model.SalePrice),
                    new SqlParameter("@UseRange", model.UseRange),
                    new SqlParameter("@StartDate", model.StartDate),
                    new SqlParameter("@EndDate", model.EndDate),
                    new SqlParameter("@Stock", model.Stock),
                }) > 0;
                flag = flag && singleResult;
            }
            return flag;
        }
        #endregion
        #region 修改操作
        public static int UpdateVipCardModel(string activityName, string activityId, SqlDbHelper dbHelper)
        {
            const string sql = @"
                                    UPDATE  Configuration..VipCardSaleConfig
                                    SET     ActivityName = @ActivityName
                                    WHERE   ActivityId = @ActivityId";
            return Convert.ToInt32(dbHelper.ExecuteNonQuery(sql, CommandType.Text, new DbParameter[]
            {
                    new SqlParameter("@ActivityId", activityId),
                     new SqlParameter("@ActivityName", activityName)
            }));
        }
        public static int SetVipCardModelDetailStatus(int cardId, List<string> batchIds, SqlDbHelper dbHelper)
        {
            string sql = $@"UPDATE Configuration..VipCardSaleConfigDetail
							SET Status=0 WHERE VipCardId={cardId} AND BatchId NOT IN(N'{string.Join(",", batchIds)}')";
            return Convert.ToInt32(dbHelper.ExecuteNonQuery(sql, CommandType.Text));
        }

        public static bool UpdateVipCardModelDetails(int cardId, List<VipCardDetailModel> details, SqlDbHelper dbHelper)
        {
            var flag = true;
            string sql = $@"	MERGE INTO Configuration..VipCardSaleConfigDetail AS T
									USING(SELECT @BatchId AS batchId,@VipCardId AS cardId) AS S
									ON T.BatchId=s.batchId and t.VipCardId=s.cardId
									WHEN MATCHED
									THEN UPDATE SET T.Status=1
									WHEN NOT MATCHED
									THEN INSERT (ClientId,
									VipCardId,
									ClientName,
									BatchId,
									CardName,
									CardValue,
									SalePrice,
									UseRange,
									StartDate,
									EndDate,
									Stock,
									SaleOutQuantity,
									Status)
									VALUES
                                    (
                                        @ClientId,
                                        @VipCardId,
                                        @ClientName,
                                        @BatchId,
                                        @CardName,
                                        @CardValue,
                                        @SalePrice,
                                        @UseRange,
                                        @StartDate,
                                        @EndDate,
                                        @Stock,
                                        0,
                                        1
                                    );";
            foreach (var model in details)
            {
                var singleResult = dbHelper.ExecuteNonQuery(sql, CommandType.Text, new DbParameter[]
                {
                    new SqlParameter("@ClientId", model.ClientId),
                    new SqlParameter("@VipCardId", cardId),
                    new SqlParameter("@ClientName", model.ClientName),
                    new SqlParameter("@BatchId", model.BatchId),
                    new SqlParameter("@CardName", model.CardName),
                    new SqlParameter("@CardValue", model.CardValue),
                    new SqlParameter("@SalePrice", model.SalePrice),
                    new SqlParameter("@UseRange", model.UseRange),
                    new SqlParameter("@StartDate", model.StartDate),
                    new SqlParameter("@EndDate", model.EndDate),
                    new SqlParameter("@Stock", model.Stock),
                }) > 0;
                flag = flag && singleResult;
            }
            return flag;
        }
        #endregion
    }
}

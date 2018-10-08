using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.ShopSync;

namespace Tuhu.Provisioning.DataAccess.DAO.ShopSync
{
    public class ShopSyncDAL
    {
        public static Tuple<List<ThirdPartyShopSyncRecord>, int> SelectThirdPartyShopSyncRecord(string thirdParty, 
            int pageIndex, int pageSize, string syncStatus, int shopId, string simpleName, string fullName)
        {
            Tuple<List<ThirdPartyShopSyncRecord>, int> result = null;
            string sqlstr = @"
SELECT  @TotalCount = COUNT(1)
FROM    Tuhu_thirdparty..ThirdPartyShopSyncRecord AS A WITH ( NOLOCK )
WHERE   A.ThirdPartyName = @ThirdPartyName
        AND ( @ShopId <= 0
              OR A.TuhuShopId = @ShopId
            )
        AND ( @SyncStatus IS NULL
              OR @SyncStatus = ''
              OR A.SyncStatus = @SyncStatus
            )
        AND ( A.ShopSimpleName LIKE N'' + @SimpleName + '%'
              OR @SimpleName IS NULL
              OR @SimpleName = ''
            )
        AND ( A.ShopFullName LIKE N'' + @FullName + '%'
              OR @FullName IS NULL
              OR @FullName = ''
            );
SELECT  A.PKID ,
        A.ThirdPartyName ,
        A.ThirdPartyShopId ,
        A.TuhuShopId ,
        A.ShopFullName ,
        A.ShopSimpleName ,
        A.WorkTime ,
        A.Address ,
        A.ShopStatus ,
        A.ServiceStatus ,
        A.SyncStatus ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_thirdparty..ThirdPartyShopSyncRecord AS A WITH ( NOLOCK )
WHERE   A.ThirdPartyName = @ThirdPartyName
        AND ( @ShopId <= 0
              OR A.TuhuShopId = @ShopId
            )
        AND ( @SyncStatus IS NULL
              OR @SyncStatus = ''
              OR A.SyncStatus = @SyncStatus
            )
        AND ( A.ShopSimpleName LIKE N'' + @SimpleName + '%'
              OR @SimpleName IS NULL
              OR @SimpleName = ''
            )
        AND ( A.ShopFullName LIKE N'' + @FullName + '%'
              OR @FullName IS NULL
              OR @FullName = ''
            )
ORDER BY PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY";
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                using (var cmd = new SqlCommand(sqlstr))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@ThirdPartyName", thirdParty);
                    cmd.Parameters.AddWithValue("@SyncStatus", syncStatus);
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    cmd.Parameters.AddWithValue("@SimpleName", simpleName);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.Add(new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    var records = dbhelper.ExecuteDataTable(cmd).ConvertTo<ThirdPartyShopSyncRecord>().ToList();
                    var totalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                    result = new Tuple<List<ThirdPartyShopSyncRecord>, int>(records, totalCount);
                }
            }
            return result;
        }

        public static List<string> SelectShopSyncThirdParties()
        {
            var result = new List<string>();
            var sql = @"
SELECT DISTINCT
        A.ThirdPartyName
FROM    Tuhu_thirdparty..ThirdPartyShopSyncRecord AS A WITH ( NOLOCK )";
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var dt = dbhelper.ExecuteDataTable(cmd);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string thirdPartyName = row.IsNull("ThirdPartyName") ? string.Empty : row["ThirdPartyName"].ToString();
                            result.Add(thirdPartyName);
                        }
                    }
                }
            }
            return result;
        }
    }
}

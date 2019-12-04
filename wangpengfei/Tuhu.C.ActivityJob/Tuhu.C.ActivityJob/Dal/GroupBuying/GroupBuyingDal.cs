using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Models.GroupBuying;

namespace Tuhu.C.ActivityJob.Dal.GroupBuying
{
    public class GroupBuyingDal
    {
        public static int SelectProductCount()
        {
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Configuration..GroupBuyingProductConfig WITH (NOLOCK);"))
            {
                cmd.CommandType = CommandType.Text;
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        public static List<GroupBuyingProduct> SelectProducts(int pageSize, ref int maxPkid)
        {
            using (var cmd = new SqlCommand($@"
                SELECT TOP {pageSize}
                    P.PKID,
                    P.PID,
                    P.ProductGroupId,
                    FLOOR(P.TotalStockCount / G.MemberCount) AS TotalPGroupCount
                FROM Configuration..GroupBuyingProductConfig AS P WITH (NOLOCK)
                    JOIN Configuration..GroupBuyingProductGroupConfig AS G WITH (NOLOCK)
                        ON P.ProductGroupId = G.ProductGroupId
                WHERE P.PKID > {maxPkid}
                ORDER BY P.PKID;"))
            {
                cmd.CommandType = CommandType.Text;
                var dt = DbHelper.ExecuteQuery(cmd, _ => _);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var result = dt.AsEnumerable().Select(r => new GroupBuyingProduct
                    {
                        Pid = r["PID"]?.ToString(),
                        ProductGroupId = r["ProductGroupId"]?.ToString(),
                        TotalPGroupCount = Convert.ToInt32(r["TotalPGroupCount"]?.ToString() ?? "0"),
                        Pkid = Convert.ToInt32(r["PKID"]?.ToString() ?? "0")
                    });

                    maxPkid = result.Max(x => x.Pkid);
                    return result.ToList();
                }
                return new List<GroupBuyingProduct>();
            }
        }

        public static int GetCurrentGroupCount(string productGroupId, string pid)
        {
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Activity..tbl_GroupBuyingInfo AS G WITH (NOLOCK)
                    JOIN Activity..tbl_GroupBuyingUserInfo AS U WITH (NOLOCK)
                        ON G.GroupId = U.GroupId
                WHERE G.ProductGroupId = @ProductGroupId
                      AND U.PID = @PID
                      AND G.OwnerId = U.UserId
                      AND G.GroupStatus < 3
                      AND U.UserStatus < 3;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@PID", pid);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }

        public static bool UpdateGroupCount(GroupBuyingProduct product)
        {
            using (var cmd = new SqlCommand($@"
                UPDATE Configuration..GroupBuyingProductConfig WITH (ROWLOCK)
                SET TotalPGroupCount = @TotalPGroupCount,
                    CurrentPGroupCount = @CurrentPGroupCount
                WHERE ProductGroupId = @ProductGroupId
                      AND PID = @PID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@TotalPGroupCount", product.TotalPGroupCount);
                cmd.Parameters.AddWithValue("@CurrentPGroupCount", product.CurrentPGroupCount);
                cmd.Parameters.AddWithValue("@ProductGroupId", product.ProductGroupId);
                cmd.Parameters.AddWithValue("@PID", product.Pid);
                return Convert.ToInt32(DbHelper.ExecuteNonQuery(cmd)) > 0;
            }
        }

        public static int SelectProductGroupCount()
        {
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Configuration..GroupBuyingProductGroupConfig WITH (NOLOCK)
                WHERE IsDelete = 0;"))
            {
                cmd.CommandType = CommandType.Text;
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        public static List<GroupBuyingProductGroup> SelectProductGroups(int pageSize, ref int maxPkid)
        {
            using (var cmd = new SqlCommand($@"
                SELECT TOP {pageSize}
                    PKID,
                    ProductGroupId
                FROM Configuration..GroupBuyingProductGroupConfig WITH (NOLOCK)
                WHERE PKID > {maxPkid}
                AND IsDelete = 0
                ORDER BY PKID;"))
            {
                cmd.CommandType = CommandType.Text;
                var dt = DbHelper.ExecuteQuery(cmd, _ => _);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var result = dt.AsEnumerable().Select(r => new GroupBuyingProductGroup
                    {
                        ProductGroupId = r["ProductGroupId"]?.ToString(),
                        Pkid = Convert.ToInt32(r["PKID"]?.ToString() ?? "0")
                    });

                    maxPkid = result.Max(x => x.Pkid);
                    return result.ToList();
                }
                return new List<GroupBuyingProductGroup>();
            }
        }

        public static List<GroupBuyingProduct> SelectGroupBuyingProducts(string productGroupId)
        {
            using (var cmd = new SqlCommand(@"
                SELECT PID
                FROM Configuration..GroupBuyingProductConfig WITH (NOLOCK)
                WHERE ProductGroupId = @ProductGroupId
                        AND IsDelete = 0;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                return DbHelper.ExecuteSelect<GroupBuyingProduct>(cmd)?.ToList()
                    ?? new List<GroupBuyingProduct>();
            }
        }
    }
}

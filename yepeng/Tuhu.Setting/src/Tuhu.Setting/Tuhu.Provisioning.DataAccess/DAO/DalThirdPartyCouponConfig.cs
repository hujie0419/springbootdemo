using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalThirdPartyCouponConfig
    {
        public static IEnumerable<ThirdPartyCouponConfigModel> SelectThirdPartyCouponConfigModels(int pageNum, int pageSize)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                var cmd = new SqlCommand(@"SELECT  *,COUNT(1) OVER () as TotalCount FROM    Configuration.dbo.ThirdPartyCouponConfig WITH(NOLOCK)
                                            ORDER BY CreateTime
                                            OFFSET (@PageNumber - 1) * @PageSize ROW
				                                        FETCH NEXT @PageSize ROW ONLY;");
                {
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@PageNumber", pageNum);
                }
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ThirdPartyCouponConfigModel>();
            }
        }
        public static int SelectThirdPartyCouponConfigsByChannelAndPatch(string thirdPartyChannel,string thirdPartyCouponPatch)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                var cmd =new SqlCommand(@"SELECT  COUNT(1)  FROM    Configuration.dbo.ThirdPartyCouponConfig WITH(NOLOCK) where ThirdPartyChannel=@ThirdPartyChannel and ThirdPartyCouponPatch=@ThirdPartyCouponPatch");
                cmd.Parameters.AddWithValue("@ThirdPartyChannel", thirdPartyChannel);
                cmd.Parameters.AddWithValue("@ThirdPartyCouponPatch", thirdPartyCouponPatch);

                return Convert.ToInt32(dbhelper.ExecuteScalar(cmd)) ;
            }
        }
        public static ThirdPartyCouponConfigModel SelectThirdPartyCouponConfigModelById(SqlConnection connection, int pkid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", pkid);
            return connection.Query<ThirdPartyCouponConfigModel>("SELECT  *FROM    Configuration.dbo.ThirdPartyCouponConfig WITH(NOLOCK) where PKID=@ID ", parameters).FirstOrDefault();
        }

        public static bool DeleteThirdPartyCouponConfig(SqlConnection connection, int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            return connection.Execute("DELETE FROM  Configuration.dbo.ThirdPartyCouponConfig WHERE PKID=@ID", parameters) > 0;
        }

        public static int UpdateThirdPartyCouponConfig(SqlConnection connection,ThirdPartyCouponConfigModel model)
        {
            string sql = @"UPDATE  Configuration.dbo.ThirdPartyCouponConfig SET  
		          ThirdPartyCouponPatch=N'{0}',
		          PatchDescription=N'{1}',
		          ThirdPartyChannel=N'{2}',
		          CouponGetRuleId=N'{3}',
		          CouponName=N'{4}',
		          CouponDescription=N'{5}',
		          CouponDiscount='{6}',
		          CouponMinMoney= '{7}',
		          CouponEffectDuration=N'{8}',
		          UpdateTime=GETDATE()
				  WHERE PKID={9}
		       ";
            sql = string.Format(sql, model.ThirdPartyCouponPatch, model.PatchDescription, model.ThirdPartyChannel, model.CouponGetRuleId, model.CouponName, model.CouponDescription,
                model.CouponDiscount, model.CouponMinMoney, model.CouponEffectDuration, model.PKID);
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql);
        
        }

        public static int InsertThirdPartyCouponConfig(SqlConnection connection,ThirdPartyCouponConfigModel model)
        {
            string sql = @"Insert  Configuration.dbo.ThirdPartyCouponConfig values(
                            N'{0}',
                            N'{1}',
                            N'{2}',
                            N'{3}',
                            N'{4}',
                            N'{5}',
                            N'{6}',
                            N'{7}',
                            N'{8}',
                            GETDATE(),
                            GETDATE());SELECT @@IDENTITY";
            sql = string.Format(sql, model.ThirdPartyCouponPatch, model.PatchDescription, model.ThirdPartyChannel, model.CouponGetRuleId, model.CouponName, model.CouponDescription,
                            model.CouponDiscount, model.CouponMinMoney, model.CouponEffectDuration);
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql));
        //}
        }
        public static GetPCodeModel SelectGetCouponRulesByGetRuleId(SqlConnection connection, Guid getruleId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@GetRuleGuid", getruleId);
            return connection.Query<GetPCodeModel>("SELECT  *FROM    Activity..tbl_GetCouponRules WITH(NOLOCK) where GetRuleGuid=@GetRuleGuid ", parameters).FirstOrDefault();

        }
    }
}

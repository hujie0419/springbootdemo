using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models;

namespace Tuhu.Service.Activity.DataAccess.OARedEnvelope
{
    /// <summary>
    ///     公众号领红包 - 分享
    /// </summary>
    public class DalOARedEnvelopeShare
    {

        /// <summary>
        ///     添加分享数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<long> InsertOARedEnvelopeShareAsync(OARedEnvelopeShareModel model)
        {
            var sql = @"
                     IF NOT EXISTS (SELECT * FROM Activity..tbl_OARedEnvelopeShare WHERE ShareingOpenId = @ShareingOpenId and SharedOpenId = @SharedOpenId)
                        insert into Activity.[dbo].[tbl_OARedEnvelopeShare]
                        ([ShareingUserId]
                           ,[ShareingOpenId]
                           ,[SharedUserId]
                           ,[SharedOpenId]
                           ,[OfficialAccountType]
                           ,[CreateDatetime]
                           ,[LastUpdateDateTime]
                        )
                        values(
                         @ShareingUserId
                        ,@ShareingOpenId
                        ,@SharedUserId
                        ,@SharedOpenId
                        ,@OfficialAccountType
                        ,getdate()
                        ,getdate()
                        );
                   SELECT SCOPE_IDENTITY();
                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ShareingUserId", model.ShareingUserId);
                cmd.AddParameter("@ShareingOpenId", model.ShareingOpenId);
                cmd.AddParameter("@SharedUserId", model.SharedUserId);
                cmd.AddParameter("@SharedOpenId", model.SharedOpenId);
                cmd.AddParameter("@OfficialAccountType", model.OfficialAccountType);


                var result = await DbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models;

namespace Tuhu.Service.Activity.DataAccess.OARedEnvelope
{
    /// <summary>
    ///     公众号领红包 - 明细 数据类
    /// </summary>
    public class DalOARedEnvelopeDetail
    {
        /// <summary>
        ///     获取公众号领红包 - 明细数据
        /// </summary>
        /// <returns></returns>
        public static async Task<List<OARedEnvelopeDetailModel>> GetOARedEnvelopeDetailAsync(
            Guid userId
            , string openId
            , int officialAccountType
            , string drivingLicense
            )

        {

            var baseSql = @"

                        select [PKID]
                              ,[UserId]
                              ,[NickName]
                              ,[OpenId]
                              ,[ReferrerUserId]
                              ,[GetMoney]
                              ,[GetDate]
                              ,[OfficialAccountType]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,DrivingLicense
                         from [Activity].[dbo].[tbl_OARedEnvelopeDetail] with (nolock)
                         where UserId = @UserId and OfficialAccountType = @OfficialAccountType and IsDeleted = 0

";

            if (!string.IsNullOrWhiteSpace(openId))
            {
                baseSql = baseSql + @"

                        UNION ALL 
                         select [PKID]
                              ,[UserId]
                              ,[NickName]
                              ,[OpenId]
                              ,[ReferrerUserId]
                              ,[GetMoney]
                              ,[GetDate]
                              ,[OfficialAccountType]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,DrivingLicense
                         from [Activity].[dbo].[tbl_OARedEnvelopeDetail] with (nolock)
                         where OpenId = @OpenId and OfficialAccountType = @OfficialAccountType and IsDeleted = 0

";
            }

            if (!string.IsNullOrWhiteSpace(drivingLicense))
            {
                baseSql = baseSql + @"

                        UNION ALL 
                         select [PKID]
                              ,[UserId]
                              ,[NickName]
                              ,[OpenId]
                              ,[ReferrerUserId]
                              ,[GetMoney]
                              ,[GetDate]
                              ,[OfficialAccountType]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,DrivingLicense
                         from [Activity].[dbo].[tbl_OARedEnvelopeDetail] with (nolock)
                         where DrivingLicense = @DrivingLicense and OfficialAccountType = @OfficialAccountType and IsDeleted = 0
";
            }


            using (var cmd = new SqlCommand(baseSql))
            {
                cmd.AddParameter("@UserId", userId);
                cmd.AddParameter("@OpenId", openId ?? "");
                cmd.AddParameter("@OfficialAccountType", officialAccountType);
                cmd.AddParameter("@DrivingLicense", drivingLicense ?? "");



                return (await DbHelper.ExecuteSelectAsync<OARedEnvelopeDetailModel>(false, cmd)).ToList();
            }
        }

        /// <summary>
        ///     新增公众号领红包 - 明细数据
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertOARedEnvelopeDetailAsync(BaseDbHelper dbHelper,
            OARedEnvelopeDetailModel redEnvelopeDetailModel)
        {
            var sql = @"
    
             INSERT INTO Activity.[dbo].[tbl_OARedEnvelopeDetail]
                   ([UserId]
                   ,[NickName]
                   ,[WXHeadImgUrl]
                   ,[OpenId]
                   ,[ReferrerUserId]
                   ,[GetMoney]
                   ,[GetDate]
                   ,[OfficialAccountType]
                   ,DrivingLicense
                   ,IsDeleted
                 )
             VALUES
                   (
                    @UserId
                   ,@NickName
                   ,@WXHeadPicUrl
                   ,@OpenId
                   ,@ReferrerUserId
                   ,@GetMoney
                   ,@GETDATE
                   ,@OfficialAccountType
                   ,@DrivingLicense
                   ,0
                 );
                SELECT SCOPE_IDENTITY();
                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", redEnvelopeDetailModel.UserId);
                cmd.AddParameter("@NickName", redEnvelopeDetailModel.NickName ?? "");
                cmd.AddParameter("@WXHeadPicUrl", redEnvelopeDetailModel.WXHeadImgUrl ?? "");

                cmd.AddParameter("@OpenId", redEnvelopeDetailModel.OpenId ?? "");
                cmd.AddParameter("@ReferrerUserId", redEnvelopeDetailModel.ReferrerUserId);

                cmd.AddParameter("@GetMoney", redEnvelopeDetailModel.GetMoney);
                cmd.AddParameter("@GetDate", DateTime.Now.Date);
                cmd.AddParameter("@OfficialAccountType", redEnvelopeDetailModel.OfficialAccountType);
                cmd.AddParameter("@DrivingLicense", redEnvelopeDetailModel.DrivingLicense ?? "");


                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     禁用掉数据
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<bool> DisableOARedEnvelopeDetailAsync(long pkid)
        {
            var sql = @"update  Activity.[dbo].[tbl_OARedEnvelopeDetail]
                        set IsDeleted = 1
                        where PKID =  @PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", pkid);
                return await DbHelper.ExecuteNonQueryAsync(cmd) > 1;
            }
        }


        /// <summary>
        ///     删除掉数据
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteOARedEnvelopeDetailAsync(Guid userId)
        {
            var sql = @"delete  Activity.[dbo].[tbl_OARedEnvelopeDetail]
                        where userid =  @userid";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@userId", userId);
                return await DbHelper.ExecuteNonQueryAsync(cmd) > 1;
            }
        }


        /// <summary>
        ///     获取数量
        /// </summary>
        /// <param name="count"></param>
        /// <param name="officialAccountType"></param>
        /// <returns></returns>
        public static async Task<List<OARedEnvelopeDetailModel>> SearchTopOARedEnvelopeDetailAsync(int count,
            int officialAccountType)
        {
            var sql = @"SELECT top " + count + @" [PKID]
                              ,[UserId]
                              ,[NickName]
                              ,[WXHeadImgUrl]
                              ,[OpenId]
                              ,[ReferrerUserId]
                              ,[GetMoney]
                              ,[GetDate]
                              ,[OfficialAccountType]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,[IsDeleted]
                              ,[DrivingLicense]
                          FROM Activity.[dbo].[tbl_OARedEnvelopeDetail] with (nolock)
                          where officialAccountType = @officialAccountType and IsDeleted = 0
                          order by pkid desc 
                        ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@officialAccountType", officialAccountType);
                return (await DbHelper.ExecuteSelectAsync<OARedEnvelopeDetailModel>(true, cmd)).ToList();
            }
        }
    }
}

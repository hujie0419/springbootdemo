using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models;

namespace Tuhu.Service.Activity.DataAccess.OARedEnvelope
{
    /// <summary>
    ///     公众号领红包 - 设置 数据类
    /// </summary>
    public class DalOARedEnvelopeSetting
    {
        /// <summary>
        ///     获取公众号领红包设置
        /// </summary>
        /// <returns></returns>
        public static async Task<OARedEnvelopeSettingModel> GetOARedEnvelopeSettingAsync(int officialAccountType = 1)
        {
            var sql = @" select [PKID]
                                  ,[ConditionPrice]
                                  ,[ConditionPriceFlag]
                                  ,[ConditionCarModelFlag]
                                  ,[DayMaxMoney]
                                  ,[AvgMoney]
                                  ,[ActivityRuleText]
                                  ,[FailTipText]
                                  ,[QRCodeUrl]
                                  ,[QRCodeTipText]
                                  ,[ShareTitleText]
                                  ,[ShareUrl]
                                  ,[SharePictureUrl]
                                  ,[ShareText]
                                  ,[OfficialAccountType]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,PerMaxMoney
                                  ,PerMinMoney
                                  ,OpenIdLegalDate
                                  ,Channel
                            
                         from [Configuration].[dbo].[tbl_OARedEnvelopeSetting] with (nolock)
                         where officialAccountType = @officialAccountType
                         order by pkid asc
                        ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@officialAccountType", officialAccountType);
                return await DbHelper.ExecuteFetchAsync<OARedEnvelopeSettingModel>(true, cmd);
            }
        }


        /// <summary>
        ///     保存公众号领红包设置
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static async Task<long> InsertOARedEnvelopeSettingAsync(BaseDbHelper dbHelper,
            OARedEnvelopeSettingModel setting)
        {
            var sql = @"INSERT INTO Configuration.[dbo].[tbl_OARedEnvelopeSetting]
                           ([ConditionPrice]
                           ,[ConditionPriceFlag]
                           ,[ConditionCarModelFlag]
                           ,[DayMaxMoney]
                           ,[AvgMoney]
                           ,[ActivityRuleText]
                           ,[FailTipText]
                           ,[QRCodeUrl]
                           ,[QRCodeTipText]
                           ,[ShareTitleText]
                           ,[ShareUrl]
                           ,[SharePictureUrl]
                           ,[ShareText]
                           ,[OfficialAccountType]
                           ,[CreateDatetime]
                           ,[LastUpdateDateTime]
                           ,[CreateBy]
                           ,[LastUpdateBy]
                           ,PerMaxMoney
                           ,PerMinMoney
                           ,OpenIdLegalDate
                           ,Channel

                            )
                        VALUES
                            (
                                @ConditionPrice
                               ,@ConditionPriceFlag
                               ,@ConditionCarModelFlag
                               ,@DayMaxMoney
                               ,@AvgMoney
                               ,@ActivityRuleText
                               ,@FailTipText
                               ,@QRCodeUrl
                               ,@QRCodeTipText
                               ,@ShareTitleText
                               ,@ShareUrl
                               ,@SharePictureUrl
                               ,@ShareText
                               ,@OfficialAccountType
                               ,@CreateDatetime
                               ,@LastUpdateDateTime
                               ,@CreateBy
                               ,@LastUpdateBy
                               ,@PerMaxMoney
                               ,@PerMinMoney
                               ,@OpenIdLegalDate
                               ,@Channel
                            );
                      SELECT SCOPE_IDENTITY();

            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ConditionPrice", setting.ConditionPrice);
                cmd.AddParameter("@ConditionPriceFlag", setting.ConditionPriceFlag);
                cmd.AddParameter("@ConditionCarModelFlag", setting.ConditionCarModelFlag);
                cmd.AddParameter("@DayMaxMoney", setting.DayMaxMoney);
                cmd.AddParameter("@AvgMoney", setting.AvgMoney);
                cmd.AddParameter("@ActivityRuleText", setting.ActivityRuleText ?? "");
                cmd.AddParameter("@FailTipText", setting.FailTipText ?? "");
                cmd.AddParameter("@QRCodeUrl", setting.QRCodeUrl ?? "");
                cmd.AddParameter("@QRCodeTipText", setting.QRCodeTipText ?? "");
                cmd.AddParameter("@ShareTitleText", setting.ShareTitleText ?? "");
                cmd.AddParameter("@ShareUrl", setting.ShareUrl ?? "");
                cmd.AddParameter("@SharePictureUrl", setting.SharePictureUrl ?? "");
                cmd.AddParameter("@ShareText", setting.ShareText ?? "");
                cmd.AddParameter("@OfficialAccountType", setting.OfficialAccountType);
                cmd.AddParameter("@CreateDatetime", DateTime.Now);
                cmd.AddParameter("@LastUpdateDateTime", DateTime.Now);
                cmd.AddParameter("@CreateBy", setting.CreateBy ?? "");
                cmd.AddParameter("@LastUpdateBy", setting.LastUpdateBy ?? "");
                cmd.AddParameter("@PerMaxMoney", setting.PerMaxMoney);
                cmd.AddParameter("@PerMinMoney", setting.PerMinMoney);
                cmd.AddParameter("@OpenIdLegalDate", ((object)setting.OpenIdLegalDate) ?? DBNull.Value);
                cmd.AddParameter("@Channel", setting.Channel ?? "");

                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     更新公众号领红包设置
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateOARedEnvelopeSettingAsync(BaseDbHelper dbHelper,
            OARedEnvelopeSettingModel setting)
        {
            var sql = @"

                        UPDATE Configuration.[dbo].[tbl_OARedEnvelopeSetting]
                               SET [ConditionPrice] = @ConditionPrice
                                  ,[ConditionPriceFlag] = @ConditionPriceFlag
                                  ,[ConditionCarModelFlag] = @ConditionCarModelFlag
                                  ,[DayMaxMoney] = @DayMaxMoney
                                  ,[AvgMoney] = @AvgMoney
                                  ,[ActivityRuleText] = @ActivityRuleText
                                  ,[FailTipText] = @FailTipText
                                  ,[QRCodeUrl] = @QRCodeUrl
                                  ,[QRCodeTipText] = @QRCodeTipText
                                  ,[ShareTitleText] = @ShareTitleText
                                  ,[ShareUrl] = @ShareUrl
                                  ,[SharePictureUrl] = @SharePictureUrl
                                  ,[ShareText] = @ShareText
                                  ,[OfficialAccountType] = @OfficialAccountType
                                  ,[LastUpdateDateTime] = getdate()
                                  ,[LastUpdateBy] = @LastUpdateBy
                                  ,PerMaxMoney = @PerMaxMoney
                                  ,PerMinMoney = @PerMinMoney
                                  ,OpenIdLegalDate = @OpenIdLegalDate
                                  ,Channel = @Channel

                             WHERE  pkid = @pkid
        
                    ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@pkid", setting.PKID);

                cmd.AddParameter("@ConditionPrice", setting.ConditionPrice);
                cmd.AddParameter("@ConditionPriceFlag", setting.ConditionPriceFlag);
                cmd.AddParameter("@ConditionCarModelFlag", setting.ConditionCarModelFlag);
                cmd.AddParameter("@DayMaxMoney", setting.DayMaxMoney);
                cmd.AddParameter("@AvgMoney", setting.AvgMoney);
                cmd.AddParameter("@ActivityRuleText", setting.ActivityRuleText ?? "");
                cmd.AddParameter("@FailTipText", setting.FailTipText ?? "");
                cmd.AddParameter("@QRCodeUrl", setting.QRCodeUrl ?? "");
                cmd.AddParameter("@QRCodeTipText", setting.QRCodeTipText ?? "");
                cmd.AddParameter("@ShareTitleText", setting.ShareTitleText ?? "");
                cmd.AddParameter("@ShareUrl", setting.ShareUrl ?? "");
                cmd.AddParameter("@SharePictureUrl", setting.SharePictureUrl ?? "");
                cmd.AddParameter("@ShareText", setting.ShareText ?? "");
                cmd.AddParameter("@OfficialAccountType", setting.OfficialAccountType);
                cmd.AddParameter("@LastUpdateBy", setting.LastUpdateBy ?? "");
                cmd.AddParameter("@PerMaxMoney", setting.PerMaxMoney);
                cmd.AddParameter("@PerMinMoney", setting.PerMinMoney);
                cmd.AddParameter("@OpenIdLegalDate", ((object)setting.OpenIdLegalDate) ?? DBNull.Value);
                cmd.AddParameter("@Channel", setting.Channel ?? "");
                    
                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}

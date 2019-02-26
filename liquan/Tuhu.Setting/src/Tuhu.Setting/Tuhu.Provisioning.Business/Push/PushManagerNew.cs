using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Newtonsoft.Json;
using Tuhu.Component.Framework.Extension;
using Tuhu.Service.Push.Models.WeiXinPush;
using System.Configuration;
using System.Data.SqlClient;
using System.Data; 
using Tuhu.Service.PushApi.Models.Push;
using Tuhu.Service.PushApi.Models.MessageBox; 

namespace Tuhu.Provisioning.Business.Push
{
    public class PushManagerNew
    {
        public static async Task<List<PushTemplate>> SelectPushTemplatesAsync()
        {
            try
            {
                using (var client = new Tuhu.Service.Push.TemplatePushClient())
                {
                    var result = await client.SelectTemplateAsync();
                    result.ThrowIfException(true);
                    var datas = result.Result; 
                    return datas.Cast<List<PushTemplate>>();
                }
            }
            catch (System.Exception ex)
            {
                throw; 
            }
        }
        public static async Task<TemplateQueryResult> SelectPushTemplatesAsync(TemplateQuery query)
        {

            using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
            {
                var result = await client.QueryTemplatesAsync(query.Cast<Tuhu.Service.PushApi.Models.Push.TemplateQuery>());
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas.Cast<TemplateQueryResult>();
            }
        }
        //public static QueryResult SelectPushTemplates(TemplateQuery query)
        //{
        //    using (var client = new Tuhu.Service.Push.TemplatePushClient())
        //    {
        //        var result = client.QueryTemplates(query);
        //        result.ThrowIfException(true);
        //        var datas = result.Result;
        //        return datas;
        //    }
        //}
        public static async Task<List<PushTemplate>> SelectPushTemplatesByBatchIDAsync(int Batchid)
        {
            string sql = $"select * from Tuhu_notification..tbl_PushTemplate  WITH(NOLOCK)  where  BatchID = {Batchid}";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<PushTemplate>()?.ToList();
            }
        }

        public static async Task<IEnumerable<WxTemplateInfo>> SelectAllWxTemplateAsync()
        {
            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
            {
                var result = await client.SelectAllWxPushTemplateAsync();
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas;
            }
        }

        public static async Task<IEnumerable<WxTemplateInfo>> SelectAllBaiduTemplateAsync()
        {
            using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
            {
                var result = await client.GetBaiduTemplateListAsync();
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas.Select(x => new WxTemplateInfo()
                {
                    template_id = x.TemplateId,
                    content = x.Content,
                    example = x.Example,
                    title = x.Title
                }
                );
            }
        }

        public static async Task<IEnumerable<WxTemplateInfo>> SelectAllWxAppTemplatesAsync(int platform)
        {
            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
            {
                var result = await client.SelectAllWxAppTemplatesAsync(platform);
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas;
            }
        }
        public static async Task<IEnumerable<WxConfig>> SelectAllWxAppConfigAsync()
        {
            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
            {
                var result = await client.SelectWxConfigsAsync();
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas?.ToList();
                //return datas?.Where(x => x.Type == "WX_APP");
            }
        }
        public static async Task<bool> PushByAliasAsync(IEnumerable<string> target, PushTemplate template)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.PushByAliasAsync(target, template.Cast<Tuhu.Service.Push.Models.Push.PushTemplate>());
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        public static async Task<bool> PushByRegidAsync(IEnumerable<string> target, PushTemplate template)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.PushByRegidAsync(target, template.Cast<Tuhu.Service.Push.Models.Push.PushTemplate>());
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        public static async Task<PushTemplate> SelectTemplateByPKIDAsync(int pkid)
        {
            using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
            {
                var result = await client.GetTemplatesByPkIdAsync(pkid);
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        public static async Task<IEnumerable<CalculateMessageInfo>> SelectTemplatePushRatesAsync(IEnumerable<int> templateids)
        {
            string sql =
                $"SELECT  * FROM Tuhu_notification..tbl_MessageStatistics WITH ( NOLOCK) WHERE Templateid IN ({string.Join(",", templateids)})";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<CalculateMessageInfo>();
            }
        }
        public static async Task<int?> SelectTemplateTagByBatchIdAsync(int batchid)
        {
            string sql = $@"SELECT  TemplateTag
        FROM    Tuhu_notification..tbl_PushTemplate WITH ( NOLOCK )
        WHERE   BatchID = {batchid};";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                var result = await dbHelper.ExecuteDataTableAsync(cmd);
                if (result != null && result.Rows.Count > 0)
                {
                    var data = result.AsEnumerable().Select(x => x[0]?.ToString())?.Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                    int temp;
                    if (int.TryParse(data, out temp))
                    {
                        return temp;
                    }
                }
                return null;
            }
        }

        public static async Task<string> UpdateTemplateTagByBatchIdAsync(int batchid, int templatetag)
        {
            string sql = $@"update  Tuhu_notification..tbl_PushTemplate WITH ( rowlock ) set templatetag={templatetag} WHERE   BatchID = {batchid};";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                var result = await dbHelper.ExecuteDataTableAsync(cmd);
                if (result != null && result.Rows.Count > 0)
                {
                    return result.AsEnumerable().Select(x => x[0]?.ToString())?.FirstOrDefault() ?? "";
                }
                return "";
            }
        }

        public static async Task<string> UpdateTemplateTypeByBatchIdAsync(int batchid, TemplateType templatetype)
        {
            string sql = $@"update  Tuhu_notification..tbl_PushTemplate WITH ( rowlock ) set TemplateType={(int)templatetype} WHERE   BatchID = {batchid};";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                var result = await dbHelper.ExecuteDataTableAsync(cmd);
                if (result != null && result.Rows.Count > 0)
                {
                    return result.AsEnumerable().Select(x => x[0]?.ToString())?.FirstOrDefault() ?? "";
                }
                return "";
            }
        }

        public static async Task<string> UpdateTemplatePriorityByBatchIdAsync(int batchid, int? wxPriority, int? appPriority)
        {
            if (!wxPriority.HasValue && !appPriority.HasValue)
            {
                return "";
            }

            var sbSql = new StringBuilder();
            if (wxPriority.HasValue)
            {
                sbSql.Append($"[WxPushPriority] = {(wxPriority.Value > 0 ? 1 : 0)} {(appPriority.HasValue ? "," : "")}");
            }
            if (appPriority.HasValue)
            {
                sbSql.Append($"[AppPushPriority] = {(appPriority.Value > 0 ? 1 : 0)}");
            }
            string sql = $@"UPDATE  Tuhu_notification..tbl_PushTemplate WITH ( ROWLOCK  ) 
SET {sbSql}
WHERE [BatchID] = {batchid}";

            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                    return result.ToString();
                }
            }
        }

        public static async Task<IEnumerable<PushTemplateModifyLog>> SelectTemplateModifyLogs(int templateid)
        {
            string sql = $"SELECT * FROM Tuhu_log..PushTemplateModifyLog WITH ( NOLOCK) WHERE templateid={templateid} order by pkid";

            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<PushTemplateModifyLog>().ToList();
            }
        }

        public static async Task<IEnumerable<int>> SelectTemplateIdsByBatchId(int batchId)
        {
            string sql = $"select PKID from Tuhu_notification..tbl_PushTemplate  WITH(NOLOCK)  where  BatchID = {batchId}";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    return (await dbHelper.ExecuteDataTableAsync(cmd)).ToList<int>();
                }
            }
        }

        public static async Task<int> UpdateTemplateBatchIDAsync(int templateid, int batchid)
        {
            using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
            {
                var result = await client.UpdateTemplateBatchIdByPkIdAsync(templateid, batchid);
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas;
            }
        }
        public static async Task<int> CreateOrUpdateTemplateAsync(PushTemplate template)
        {
            try
            {
#if DEBUG
                template.CreateUser = "test";
#endif
                if (template.PKID != 0)
                {
                    using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
                    {
                        var result = await client.UpdateTemplateAsync(template);
                        result.ThrowIfException(true);
                        var datas = result.Result;

                        return datas;
                    }
                }
                else
                {
                    if (template.BatchID != 0)
                    {
                        var temp = await SelectPushTemplateByBatchIDAndDeviceTypeAsync(template.BatchID, template.DeviceType);
                        if (temp != null)
                        {
                            template.PKID = temp.PKID;
                            return await CreateOrUpdateTemplateAsync(template);
                        }
                    }
                    if (string.IsNullOrEmpty(template.PlanName))
                    {
                        template.PlanName = "计划名称";
                    }
                    using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
                    {
                        var result = await client.CreateTemplateAsync(template);
                        result.ThrowIfException(true);
                        var datas = result.Result;

                        return datas;
                    }
                }
            }
            catch (System.Exception ex)
            {
                WebLog.LogException(ex);
                return 0;
            }

        }

        public static async Task<bool> UpdateTemplatePlanInfoAsync(int batchid, string planname,DateTime templateExpireTime, Dictionary<int, PushStatus> status)
        {
            using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
            { 
                var result = await client.UpdatePlanAsync(batchid, new Service.PushApi.Models.Push.UpdatePlanRequest() {
                    PlanName = planname,
                    Status = status,
                    TemplateExpireTime=templateExpireTime
                });
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas;
            }
        }

        public static async Task<bool> UpdateTemplatePushStatusAsync(int pkid, PushStatus status)
        {
            using (var client = new Tuhu.Service.PushApi.TemplateApiClient())
            {
                var result = await client.UpdateTemplatePushStatusAsync(pkid, status.Cast<Tuhu.Service.PushApi.Models.Push.PushStatus>());
                result.ThrowIfException(true);
                var data = result.Result;
                return data;
            }
        }

        public static string GetDeviceTypeName(DeviceType type)
        {
            switch (type)
            {
                case DeviceType.Android:
                    return "安卓推送";
                case DeviceType.iOS:
                    return "iOS推送";
                case DeviceType.MessageBox:
                    return "APP消息";
                case DeviceType.WeChat:
                    return "微信公众号模版消息";
                default:
                    return "";
            }
        }
        public static string GetPushStatusName(PushStatus status)
        {
            switch (status)
            {
                case PushStatus.Failed:
                    return "推送失败";
                case PushStatus.Intend:
                    return "计划推送";
                case PushStatus.Prepare:
                    return "已创建";
                case PushStatus.Pushing:
                    return "推送中";
                case PushStatus.Success:
                    return "推送成功";
                case PushStatus.Suspend:
                    return "推送取消";
                default:
                    return "";
            }
        }

        public static async Task<Guid?> SelectUserIDByPhoneNumberAsync(string number)
        {
            using (var client = new Tuhu.Service.UserAccount.UserAccountClient())
            {
                var result = await client.GetUserByMobileAsync(number);
                return result?.Result?.UserId;
            }
        }
        public static async Task<PushTemplate> SelectPushTemplateByBatchIDAndDeviceTypeAsync(int batchid, DeviceType deviceType)
        {
            string sql = $"SELECT TOP 10 * FROM Tuhu_notification..tbl_PushTemplate WITH ( NOLOCK) WHERE BatchID={batchid} AND DeviceType={(int)deviceType}";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<PushTemplate>()?.FirstOrDefault();
            }
        }

        public static async Task<IEnumerable<UserAuthInfo>> SelectUserAuthInfoAsync(Guid userid)
        {

            string sql = $"SELECT * FROM Tuhu_profiles..Userauth WITH ( NOLOCK) where userid='{userid}' and BindingStatus='Bound' ";
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<UserAuthInfo>()?.Distinct()?.ToList();
            }
        }

        public static async Task<IEnumerable<UserAuthInfo>> SelectWxOpenUserAuthInfoAsync(Guid userid)
        {
            string sql = $"SELECT    userid,UnionId,OpenId,'wxopen' AS channel  FROM Tuhu_notification..WXUserAuth WITH ( NOLOCK) WHERE UserId='{userid}' and BindingStatus='Bound' and AuthorizationStatus='Authorized'";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<UserAuthInfo>()?.Distinct().ToList();
            }
        }

        public static async Task<IEnumerable<WxAppPushToken>> SelectWxAppPushTokensByOpenIdAsync(string openid, int count)
        {
            string sql = $"SELECT TOP {count} * FROM Tuhu_notification..tbl_WxAppPushToken WITH ( NOLOCK) WHERE OpenID='{openid}' order by pkid desc ";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<WxAppPushToken>().ToList();
            }
        }

        public static async Task<IEnumerable<MessageNavigationType>> SelectMessageNavigationTypesAsync()
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.SelectAllMessageNavigationTypeAsync();
                result.ThrowIfException(true);
                return result.Result.Cast<List<MessageNavigationType>>() ?? new List<MessageNavigationType>().AsEnumerable();
            }
        }
        public static async Task<bool> UpdateMessageNavigationTypeAsync(MessageNavigationType type)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.UpdateMessageNavigationTypeAsync(type.Cast<Tuhu.Service.Push.Models.MessageBox.MessageNavigationType>());
                result.ThrowIfException(true);
                return result.Result > 0;
            }
        }
        public static async Task<bool> LogMessageNavigationTypeAsync(MessageNavigationType type)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.LogMessageNavigationTypeAsync(type.Cast<Tuhu.Service.Push.Models.MessageBox.MessageNavigationType>());
                result.ThrowIfException(true);
                return result.Result > 0;
            }
        }
        public static async Task<bool> DeleteMessageNavigationTypeAsync(int pkid)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.DeleteMessageNavigationTypeAsync(pkid);
                result.ThrowIfException(true);
                return result.Result > 0;
            }
        }

        public static async Task<int> CheckNavigationOrderAsync(int order, int pkid)
        {
            string sql = $"SELECT COUNT(1) FROM Tuhu_notification..MessageNavigationType WHERE [Order]={order} and pkid <>{pkid}";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                var result = await dbHelper.ExecuteScalarAsync(sql);
                int count = 1;
                int.TryParse(result?.ToString(), out count);
                return count;
            }
        }

        public static async Task<IEnumerable<MessageImageText>> SelectImageTextByBatchIdAsync(int templateid)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.SelectImageTextByBatchIdAsync(templateid);
                result.ThrowIfException(true);
                return result.Result.Cast<List<MessageImageText>>();
            }
        }

        public static async Task<bool> DeleteMessageImageTextAsync(int pkid)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.DeleteMessageImageTextAsync(pkid);
                result.ThrowIfException(true);
                return result.Result > 0;
            }
        }
        public static async Task<bool> SubmitMessageImageTextAsync(MessageImageText model)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                if (model.Pkid != 0)
                {
                    var result = await client.UpdateMessageImageTextAsync(model.Cast<Tuhu.Service.Push.Models.MessageBox.MessageImageText>());
                    result.ThrowIfException(true);
                    return result.Result > 0;
                }
                else
                {
                    var result = await client.InsertMessageImageTextAsync(model.Cast<Tuhu.Service.Push.Models.MessageBox.MessageImageText>());
                    result.ThrowIfException(true);
                    return result.Result > 0;
                }
            }
        }
        public static async Task<bool> CheckMessageImageTextOrderAsync(int batchid, int order, int pkid)
        {
            string sql =
                $"SELECT count(1) FROM Tuhu_notification..MessageImageText WITH(NOLOCK) WHERE batchid={batchid} AND [Order]={order} AND pkid<>{pkid}";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                var result = await dbHelper.ExecuteScalarAsync(sql);
                int count = 1;
                int.TryParse(result?.ToString(), out count);
                return count >= 1;
            }
        }

        /// <summary>
        /// 根据搜索条件查询出推送计划Id列表【1】
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<List<int>> GetPushPlanIdsWithQueryInfo(TemplateLogQueryInfo query)
        {
            var wheresb = new StringBuilder();

            var parameters = new List<SqlParameter>() { };

            wheresb.Append("SELECT  pkid FROM Tuhu_notification..tbl_PushTemplate WITH(NOLOCK) where 1=1 ");

            if (query.TemplateId > 0)
            {
                parameters.Add(new SqlParameter("@BatchId", query.TemplateId));
                wheresb.Append($" and batchId=@BatchId ");
            }

            if (query.PushPlanId > 0)
            {
                parameters.Add(new SqlParameter("@PushPlanId", query.PushPlanId));
                wheresb.Append($" and  pkid=@PushPlanId ");
            }

            if (!string.IsNullOrEmpty(query.PushPlanTitle))
            {
                parameters.Add(new SqlParameter("@PushPlanTitle", query.PushPlanTitle));
                wheresb.Append($" and  title like  '%' + @PushPlanTitle + '%' ");
            }

            if (query.DeviceType.HasValue)
            {
                parameters.Add(new SqlParameter("@DeviceType", (int)query.DeviceType.Value));
                wheresb.Append($" and  DeviceType=@DeviceType ");
            }

            wheresb.Append("order by pkid desc  OFFSET 0 ROWS  FETCH NEXT 1000 ROWS ONLY;");

            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            try
            {
                using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
                {
                    var result = await dbHelper.ExecuteDataTableAsync(wheresb.ToString(), CommandType.Text, parameters.ToArray());
                    if (result != null && result.Rows.Count > 0)
                    {
                        var data = result.AsEnumerable().Select(x => int.Parse(x[0].ToString())).ToList();
                        return data;
                    }
                    return new List<int>();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 根据搜索条件,推送计划Id列表,查询出模板变更记录列表【2】
        /// </summary>
        /// <param name="pushPlanIds"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<List<TemplateModifyLogInfo>> GetTemplateModifyLogInfoWithIdsAndQueryInfo(List<int> pushPlanIds, TemplateLogQueryInfo query)
        {
            var stringBuilder = new StringBuilder();
            if (query == null)
            {
                query = new TemplateLogQueryInfo() { PageIndex = 1, PageSize = 15 };
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@pagesize", query.PageSize== 0 ? 50 : query.PageSize),
                new SqlParameter("@pageindex", query.PageIndex == 0 ? 1 : query.PageIndex),
            };

            stringBuilder.Append("SELECT Pkid, [User] as ModifyUser,OriginTemplate,NewTemplate,TemplateId as PushPlanId,LastUpdateDateTime  from  Tuhu_log..PushTemplateModifyLog WITH(NOLOCK) where templateId>0 ");

            if (!pushPlanIds.Any())
            {
                pushPlanIds.Add(0);
            }

            stringBuilder.Append($" and TemplateId in({string.Join(",", pushPlanIds)})");


            if (!string.IsNullOrEmpty(query.ModifyUser))
            {
                parameters.Add(new SqlParameter("@ModifyUser", query.ModifyUser));
                stringBuilder.Append($" and  [user] like  '%' + @ModifyUser + '%'");
            }

            if (query.ModifyStartTime.HasValue)
            {
                parameters.Add(new SqlParameter("@ModifyStartTime", query.ModifyStartTime.Value));
                stringBuilder.Append($" and  lastupdateDateTime>=@ModifyStartTime ");
            }
            if (query.ModifyEndTime.HasValue)
            {
                parameters.Add(new SqlParameter("@ModifyEndTime", query.ModifyEndTime.Value));
                stringBuilder.Append($" and  lastupdateDateTime<=@ModifyEndTime ");
            }

            stringBuilder.Append($"order by Pkid desc  OFFSET  ( @pagesize * (@pageindex - 1 ) )  ROWS  FETCH NEXT @pagesize  ROWS ONLY;");



            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;

            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                return (await dbHelper.ExecuteDataTableAsync(stringBuilder.ToString(), CommandType.Text, parameters.ToArray())).ConvertTo<TemplateModifyLogInfo>()?.ToList();
            }
        }


        /// <summary>
        /// 根据搜索条件,推送计划Id列表,查询出模板变更记录列表【2】
        /// </summary>
        /// <param name="pushPlanIds"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<int> GetTemplateModifyLogCountWithIdsAndQueryInfo(List<int> pushPlanIds, TemplateLogQueryInfo query)
        {
            var stringBuilder = new StringBuilder();
            var parameters = new List<SqlParameter>();
            if (query == null)
            {
                query = new TemplateLogQueryInfo() { };
            }

            stringBuilder.Append("SELECT count(1)  from  Tuhu_log..PushTemplateModifyLog WITH(NOLOCK) where 1=1 ");

            if (!pushPlanIds.Any())
            {
                pushPlanIds.Add(0);
            }

            stringBuilder.Append($" and TemplateId in({string.Join(",", pushPlanIds)})");

            if (!string.IsNullOrEmpty(query.ModifyUser))
            {
                parameters.Add(new SqlParameter("@ModifyUser", query.ModifyUser));
                stringBuilder.Append($" and  [user] like  '%' + @ModifyUser + '%'");
            }

            if (query.ModifyStartTime.HasValue)
            {
                parameters.Add(new SqlParameter("@ModifyStartTime", query.ModifyStartTime.Value));
                stringBuilder.Append($" and  lastupdateDateTime>=@ModifyStartTime ");
            }
            if (query.ModifyEndTime.HasValue)
            {
                parameters.Add(new SqlParameter("@ModifyEndTime", query.ModifyEndTime.Value));
                stringBuilder.Append($" and  lastupdateDateTime<=@ModifyEndTime ");
            }

            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var result = await dbHelper.ExecuteScalarAsync(stringBuilder.ToString(), CommandType.Text, parameters.ToArray());
                int count = 1;
                int.TryParse(result?.ToString(), out count);
                return count;
            }
        }

        /// <summary>
        /// 根据推送ID列表，获取模板列表【3】
        /// </summary>
        /// <param name="pushPlanIds"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TemplateModifyLogInfo>> GetTemplateInfoFromIds(List<int> pushPlanIds)
        {
            if (pushPlanIds == null || !pushPlanIds.Any())
            {
                return new List<TemplateModifyLogInfo>();
            }
            var parameters = new List<SqlParameter>();

            string sql = $@"
                SELECT 
                BatchID as TemplateId,
                PlanName as TemplateTitle,
                Pkid as PushPlanId,
                Title as PushPlanTitle,
                DeviceType
                FROM Tuhu_notification..tbl_PushTemplate WITH(NOLOCK) where pkid in ({string.Join(",", pushPlanIds)})";



            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;

            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                return (await dbHelper.ExecuteDataTableAsync(cmd)).ConvertTo<TemplateModifyLogInfo>();
            }

        }


        /// <summary>
        /// 分页获取操作历史列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<TemplateLogInfoQueryResult> SelectPushTemplateModifyLogsAsync(TemplateLogQueryInfo query)
        {
            try
            {
                var pushIds = await GetPushPlanIdsWithQueryInfo(query);

                var logList = await GetTemplateModifyLogInfoWithIdsAndQueryInfo(pushIds, query);

                var countresult = await GetTemplateModifyLogCountWithIdsAndQueryInfo(pushIds, query);

                var templateList = await GetTemplateInfoFromIds(logList?.Select(o => o.PushPlanId).Distinct().ToList());

                foreach (var item in logList)
                {
                    var templateInfo = templateList.FirstOrDefault(o => o.PushPlanId == item.PushPlanId);

                    if (templateInfo == null) continue;

                    item.TemplateId = templateInfo.TemplateId;
                    item.TemplateTitle = templateInfo.TemplateTitle;
                    item.PushPlanTitle = templateInfo.PushPlanTitle;
                    item.DeviceType = templateInfo.DeviceType;
                }

                var result = new TemplateLogInfoQueryResult()
                {
                    TotalCount = countresult,
                    PageIndex = query.PageIndex,
                    Result = logList.ToList(),
                    PageSize = query.PageSize
                };

                return result;
            }
            catch (Exception ex)
            {
                return new TemplateLogInfoQueryResult() { };
            }
        }


        /// <summary>
        /// 搜索满足条件的数据，最多抓取500条记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<List<TemplateModifyLogInfo>> GetExportTemplateModifyLogDatas(TemplateLogQueryInfo query)
        {
            try
            {
                query.PageIndex = 1;
                query.PageSize = 500;

                var pushIds = await GetPushPlanIdsWithQueryInfo(query);

                var logList = await GetTemplateModifyLogInfoWithIdsAndQueryInfo(pushIds, query);

                var countresult = await GetTemplateModifyLogCountWithIdsAndQueryInfo(pushIds, query);

                var templateList = await GetTemplateInfoFromIds(logList?.Select(o => o.PushPlanId).Distinct().ToList());

                foreach (var item in logList)
                {
                    var templateInfo = templateList.FirstOrDefault(o => o.PushPlanId == item.PushPlanId);

                    if (templateInfo == null) continue;

                    item.TemplateId = templateInfo.TemplateId;
                    item.TemplateTitle = templateInfo.TemplateTitle;
                    item.PushPlanTitle = templateInfo.PushPlanTitle;
                    item.DeviceType = templateInfo.DeviceType;
                    item.NewTemplate = item.NewTemplate.Replace('\"', '\'');
                    item.OriginTemplate = item.OriginTemplate.Replace('\"', '\'');
                }
                return logList;
            }
            catch (Exception ex)
            {
                return new List<TemplateModifyLogInfo>();
            }
        }

        /// <summary>
        /// 转换列表成为内存流
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static System.IO.MemoryStream GetExportTemplateModifyLogStreams(List<TemplateModifyLogInfo> infos)
        {
            System.IO.MemoryStream output = new System.IO.MemoryStream();

            System.IO.StreamWriter writer = new System.IO.StreamWriter(output, System.Text.Encoding.UTF8);

            writer.Write("序号,操作时间,计划ID,计划名称,模板ID,模版标题,模板类型,操作人,操作前,操作后");//输出标题，逗号分割（注意最后一列不加逗号）

            writer.WriteLine();

            foreach (var item in infos)
            {
                writer.Write($"{item.PKID}\",\"");
                writer.Write($"{item.LastUpdateDateTime}\",\"");
                writer.Write($"{item.PushPlanId}\",\"");
                writer.Write($"{item.PushPlanTitle}\",\"");
                writer.Write($"{item.TemplateId}\",\"");
                writer.Write($"{item.TemplateTitle}\",\"");
                writer.Write($"{item.DeviceType.ToString()}\",\"");
                writer.Write($"{item.ModifyUser}\",\"");
                writer.Write($"{item.OriginTemplate}\",\"");
                writer.Write($"{item.NewTemplate}\"");

                writer.WriteLine();
            }

            writer.Flush();
            output.Position = 0;
            return output;
        }

    }

    public class TemplateLogQueryInfo
    {
        /// <summary> 模板ID【总模板ID】 </summary>
        public int TemplateId { get; set; }

        /// <summary> 计划ID【推送子渠道的计划ID】 </summary>
        public int PushPlanId { get; set; }

        /// <summary>各个渠道的标题</summary>
        public string PushPlanTitle { get; set; }

        /// <summary>模板类型</summary>
        public DeviceType? DeviceType { get; set; }

        /// <summary>是否启用</summary>
        public bool? IsEnable { get; set; }

        /// <summary>修改用户</summary>
        public string ModifyUser { get; set; }

        /// <summary>开始修改时间</summary>
        public DateTime? ModifyStartTime { get; set; }

        /// <summary>结束修改时间</summary>
        public DateTime? ModifyEndTime { get; set; }

        /// <summary>页索引</summary>
        public int PageIndex { get; set; }

        /// <summary>页大小</summary>
        public int PageSize { get; set; }
    }

    public class TemplateLogInfoQueryResult
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount
        {
            get
            {
                if ((TotalCount + PageSize - 1) <= 0) return 0;
                return (TotalCount + PageSize - 1) / PageSize;
            }
        }
        public List<TemplateModifyLogInfo> Result { get; set; }
    }

    public class TemplateModifyLogInfo
    {
        /// <summary>
        ///修改日志 的PKID【无意义】
        /// </summary>
        public int PKID { get; set; }

        /// <summary>模板ID</summary>
        public int TemplateId { get; set; }

        /// <summary>模版标题</summary>
        public string TemplateTitle { get; set; }

        /// <summary> 计划ID【推送子渠道的计划ID】 </summary>
        public int PushPlanId { get; set; }

        /// <summary>计划标题【各个渠道的标题】</summary>
        public string PushPlanTitle { get; set; }

        /// <summary>推送渠道</summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>是否启用</summary>
        public bool? IsEnable { get; set; }

        /// <summary>修改前模板</summary>
        public string OriginTemplate { get; set; }

        /// <summary>修改后模板</summary>
        public string NewTemplate { get; set; }

        /// <summary>修改用户</summary>
        public string ModifyUser { get; set; }

        /// <summary>最后修改时间</summary>
        public DateTime LastUpdateDateTime { get; set; }
    }

    public static class CastExtension
    {
        public static T Cast<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
    }
}

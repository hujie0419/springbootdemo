using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity.Push;
using Tuhu.Service.Push.Models.Push;
using Newtonsoft.Json;
using Tuhu.Component.Framework.Extension;
using Tuhu.Service.Push.Models.WeiXinPush;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Tuhu.Service.Push.Models.MessageBox;

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
                    //var list = from template in datas
                    //           group template by template.BatchID
                    //           into g
                    //           select new
                    //           {
                    //               batchid = g.Key,
                    //               datas = g
                    //           };
                    return datas;
                }
            }
            catch (System.Exception ex)
            {
                throw;
                return null;
            }
        }
        public static async Task<TemplateQueryResult> SelectPushTemplatesAsync(TemplateQuery query)
        {

            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.QueryTemplatesAsync(query);
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas;
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
                var result = await client.PushByAliasAsync(target, template);
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        public static async Task<bool> PushByRegidAsync(IEnumerable<string> target, PushTemplate template)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.PushByRegidAsync(target, template);
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        public static async Task<PushTemplate> SelectTemplateByPKIDAsync(int pkid)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.SelectTemplateByPKIDAsync(pkid);
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
        public static async Task<int> UpdateTemplateBatchIDAsync(int templateid, int batchid)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.UpdatePushTemplateBatchIDAsync(templateid, batchid);
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
                    using (var client = new Tuhu.Service.Push.TemplatePushClient())
                    {
                        var result = await client.UpdatePushTemplateAsync(template);
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
                    using (var client = new Tuhu.Service.Push.TemplatePushClient())
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

        public static async Task<int> UpdateTemplatePlanInfoAsync(int batchid, string planname, Dictionary<int, PushStatus> status)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.UpdatePlanInfoAsync(batchid, planname, status);
                result.ThrowIfException(true);
                var datas = result.Result;
                return datas;
            }
        }

        public static async Task<int> UpdateTemplatePushStatusAsync(int pkid, PushStatus status)
        {
            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.UpdateTemplatePushStatusAsync(pkid, status);
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
                return result.Result ?? new List<MessageNavigationType>().AsEnumerable();
            }
        }
        public static async Task<bool> UpdateMessageNavigationTypeAsync(MessageNavigationType type)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.UpdateMessageNavigationTypeAsync(type);
                result.ThrowIfException(true);
                return result.Result > 0;
            }
        }
        public static async Task<bool> LogMessageNavigationTypeAsync(MessageNavigationType type)
        {
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = await client.LogMessageNavigationTypeAsync(type);
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
                return result.Result;
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
                    var result = await client.UpdateMessageImageTextAsync(model);
                    result.ThrowIfException(true);
                    return result.Result > 0;
                }
                else
                {
                    var result = await client.InsertMessageImageTextAsync(model);
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
    }
}

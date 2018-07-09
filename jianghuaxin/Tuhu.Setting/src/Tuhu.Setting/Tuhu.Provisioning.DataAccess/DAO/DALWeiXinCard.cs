using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALWeiXinCard
    {
        public static int UpdateWeiXinCardPushCount(string cardid, int count)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"Update  [dbo].[WeiXinCard] SET PushedCount+=@PushedCount where card_id=@card_id");
                cmd.Parameters.AddWithValue("@PushedCount", count);
                cmd.Parameters.AddWithValue("@card_id", cardid);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int SaveWeiXinCard(WeixinCardModel model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"INSERT INTO [dbo].[WeiXinCard]([supplierId]
           ,[card_id]
           ,[card_type]           
           ,[deal_detail]
           ,[least_cost]
           ,[reduce_cost]
           ,[discount]
           ,[gift]
           ,[default_detail]
           ,[color]
           ,[title]
           ,[notice]
           ,[description]
           ,[quantity]
           ,[type]
           ,[begin_time]
           ,[end_time]
           ,[begin_timestamp]
           ,[end_timestamp]
           ,[fixed_term]
           ,[fixed_begin_term]
           ,[center_title]
           ,[center_sub_title]
           ,[center_url]
           ,[custom_url_name]
           ,[custom_url_sub_title]
           ,[custom_url]
           ,[promotion_url_name]
           ,[promotion_url_sub_title]
           ,[promotion_url]
           ,[service_phone]
           ,[get_limit]
           ,[use_limit]
           ,[can_share]
           ,[can_give_friend]
           ,[use_all_locations]
           ,[location_id_list]
           ,[source]
           ,[use_custom_code]
           ,[get_custom_code_mode]
           ,[bind_openid]       
           ,[LastUpdateDate],
            [accept_category],
reject_category,
object_use_for,
use_condition_least_cost,
can_use_with_other_discount,
abstract,
icon_url_list,
text_image_list)
           VALUES(@supplierId,@card_id,@card_type,@deal_detail,@least_cost,@reduce_cost,@discount,@gift,@default_detail,@color,@title,@notice,@description,@quantity,@type,@begin_time,@end_time,@begin_timestamp,@end_timestamp,@fixed_term,
            @fixed_begin_term,@center_title,@center_sub_title,@center_url,@custom_url_name,@custom_url_sub_title,@custom_url,@promotion_url_name,
            @promotion_url_sub_title,@promotion_url,@service_phone,@get_limit,@use_limit,@can_share,@can_give_friend,@use_all_locations,@location_id_list,@source,@use_custom_code,@get_custom_code_mode,@bind_openid,@LastUpdateDate,@accept_category,@reject_category,@object_use_for,@use_condition_least_cost,@can_use_with_other_discount,@abstract,@icon_url_list,@text_image_list) ");
                cmd.CommandType = CommandType.Text;
                WeixinCardBaseInfo weixinCardbaseinfo = model.total_info.base_info;
                WeixinCardAdvancedInfo weixinCardadvancedinfo = model.total_info.advanced_info;
                cmd.Parameters.AddWithValue("@supplierId", weixinCardbaseinfo.supplierId);
                cmd.Parameters.AddWithValue("@card_id", model.card_id);
                cmd.Parameters.AddWithValue("@card_type", model.card_type);
                cmd.Parameters.AddWithValue("@deal_detail", model.total_info.deal_detail);
                cmd.Parameters.AddWithValue("@least_cost", model.total_info.least_cost);
                cmd.Parameters.AddWithValue("@reduce_cost", model.total_info.reduce_cost);
                cmd.Parameters.AddWithValue("@discount", model.total_info.discount);
                cmd.Parameters.AddWithValue("@gift", model.total_info.gift);

                //@default_detail,@color,@title,@notice,@description,@quantity,@type,@begin_timestamp,@end_timestamp,@fixed_term,
                cmd.Parameters.AddWithValue("@default_detail", model.total_info.default_detail);
                cmd.Parameters.AddWithValue("@color", weixinCardbaseinfo.color);
                cmd.Parameters.AddWithValue("@title", weixinCardbaseinfo.title);
                cmd.Parameters.AddWithValue("@notice", weixinCardbaseinfo.notice);
                cmd.Parameters.AddWithValue("@description", weixinCardbaseinfo.description);
                cmd.Parameters.AddWithValue("@quantity", 0);
                cmd.Parameters.AddWithValue("@type", weixinCardbaseinfo.date_info.type);
                cmd.Parameters.AddWithValue("@begin_time", weixinCardbaseinfo.date_info.begin_time);
                cmd.Parameters.AddWithValue("@end_time", weixinCardbaseinfo.date_info.end_time);
                cmd.Parameters.AddWithValue("@begin_timestamp", (int)weixinCardbaseinfo.date_info.begin_timestamp);
                cmd.Parameters.AddWithValue("@end_timestamp", (int)weixinCardbaseinfo.date_info.end_timestamp);
                cmd.Parameters.AddWithValue("@fixed_term", weixinCardbaseinfo.date_info.fixed_term);
                //@fixed_begin_term,@center_title,@center_sub_title,@center_url,@custom_url_name,@custom_url_sub_title,@custom_url,@promotion_url_name,
                //@promotion_url_sub_title,@promotion_url,@service_phone,@get_limit
                cmd.Parameters.AddWithValue("@fixed_begin_term", weixinCardbaseinfo.date_info.fixed_begin_term);
                cmd.Parameters.AddWithValue("@center_title", weixinCardbaseinfo.center_title);
                cmd.Parameters.AddWithValue("@center_sub_title", weixinCardbaseinfo.center_sub_title);
                cmd.Parameters.AddWithValue("@center_url", weixinCardbaseinfo.center_url);
                cmd.Parameters.AddWithValue("@custom_url_name", weixinCardbaseinfo.custom_url_name);
                cmd.Parameters.AddWithValue("@custom_url_sub_title", weixinCardbaseinfo.custom_url_sub_title);
                cmd.Parameters.AddWithValue("@custom_url", weixinCardbaseinfo.custom_url);


                cmd.Parameters.AddWithValue("@promotion_url_name", weixinCardbaseinfo.promotion_url_name);
                cmd.Parameters.AddWithValue("@promotion_url_sub_title", weixinCardbaseinfo.promotion_url_sub_title);
                cmd.Parameters.AddWithValue("@promotion_url", weixinCardbaseinfo.promotion_url);
                cmd.Parameters.AddWithValue("@service_phone", weixinCardbaseinfo.service_phone);

                //@use_limit,@can_share,@can_give_friend,@use_all_locations,@location_id_list,@source,@use_custom_code,@get_custom_code_mode,@bind_openid,@LastUpdateDate
                cmd.Parameters.AddWithValue("@get_limit", weixinCardbaseinfo.get_limit);
                cmd.Parameters.AddWithValue("@use_limit", weixinCardbaseinfo.use_limit);
                cmd.Parameters.AddWithValue("@can_share", weixinCardbaseinfo.can_share);
                cmd.Parameters.AddWithValue("@can_give_friend", weixinCardbaseinfo.can_give_friend);
                cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);


                cmd.Parameters.AddWithValue("@use_custom_code", weixinCardbaseinfo.use_custom_code);
                cmd.Parameters.AddWithValue("@get_custom_code_mode", weixinCardbaseinfo.get_custom_code_mode);
                cmd.Parameters.AddWithValue("@bind_openid", weixinCardbaseinfo.bind_openid);                
                cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                //cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                //cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                //cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);

                cmd.Parameters.AddWithValue("@accept_category", weixinCardadvancedinfo.use_condition.accept_category);
                cmd.Parameters.AddWithValue("@reject_category", weixinCardadvancedinfo.use_condition.reject_category);
                cmd.Parameters.AddWithValue("@object_use_for", weixinCardadvancedinfo.use_condition.object_use_for);
                cmd.Parameters.AddWithValue("@use_condition_least_cost", weixinCardadvancedinfo.use_condition.least_cost);
                cmd.Parameters.AddWithValue("@can_use_with_other_discount", weixinCardadvancedinfo.use_condition.can_use_with_other_discount);
                cmd.Parameters.AddWithValue("@abstract", weixinCardadvancedinfo.abstractinfo.abstractstr);
                cmd.Parameters.AddWithValue("@icon_url_list", weixinCardadvancedinfo.abstractinfo.icon_url_list?.Count()>0?string.Join(";", weixinCardadvancedinfo.abstractinfo.icon_url_list):string.Empty);
                cmd.Parameters.AddWithValue("@text_image_list", weixinCardadvancedinfo.text_image_list?.Count()>0?JsonConvert.SerializeObject(weixinCardadvancedinfo.text_image_list):string.Empty);
                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static int UpdateWeiXinCard(WeixinCardModel model)
        {
           
                using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
                {
                    var cmd = new SqlCommand(@"Update  [dbo].[WeiXinCard]          
           set [card_type]=@card_type           
           ,[deal_detail]=@deal_detail
           ,[least_cost]=@least_cost
           ,[reduce_cost]=@reduce_cost
           ,[discount]=@discount
           ,[gift]=@gift
           ,[default_detail]=@default_detail
           ,[color]=@color
           ,[title]=@title
           ,[notice]=@notice
           ,[description]=@description
           ,[quantity]=@quantity
           ,[type]=@type
           ,[begin_time]=@begin_time
           ,[end_time]=@end_time
           ,[begin_timestamp]=@begin_timestamp
           ,[end_timestamp]=@end_timestamp
           ,[fixed_term]=@fixed_term
           ,[fixed_begin_term]=@fixed_begin_term
           ,[center_title]=@center_title
           ,[center_sub_title]=@center_sub_title
           ,[center_url]=@center_url
           ,[custom_url_name]=@custom_url_name
           ,[custom_url_sub_title]=@custom_url_sub_title
           ,[custom_url]=@custom_url
           ,[promotion_url_name]=@promotion_url_name
           ,[promotion_url_sub_title]=@promotion_url_sub_title
           ,[promotion_url]=@promotion_url
           ,[service_phone]=@service_phone
           ,[get_limit]=@get_limit
           ,[use_limit]=@use_limit
           ,[can_share]=@can_share
           ,[can_give_friend]=@can_give_friend
           ,[use_all_locations]=@use_all_locations
           ,[location_id_list]=@location_id_list
           ,[source]=@source
           ,[use_custom_code]=@use_custom_code
           ,[get_custom_code_mode]=@get_custom_code_mode
           ,[bind_openid]=@bind_openid
           ,[LastUpdateDate]=@LastUpdateDate
           , [accept_category]=@accept_category
           ,[reject_category]=@reject_category
           ,object_use_for=@object_use_for
           ,use_condition_least_cost=@use_condition_least_cost            
            ,can_use_with_other_discount=@can_use_with_other_discount
           ,abstract=@abstract
           ,icon_url_list=@icon_url_list
           ,text_image_list=@text_image_list
           where card_id=@card_id");
                    cmd.CommandType = CommandType.Text;
                    WeixinCardBaseInfo weixinCardbaseinfo = model.total_info.base_info;
                    WeixinCardAdvancedInfo weixinCardadvancedinfo = model.total_info.advanced_info;
                    cmd.Parameters.AddWithValue("@card_id", model.card_id);
                    cmd.Parameters.AddWithValue("@card_type", model.card_type);
                    cmd.Parameters.AddWithValue("@deal_detail", model.total_info.deal_detail);
                    cmd.Parameters.AddWithValue("@least_cost", model.total_info.least_cost);
                    cmd.Parameters.AddWithValue("@reduce_cost", model.total_info.reduce_cost);
                    cmd.Parameters.AddWithValue("@discount", model.total_info.discount);
                    cmd.Parameters.AddWithValue("@gift", model.total_info.gift);

                    //@default_detail,@color,@title,@notice,@description,@quantity,@type,@begin_timestamp,@end_timestamp,@fixed_term,
                    cmd.Parameters.AddWithValue("@default_detail", model.total_info.default_detail);
                    cmd.Parameters.AddWithValue("@color", weixinCardbaseinfo.color);
                    cmd.Parameters.AddWithValue("@title", weixinCardbaseinfo.title);
                    cmd.Parameters.AddWithValue("@notice", weixinCardbaseinfo.notice);
                    cmd.Parameters.AddWithValue("@description", weixinCardbaseinfo.description);
                    cmd.Parameters.AddWithValue("@quantity", 0);
                    cmd.Parameters.AddWithValue("@type", weixinCardbaseinfo.date_info.type);
                    cmd.Parameters.AddWithValue("@begin_time", weixinCardbaseinfo.date_info.begin_time);
                    cmd.Parameters.AddWithValue("@end_time", weixinCardbaseinfo.date_info.end_time);
                    cmd.Parameters.AddWithValue("@begin_timestamp", (int)weixinCardbaseinfo.date_info.begin_timestamp);
                    cmd.Parameters.AddWithValue("@end_timestamp", (int)weixinCardbaseinfo.date_info.end_timestamp);
                    cmd.Parameters.AddWithValue("@fixed_term", weixinCardbaseinfo.date_info.fixed_term);
                    //@fixed_begin_term,@center_title,@center_sub_title,@center_url,@custom_url_name,@custom_url_sub_title,@custom_url,@promotion_url_name,
                    //@promotion_url_sub_title,@promotion_url,@service_phone,@get_limit
                    cmd.Parameters.AddWithValue("@fixed_begin_term", weixinCardbaseinfo.date_info.fixed_begin_term);
                    cmd.Parameters.AddWithValue("@center_title", weixinCardbaseinfo.center_title);
                    cmd.Parameters.AddWithValue("@center_sub_title", weixinCardbaseinfo.center_sub_title);
                    cmd.Parameters.AddWithValue("@center_url", weixinCardbaseinfo.center_url);
                    cmd.Parameters.AddWithValue("@custom_url_name", weixinCardbaseinfo.custom_url_name);
                    cmd.Parameters.AddWithValue("@custom_url_sub_title", weixinCardbaseinfo.custom_url_sub_title);
                    cmd.Parameters.AddWithValue("@custom_url", weixinCardbaseinfo.custom_url);


                    cmd.Parameters.AddWithValue("@promotion_url_name", weixinCardbaseinfo.promotion_url_name);
                    cmd.Parameters.AddWithValue("@promotion_url_sub_title", weixinCardbaseinfo.promotion_url_sub_title);
                    cmd.Parameters.AddWithValue("@promotion_url", weixinCardbaseinfo.promotion_url);
                    cmd.Parameters.AddWithValue("@service_phone", weixinCardbaseinfo.service_phone);

                    //@use_limit,@can_share,@can_give_friend,@use_all_locations,@location_id_list,@source,@use_custom_code,@get_custom_code_mode,@bind_openid,@LastUpdateDate
                    cmd.Parameters.AddWithValue("@get_limit", weixinCardbaseinfo.get_limit);
                    cmd.Parameters.AddWithValue("@use_limit", weixinCardbaseinfo.use_limit);
                    cmd.Parameters.AddWithValue("@can_share", weixinCardbaseinfo.can_share);
                    cmd.Parameters.AddWithValue("@can_give_friend", weixinCardbaseinfo.can_give_friend);
                    cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                    cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                    cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);


                    cmd.Parameters.AddWithValue("@use_custom_code", weixinCardbaseinfo.use_custom_code);
                    cmd.Parameters.AddWithValue("@get_custom_code_mode", weixinCardbaseinfo.get_custom_code_mode);
                    cmd.Parameters.AddWithValue("@bind_openid", weixinCardbaseinfo.bind_openid);
                    cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@accept_category", weixinCardadvancedinfo.use_condition.accept_category);
                    cmd.Parameters.AddWithValue("@reject_category", weixinCardadvancedinfo.use_condition.reject_category);
                    cmd.Parameters.AddWithValue("@object_use_for", weixinCardadvancedinfo.use_condition.object_use_for);
                    cmd.Parameters.AddWithValue("@use_condition_least_cost", weixinCardadvancedinfo.use_condition.least_cost);
                    cmd.Parameters.AddWithValue("@can_use_with_other_discount", weixinCardadvancedinfo.use_condition.can_use_with_other_discount);
                    cmd.Parameters.AddWithValue("@abstract", weixinCardadvancedinfo.abstractinfo.abstractstr);
                    cmd.Parameters.AddWithValue("@icon_url_list", weixinCardadvancedinfo.abstractinfo.icon_url_list != null && weixinCardadvancedinfo.abstractinfo.icon_url_list.Count() > 0 ? string.Join(";", weixinCardadvancedinfo.abstractinfo.icon_url_list) : string.Empty);
                    cmd.Parameters.AddWithValue("@text_image_list", weixinCardadvancedinfo.text_image_list != null && weixinCardadvancedinfo.text_image_list.Count() > 0 ? JsonConvert.SerializeObject(weixinCardadvancedinfo.text_image_list) : string.Empty);
                    //cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                    //cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                    //cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);

                    return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
                }
            }          
            
        




        public static int SaveWeiXinCardSupplier(SupplierInfo model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"INSERT INTO [dbo].[WeiXinCardSupplier]([brand_name]
           ,[logo_url],[LastUpdateDate])
           VALUES(@brand_name,@logo_url,@LastUpdateDate) ");
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@brand_name", model.brand_name);
                cmd.Parameters.AddWithValue("@logo_url", model.logo_url);
                cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                //cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                //cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                //cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);

                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static int DeleteWeiXinCardSupplier(int pkid)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"Delete [dbo].[WeiXinCardSupplier] WHERE PKID=@PKID");
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@PKID", pkid);

                //cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                //cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                //cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);

                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static int DeleteWeiXinCard(int pkid)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"Delete [dbo].[WeiXinCard] WHERE PKID=@PKID");
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@PKID", pkid);

                //cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                //cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                //cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);

                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static List<SupplierInfo> GetSupplierInfo(int pkid)
        {
            // SupplierInfo result = null;
            List<SupplierInfo> items;
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                string sqlstr = string.Empty;
                if (pkid > 0)
                {
                    sqlstr = @"SELECT [PKID],[brand_name],[logo_url],[LastUpdateDate] FROM WeiXinCardSupplier WHERE PKID=@PKID";
                }
                else
                {
                    sqlstr = @"SELECT [PKID],[brand_name],[logo_url],[LastUpdateDate] FROM WeiXinCardSupplier";
                }
                var cmd = new SqlCommand(sqlstr);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@PKID", pkid);              

                items = dbhelper.ExecuteDataTable(cmd).ConvertTo<SupplierInfo>().ToList();

            }
            return items;
        }

        public static DataTable GetWeiXinCardList(int pkid = -1)
        {
            DataTable dt = null;
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                string sqlstr = string.Empty;

                sqlstr = @"SELECT PKID,[supplierId],[card_id]
           ,[card_type]           
           ,[deal_detail]
           ,[least_cost]
           ,[reduce_cost]
           ,[discount]
           ,[gift]
           ,[default_detail]
           ,[color]
           ,[title]
           ,[notice]
           ,[description]
           ,[quantity]
           ,[type]
           ,[begin_time]
           ,[end_time]
           ,[begin_timestamp]
           ,[end_timestamp]
           ,[fixed_term]
           ,[fixed_begin_term]
           ,[center_title]
           ,[center_sub_title]
           ,[center_url]
           ,[custom_url_name]
           ,[custom_url_sub_title]
           ,[custom_url]
           ,[promotion_url_name]
           ,[promotion_url_sub_title]
           ,[promotion_url]
           ,[service_phone]
           ,[get_limit]
           ,[use_limit]
           ,[can_share]
           ,[can_give_friend]
           ,[use_all_locations]
           ,[location_id_list]
           ,[source]
           ,[use_custom_code]
           ,[get_custom_code_mode]
           ,[bind_openid]         
           ,[LastUpdateDate],PushedCount,[accept_category],
reject_category,
object_use_for,
use_condition_least_cost,
can_use_with_other_discount,
abstract,
icon_url_list,
text_image_list FROM WeiXinCard WHERE PKID=@pkid";

                var cmd = new SqlCommand(sqlstr);
                cmd.CommandType = CommandType.Text;

                if (pkid > 0)
                {
                    cmd.Parameters.AddWithValue("@pkid", pkid);
                }
                else
                {
                    cmd.CommandText += " OR (1=1)";
                    cmd.Parameters.AddWithValue("@pkid", -1);
                }
           
                dt = dbhelper.ExecuteDataTable(cmd);

            }
            return dt;
        }

        public static int UpdateSupplierInfo(SupplierInfo model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"UPDATE [dbo].[WeiXinCardSupplier]
   SET [brand_name] = @brand
      ,[logo_url] = @logo
      ,[LastUpdateDate] = @time
WHERE pkid=@pkid");

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@brand", model.brand_name);
                cmd.Parameters.AddWithValue("@logo", model.logo_url);
                cmd.Parameters.AddWithValue("@time", DateTime.Now);
                cmd.Parameters.AddWithValue("@pkid", model.pkid);


                //cmd.Parameters.AddWithValue("@use_all_locations", weixinCardbaseinfo.use_all_locations);
                //cmd.Parameters.AddWithValue("@location_id_list", weixinCardbaseinfo.location_id_list);
                //cmd.Parameters.AddWithValue("@source", weixinCardbaseinfo.source);

                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static int InsertPromotionCodeToWeixinCard(string cardid,int count=100)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {

                string sql = @"DECLARE @Count INT = 1
DECLARE @Code NVARCHAR(20)

WHILE @Count <=@SpecificCount
BEGIN
SET @Code = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)
                                                              * CHECKSUM(NEWID())), 12)
															  
INSERT INTO [dbo].[WeiXinCardCode]
   ([card_id] ,[code] ,[LastUpdatedTime] )
   VALUES(@card_id,@Code,GETDATE())
   SET @Count+=1
END";


                var cmd = new SqlCommand(sql);


                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@card_id", cardid);
                cmd.Parameters.AddWithValue("@SpecificCount", count);
                return dbhelper.ExecuteNonQuery(cmd);
            }

        }


        public static int GetTopPkidByCardId(string cardid)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                string sql = "SELECT TOP 1 pkid FROM [dbo].WeiXinCardCode WHERE card_id=@card_id ORDER BY PKID DESC";
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@card_id", cardid);
                return (int)dbhelper.ExecuteScalar(cmd);
            }
        }
        public static DataTable GetWeixinCardCode(string cardid,int pkid, int count = 100)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {

                string sql = @"SELECT TOP "+Convert.ToInt16(count)+ " code FROM [dbo].[WeiXinCardCode] WHERE card_id=@card_id AND PKID<=@pkid ORDER BY PKID DESC";


                var cmd = new SqlCommand(sql);


                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@card_id", cardid);
                cmd.Parameters.AddWithValue("@pkid", pkid);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
    }
}

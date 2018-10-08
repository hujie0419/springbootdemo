using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.Models.Activity;

namespace Tuhu.Service.Activity.DataAccess
{
   public  class DalActivityPage
    {
        public static async Task<ActivityPageInfoTireSizeConfigModel> FetchActivityPageTireSizeConfigModelAsync(int id)
        {
            using (var cmd = new SqlCommand(@"select * from Configuration.dbo.ActivePageTireSizeConfig with(nolock)
                                        where FKActiveID=@ID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return await DbHelper.ExecuteFetchAsync<ActivityPageInfoTireSizeConfigModel>(true, cmd);
            }
        }
        public static async Task<IEnumerable<DalActivityPageContent>> SelectActivityPageContentsAsync(int id, string channel)
        {
            using (var cmd = new SqlCommand(@"select [Group],RowType,Type,OrderBy from Configuration.DBO.ActivePageContent with(nolock) where FKActiveID=@ID AND (channel=@channel or channel='all') order by OrderBY"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@channel", channel);
                return await DbHelper.ExecuteSelectAsync<DalActivityPageContent>(true, cmd);
            }
        }
        public static async Task<ActivityPageInfoModel> FetchActivityPageModelAsync(int id)
        {
            using (var cmd = new SqlCommand(@"
                                            SELECT  Title ,
                                                    H5Uri ,
                                                    WWWUri ,
                                                    BgImageUrl ,
                                                    BgColor ,
                                                    TireBrand ,
                                                    ActivityType ,
                                                    DataParames ,
                                                    MenuType ,
                                                    IsShowDate ,
                                                    SelKeyImage ,
                                                    SelKeyName ,
                                                    IsTireSize ,
                                                    StartDate ,
                                                    EndDate ,
                                                    CustomerService ,
                                                    IsNeedLogIn,
		                                            FloatWindow,
		                                            FloatWindowImageUrl,
		                                            AlertTabImageUrl,
		                                            AlertJumpApp,
		                                            AlertJumpWxApp,
		                                            FloatWindowJump
                                            FROM    Configuration.dbo.ActivePageList WITH ( NOLOCK ) where PKID=@ID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return await DbHelper.ExecuteFetchAsync<ActivityPageInfoModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<DalActivityPageMenuModel>> SelectActivePageMenus(List<int> contentIds)
        {
            using (var cmd = new SqlCommand(@"select FKActiveContentID,MenuName,MenuValue,MenuValueEnd,Sort,Color,Description from Configuration.dbo.ActivePageMenu with(nolock) where FKActiveContentID in (SELECT  *
                                                                                                             FROM    Gungnir.dbo.Split(@ContentIds, ',')) order by Sort"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ContentIds", string.Join(",", contentIds));
                return await DbHelper.ExecuteSelectAsync<DalActivityPageMenuModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<T>> SelectActivePageInfoCells<T>(int id,List<int> types,string channel) where T: class,new()
        {
            using (var cmd = new SqlCommand(@"select * from Configuration.DBO.ActivePageContent with(nolock) where FKActiveID=@ID AND (channel=@channel or channel='all') AND Type in (SELECT  *
                                                                                                             FROM    Gungnir.dbo.Split(@types, ',')) order by OrderBY"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@types", string.Join(",", types));
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@channel", channel);
                return await DbHelper.ExecuteSelectAsync<T>(true, cmd);
            }
        }
    }
}

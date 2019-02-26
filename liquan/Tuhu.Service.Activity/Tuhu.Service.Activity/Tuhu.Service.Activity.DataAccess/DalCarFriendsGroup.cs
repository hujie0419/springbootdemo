using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Response;
using static Tuhu.Service.Activity.Models.Response.CarFriendsGroupInfoResponse;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalCarFriendsGroup
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalCarFriendsGroup));

        /// <summary>
        /// 筛选车型得到的车型群
        /// </summary>
        /// <param name="VehicleList"></param>
        /// <returns></returns>
        public static async Task<CarFriendsGroupInfoResponse> GetFilterCarFriendsGroupListAsync(List<string> VehicleList)
        {
            var carFriendsGroupInfoResponse = new CarFriendsGroupInfoResponse();
            var carFriendsGroupList = new List<CarFriendsGroup>();
            int carFriendsGroupCount = 0;
            try
            {
                string sql = @"
                                SELECT  [PKID] ,
                                        [GroupName] ,
                                        [GroupDesc] ,
                                        [BindVehicleType] ,
                                        [BindVehicleTypeID] ,
                                        [GroupHeadPortrait] ,
                                        [GroupQRCode] ,
                                        [GroupCategory] ,
                                        [GroupWeight] ,
                                        [IsRecommend] ,
                                        [Is_Deleted] ,
                                        [GroupCreateTime] ,
                                        [GroupOverdueTime] ,
                                        [CreateDatetime] ,
                                        [LastUpdateDateTime] ,
                                        [CreateBy] ,
                                        [LastUpdateBy]
                                FROM    Activity.[dbo].[CarFriendsWeChatGroup] WITH(NOLOCK)
                                WHERE   GroupCategory = 0
                                        AND Is_Deleted=0
                                        AND (";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(sql);
                for (int i = 0; i < VehicleList.Count; i++)
                {
                    string parameter = "@BindVehicleType" + i;
                    stringBuilder.Append("BindVehicleType="+parameter+" or ");
                }
                stringBuilder.Remove(stringBuilder.Length - 3, 3);
                stringBuilder.Append(" ) ORDER BY GroupWeight;");

                string sqlCount = @"
                                    SELECT  COUNT(*)
                                    FROM    Activity.[dbo].[CarFriendsWeChatGroup] WITH(NOLOCK)
                                    WHERE   GroupCategory = 0
                                            AND Is_Deleted=0
                                            AND (";
                StringBuilder stringBuilderCount = new StringBuilder();
                stringBuilderCount.Append(sqlCount);
                for (int i = 0; i < VehicleList.Count; i++)
                {
                    string parameter = "@BindVehicleType" + i;
                    stringBuilderCount.Append("BindVehicleType=" + parameter + " or ");
                }
                stringBuilderCount.Remove(stringBuilderCount.Length - 3, 3);
                stringBuilderCount.Append(" ) ;");

                var parameterList = new List<SqlParameter>();
                for (int i = 0; i < VehicleList.Count; i++)
                {
                    parameterList.Add(new SqlParameter($"@BindVehicleType{i}", VehicleList[i]));
                }
                using (var cmd = new SqlCommand(stringBuilder.ToString()))
                {
                    cmd.Parameters.AddRange(parameterList.ToArray());
                    carFriendsGroupList = (await DbHelper.ExecuteSelectAsync<CarFriendsGroup>(true, cmd)).ToList();
                    cmd.Parameters.Clear();
;                }
                using (var cmd = new SqlCommand(stringBuilderCount.ToString()))
                {
                    cmd.Parameters.AddRange(parameterList.ToArray());
                    carFriendsGroupCount = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetFilterCarFriendsGroupListAsync -> {string.Join("','", VehicleList)}", e);
            }
            carFriendsGroupInfoResponse.groupList = carFriendsGroupList;
            carFriendsGroupInfoResponse.groupCount = carFriendsGroupCount;
            return carFriendsGroupInfoResponse;
        }

        /// <summary>
        /// 热门车友群/热门推荐群/全部车友群
        /// </summary>
        /// <param name="isRecommend"></param>
        /// <returns></returns>
        public static async Task<CarFriendsGroupInfoResponse> GetIsRecommendCarFriendsGroupListAsync(bool isRecommend)
        {
            var carFriendsGroupInfoResponse = new CarFriendsGroupInfoResponse();
            var carFriendsGroupList = new List<CarFriendsGroup>();
            int carFriendsGroupCount = 0;
            try
            {
                string sql = @"
                                SELECT  [PKID] ,
                                        [GroupName] ,
                                        [GroupDesc] ,
                                        [BindVehicleType] ,
                                        [BindVehicleTypeID] ,
                                        [GroupHeadPortrait] ,
                                        [GroupQRCode] ,
                                        [GroupCategory] ,
                                        [GroupWeight] ,
                                        [IsRecommend] ,
                                        [Is_Deleted] ,
                                        [GroupCreateTime] ,
                                        [GroupOverdueTime] ,
                                        [CreateDatetime] ,
                                        [LastUpdateDateTime] ,
                                        [CreateBy] ,
                                        [LastUpdateBy]
                                FROM    Activity.[dbo].[CarFriendsWeChatGroup] WITH(NOLOCK)
                                WHERE   GroupCategory = 0
                                        AND Is_Deleted = 0";
                if (isRecommend)
                {
                    sql += " AND isRecommend=1";
                }
                sql += " ORDER BY GroupWeight";

                string sqlCount = @"
                                    SELECT  COUNT(*)
                                    FROM    Activity.[dbo].[CarFriendsWeChatGroup] WITH(NOLOCK)
                                    WHERE   GroupCategory = 0
                                            AND Is_Deleted = 0";
                if (isRecommend)
                {
                    sqlCount += " AND isRecommend=1";
                }
                using (var cmd = new SqlCommand(sql))
                {
                    carFriendsGroupList = (await DbHelper.ExecuteSelectAsync<CarFriendsGroup>(true, cmd)).ToList();
                }
                using (var cmd = new SqlCommand(sqlCount))
                {
                    carFriendsGroupCount = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetIsRecommendCarFriendsGroupListAsync -> {isRecommend}", e);
            }
            carFriendsGroupInfoResponse.groupList = carFriendsGroupList;
            carFriendsGroupInfoResponse.groupCount = carFriendsGroupCount;
            return carFriendsGroupInfoResponse;
        }

        /// <summary>
        /// 获取所有热门车型 
        /// </summary>
        /// <returns></returns>
        public static async Task<List<RecommendVehicleResponse>> GetRecommendVehicleListAsync()
        {
            try
            {
                string sql = @"
                            SELECT  BindVehicleType ,
                                    BindVehicleTypeID
                            FROM    Activity.dbo.CarFriendsWeChatGroup WITH(NOLOCK)
                            WHERE   GroupCategory = 0
                                    AND IsRecommend = 1
                                    AND Is_Deleted = 0
                            GROUP BY BindVehicleType ,
                                    BindVehicleTypeID;";
                using (var cmd = new SqlCommand(sql))
                {
                    return (await DbHelper.ExecuteSelectAsync<RecommendVehicleResponse>(true, cmd)).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error("GetRecommendVehicleListAsync", e);
                throw;
            }
        }

        /// <summary>
        /// 根据pkid获取车友群
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<CarFriendsGroupInfoResponse> GetCarFriendsGroupModelAsync(int pkid)
        {
            var carFriendsGroupInfoResponse = new CarFriendsGroupInfoResponse();
            var carFriendsGroupList = new List<CarFriendsGroup>();
            try
            {
                string sql = @"
                             SELECT [PKID] ,
                                    [GroupName] ,
                                    [GroupDesc] ,
                                    [BindVehicleType] ,
                                    [BindVehicleTypeID] ,
                                    [GroupHeadPortrait] ,
                                    [GroupQRCode] ,
                                    [GroupCategory] ,
                                    [GroupWeight] ,
                                    [IsRecommend] ,
                                    [Is_Deleted] ,
                                    [GroupCreateTime] ,
                                    [GroupOverdueTime] ,
                                    [CreateDatetime] ,
                                    [LastUpdateDateTime] ,
                                    [CreateBy] ,
                                    [LastUpdateBy]
                             FROM   Activity.[dbo].[CarFriendsWeChatGroup] WITH ( NOLOCK )
                             WHERE  PKID = @PKID;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    carFriendsGroupList = (await DbHelper.ExecuteSelectAsync<CarFriendsGroup>(true, cmd)).ToList();
                    carFriendsGroupInfoResponse.groupList = carFriendsGroupList;
                    carFriendsGroupInfoResponse.groupCount = carFriendsGroupList.Count;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetCarFriendsGroupModelAsync -> {pkid}", e);
                throw;
            }
            return carFriendsGroupInfoResponse;
        }

       /// <summary>
       /// 获取群主信息
       /// </summary>
       /// <returns></returns>
        public static async Task<CarFriendsAdministratorsResponse> GetCarFriendsAdministratorsModelAsync()
        {
            try
            {
                string sql = @"
                         SELECT TOP 1
                                    [PKID] ,
                                    [WeChatNickName] ,
                                    [WeChatNumber] ,
                                    [WeChatHeadPortrait] ,
                                    [WeChatQRCode] ,
                                    [Is_Deleted] ,
                                    [CreateDatetime] ,
                                    [LastUpdateDateTime] ,
                                    [CreateBy] ,
                                    [LastUpdateBy]
                            FROM    Activity.[dbo].[CarFriendsAdministrators] WITH ( NOLOCK )
                            WHERE   Is_Deleted = 0
                            ORDER BY CreateDatetime DESC;";
                using (var cmd = new SqlCommand(sql))
                {
                    return (await DbHelper.ExecuteSelectAsync<CarFriendsAdministratorsResponse>(true, cmd)).ToList().FirstOrDefault();
                }
            }
            catch(Exception e)
            {
                Logger.Error("GetCarFriendsAdministratorsModelAsync", e);
                throw;
            }
        }

        /// <summary>
        /// 根据pkid获取群主信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<CarFriendsAdministratorsResponse> GetCarFriendsAdministratorsModelByPkidAsync(int pkid)
        {
            try
            {
                string sql = @"
                         SELECT [PKID] ,
                                [WeChatNickName] ,
                                [WeChatNumber] ,
                                [WeChatHeadPortrait] ,
                                [WeChatQRCode] ,
                                [Is_Deleted] ,
                                [CreateDatetime] ,
                                [LastUpdateDateTime] ,
                                [CreateBy] ,
                                [LastUpdateBy]
                        FROM    Activity.[dbo].[CarFriendsAdministrators] WITH ( NOLOCK )
                        WHERE   Is_Deleted = 0 AND PKID=@PKID";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return (await DbHelper.ExecuteSelectAsync<CarFriendsAdministratorsResponse>(true, cmd)).ToList().FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetCarFriendsAdministratorsModelByPkidAsync -> {pkid}", e);
                throw;
            }
        }
    }
}

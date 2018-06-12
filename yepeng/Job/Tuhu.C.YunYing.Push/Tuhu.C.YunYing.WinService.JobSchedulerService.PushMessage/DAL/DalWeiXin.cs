using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.WeiXinPush;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL
{
    public class DalWeiXin
    {
        public static string GetMaxJobOpenId()
        {
            var result = DalTemplatePush.CheckIsOpenWithDescription("logwxuser");
            return result?.Item2;
        }
        public static string GetMaxJobOpenId(int platform)
        {
            var result = DalTemplatePush.CheckIsOpenWithDescription("logwxuser" + platform);
            return result?.Item2;
        }
        public static int InsertWeiXinAuth(WXUserAuthModel model)
        {
            //            string sql = @"
            //IF NOT EXISTS ( SELECT *
            //                FROM    Tuhu_notification..WXUserAuth l
            //                WHERE   l.OpenId = @OpenId )
            //    BEGIN
            //INSERT INTO Tuhu_notification..WXUserAuth
            //        ( UserId ,
            //          Channel ,
            //          OpenId ,
            //          UnionId ,
            //          AuthSource ,
            //          Source ,
            //          BindingStatus ,
            //          AuthorizationStatus,
            //          MetaData ,
            //          CreatedTime ,
            //          UpdatedTime
            //        )
            //VALUES  ( @UserId , -- UserId - uniqueidentifier
            //          @Channel , -- Channel - nvarchar(255)
            //          @OpenId , -- OpenId - nvarchar(255)
            //          @UnionId , -- UnionId - nvarchar(255)
            //          @AuthSource , -- AuthSource - nvarchar(255)
            //          @Source , -- Source - nvarchar(255)
            //          @BindingStatus , -- BindingStatus - nvarchar(255)
            //          @AuthorizationStatus , -- BindingStatus - nvarchar(255)
            //          @MetaData , -- MetaData - nvarchar(1000)
            //          GETDATE() , -- CreatedTime - datetime
            //          GETDATE()  -- UpdatedTime - datetime
            //        );  END;";
            model.Channel = "WX_APP_OfficialAccount";
            string sql = @"
MERGE INTO Tuhu_notification..WXUserAuthForImport WITH (ROWLOCK) AS s
USING
    ( SELECT    @OpenId AS OpenId
    ) AS c
ON c.OpenId = s.OpenId
WHEN MATCHED THEN
    UPDATE SET s.UserId = @UserId ,
               s.UnionId = @UnionId ,
               s.BindingStatus = @BindingStatus ,
               s.AuthorizationStatus = @AuthorizationStatus ,
               s.MetaData = @MetaData ,
               s.UpdatedTime = GETDATE()
WHEN NOT MATCHED THEN
    INSERT ( UserId ,
             Channel ,
             OpenId ,
             UnionId ,
             AuthSource ,
             Source ,
             BindingStatus ,
             AuthorizationStatus ,
             MetaData ,
             CreatedTime ,
             UpdatedTime
           )
    VALUES ( @UserId , -- UserId - uniqueidentifier
             @Channel , -- Channel - nvarchar(255)
             @OpenId , -- OpenId - nvarchar(255)
             @UnionId , -- UnionId - nvarchar(255)
             @AuthSource , -- AuthSource - nvarchar(255)
             @Source , -- Source - nvarchar(255)
             @BindingStatus , -- BindingStatus - nvarchar(255)
             @AuthorizationStatus , -- BindingStatus - nvarchar(255)
             @MetaData , -- MetaData - nvarchar(1000)
             GETDATE() , -- CreatedTime - datetime
             GETDATE()  -- UpdatedTime - datetime
           );  ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == "PKID" || prop.Name == "CreatedTime" || prop.Name == "UpdatedTime")
                {
                    continue;
                }
                else
                {
                    var value = prop.GetValue(model);
                    parameters.Add(new SqlParameter(string.Concat("@", prop.Name), value));
                }
            }
            using (var dbhelper = DbHelper.CreateLogDbHelper(false))
            {
                var result = dbhelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, parameters);
                return result;
            }

        }

        public static int UpdateAuthLogAsync(WXUserAuthModel log)
        {
            string sql =
                @" update Tuhu_notification..WXUserAuth   WITH (ROWLOCK) 
                set BindingStatus=@BindingStatus,MetaData=@MetaData,UnionId=@UnionId,AuthorizationStatus=@AuthorizationStatus,UpdatedTime=GETDATE(),UserId=@UserId
                where OpenId=@OpenId ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@BindingStatus", log.BindingStatus));
            parameters.Add(new SqlParameter("@MetaData", log.MetaData));
            parameters.Add(new SqlParameter("@UnionId", log.UnionId));
            parameters.Add(new SqlParameter("@AuthorizationStatus", log.AuthorizationStatus));
            parameters.Add(new SqlParameter("@OpenId", log.OpenId));
            parameters.Add(new SqlParameter("@UserId", log.UserId));
            using (var dbhelper = DbHelper.CreateLogDbHelper(false))
            {
                var result = dbhelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, parameters);
                return result;
            }
        }

        public static WXUserAuthModel SelectAuthModelByOpenID(string openid)
        {
            string sql = $"SELECT TOP 1 * FROM Tuhu_notification..WXUserAuth with(nolock) WHERE OpenId='{openid}' ";
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                var result = dbhelper.ExecuteFetch<WXUserAuthModel>(sql);
                return result;
            }
        }

        public static int UpdateInvalidOpenid(string openid)
        {
            string sql =
                $"UPDATE Tuhu_notification..WXUserAuth  WITH (ROWLOCK)  SET IsSuccess=0,pushtime=GETDATE(),pushstatus='invalidopenid' WHERE OpenId='{openid}'";
            using (var dbhelper = DbHelper.CreateLogDbHelper(false))
            {
                var result = dbhelper.ExecuteNonQuery(sql);
                return result;
            }
        }
        public static IEnumerable<WXUserAuthModel> SelectPushLogs()
        {
            string sql = @" SELECT TOP 250
                         *
                         FROM    Tuhu_notification..WXUserAuth with(nolock)
            WHERE IsSuccess IS NULL
            AND OpenId IS NOT NULL
            AND UnionId IS NOT NULL
            AND BindingStatus = 'Bound'
            AND AuthorizationStatus = 'Authorized'
            and channel='WX_APP_OfficialAccount'
            ORDER BY PKID;  ";
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                var result = dbhelper.ExecuteSelect<WXUserAuthModel>(sql);
                return result;
            }
        }
    }
}

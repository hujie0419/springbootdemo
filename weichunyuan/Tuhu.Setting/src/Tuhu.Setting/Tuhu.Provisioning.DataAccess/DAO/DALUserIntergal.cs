using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using static System.Data.CommandType;


namespace Tuhu.Provisioning.DataAccess.DAO
{
   public class DALUserIntergal
    {

        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

       public List<UserIntergalMessage> PhoneSearch(string phone)
       {
           SqlParameter param = new SqlParameter("@Phone", phone);
           string sql = @"SELECT  a.UserID ,
                                  a.u_mobile_number ,
                                  b.Integral ,
                                  b.IntegralID 
                          FROM    Tuhu_profiles..UserObject AS a WITH(NOLOCK)
                                  JOIN Tuhu_profiles..tbl_UserIntegral AS b WITH(NOLOCK) ON b.UserID = a.UserID
                          WHERE   a.u_mobile_number = @Phone";
           return SqlHelper.ExecuteDataTable(connOnRead, Text, sql,param).ConvertTo<UserIntergalMessage>().ToList();
       }


        public List<UserIntergalDetail> GetUserIntegralDetail(string userId)
        {
            SqlParameter param = new SqlParameter("@UserID", userId);
            string sql = @"SELECT  a.IntegralDetailID ,
                                  a.TransactionDescribe ,
                                  a.IntegralRuleID , 
                                  a.TransactionIntegral ,
                                  a.CreateDateTime ,
                                  a.IntegralType ,
                                  a.IsActive 
                                 
                          FROM    ( SELECT TOP 1000
                                              UID.IntegralDetailID ,
                                              UID.TransactionDescribe ,
                                              UID.IntegralRuleID ,
                                              UID.TransactionIntegral ,
                                              UID.CreateDateTime ,
                                              UIR.IntegralType ,
                                              UID.IsActive
                                    FROM      Tuhu_profiles..tbl_UserIntegral AS UI WITH ( NOLOCK )
                                              JOIN Tuhu_profiles..tbl_UserIntegralDetail AS [UID] WITH( NOLOCK )
                                    ON           [UID].IntegralID =     UI.IntegralID
                                              JOIN Tuhu_profiles..tbl_UserIntegralRule AS UIR WITH ( NOLOCK ) 
                                    ON           UIR.IntegralRuleID   =       UID.IntegralRuleID
                          		 WHERE   UI.UserID = @UserID
                                   ORDER BY  UID.CreateDateTime DESC
                                  ) AS a
                                 ";

            var result = SqlHelper.ExecuteDataTable(connOnRead, Text, sql, param).ConvertTo<UserIntergalDetail>().ToList();

         
            string sql1 = @"SELECT DISTINCT IntegralDetailID  from [Tuhu_Log].[dbo].[tbl_UserIntegralDetailLog]	WITH(NOLOCK)";
            var connLog = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(connLog))
            {
                connLog = SecurityHelp.DecryptAES(connLog);
            }
            List<string> list1 = new List<string>();
            using (var dbhelper = new SqlDbHelper(connLog))
            {
                var cmd = new SqlCommand(sql1);
                cmd.CommandType = Text;
               
                var dt=dbhelper.ExecuteDataTable(cmd);
                if (dt != null && dt.Rows.Count > 0)
                    list1 = dt.AsEnumerable().Select(r => r[0].ToString()).ToList();
            }
            if (list1.Any())
            {
                foreach (var item in result)
                {
                    string detailId = list1.Find(o => o.Equals(item.IntegralDetailID.ToString()));
                    if (string.IsNullOrWhiteSpace(detailId))
                        item.id = 1;
                }
            }
            return result;
        }
        public int InsertIntergal(InsertIntergalModal insertIntergal)
       {
           #region sql

           SqlParameter[] parameters =
           {
               new SqlParameter("@IntegralDetailID", insertIntergal.IntegralDetailID),
               new SqlParameter("@IntegralID", insertIntergal.IntegralID),
               new SqlParameter("@IntegralRuleID", insertIntergal.IntegralRuleID),
               new SqlParameter("@TransactionIntegral", insertIntergal.TransactionIntegral), //增加的积分
               new SqlParameter("@TransactionRemark", insertIntergal.TransactionRemark),
               new SqlParameter("@TransactionDescribe", insertIntergal.TransactionDescribe), //增加的积分的描述原因
               new SqlParameter("@IsActive", insertIntergal.IsActive), //默认启用有效
               new SqlParameter("@TransactionChannel", insertIntergal.TransactionChannel),
               new SqlParameter("@Versions", insertIntergal.Versions)
           };
           string sql = @"INSERT  INTO Tuhu_profiles..tbl_UserIntegralDetail
                                        (IntegralDetailID,
                                            IntegralID,
                                            IntegralRuleID,
                                            TransactionIntegral,
                                            TransactionRemark,
                                            TransactionDescribe,
                                            IsActive,
                                            TransactionChannel,
                                            Versions,
                                            CreateDateTime,
                                            LasetUpdateTime
                                            )
                                            VALUES(
                                            @IntegralDetailID,
                                            @IntegralID, 
                                            @IntegralRuleID, 
                                            @TransactionIntegral, 
                                            @TransactionRemark,
                                            @TransactionDescribe,
                                            @IsActive,
                                            @TransactionChannel, 
                                            @Versions, 
                                            GETDATE(), 
                                            GETDATE()
                                            );  ";
           SqlParameter[] parameterUpdate =
           {
               new SqlParameter("@Integral", insertIntergal.Integral + insertIntergal.TransactionIntegral),
               new SqlParameter("@UserID", insertIntergal.UserId)
           };
           string sqlUpdate = @" UPDATE Tuhu_profiles..tbl_UserIntegral SET Integral=@Integral WHERE UserID=@UserID ";

           string sqlLog = @"INSERT INTO [Tuhu_Log].[dbo].[tbl_UserIntegralDetailLog]
                                                ( [IntegralDetailID] ,
                                                  [IntegralRuleID] ,
                                                  [Author] ,
                                                  [ChangeResult] ,
                                                  [Reason] ,
                                                  [CreateTime] ,
                                                  [UpdateTime] ,
                                                  [Remark]
                                                )
                                        VALUES  ( @IntegralDetailID , 
                                                  @IntegralRuleID , 
                                                  @Author , 
                                                  @ChangeResult ,
                                                  @Reason , 
                                                  GETDATE() ,
                                                  GETDATE() , 
                                                  @Remark  
                                                )";

           #endregion

           int result = 0;
           using (var dbHelper = new SqlDbHelper(strConn))
           {
               try
               {
                   dbHelper.BeginTransaction();

                   //新增用户积分详情
                   result = SqlHelper.ExecuteNonQuery(conn, Text, sql, parameters);
                   //修改用户的总积分
                   if (result > 0)
                   {
                       result = SqlHelper.ExecuteNonQuery(conn, Text, sqlUpdate, parameterUpdate);
                   }
                   dbHelper.Commit();
               }
               catch (Exception ex)
               {
                   dbHelper.Rollback();
               }
           }
           //插入日志表
           if (result > 0)
           {
               var connLog = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
               if (SecurityHelp.IsBase64Formatted(connLog))
               {
                   connLog = SecurityHelp.DecryptAES(connLog);
               }
               using (var dbhelper = new SqlDbHelper(connLog))
               {
                   var cmd = new SqlCommand(sqlLog);
                   cmd.CommandType = Text;

                   cmd.Parameters.AddWithValue("@IntegralDetailID", insertIntergal.IntegralDetailID);
                   cmd.Parameters.AddWithValue("@IntegralRuleID", insertIntergal.IntegralRuleID);
                   cmd.Parameters.AddWithValue("@Author", insertIntergal.Author);
                   cmd.Parameters.AddWithValue("@ChangeResult", insertIntergal.IsActive);
                   cmd.Parameters.AddWithValue("@Reason", insertIntergal.TransactionDescribe);
                   cmd.Parameters.AddWithValue("@Remark", insertIntergal.TransactionRemark);

                   dbhelper.ExecuteNonQuery(cmd);
               }
           }
           return result;
       }

        public List<UpdateLogModal> SelectUpdateLog(Guid detailId)
        {
            string sql = @"
                    SELECT  PKID , IntegralDetailID , Author , ChangeResult , UpdateTime , Reason
                    FROM    [Tuhu_Log].[dbo].[tbl_UserIntegralDetailLog] WITH ( NOLOCK ) 
                    WHERE   IntegralDetailID =@Id
                    ORDER BY UpdateTime DESC;";
            var connLog = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(connLog))
            {
                connLog = SecurityHelp.DecryptAES(connLog);
            }
            using (var dbhelper = new SqlDbHelper(connLog))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = Text;
                cmd.Parameters.AddWithValue("@Id", detailId);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<UpdateLogModal>().ToList();
            }
        }


        public bool CheckUser(string phone)
        {
            SqlParameter param = new SqlParameter("@Phone", phone);
            string sql = @"	SELECT  COUNT(0)
                            FROM    Tuhu_profiles..UserObject AS a WITH ( NOLOCK )
                            WHERE   a.u_mobile_number = @Phone";
            var result= (int)SqlHelper.ExecuteScalar(connOnRead, Text, sql, param);
            return result > 0;
        }
    }
}

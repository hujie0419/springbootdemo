using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;

namespace Tuhu.Provisioning.DataAccess.DAO.MemberPage
{
    public class DALMemberPageChannel
    {
        /// <summary>
        /// 根据内容ID查询渠道列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageModuleContentID"></param>
        /// <returns></returns>
        public List<MemberPageChannelModel> GetMemberPageChannelList(SqlConnection conn,long memberPageModuleContentID)
        {
            #region SQL
            string sql = @"
                            SELECT  [PKID] ,
                                    [Channel] ,
                                    [MemberPageModuleContentID] ,
                                    [Link] ,
                                    [MiniProgramAppID] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted]
                            FROM    [Configuration]..[MemberPageChannel] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0 
                            AND MemberPageModuleContentID=@MemberPageModuleContentID ";
            #endregion
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@MemberPageModuleContentID", memberPageModuleContentID);
            return conn.Query<MemberPageChannelModel>(sql, dp).ToList();
        }
        /// <summary>
        /// 新增个人中心页面配置渠道
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMemberPageChannel(SqlConnection conn,MemberPageChannelModel model)
        {
            #region SQL
            string sql = @"
                            INSERT INTO [dbo].[MemberPageChannel]
                                       ([Channel]
                                       ,[MemberPageModuleID]
                                       ,[MemberPageModuleContentID]
                                       ,[Link]
                                       ,[MiniProgramAppID])
                                 VALUES
                                       (@Channel
                                       ,@MemberPageModuleID
                                       ,@MemberPageModuleContentID
                                       ,@Link
                                       ,@MiniProgramAppID);
                            SELECT CAST(SCOPE_IDENTITY() as int);";
            #endregion
            model.PKID = conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 修改个人中心页面配置渠道
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateMemberPageChannel(SqlConnection conn,MemberPageChannelModel model)
        {
            #region SQL
            string sql = @" 
                            UPDATE [Configuration]..[MemberPageChannel] WITH(ROWLOCK)
                               SET [Channel] = @Channel
                                  ,[MemberPageModuleContentID] = @MemberPageModuleContentID
                                  ,[Link] = @Link
                                  ,[MiniProgramAppID] = @MiniProgramAppID
                                  ,[LastUpdateDateTime] =GETDATE()
                             WHERE PKID=@PKID";
            #endregion
            return conn.Execute(sql, model) > 0;
        }
        /// <summary>
        /// 删除个人中心页面配置渠道
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageModuleContentID"></param>
        /// <returns></returns>
        public bool DeleteMemberPageChannel(SqlConnection conn,long memberPageModuleContentID)
        {
            string sql = "UPDATE [Configuration]..[MemberPageChannel] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE MemberPageModuleContentID=@MemberPageModuleContentID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@MemberPageModuleContentID", memberPageModuleContentID);
            return conn.Execute(sql, dp) > 0;
        }
        /// <summary>
        /// 根据模块标识删除个人中心页面配置渠道
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageModuleID"></param>
        /// <returns></returns>
        public bool DeleteMemberPageChannelByModuleID(SqlConnection conn, long memberPageModuleID)
        {
            string sql = "UPDATE [Configuration]..[MemberPageChannel] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE MemberPageModuleID=@MemberPageModuleID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@MemberPageModuleID", memberPageModuleID);
            return conn.Execute(sql, dp) > 0;
        }
    }
}

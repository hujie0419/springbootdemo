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
    public class DALMemberPageModuleContent
    {
        /// <summary>
        /// 根据默认配置ID获取模块内容列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageID"></param>
        /// <returns></returns>
        public List<MemberPageModuleContentModel> GetMemberPageModuleContentList(SqlConnection conn, long memberPageID)
        {
            #region SQL
            string sql = @"
                            SELECT  [PKID]
                                    ,[MemberPageID]
                                    ,[MemberPageModuleID]
                                    ,[ContentName]
                                    ,[ShowType]
                                    ,[DataType]
                                    ,[SupportedChannels]
                                    ,[ImageUrl]
                                    ,[NumColor]
                                    ,[IsEnableCornerMark]
                                    ,[Title]
                                    ,[TitleColor]
                                    ,[Description]
                                    ,[DescriptionColor]
                                    ,[ButtonText]
                                    ,[ButtonTextColor]
                                    ,[ButtonColor]
                                    ,[BgColor]
                                    ,[StartVersion]
                                    ,[EndVersion]
                                    ,[DisplayIndex]
                                    ,[Status]
                                    ,[Creator]
                                    ,[CreateDateTime]
                                    ,[LastUpdateDateTime]
                                    ,[IsDeleted]
                            FROM    [Configuration]..[MemberPageModuleContent] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND MemberPageID = @MemberPageID 
                                    ORDER BY DisplayIndex ASC ";
            #endregion
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@MemberPageID", memberPageID);
            return conn.Query<MemberPageModuleContentModel>(sql, dp).ToList();
        }
        /// <summary>
        /// 获取模块内容详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public MemberPageModuleContentModel GetMemberPageModuleContentInfo(SqlConnection conn, long pkid)
        {
            #region SQL
            string sql = @"
                SELECT [PKID]
                      ,[MemberPageID]
                      ,[MemberPageModuleID]
                      ,[ContentName]
                      ,[ShowType]
                      ,[DataType]
                      ,[SupportedChannels]
                      ,[ImageUrl]
                      ,[NumColor]
                      ,[IsEnableCornerMark]
                      ,[Title]
                      ,[TitleColor]
                      ,[Description]
                      ,[DescriptionColor]
                      ,[ButtonText]
                      ,[ButtonTextColor]
                      ,[ButtonColor]
                      ,[BgColor]
                      ,[StartVersion]
                      ,[EndVersion]
                      ,[DisplayIndex]
                      ,[Status]
                      ,[Creator]
                      ,[CreateDateTime]
                      ,[LastUpdateDateTime]
                      ,[IsDeleted]
                  FROM [Configuration]..[MemberPageModuleContent] WITH(NOLOCK)
                  WHERE PKID=@PKID ";
            #endregion
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Query<MemberPageModuleContentModel>(sql, dp).FirstOrDefault();
        }
        /// <summary>
        /// 新增模块内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMemberPageModuleContent(SqlConnection conn, MemberPageModuleContentModel model)
        {
            #region SQL
            string sql = @"
                        INSERT INTO [Configuration]..[MemberPageModuleContent]
                           ([MemberPageID]
                           ,[MemberPageModuleID]
                           ,[ContentName]
                           ,[ShowType]
                           ,[DataType]
                           ,[SupportedChannels]
                           ,[ImageUrl]
                           ,[NumColor]
                           ,[IsEnableCornerMark]
                           ,[Title]
                           ,[TitleColor]
                           ,[Description]
                           ,[DescriptionColor]
                           ,[ButtonText]
                           ,[ButtonTextColor]
                           ,[ButtonColor]
                           ,[BgColor]
                           ,[StartVersion]
                           ,[EndVersion]
                           ,[DisplayIndex]
                           ,[Status]
                           ,[Creator])
                     VALUES
                           (@MemberPageID
                           ,@MemberPageModuleID
                           ,@ContentName
                           ,@ShowType
                           ,@DataType
                           ,ISNULL(@SupportedChannels,'')
                           ,@ImageUrl
                           ,@NumColor
                           ,@IsEnableCornerMark
                           ,@Title
                           ,@TitleColor
                           ,@Description
                           ,@DescriptionColor
                           ,@ButtonText
                           ,@ButtonTextColor
                           ,@ButtonColor
                           ,@BgColor
                           ,@StartVersion
                           ,@EndVersion
                           ,@DisplayIndex
                           ,@Status
                           ,@Creator);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            #endregion
            model.PKID = conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 修改模块内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateMemberPageModuleContent(SqlConnection conn, MemberPageModuleContentModel model)
        {
            #region SQL
            string sql = @"
                        UPDATE [Configuration]..[MemberPageModuleContent] WITH(ROWLOCK)
                           SET [MemberPageID] = @MemberPageID
                              ,[MemberPageModuleID] = @MemberPageModuleID
                              ,[ContentName] = @ContentName
                              ,[ShowType] = @ShowType
                              ,[DataType] = @DataType
                              ,[SupportedChannels] = ISNULL(@SupportedChannels,'')
                              ,[ImageUrl] = @ImageUrl
                              ,[NumColor] = @NumColor
                              ,[IsEnableCornerMark] = @IsEnableCornerMark
                              ,[Title] = @Title
                              ,[TitleColor] = @TitleColor
                              ,[Description] = @Description
                              ,[DescriptionColor] = @DescriptionColor
                              ,[ButtonText] = @ButtonText
                              ,[ButtonTextColor] = @ButtonTextColor
                              ,[ButtonColor] = @ButtonColor
                              ,[BgColor] = @BgColor
                              ,[StartVersion] = @StartVersion
                              ,[EndVersion] = @EndVersion
                              ,[DisplayIndex] = @DisplayIndex
                              ,[Status] = @Status
                              ,[LastUpdateDateTime] = GETDATE()
                         WHERE PKID=@PKID";
            #endregion
            return conn.Execute(sql, model) > 0;
        }
        /// <summary>
        /// 删除模块内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteMemberPageModuleContent(SqlConnection conn,long pkid)
        {
            string sql = "UPDATE [Configuration]..[MemberPageModuleContent] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Execute(sql, dp) > 0;
        }
        /// <summary>
        /// 根据模块标识删除模块内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageModuleID"></param>
        /// <returns></returns>
        public bool DeleteMemberPageModuleContentByModuleID(SqlConnection conn, long memberPageModuleID)
        {
            string sql = "UPDATE [Configuration]..[MemberPageModuleContent] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE MemberPageModuleID=@MemberPageModuleID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@MemberPageModuleID", memberPageModuleID);
            return conn.Execute(sql, dp) > 0;
        }
    }
}

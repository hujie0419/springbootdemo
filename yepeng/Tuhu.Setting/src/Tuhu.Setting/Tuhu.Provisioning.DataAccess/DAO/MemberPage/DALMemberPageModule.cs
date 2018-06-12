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
    public class DALMemberPageModule
    {
        /// <summary>
        /// 获取默认配置的模块列表
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<MemberPageModuleModel> GetMemberPageModuleList(SqlConnection conn)
        {
            #region SQL
            string sql = @"
                            SELECT  mpm.[PKID],
                                    mpm.[MemberPageID] ,
                                    mpm.[ModuleName] ,
                                    mpm.[ModuleType] ,
                                    mpm.[MarginTop] ,
                                    mpm.[DisplayIndex] ,
                                    mpm.[Status] ,
                                    mpm.[Creator] ,
                                    mpm.[CreateDateTime]
                            FROM    Configuration..MemberPageModule mpm WITH ( NOLOCK )
                                    LEFT JOIN Configuration..MemberPage mp WITH ( NOLOCK ) ON mp.PKID = mpm.MemberPageID
                            WHERE   mp.IsDefault = 1
                                    AND mp.IsDeleted = 0
                                    AND mpm.IsDeleted = 0
                                    ORDER BY mpm.DisplayIndex ASC";
            #endregion
            return conn.Query<MemberPageModuleModel>(sql).ToList();
        }
        /// <summary>
        /// 获取模块详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public MemberPageModuleModel GetMemberPageModuleInfo(SqlConnection conn,long pkid)
        {
            #region SQL
            string sql = @"
                            SELECT [PKID]
                                  ,[MemberPageID]
                                  ,[ModuleName]
                                  ,[ModuleType]
                                  ,[MarginTop]
                                  ,[DisplayIndex]
                                  ,[Status]
                                  ,[Creator]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Configuration]..[MemberPageModule] WITH(NOLOCK)
                              WHERE PKID=@PKID";
            #endregion
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Query<MemberPageModuleModel>(sql, dp).FirstOrDefault();
        }
        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMemberPageModule(SqlConnection conn, MemberPageModuleModel model)
        {
            #region SQL
            string sql = @"
                INSERT INTO [Configuration]..[MemberPageModule]
                           ([MemberPageID]
                           ,[ModuleName]
                           ,[ModuleType]
                           ,[MarginTop]
                           ,[DisplayIndex]
                           ,[Status]
                           ,[Creator])
                     VALUES
                           (@MemberPageID
                           ,@ModuleName
                           ,@ModuleType
                           ,@MarginTop
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
        /// 修改模块
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateMemberPageModule(SqlConnection conn, MemberPageModuleModel model)
        {
            #region SQL
            string sql = @"
                UPDATE [Configuration]..[MemberPageModule] WITH(ROWLOCK)
                   SET [MemberPageID] = @MemberPageID
                      ,[ModuleName] = @ModuleName
                      ,[ModuleType] = @ModuleType
                      ,[MarginTop] = @MarginTop
                      ,[DisplayIndex] = @DisplayIndex
                      ,[Status] = @Status
                      ,[LastUpdateDateTime] = GETDATE()
                 WHERE PKID=@PKID";
            #endregion
            return conn.Execute(sql, model) > 0;
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteMemberPageModule (SqlConnection conn,long pkid)
        {
            string sql = "UPDATE [Configuration]..[MemberPageModule] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Execute(sql, dp) > 0;
        }
    }
}

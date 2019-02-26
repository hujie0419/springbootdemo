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
    public class DALMemberPage
    {
        /// <summary>
        /// 根据页面编码获取页面配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageCode">页面编码，member=个人中心，more=更多</param>
        /// <returns></returns>
        public MemberPageModel GetMemberPageByPageCode(SqlConnection conn,string pageCode)
        {
            string sql = @"
                            SELECT TOP 1
	                              [PKID]
                                  ,[Name]
                                  ,[IsTemplate]
                                  ,[TemplateID]
                                  ,[DisplayIndex]
                                  ,[StartVersion]
                                  ,[EndVersion]
                                  ,[IsDefault]
                                  ,[Status]
                                  ,[Creator]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                                  ,[PageCode]
                              FROM [Configuration]..[MemberPage] WITH ( NOLOCK ) WHERE PageCode=@PageCode AND Status=1 AND IsDeleted=0 AND IsDefault=1 ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PageCode", pageCode);
            return conn.Query<MemberPageModel>(sql, dp).FirstOrDefault();
        }
    }
}

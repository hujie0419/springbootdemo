using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public class UserBlackListDAL
    {

        /// <summary>
        /// 加入黑名单
        /// </summary>
        public static bool AddBlackList(SqlConnection connection,UserBlackList model)
        {
            using (connection)
            {
                if (model!=null&&!string.IsNullOrEmpty(model.UserId))
                {
                    string strSql = @" 
                    IF((SELECT COUNT(UserId) FROM Marketing..tbl_FaXianBlackList WITH(NOLOCK) WHERE  UserId = @UserId) <= 0)
	                 BEGIN
                        INSERT INTO Marketing..tbl_FaXianBlackList(UserId,RelatedContent,BanType,Operator,CreateTime) VALUES(@UserId,@RelatedContent,@BanType,@Operator,@CreateTime)
                     END";
                    var sqlParamsInfo = new SqlParameter[]
                    {
                                    new SqlParameter("@UserId",model.UserId),
                                    new SqlParameter("@RelatedContent",model.RelatedContent),
                                    new SqlParameter("@BanType",model.BanType),
                                    new SqlParameter("@Operator",model.Operator),
                                    new SqlParameter("@CreateTime",model.CreateTime)
                    };

                    int res = SqlHelper.ExecuteNonQuery(connection, CommandType.Text, strSql, sqlParamsInfo);
                    return res > 0;
                }
                return false;             
            }
        }

        public static bool DeleteBlackList(SqlConnection connection, string UserId)
        {
            using (connection)
            {
                if (!string.IsNullOrEmpty(UserId))
                {
                    int res = SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "delete from [Marketing].[dbo].[tbl_FaXianBlackList] where [UserId]=@UserId", new SqlParameter("@UserId", UserId));
                    return res > 0;
                }
                return false;
            }
        }


    }
}

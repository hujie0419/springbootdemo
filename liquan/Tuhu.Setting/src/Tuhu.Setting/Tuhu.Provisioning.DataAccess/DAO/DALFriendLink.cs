using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALFriendLink
    {
        public static List<FriendLink> GetFrindLink(SqlConnection conn)
        {
            const string sql = @"SELECT *  FROM [Gungnir].[dbo].[tb_FriendLink] WITH (NOLOCK) ORDER BY position DESC";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<FriendLink>().ToList();
        }

        public static int InsertFriendLink(SqlConnection conn, FriendLink model)
        {
            const string sql1 =
                @"SELECT  COUNT(0) FROM [Gungnir].[dbo].[tb_FriendLink] WITH ( NOLOCK ) WHERE FriendlinkName = @FriendlinkName ";

            var sqlParam1 = new SqlParameter[]
            {
                new SqlParameter("@FriendlinkName", model.FriendLinkName)
            };

            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, sqlParam1);
            if (count > 0)
            {
                return -1;
            }

            const string sql = @"INSERT INTO [Gungnir].[dbo].[tb_FriendLink]
                                        (FriendlinkName, Link, position)
                                VALUES  (
                                         @FriendlinkName, -- FriendlinkName - varchar(50)
                                         @Link, -- Link - varchar(500)
                                         @position
                                          ); SELECT @@IDENTITY";

            var sqlParam = new SqlParameter[]
            {
                new SqlParameter("@FriendlinkName", model.FriendLinkName),
                new SqlParameter("@Link", model.Link),
                new SqlParameter("@position", model.Position)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam));
        }

        public static bool DeleteFriendLink(SqlConnection conn, int id)
        {
            const string sql = @"DELETE FROM [Gungnir].[dbo].[tb_FriendLink] WHERE Fid = @Fid";
            var sqlParam = new SqlParameter[]
           {
                new SqlParameter("@Fid", id)
           };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
        }

        public static bool UpdateFriendLink(SqlConnection conn, FriendLink model)
        {
            const string sql = @"UPDATE [Gungnir].[dbo].[tb_FriendLink] 
                                    SET FriendlinkName = @FriendlinkName,
                                        Link = @Link,   
                                        position = @position
                                    WHERE Fid=@Fid";
            var sqlParam = new SqlParameter[]
                       {
                new SqlParameter("@Fid", model.Fid),
                new SqlParameter("@FriendlinkName", model.FriendLinkName),
                new SqlParameter("@Link", model.Link),
                new SqlParameter("@position", model.Position)
                       };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
        }
    }
}

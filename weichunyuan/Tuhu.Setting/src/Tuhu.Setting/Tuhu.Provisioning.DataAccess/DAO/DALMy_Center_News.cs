using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALMy_Center_News
	{
		public static void Add(SqlConnection connection, My_Center_News _My_Center_News)
		{
			var sqlParamters = new[] 
            { 
                new SqlParameter("@UserObjectID",_My_Center_News.UserObjectID??""),
                new SqlParameter("@News",_My_Center_News.News??""),
                new SqlParameter("@Type",_My_Center_News.Type??""),
                new SqlParameter("@Titel",_My_Center_News.Titel??""),
                new SqlParameter("@OrderNo",_My_Center_News.OrderNo??"")
            };
            var sql = "INSERT INTO tbl_My_Center_News(UserObjectID,News,Type,CreateTime,Title,OrderID) VALUES(@UserObjectID,@News,@Type,GETDATE(),@Titel,@OrderNo)";
			SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
		}
	}
}

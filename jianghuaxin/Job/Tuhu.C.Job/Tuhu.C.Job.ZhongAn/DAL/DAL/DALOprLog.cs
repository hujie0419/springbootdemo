using K.DLL.Common;
using K.Domain;
using System;
using System.Data.SqlClient;

namespace K.DLL.DAL
{
    public static class DALOprLog
    {
        private static readonly string oprLogConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["OprLogConnection"].ConnectionString;
        public static void InsertOprLog(DOprLog oprLog)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@Author", oprLog.Author?? string.Empty),
                new SqlParameter("@ObjectType", oprLog.ObjectType?? string.Empty),
                new SqlParameter("@ObjectId", oprLog.ObjectID),
                new SqlParameter("@BeforeValue", oprLog.BeforeValue?? string.Empty),
                new SqlParameter("@AfterValue", oprLog.AfterValue?? string.Empty),
                new SqlParameter("@ChangeDatetime", oprLog.ChangeDatetime?? DateTime.Now),
                new SqlParameter("@IpAddress", oprLog.IPAddress?? string.Empty),
                new SqlParameter("@HostName",oprLog.HostName?? string.Empty),
				new SqlParameter("@Operation",oprLog.Operation?? string.Empty)
            };
            new DataHelper(oprLogConnectionString).ExecuteNonQuery("INSERT INTO tbl_OprLog(Author,ObjectType,ObjectID,BeforeValue,AfterValue,ChangeDatetime,IPAddress,HostName,Operation) VALUES ( @Author ,@ObjectType ,@ObjectId ,@BeforeValue ,@AfterValue ,@ChangeDatetime ,@IpAddress ,@HostName , @Operation)", sqlParamters);
        }
    }
}

using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalAirlines
    {
        //查找客服信息
        public static List<Airlines> GetAllAirlines(SqlConnection connection)
        {
            var sql = "SELECT * FROM Gungnir..tbl_Airlines WITH (NOLOCK) ORDER BY ID";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<Airlines>().ToList();
        }

        //添加客服信息
        public static void AddAirlines(SqlConnection connection, Airlines airlines)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ID",airlines.ID),
                new SqlParameter("@AirlinesName",airlines.AirlinesName),
                new SqlParameter("@CreateDateTime",airlines.CreateDateTime),
                new SqlParameter("@State",airlines.State)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, @"insert into  Gungnir..tbl_Airlines(ID,AirlinesName,CreateDateTime,State) values(@ID,@AirlinesName,@CreateDateTime,@State)", sqlParamters);
        }
        //修改客服信息
        public static void UpdateAirlines(SqlConnection connection, Airlines airlines)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ID",airlines.ID),
                new SqlParameter("@AirlinesName",airlines.AirlinesName),
                new SqlParameter("@State",airlines.State)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, @"update  Gungnir..tbl_Airlines set AirlinesName=@AirlinesName,State=@State where ID=@ID", sqlParamters);
        }

        //根据ID获取客服实体
        public static Airlines GetAirlinesByID(SqlConnection connection, string ID)
        {
            Airlines airlines=null;
            var parameters = new[]
            {
                new SqlParameter("@ID", ID)
            };
            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1*  FROM Gungnir..tbl_Airlines where ID=@ID", parameters))
            {
                if(_DR.Read())
                {
                    airlines=new Airlines();
                    //airlines.ID = _DR.GetTuhuString(0); update by renyingqiang  ID是int 
                    airlines.ID = _DR.GetTuhuValue<int>(0).ToString();
                    airlines.AirlinesName = _DR.GetTuhuString(1);
                    airlines.CreateDateTime = _DR.GetDateTime(2);
                    airlines.State = _DR.GetTuhuValue<int>(3);
                }
            }
            return airlines;
        }

        //删除客服信息
        public static void DeleteAirlines(SqlConnection connection, string ID)
        {
            var sqlParamters = new[] 
            { 
                new SqlParameter("@ID",ID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Gungnir..tbl_Airlines WHERE ID=@ID", sqlParamters);
        }
    }
}

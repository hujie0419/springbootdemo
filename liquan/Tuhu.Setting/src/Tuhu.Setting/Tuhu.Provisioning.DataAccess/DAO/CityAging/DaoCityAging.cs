using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.CityAging
{
    public class DaoCityAging
    {
        /// <summary>
        /// 查询城市时效
        /// </summary>
        /// <returns></returns>
        public static List<CityAgingModel> SelectCityAgingInfo()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sql = @"SELECT PKid,
                           CityId,
                           CityName,
                           IsShow,
                           Title,
                           Content,
                           CreateDateTime,
                           CreateUser,
                           LastUpdateDateTime,
                           UpdateUser,Disabled FROM Configuration..[CityAgingConfig] WITH (nolock) WHERE Disabled=0 ";
                return dbHelper.ExecuteDataTable(sql).ConvertTo<CityAgingModel>().ToList();
            }
        }


        /// <summary>
        /// 查询城市时效
        /// </summary>
        /// <returns></returns>
        public static List<CityAgingModel> SelectCityAgingInfoByIds(List<int> Ids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sqlIds = string.Join(",", Ids);
                var sql = $@"SELECT PKid,
                           CityId,
                           CityName,
                           IsShow,
                           Title,
                           Content,
                           CreateDateTime,
                           CreateUser,
                           LastUpdateDateTime,
                           UpdateUser,Disabled FROM Configuration..[CityAgingConfig] WITH (nolock) WHERE Disabled=0 and PKid in ({sqlIds})";
                return dbHelper.ExecuteDataTable(sql).ConvertTo<CityAgingModel>().ToList();
            }
        }


        /// <summary>
        /// 新增城市时效
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="cityName"></param>
        /// <param name="isShow"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="createUser"></param>
        /// <returns></returns>
        public static int CreateSelectCityAging(int cityId, string cityName, int isShow, string title, string content, string createUser)
        {
            var IsShowSql = isShow == -1 ? "" : "IsShow,";
            var titleSql = title == "-1" ? "" : "Title,";
            var contentSql = content == "-1" ? "" : "Content,";
            var IsShowSqlPara = isShow == -1 ? "" : "@IsShow,";
            var titleSqlPara = title == "-1" ? "" : "@Title,";
            var contentSqlPara = content == "-1" ? "" : "@Content,";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var sql = $@"INSERT INTO [Configuration]..[CityAgingConfig]
                            (
                                [CityId],
                                [CityName],
                                {IsShowSql}
                                {titleSql}
                                {contentSql}
                                [CreateDateTime],
                                [CreateUser],
                                [Disabled]
                            )
                            VALUES
                            (@CityId, @CityName, {IsShowSqlPara} {titleSqlPara} {contentSqlPara} GETDATE(), @CreateUser,0)";
                return Convert.ToInt32(dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[]
                {
                    new SqlParameter("@CityId",cityId),
                    new SqlParameter("@CityName", cityName),
                    new SqlParameter("@IsShow", isShow),
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Content",content),
                    new SqlParameter("@CreateUser", createUser)
                }));

            }
        }


        /// <summary>
        /// 更新城市时效
        /// </summary>
        /// <param name="pKid"></param>
        /// <param name="isShow"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public static int UpdateSelectCityAging(int pKid, int isShow, string title, string content, string updateUser)
        {
            var IsShowSql = isShow == -1 ? "" : "[IsShow] = @IsShow,";
            var titleSql = title == "-1" ? "" : "Title = @Title,";
            var contentSql = content == "-1" ? "" : " [Content] = @Content,";

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sql = $@"UPDATE [Configuration]..[CityAgingConfig]
                            SET {IsShowSql}
                                {titleSql}
                                {contentSql}
                            UpdateUser = @UpdateUser,
                            LastUpdateDateTime = GETDATE()
                            WHERE PKid = @PKid";
                return Convert.ToInt32(dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[]
                {
                    new SqlParameter("@PKid",pKid),
                    new SqlParameter("@IsShow", isShow),
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Content",content),
                    new SqlParameter("@UpdateUser", updateUser)
                }));

            }
        }
    }
}

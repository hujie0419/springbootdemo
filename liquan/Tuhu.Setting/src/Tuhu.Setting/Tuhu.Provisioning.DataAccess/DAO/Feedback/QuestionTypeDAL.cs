using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.Feedback;

namespace Tuhu.Provisioning.DataAccess.DAO.Feedback
{
    public class QuestionTypeDAL
    {

        /// <summary>
        /// 获取问题类型集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QuestionTypeEntity> GetQuestionTypeList()
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = "SELECT Id ,TypeName,Description FROM Tuhu_profiles..QuestionType WITH(NOLOCK) WHERE IsDelete=0 ORDER BY Id DESC";
                return sqlHelper.ExecuteDataTable(sqlStr, CommandType.Text).ConvertTo<QuestionTypeEntity>();
            }
        }

        /// <summary>
        /// 查找是否存在指定的问题类型
        /// </summary>
        /// <param name="typeName">指定问题类型</param>
        /// <returns></returns>
        public async Task<bool> GetQuestionTypeEntityByTypeName(string typeName)
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = "SELECT Id ,TypeName FROM Tuhu_profiles..QuestionType WITH(NOLOCK) WHERE IsDelete=0 AND TypeName=@typeName";
                var sqlparameters = new DbParameter[]
                {
                        new SqlParameter("@typeName", typeName)
                };
                return Convert.ToInt32(await sqlHelper.ExecuteScalarAsync(sqlStr, CommandType.Text, sqlparameters)) > 0;
            }
        }

        /// <summary>
        /// 添加问题类型（先判断是否存在此问题类型，不存在，则添加）
        /// </summary>
        /// <returns>存在此问题类型：-1，成功：返回添加数据生成的Id，失败：0</returns>
        public async Task<int> AddQuestionType(string typeName, string describtion, string currentUserName)
        {
            var haveQuestionType = await GetQuestionTypeEntityByTypeName(typeName);
            if (haveQuestionType)
                return -1;
            using (var sqlHelper = new SqlDbHelper())
            {
                var sql = "INSERT  Tuhu_profiles..QuestionType (TypeName,Description,CreateName,CreateDate) VALUES (@typeName,@describtion,@currentUserName,GETDATE())";
                var sqlparameters = new DbParameter[]
                {
                        new SqlParameter("@typeName", typeName),
                        new SqlParameter("@describtion", describtion),
                        new SqlParameter("@currentUserName", currentUserName)
                };
                var result = await sqlHelper.ExecuteScalarAsync(sql + ";select SCOPE_IDENTITY() as id", CommandType.Text, sqlparameters);
                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return 0;
            }
        }
        /// <summary>
        /// 更新问题类型
        /// </summary>
        /// <returns>成功：返回1，失败：0</returns>
        public async Task<int> UpdateQuestionType(string typeName, string describtion, string currentUserName, int id)
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sql = "Update  Tuhu_profiles..QuestionType SET  TypeName=@typeName,Description=@describtion,CreateName=@currentUserName,CreateDate=GETDATE() WHERE Id=@Id";
                var sqlparameters = new DbParameter[]
                {
                        new SqlParameter("@typeName", typeName),
                        new SqlParameter("@describtion", describtion),
                        new SqlParameter("@currentUserName", currentUserName),
                        new SqlParameter("@Id", id)
                };
                var result = await sqlHelper.ExecuteNonQueryAsync(sql, CommandType.Text, sqlparameters);
                return result;
            }
        }
        /// <summary>
        /// 通过id删除问题类型
        /// </summary>
        /// <param name="id">问题类型Id</param>
        /// <returns></returns>
        public async Task<int> DeleteQuestionType(int id)
        {
            using (var sqlHelper = new SqlDbHelper())
            {
                var sqlStr = "Update Tuhu_profiles..QuestionType SET IsDelete=1 WHERE Id=@Id";
                DbParameter[] parameter = new DbParameter[] { new SqlParameter("@Id", id) };
                return await sqlHelper.ExecuteNonQueryAsync(sqlStr, CommandType.Text, parameter);
            }
        }
    }
}

using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 微信生成二维码管理
    /// </summary>
    public class QRCodeManageDAL
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SqlConnection sqlconn, QRCodeManageModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Configuration..SE_QRCodeManageConfig(");
            strSql.Append("ChannelName,QRCodeType,QRCodeUrl,ValidityTime,CreateTime,IsShow,TraceId,ResponseContent)");
            strSql.Append(" values (");
            strSql.Append("@ChannelName,@QRCodeType,@QRCodeUrl,@ValidityTime,@CreateTime,@IsShow,@TraceId,@ResponseContent)");
            SqlParameter[] parameters = {
                    new SqlParameter("@ChannelName", SqlDbType.NVarChar),
                    new SqlParameter("@QRCodeType", SqlDbType.Int),
                    new SqlParameter("@QRCodeUrl", SqlDbType.NVarChar),
                    new SqlParameter("@ValidityTime", SqlDbType.NVarChar),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime),
                    new SqlParameter("@IsShow", SqlDbType.Int),
                    new SqlParameter("@TraceId", SqlDbType.Int),
                    new SqlParameter("@ResponseContent",SqlDbType.VarChar)
            };
            parameters[0].Value = model.ChannelName;
            parameters[1].Value = model.QRCodeType;
            parameters[2].Value = model.QRCodeUrl;
            parameters[3].Value = model.ValidityTime;
            parameters[4].Value = model.CreateTime;
            parameters[5].Value = model.IsShow;
            parameters[6].Value = model.TraceId;
            parameters[7].Value = model.ResponseContent;

            return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SqlConnection sqlconn, QRCodeManageModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Configuration..SE_QRCodeManageConfig set ");
            strSql.Append("ChannelName=@ChannelName,");
            strSql.Append("QRCodeType=@QRCodeType,");
            strSql.Append("QRCodeUrl=@QRCodeUrl,");
            strSql.Append("ValidityTime=@ValidityTime,");
            strSql.Append("CreateTime=@CreateTime,");
            strSql.Append("IsShow=@IsShow,");
            strSql.Append("TraceId=@TraceId,");
            strSql.Append("ResponseContent=@ResponseContent");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
                    new SqlParameter("@ChannelName", SqlDbType.NVarChar),
                    new SqlParameter("@QRCodeType", SqlDbType.Int),
                    new SqlParameter("@QRCodeUrl", SqlDbType.NVarChar),
                    new SqlParameter("@ValidityTime", SqlDbType.NVarChar),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime),
                    new SqlParameter("@IsShow", SqlDbType.Int),
                    new SqlParameter("@Id", SqlDbType.Int),
                    new SqlParameter("@TraceId", SqlDbType.Int),
                    new SqlParameter("@ResponseContent",SqlDbType.VarChar)
            };
            parameters[0].Value = model.ChannelName;
            parameters[1].Value = model.QRCodeType;
            parameters[2].Value = model.QRCodeUrl;
            parameters[3].Value = model.ValidityTime;
            parameters[4].Value = model.CreateTime;
            parameters[5].Value = model.IsShow;
            parameters[6].Value = model.Id;
            parameters[7].Value = model.TraceId;
            parameters[8].Value = model.ResponseContent;

            return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Delete(SqlConnection sqlconn, int Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Configuration..SE_QRCodeManageConfig ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", SqlDbType.Int,4)
            };
            parameters[0].Value = Id;

            return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        public static QRCodeManageModel GetModel(SqlConnection sqlconn, int id)
        {
            string sql = @"SELECT * FROM Configuration..SE_QRCodeManageConfig WITH(NOLOCK) WHERE ID = @ID";

            var sqlParas = new SqlParameter[] {
                new SqlParameter("@ID",id)
            };

            return SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, sql, sqlParas).ConvertTo<QRCodeManageModel>().FirstOrDefault();
        }

        /// <summary>
        /// 检查是否存在TraceId
        /// </summary>
        /// <param name="traceId"></param>
        /// <returns></returns>
        public static bool CheckedTraceId(SqlConnection sqlconn, int? traceId = 0, int? id = 0)
        {
            List<SqlParameter> sqlParas = new List<SqlParameter>();
            string sql = @"SELECT top 1 * FROM Configuration..SE_QRCodeManageConfig WITH(NOLOCK) WHERE TraceId = @TraceId ";

            sqlParas.Add(new SqlParameter("@TraceId", traceId));

            if (id > 0)
            {
                sql += " AND ID <> @id ";
                sqlParas.Add(new SqlParameter("@id", id));
            }

            object result = SqlHelper.ExecuteScalar(sqlconn, CommandType.Text, sql, sqlParas.ToArray());

            if (result == null)
                return false;

            return (int)result > 0;
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public static IEnumerable<QRCodeManageModel> GetListByPage(SqlConnection sqlconn, string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby))
                strSql.Append("order by T." + orderby.Trim());
            else
                strSql.Append("order by T.Id desc");

            strSql.Append(")AS Row, T.*  from Configuration..SE_QRCodeManageConfig T WITH(NOLOCK) ");

            if (!string.IsNullOrEmpty(strWhere))
                strSql.Append(" WHERE " + strWhere.Trim());

            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);

            return SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, strSql.ToString(), null).ConvertTo<QRCodeManageModel>();
        }
    }
}

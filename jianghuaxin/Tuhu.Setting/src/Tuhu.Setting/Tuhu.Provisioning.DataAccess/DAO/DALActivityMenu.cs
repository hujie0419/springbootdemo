using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 活动菜单
    /// </summary>
  public  class DALActivityMenu
    {

        public static bool Add(ActivityMenu model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ActivityMenu(");
            strSql.Append("ActivityID,MenuName,MenuValue,Sort,Color,MenuType)");
            strSql.Append(" values (");
            strSql.Append("@ActivityID,@MenuName,@MenuValue,@Sort,@Color,@MenuType)");
            SqlParameter[] parameters = {
                    new SqlParameter("@ActivityID", SqlDbType.Int,4),
                    new SqlParameter("@MenuName", SqlDbType.NVarChar,50),
                    new SqlParameter("@MenuValue", SqlDbType.NVarChar,50),
                    new SqlParameter("@Sort", SqlDbType.Int,4),
                    new SqlParameter("@Color", SqlDbType.NVarChar,30),
                    new SqlParameter("@MenuType",SqlDbType.Int,4)
            };
            parameters[0].Value = model.ActivityID;
            parameters[1].Value = model.MenuName;
            parameters[2].Value = model.MenuValue;
            parameters[3].Value = model.Sort;
            parameters[4].Value = model.Color;
            parameters[5].Value = model.MenuType;

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                return db.ExecuteNonQuery(cmd)>0;
            }
           
        }


        /// <summary>
		/// 更新一条数据
		/// </summary>
		public static bool Update(ActivityMenu model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ActivityMenu set ");
            strSql.Append("ID=@ID,");
            strSql.Append("ActivityID=@ActivityID,");
            strSql.Append("MenuName=@MenuName,");
            strSql.Append("MenuValue=@MenuValue,");
            strSql.Append("Sort=@Sort,");
            strSql.Append("Color=@Color");
            strSql.Append(" where ID=@ID and ActivityID=@ActivityID and MenuName=@MenuName and MenuValue=@MenuValue and Sort=@Sort and Color=@Color ");
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@ActivityID", SqlDbType.Int,4),
                    new SqlParameter("@MenuName", SqlDbType.NVarChar,50),
                    new SqlParameter("@MenuValue", SqlDbType.NVarChar,10),
                    new SqlParameter("@Sort", SqlDbType.Int,4),
                    new SqlParameter("@Color", SqlDbType.NVarChar,30)};
            parameters[0].Value = model.ID;
            parameters[1].Value = model.ActivityID;
            parameters[2].Value = model.MenuName;
            parameters[3].Value = model.MenuValue;
            parameters[4].Value = model.Sort;
            parameters[5].Value = model.Color;

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                return db.ExecuteNonQuery(cmd)>0;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Delete(int ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ActivityMenu ");
            strSql.Append(" where  ActivityID=@ActivityID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@ActivityID", SqlDbType.Int,4)
                           };
            parameters[0].Value = ID;

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                return db.ExecuteNonQuery(cmd) > 0;
            }

        }


        public static DataTable GetTable(string activityID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT * FROM Gungnir..ActivityMenu WITH(NOLOCK) WHERE ActivityID=@ActivityID");
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityID);

                return db.ExecuteDataTable(cmd);
            }

        }


        public static DataRow GetDataRow(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT * FROM Gungnir..ActivityMenu WITH(NOLOCK) WHERE ID=@ID ");
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(strSql.ToString());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);

                return db.ExecuteDataRow(cmd);
            }
        }


       


    }
}

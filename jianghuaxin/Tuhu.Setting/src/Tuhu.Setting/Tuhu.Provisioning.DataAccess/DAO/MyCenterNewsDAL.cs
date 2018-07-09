using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class MyCenterNewsDAL
    {
        public static bool Insert(SqlConnection connection, MyCenterNewsModel model)
        {
            if (model != null)
            {
                string strSql = " INSERT INTO Gungnir..tbl_My_Center_News({0}) VALUES({1}) ";

                Dictionary<string, object> dicParams = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(model.UserObjectID)) { dicParams.Add("UserObjectID", model.UserObjectID); }
                if (!string.IsNullOrEmpty(model.News)) { dicParams.Add("News", model.News); }
                if (!string.IsNullOrEmpty(model.Type)) { dicParams.Add("Type", model.Type); }
                if (DateTime.Compare(model.CreateTime, DateTime.MinValue) > 0) { dicParams.Add("CreateTime", model.CreateTime); }
                if (DateTime.Compare(model.UpdateTime, DateTime.MinValue) > 0) { dicParams.Add("UpdateTime", model.UpdateTime); }
                if (!string.IsNullOrEmpty(model.Title)) { dicParams.Add("Title", model.Title); }
                if (!string.IsNullOrEmpty(model.HeadImage)) { dicParams.Add("HeadImage", model.HeadImage); }
                if (!string.IsNullOrEmpty(model.IOSKey)) { dicParams.Add("IOSKey", model.IOSKey); }
                if (!string.IsNullOrEmpty(model.IOSValue)) { dicParams.Add("IOSValue", model.IOSValue); }
                if (!string.IsNullOrEmpty(model.androidKey)) { dicParams.Add("androidKey", model.androidKey); }
                if (!string.IsNullOrEmpty(model.androidValue)) { dicParams.Add("androidValue", model.androidValue); }
                //if (DateTime.Compare(model.BeginShowTime, DateTime.MinValue) > 0) { dicParams.Add("BeginShowTime", model.BeginShowTime); }
                dicParams.Add("isdelete", model.isdelete);

                StringBuilder strSqlField = new StringBuilder();                //字段
                StringBuilder strSqlWhere = new StringBuilder();                //条件
                List<SqlParameter> sqlParams = new List<SqlParameter>();        //参数值

                for (int i = 0; i < dicParams.Count; i++)
                {
                    var dicKey = dicParams.ElementAt(i).Key;
                    var dicValue = dicParams.ElementAt(i).Value;
                    sqlParams.Add(new SqlParameter("@" + dicKey, dicValue));

                    strSqlField.AppendFormat("{0},", dicKey);
                    strSqlWhere.AppendFormat("{0},", "@" + dicKey);
                }
                strSql = string.Format(strSql, strSqlField.ToString().TrimEnd(','), strSqlWhere.ToString().TrimEnd(','));
                return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, strSql, sqlParams.ToArray()) > 0 ? true : false;
            }
            return false;
        }
    }
}
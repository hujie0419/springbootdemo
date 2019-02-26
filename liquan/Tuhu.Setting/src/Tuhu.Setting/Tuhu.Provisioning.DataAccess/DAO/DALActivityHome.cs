using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public class DALActivityHome
    {

        public DALActivityHome() { }

        public  static  bool Add(ActivityHome model)
        {
            string sql = @"INSERT INTO Gungnir..ActivityHome
                ( ActivityID ,
                  Icon ,
                  Name ,
                  AppUrl ,
                  WXUrl ,
                  WwwUrl ,
                  Sort
                )
            VALUES  ( @ActivityID , 
                     @Icon ,
                     @Name , 
                     @AppUrl , 
                      @WXUrl , 
                     @WwwUrl , 
                     @Sort 
                    )
            ";

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                cmd.Parameters.AddWithValue("@Icon",model.Icon);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@AppUrl",model.AppUrl);
                cmd.Parameters.AddWithValue("@WXUrl",model.WXUrl);
                cmd.Parameters.AddWithValue("@WwwUrl",model.WwwUrl);
                cmd.Parameters.AddWithValue("@Sort",model.Sort);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }



        public static bool Delete(int activityID)
        {
            string sql = @"DELETE FROM Gungnir..ActivityHome WHERE ActivityID=@ActivityID ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }


        public static DataTable GetList(int activityID)
        {
            string sql = @" SELECT * FROM Gungnir..ActivityHome WHERE ActivityID=@ActivityID ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                return db.ExecuteDataTable(cmd);
            }

        }


    }
}

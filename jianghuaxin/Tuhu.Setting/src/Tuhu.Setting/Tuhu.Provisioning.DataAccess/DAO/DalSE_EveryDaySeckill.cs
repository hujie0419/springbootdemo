using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using System.Data;


namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 
    /// </summary>
    public class DalSE_EveryDaySeckill
    {

        public bool Add(SE_EveryDaySeckill model)
        {
            bool result = false;
            string sql = @"INSERT INTO Configuration.dbo.SE_EveryDaySeckill
        ( ActivityGuid ,
          ActivityName ,
          StartDate ,
          EndDate ,
          CreateDate ,
          UpdateDate
        )
VALUES  ( @ActivityGuid , -- ActivityGuid - uniqueidentifier
          @ActivityName , -- ActivityName - nvarchar(50)
          @StartDate , -- StartDate - datetime
          @EndDate , -- EndDate - datetime
          @CreateDate , -- CreateDate - datetime
          @UpdateDate  -- UpdateDate - datetime
        ) ";
            var db = DbHelper.CreateDefaultDbHelper();

            db.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityGuid", model.ActivityGuid);
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);

                db.ExecuteNonQuery(cmd);

                sql = @"INSERT INTO Configuration.dbo.SE_EveryDaySeckillInfo
                            ( FK_EveryDaySeckill ,
                              SpecialUrl ,
                              SpecialPicture ,
                              SpecialStyle ,
                              ActivityContent ,
                              FlashActivityGuid ,
                              CreateDate ,
                              UpdateDate
                            )
                    VALUES  ( @FK_EveryDaySeckill , -- FK_EveryDaySeckill - uniqueidentifier
                              @SpecialUrl , -- SpecialUrl - nvarchar(200)
                              @SpecialPicture , -- SepecialPicture - nvarchar(max)
                              @SpecialStyle , -- SepecialStyle - int
                              @ActivityContent , -- ActivityContent - nvarchar(max)
                              @FlashActivityGuid , -- FlashActivityGuid - uniqueidentifier
                              @CreateDate , -- CreateDate - datetime
                              @UpdateDate  -- UpdateDate - datetime
                            )";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FK_EveryDaySeckill", model.ActivityGuid);
                cmd.Parameters.AddWithValue("@SpecialUrl", model.EveryDaySeckillInfo.SpecialUrl);
                cmd.Parameters.AddWithValue("@SpecialPicture", model.EveryDaySeckillInfo.SpecialPicture);
                cmd.Parameters.AddWithValue("@SpecialStyle", model.EveryDaySeckillInfo.SpecialStyle);
                cmd.Parameters.AddWithValue("@ActivityContent", model.EveryDaySeckillInfo.ActivityContent);
                //cmd.Parameters.AddWithValue("@ActivityCoupon", model.EveryDaySeckillInfo.ActivityCoupon);
                cmd.Parameters.AddWithValue("@FlashActivityGuid", model.EveryDaySeckillInfo.FlashActivityGuid);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                db.ExecuteNonQuery(cmd);
                db.Commit();
                result = true;

            }
            catch (Exception em)
            {
                db.Rollback();

            }
            finally
            {
                db.Dispose();
            }
            return result;
        }


        public bool Update(SE_EveryDaySeckill model)
        {
            bool result = false;
            string sql = @"UPDATE Configuration.dbo.SE_EveryDaySeckill SET ActivityName=@ActivityName,StartDate=@StartDate,EndDate=@EndDate ,UpdateDate=@UpdateDate where ActivityGuid=@ActivityGuid ";
            var db = DbHelper.CreateDefaultDbHelper();

            try
            {
                db.BeginTransaction();

                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityGuid",model.ActivityGuid);
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);

                db.ExecuteNonQuery(cmd);

                sql = @"UPDATE Configuration.dbo.SE_EveryDaySeckillInfo SET SpecialUrl=@SpecialUrl, SpecialPicture=@SpecialPicture ,SpecialStyle=@SpecialStyle ,ActivityContent=@ActivityContent,FlashActivityGuid=@FlashActivityGuid , UpdateDate=@UpdateDate where FK_EveryDaySeckill=@FK_EveryDaySeckill";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@SpecialUrl", model.EveryDaySeckillInfo.SpecialUrl);
                cmd.Parameters.AddWithValue("@SpecialPicture", model.EveryDaySeckillInfo.SpecialPicture);
                cmd.Parameters.AddWithValue("@SpecialStyle", model.EveryDaySeckillInfo.SpecialStyle);
                cmd.Parameters.AddWithValue("@ActivityContent", model.EveryDaySeckillInfo.ActivityContent);
               // cmd.Parameters.AddWithValue("@ActivityCoupon", model.EveryDaySeckillInfo.ActivityCoupon);
                cmd.Parameters.AddWithValue("@FlashActivityGuid", model.EveryDaySeckillInfo.FlashActivityGuid);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@FK_EveryDaySeckill",model.ActivityGuid);
                db.ExecuteNonQuery(cmd);
                db.Commit();
                result = true;
            }
            catch (Exception em)
            {
                db.Rollback();
                result = false;
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }


        public bool Delete(Guid ActivityGuid)
        {
            bool result = false;
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Configuration.dbo.SE_EveryDaySeckill WHERE ActivityGuid=@ActivityGuid ");
                    cmd.Parameters.AddWithValue("@ActivityGuid", ActivityGuid);
                    db.ExecuteNonQuery(cmd);
                    cmd = new SqlCommand("DELETE FROM Configuration.dbo.SE_EveryDaySeckillInfo WHERE FK_EveryDaySeckill=@FK_EveryDaySeckill ");
                    cmd.Parameters.AddWithValue("@FK_EveryDaySeckill", ActivityGuid);
                    db.ExecuteNonQuery(cmd);
                    db.Commit();
                    result = true;

                }
                catch (Exception em)
                {
                    result = false;
                    db.Rollback();
                }
                return result;
            }
        }


        public DataTable GetEntity(Guid activityGuid)
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_EveryDaySeckill (NOLOCK) WHERE ActivityGuid=@ActivityGuid ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityGuid", activityGuid);
                return db.ExecuteDataTable(cmd);
            }
        }


        public DataTable GetEntityDataTable(Guid activityGuid)
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_EveryDaySeckillInfo (NOLOCK) WHERE  FK_EveryDaySeckill=@FK_EveryDaySeckill  ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FK_EveryDaySeckill", activityGuid);
                return db.ExecuteDataTable(cmd);
            }
        }

        public DataTable GetList()
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_EveryDaySeckill (NOLOCK)  ORDER BY StartDate DESC ";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                return db.ExecuteDataTable(sql);
            }
        }

        public DataTable GetListByWhere(string whereString, SqlParameter [] parameter )
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_EveryDaySeckill (NOLOCK) where 1=1 " + whereString + "  ORDER BY StartDate DESC ";

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                if (parameter != null && parameter.Length != 0)
                {
                    foreach (var p in parameter)
                        cmd.Parameters.Add(p);
                }

                DataTable dt = db.ExecuteDataTable(cmd);
                if (dt == null || dt.Rows.Count <= 0)
                    return null;
                return dt;
            }
        }

        /// <summary>
        /// 判断时间是否合法
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public bool Exits(DateTime startDateTime, DateTime endDateTime)
        {
            return (int) DbHelper.ExecuteScalar("SELECT COUNT(*) FROM Configuration.dbo.SE_EveryDaySeckill WHERE StartDate =@StartDate OR EndDate=@EndDate  ", CommandType.Text, new SqlParameter(){
                  ParameterName= "@StartDate",
                  Value=startDateTime
            },
            new SqlParameter() {
                 ParameterName= "@EndDate",
                 Value=endDateTime
            })>0;
        }


    }
}

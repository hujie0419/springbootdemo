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
   public static class DALSE_Activity
    {

        public static bool Add(SE_Activity model)
        {
            bool result = false;
            string sql = @"INSERT INTO Configuration.dbo.SE_ActivityConfig
        ( ID ,
          Title ,
          CreateDT ,
          UpdateDT ,
          BgImageUrl ,
          BgColor
        )
VALUES  ( @ID , -- ID - uniqueidentifier
          @Title , -- Title - nvarchar(50)
          GETDATE() , -- CreateDT - datetime
          GETDATE() , -- UpdateDT - datetime
          @BgImageUrl , -- BgImageUrl - nvarchar(500)
          @BgColor  -- BgColor - nvarchar(50)
        )";
            var db = DbHelper.CreateDefaultDbHelper();
            Guid ID = Guid.NewGuid();
            model.ID = ID;
            try
            {
                db.BeginTransaction();
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID", ID.ToString());
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@BgImageUrl", model.BgImageUrl);
                cmd.Parameters.AddWithValue("@BgColor", model.BgColor);

                db.ExecuteNonQuery(cmd);

                foreach (var item in model.Items)
                {
                    item.FK_Activity = ID;
                    AddDeatil(db, item);
                }
                db.Commit();
                result = true;
            }
            catch (Exception e)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;

        }

        public static bool Update(SE_Activity model)
        {
            bool result = false;
            string sql = @"UPDATE Configuration.dbo.SE_ActivityConfig SET Title=@Title,UpdateDT=GETDATE(),BgImageUrl=@BgImageUrl, BgColor=@BgColor WHERE ID=@ID";
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                db.BeginTransaction();
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID", model.ID.ToString());
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@BgImageUrl", model.BgImageUrl);
                cmd.Parameters.AddWithValue("@BgColor", model.BgColor);

                db.ExecuteNonQuery(cmd);

                DeteleDeatil(db, model.ID.ToString());

                foreach (var item in model.Items)
                {
                    item.FK_Activity = model.ID;
                    AddDeatil(db, item);
                }
                db.Commit();
                result = true;
            }
            catch (Exception e)
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


        public static bool Delete(string id)
        {
            bool result = false;
            string sql = @"DELETE FROM Configuration.dbo.SE_ActivityConfig WHERE ID=@ID";
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id);
                db.ExecuteNonQuery(cmd);
                DeteleDeatil(db, id);
                db.Commit();
                result = true;
            }
            catch (Exception e)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }

        public static void AddDeatil(AsyncDbHelper db, SE_ActivityDeatil model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_ActivityDeatilConfig
        (FK_Activity,
          [GROUP],
          PID,
          ActivityFlashID,
          CouponID,
          SmallImage,
          BigImage,
          ColunmNumber,
          [Type],
          OrderBy,
          AppUrl,
          WapUrl,
          PCUrl,
          HandlerIOS,
          HandlerAndroid,
          SOAPIOS,
          SOAPAndroid,
          IsImage,
          DisplayWay,
          [Description],
         ProductName,ActivityPrice
        )
VALUES(@FK_Activity, --FK_Activity - uniqueidentifier
          @GROUP, --GROUP - nvarchar(50)
          @PID, --PID - nvarchar(50)
          @ActivityFlashID, --ActivityFlashID - nvarchar(100)
          @CouponID, --CouponID - int
          @SmallImage, --SmallImage - nvarchar(200)
          @BigImage, --BigImage - nvarchar(200)
          @ColunmNumber, --ColunmNumber - int
          @Type, --Type - int
          @OrderBy, --OrderBy - int
          @AppUrl, --AppUrl - nvarchar(100)
          @WapUrl, --WapUrl - nvarchar(100)
          @PCUrl, --PCUrl - nvarchar(100)
          @HandlerIOS, --HandlerIOS - nvarchar(200)
          @HandlerAndroid, --HandlerAndroid - nvarchar(200)
          @SOAPIOS, --SOAPIOS - nvarchar(200)
          @SOAPAndroid, --SOAPAndroid - nvarchar(200)
          @IsImage, --IsImage - int
          @DisplayWay, --DisplayWay - int
          @Description, -- Description - text
          @ProductName,
          @ActivityPrice
        )
";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@FK_Activity", model.FK_Activity.ToString());
            cmd.Parameters.AddWithValue("@GROUP",model.GROUP);
            cmd.Parameters.AddWithValue("@PID",model.PID);
            cmd.Parameters.AddWithValue("@ActivityFlashID",model.ActivityFlashID);
            cmd.Parameters.AddWithValue("@CouponID",model.CouponID);
            cmd.Parameters.AddWithValue("@SmallImage",model.SmallImage);
            cmd.Parameters.AddWithValue("@BigImage",model.BigImage);
            cmd.Parameters.AddWithValue("@ColunmNumber",model.ColunmNumber);
            cmd.Parameters.AddWithValue("@Type",model.Type);
            cmd.Parameters.AddWithValue("@OrderBy",model.OrderBy);
            cmd.Parameters.AddWithValue("@AppUrl",model.AppUrl);
            cmd.Parameters.AddWithValue("@WapUrl",model.WapUrl);
            cmd.Parameters.AddWithValue("@PCUrl",model.PCUrl);
            cmd.Parameters.AddWithValue("@HandlerIOS",model.HandlerIOS);
            cmd.Parameters.AddWithValue("@HandlerAndroid",model.HandlerAndroid);
            cmd.Parameters.AddWithValue("@SOAPIOS",model.SOAPIOS);
            cmd.Parameters.AddWithValue("@SOAPAndroid",model.SOAPAndroid);
            cmd.Parameters.AddWithValue("@IsImage",model.IsImage);
            cmd.Parameters.AddWithValue("@DisplayWay",model.DisplayWay);
            cmd.Parameters.AddWithValue("@Description",model.Description);
            cmd.Parameters.AddWithValue("@ProductName",model.ProductName);
            cmd.Parameters.AddWithValue("@ActivityPrice",model.ActivityPrice);
            db.ExecuteNonQuery(cmd);
        }

        public static void UpdateDeatil(AsyncDbHelper db, SE_ActivityDeatil model)
        {
            string sql = @"UPDATE Configuration].[dbo].[SE_ActivityDeatil]
   SET 
      [GROUP] =@GROUP
      ,[PID] = @PID
      ,[ActivityFlashID] = @ActivityFlashID
      ,[CouponID] = @CouponID
      ,[SmallImage] =@SmallImage
      ,[BigImage] = @BigImage
      ,[ColunmNumber] = @ColunmNumber
      ,[Type] = @Type
      ,[OrderBy] = @OrderBy
      ,[AppUrl] = @AppUrl
      ,[WapUrl] = @WapUrl
      ,[PCUrl] = @PCUrl
      ,[HandlerIOS] = @HandlerIOS
      ,[HandlerAndroid] = @HandlerAndroid
      ,[SOAPIOS] = @SOAPIOS
      ,[SOAPAndroid] = @SOAPAndroid
      ,[IsImage] = @IsImage
      ,[DisplayWay] = @DisplayWay
      ,[Description] = @Description
 WHERE ID=@ID ";
            SqlCommand cmd = new SqlCommand(sql);
           // cmd.Parameters.AddWithValue("@FK_Activity", model.FK_Activity.ToString());
            cmd.Parameters.AddWithValue("@GROUP", model.GROUP);
            cmd.Parameters.AddWithValue("@PID", model.PID);
            cmd.Parameters.AddWithValue("@ActivityFlashID", model.ActivityFlashID);
            cmd.Parameters.AddWithValue("@CouponID", model.CouponID);
            cmd.Parameters.AddWithValue("@SmallImage", model.SmallImage);
            cmd.Parameters.AddWithValue("@BigImage", model.BigImage);
            cmd.Parameters.AddWithValue("@ColunmNumber", model.ColunmNumber);
            cmd.Parameters.AddWithValue("@Type", model.Type);
            cmd.Parameters.AddWithValue("@OrderBy", model.OrderBy);
            cmd.Parameters.AddWithValue("@AppUrl", model.AppUrl);
            cmd.Parameters.AddWithValue("@WapUrl", model.WapUrl);
            cmd.Parameters.AddWithValue("@PCUrl", model.PCUrl);
            cmd.Parameters.AddWithValue("@HandlerIOS", model.HandlerIOS);
            cmd.Parameters.AddWithValue("@HandlerAndroid", model.HandlerAndroid);
            cmd.Parameters.AddWithValue("@SOAPIOS", model.SOAPIOS);
            cmd.Parameters.AddWithValue("@SOAPAndroid", model.SOAPAndroid);
            cmd.Parameters.AddWithValue("@IsImage", model.IsImage);
            cmd.Parameters.AddWithValue("@DisplayWay", model.DisplayWay);
            cmd.Parameters.AddWithValue("@Description", model.Description);
            cmd.Parameters.AddWithValue("@ID",model.ID);
            db.ExecuteNonQuery(cmd);
        }


        public static void DeteleDeatil(AsyncDbHelper db, string id)
        {
            string sql = "DELETE FROM Configuration.dbo.SE_ActivityDeatilConfig WHERE FK_Activity=@FK_Activity";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@FK_Activity",id);
            db.ExecuteNonQuery(cmd);
        }

        public static DataTable GetList(string whereStr , int pageSize, int pageIndex, out int rowCount)
        {
            string sql = @"
     SELECT M.* FROM (
        SELECT ROW_NUMBER()OVER(ORDER BY CreateDT DESC) AS RowNum,* FROM Configuration.dbo.SE_ActivityConfig WITH(NOLOCK) 
      ) M  WHERE M.RowNum>=((@PageIndex-1)*@PageSize) AND M.RowNum<(@PageIndex*@PageSize) " + whereStr ;
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@PageIndex",pageIndex);
                cmd.Parameters.AddWithValue("@PageSize",pageSize);

                DataTable dt = db.ExecuteDataTable(cmd);
                sql = "SELECT ISNULL(COUNT(*),0) FROM Configuration.dbo.SE_ActivityConfig  WITH(NOLOCK) ";
                cmd = new SqlCommand(sql);
                
                rowCount = Convert.ToInt32(db.ExecuteScalar(cmd));
                return dt;
            }
               
        }


        public static SE_Activity GetEntity(string id)
        {
            SE_Activity model = null;
            var db = DbHelper.CreateDefaultDbHelper();
            try
            {
                string sql = "SELECT * FROM Configuration.dbo.SE_ActivityConfig WITH(NOLOCK) WHERE ID=@ID";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id.ToString());
                model = db.ExecuteDataTable(cmd).ConvertTo<SE_Activity>().FirstOrDefault();
                if (model == null)
                    return null;
                sql = "SELECT * FROM Configuration.dbo.SE_ActivityDeatilConfig WITH(NOLOCK) WHERE FK_Activity=@FK_Activity";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FK_Activity",id.ToString());
                model.Items = db.ExecuteDataTable(cmd).ConvertTo<SE_ActivityDeatil>();
            }
            catch (Exception e)
            {

            }
            finally
            {
                db.Dispose();
            }
            return model;
        }

    }
}

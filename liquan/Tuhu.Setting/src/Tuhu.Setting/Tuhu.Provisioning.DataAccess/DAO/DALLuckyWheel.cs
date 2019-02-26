using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public class DALLuckyWheel
    {


        public static bool Insert(LuckyWheel model)
        {
            string sql = @"INSERT INTO Activity..LuckyWheel
        ( ID ,
          Title ,
          isNewUser ,
          isStatus ,
          CreateDate ,
          UpdateDate ,
          DataParames,
          IsAddOne,
          IsIntegral,
          Integral,
          CreatorUser,
          UpdateUser,PreShareTimes,CompletedShareTimes
        )
VALUES  ( @ID ,   @Title ,  @isNewUser ,   @isStatus , GETDATE() ,   @UpdateDate ,  @DataParames ,@IsAddOne,@IsIntegral,@Integral,@CreatorUser,@UpdateUser ,@PreShareTimes, @CompletedShareTimes )";
            bool result = false;

            SqlConnection conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir"));
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                //
                string sqlDelete = @" DELETE FROM Activity..LuckyWheel WHERE ID = @ID    DELETE FROM Activity..LuckyWheelDeatil WHERE FKLuckyWheelID = @ID";
                SqlHelper.ExecuteNonQuery(trans, System.Data.CommandType.Text, sqlDelete, new SqlParameter("@ID", model.ID));

                SqlParameter[] parameters = new SqlParameter[13];
                parameters[0] = new SqlParameter("@ID", model.ID);
                parameters[1] = new SqlParameter("@Title", model.Title);
                parameters[2] = new SqlParameter("@isNewUser", model.isNewUser);
                parameters[3] = new SqlParameter("@isStatus", model.isStatus);
                parameters[4] = new SqlParameter("@UpdateDate", model.UpdateDate);
                parameters[5] = new SqlParameter("@DataParames", model.DataParames);
                parameters[6] = new SqlParameter("@IsAddOne", model.IsAddOne);
                parameters[7] = new SqlParameter("@IsIntegral", model.IsIntegral);
                parameters[8] = new SqlParameter("@Integral", model.Integral);
                parameters[9] = new SqlParameter("@CreatorUser", model.CreatorUser);
                parameters[10] = new SqlParameter("@UpdateUser", model.UpdateUser);
                parameters[11] = new SqlParameter("@PreShareTimes",model.PreShareTimes);
                parameters[12] = new SqlParameter("@CompletedShareTimes",model.CompletedShareTimes);

                SqlHelper.ExecuteNonQuery(trans, System.Data.CommandType.Text, sql, parameters);


                string sqlDeatil = @" INSERT INTO Activity..LuckyWheelDeatil
          ( FKLuckyWheelID ,
            Type ,
            CouponRuleID ,
            MaxCoupon ,
            BGImage ,
            ContentImage ,
            ChangeImage ,
            GetDescription ,
            GoDescription ,
            APPUrl ,
            WapUrl ,
            WwwUrl ,
            HandlerAndroid ,
            SOAPAndroid ,
            HandlerIOS ,
            SOAPIOS ,
            UserRank ,
            OrderBy
          )
  VALUES  ( @FKLuckyWheelID ,
            @Type , 
            @CouponRuleID , 
            @MaxCoupon , 
           @BGImage , 
           @ContentImage , 
           @ChangeImage , 
           @GetDescription ,
           @GoDescription , 
           @APPUrl ,
           @WapUrl , 
           @WwwUrl,
          @HandlerAndroid , 
          @SOAPAndroid ,
           @HandlerIOS ,
           @SOAPIOS ,
            @UserRank , 
            @OrderBy 
          )";
                foreach (var item in model.Items)
                {
                    SqlParameter[] paras = new SqlParameter[18];
                    paras[0] = new SqlParameter("@FKLuckyWheelID", model.ID);
                    paras[1] = new SqlParameter("@Type", item.Type);
                    paras[2] = new SqlParameter("@CouponRuleID", item.CouponRuleID);
                    paras[3] = new SqlParameter("@MaxCoupon", string.IsNullOrWhiteSpace( item.MaxCoupon)?null:item.MaxCoupon);
                    paras[4] = new SqlParameter("@BGImage", item.BGImage);
                    paras[5] = new SqlParameter("@ContentImage", item.ContentImage);
                    paras[6] = new SqlParameter("@ChangeImage", item.ChangeImage);
                    paras[7] = new SqlParameter("@GetDescription", item.GetDescription);
                    paras[8] = new SqlParameter("@GoDescription", item.GoDescription);
                    paras[9] = new SqlParameter("@APPUrl", item.APPUrl);
                    paras[10] = new SqlParameter("@WapUrl", item.WapUrl);
                    paras[11] = new SqlParameter("@WwwUrl", item.WwwUrl);
                    paras[12] = new SqlParameter("@HandlerAndroid", item.HandlerAndroid);
                    paras[13] = new SqlParameter("@SOAPAndroid", item.SOAPAndroid);
                    paras[14] = new SqlParameter("@HandlerIOS", item.HandlerIOS);
                    paras[15] = new SqlParameter("@SOAPIOS", item.SOAPIOS);
                    paras[16] = new SqlParameter("@UserRank", item.UserRank);
                    paras[17] = new SqlParameter("@OrderBy", item.OrderBy);
                    SqlHelper.ExecuteNonQuery(trans, System.Data.CommandType.Text, sqlDeatil, paras);
                }



                trans.Commit();
                result = true;
            }
            catch (Exception em)
            {
                trans.Rollback();
                result = false;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                trans.Dispose();
            }




            return result;
        }


        public static  DataTable  GetTableList(string selSql)
        {
            string sql = "SELECT * FROM Activity..LuckyWheel WITH( NOLOCK)";
            if (!string.IsNullOrEmpty(selSql))
            {
                sql += selSql;
            }
            using (var db = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                SqlCommand cmd = new SqlCommand(sql);
               
                return db.ExecuteDataTable(cmd);
            }
        }


        public static LuckyWheel GetEntity(string id)
        {
            LuckyWheel model = new LuckyWheel();
            string sql = "SELECT * FROM Activity..LuckyWheel WITH( NOLOCK) where ID=@ID";
            string sqlDeatil = "SELECT * FROM Activity..LuckyWheelDeatil WHERE FKLuckyWheelID=@FKLuckyWheelID";
            using (var db = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID",id);
                model = db.ExecuteDataTable(cmd).ConvertTo<LuckyWheel>().ToList().FirstOrDefault();
                cmd = new SqlCommand(sqlDeatil);
                cmd.Parameters.AddWithValue("@FKLuckyWheelID",id);
                model.Items = db.ExecuteDataTable(cmd).ConvertTo<LuckyWheelDeatil>().ToList();
                return model;
            }
        }

        public static bool Delete(string id)
        {
            string sql = @"DELETE FROM Activity..LuckyWheel WHERE ID=@ID; DELETE FROM Activity..LuckyWheelDeatil WHERE FKLuckyWheelID=@FKLuckyWheelID";
            using (var db = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@FKLuckyWheelID",id);
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }
       


    }
}

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



  public  class DALCarTag
    {
        public bool Add(SE_CarTagCouponConfig model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_CarTagCouponConfig
        ( CouponGuid ,
          Discount ,
          Description ,
          MinMoney ,
          Status ,
          CreateDate
        )
VALUES  ( @CouponGuid , -- CouponGuid - nvarchar(50)
          @Discount , -- Discount - money
         @Description , -- Description - nvarchar(50)
          @MinMoney , -- MinMoney - money
          @Status , -- Status - bit
         @CreateDate -- CreateDate - datetime
        )";
            return DbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[] {
                new SqlParameter("@CouponGuid",model.CouponGuid),
                new SqlParameter() { ParameterName="@Discount", Value=model.Discount },
                new SqlParameter() {  ParameterName="@Description",Value = model.Description},
                new SqlParameter() { ParameterName="@MinMoney",Value=model.MinMoney},
                new SqlParameter() { ParameterName="@Status", Value=1 },
                new SqlParameter() { ParameterName="@CreateDate",Value=DateTime.Now}
               // new SqlParameter() { ParameterName="@StartDateTime",Value=model.StartDateTime},
              //  new SqlParameter() { ParameterName="@EndDateTime" ,Value=model.EndDateTime}
            })>0;
        }

        public IEnumerable<SE_CarTagCouponConfig> GetList()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var dt = dbHelper.ExecuteDataTable("     SELECT * FROM Configuration.dbo.SE_CarTagCouponConfig (NOLOCK) ORDER BY CreateDate ASC  ");
                if (dt == null || dt.Rows.Count == 0)
                    return null;
                return dt.ConvertTo<SE_CarTagCouponConfig>();
            }
        }

        public bool UpdateStatus(int id, int status)
        {
            return DbHelper.ExecuteNonQuery("    UPDATE Configuration.dbo.SE_CarTagCouponConfig SET [Status]=@Status WHERE ID =@ID ", CommandType.Text, new SqlParameter[] {
                new SqlParameter() { ParameterName="@Status",Value = status },
                new SqlParameter() { ParameterName="@ID", Value= id }
            }) > 0;
        }

        public bool UpdateDateTime(DateTime startDateTime, DateTime endDateTime,string name)
        {

            return DbHelper.ExecuteNonQuery("UPDATE  Configuration.dbo.SE_CarTagCouponConfig SET Name=@Name, StartDateTime=@StartDateTime, EndDateTime=@EndDateTime ",CommandType.Text,new SqlParameter {
                 ParameterName= "@StartDateTime",
                 Value=startDateTime
            },new SqlParameter() {
                ParameterName= "@EndDateTime",
                Value=endDateTime
            },new SqlParameter() {
                ParameterName="@Name",
                Value = name
            }) > 0;
        }

        public bool UpdateImageUrl(int id, string url)
        {
            return DbHelper.ExecuteNonQuery("UPDATE Configuration.dbo.SE_CarTagCouponConfig SET ImageUrl=@ImageUrl WHERE id =@ID ", CommandType.Text, new SqlParameter()
            {
                ParameterName = "@ImageUrl",
                Value = url
            },new SqlParameter() {
                ParameterName="@ID",
                Value=id
            }) > 0;
        }



    }
}

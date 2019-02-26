using System;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALVin
    {

        public static int DeleteVin_recordById(string id)
        {
            return SqlAdapter.Create("delete from gungnir..vin_record where id=@id", CommandType.Text, "Aliyun")
                       .Par("@id", id, SqlDbType.UniqueIdentifier)
                       .ExecuteNonQuery();

        }

        public static int UpdateVin_record(string phone, string vin, string id)
        {
            return SqlAdapter.Create("update gungnir..vin_record with (ROWLOCK) set rphone=@phone,vin=@vin where id=@id", CommandType.Text, "Aliyun")
                      .Par("@phone", phone, SqlDbType.NVarChar, 20)
                      .Par("@vin", vin, SqlDbType.NVarChar, 32)
                      .Par("@id", id, SqlDbType.UniqueIdentifier)
                      .ExecuteNonQuery();
        }

        public static bool IsRepeatVin_record(string phone, string vin)
        {
            return SqlAdapter.Create("select top 1 * from gungnir..vin_record with(NOLOCK) where rphone=@phone and vin=@vin", CommandType.Text, "Aliyun")
               .Par("@phone", phone, SqlDbType.NVarChar, 20)
               .Par("@vin", vin, SqlDbType.NVarChar, 32)
               .ExecuteModel().IsEmpty;

        }

        public static bool InsertVin_record(string id, string phone, string vin, string u)
        {
            return SqlAdapter.Create("insert into gungnir..vin_record (id,rphone,vin,src,status) values(@id,@phone,@vin,@usr,1)", CommandType.Text, "Aliyun")
                        .Par("@id", id, SqlDbType.UniqueIdentifier)
                        .Par("@phone", phone, SqlDbType.NVarChar, 20)
                        .Par("@vin", vin, SqlDbType.NVarChar, 32)
                        .Par("@usr", u, SqlDbType.NVarChar, 50)
                        .ExecuteNonQuery() > 0;
        }

        public static DyModel GetVin_record(int ps, int pe)
        {
            return SqlAdapter.Create("select * from (select (row_number() over(order by id)) as rownum,* from gungnir..vin_record) T where T.rownum between @start and @end order by createtime desc", CommandType.Text, "Aliyun")
                     .Par("@start", ps, SqlDbType.Int)
                     .Par("@end", pe, SqlDbType.Int)
                     .ExecuteModel();
        }

        public static bool GetVIN_REGION()
        {
            return SqlAdapter.Create("SELECT * FROM gungnir..VIN_REGION WITH (NOLOCK)").ExecuteModel().IsEmpty;
        }

        public static DyModel GetVIN_REGIONDyModel()
        {
            return SqlAdapter.Create("SELECT * FROM gungnir..VIN_REGION WITH (NOLOCK)").ExecuteModel();
        }

        public static void Operate(int isEnable, int id, string region)
        {
            //改变状态
            SqlAdapter.Create("UPDATE gungnir..VIN_REGION SET isenable=@isenable WHERE Id=@Id")
           .Par("@isenable", isEnable, SqlDbType.Int)
           .Par("@Id", id, SqlDbType.Int).ExecuteNonQuery();

            //插入到记录表
            SqlAdapter.Create("INSERT INTO gungnir..Vin_OnOffRecord(vinregionid,region,changetime,onoffstate) VALUES(@vinregionid,@region,@changetime,@onoffstate)")
             .Par("@vinregionid", id, SqlDbType.Int)
             .Par("@region", region, SqlDbType.NVarChar, 50)
             .Par("@changetime", DateTime.Now, SqlDbType.DateTime)
             .Par("@onoffstate", isEnable == 1 ? "启用" : "禁用", SqlDbType.NVarChar, 10).ExecuteNonQuery();
        }

        public static void InsertVIN_REGION(string region)
        {
            SqlAdapter.Create("INSERT INTO VIN_REGION (region,isenable) VALUES(@region,@isenable)")
                  .Par("@region", region, SqlDbType.NVarChar)
                  .Par("@isenable", 0, SqlDbType.Int)
                  .ExecuteNonQuery();
        }
    }
}

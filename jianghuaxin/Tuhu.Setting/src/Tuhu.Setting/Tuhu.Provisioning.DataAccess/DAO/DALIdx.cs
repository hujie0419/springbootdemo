using System;
using System.Collections.Specialized;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALIdx
    {
        public static bool Insert_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)
        {
            string sql = @"insert into gungnir..tal_newappsetdata
                      (apptype, modelname,modelfloor,showorder,icoimgurl,jumph5url,showstatic,starttime,overtime,cpshowtype,
                       cpshowbanner,appoperateval,operatetypeval,pronumberval,keyvaluelenth,umengtongji,createtime)
                     values(@apptype,@modelname,@modelfloor,@showorder,@icoimgurl,@jumph5url,@showstatic,@starttime,@overtime,@cpshowtype,
                      @cpshowbanner,@appoperateval,@operatetypeval,@pronumberval,@keyvaluelenth,@umengtongji,@createtime)";

            return SqlAdapter.Create(sql, smAppModel, CommandType.Text, "gungnir")
                             .Par(form)
                             .Par("@createtime", DateTime.Now)
                             .ExecuteNonQuery() > 0;
        }

        public static bool Update_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)
        {
            string sql = @"update gungnir..tal_newappsetdata set modelname=@modelname,showorder=@showorder,
                           icoimgurl=@icoimgurl,jumph5url=@jumph5url,showstatic=@showstatic,starttime=@starttime,
                           overtime=@overtime,cpshowtype=@cpshowtype,cpshowbanner=@cpshowbanner,appoperateval=@appoperateval,
                           operatetypeval=@operatetypeval,pronumberval=@pronumberval,keyvaluelenth=@keyvaluelenth,umengtongji=@umengtongji,
                           updatetime=@updatetime,Version=@Version,edittime=@edittime where id=@id";

            return SqlAdapter.Create(sql, smAppModel, CommandType.Text, "gungnir")
                            .Par(form)
                            .Par("@updatetime", DateTime.Now)
                            .ExecuteNonQuery() > 0;
        }

        public static DyModel Get_tal_newappsetdata(SqlSchema smAppModel)
        {
            string sql = @"SELECT * FROM [Gungnir].[dbo].[tal_newappsetdata] order by apptype,modelfloor,showorder,modelname";

            return SqlAdapter.Create(sql, smAppModel, CommandType.Text, "Gungnir_AlwaysOnRead").ExecuteModel();
        }

        public static DyModel Get_tbl_AdProduct(SqlSchema smp, string mid)
        {
            string sql = @"select * from [Gungnir].[dbo].[tbl_AdProduct] where [new_modelid]=@new_modelid order by Position";

            return SqlAdapter.Create(sql, smp, CommandType.Text, "Gungnir_AlwaysOnRead")
                    .Par("@new_modelid", mid)
                    .ExecuteModel();
        }

        public static bool Delete_tal_newappsetdata(SqlSchema smAppModel, string id)
        {
            string sql = @"delete from gungnir..tal_newappsetdata where id=@id";

            return SqlAdapter.Create(sql, smAppModel, CommandType.Text, "gungnir")
                     .Par("@id", id).ExecuteNonQuery() > 0;
        }

        public static bool Delete_tbl_AdProduct(SqlSchema smp, string mid, string id)
        {
            string sql = @"delete from [Gungnir].[dbo].[tbl_AdProduct] where pid=@id and new_modelid=@new_modelid";

            return SqlAdapter.Create(sql, smp, CommandType.Text, "gungnir")
                   .Par("@new_modelid", mid)
                   .Par("@id", id, SqlDbType.VarChar, 256).ExecuteNonQuery() > 0;
        }

        public static bool Insert_tbl_AdProduct(SqlSchema smp, NameValueCollection form)
        {
            //string sql = @"insert into [Gungnir].[dbo].[tbl_AdProduct] 
            //               (advertiseid,pid,position,state,createdatetime,promotionprice,promotionnum,new_modelid)
            //               values(@advertiseid,@pid,@position,@state,@cdt,@promotionprice,@promotionnum,@new_modelid)";

            if (form == null)
                return false;

            string sql = @" declare @Count int
                            select @Count = count(1) from [Gungnir].[dbo].[tbl_AdProduct] WITH (NOLOCK) where PID=@pid and new_modelID=@new_modelid
                            if(@Count = 0)
                               begin
		                           insert into [Gungnir].[dbo].[tbl_AdProduct] 
		                           (advertiseid,pid,position,state,createdatetime,promotionprice,promotionnum,new_modelid)
		                           values(@advertiseid,@pid,@position,@state,@cdt,@promotionprice,@promotionnum,@new_modelid)
                               end ";

            return SqlAdapter.Create(sql, smp, CommandType.Text, "gungnir")
                         .Par(form)
                         .Par("@cdt", DateTime.Now, SqlDbType.DateTime)
                         .ExecuteNonQuery() > 0;
        }

        public static bool Update_tbl_AdProduct(SqlSchema smp, NameValueCollection form, string id)
        {
            //string sql = @"update [Gungnir].[dbo].[tbl_AdProduct] set pid=@pid,position=@position,state=@state,
            //                           lastupdatedatetime=@lastupdatedatetime,promotionprice=@promotionprice,
            //                           promotionnum=@promotionnum where pid=@id and new_modelid=@new_modelid";

            if (form == null && form["AdvertiseID"] == null)
                return false;

            string sql = @"
                            declare @GetAdvertiseID varchar(256),@Count int
                            select @GetAdvertiseID = AdvertiseID from [Gungnir].[dbo].[tbl_AdProduct] WITH (NOLOCK) where PID=@pid and new_modelID=@new_modelid
                            select @Count = count(1) from [Gungnir].[dbo].[tbl_AdProduct] WITH (NOLOCK) where PID=@pid and new_modelID=@new_modelid
                            if(@Count = 0)
	                            begin
		                            update [Gungnir].[dbo].[tbl_AdProduct] set pid=@pid,position=@position,state=@state,
                                    lastupdatedatetime=@lastupdatedatetime,promotionprice=@promotionprice,
                                    promotionnum=@promotionnum where pid=@id and new_modelid=@new_modelid;
	                            end
                            else if(@Count = 1 and @GetAdvertiseID = @advertiseid)
	                            begin
		                            update [Gungnir].[dbo].[tbl_AdProduct] set pid=@pid,position=@position,state=@state,
                                    lastupdatedatetime=@lastupdatedatetime,promotionprice=@promotionprice,
                                    promotionnum=@promotionnum where pid=@id and new_modelid=@new_modelid;
	                            end
                          ";

            return SqlAdapter.Create(sql, smp, CommandType.Text, "gungnir")
                        .Par(form)
                        .Par("@lastupdatedatetime", DateTime.Now)
                        .Par("@id", id, SqlDbType.VarChar, 256)
                        .ExecuteNonQuery() > 0;
        }
    }
}

using System;
using System.Collections.Specialized;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALSale
    {
        public static int Delete_act_saleproduct(int id, SqlSchema smSaleProd)
        {
            string sql = @"delete from gungnir..act_saleproduct where id=@id";

            return SqlAdapter.Create(sql, smSaleProd, CommandType.Text, "Aliyun")
                         .Par("@id", id).ExecuteNonQuery();
        }

        public static bool Get_act_saleproduct_repeat(SqlSchema smSaleProd, NameValueCollection form)
        {
            string sql = @"select top 1 pnum from gungnir..act_saleproduct with (nolock) where pnum like @pnum and mid=@mid";

            return SqlAdapter.Create(sql, smSaleProd, CommandType.Text, "Aliyun")
                         .Par(form).ExecuteModel().IsEmpty;
        }

        public static int Insert_act_saleproduct(SqlSchema smSaleProd, NameValueCollection form)
        {
            string addsql = @"insert into gungnir..act_saleproduct 
                           (mid,pnum,sort,name,lprice,oprice,plimit,tlimit,status,createtime,pimg,quantityleft) 
                             values
                            (@mid,@pnum,@sort,@name,@lprice,@oprice,@plimit,@tlimit,@status,@createtime,@pimg,@tlimit)";

            return SqlAdapter.Create(addsql, smSaleProd, CommandType.Text, "Aliyun")
                    .Par(form)
                    .Par("@createtime", DateTime.Now.ToString())
                    .ExecuteNonQuery();
        }
        public static bool Get_act_saleproduct_mid(SqlSchema smSaleProd, NameValueCollection form)
        {
            string sql = @"select top 1 pnum from gungnir..act_saleproduct with (nolock) where pnum like @pnum and mid=@mid and id<>@id";

            return SqlAdapter.Create(sql, smSaleProd, CommandType.Text, "Aliyun")
                       .Par(form).ExecuteModel().IsEmpty;
        }

        public static int Update_act_saleproduct(SqlSchema smSaleProd, NameValueCollection form)
        {
            string updsql = @"update gungnir..act_saleproduct set mid=@mid,pnum=@pnum,sort=@sort,name=@name,
            lprice=@lprice,oprice=@oprice,plimit=@plimit,tlimit=@tlimit,quantityleft=@tlimit,
            status=@status,updatetime=@updatetime,pimg=@pimg where id=@id";

            return SqlAdapter.Create(updsql, smSaleProd, CommandType.Text, "Aliyun")
                 .Par(form)
                 .Par("@updatetime", DateTime.Now.ToString())
                 .ExecuteNonQuery();
        }

        public static DyModel Get_act_salemodule(SqlSchema smSaleModule, int parentid)
        {
            string sql = @"select * from gungnir..act_salemodule with(nolock) where parentid=@parentid order by sort";

            return SqlAdapter.Create(sql, smSaleModule, CommandType.Text, "Aliyun")
                   .Par("@parentid", parentid)
                   .ExecuteModel();
        }

        public static DyModel Get_act_salemodule()
        {
            string sql = @"SELECT sm.mname ppname, sm.mstatus ppstatus, cm.mname pname, cm.mstatus pstatus,gm.* 
                           FROM [Gungnir].[dbo].[act_salemodule] gm with(nolock) 
                           left join gungnir..act_salemodule cm with(nolock) on gm.parentid=cm.id 
                           left join gungnir..act_salemodule sm with(nolock) on cm.parentid=sm.id 
                           order by parentid, sort";

            return SqlAdapter.Create(sql).ExecuteModel();
        }

        public static DyModel Get_act_saleproduct(string od, int mid)
        {
            return SqlAdapter.Create("select * from gungnir..act_saleproduct where mid=@mid order by sort")
                          .Par("@od", od, SqlDbType.NVarChar, 50)
                          .Par("@mid", mid).ExecuteModel();
        }
    }
}

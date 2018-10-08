using System;
using System.Data;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Controllers
{
    public class SaleController : Controller
    {
        public ActionResult SectorIndex()
        {
            return View();
        }
        [PowerManage]
        public ActionResult SaleProIndex()
        {
            var msg = Session["msg"];
            if (msg != null)
            {
                ViewBag.msg = msg.ToString();
            }
            Session["msg"] = null;
            return View();
        }

        public ActionResult Pinfo(string pnum)
        {
            var md = SqlAdapter.Create(@"SELECT * FROM Tuhu_productcatalog..[CarPAR_zh-CN] where ProductID + '|' + VariantID  COLLATE Chinese_PRC_CI_AS=@pid", CommandType.Text, "Gungnir_AlwaysOnRead")
                .Par("@pid", pnum, SqlDbType.NVarChar, 513)
                .ExecuteModel();
            if (!md.IsEmpty)
            {
                return Content(md.Serialize(true));
            }
            return Content("{\"error\":true, \"Msg\":\"产品不存在\"}");
        }
        public ActionResult ProdRemove(int? id)
        {
            if (id.HasValue)
            {
                var n = SqlAdapter.Create("delete from gungnir..act_saleproduct where id=@id", smSaleProd, CommandType.Text, "Aliyun")
                    .Par("@id", id).ExecuteNonQuery();
                if (n > 0)
                {
                    return Content(@"{""IsSuccess"":true}");
                }
                return Content(@"{""error"":true, ""Msg"":""删除失败""}");
            }
            return Content(@"{""error"":true, ""Msg"":""标识不存在""}");
        }
        [HttpPost]
        public ActionResult SaveProds(string a)
        {
            if (string.Equals(a, "1"))
            {
                var mva = SqlAdapter.Create("select top 1 pnum from gungnir..act_saleproduct with (nolock) where pnum like @pnum and mid=@mid", smSaleProd, CommandType.Text, "Aliyun")
                    .Par(Request.Form).ExecuteModel();
                if (mva.IsEmpty)
                {
                    //Request.Form;
                    string addsql = "insert into gungnir..act_saleproduct (mid,pnum,sort,name,lprice,oprice,plimit,tlimit,status,createtime,pimg,quantityleft) values(@mid,@pnum,@sort,@name,@lprice,@oprice,@plimit,@tlimit,@status,@createtime,@pimg,@tlimit)";
                    var n = SqlAdapter.Create(addsql, smSaleProd, CommandType.Text, "Aliyun")
                        .Par(Request.Form)
                        .Par("@createtime", DateTime.Now.ToString())
                        .ExecuteNonQuery();
                    Session["msg"] = n > 0 ? "添加成功" : "添加失败";
                }
                else
                {
                    Session["msg"] = "该型号产品已存在";
                }
            }
            else
            {
                var mvu = SqlAdapter.Create("select top 1 pnum from gungnir..act_saleproduct with (nolock) where pnum like @pnum and mid=@mid and id<>@id", smSaleProd, CommandType.Text, "Aliyun")
                    .Par(Request.Form).ExecuteModel();
                if (mvu.IsEmpty)
                {
                    string updsql = "update gungnir..act_saleproduct set mid=@mid,pnum=@pnum,sort=@sort,name=@name,lprice=@lprice,oprice=@oprice,plimit=@plimit,tlimit=@tlimit,quantityleft=@tlimit,status=@status,updatetime=@updatetime,pimg=@pimg where id=@id";
                    var n = SqlAdapter.Create(updsql, smSaleProd, CommandType.Text, "Aliyun")
                        .Par(Request.Form)
                        .Par("@updatetime", DateTime.Now.ToString())
                        .ExecuteNonQuery();
                    Session["msg"] = n > 0 ? "保存成功" : "保存失败";
                }
                else
                {
                    Session["msg"] = "该型号产品已存在";
                }
            }
            return Redirect("/sale/saleproindex");
        }
        public ActionResult Sectors(int? pid)
        {
            var r = new ModelResult();
            int parentid = pid.HasValue ? pid.Value : 0;
            var md = SqlAdapter.Create("select * from gungnir..act_salemodule with(nolock) where parentid=@parentid order by sort", smSaleModule, CommandType.Text, "Aliyun")
                .Par("@parentid", parentid)
                .ExecuteModel();
            if (md.IsEmpty)
            {
                r.Error("暂无数据");
            }
            r.Model = md;
            return Content(r.Model.Json(false));
        }

        public ActionResult AllSectors()
        {
            var md = SqlAdapter.Create("select * FROM [Gungnir].[dbo].[act_salemodule] with(nolock) order by parentid, sort")
                .ExecuteModel();
            if (md.IsEmpty)
            {
                return Content("{error:true,Msg:'暂无数据'}");
            }
            var s = md.Serialize();
            return Content(s);
        }

        public ActionResult Prods(int? mid, string od = null)
        {
            if (mid.HasValue)
            {
                if (string.IsNullOrWhiteSpace(od))
                {
                    od = "name";
                }
                var md = SqlAdapter.Create("select * from gungnir..act_saleproduct where mid=@mid order by sort")
                    .Par("@od", od, SqlDbType.NVarChar, 50)
                    .Par("@mid", mid.Value).ExecuteModel();
                if (md.IsEmpty)
                {
                    return Content(@"{""error"":true,""Msg"":""暂无数据""}");
                }
                var s = md.Serialize();
                return Content(s);
            }
            return Content("{error:true,Msg:'Invalid parameter'}");
        }

        #region constructors
        protected SqlSchema smSaleModule;
        protected SqlSchema smSaleProd;
        public SaleController()
        {
            smSaleModule = new SqlSchema();
            smSaleModule.Bind("grp", SqlDbType.NVarChar, 50)
                .Bind("device", SqlDbType.NVarChar, 50)
                .Bind("mid", SqlDbType.Int)
                .Bind("parentid", SqlDbType.Int);
            smSaleProd = new SqlSchema();
            smSaleProd.Bind("id", SqlDbType.Int)
                .Bind("mid", SqlDbType.Int)
                .Bind("pnum", SqlDbType.NVarChar, 50)
                .Bind("pv", SqlDbType.NVarChar, 50)
                .Bind("sort", SqlDbType.Int)
                .Bind("name", SqlDbType.NVarChar, 128)
                .Bind("lprice", SqlDbType.Money)
                .Bind("oprice", SqlDbType.Money)
                .Bind("plimit", SqlDbType.Int)
                .Bind("tlimit", SqlDbType.Int)
                .Bind("status", SqlDbType.Int)
                .Bind("createtime", SqlDbType.DateTime)
                .Bind("updatetime", SqlDbType.DateTime)
                .Bind("quantityleft", SqlDbType.Int)
                .Bind("pimg", SqlDbType.NVarChar, 4000);
        }
        #endregion
    }
}
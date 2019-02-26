using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ModuleController : Controller
    {
        protected virtual void Init()
        {
            sqlins = "insert into gungnir..act_salemodule (mname,mstatus,banner,dispbanner,sort,createtime,stime,etime,parentid) values(@mname,@mstatus,@banner,@dispbanner,@sort,@createtime,@stime,@etime,@parentid)";
            sqlupd = "update gungnir..act_salemodule set mname=@mname,mstatus=@mstatus,banner=@banner,dispbanner=@dispbanner,sort=@sort,updatetime=@updatetime,stime=@stime,etime=@etime where id=@id";
            sqldel = "delete from gungnir..act_salemodule where id=@id";
            sqlsel = "SELECT * FROM [Gungnir].[dbo].[act_salemodule] where parentid=@parentid order by parentid,sort";
            selectparentid = "  select parentid from Gungnir..act_salemodule";

            sqlupdschema = new SqlSchema().Bind("id", SqlDbType.Int).Bind("mname", SqlDbType.NVarChar, 50).Bind("mstatus", SqlDbType.Int).Bind("banner", SqlDbType.NVarChar, 4000)
                .Bind("dispbanner", SqlDbType.Int).Bind("sort", SqlDbType.Int).Bind("updatetime", SqlDbType.DateTime)
                .Bind("stime", SqlDbType.DateTime).Bind("etime", SqlDbType.DateTime);//修改

            sqlinsschema = new SqlSchema().Bind("mname", SqlDbType.NVarChar, 50).Bind("mstatus", SqlDbType.Int).Bind("banner", SqlDbType.NVarChar, 4000)
                .Bind("dispbanner", SqlDbType.Int).Bind("sort", SqlDbType.Int).Bind("createtime", SqlDbType.DateTime).Bind("updatetime", SqlDbType.DateTime).Bind("grp", SqlDbType.NVarChar, 50)
                .Bind("device", SqlDbType.NVarChar, 50).Bind("stime", SqlDbType.DateTime).Bind("etime", SqlDbType.DateTime).Bind("parentid", SqlDbType.Int);//添加

            schema = new SqlSchema().Bind("id", SqlDbType.Int).Bind("mname", SqlDbType.NVarChar, 50).Bind("mstatus", SqlDbType.Int).Bind("banner", SqlDbType.NVarChar, 4000)
                .Bind("dispbanner", SqlDbType.Int).Bind("sort", SqlDbType.Int).Bind("createtime", SqlDbType.DateTime).Bind("updatetime", SqlDbType.DateTime).Bind("grp", SqlDbType.NVarChar, 50)
                .Bind("device", SqlDbType.NVarChar, 50).Bind("stime", SqlDbType.DateTime).Bind("etime", SqlDbType.DateTime).Bind("parentid", SqlDbType.Int);//查找
        }

        public ActionResult ModuleMange()
        {
            readMsg();
            return View();
        }


        public ActionResult Save(string a)
        {
            hastable.Clear();
            string mname = Request["mname"];
            var _file = Request.Files;
            long size = _file[0].ContentLength;
            //文件类型  
            string type = _file[0].ContentType;
            //文件名  
            string name = _file[0].FileName;
            //文件格式  
            string _tp = System.IO.Path.GetExtension(name);

            //  else
            //   {
            string _BImage = string.Empty;
            string hiddenBanner = Request["bannerurl"];
            if (size > 0)//修改的时候i，，如果没有上传图片，则获取隐藏url里面的插入到数据库
            {
                if (_tp.ToLower() == ".jpg" || _tp.ToLower() == ".jpeg" || _tp.ToLower() == ".gif" || _tp.ToLower() == ".png" || _tp.ToLower() == ".swf")
                {
                    //获取文件流  
                    System.IO.Stream stream = _file[0].InputStream;
                    _BImage = UploadFile(stream, name + type);
                }
            }
            else//插入到七牛，并获取返回路径
            {
                _BImage = hiddenBanner;
            }
            int mstatus = 0;
            int dispbanner = 0;
            string banner = _BImage;
            string requestMstatus = Request["mstatus"];
            string requestDispbanner = Request["dispbanner"];
            int id = Convert.ToInt32(Request["id"]);
            DateTime stime = Convert.ToDateTime(Request["stime"]);
            DateTime etime = Convert.ToDateTime(Request["etime"]);
            int sort = Convert.ToInt32(Request["sort"]);
            if (requestMstatus == "on")
            {
                mstatus = 1;
            }
            if (requestDispbanner == "on")
            {
                dispbanner = 1;
            }

            if ("1".Equals(a))
            {
                var x = SqlAdapter.Create("select count(*)  from Gungnir..act_salemodule where mname=@mname", schema)
              .Par("@mname", mname)
              .ExecuteScalar();
                if (Convert.ToInt32(x) > 0)//是否已存在此记录
                {
                    hastable.Add("resultCode", "2");//已存在此记录
                }
                else
                {
                    var n = SqlAdapter.Create(sqlins, sqlinsschema)
                   .Par("@mname", mname)
                   .Par("@mstatus", mstatus)
                   .Par("@dispbanner", dispbanner)
                   .Par("@banner", banner)
                   .Par("@stime", stime)
                   .Par("@etime", etime)
                   .Par("@parentid", id)
                   .Par("@sort", sort)
                   .Par("@createtime", DateTime.Now)
                   .ExecuteNonQuery();
                    if (n > 0)
                    {
                        hastable.Add("resultCode", "1");
                    }
                    else
                    {
                        hastable.Add("resultCode", "0");
                    }
                }
            }
            else
            {
                var n = SqlAdapter.Create(sqlupd, sqlupdschema)
                     .Par("@mname", mname)
                     .Par("@mstatus", mstatus)
                     .Par("@dispbanner", dispbanner)
                     .Par("@banner", banner)
                     .Par("@id", id)
                     .Par("@stime", stime)
                     .Par("@etime", etime)
                      .Par("@sort", sort)
                     .Par("@updatetime", DateTime.Now)
                     .ExecuteNonQuery();
                if (n > 0)
                {
                    hastable.Add("resultCode", "1");
                    //writeMsg("更新成功");
                }
                else
                {
                    hastable.Add("resultCode", "0");

                    //writeMsg("更新失败 " + SqlAdapter.ErrorMsg);
                }
            }
            //   }
            return Content(hastable.ToJson().ToString());
        }

        //根据parentid获取SaleModule的数据
        public ActionResult Modules(string id)
        {

            var md = SqlAdapter.Create(sqlsel, schema).Par("@parentid", id)
                .ExecuteModel();
            if (md.IsEmpty)
            {
                return Content(error("模块不存在"));
            }
            var parentidmd = SqlAdapter.Create(selectparentid, schema).ExecuteModel();
            var parentidArray = parentidmd.DATA.AsEnumerable().Select(r => r["parentid"]);
            List<SaleModule> list = ModelConvertHelper<SaleModule>.ConvertToModel(md.DATA).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (!parentidArray.Contains(list[i].id))
                {
                    list[i].state = "open";
                }
                else
                {
                    list[i].state = "closed";
                }
            }
            return Content(list.ToJson().ToString());
        }

        //上传到七牛，并返回路径
        protected string UploadFile(Stream stream, string fileName)
        {
            string _ReturnStr = string.Empty;
            stream.Seek(0, SeekOrigin.Begin);
            IOClient _IOClient = new IOClient();
            var _PutPolicy = new PutPolicy(WebConfigurationManager.AppSettings["Qiniu:comment_scope"], 3600).Token();
            var _Result = _IOClient.Put(_PutPolicy, fileName, stream, new PutExtra());
            if (_Result.OK)
                _ReturnStr = WebConfigurationManager.AppSettings["Qiniu:comment_url"] + fileName;
            return _ReturnStr;
        }



        public ActionResult Remove(string id)
        {
            SqlAdapter.Create(sqldel, schema)
            .Par("@id", id).ExecuteNonQuery();
            return RemoveChilend(id);
        }

        //删除SaleModule节点
        public ActionResult RemoveChilend(string id)
        {
            try
            {
                var chenlidenNod = SqlAdapter.Create(sqlsel, schema).Par("@parentid", id).ExecuteModel();
                var parentidNodArray = chenlidenNod.DATA.AsEnumerable().Select(r => r["parentid"]).ToList();
                var idArray = chenlidenNod.DATA.AsEnumerable().Select(r => r["id"]).ToList();
                if (chenlidenNod.Count > 0)
                {
                    SqlAdapter.Create("delete from gungnir..act_salemodule where parentid=@parentid", schema).Par("@parentid", parentidNodArray[0]).ExecuteNonQuery();
                    return Remove(idArray[0].ToString());
                }
                hastable.Add("resultCode", "1");
            }
            catch (Exception ex)
            {
                hastable.Add("resultCode", "0");
            }
            return Content(hastable.ToJson().ToString());
        }

        #region Constructor

        protected string sqlins;
        protected SqlSchema sqlinsschema;
        protected string sqlupd;
        protected SqlSchema sqlupdschema;
        protected string sqldel;
        protected string sqlsel;
        protected string selectparentid;
        protected SqlSchema schema;
        Hashtable hastable = new Hashtable();
        public ModuleController()
        {
            Init();//初始化语句
        }

        private string error(string msg)
        {
            var r = new Dictionary<string, string>();
            r["error"] = "true";
            r["Msg"] = msg;
            return r.ToJson();
        }
        private void readMsg()
        {
            var msg = Session["msg"];
            if (msg != null)
            {
                ViewBag.msg = msg.ToString();
            }
            else
            {
                ViewBag.msg = string.Empty;
            }
            Session["msg"] = string.Empty;
        }
        private void writeMsg(string s)
        {
            Session["msg"] = HttpUtility.HtmlEncode(s);
        }
        #endregion

        //将datatable转换成table
        public class ModelConvertHelper<T> where T : new()  // 此处一定要加上new()
        {
            public static IList<T> ConvertToModel(DataTable dt)
            {
                IList<T> ts = new List<T>();// 定义集合
                Type type = typeof(T); // 获得此模型的类型
                string tempName = "";
                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();
                    PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                    foreach (PropertyInfo pi in propertys)
                    {
                        tempName = pi.Name;
                        if (dt.Columns.Contains(tempName))
                        {
                            if (!pi.CanWrite) continue;
                            object value = dr[tempName];
                            if (value != DBNull.Value)
                                pi.SetValue(t, value, null);
                        }
                    }
                    ts.Add(t);
                }
                return ts;
            }
        }
    }
}

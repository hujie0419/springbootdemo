using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.ExportImport;

namespace Tuhu.Provisioning.Controllers
{
    public class SelectController : Controller
    {
        private static readonly string[] users = new string[]
          {
            "hulang@tuhu.cn","renyingqiang@tuhu.cn","wangjunqiao@tuhu.cn","lixiao1@tuhu.cn","zhangchen@tuhu.cn","zhanglei1@tuhu.cn"
          };

        public ActionResult ShowAllResult()
        {
            return View();
        }
        private List<SqlInfoMessageEventArgs> _sqlInfoMessages = new List<SqlInfoMessageEventArgs>();

        [HttpPost]
        public ActionResult ShowAllResult(string sql, string serverName, int? download)
        {
            if (users.Contains(User.Identity.Name))
            {
                // ExceptionMonitor.AddNewMonitor("开始查询语句", "", "查询内容：" + sql, ThreadIdentity.Identifier.Name, "数据库查询", MonitorLevel.Info, MonitorModule.DataSearch);

                if (!string.IsNullOrWhiteSpace(sql))
                {
                    ViewBag.sql = sql;
                    ViewBag.SqlInfoMessages = _sqlInfoMessages;

                    using (var adapter = new SqlDataAdapter(sql, GetConnectionString(serverName)))
                    {
                        adapter.SelectCommand.CommandTimeout = 1800;

                        try
                        {
                            adapter.SelectCommand.Connection.InfoMessage += Connection_InfoMessage;

                            var ds = new DataSet();
                            var sw = Stopwatch.StartNew();

                            adapter.Fill(ds);

                            sw.Stop();
                            ViewBag.Milliseconds = sw.ElapsedMilliseconds;

                            if (download == 2 || download == 1 && ds.Tables.OfType<DataTable>().Sum(dt => dt.Rows.Count) > 100)
                                return File(ExportImportFactory.ExportExcel(ds), "application/ms-excel", "%E6%9F%A5%E8%AF%A2%E7%BB%93%E6%9E%9C(" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ").xlsx");

                            return View(ds);
                        }
                        catch (SqlException ex)
                        {
                            ViewBag.Exception = ex;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.Message;
                        }
                    }
                }
                // ExceptionMonitor.AddNewMonitor("结束查询语句", "", "有人在查询数据库", ThreadIdentity.Identifier.Name, "数据库查询");

                return View();
            }
            else
                return HttpNotFound();
        }

        void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _sqlInfoMessages.Add(e);
        }

        private static string GetConnectionString(string serverName)
        {
            switch (serverName)
            {
                case "Aliyun":
                    return WebConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
                case "Cache":
                    return WebConfigurationManager.ConnectionStrings["CacheReadOnly"].ConnectionString;
                case "WMS":
                    return WebConfigurationManager.ConnectionStrings["WmsReadOnly"].ConnectionString;
                default:
                    return WebConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            }
        }

    }
}

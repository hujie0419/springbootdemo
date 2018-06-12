using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using Tuhu.DbMap.Business;
using Tuhu.DbMap.Models;

namespace Tuhu.DbMap.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public JsonResult GetTableList()
		{
			var result = DbMapManager.GetTableList();
			return Json(result);
		}

		public JsonResult FetchTableDetail(string DbName, string TableName)
		{
			if (string.IsNullOrWhiteSpace(DbName) || string.IsNullOrWhiteSpace(TableName))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				});
			}

			var data = DbMapManager.FetchTableMap(DbName, TableName);
			return Json(new
			{
				Tablename = TableName,
				Description = DbMapManager.FetchTableDescruption(DbName, TableName),
				Data = data.Select(g => new SimpleDbMapModel
				{
					FieldName = g.FieldName,
					FieldType = g.FieldType,
					ByteCount = g.ByteCount,
					FieldSize = g.FieldSize,
					IsNullable = g.IsNullable,
					Description = g.Description
				})
			});
		}

		public ActionResult DbMap()
		{
			var result = DbMapManager.GetDbList().OrderBy(g => g).ToList();
			return View(result);
		}

		public JsonResult DbMapList(bool isCurrent = false)
		{
			var result = DbMapManager.GetDbList(isCurrent).OrderBy(g => g).ToList();
			return Json(result);
		}

		public JsonResult SelectTablesByDbName(string DbName, bool isCurrent = true)
		{
			if (string.IsNullOrWhiteSpace(DbName))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				});
			}

			var result = DbMapManager.SelectTablesByDbName(DbName, isCurrent);
			return Json(new
			{
				Code = 1,
				Data = result.OrderBy(g => g).ToList()
			});
		}

		public JsonResult AddTableMap(string DbName, string TableName)
		{
			if (string.IsNullOrWhiteSpace(DbName) || string.IsNullOrWhiteSpace(TableName))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				},JsonRequestBehavior.AllowGet);
			}

			var result = DbMapManager.AddTableMap(DbName, TableName);
			return Json(new
			{
				Code = 1,
				Data = result
			},JsonRequestBehavior.AllowGet);
		}

		public JsonResult FetchTableMap(string DbName, string TableName)
		{
			if (string.IsNullOrWhiteSpace(DbName) || string.IsNullOrWhiteSpace(TableName))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				});
			}

			var result = DbMapManager.FetchTableMap(DbName, TableName);
			return Json(new
			{
				Code = 1,
				Data = result
			});
		}

		public JsonResult UpdateDescription(int TableId, int FieldId, string Description)
		{
			if (TableId < 1 || FieldId < 1 || string.IsNullOrWhiteSpace(Description))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				});
			}

			var result = DbMapManager.UpdateDescription(TableId, FieldId, Description);
			return Json(new
			{
				Code = result,
				Info = result > 0 ? "修改成功" : "修改失败"
			});
		}

		public JsonResult FetchTableDescruption(string DbName, string TableName)
		{
			if (string.IsNullOrWhiteSpace(DbName) || string.IsNullOrWhiteSpace(TableName))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				});
			}

			var result = DbMapManager.FetchTableDescruption(DbName, TableName);
			return Json(new
			{
				Code = result == null ? 0 : 1,
				Info = result
			});
		}

		public JsonResult SetTableDescription(string DbName, string TableName, string Description)
		{
			if (string.IsNullOrWhiteSpace(DbName) || string.IsNullOrWhiteSpace(TableName) ||
			    string.IsNullOrWhiteSpace(Description))
			{
				return Json(new
				{
					Code = 0,
					Info = "参数格式不正确"
				});
			}

			var result = DbMapManager.SetTableDescription(DbName, TableName, Description);
			return Json(new
			{
				Code = result,
				Info = result == 0 ? "参数不正确" : "修改成功"
			});
		}

		public FileResult ExportFile(string tableStr)
		{
			if (string.IsNullOrEmpty(tableStr)) return File("text/plain", "dbd.md");

			var outInfo = "## 数据库描述导出信息\n";
			var tableList = tableStr.Split(';').ToList();
			foreach (var item in tableList)
			{
				var tableInfo = item.Split('/').Where(g => !string.IsNullOrEmpty(g)).ToList();
				if (tableInfo.Count < 2)
				{
					outInfo += $"### 导出 `{item}` 表参数不正确\n\n";
				}
				else
				{
					outInfo += $"### `{item}表信息导出`\n";
					var desc = DbMapManager.FetchTableDescruption(tableInfo[0], tableInfo[1]);
					var data = DbMapManager.FetchTableMap(tableInfo[0], tableInfo[1]);
					desc = string.IsNullOrWhiteSpace(desc) ? "暂无描述" : desc;
					outInfo += $"{DbMapManager.ConvertToMdStr(desc, data)}\n";
				}

			}

			var output = new MemoryStream();
			var writer = new StreamWriter(output, System.Text.Encoding.UTF8);
			writer.Write(outInfo);
			writer.Flush();
			output.Position = 0;
			return File(output, "text/plain", "dbd.md");
		}

	}
}
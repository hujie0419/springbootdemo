using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.DbMap.DataAccess;
using Tuhu.DbMap.Models;

namespace Tuhu.DbMap.Business
{
	public static class DbMapManager
	{
		public static List<TreeViewModel> GetTableList()
		{
			var data = DalDbMap.GetAllTableList();
			var result = data.GroupBy(g => g.DbName).Select(g => new TreeViewModel
			{
				text = g.Key,
				id = "",
				nodes = g.Select(t => new TreeViewModel
				{
					text = t.TableName,
					id = g.Key,
				}).OrderBy(t => t.text)?.ToList()
			}).OrderBy(g => g.text).ToList();
			return result;
		}

		public static List<string> GetDbList(bool isCurrent = true)
		{
			if (!isCurrent)
			{
				return GlobalConstants.DataBaseAccount.Select(g => g.Key).ToList();
			}

			return DalDbMap.GetDbList();
		}

		public static List<string> SelectTablesByDbName(string dbName, bool isCurrent)
			=> DalDbMap.SelectTablesByDbName(dbName, isCurrent);

		public static int AddTableMap(string dbName, string tableName)
		{
			var tableId = DalDbMap.AddDbMap(dbName, tableName);
			var result = 1;
			if (tableId > 0)
			{
				var data = DalDbMap.SelectTableMapList(dbName, tableName);
				if (data.Any())
				{
					foreach (var item in data)
					{
						item.TableId = tableId;
						var dat = DalDbMap.UpdateTableMap(item);
						if (dat < 1)
						{
							var addResult = DalDbMap.AddtableMap(item);
							if (addResult < 1)
							{
								result = 0;
							}
						}
					}

					DalDbMap.DeleteNeedlessmap(data.Select(g => g.FieldName).ToList(), tableId);
				}
			}

			return result;
		}

		public static List<TableMapModel> FetchTableMap(string dbName, string tableName)
			=> DalDbMap.FetchTableMap(dbName, tableName);

		public static int UpdateDescription(int tableId, int fieldId, string description)
		{
			var fieldInfo = DalDbMap.GetTableInfoById(tableId,fieldId);
			var checkResult = DalDbMap.CheckSystemColumnDescript(fieldInfo.Item1, fieldInfo.Item2, fieldInfo.Item3);
			var result =
				DalDbMap.UpdateSystemDescription(fieldInfo.Item1, fieldInfo.Item2, fieldInfo.Item3, description, checkResult);
			return result ? DalDbMap.UpdateDescription(tableId, fieldId, description) : 0;
		}

		public static string FetchTableDescruption(string dbName, string tableName)
			=> DalDbMap.FetchTableDescruption(dbName, tableName);

		public static int SetTableDescription(string dbName, string tableName, string description)
		{
			var checkResult = DalDbMap.CheckSystemTableDescript(dbName, tableName);
			var result = DalDbMap.UpdateSystemDescription(dbName, tableName, null, description, checkResult);
			return result ? DalDbMap.SetTableDescription(dbName, tableName, description) : 0;
		}

		public static string ConvertToMdStr(string desc, List<TableMapModel> data)
		{
			var result = $@"
> {desc}    

|字段名|字段类型|字节数|字段长度|是否可|备注|" + "\n|:----|:----|:----|:----|:----|:----|\n";
			foreach (var item in data)
			{
				var byteCount = item.ByteCount == -1 ? "MAX" : item.ByteCount.ToString();
				var fieldSize = item.FieldSize == -1 ? "MAX" : item.FieldSize.ToString();
				var description = string.IsNullOrWhiteSpace(item.Description)
					? "暂无描述"
					: item.Description.Replace("\t", "").Replace("\n", "");
				result += $@"|{item.FieldName}|{item.FieldType}|{byteCount}|{fieldSize}|{item.IsNullable}|{description}|" + "\n";
			}

			return result;
		}

	}
}
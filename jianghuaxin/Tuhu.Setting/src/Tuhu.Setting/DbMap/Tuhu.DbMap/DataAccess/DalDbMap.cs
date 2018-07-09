using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Tuhu;
using Tuhu.DbMap.Models;

namespace Tuhu.DbMap.DataAccess
{
	public class DalDbMap
	{
		public static List<DbMapModel> GetAllTableList()
		{
			const string sqlStr = @"SELECT  DM.DbName ,
        DM.TableName ,
        DM.PKID AS TableId
FROM    Tuhu_dbd..DbMap AS DM WITH ( NOLOCK );";
			using (var cmd = new SqlCommand(sqlStr))
			{
				return DbHelper.ExecuteSelect<DbMapModel>(cmd)?.ToList() ?? new List<DbMapModel>();
			}
		}

		public static List<string> GetDbList()
		{
			const string sqlStr = @"SELECT DISTINCT DbName FROM Tuhu_dbd..DbMap WITH(NOLOCK)";

			//@"SELECT name FROM sys.databases;";
			List<string> Action(DataTable dt)
			{
				var result = new List<string>();

				if (dt != null && dt.Rows.Count > 0)
				{
					for (var i = 0; i < dt.Rows.Count; i++)
					{
						var value = dt.Rows[i].GetValue<string>("DbName");
						if (!string.IsNullOrWhiteSpace(value))
						{
							result.Add(value);
						}
					}
				}

				return result;
			}

			using (var dbHelper = DbHelper.CreateDbHelper("Gungnir"))
			{
				using (var cmd = new SqlCommand(sqlStr))
				{
					return dbHelper.ExecuteQuery(cmd, Action);
				}
			}
		}

		public static List<string> SelectTablesByDbName(string dbName, bool isCurrent)
		{
			var sqlStr = isCurrent
				? @"SELECT DISTINCT TableName AS Name FROM Tuhu_dbd..DbMap WITH(NOLOCK) WHERE DbName=N'" + dbName + "';"
				: @"select Name from " + dbName + @"..sysobjects where xtype='u' and status>=0;";
			var account = isCurrent ? "Gungnir" : GlobalConstants.DataBaseAccount[dbName];
			if (string.IsNullOrWhiteSpace(account))
			{
				return new List<string>();
			}

			List<string> Action(DataTable dt)
			{
				var result = new List<string>();

				if (dt != null && dt.Rows.Count > 0)
				{
					for (var i = 0; i < dt.Rows.Count; i++)
					{
						var value = dt.Rows[i].GetValue<string>("Name");
						if (!string.IsNullOrWhiteSpace(value))
						{
							result.Add(value);
						}
					}
				}

				return result;
			}

			using (var dbHelper = DbHelper.CreateDbHelper(account))
			{
				using (var cmd = new SqlCommand(sqlStr))
				{
					return dbHelper.ExecuteQuery(cmd, Action);
				}
			}
		}

		public static int AddDbMap(string dbName, string tableName)
		{
			const string sqlStr1 =
				@"SELECT TOP 1 PKID FROM Tuhu_dbd..DbMap WITH(NOLOCK) WHERE DbName=@dbname AND TableName=@tablename;";
			using (var cmd = new SqlCommand(sqlStr1))
			{
				cmd.Parameters.AddWithValue("@dbname", dbName);
				cmd.Parameters.AddWithValue("@tablename", tableName);
				var data = DbHelper.ExecuteScalar(cmd);
				if (data != null)
				{
					Int32.TryParse(data.ToString(), out int value);
					return value;
				}

				const string sqlStr2 =
					@"INSERT INTO Tuhu_dbd..DbMap(DbName, TableName) OUTPUT Inserted.PKID VALUES ( @dbname, @tablename);";
				using (var cmd2 = new SqlCommand(sqlStr2))
				{
					cmd2.Parameters.AddWithValue("@dbname", dbName);
					cmd2.Parameters.AddWithValue("@tablename", tableName);
					var data2 = DbHelper.ExecuteScalar(cmd2);
					Int32.TryParse(data2?.ToString(), out int value);
					return value;
				}
			}

		}

		public static List<TableMapModel> SelectTableMapList(string dbName, string tableName)
		{
			#region SQL

			var sqlStr = @"USE {0};
SELECT	a.colorder AS FieldId ,
		a.name AS FieldName ,
		b.name AS FieldType ,
		COLUMNPROPERTY(a.id, a.name, 'PRECISION') AS FieldSize ,
		a.length AS ByteCount ,
		ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) DecimalSize ,
		( CASE	WHEN a.isnullable = 1 THEN 1
				ELSE 0
			END ) AS IsNullable ,
		ISNULL(e.text, ' ') AS DefaultValue ,
		ISNULL(g.[value], ' ') AS Description ,
		( CASE	WHEN ( SELECT	COUNT(*)
						FROM	sysobjects
						WHERE	( name IN ( SELECT	name
											FROM	sysindexes
											WHERE	( id = a.id )
													AND ( indid IN ( SELECT	indid
																		FROM
																			sysindexkeys
																		WHERE
																			( id = a.id )
																			AND ( colid IN ( SELECT	colid
																								FROM
																									syscolumns
																								WHERE
																									( id = a.id )
																									AND ( name = a.name ) ) ) ) ) ) )
								AND ( xtype = 'PK' )
						) > 0 THEN 1
				ELSE 0
			END ) AS IsPk ,
		( CASE	WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1 THEN 1
				ELSE 0
			END ) AS IsIdentity
FROM	syscolumns a
		LEFT JOIN systypes b ON a.xtype = b.xusertype
		INNER JOIN sysobjects d ON a.id = d.id
									AND d.xtype = 'U'
									AND d.name <> 'dtproperties'
		LEFT JOIN syscomments e ON a.cdefault = e.id
		LEFT JOIN sys.extended_properties g ON a.id = g.major_id
												AND a.colid = g.minor_id
		LEFT JOIN sys.extended_properties f ON d.id = f.class
												AND f.minor_id = 0
		WHERE	d.name = @tablename
ORDER BY a.id ,
		a.colorder;";

			#endregion

			sqlStr = string.Format(sqlStr, dbName);
			if (!GlobalConstants.DataBaseAccount.Keys.Contains(dbName)) return new List<TableMapModel>();
			using (var dbHelper = DbHelper.CreateDbHelper(GlobalConstants.DataBaseAccount[dbName]))
			{
				using (var cmd = new SqlCommand(sqlStr))
				{
					cmd.Parameters.AddWithValue("@tablename", tableName);
					return dbHelper.ExecuteSelect<TableMapModel>(cmd)?.ToList() ?? new List<TableMapModel>();
				}
			}
		}

		public static int UpdateTableMap(TableMapModel model)
		{
			const string sqlStr = @"UPDATE  Tuhu_dbd..TableMap WITH ( ROWLOCK )
SET     TableId = @tableid ,
        FieldId = @fieldid ,
        FieldType = @fieldtype ,
        FieldSize = @fieldsize ,
        ByteCount = @bytecount ,
        DecimalSize = @decimalsize ,
        IsNullable = @isnullable ,
        Description = IIF(LTRIM(Description) <> '', Description, @description) ,
        DefaultValue = @defaultvalue ,
        IsPK = @ispk ,
        IsIdentity = @isidentity
WHERE   TableId = @tableid
        AND FieldName = @fieldname;";
			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@fieldsize", model.FieldSize);
				cmd.Parameters.AddWithValue("@fieldid", model.FieldId);
				cmd.Parameters.AddWithValue("@fieldtype", model.FieldType);
				cmd.Parameters.AddWithValue("@bytecount", model.ByteCount);
				cmd.Parameters.AddWithValue("@decimalsize", model.DecimalSize);
				cmd.Parameters.AddWithValue("@isnullable", model.IsNullable);
				cmd.Parameters.AddWithValue("@defaultvalue", model.DefaultValue);
				cmd.Parameters.AddWithValue("@description", model.Description);
				cmd.Parameters.AddWithValue("@ispk", model.IsPk);
				cmd.Parameters.AddWithValue("@isidentity", model.IsIdentity);
				cmd.Parameters.AddWithValue("@tableid", model.TableId);
				cmd.Parameters.AddWithValue("@fieldname", model.FieldName);
				return DbHelper.ExecuteNonQuery(cmd);
			}
		}

		public static int AddtableMap(TableMapModel model)
		{
			#region SQLStr

			const string sqlStr = @"INSERT	INTO Tuhu_dbd..TableMap
			(TableId,
				FieldId,
				FieldName,
				FieldType,
				FieldSize,
				ByteCount,
				DecimalSize,
				IsNullable,
				DefaultValue,
				Description,
				IsPK,
				IsIdentity
			)
			VALUES(@tableid,
				@fieldid,
				@fieldname,
				@fieldtype,
				@fieldsize,
				@bytecount,
				@decimalsize,
				@isnullable,
				@defaultvalue,
				@description,
				@ispk,
				@isidentity
			); ";

			#endregion

			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@tableid", model.TableId);
				cmd.Parameters.AddWithValue("@fieldid", model.FieldId);
				cmd.Parameters.AddWithValue("@fieldname", model.FieldName);
				cmd.Parameters.AddWithValue("@fieldtype", model.FieldType);
				cmd.Parameters.AddWithValue("@fieldsize", model.FieldSize);
				cmd.Parameters.AddWithValue("@bytecount", model.ByteCount);
				cmd.Parameters.AddWithValue("@decimalsize", model.DecimalSize);
				cmd.Parameters.AddWithValue("@isnullable", model.IsNullable);
				cmd.Parameters.AddWithValue("@defaultvalue", model.DefaultValue);
				cmd.Parameters.AddWithValue("@description", model.Description);
				cmd.Parameters.AddWithValue("@ispk", model.IsPk);
				cmd.Parameters.AddWithValue("@isidentity", model.IsIdentity);
				return DbHelper.ExecuteNonQuery(cmd);
			}
		}


		public static bool DeleteNeedlessmap(List<string> fieldList,int tableId)
		{
			string sqlStr = @"delete Tuhu_dbd.dbo.TableMap with(rowlock)
WHERE TableId = @tableId AND FieldName NOT IN ('" + string.Join("','", fieldList) + "');";
			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@tableId", tableId);
				return DbHelper.ExecuteNonQuery(cmd) > 0;
			}
			
		}
		public static List<TableMapModel> FetchTableMap(string dbName, string tableName)
		{
			#region SqlStr

			const string sqlStr = @"SELECT	T.DbName ,
		T.TableName ,
		S.TableId ,
		S.FieldName ,
		S.FieldId ,
		S.FieldType ,
		S.ByteCount ,
		S.FieldSize ,
		S.DefaultValue ,
		S.DecimalSize ,
		S.IsNullable ,
		S.IsExist ,
		S.IsPK ,
		S.IsIdentity ,
		S.Description
FROM	Tuhu_dbd..DbMap AS T WITH ( NOLOCK )
		LEFT JOIN Tuhu_dbd..TableMap AS S WITH ( NOLOCK ) ON T.PKID = S.TableId
WHERE	T.DbName = @dbname
		AND T.TableName = @tablename;";

			#endregion

			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@dbname", dbName);
				cmd.Parameters.AddWithValue("@tablename", tableName);
				return DbHelper.ExecuteSelect<TableMapModel>(cmd)?.ToList() ?? new List<TableMapModel>();
			}
		}

		public static bool UpdateSystemDescription(string dbName, string tableName, string columnName, string description,bool checkResult)
		{
			if (!GlobalConstants.DataBaseAccount.Keys.Contains(dbName)) return false;
			var methon = checkResult ? "sp_updateextendedproperty" : "sp_addextendedproperty";
			var sqlStr = @"
use {0};
execute {1} N'MS_Description',
                               @desc,
                               N'user',
                               N'dbo',
                               N'table',
                               @tableName,
                               @level2type,
                               @columnName;";
			sqlStr = string.Format(sqlStr, dbName, methon);
			using (var dbHelper = DbHelper.CreateDbHelper(GlobalConstants.DataBaseAccount[dbName]))
			{
				using (var cmd = new SqlCommand(sqlStr))
				{
					cmd.Parameters.AddWithValue("@tableName", tableName);
					cmd.Parameters.AddWithValue("@desc", description);
					cmd.Parameters.AddWithValue("@columnName", string.IsNullOrWhiteSpace(columnName) ? null : columnName);
					cmd.Parameters.AddWithValue("@level2type", string.IsNullOrWhiteSpace(columnName) ? null : "COLUMN");
					return dbHelper.ExecuteNonQuery(cmd) == -1;
				}
			}
		}


		public static int UpdateDescription(int tableId, int fieldId, string description)
		{
			const string sqlStr = @"UPDATE	Tuhu_dbd..TableMap WITH ( ROWLOCK )
SET		Description = @description
WHERE	TableId = @tableid
		AND FieldId = @fieldid;";
			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@description", description);
				cmd.Parameters.AddWithValue("@tableid", tableId);
				cmd.Parameters.AddWithValue("@fieldid", fieldId);
				return DbHelper.ExecuteNonQuery(cmd);
			}
		}

		public static string FetchTableDescruption(string dbName, string tableName)
		{
			const string sqlStr =
				@"select Description from Tuhu_dbd..DbMap with(nolock) where DbName=@dbname and TableName=@tablename;";
			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@dbname", dbName);
				cmd.Parameters.AddWithValue("@tablename", tableName);
				return DbHelper.ExecuteScalar(cmd)?.ToString();
			}
		}

		public static int SetTableDescription(string dbName, string tableName, string description)
		{
			const string sqlStr =
				@"update Tuhu_dbd..DbMap with(rowlock) set Description=@description where DbName=@dbname and TableName=@tablename;";
			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@dbname", dbName);
				cmd.Parameters.AddWithValue("@TableName", tableName);
				cmd.Parameters.AddWithValue("@description", description);
				return DbHelper.ExecuteNonQuery(cmd);
			}
		}

		public static Tuple<string, string, string> GetTableInfoById(int tableId,int fieldId)
		{
			const string sqlStr = @"
select T.DbName,
       T.TableName,
       S.FieldName
from Tuhu_dbd..DbMap as T with (nolock)
    left join Tuhu_dbd..TableMap as S with (nolock)
        on T.PKID = S.TableId
where S.TableId = @tableId
      and S.FieldId = @fieldId;";
			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@fieldId", fieldId);
				cmd.Parameters.AddWithValue("@tableId", tableId);
				return DbHelper.ExecuteQuery(cmd, dt =>
				{
					if (dt == null || dt.Rows.Count < 1) return Tuple.Create<string, string, string>(null, null, null);
					var dbName = dt.Rows[0].GetValue<string>("DbName");
					var tableName = dt.Rows[0].GetValue<string>("TableName");
					var fieldName = dt.Rows[0].GetValue<string>("FieldName");
					return Tuple.Create(dbName, tableName, fieldName);

				});
			}
		}

		public static bool CheckSystemColumnDescript(string dbName, string tableName, string fieldName)
		{
			if (!GlobalConstants.DataBaseAccount.Keys.Contains(dbName)) return false;
			var sqlStr = $@"use {dbName};
select COUNT(g.[value])
from syscolumns a
    inner join sysobjects d
        on a.id = d.id
           and d.xtype = 'U'
           and d.name <> 'dtproperties'
    left join sys.extended_properties g
        on a.id = g.major_id
           and a.colid = g.minor_id
where d.name = @tableName
      and a.name = @fieldName;";
			using (var dbHelper = DbHelper.CreateDbHelper(GlobalConstants.DataBaseAccount[dbName]))
			{
				using (var cmd = new SqlCommand(sqlStr))
				{
					cmd.Parameters.AddWithValue("@tableName", tableName);
					cmd.Parameters.AddWithValue("@fieldName", fieldName);
					var result = dbHelper.ExecuteScalar(cmd);
					int.TryParse(result?.ToString(), out var value);
					return value > 0;
				}
			}
		}

		public static bool CheckSystemTableDescript(string dbName, string tableName)
		{
			if (!GlobalConstants.DataBaseAccount.Keys.Contains(dbName)) return false;
			var sqlStr = $@"
use {dbName};
select COUNT(1)
from sys.extended_properties ds
    left join sysobjects tbs
        on ds.major_id = tbs.id
where ds.minor_id = 0
      and tbs.name = @tableName
      and ds.value is not null";
			using (var dbHelper = DbHelper.CreateDbHelper(GlobalConstants.DataBaseAccount[dbName]))
			{
				using (var cmd = new SqlCommand(sqlStr))
				{
					cmd.Parameters.AddWithValue("@tableName", tableName);
					var result = dbHelper.ExecuteScalar(cmd);
					int.TryParse(result?.ToString(), out var value);
					return value > 0;
				}
			}
		}

	}
}
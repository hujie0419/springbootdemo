using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalMessagePush
	{
		public static List<MessagePush> GetAllMessagePush(SqlConnection connection)
		{
			var sql = "SELECT * FROM Configuration.dbo.tbl_MessagePush WITH (NOLOCK)";
			return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<MessagePush>().ToList();
		}
		public static void Delete(SqlConnection connection, int PKID)
		{
			var sqlParamters = new[] 
            { 
                new SqlParameter("@PKID",PKID)
            };
			SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Configuration.dbo.tbl_MessagePush WHERE PKID=@PKID", sqlParamters);
		}
		public static void Add(SqlConnection connection, MessagePush messagePush)
		{
			var sqlParamters = new[] 
            { 
                new SqlParameter("@EnID",messagePush.EnID??string.Empty),
                new SqlParameter("@MsgTitle",messagePush.MsgTitle??string.Empty),
                new SqlParameter("@MsgContent",messagePush.MsgContent??string.Empty),
                new SqlParameter("@MsgLink",messagePush.MsgLink??string.Empty),
                new SqlParameter("@MsgDescription",messagePush.MsgDescription??string.Empty),
                new SqlParameter("@TotalDuration",messagePush.TotalDuration),
                new SqlParameter("@AheadHour",messagePush.AheadHour)
            };
			SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
				@"insert into Configuration.dbo.tbl_MessagePush(EnID,MsgTitle,MsgContent,MsgLink,MsgDescription,TotalDuration,AheadHour) values (@EnID,@MsgTitle,@MsgContent,@MsgLink,@MsgDescription,@TotalDuration,@AheadHour)", sqlParamters);
		}
		public static void Update(SqlConnection connection, MessagePush messagePush)
		{
			var sqlParamters = new[] 
            { 
                new SqlParameter("@PKID",messagePush.PKID),
                new SqlParameter("@EnID",messagePush.EnID??string.Empty),
                new SqlParameter("@MsgTitle",messagePush.MsgTitle??string.Empty),
                new SqlParameter("@MsgContent",messagePush.MsgContent??string.Empty),
                new SqlParameter("@MsgLink",messagePush.MsgLink??string.Empty),
                new SqlParameter("@MsgDescription",messagePush.MsgDescription??string.Empty),
                new SqlParameter("@TotalDuration",messagePush.TotalDuration),
                new SqlParameter("@AheadHour",messagePush.AheadHour)
            };
			SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
				@"update Configuration.dbo.tbl_MessagePush set EnID=@EnID,MsgTitle=@MsgTitle,MsgContent=@MsgContent,MsgLink=@MsgLink,MsgDescription=@MsgDescription,TotalDuration=@TotalDuration,AheadHour=@AheadHour where PKID=@PKID"
				, sqlParamters);
		}
		public static MessagePush GetMessagePushByID(SqlConnection connection, int PKID)
		{
			MessagePush _MessagePush = null;
			var parameters = new[]
            {
                new SqlParameter("@PKID", PKID)
            };

			using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
      PKID
      ,EnID
      ,MsgTitle
      ,MsgContent
      ,MsgLink
      ,MsgDescription
      ,TotalDuration
      ,AheadHour
FROM Configuration.dbo.tbl_MessagePush WHERE PKID=@PKID", parameters))
			{
				if (_DR.Read())
				{
					_MessagePush = new MessagePush();
					_MessagePush.PKID = _DR.GetTuhuValue<int>(0);
					_MessagePush.EnID = _DR.GetTuhuString(1);
					_MessagePush.MsgTitle = _DR.GetTuhuString(2);
					_MessagePush.MsgContent = _DR.GetTuhuString(3);
					_MessagePush.MsgLink = _DR.GetTuhuString(4);
					_MessagePush.MsgDescription = _DR.GetTuhuString(5);
					_MessagePush.TotalDuration = _DR.GetTuhuValue<int>(6);
					_MessagePush.AheadHour = _DR.GetTuhuValue<int>(7);
				}
			}
			return _MessagePush;
		}
		public static MessagePush GetMessagePushByEnID(SqlConnection connection, string EnID)
		{
			MessagePush _MessagePush = null;
			var parameters = new[]
            {
                new SqlParameter("@EnID", EnID)
            };

			using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
      PKID
      ,EnID
      ,MsgTitle
      ,MsgContent
      ,MsgLink
      ,MsgDescription
      ,TotalDuration
      ,AheadHour
FROM Configuration.dbo.tbl_MessagePush WHERE EnID=@EnID", parameters))
			{
				if (_DR.Read())
				{
					_MessagePush = new MessagePush();
					_MessagePush.PKID = _DR.GetTuhuValue<int>(0);
					_MessagePush.EnID = _DR.GetTuhuString(1);
					_MessagePush.MsgTitle = _DR.GetTuhuString(2);
					_MessagePush.MsgContent = _DR.GetTuhuString(3);
					_MessagePush.MsgLink = _DR.GetTuhuString(4);
					_MessagePush.MsgDescription = _DR.GetTuhuString(5);
					_MessagePush.TotalDuration = _DR.GetTuhuValue<int>(6);
					_MessagePush.AheadHour = _DR.GetTuhuValue<int>(7);
				}
			}
			return _MessagePush;
		}



        public static List<MessagePush> GetAppMessagePush(SqlConnection connection)
        {
            var sql = "SELECT * FROM Configuration.dbo.tbl_AppMessagePush WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<MessagePush>().ToList();
        }

        public static void DeleteAppMessagePush(SqlConnection connection, int PKID)
        {
            var sqlParamters = new[] 
            { 
                new SqlParameter("@PKID",PKID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Configuration.dbo.tbl_AppMessagePush WHERE PKID=@PKID", sqlParamters);
        }

        public static void AddAppMessagePush(SqlConnection connection, MessagePush messagePush)
        {
            var sqlParamters = new[] 
            { 
                new SqlParameter("@EnID",messagePush.EnID??string.Empty),
                new SqlParameter("@MsgTitle",messagePush.MsgTitle??string.Empty),
                new SqlParameter("@MsgContent",messagePush.MsgContent??string.Empty),
                new SqlParameter("@MsgLink",messagePush.MsgLink??string.Empty),
                new SqlParameter("@MsgDescription",messagePush.MsgDescription??string.Empty),
                new SqlParameter("@TotalDuration",messagePush.TotalDuration),
                new SqlParameter("@AheadHour",messagePush.AheadHour)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into Configuration.dbo.tbl_AppMessagePush(EnID,MsgTitle,MsgContent,MsgLink,MsgDescription,TotalDuration,AheadHour) values (@EnID,@MsgTitle,@MsgContent,@MsgLink,@MsgDescription,@TotalDuration,@AheadHour)", sqlParamters);
        }

        public static void UpdateAppMessagePush(SqlConnection connection, MessagePush messagePush)
        {
            var sqlParamters = new[] 
            { 
                new SqlParameter("@PKID",messagePush.PKID),
                new SqlParameter("@EnID",messagePush.EnID??string.Empty),
                new SqlParameter("@MsgTitle",messagePush.MsgTitle??string.Empty),
                new SqlParameter("@MsgContent",messagePush.MsgContent??string.Empty),
                new SqlParameter("@MsgLink",messagePush.MsgLink??string.Empty),
                new SqlParameter("@MsgDescription",messagePush.MsgDescription??string.Empty),
                new SqlParameter("@TotalDuration",messagePush.TotalDuration),
                new SqlParameter("@AheadHour",messagePush.AheadHour)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"update Configuration.dbo.tbl_AppMessagePush set EnID=@EnID,MsgTitle=@MsgTitle,MsgContent=@MsgContent,MsgLink=@MsgLink,MsgDescription=@MsgDescription,TotalDuration=@TotalDuration,AheadHour=@AheadHour where PKID=@PKID"
                , sqlParamters);
        }

        public static MessagePush GetAppMessagePushByID(SqlConnection connection, int PKID)
        {
            MessagePush _MessagePush = null;
            var parameters = new[]
            {
                new SqlParameter("@PKID", PKID)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
                PKID
                ,EnID
                ,MsgTitle
                ,MsgContent
                ,MsgLink
                ,MsgDescription
                ,TotalDuration
                ,AheadHour
                FROM Configuration.dbo.tbl_AppMessagePush WHERE PKID=@PKID", parameters))
            {
                if (_DR.Read())
                {
                    _MessagePush = new MessagePush();
                    _MessagePush.PKID = _DR.GetTuhuValue<int>(0);
                    _MessagePush.EnID = _DR.GetTuhuString(1);
                    _MessagePush.MsgTitle = _DR.GetTuhuString(2);
                    _MessagePush.MsgContent = _DR.GetTuhuString(3);
                    _MessagePush.MsgLink = _DR.GetTuhuString(4);
                    _MessagePush.MsgDescription = _DR.GetTuhuString(5);
                    _MessagePush.TotalDuration = _DR.GetTuhuValue<int>(6);
                    _MessagePush.AheadHour = _DR.GetTuhuValue<int>(7);
                }
            }
            return _MessagePush;
        }

        public static MessagePush GetAppMessagePushByEnID(SqlConnection connection, string EnID)
        {
            MessagePush _MessagePush = null;
            var parameters = new[]
            {
                new SqlParameter("@EnID", EnID)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
              PKID
              ,EnID
              ,MsgTitle
              ,MsgContent
              ,MsgLink
              ,MsgDescription
              ,TotalDuration
              ,AheadHour
            FROM Configuration.dbo.tbl_AppMessagePush WHERE EnID=@EnID", parameters))
            {
                if (_DR.Read())
                {
                    _MessagePush = new MessagePush();
                    _MessagePush.PKID = _DR.GetTuhuValue<int>(0);
                    _MessagePush.EnID = _DR.GetTuhuString(1);
                    _MessagePush.MsgTitle = _DR.GetTuhuString(2);
                    _MessagePush.MsgContent = _DR.GetTuhuString(3);
                    _MessagePush.MsgLink = _DR.GetTuhuString(4);
                    _MessagePush.MsgDescription = _DR.GetTuhuString(5);
                    _MessagePush.TotalDuration = _DR.GetTuhuValue<int>(6);
                    _MessagePush.AheadHour = _DR.GetTuhuValue<int>(7);
                }
            }
            return _MessagePush;
        }
	}
}

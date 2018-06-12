using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalActionsetTab
    {
        public static List<BizActionsetTab> GetAllActionsetTab(SqlConnection connection)
        {
            var sql = @"SELECT id
                      ,action_bgimg
                      ,action_adproimg
                      ,action_explain
                      ,action_rule
                      ,action_label
                      ,action_proname
                      ,action_proID
                      ,action_proimg
                      ,action_needfriendcount
                      ,action_totaljoinperson
                      ,action_totaldownprice
                      ,action_endTime
                      ,action_dealmoney
                      ,action_static
                      ,action_submitTime
                      ,action_updateTime
                      ,action_introurl
                      ,action_endurl FROM Activity.dbo.actionsetTab WITH (NOLOCK) ORDER BY action_static DESC,action_endTime";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<BizActionsetTab>().ToList();
        }


        public static void DeleteActionsetTab(SqlConnection connection, int id)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@id",id)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Activity.dbo.actionsetTab WHERE id=@id", sqlParamters);
        }


        public static void AddActionsetTab(SqlConnection connection, BizActionsetTab actionsetTab)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@action_bgimg",actionsetTab.action_bgimg??""),
                new SqlParameter("@action_adproimg",actionsetTab.action_adproimg??""),
                new SqlParameter("@action_explain",actionsetTab.action_explain??""),
                new SqlParameter("@action_rule",actionsetTab.action_rule??""),
                new SqlParameter("@action_label",actionsetTab.action_label??""),
                new SqlParameter("@action_proname",actionsetTab.action_proname??""),
                new SqlParameter("@action_proID",actionsetTab.action_proID??""),
                new SqlParameter("@action_proimg",actionsetTab.action_proimg??""),
                new SqlParameter("@action_needfriendcount",actionsetTab.action_needfriendcount),
                new SqlParameter("@action_totaljoinperson",actionsetTab.action_totaljoinperson),
                new SqlParameter("@action_totaldownprice",actionsetTab.action_totaldownprice),
                new SqlParameter("@action_endTime",actionsetTab.action_endTime),
                new SqlParameter("@action_dealmoney",actionsetTab.action_dealmoney),
                new SqlParameter("@action_static",actionsetTab.action_static),
                new SqlParameter("@action_submitTime",actionsetTab.action_submitTime),
                new SqlParameter("@action_updateTime",actionsetTab.action_updateTime),
                new SqlParameter("@action_introurl",actionsetTab.action_introurl??string.Empty),
                new SqlParameter("@action_endurl",actionsetTab.action_endurl??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into Activity.dbo.actionsetTab(action_bgimg
                      ,action_adproimg
                      ,action_explain
                      ,action_rule
                      ,action_label
                      ,action_proname
                      ,action_proID
                      ,action_proimg
                      ,action_needfriendcount
                      ,action_totaljoinperson
                      ,action_totaldownprice
                      ,action_endTime
                      ,action_dealmoney
                      ,action_static
                      ,action_submitTime
                      ,action_updateTime
                      ,action_introurl
                      ,action_endurl) 
                values (@action_bgimg
                      ,@action_adproimg
                      ,@action_explain
                      ,@action_rule
                      ,@action_label
                      ,@action_proname
                      ,@action_proID
                      ,@action_proimg
                      ,@action_needfriendcount
                      ,@action_totaljoinperson
                      ,@action_totaldownprice
                      ,@action_endTime
                      ,@action_dealmoney
                      ,@action_static
                      ,@action_submitTime
                      ,@action_updateTime
                      ,@action_introurl
                      ,@action_endurl)"
                , sqlParamters);
        }
        public static void UpdateActionsetTab(SqlConnection connection, BizActionsetTab actionsetTab)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@id",actionsetTab.id),
                new SqlParameter("@action_bgimg",actionsetTab.action_bgimg??""),
                new SqlParameter("@action_adproimg",actionsetTab.action_adproimg??""),
                new SqlParameter("@action_explain",actionsetTab.action_explain??""),
                new SqlParameter("@action_rule",actionsetTab.action_rule??""),
                new SqlParameter("@action_label",actionsetTab.action_label??""),
                new SqlParameter("@action_proname",actionsetTab.action_proname??""),
                new SqlParameter("@action_proID",actionsetTab.action_proID??""),
                new SqlParameter("@action_proimg",actionsetTab.action_proimg??""),
                new SqlParameter("@action_needfriendcount",actionsetTab.action_needfriendcount),
                new SqlParameter("@action_totaljoinperson",actionsetTab.action_totaljoinperson),
                new SqlParameter("@action_totaldownprice",actionsetTab.action_totaldownprice),
                new SqlParameter("@action_endTime",actionsetTab.action_endTime),
                new SqlParameter("@action_dealmoney",actionsetTab.action_dealmoney),
                new SqlParameter("@action_static",actionsetTab.action_static),
                new SqlParameter("@action_updateTime",actionsetTab.action_updateTime),
                new SqlParameter("@action_introurl",actionsetTab.action_introurl??string.Empty),
                new SqlParameter("@action_endurl",actionsetTab.action_endurl??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"update Activity.dbo.actionsetTab set action_bgimg=@action_bgimg,action_adproimg=@action_adproimg,action_explain=@action_explain,action_rule=@action_rule
                    ,action_label=@action_label,action_proname=@action_proname,action_proID=@action_proID,action_proimg=@action_proimg,action_needfriendcount=@action_needfriendcount
,action_totaljoinperson=@action_totaljoinperson,action_totaldownprice=@action_totaldownprice,action_endTime=@action_endTime,action_dealmoney=@action_dealmoney,action_static=@action_static
,action_updateTime=@action_updateTime,action_introurl=@action_introurl,action_endurl=@action_endurl where id=@id"
                , sqlParamters);
        }
        public static BizActionsetTab GetActionsetTabByID(SqlConnection connection, int id)
        {
            BizActionsetTab _ActionsetTab = null;
            var parameters = new[]
            {
                new SqlParameter("@id", id)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
        id
      ,action_bgimg
      ,action_adproimg
      ,action_explain
      ,action_rule
      ,action_label
      ,action_proname
      ,action_proID
      ,action_proimg
      ,action_needfriendcount
      ,action_totaljoinperson
      ,action_totaldownprice
      ,action_endTime
      ,action_dealmoney
      ,action_static
      ,action_submitTime
      ,action_updateTime
      ,action_introurl
      ,action_endurl
FROM Activity.dbo.actionsetTab WHERE id=@id", parameters))
            {
                if (_DR.Read())
                {
                    _ActionsetTab = new BizActionsetTab();
                    _ActionsetTab.id = _DR.GetTuhuValue<long>(0);
                    _ActionsetTab.action_bgimg = _DR.GetTuhuString(1);
                    _ActionsetTab.action_adproimg = _DR.GetTuhuString(2);
                    _ActionsetTab.action_explain = _DR.GetTuhuString(3);
                    _ActionsetTab.action_rule = _DR.GetTuhuString(4);
                    _ActionsetTab.action_label = _DR.GetTuhuString(5);
                    _ActionsetTab.action_proname = _DR.GetTuhuString(6);
                    _ActionsetTab.action_proID = _DR.GetTuhuString(7);
                    _ActionsetTab.action_proimg = _DR.GetTuhuString(8);
                    _ActionsetTab.action_needfriendcount = _DR.GetTuhuValue<int>(9);
                    _ActionsetTab.action_totaljoinperson = _DR.GetTuhuValue<int>(10);
                    _ActionsetTab.action_totaldownprice = _DR.GetTuhuValue<int>(11);
                    _ActionsetTab.action_endTime = _DR.GetTuhuValue<DateTime>(12);
                    _ActionsetTab.action_dealmoney = _DR.GetTuhuValue<decimal>(13);
                    _ActionsetTab.action_static = _DR.GetTuhuValue<bool>(14);
                    _ActionsetTab.action_submitTime = _DR.GetTuhuValue<DateTime>(15);
                    _ActionsetTab.action_updateTime = _DR.GetTuhuValue<DateTime>(16);
                    _ActionsetTab.action_introurl = _DR.GetTuhuString(17);
                    _ActionsetTab.action_endurl = _DR.GetTuhuString(18);
                }
            }
            return _ActionsetTab;
        }
    }
}

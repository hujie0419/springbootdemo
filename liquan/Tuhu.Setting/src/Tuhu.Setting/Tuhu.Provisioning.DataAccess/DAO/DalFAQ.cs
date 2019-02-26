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
    public static class DalFAQ
    {
        public static List<FAQ> SelectBy(SqlConnection connection, string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@Orderchannel",Orderchannel??string.Empty),
                new SqlParameter("@CateOne",CateOne??string.Empty),
                new SqlParameter("@CateTwo",CateTwo??string.Empty),
                new SqlParameter("@CateThree",CateThree??string.Empty),
                new SqlParameter("@Question",string.IsNullOrEmpty(Question)?string.Empty:"'%"+Question+"%'"),
            };
            string _Addsql = string.IsNullOrEmpty(Orderchannel) ? string.Empty : " where Orderchannel=@Orderchannel";
            _Addsql += (string.IsNullOrEmpty(CateOne) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "CateOne=@CateOne");
            _Addsql += (string.IsNullOrEmpty(CateTwo) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "CateTwo=@CateTwo");
            _Addsql += (string.IsNullOrEmpty(CateThree) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "CateThree=@CateThree");
            _Addsql += (string.IsNullOrEmpty(Question) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "Question like @Question");
            var sql = "SELECT * FROM tbl_FAQ " + _Addsql + " WITH (NOLOCK) ORDER BY Orderchannel,CateOne";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<FAQ>().ToList();
        }
        public static List<FAQ> SelectAll(SqlConnection connection)
        {
            var sql = "SELECT * FROM tbl_FAQ WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<FAQ>().ToList();
        }
        public static void Delete(SqlConnection connection, int PKID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM tbl_FAQ WHERE PKID=@PKID", sqlParamters);
        }
        public static void Add(SqlConnection connection, FAQ fAQ)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@Orderchannel",fAQ.Orderchannel??string.Empty),
                new SqlParameter("@CateOne",fAQ.CateOne??string.Empty),
                new SqlParameter("@CateTwo",fAQ.CateTwo??string.Empty),
                new SqlParameter("@CateThree",fAQ.CateThree??string.Empty),
                new SqlParameter("@Question",fAQ.Question??string.Empty),
                new SqlParameter("@Answer",fAQ.Answer??string.Empty),
                new SqlParameter("@EndTime",fAQ.EndTime)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure,
                "proc_AddFaq"
                , sqlParamters);
        }
        public static void Update(SqlConnection connection, FAQ fAQ)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",fAQ.PKID),
                new SqlParameter("@Orderchannel",fAQ.Orderchannel??string.Empty),
                new SqlParameter("@CateOne",fAQ.CateOne??string.Empty),
                new SqlParameter("@CateTwo",fAQ.CateTwo??string.Empty),
                new SqlParameter("@CateThree",fAQ.CateThree??string.Empty),
                new SqlParameter("@Question",fAQ.Question??string.Empty),
                new SqlParameter("@Answer",fAQ.Answer??string.Empty),
                new SqlParameter("@EndTime",  fAQ.EndTime )
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"update tbl_FAQ set Orderchannel=@Orderchannel,CateOne=@CateOne,CateTwo=@CateTwo,CateThree=@CateThree
                    ,Question=@Question,Answer=@Answer,EndTime=@EndTime where PKID=@PKID"
                , sqlParamters);
        }
        public static FAQ GetByPKID(SqlConnection connection, int PKID)
        {
            FAQ _FAQ = null;
            var parameters = new[]
            {
                new SqlParameter("@PKID", PKID)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
       PKID
      ,Orderchannel
      ,CateOne
      ,CateTwo
      ,CateThree
      ,Question
      ,Answer
      ,EndTime
FROM tbl_FAQ WHERE PKID=@PKID", parameters))
            {
                if (_DR.Read())
                {
                    _FAQ = new FAQ();
                    _FAQ.PKID = _DR.GetTuhuValue<int>(0);
                    _FAQ.Orderchannel = _DR.GetTuhuString(1);
                    _FAQ.CateOne = _DR.GetTuhuString(2);
                    _FAQ.CateTwo = _DR.GetTuhuString(3);
                    _FAQ.CateThree = _DR.GetTuhuString(4);
                    _FAQ.Question = _DR.GetTuhuString(5);
                    _FAQ.Answer = _DR.GetTuhuString(6);
                    _FAQ.EndTime = _DR.GetTuhuValue<DateTime>(7);
                }
            }
            return _FAQ;
        }


        public static List<FAQ> TousuSelectBy(SqlConnection connection, string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@Orderchannel",Orderchannel??string.Empty),
                new SqlParameter("@CateOne",CateOne??string.Empty),
                new SqlParameter("@CateTwo",CateTwo??string.Empty),
                new SqlParameter("@CateThree",CateThree??string.Empty),
                new SqlParameter("@Question",string.IsNullOrEmpty(Question)?string.Empty:"'%"+Question+"%'"),
            };
            string _Addsql = string.IsNullOrEmpty(Orderchannel) ? string.Empty : " where Orderchannel=@Orderchannel";
            _Addsql += (string.IsNullOrEmpty(CateOne) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "CateOne=@CateOne");
            _Addsql += (string.IsNullOrEmpty(CateTwo) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "CateTwo=@CateTwo");
            _Addsql += (string.IsNullOrEmpty(CateThree) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "CateThree=@CateThree");
            _Addsql += (string.IsNullOrEmpty(Question) ? string.Empty : (string.IsNullOrEmpty(_Addsql) ? " where " : " and ") + "Question like @Question");
            var sql = "SELECT * FROM tbl_TousuFAQ " + _Addsql + " WITH (NOLOCK) ORDER BY Orderchannel,CateOne";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<FAQ>().ToList();
        }
        public static List<FAQ> TousuSelectAll(SqlConnection connection)
        {
            var sql = "SELECT * FROM tbl_TousuFAQ WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<FAQ>().ToList();
        }
        public static void TousuDelete(SqlConnection connection, int PKID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM tbl_TousuFAQ WHERE PKID=@PKID", sqlParamters);
        }
        public static void TousuAdd(SqlConnection connection, FAQ fAQ)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@Orderchannel",fAQ.Orderchannel??string.Empty),
                new SqlParameter("@CateOne",fAQ.CateOne??string.Empty),
                new SqlParameter("@CateTwo",fAQ.CateTwo??string.Empty),
                new SqlParameter("@CateThree",fAQ.CateThree??string.Empty),
                new SqlParameter("@Question",fAQ.Question??string.Empty),
                new SqlParameter("@Answer",fAQ.Answer??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into tbl_TousuFAQ(Orderchannel,CateOne,CateTwo,CateThree,Question,Answer) values
                 (@Orderchannel,@CateOne,@CateTwo,@CateThree,@Question,@Answer)"
                , sqlParamters);
        }
        public static void TousuUpdate(SqlConnection connection, FAQ fAQ)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",fAQ.PKID),
                new SqlParameter("@Orderchannel",fAQ.Orderchannel??string.Empty),
                new SqlParameter("@CateOne",fAQ.CateOne??string.Empty),
                new SqlParameter("@CateTwo",fAQ.CateTwo??string.Empty),
                new SqlParameter("@CateThree",fAQ.CateThree??string.Empty),
                new SqlParameter("@Question",fAQ.Question??string.Empty),
                new SqlParameter("@Answer",fAQ.Answer??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"update tbl_TousuFAQ set Orderchannel=@Orderchannel,CateOne=@CateOne,CateTwo=@CateTwo,CateThree=@CateThree
                    ,Question=@Question,Answer=@Answer where PKID=@PKID"
                , sqlParamters);
        }
        public static FAQ TousuGetByPKID(SqlConnection connection, int PKID)
        {
            FAQ _FAQ = null;
            var parameters = new[]
            {
                new SqlParameter("@PKID", PKID)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
       PKID
      ,Orderchannel
      ,CateOne
      ,CateTwo
      ,CateThree
      ,Question
      ,Answer
      ,EndTime
FROM tbl_TousuFAQ WHERE PKID=@PKID", parameters))
            {
                if (_DR.Read())
                {
                    _FAQ = new FAQ();
                    _FAQ.PKID = _DR.GetTuhuValue<int>(0);
                    _FAQ.Orderchannel = _DR.GetTuhuString(1);
                    _FAQ.CateOne = _DR.GetTuhuString(2);
                    _FAQ.CateTwo = _DR.GetTuhuString(3);
                    _FAQ.CateThree = _DR.GetTuhuString(4);
                    _FAQ.Question = _DR.GetTuhuString(5);
                    _FAQ.Answer = _DR.GetTuhuString(6);
                    _FAQ.EndTime = _DR.GetTuhuValue<DateTime>(7);
                }
            }
            return _FAQ;
        }
    }
}

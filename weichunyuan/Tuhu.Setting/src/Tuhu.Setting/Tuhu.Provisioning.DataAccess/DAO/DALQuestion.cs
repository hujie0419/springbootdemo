using Dapper;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALQuestion
    {
        public Question GetLastestQustion(SqlConnection conn)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT top 1 qt.* FROM Activity..Question qt WITH (NOLOCK) 
                                                    INNER JOIN Activity..Questionnaire qa WITH(NOLOCK) 
                                                    on qt.QuestionnaireID=qa.PKID
                                                    where qa.QuestionnaireType=5 and qt.IsDeleted=0 order by qt.EndTime desc,qt.PKID
                                                       	;", CommandType.Text).ConvertTo<Question>().FirstOrDefault();
            }
        }
        public  IEnumerable<Question> GetQuestionAnswerList(SqlConnection conn,DateTime enddatetime)
        {
            string sql = @"SELECT qt.* FROM Activity..Question qt WITH (NOLOCK) 
                                                    INNER JOIN Activity..Questionnaire qa WITH(NOLOCK)
                                                    on qt.QuestionnaireID = qa.PKID
                                                    where qa.QuestionnaireType = 5 and datediff(dd,qt.EndTime,@EndTime)=0 and qt.IsDeleted=0";         
            var sqlParam = new[]
         {
                new SqlParameter("@EndTime",enddatetime)               
            };           
            IEnumerable<Question> questionAnswerList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Question>();           
            foreach (var question in questionAnswerList.ToList())
            {
                if (question != null && question.QuestionConfirm != 0)
                {
                    string answersql = "select * from Activity..QuestionOption where IsRightValue = 1 and QuestionID = @id and IsDeleted=0";
                 
                    var answersqlParam = new[]
      {
                new SqlParameter("@id", question.PKID)
            };
                    QuestionOption questionOption = SqlHelper.ExecuteDataTable(conn, CommandType.Text, answersql, answersqlParam).ConvertTo<QuestionOption>().FirstOrDefault();
                    if (questionOption != null)
                    {
                        if (questionOption.OptionContent.Trim().Equals("是", StringComparison.OrdinalIgnoreCase))
                        {
                            question.AnswerYes = 1;
                        }
                        else if (questionOption.OptionContent.Trim().Equals("否", StringComparison.OrdinalIgnoreCase))
                        {
                            question.AnswerYes = -1;
                        }
                    }
                }
            }
            return questionAnswerList;
        }

        public IEnumerable<ActivityPrize> GetActivityPrizeList(SqlConnection conn, string prizeName,int OnSale,  int pagesize, int pageindex)
        {

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT * FROM Activity..tbl_ActivityPrize WITH (NOLOCK) WHERE (ActivityPrizeName=@ActivityPrizeName or @ActivityPrizeName='') and(OnSale=@OnSale or @OnSale=-1) order by IsDeleted,PKID desc
                                                               OFFSET(@PageIndex - 1) * @PageSize ROWS
                                                                                                       FETCH NEXT @PageSize ROWS ONLY
                                                       	;", CommandType.Text,
                                                                                         new SqlParameter[] {
                                                                                               new SqlParameter("@ActivityPrizeName",prizeName),
                                                                                               new SqlParameter("@OnSale",OnSale),
                                                                                               new SqlParameter("@PageIndex",pageindex),
                                                                                               new SqlParameter("@PageSize",pagesize),
                                                                                         }).ConvertTo<ActivityPrize>();
            }
            
        }

        public int GetActivityPrizeCount(SqlConnection conn, string prizeName, int OnSale)
        {
            string sql = @"SELECT count(1) FROM Activity..tbl_ActivityPrize WITH (NOLOCK) WHERE  (ActivityPrizeName=@ActivityPrizeName or @ActivityPrizeName='') and (OnSale=@OnSale or @OnSale=-1)";
            var sqlParam = new[]
           {
                new SqlParameter("@ActivityPrizeName",prizeName),
                  new SqlParameter("@OnSale",OnSale)
            };

            object countObj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam);
            return Convert.ToInt32(countObj);
        }

        public  IEnumerable<Question> GetQuestionList(SqlConnection conn, DateTime enddatetime,int QuestionConfirm,int pagesize,int pageindex,bool useNowTime)
        {


            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                //  pager.TotalItem = GetListCount(dbHelper, model, pager);

                if (useNowTime)
                {
                    return dbHelper.ExecuteDataTable(@"SELECT qt.* FROM Activity..Question qt WITH (NOLOCK) 
                                                    INNER JOIN Activity..Questionnaire qa WITH(NOLOCK) 
                                                    on qt.QuestionnaireID=qa.PKID
                                                    where qa.QuestionnaireType=5 
                                                    and (datediff(dd,qt.EndTime,@EndTime)=0 or datediff(dd,getdate(),@EndTime)=0 ) and (qt.QuestionConfirm=@QuestionConfirm or @QuestionConfirm=-1) order by qt.IsDeleted,qt.EndTime desc,qt.PKID
                                                               OFFSET(@PageIndex - 1) * @PageSize ROWS
                                                                                                       FETCH NEXT @PageSize ROWS ONLY
                                                       	;", CommandType.Text,
                                                                                         new SqlParameter[] {
                                                                                               new SqlParameter("@EndTime",enddatetime),
                                                                                               new SqlParameter("@QuestionConfirm",QuestionConfirm),
                                                                                               new SqlParameter("@PageIndex",pageindex),
                                                                                               new SqlParameter("@PageSize",pagesize),
                                                                                         }).ConvertTo<Question>();
                }
                else
                {
                    return dbHelper.ExecuteDataTable(@"SELECT qt.* FROM Activity..Question qt WITH (NOLOCK) 
                                                    INNER JOIN Activity..Questionnaire qa WITH(NOLOCK) 
                                                    on qt.QuestionnaireID=qa.PKID
                                                    where qa.QuestionnaireType=5 
                                                    and (datediff(dd,qt.EndTime,@EndTime)=0) and (qt.QuestionConfirm=@QuestionConfirm or @QuestionConfirm=-1) order by qt.IsDeleted,qt.EndTime desc,qt.PKID
                                                               OFFSET(@PageIndex - 1) * @PageSize ROWS
                                                                                                       FETCH NEXT @PageSize ROWS ONLY
                                                       	;", CommandType.Text,
                                                                                               new SqlParameter[] {
                                                                                               new SqlParameter("@EndTime",enddatetime),
                                                                                               new SqlParameter("@QuestionConfirm",QuestionConfirm),
                                                                                               new SqlParameter("@PageIndex",pageindex),
                                                                                               new SqlParameter("@PageSize",pagesize),
                                                                                               }).ConvertTo<Question>();
                }
            }           
        }

        public IEnumerable<Question> GetALLQuestionList(SqlConnection conn)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                //  pager.TotalItem = GetListCount(dbHelper, model, pager);

                return dbHelper.ExecuteDataTable(@"SELECT qt.* FROM Activity..Question qt WITH (NOLOCK) 
                                                    INNER JOIN Activity..Questionnaire qa WITH(NOLOCK) 
                                                    on qt.QuestionnaireID=qa.PKID
                                                    where qa.QuestionnaireType=5 and qt.IsDeleted=0                                                   
                                                       	;", CommandType.Text
                                                                                           ).ConvertTo<Question>();
            }
        }


        public int GetQuestionCount(SqlConnection conn, DateTime enddatetime, int QuestionConfirm,bool useNowTime)
        {
            
            string sql = @"SELECT count(1) FROM Activity..Question qt WITH (NOLOCK) 
                        INNER JOIN Activity..Questionnaire qa WITH(NOLOCK) 
                        on qt.QuestionnaireID=qa.PKID
                        where qa.QuestionnaireType=5 
                        and (datediff(dd,qt.EndTime,@EndTime)=0 or datediff(dd,getdate(),@EndTime)=0 ) and (qt.QuestionConfirm=@QuestionConfirm or @QuestionConfirm=-1)";
            if (!useNowTime)
            {
                sql = @"SELECT count(1) FROM Activity..Question qt WITH (NOLOCK) 
                        INNER JOIN Activity..Questionnaire qa WITH(NOLOCK) 
                        on qt.QuestionnaireID=qa.PKID
                        where qa.QuestionnaireType=5 
                        and (datediff(dd,qt.EndTime,@EndTime)=0) and (qt.QuestionConfirm=@QuestionConfirm or @QuestionConfirm=-1)";
            }
            var sqlParam = new[]
           {
                new SqlParameter("@EndTime",enddatetime),
                new SqlParameter("@QuestionConfirm",QuestionConfirm)              
            };

            object countObj= SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam);
            return Convert.ToInt32(countObj);
        }

        public bool UpdateQuestionList(List<Question> questionList)
        {
            bool updatesuccess = true;
            string conn = ConnectionHelper.GetDecryptConn("Gungnir");
            using (var db = new SqlDbHelper(conn))
            {
                try
                {
                    
                    db.BeginTransaction();
                    string questionUpdateSql = string.Empty;
                    foreach (var question in questionList)
                    {
                        if (question.AnswerYes != 0)
                        {
                            question.QuestionConfirm = 1;
                        }
                        questionUpdateSql += "update Activity..Question WITH(ROWLOCK) set QuestionTitle=" +"N\'"+ question.QuestionTitle +"\'"+ ",QuestionTextResult=" + "N\'" + question.QuestionTextResult + "\'" + ",QuestionConfirm="+question.QuestionConfirm+ ",LastUpdateDateTime=" + "\'"+DateTime.Now + "\'" + " where PKID=" + question.PKID;
                        questionUpdateSql += "\n";
                        if (question.AnswerYes == 1)
                        {
                            questionUpdateSql += "update Activity..QuestionOption WITH(ROWLOCK) set IsRightValue=1,LastUpdateDateTime=getdate() where QuestionID=" + question.PKID + " and OptionContent=N'是'";
                            questionUpdateSql += "\n";
                            questionUpdateSql += "update Activity..QuestionOption WITH(ROWLOCK) set IsRightValue=0,LastUpdateDateTime=getdate() where QuestionID=" + question.PKID + " and OptionContent=N'否'";
                        }
                        if (question.AnswerYes == -1)
                        {
                            questionUpdateSql += "update Activity..QuestionOption WITH(ROWLOCK) set IsRightValue=1,LastUpdateDateTime=getdate() where QuestionID=" + question.PKID + " and OptionContent=N'否'";
                            questionUpdateSql += "\n";
                            questionUpdateSql += "update Activity..QuestionOption WITH(ROWLOCK) set IsRightValue=0,LastUpdateDateTime=getdate() where QuestionID=" + question.PKID + " and OptionContent=N'是'";
                        }                    
                    }

                    var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, questionUpdateSql);
                    if (result < 0)
                    {
                        db.Rollback();
                        return false;
                    }
                    else
                    {
                        db.Commit();
                        return true;
                    }
                }

                catch (Exception ex)
                {
                    db.Rollback();
                    return false;
                }
            }
        }

        public bool ReleaseQuestionList(SqlConnection conn, DateTime enddatetime)
        {
            string strSql = @"update Activity..Question  set QuestionConfirm=2,LastUpdateDateTime=getdate() where datediff(dd,EndTime,@EndTime)=0 and QuestionnaireID in 
                              (
                               select PKID from Activity..Questionnaire  where QuestionnaireType = 5
                              )";
            List<SqlParameter> sqlparams = new List<SqlParameter>();
            sqlparams.Add(new SqlParameter("@EndTime", enddatetime));
            int rowaffected = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql, sqlparams.ToArray());
            if (rowaffected >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        

        public List<DateTime> GetQuestionStartTimeList(SqlConnection conn)
        {
            List<DateTime> starttimelist = new List<DateTime>();
            long questionaireId = 0;
            string sql = "select top 1 PKID from Activity..Questionnaire where QuestionnaireType=5";
            object questionaireIdObj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (questionaireIdObj != null)
            {
                questionaireId = Convert.ToInt64(questionaireIdObj);
            }
            string questionstarttimeSql = @"select distinct(starttime)  from Activity..Question where questionnaireid ="+ questionaireId;
          
            DataTable dt=SqlHelper.ExecuteDataTable(conn, CommandType.Text, questionstarttimeSql);
            if (dt!=null && dt.Rows != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow rw= dt.Rows[i];
                    DateTime starttime = Convert.ToDateTime(rw["starttime"]);
                    starttimelist.Add(starttime);
                }
            }
            return starttimelist;
        }

        public List<DateTime> GetQuestionEndTimeList(SqlConnection conn)
        {
            List<DateTime> endtimelist = new List<DateTime>();
            long questionaireId = 0;
            string sql = "select top 1 PKID from Activity..Questionnaire where QuestionnaireType=5";
            object questionaireIdObj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (questionaireIdObj != null)
            {
                questionaireId = Convert.ToInt64(questionaireIdObj);
            }
            string questionendtimeSql = @"select distinct(EndTime)  from Activity..Question where questionnaireid =" + questionaireId;

            DataTable dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, questionendtimeSql);
            if (dt != null && dt.Rows != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow rw = dt.Rows[i];
                    DateTime endtime = Convert.ToDateTime(rw["EndTime"]);
                    endtimelist.Add(endtime);
                }
            }
            return endtimelist;
        }

        public bool SaveQuestionWithOptionList(List<QuestionWithOption> questionWithOptionList)
        {
            var operationsuccess = true;
            string conn = ConnectionHelper.GetDecryptConn("Gungnir");
            using (var db = new SqlDbHelper(conn))
            {
                try
                {                    
                    long questionaireId = 0;
                    db.BeginTransaction();
                    string sql = "select top 1 PKID from Activity..Questionnaire where QuestionnaireType=5";
                    object questionaireIdObj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                    if (questionaireIdObj != null)
                    {
                        questionaireId = Convert.ToInt64(questionaireIdObj);
                    }
                    string questionInsertSql = string.Empty;
                    foreach (var questionwithOption in questionWithOptionList)
                    {

                        questionInsertSql = @"insert into Activity..Question(QuestionnaireID,QuestionTitle,QuestionType,IsRequired,IsValidateStartDate,StartTime,IsValidateEndDate,EndTime,DeadLineTime) values(" + questionaireId + "," + "N\'" + questionwithOption.QuestionTitle + "\'" + "," + questionwithOption.QuestionType + "," + "1" + "," + "1" + "," + "N\'" + questionwithOption.StartTime + "\'" + "," + "1" + "," + "N\'" + questionwithOption.EndTime + "\'" + "," + "N\'" + questionwithOption.DeadLineTime + "\'" + "); SELECT SCOPE_IDENTITY();";
                        var insertquestionidOjb = SqlHelper.ExecuteScalar(conn, CommandType.Text, questionInsertSql);
                        long insertquestionid = 0;
                        if (insertquestionidOjb != null && long.TryParse(insertquestionidOjb.ToString(), out insertquestionid) && insertquestionid > 0)
                        {
                            questionwithOption.PKID = insertquestionid;
                            #region YesOptionInsert
                            var yesquestionInsertSql= @"insert into Activity..QuestionOption(QuestionnaireID, QuestionID, OptionContent) values(" + questionaireId + "," + insertquestionid + "," + "N\'是\'" + "); SELECT SCOPE_IDENTITY();";
                            var yesinsertquestionidOjb = SqlHelper.ExecuteScalar(conn, CommandType.Text, yesquestionInsertSql);
                            long yesinsertquestionid = 0;
                            if (yesinsertquestionidOjb != null && long.TryParse(yesinsertquestionidOjb.ToString(), out yesinsertquestionid) && yesinsertquestionid > 0)
                            {
                                var optionAYescontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.YesOptionAUseIntegral, questionwithOption.YesOptionAWinCouponCount);
                                var optionBYescontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.YesOptionBUseIntegral, questionwithOption.YesOptionBWinCouponCount);
                                var optionCYescontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.YesOptionCUseIntegral, questionwithOption.YesOptionCWinCouponCount);
                                string yesoptionInsertSql = string.Empty;
                                yesoptionInsertSql += @"insert into Activity..QuestionOption(QuestionnaireID,QuestionID,OptionContent,QuestionParentID,UseIntegral,WinCouponCount) values(" + questionaireId + "," + insertquestionid + "," + "N\'" + optionAYescontent + "\'" + ","+ yesinsertquestionid + "," + questionwithOption.YesOptionAUseIntegral + "," + questionwithOption.YesOptionAWinCouponCount + ")";
                                yesoptionInsertSql += "\n";
                                yesoptionInsertSql += @"insert into Activity..QuestionOption(QuestionnaireID,QuestionID,OptionContent,QuestionParentID,UseIntegral,WinCouponCount) values(" + questionaireId + "," + insertquestionid + "," + "N\'" + optionBYescontent + "\'"+"," + yesinsertquestionid + "," + questionwithOption.YesOptionBUseIntegral + "," + questionwithOption.YesOptionBWinCouponCount + ")";
                                yesoptionInsertSql += "\n";
                                yesoptionInsertSql += @"insert into Activity..QuestionOption(QuestionnaireID,QuestionID,OptionContent,QuestionParentID,UseIntegral,WinCouponCount) values(" + questionaireId + "," + insertquestionid + "," + "N\'" + optionCYescontent + "\'" + "," + yesinsertquestionid + "," + questionwithOption.YesOptionCUseIntegral + "," + questionwithOption.YesOptionCWinCouponCount + ")";
                                yesoptionInsertSql += "\n";
                                int affectrows=SqlHelper.ExecuteNonQuery(conn, CommandType.Text, yesoptionInsertSql);
                                if (affectrows < 0)
                                {
                                    db.Rollback();
                                    operationsuccess = false;
                                    return false;
                                }
                            }
                            else
                            {
                                db.Rollback();
                                operationsuccess = false;
                                return false;
                            }
                            #endregion
                            #region NoOptionInsert
                            var noquestionInsertSql = @"insert into Activity..QuestionOption(QuestionnaireID, QuestionID, OptionContent) values(" + questionaireId + "," + insertquestionid + "," + "N\'否\'" + "); SELECT SCOPE_IDENTITY();";
                            var noinsertquestionidOjb = SqlHelper.ExecuteScalar(conn, CommandType.Text, noquestionInsertSql);
                            long noinsertquestionid = 0;
                            if (noinsertquestionidOjb != null && long.TryParse(noinsertquestionidOjb.ToString(), out noinsertquestionid) && noinsertquestionid > 0)
                            {
                                var optionANocontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.NoOptionAUseIntegral, questionwithOption.NoOptionAWinCouponCount);
                                var optionBNocontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.NoOptionBUseIntegral, questionwithOption.NoOptionBWinCouponCount);
                                var optionCNocontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.NoOptionCUseIntegral, questionwithOption.NoOptionCWinCouponCount);
                                string nooptionInsertSql = string.Empty;
                                nooptionInsertSql += @"insert into Activity..QuestionOption(QuestionnaireID,QuestionID,OptionContent,QuestionParentID,UseIntegral,WinCouponCount) values(" + questionaireId + "," + insertquestionid + "," + "N\'" + optionANocontent + "\'" +","+ noinsertquestionid + "," + questionwithOption.YesOptionAUseIntegral + "," + questionwithOption.YesOptionAWinCouponCount + ")";
                                nooptionInsertSql += "\n";
                                nooptionInsertSql += @"insert into Activity..QuestionOption(QuestionnaireID,QuestionID,OptionContent,QuestionParentID,UseIntegral,WinCouponCount) values(" + questionaireId + "," + insertquestionid + "," + "N\'" + optionBNocontent + "\'" + "," + noinsertquestionid + "," + questionwithOption.YesOptionBUseIntegral + "," + questionwithOption.YesOptionBWinCouponCount + ")";
                                nooptionInsertSql += "\n";
                                nooptionInsertSql += @"insert into Activity..QuestionOption(QuestionnaireID,QuestionID,OptionContent,QuestionParentID,UseIntegral,WinCouponCount) values(" + questionaireId + "," + insertquestionid + "," + "N\'" + optionCNocontent + "\'" + "," + noinsertquestionid + "," + questionwithOption.YesOptionCUseIntegral + "," + questionwithOption.YesOptionCWinCouponCount + ")";
                                nooptionInsertSql += "\n";
                                int affectrows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, nooptionInsertSql);
                                if (affectrows < 0)
                                {
                                    db.Rollback();
                                    operationsuccess = false;
                                    return false;
                                }
                            }
                            else
                            {
                                db.Rollback();
                                operationsuccess = false;
                                return false;
                            }
#endregion
                        }
                        else
                        {
                            db.Rollback();
                            operationsuccess = false;
                            return false;
                        }
                        
                      
                    }

                    if(operationsuccess)
                    {
                        db.Commit();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    return false;
                }
            }
            return operationsuccess;
        }

        public bool UpdateQuestionWithOptionList(List<QuestionWithOption> questionWithOptionList)
        {
            var operationsuccess = true;
            string conn = ConnectionHelper.GetDecryptConn("Gungnir");
            using (var db = new SqlDbHelper(conn))
            {
                try
                {
                    long questionaireId = 0;
                    db.BeginTransaction();                  
                    string questionUpdateSql = string.Empty;
                    foreach (var questionwithOption in questionWithOptionList)
                    {

                        questionUpdateSql = "update Activity..Question set QuestionTitle="+"N\'"+ questionwithOption.QuestionTitle+"\'"+ ",StartTime=" + "N\'" + questionwithOption.StartTime + "\'" + ",EndTime=" + "N\'" + questionwithOption.EndTime + "\'" + ",DeadLineTime=" + "N\'" + questionwithOption.DeadLineTime + "\'"+ ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" + questionwithOption.PKID;
                       int questionupdateresult = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, questionUpdateSql);
                        //  long insertquestionid = 0;
                        if (questionupdateresult >= 0)
                        {
                            #region YesOptionUpdate

                            var optionAYescontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.YesOptionAUseIntegral, questionwithOption.YesOptionAWinCouponCount);
                            var optionBYescontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.YesOptionBUseIntegral, questionwithOption.YesOptionBWinCouponCount);
                            var optionCYescontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.YesOptionCUseIntegral, questionwithOption.YesOptionCWinCouponCount);
                            string yesoptionUpdateSql = string.Empty;
                            yesoptionUpdateSql += "Update Activity..QuestionOption set OptionContent=" + "N\'" + optionAYescontent + "\'" + ",UseIntegral=" + questionwithOption.YesOptionAUseIntegral + ",WinCouponCount=" + questionwithOption.YesOptionAWinCouponCount+ ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" +questionwithOption.YesOptionAPKID;
                            yesoptionUpdateSql += "\n";
                            yesoptionUpdateSql += "Update Activity..QuestionOption set OptionContent=" + "N\'" + optionBYescontent + "\'" + ",UseIntegral=" + questionwithOption.YesOptionBUseIntegral + ",WinCouponCount=" + questionwithOption.YesOptionBWinCouponCount + ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" + questionwithOption.YesOptionBPKID;
                            yesoptionUpdateSql += "\n";
                            yesoptionUpdateSql += "Update Activity..QuestionOption set OptionContent=" + "N\'" + optionCYescontent + "\'" + ",UseIntegral=" + questionwithOption.YesOptionCUseIntegral + ",WinCouponCount=" + questionwithOption.YesOptionCWinCouponCount + ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" + questionwithOption.YesOptionCPKID;
                            yesoptionUpdateSql += "\n";
                                int Yesaffectrows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, yesoptionUpdateSql);
                                if (Yesaffectrows < 0)
                                {
                                    db.Rollback();
                                    operationsuccess = false;
                                    return false;
                                }
                           
                            #endregion
                            #region NoOptionInsert
                          
                                var optionANocontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.NoOptionAUseIntegral, questionwithOption.NoOptionAWinCouponCount);
                                var optionBNocontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.NoOptionBUseIntegral, questionwithOption.NoOptionBWinCouponCount);
                                var optionCNocontent = string.Format("使用{0}积分，猜对赢{1}兑换券", questionwithOption.NoOptionCUseIntegral, questionwithOption.NoOptionCWinCouponCount);
                                string nooptionUpdateSql = string.Empty;
                            nooptionUpdateSql += "Update Activity..QuestionOption set OptionContent=" + "N\'" + optionANocontent + "\'" + ",UseIntegral=" + questionwithOption.NoOptionAUseIntegral + ",WinCouponCount=" + questionwithOption.NoOptionAWinCouponCount + ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" + questionwithOption.NoOptionAPKID;
                            nooptionUpdateSql += "\n";
                            nooptionUpdateSql += "Update Activity..QuestionOption set OptionContent=" + "N\'" + optionBNocontent + "\'" + ",UseIntegral=" + questionwithOption.NoOptionBUseIntegral + ",WinCouponCount=" + questionwithOption.NoOptionBWinCouponCount + ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" + questionwithOption.NoOptionBPKID;
                            nooptionUpdateSql += "\n";
                            nooptionUpdateSql += "Update Activity..QuestionOption set OptionContent=" + "N\'" + optionCNocontent + "\'" + ",UseIntegral=" + questionwithOption.NoOptionCUseIntegral + ",WinCouponCount=" + questionwithOption.NoOptionCWinCouponCount + ",LastUpdateDateTime=" + "N\'" + DateTime.Now + "\'" + " where PKID=" + questionwithOption.NoOptionCPKID;
                            nooptionUpdateSql += "\n";
                                int Noaffectrows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, nooptionUpdateSql);
                                if (Noaffectrows < 0)
                                {
                                    db.Rollback();
                                    operationsuccess = false;
                                    return false;
                                }
                            }
                            else
                            {
                                db.Rollback();
                                operationsuccess = false;
                                return false;
                            }
                            #endregion
                      


                    }

                    if (operationsuccess)
                    {
                        db.Commit();

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    return false;
                }
            }
            return operationsuccess;
        }

       public IEnumerable<QuestionWithOption> GetQuestionWithOptionList(SqlConnection conn, DateTime enddatetime,bool isdeleted)
        {
            List<QuestionWithOption> questionWithOptionList = new List<QuestionWithOption>();
            int isdel = Convert.ToInt32(isdeleted);
            //string sql = "SELECT * FROM Activity..Question WITH (NOLOCK) where datediff(dd,EndTime,@EndTime)=0 and IsDeleted=0";
            string sql = @"SELECT qt.* FROM Activity..Question qt WITH (NOLOCK) 
                                                    INNER JOIN Activity..Questionnaire qa WITH(NOLOCK)
                                                    on qt.QuestionnaireID = qa.PKID
                                                    where qa.QuestionnaireType = 5 and datediff(dd,qt.EndTime,@EndTime)=0 and qt.IsDeleted=@IsDeleted";
            var sqlParam = new[]
            {
                new SqlParameter("@EndTime",enddatetime),
                new SqlParameter("@IsDeleted",isdel)
            };
           
            IEnumerable<Question> questionList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<Question>();
            foreach (var question in questionList.ToList())
            {
                var questionWithOption = new QuestionWithOption()
                {
                    QuestionnaireID = question.QuestionnaireID,
                    QuestionTitle = question.QuestionTitle,
                    PKID = question.PKID,
                    StartTime = question.StartTime,
                    EndTime = question.EndTime,
                    DeadLineTime = question.DeadLineTime
                };
                string optionSql = "SELECT * FROM Activity..QuestionOption WITH(NOLOCK) where QuestionID=@QuestionID and QuestionParentID > 0 order by PKID";
                var sqlOptionParam = new[]
            {
                new SqlParameter("@QuestionID",question.PKID)
            };
                IEnumerable<QuestionOption> questionOptionList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, optionSql, sqlOptionParam).ConvertTo<QuestionOption>();
                var YesOptionA = questionOptionList.Take(1).FirstOrDefault();
                var YesOptionB = questionOptionList.Skip(1).Take(1).FirstOrDefault();
                var YesOptionC = questionOptionList.Skip(2).Take(1).FirstOrDefault();
                var NoOptionA = questionOptionList.Skip(3).Take(1).FirstOrDefault();
                var NoOptionB = questionOptionList.Skip(4).Take(1).FirstOrDefault();
                var NoOptionC = questionOptionList.Skip(5).Take(1).FirstOrDefault();
                questionWithOption.YesOptionAPKID= YesOptionA != null ? YesOptionA.PKID : 0;
                questionWithOption.YesOptionAUseIntegral = YesOptionA != null ? YesOptionA.UseIntegral : 0;
                questionWithOption.YesOptionAWinCouponCount = YesOptionA != null ? YesOptionA.WinCouponCount : 0;
                questionWithOption.YesOptionBPKID = YesOptionB != null ? YesOptionB.PKID : 0;
                questionWithOption.YesOptionBUseIntegral = YesOptionB != null ? YesOptionB.UseIntegral : 0;
                questionWithOption.YesOptionBWinCouponCount = YesOptionB != null ? YesOptionB.WinCouponCount : 0;
                questionWithOption.YesOptionCPKID = YesOptionC != null ? YesOptionC.PKID : 0;
                questionWithOption.YesOptionCUseIntegral = YesOptionC != null ? YesOptionC.UseIntegral : 0;
                questionWithOption.YesOptionCWinCouponCount = YesOptionC != null ? YesOptionC.WinCouponCount : 0;
                questionWithOption.NoOptionAPKID = NoOptionA != null ? NoOptionA.PKID : 0;
                questionWithOption.NoOptionAUseIntegral = NoOptionA != null ? NoOptionA.UseIntegral : 0;
                questionWithOption.NoOptionAWinCouponCount = NoOptionA != null ? NoOptionA.WinCouponCount : 0;
                questionWithOption.NoOptionBPKID = NoOptionB != null ? NoOptionB.PKID : 0;
                questionWithOption.NoOptionBUseIntegral = NoOptionB != null ? NoOptionB.UseIntegral : 0;
                questionWithOption.NoOptionBWinCouponCount = NoOptionB != null ? NoOptionB.WinCouponCount : 0;
                questionWithOption.NoOptionCPKID = NoOptionC != null ? NoOptionC.PKID : 0;
                questionWithOption.NoOptionCUseIntegral = NoOptionC != null ? NoOptionC.UseIntegral : 0;
                questionWithOption.NoOptionCWinCouponCount = NoOptionC != null ? NoOptionC.WinCouponCount : 0;
                questionWithOptionList.Add(questionWithOption);
            }
            return questionWithOptionList;
        }

        public bool DeleteQuestionWithOptionList(DateTime enddatetime)
        {
            var operationsuccess = true;
            string conn = ConnectionHelper.GetDecryptConn("Gungnir");
            using (var db = new SqlDbHelper(conn))
            {
                try
                {
                    db.BeginTransaction();
                    string strSql = @"update Activity..Question set IsDeleted=1,LastUpdateDateTime=getdate() where datediff(dd,EndTime,@EndTime)=0 and QuestionnaireID in 
                                      (
                                       select PKID from Activity..Questionnaire where QuestionnaireType = 5
                                      )";
                    List<SqlParameter> sqlparams = new List<SqlParameter>();
                    sqlparams.Add(new SqlParameter("@EndTime", enddatetime));
                    int rowaffected = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql, sqlparams.ToArray());
                    if (rowaffected >= 0)
                    {
                        string updatequestionoptionSql = @"update Activity..QuestionOption set IsDeleted = 1,LastUpdateDateTime=getdate() where QuestionID in (
                           select distinct(pkid) from Activity..Question where
                           datediff(dd, EndTime, @EndTime)= 0)";
                        int rowoptionaffected = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updatequestionoptionSql, sqlparams.ToArray());
                        if (rowoptionaffected < 0)
                        {
                            operationsuccess = false;
                            db.Rollback();
                        }
                    }
                    else
                    {
                        operationsuccess= false;
                        db.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    operationsuccess = false;
                    db.Rollback();
                }
                if (operationsuccess)
                {
                    db.Commit();
                }
                return operationsuccess;
            }
        }

        public bool InsertActivityPrize(SqlConnection conn,ActivityPrize activityPrize)
        {
           // activityPrize.SumStock = activityPrize.Stock=activityPrize.UpdateStock;
            activityPrize.Stock = activityPrize.SumStock;
            int isdelete = Convert.ToInt32(activityPrize.IsDeleted);
            int isDisableSale = Convert.ToInt32(activityPrize.IsDisableSale);
            long activityid = 0; 
            string activityidsql = @"select top 1 PKID from  Activity..tbl_Activity where ActivityType=0";
            object activityidobj=SqlHelper.ExecuteScalar(conn, CommandType.Text, activityidsql);
            if (activityidobj != null)
            {
                activityid = Convert.ToInt64(activityidobj);
            }
            string insertSql = @"insert into Activity..tbl_ActivityPrize(ActivityId,PID,ActivityPrizeName,PicUrl,CouponCount,Stock,SumStock,OnSale,GetRuleId,IsDeleted,IsDisableSale)
                              values(" + activityid + "," + "N\'" + "\'" + "," + "N\'" + activityPrize.ActivityPrizeName + "\'" + "," + "N\'" + activityPrize.PicUrl + "\'" + "," + activityPrize.CouponCount + "," + activityPrize.Stock + "," + activityPrize.SumStock + "," + activityPrize.OnSale + "," + "N\'" + activityPrize.GetRuleId + "\'" + "," + isdelete + "," + isDisableSale + ");SELECT SCOPE_IDENTITY();";
            object prizeobjid=SqlHelper.ExecuteScalar(conn, CommandType.Text, insertSql);
          
            if (prizeobjid != null)
            {
                activityPrize.PKID = Convert.ToInt64(prizeobjid);
            }
            return true;

        }

        public bool UpdateActivityPrize(SqlConnection conn, ActivityPrize activityPrize)
        {
            string strSql = "update Activity..tbl_ActivityPrize set Stock=Stock+@UpdateStock,SumStock=SumStock+@UpdateStock where PKID=@PKID";
            List<SqlParameter> sqlparams = new List<SqlParameter>();
            sqlparams.Add(new SqlParameter("@UpdateStock", activityPrize.UpdateStock));
            sqlparams.Add(new SqlParameter("@PKID", activityPrize.PKID));
            int rowaffected = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql,sqlparams.ToArray());
            if (rowaffected >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteActivityPrize(SqlConnection conn, long pkid)
        {
            string strSql = "update Activity..tbl_ActivityPrize set IsDeleted=1 where PKID=@PKID";
            List<SqlParameter> sqlparams = new List<SqlParameter>();            
            sqlparams.Add(new SqlParameter("@PKID", pkid));
            int rowaffected = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql, sqlparams.ToArray());
            if (rowaffected >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateActivityPrizeSale(SqlConnection conn,int onsale, long pkid)
        {
            string strSql = "update Activity..tbl_ActivityPrize set OnSale=@OnSale where PKID=@PKID";
            List<SqlParameter> sqlparams = new List<SqlParameter>();
            sqlparams.Add(new SqlParameter("@OnSale", onsale));
            sqlparams.Add(new SqlParameter("@PKID", pkid));
            int rowaffected = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql, sqlparams.ToArray());
            if (rowaffected >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActivityPrize GetActivityPrizeByPKID(long pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT * FROM Activity..tbl_ActivityPrize WITH (NOLOCK) WHERE PKID=@PKID                                                              
                                                       	;", CommandType.Text,
                                                                                         new SqlParameter[] {
                                                                                               new SqlParameter("@PKID",pkid)
                                                                                         }).ConvertTo<ActivityPrize>().FirstOrDefault();
            }
        }

    }
}

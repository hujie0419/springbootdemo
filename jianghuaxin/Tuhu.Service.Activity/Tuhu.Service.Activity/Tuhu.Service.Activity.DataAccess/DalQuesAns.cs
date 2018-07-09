using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalQuesAns
    {
        /// <summary>
        /// 获取问答题库信息
        /// </summary>
        /// <returns></returns>
        public async static Task<List<AnswerInfoListModel>> GetAnswerInfoList()
        {
            string sql = "SELECT * FROM Configuration.dbo.AnswerInfoList WITH (NOLOCK) WHERE IsEnabled=1 ";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                var result =  await db.ExecuteSelectAsync<AnswerInfoListModel>(sql);
                return result?.ToList();
            }
        }

        /// <summary>
        /// 插入试卷
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool InsertBigBrandQues(List<BigBrandQuesList> list)
        {
            #region 插入一句
            string sql = @"INSERT INTO Tuhu_log.dbo.BigBrandQuesList
                            ( CreateDateTime ,
                              LastUpdateDateTime ,
                              UserId ,
                              IsFinish ,
                              ResValue ,
                              Tip ,
                              Answer ,
                              OptionsA ,
                              OptionsB ,
                              OptionsC ,
                              OptionsD ,
                              OptionsReal ,
                              OrderBy ,
                              CurAnsNo ,
                              HashKey
                            )
                    VALUES  ( GETDATE() , -- CreateDateTime - datetime
                              GETDATE() , -- LastUpdateDateTime - datetime
                              @UserId , -- UserId - uniqueidentifier
                              @IsFinish , -- IsFinish - bit
                              @ResValue , -- ResValue - varchar(10)
                              @Tip , -- Tip - varchar(50)
                              @Answer , -- Answer - varchar(500)
                              @OptionsA , -- OptionsA - varchar(500)
                              @OptionsB , -- OptionsB - varchar(500)
                              @OptionsC , -- OptionsC - varchar(500)
                              @OptionsD , -- OptionsD - varchar(500)
                              @OptionsReal , -- OptionsReal - varchar(10)
                              @OrderBy , -- OrderBy - int
                              @CurAnsNo , -- CurAnsNo - varchar(50)
                              @HashKey  -- HashKey - varchar(50)
                            )";
            #endregion

            using (var db = DbHelper.CreateLogDbHelper())
            {
                db.BeginTransaction();
                foreach (var model in list)
                {
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@IsFinish", model.IsFinish);
                    cmd.Parameters.AddWithValue("@ResValue", model.ResValue);
                    cmd.Parameters.AddWithValue("@Tip", model.Tip);
                    cmd.Parameters.AddWithValue("@Answer", model.Answer);
                    cmd.Parameters.AddWithValue("@OptionsA", model.OptionsA);
                    cmd.Parameters.AddWithValue("@OptionsB", model.OptionsB);
                    cmd.Parameters.AddWithValue("@OptionsC", model.OptionsC);
                    cmd.Parameters.AddWithValue("@OptionsD", model.OptionsD);
                    cmd.Parameters.AddWithValue("@OptionsReal", model.OptionsReal);
                    cmd.Parameters.AddWithValue("@OrderBy", model.OrderBy);
                    cmd.Parameters.AddWithValue("@CurAnsNo", model.CurAnsNo);
                    cmd.Parameters.AddWithValue("@HashKey", model.HashKey);
                    db.ExecuteNonQuery(cmd);
                }
                try
                {
                    db.Commit();
                    return true;
                }
                catch (Exception em)
                {
                    db.Rollback();
                    throw em;
                }
            }
        }

        /// <summary>
        /// 获取问答试卷信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async static Task<List<BigBrandQuesList>> GetBigBrandQuesList(Guid userId, string hashKey, bool isReady)
        {
            string sql = @"SELECT * FROM Tuhu_log.dbo.BigBrandQuesList AS B WITH (NOLOCK)  WHERE B.CurAnsNo =(
                            SELECT TOP 1  R.CurAnsNo FROM Tuhu_log.dbo.BigBrandQuesList AS R WITH (NOLOCK) WHERE R.UserId=@UserId AND R.HashKey=@HashKey
                            ORDER BY R.CreateDateTime DESC
                            )";
            using (var db = DbHelper.CreateLogDbHelper(isReady))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@HashKey", hashKey);
                return (await db.ExecuteSelectAsync<BigBrandQuesList>(cmd))?.ToList();
            }
        }

        public async static Task<BigBrandQuesList> GetBigBrandQuesEntity(int pkid)
        {
            string sql = @"SELECT  * FROM Tuhu_log.dbo.BigBrandQuesList WITH (NOLOCK) WHERE PKID=@PKID ";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return (await db.ExecuteSelectAsync<BigBrandQuesList>(cmd))?.ToList()?.FirstOrDefault();
            }
        }

        /// <summary>
        /// 更新答题信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="realValue"></param>
        /// <returns></returns>
        public async static Task<bool> UpdateBigBrandQues(int pkid, string realValue)
        {
            string sql = "UPDATE Tuhu_log.dbo.BigBrandQuesList SET IsFinish=1 , LastUpdateDateTime=GETDATE(), ResValue=@ResValue WHERE PKID=@PKID";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ResValue", realValue);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return ( await db.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

    }
}

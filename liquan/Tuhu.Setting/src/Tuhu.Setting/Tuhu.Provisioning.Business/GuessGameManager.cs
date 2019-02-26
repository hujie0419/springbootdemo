using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class GuessGameManager
    {
        DALQuestion dal = null;
        public GuessGameManager()
        {
            dal = new DALQuestion();
        }
        public  IEnumerable<Question> GetQuestionList(DateTime enddatetime, int QuestionConfirm, int pagesize, int pageindex,bool useNowTime)
        {

            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetQuestionList(connection, enddatetime,QuestionConfirm,pagesize,pageindex, useNowTime);
            }
        }

        public IEnumerable<Question> GetALLQuestionList()
        {

            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetALLQuestionList(connection);
            }
        }
        public IEnumerable<ActivityPrize> GetActivityPrizeList(string prizeName, int OnSale, int pagesize, int pageindex)
        {

            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetActivityPrizeList(connection, prizeName, OnSale, pagesize, pageindex);
            }
        }

        public IEnumerable<Question> GetQuestionAnswerList(DateTime enddatetime)
        {

            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetQuestionAnswerList(connection, enddatetime);
            }
        }

        public Question GetLastestQustion()
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetLastestQustion(connection);
            }
        }

        public IEnumerable<QuestionWithOption> GetQuestionWithOptionList(DateTime enddatetime, bool isdeleted)
        {

            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetQuestionWithOptionList(connection, enddatetime,isdeleted);
            }
        }


        public bool ReleaseQuestionList(DateTime enddatetime)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.ReleaseQuestionList(connection, enddatetime);
            }
        }

        public List<DateTime> GetQuestionStartTimeList()
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetQuestionStartTimeList(connection);
            }
        }

        public List<DateTime> GetQuestionEndTimeList()
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetQuestionEndTimeList(connection);
            }
        }

        public bool DeleteQuestionWithOptionList(DateTime enddatetime)
        {

          
                return dal.DeleteQuestionWithOptionList(enddatetime);
           
        }


        public int GetQuestionCount(DateTime enddatetime, int QuestionConfirm,bool useNowTime)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetQuestionCount(connection,enddatetime, QuestionConfirm, useNowTime);
            }
        }

        public int GetActivityPrizeCount(string prizeName, int OnSale)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.GetActivityPrizeCount(connection,prizeName, OnSale);
            }
        }

        public bool UpdateQuestionList(List<Question> questionList)
        {
            return dal.UpdateQuestionList(questionList);
        }

        public bool SaveQuestionWithOptionList(List<QuestionWithOption> questionWithOptionList)
        {
            return dal.SaveQuestionWithOptionList(questionWithOptionList);
        }
        
        public bool UpdateQuestionWithOptionList(List<QuestionWithOption> questionWithOptionList)
        {
            return dal.UpdateQuestionWithOptionList(questionWithOptionList);
        }

        public bool SaveActivityPrize(ActivityPrize activityPrize)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.InsertActivityPrize(connection,activityPrize);
            }
        }

        public bool DeleteActivityPrize(long pkid)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.DeleteActivityPrize(connection, pkid);
            }
        }

        public bool UpdateActivityPrizeSale(int onsale, long pkid)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateActivityPrizeSale(connection, onsale, pkid);
            }
        }
        public bool UpdateActivityPrize(ActivityPrize activityPrize)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateActivityPrize(connection, activityPrize);
            }
        }

        public ActivityPrize GetActivityPrizeByPKID(long pkid)
        {
            return dal.GetActivityPrizeByPKID(pkid);
        }
    }
}

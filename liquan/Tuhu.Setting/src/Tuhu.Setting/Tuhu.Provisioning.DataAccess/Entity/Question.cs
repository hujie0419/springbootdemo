using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class Question
    {
        public long PKID { get; set; }

        public long QuestionnaireID { get; set; }

        public string QuestionTitle { get; set; }

        public int QuestionType { get; set; }
        
        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string CreateDateTime { get; set; }

        public string LastUpdateDateTime { get; set; }

        public bool IsDeleted { get; set; }

        public string QuestionTextResult { get; set; }

        public int QuestionConfirm { get; set; }

        public string DeadLineTime { get; set; }
       
        public int AnswerYes { get; set; }

        public int AnswerNo { get; set; }

    }


    public class QuestionOption
    {
        public long PKID { get; set; }

        public long QuestionnaireID { get; set; }

        public long QuestionID { get; set; }
        public string OptionContent { get; set; }

       

        public string CreateDateTime { get; set; }

        public string LastUpdateDateTime { get; set; }

        public bool IsDeleted { get; set; }

        public long QuestionParentID { get; set; }


        public int WinCouponCount { get; set; }

        public int UseIntegral { get; set; }

     

    }


    public class QuestionWithOption
    {
        public long PKID { get; set; }

        public long QuestionnaireID { get; set; }

        public string QuestionTitle { get; set; }

        public int QuestionType { get; set; } = 3;

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public long YesOptionAPKID { get; set; }
        public int YesOptionAUseIntegral { get; set; }

        public int YesOptionAWinCouponCount { get; set; }
        
        public string YesOptionAContent { get; set; }

        public long YesOptionBPKID { get; set; }
        public int YesOptionBUseIntegral { get; set; }

        public int YesOptionBWinCouponCount { get; set; }

        public string YesOptionBContent { get; set; }

        public long YesOptionCPKID { get; set; }
        public int YesOptionCUseIntegral { get; set; }

        public int YesOptionCWinCouponCount { get; set; }

        public string YesOptionCContent { get; set; }

        public long NoOptionAPKID { get; set; }

        public long YesQuestionParentID { get; set; }


        public int NoOptionAUseIntegral { get; set; }

        public int NoOptionAWinCouponCount { get; set; }

        public string NoOptionAContent { get; set; }

        public long NoOptionBPKID { get; set; }
        public int NoOptionBUseIntegral { get; set; }

        public int NoOptionBWinCouponCount { get; set; }

        public string NoOptionBContent { get; set; }

        public long NoOptionCPKID { get; set; }
        public int NoOptionCUseIntegral { get; set; }

        public int NoOptionCWinCouponCount { get; set; }

        public string NoOptionCContent { get; set; }

        public long NoQuestionParentID { get; set; }

        public bool IsDeleted { get; set; }

        public string DeadLineTime { get; set; }


    }

    public class ActivityPrize
    {
        public long PKID { get; set; }

        public long ActivityId { get; set; }

        public string PID { get; set; }

        public string ActivityPrizeName { get; set; }

        public string PicUrl { get; set; }

        public int CouponCount { get; set; }

        public int Stock { get; set; }

        public int SumStock { get; set; }
       
        public int UpdateStock { get; set; }

        public int OnSale { get; set; }

        public Guid GetRuleId { get; set; } = new Guid();

        public string CreateDatetime { get; set; }

        public string LastUpdateDateTime { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] timestamp { get; set; }

        public bool IsDisableSale { get; set; }
    }
}

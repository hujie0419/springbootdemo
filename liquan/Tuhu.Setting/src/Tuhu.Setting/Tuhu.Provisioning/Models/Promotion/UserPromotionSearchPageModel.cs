using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class UserPromotionSearchPageModel
    {
        public List<SelectPromotionCodeByUserCellPhonePager> UserPromotionList { get; set; }
        public int SumNum { get; set; }

        public string PagerStr { get; set; }

        public string UserCellPhone { get; set; }

        public int PromotionCodeStatus { get; set; }

        public int PageNo { get; set; }
    }
}
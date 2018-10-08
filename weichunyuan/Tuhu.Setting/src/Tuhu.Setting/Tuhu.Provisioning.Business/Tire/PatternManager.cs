using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Tire
{
    public class PatternManager
    {
        public static IEnumerable<string> SelectPatternsByBrand(string brand)
        {
            var dt = DalPattern.SelectPatternsByBrand(brand);

            var list = new List<string>();
            if (dt?.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                {
                    var pattern = row["Pattern"].ToString();
                    if (!string.IsNullOrWhiteSpace(pattern))
                        list.Add(pattern);
                }
            return list;
        }

        public static IEnumerable<TirePatternModel> SelectList(TirePatternModel model, PagerModel pager) => DalPattern.SelectList(model,pager);


        public static int SaveAddMany(TirePatternModel model) => DalPattern.SaveAddMany(model);
        public static int SaveUpdateOrAdd(TirePatternModel model) => DalPattern.SaveUpdateOrAdd(model);
        public static bool CanShow(TirePatternModel model) => DalPattern.CanShow(model);
        public static IEnumerable<PatternArticleModel> SelectPatternForCache(string Pattern) => DalPattern.SelectPatternForCache(Pattern);
        public static int DeletePatternArticle(int PKID)=> DalPattern.DeletePatternArticle(PKID);
        public static string SelectPIDByPattern(string pattern) => DalPattern.SelectPIDByPattern(pattern);
        public static TirePatternModel FetchByPKID(int PKID)=> DalPattern.FetchByPKID(PKID);

        public static IEnumerable<CouponBlackItem> GetCouponBlackList(PagerModel pager, string phoneNum)
            => DalPattern.GetCouponBlackList(pager, phoneNum);
        public static int AddCouponBlackList(string phoneNums)
            => DalPattern.AddCouponBlackList(phoneNums);
        public static int DeleteCouponBlackList(string phoneNum)
            => DalPattern.DeleteCouponBlackList(phoneNum);

        public static IEnumerable<TireBlackListItem> GetTireBlackList(string blackNumber, int type, PagerModel pager)
        {
            if ((type == 2 || (type == 4)) && !string.IsNullOrWhiteSpace(blackNumber))
            {
                if (Guid.TryParse(blackNumber, out Guid value))
                {
                    blackNumber = value.ToString("D");
                }
            }
            return DalPattern.GetTireBlackList(blackNumber, type, pager);
        }
		public static int DeleteTireBlackListItem(string BlackNumber, int Type)
			=> DalPattern.DeleteTireBlackListItem(BlackNumber,Type);
        public static bool CheckTireBlackListItem(string BlackNumber, int Type)
            => DalPattern.CheckTireBlackListItem(BlackNumber, Type);
        public static bool AddTireBlackListItem(string BlackNumber,int Type)
        {
            Guid value = Guid.Empty;
            Regex reg = new Regex(@"^1\d{10}$");
            if ((Type == 2 && !Guid.TryParse(BlackNumber, out value)) || (Type == 1 && !reg.IsMatch(BlackNumber)))
            {
                return false;
            }
            return DalPattern.AddTireBlackListItem(BlackNumber, Type);
        }
        public static bool AddTireBlackListLog(string BlackNumber, string Operator, int Type)
            => DalPattern.AddTireBlackListLog(BlackNumber, Operator, Type);
        public static IEnumerable<TireBlackListLog> BlackListItemHistory(string BlackNumber)
            => DalPattern.BlackListItemHistory(BlackNumber);
        public static IEnumerable<TireBlackListLog> SelectTireBlackListLog(string BlackNumber, PagerModel pager)
             => DalPattern.SelectTireBlackListLog(BlackNumber, pager);      
    }
}

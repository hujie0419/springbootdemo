using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class CategoryBrandRankManager
    {
        public static async Task<IEnumerable<CategoryBrandRankModel>> SelectCategoryBrandRankAsync(DateTime date)
        {

            var list = await DalCategoryBrandRank.SelectCategoryBrandRankAsync(date);

            var dic = list.GroupBy(x => x.ParentPkid).ToDictionary(x => x.Key, x => x.ToList());
            var result = new List<CategoryBrandRankModel>();
            if (dic.ContainsKey(0))
            {
                result = dic[0].OrderBy(x => int.Parse(x.NameIndex ?? "0")).ToList();
                foreach (var item in result)
                {
                    if (dic.ContainsKey(item.PKID))
                    {
                        item.Branks = dic[item.PKID].GroupBy(x => new {x.ParentPkid, x.Date, x.Name}).Select(m =>
                            {
                                var x = m.FirstOrDefault();
                                return new CategoryBrandModel()
                                {
                                    Name = x.Name,
                                    PKID = x.PKID,
                                    NameIndex = x.NameIndex,
                                    ParentPkid = x.ParentPkid
                                };
                            }
                        ).OrderBy(x => int.Parse(x.NameIndex ?? "0")).Take(10).ToList();
                    }
                }
            }
            return result;
        }
    }
}

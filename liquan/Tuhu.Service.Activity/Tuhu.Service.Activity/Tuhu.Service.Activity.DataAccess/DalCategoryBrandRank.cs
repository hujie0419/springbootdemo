using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalCategoryBrandRank
    {
        public static async Task<IEnumerable<CategoryBrandRankModel>> SelectCategoryBrandRankAsync(DateTime date)
        {

            using (var cmd =
                new SqlCommand(
                    $@"SELECT * FROM Activity..tbl_CategoryBrandRank WITH(NOLOCK) WHERE Date=@Date OR ParentPkid=0"))
            {
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                return await DbHelper.ExecuteSelectAsync<CategoryBrandRankModel>(true, cmd);
            }
        }
    }
}

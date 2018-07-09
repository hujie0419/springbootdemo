using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class CategoryBrandRankService: ICategoryBrandRankService
    {
        public Task<OperationResult<IEnumerable<CategoryBrandRankModel>>> SelectAllCategoryBrandByDateAsync(DateTime date)
        {
            return OperationResult.FromResultAsync(CategoryBrandRankManager.SelectCategoryBrandRankAsync(date));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    public interface ISearchWordConvertMgr
    {
        List<SearchWordConvertMapDb> GetAllSearchWord();

        bool DeleteSearchWord(List<SearchWordConvertMapDb> deleteList);

        bool ImportExcelInfoToDb(DataTable tb);
    }
}

using System.Data;
using System.Collections.Generic;

using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    public interface ISearchWordConvertMgr
    {
        List<SearchWordConvertMapDb> GetAllSearchWord(SearchWordConfigType configType);

        bool DeleteSearchWord(List<SearchWordConvertMapDb> deleteList, SearchWordConfigType configType);

        bool ImportExcelInfoToDb(DataTable tb, SearchWordConfigType configType);

        bool UpdateSearchWord(List<SearchWordConvertMapDb> list, SearchWordConfigType configType);
    }
}

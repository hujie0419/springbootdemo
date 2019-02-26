using System.Data;
using System.Configuration;
using System.Collections.Generic;

using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    public class SearchWordConvertMgr : ISearchWordConvertMgr
    {
        static readonly string StrReadConn = ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString;
        static readonly string StrConn = ConfigurationManager.ConnectionStrings["StarterSite_productcatalogConnectionString"].ConnectionString;
        private static readonly IConnectionManager TuhuProductConnectionManager =
            new ConnectionManager(SecurityHelp.IsBase64Formatted(StrConn) ? SecurityHelp.DecryptAES(StrConn) : StrConn);

        private static readonly IConnectionManager TuhuProductConnectionManagerReadOnly =
            new ConnectionManager(SecurityHelp.IsBase64Formatted(StrReadConn) ? SecurityHelp.DecryptAES(StrReadConn) : StrReadConn);

        private static readonly IDBScopeManager TuHuProductDbScopeManager = new DBScopeManager(TuhuProductConnectionManager);

        private static readonly IDBScopeManager TuHuProductDbScopeManagerReadOnly = new DBScopeManager(TuhuProductConnectionManagerReadOnly);

        private readonly SearchWordConvertHandler _searchHandler;

        public SearchWordConvertMgr()
        {
            _searchHandler = new SearchWordConvertHandler(TuHuProductDbScopeManager, TuHuProductDbScopeManagerReadOnly);
        }

        public List<SearchWordConvertMapDb> GetAllSearchWord(SearchWordConfigType configType)
        {
            return _searchHandler.GetAllSearchWord(configType);
        }

        public bool DeleteSearchWord(List<SearchWordConvertMapDb> deleteList, SearchWordConfigType configType)
        {
            return _searchHandler.DeleteSearchWord(deleteList, configType);
        }

        public bool ImportExcelInfoToDb(DataTable tb, SearchWordConfigType configType)
        {
            return _searchHandler.ImportExcelInfoToDb(tb, configType);
        }

        public bool UpdateSearchWord(List<SearchWordConvertMapDb> list, SearchWordConfigType configType)
        {
            return _searchHandler.UpdateSearchWord(list, configType);
        }
    }
}

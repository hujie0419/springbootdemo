using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<SearchWordConvertMapDb> GetAllSearchWord()
        {
            return _searchHandler.GetAllSearchWord();
        }

        public bool DeleteSearchWord(List<SearchWordConvertMapDb> deleteList)
        {
            return _searchHandler.DeleteSearchWord(deleteList);
        }

        public bool ImportExcelInfoToDb(DataTable tb)
        {
            return _searchHandler.ImportExcelInfoToDb(tb);
        }
    }
}

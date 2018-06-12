using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.ProductVehicleInfoDao;
using Tuhu.Provisioning.DataAccess.DAO.SearchConfigDao;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    class SearchWordConvertHandler
    {
        private readonly IDBScopeManager _dbManager, _dbManagerReadOnly;

        public SearchWordConvertHandler(IDBScopeManager dbMgr, IDBScopeManager dbMgrReadOnly)
        {
            this._dbManager = dbMgr;
            this._dbManagerReadOnly = dbMgrReadOnly;
        }

        public List<SearchWordConvertMapDb> GetAllSearchWord()
        {
            try
            {
                Func<SqlConnection, List<SearchWordConvertMapDb>> action = DalSearchWordConvert.GetAllSearchWord;
                return _dbManagerReadOnly.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("检索搜索关键词信息异常", e, "检索异常", MonitorLevel.Critial, MonitorModule.Other);
                return new List<SearchWordConvertMapDb>();
            }

        }

        public bool DeleteSearchWord(List<SearchWordConvertMapDb> delList)
        {
            try
            {
                Func<SqlConnection, bool> action = (connection) => DalSearchWordConvert.DeleteSearchWord(connection, delList);
                return _dbManager.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("删除搜索关键词配置异常", e, "删除异常", MonitorLevel.Critial, MonitorModule.Other);
                return false;
            }
        }

        public bool ImportExcelInfoToDb(DataTable tb)
        {
            var isSuccess = true;
            try
            {
                var dt = new DataTable();
                dt.Columns.Add("TargetWord", typeof(string));
                dt.Columns.Add("SourceWord", typeof(string));
                dt.Columns.Add("CreateTime", typeof(DateTime));

                for (var i = 0; i < tb.Rows.Count; i++)
                {
                    var targetWord = tb.Rows[i][0].ToString();
                    var sourceWord = tb.Rows[i][1].ToString();

                    var dr = dt.NewRow();
                    dr["TargetWord"] = targetWord;
                    dr["SourceWord"] = sourceWord;
                    dr["CreateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }

                Func<SqlConnection, bool> action = (connection) => DalSearchWordConvert.BulkSaveSearchWordInfo(connection, dt);
                _dbManager.Execute(action);

            }
            catch (Exception e)
            {
                isSuccess = false;
                Monitor.ExceptionMonitor.AddNewMonitor("导入Excel配置信息异常", e, "上传异常", MonitorLevel.Critial, MonitorModule.Other);
            }
            return isSuccess;
        }
    }
}

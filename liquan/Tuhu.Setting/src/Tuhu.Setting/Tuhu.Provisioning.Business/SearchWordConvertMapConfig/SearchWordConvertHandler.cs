using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO.SearchConfigDao;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    /// <summary>
    /// 
    /// </summary>
    internal class SearchWordConvertHandler
    {
        private readonly IDBScopeManager _dbManager, _dbManagerReadOnly;

        public SearchWordConvertHandler(IDBScopeManager dbMgr, IDBScopeManager dbMgrReadOnly)
        {
            _dbManager = dbMgr;
            _dbManagerReadOnly = dbMgrReadOnly;
        }

        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public List<SearchWordConvertMapDb> GetAllSearchWord(SearchWordConfigType configType)
        {
            try
            {
                List<SearchWordConvertMapDb> Action(SqlConnection connection) => DalSearchWordConvert.GetAllSearchWord(connection, configType);
                return _dbManagerReadOnly.Execute(Action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("检索搜索关键词信息异常", e, "检索异常", MonitorLevel.Critial);
                return new List<SearchWordConvertMapDb>();
            }

        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="delList"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public bool DeleteSearchWord(List<SearchWordConvertMapDb> delList, SearchWordConfigType configType)
        {
            try
            {
                bool Action(SqlConnection connection) => DalSearchWordConvert.DeleteSearchWord(connection, delList, configType);
                return _dbManager.Execute(Action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("删除搜索关键词配置异常", e, "删除异常", MonitorLevel.Critial);
                return false;
            }
        }

        /// <summary>
        /// 导入Excel数据到数据库
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public bool ImportExcelInfoToDb(DataTable tb, SearchWordConfigType configType)
        {
            if (tb == null || tb.Rows.Count <= 0)
                return true;

            var isSuccess = true;

            try
            {
                var dt = new DataTable();
                dt.Columns.Add("UpdateBy", typeof(string));
                dt.Columns.Add("TargetWord", typeof(string));
                dt.Columns.Add("SourceWord", typeof(string));
                dt.Columns.Add("CreateTime", typeof(DateTime));
                dt.Columns.Add("UpdateTime", typeof(DateTime));
                switch (configType)
                {
                    case SearchWordConfigType.Config:
                        dt.Columns.Add("Tag", typeof(int));
                        break;
                    case SearchWordConfigType.VehicleType:
                        dt.Columns.Add("Type", typeof(string));
                        dt.Columns.Add("VehicleID", typeof(string));
                        dt.Columns.Add("Sort", typeof(int));
                        dt.Columns.Add("TireSize", typeof(string));
                        dt.Columns.Add("SpecialTireSize", typeof(string));
                        dt.Columns.Add("VehicleName", typeof(string));
                        break;
                }
                for (var i = 0; i < tb.Rows.Count; i++)
                {
                    var targetWord = tb.Rows[i][0].ToString();
                    var sourceWord = tb.Rows[i][1].ToString();

                    if (string.IsNullOrEmpty(targetWord) || string.IsNullOrEmpty(sourceWord))
                        continue;

                    var dr = dt.NewRow();
                    dr["TargetWord"] = targetWord;
                    dr["SourceWord"] = sourceWord;
                    dr["CreateTime"] = DateTime.Now;
                    if (configType == SearchWordConfigType.Config)
                    {
                        dr["Tag"] = tb.Rows[i][3];
                        dr["UpdateBy"] = tb.Rows[i][4];
                        dr["UpdateTime"] = DateTime.Now;
                    }
                    if (configType == SearchWordConfigType.VehicleType)
                    {
                        int sort;
                        int.TryParse(tb.Rows[i][4].ToString(), out sort);

                        dr["Type"] = "二级车型";
                        dr["VehicleID"] = tb.Rows[i][3];
                        dr["Sort"] = sort == 0 ? int.MaxValue : sort;
                        dr["TireSize"] = tb.Rows[i][5];
                        dr["SpecialTireSize"] = tb.Rows[i][6];
                        dr["VehicleName"] = tb.Rows[i][7];
                    }
                    dt.Rows.Add(dr);
                }

                bool Action(SqlConnection connection) => DalSearchWordConvert.BulkSaveSearchWordInfo(connection, dt, configType);
                _dbManager.Execute(Action);

            }
            catch (Exception e)
            {
                isSuccess = false;
                Monitor.ExceptionMonitor.AddNewMonitor("导入Excel配置信息异常", e, "上传异常", MonitorLevel.Critial);
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public bool UpdateSearchWord(List<SearchWordConvertMapDb> list, SearchWordConfigType configType)
        {
            try
            {
                bool Action(SqlConnection connection) => DalSearchWordConvert.UpdateSearchWord(connection, list, configType);
                return _dbManager.Execute(Action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("更新搜索关键词配置异常", e, "更新异常", MonitorLevel.Critial);
                return false;
            }
        }
    }
}

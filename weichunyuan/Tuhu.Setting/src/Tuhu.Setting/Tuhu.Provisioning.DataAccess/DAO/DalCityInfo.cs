using Microsoft.ApplicationBlocks.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 获取城市信息
    /// </summary>
    public class DalCityInfo
    {
        #region 单例
        private static DalCityInfo _DalCityInfo = null;
        public static DalCityInfo CreateDalCityInfo
        {
            get
            {
                if (_DalCityInfo == null)
                    return _DalCityInfo = new DalCityInfo();
                else
                    return _DalCityInfo;
            }
        }
        private DalCityInfo() { }
        #endregion

        /// <summary>
        /// 通过parentID获取关联城市信息
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="parentID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public DataTable GetCityInfo(string connectionStr, int parentID, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
            {
                new SqlParameter("@ParentID",SqlDbType.Int)
            };

                sqlParamters[0].Value = parentID;
                var dataSet = SqlHelper.ExecuteDataset(connectionStr, CommandType.StoredProcedure, "csp_GetCityInfo", sqlParamters).Tables[0];
                if (dataSet != null && dataSet.Rows.Count > 0)
                    return dataSet;
                else
                    return null;
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("方法名称:GetCityInfo,存储过程:csp_GetCityInfo=>参数:parentID={1},错误信息:{0}", ex.ToString(), parentID);
                return null;
            }
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.Business
{
    /// <summary>
    /// 实体转换辅助类
    /// </summary>
    public class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// DataTable转换为List集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToModel(DataTable dt)
        {
            try
            {
                // 定义集合
                IList<T> ts = new List<T>();
                // 获得此模型的类型
                System.Type type = typeof(T);
                string tempName = "";
                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();
                    // 获得此模型的公共属性
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        tempName = pi.Name;

                        // 检查DataTable是否包含此列
                        if (dt.Columns.Contains(tempName))
                        {
                            // 判断此属性是否有Setter
                            if (!pi.CanWrite) continue;

                            var dt_DataType = dt.Columns[tempName].DataType.ToString().Trim();
                            var model_DataType = pi.PropertyType.FullName;

                            object value = dr[tempName];
                            if (value != System.DBNull.Value)
                            {
                                if (model_DataType.Equals(dt_DataType))
                                    pi.SetValue(t, value, null);
                                else
                                    pi.SetValue(t, System.Convert.ChangeType(value.ToString(), pi.PropertyType), null);
                            }
                        }
                    }
                    ts.Add(t);
                }
                return ts;
            }
            catch
            {
                return null;
            }
        }
    }
    /// <summary>
    /// 获取城市信息
    /// </summary>
    public class GetCityInfo
    {
        #region 单例
        private static GetCityInfo _GetCityInfo = null;
        public static GetCityInfo CreateGetCityInfo
        {
            get
            {
                if (_GetCityInfo == null)
                    return _GetCityInfo = new GetCityInfo();
                else
                    return _GetCityInfo;
            }
        }
        private GetCityInfo() { }
        #endregion

        /// <summary>
        /// 返回城市集合
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public List<CityModel> GetCityList(int parentID)
        {
            string errerMsg = null;
            try
            {
                var dt = DalCityInfo.CreateDalCityInfo.GetCityInfo(Crm.CrmUserCaseManager.CreateCrmUserCase.connectionStr, parentID, out errerMsg);
                if (dt != null)
                    return (List<CityModel>)ModelConvertHelper<CityModel>.ConvertToModel(dt);
                else
                    return null;
            }
            catch
            {
                return null; ;
            }
        }

        /// <summary>
        /// 返回城市集合JSON
        /// </summary>
        /// <param name="cityList"></param>
        /// <returns>JSON</returns>
        public string GetCityJson(int parentID)
        {
            try
            {
                var cityList = GetCityList(parentID);
                if (cityList != null && cityList.Count > 0)
                    return Newtonsoft.Json.JsonConvert.SerializeObject(cityList);
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

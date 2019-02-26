using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business.CityPaint
{
    public class CityPaintManager
    {
        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager grAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManagerBY = null;
        private readonly IDBScopeManager GRAlwaysOnReadDbScopeManager = null;

        public CityPaintManager()
        {
            dbScopeManagerBY = new DBScopeManager(connectionManager);
            GRAlwaysOnReadDbScopeManager = new DBScopeManager(grAlwaysOnReadConnectionManager);

        }

        /// <summary>
        /// 获取油漆产品全国价列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PaintInfoModel> SelectCountryPaintList()
        {
            DataTable resulTable = DalCityPaint.SelectCountryPaintList();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new PaintInfoModel(dr));
            }
            else
            {
                return new PaintInfoModel[0];
            }
        }

        /// <summary>
        /// 获取已有油漆产品
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PaintModel> SelectPaintList()
        {
            DataTable resulTable = DalCityPaint.SelectPaintList();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new PaintModel(dr));
            }
            else
            {
                return new PaintModel[0];
            }
        }

        /// 根据PKID获取油漆产品全国价信息
        public static PaintInfoModel GetCountryPaintByPKID(int pkid)
        {
            var dr = DalCityPaint.GetCountryPaintByPKID(pkid);
            return new PaintInfoModel(dr);
        }

        /// 根据PID获取油漆产品全国价信息
        public static PaintInfoModel GetCountryPaintByPID(string pid)
        {
            var dr = DalCityPaint.GetCountryPaintByPID(pid);
            return new PaintInfoModel(dr);
        }
        /// <summary>
        /// 更新油漆产品全国价信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCountryPaintModel(PaintInfoModel model)
        {
            return DalCityPaint.UpdateCountryPaint(model);
        }


        /// <summary>
        /// 添加油漆产品全国价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertCountryPaintModelModel(PaintInfoModel model)
        {
            var result = DalCityPaint.InsertCountryPaint(model);
            return result;
        }


        /// <summary>
        /// 根据PKID删除油漆产品全国价
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteCountryPaintModelByPkid(int pkid)
        {
            return DalCityPaint.DeleteCountryPaintByPkid(pkid);
        }

        /// <summary>
        /// 获取油漆产品特殊价列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PaintInfoModel> SelectCityPaintList()
        {
            DataTable resulTable = DalCityPaint.SelectCityPaintList();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new PaintInfoModel(dr));
            }
            else
            {
                return new PaintInfoModel[0];
            }
        }

        /// 根据PKID获取油漆产品特殊价信息
        public static PaintInfoModel GetCityPaintByPKID(int pkid)
        {
            var dr = DalCityPaint.GetCityPaintByPKID(pkid);
            return new PaintInfoModel(dr);
        }

        /// 根据PID和城市ID获取油漆产品特殊价
        public static IEnumerable<PaintInfoModel> GetCityPaintByPIDAndCityId(string pid, int cityId, int pkid)
        {
            DataTable resulTable = DalCityPaint.GetCityPaintByPIDAndCityId(pid, cityId, pkid);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new PaintInfoModel(dr));
            }
            else
            {
                return new PaintInfoModel[0];
            }
        }

        /// <summary>
        /// 更新油漆产品特殊价信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCityPaintModel(PaintInfoModel model)
        {
            return DalCityPaint.UpdateCityPaint(model);
        }

        /// <summary>
        /// 添加油漆产品特殊价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertCityPaintModelModel(PaintInfoModel model)
        {
            var result = DalCityPaint.InsertCityPaint(model);
            return result;
        }

        /// <summary>
        /// 根据PKID删除油漆产品特殊价
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteCityPaintModelByPkid(int pkid)
        {
            return DalCityPaint.DeleteCityPaintByPkid(pkid);
        }

        /// <summary>
        /// 根据PKIDs删除已关联城市
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool DeleteCityPaintByList(List<CityPaintModel> list)
        {
            return DalCityPaint.DeleteCity(list);
        }


        /// <summary>
        /// 批量油漆产品特殊价关联城市
        /// </summary>
        /// <returns></returns>
        public static bool BulkSaveCityPaint(DataTable dt)
        {
            var result = DalCityPaint.BulkSaveCityPaint(dt);
            return result;
        }

        /// <summary>
        /// 根据城市ID获取城市信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RegionCityModel> SelectCityRegionList(string pkids)
        {
            DataTable resulTable = DalCityPaint.SelectCityRegionList(pkids);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new RegionCityModel(dr));
            }
            else
            {
                return new RegionCityModel[0];
            }
        }

        /// <summary>
        /// 根据油漆ID获取油漆已配置城市
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CityPaintModel> SelectPaintCityList(int paintId)
        {
            DataTable resulTable = DalCityPaint.SelectPaintCityList(paintId);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new CityPaintModel(dr));
            }
            else
            {
                return new CityPaintModel[0];
            }
        }

        /// <summary>
        /// 根据油漆PID获取油漆已配置城市
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CityPaintModel> SelectPaintCityListByPid(string pid, int pkid)
        {
            DataTable resulTable = DalCityPaint.SelectPaintCityListByPid(pid, pkid);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new CityPaintModel(dr));
            }
            else
            {
                return new CityPaintModel[0];
            }
        }

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RegionCityModel> SelectAllCityRegionList()
        {
            DataTable resulTable = DalCityPaint.SelectAllCityRegionList();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new RegionCityModel(dr));
            }
            else
            {
                return new RegionCityModel[0];
            }
        }

    }
}

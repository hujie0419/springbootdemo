using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.CityAging;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CityAging
{
   public class CityAgingManage
    {
        private static readonly ILog logger = LogManager.GetLogger<CityAgingManage>();


        /// <summary>
        /// 查询城市时效
        /// </summary>
        /// <returns></returns>
        public static List<CityAgingModel> SelectCityAgingInfo()
        {
            return DaoCityAging.SelectCityAgingInfo();
        }

        /// <summary>
        /// 查询城市时效
        /// </summary>
        /// <returns></returns>
        public static List<CityAgingModel> SelectCityAgingInfoByIds(List<int> Ids)
        {
            return DaoCityAging.SelectCityAgingInfoByIds(Ids);
        }

        /// <summary>
            /// 新增城市时效
            /// </summary>
            /// <param name="cityId"></param>
            /// <param name="cityName"></param>
            /// <param name="isShow"></param>
            /// <param name="title"></param>
            /// <param name="content"></param>
            /// <param name="createUser"></param>
            /// <returns></returns>
            public static int CreateSelectCityAging(int cityId, string cityName, int isShow, string title, string content,
            string createUser)
        {
            return DaoCityAging.CreateSelectCityAging(cityId,  cityName,  isShow,  title,  content,
             createUser);
        }

        /// <summary>
        /// 更新城市时效
        /// </summary>
        /// <param name="pKid"></param>
        /// <param name="isShow"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public static int UpdateSelectCityAging(int pKid, int isShow, string title, string content, string updateUser)
        {
            return DaoCityAging.UpdateSelectCityAging( pKid,  isShow,  title,  content,  updateUser);
        }



    }
}

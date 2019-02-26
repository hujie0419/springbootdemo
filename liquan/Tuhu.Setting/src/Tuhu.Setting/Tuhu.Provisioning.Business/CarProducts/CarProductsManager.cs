using Common.Logging;
using System;
using System.Collections.Generic;

using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CarProducts
{
    /// <summary>
    /// 车品模块服务
    /// </summary>
    public class CarProductsManager
    {
        private static readonly ILog logger = LogManager.GetLogger<CarProductsManager>();

        /// <summary>
        /// 获取banner实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CarProductsBanner GetCarProductsBannerEntity(int id)
        {
            try
            {
                return DALCarProducts.GetCarProductsBannerEntity(id);
            }
            catch (Exception ex)
            {
                logger.Error("GetCarProductsBannerEntity", ex);
            }
            return null;
        }

        /// <summary>
        /// 获取banner实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CarProductsBanner GetCarProductsBannerEntityByFloorId(int floorId)
        {
            try
            {
                return DALCarProducts.GetCarProductsBannerEntityByFloorId(floorId);
            }
            catch (Exception ex)
            {
                logger.Error("GetCarProductsBannerEntityByFloorId", ex);
            }
            return null;
        }

        /// <summary>
        /// 根据类型获取banner列表
        /// </summary>
        /// <returns></returns>
        public List<CarProductsBanner> GetCarProductsBannerList(int type)
        {
            try
            {
                return DALCarProducts.GetCarProductsBannerList(type);
            }
            catch (Exception ex)
            {
                logger.Error("GetCarProductsBannerList", ex);
            }
            return null;
        }

        /// <summary>
        /// 插入banner图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertCarProductsBanner(CarProductsBanner model)
        {
            try
            {
                return DALCarProducts.InsertCarProductsBanner(model);
            }
            catch (Exception ex)
            {
                logger.Error("InsertCarProductsBanner", ex);
            }
            return 0;
        }

        /// <summary>
        /// 更新banner图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarProductsBanner(CarProductsBanner model)
        {
            try
            {
                return DALCarProducts.UpdateCarProductsBanner(model);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateCarProductsBanner", ex);
            }
            return false;
        }

        /// <summary>
        /// 是否存在相同排序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistEqualSort(CarProductsBanner model)
        {
            var result = true;
            try
            {
                result = DALCarProducts.IsExistEqualSort(model);
            }
            catch (Exception ex)
            {
                logger.Error("IsExistEqualSort", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询相同类型banner数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SelectCarProductsBannerCount(CarProductsBanner model)
        {
            var result = 0;
            try
            {
                result = DALCarProducts.SelectCarProductsBannerCount(model);
            }
            catch (Exception ex)
            {
                logger.Error("SelectCarProductsBannerCount", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除banner图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCarProductsBanner(int id)
        {
            try
            {
                return DALCarProducts.DeleteCarProductsBanner(id);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteCarProductsBanner", ex);
            }
            return false;
        }

        /// <summary>
        /// 模块信息列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<CarProductsHomePageModuleConfig> GetHomePageModuleConfigs(int type = 0)
        {
            try
            {
                return DALCarProducts.GetHomePageModuleConfigs(type);
            }
            catch (Exception ex)
            {
                logger.Error("GetHomePageModuleConfigs", ex);
            }
            return null;
        }

        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateHomePageModuleConfig(CarProductsHomePageModuleConfig model)
        {
            try
            {
                return DALCarProducts.UpdateHomePageModuleConfig(model);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateHomePageModuleConfig", ex);
            }
            return false;
        }

        /// <summary>
        /// 获取楼层列表
        /// </summary>
        /// <returns></returns>
        public List<CarProductsFloor> GetFloorList()
        {
            try
            {
                return new DALCarProducts().GetFloorList();
            }
            catch (Exception ex)
            {
                logger.Error("GetFloorList", ex);
            }
            return null;
        }

        /// <summary>
        /// 获取楼层实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CarProductsFloor GetCarProductsFloorEntity(int id)
        {
            try
            {
                return DALCarProducts.GetCarProductsFloorEntity(id);
            }
            catch (Exception ex)
            {
                logger.Error("GetCarProductsFloorEntity", ex);
            }
            return null;
        }

        /// <summary>
        /// 楼层配置列表
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public List<CarProductsFloorConfig> GetFloorConfigList(int floorId)
        {
            try
            {
                return new DALCarProducts().GetFloorConfigList(floorId);
            }
            catch (Exception ex)
            {
                logger.Error("GetFloorConfigList", ex);
            }
            return null;
        }

        /// <summary>
        /// 获取单个楼层配置实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CarProductsFloorConfig GetCarProductsFloorConfigEntity(int id)
        {
            try
            {
                return DALCarProducts.GetCarProductsFloorConfigEntity(id);
            }
            catch (Exception ex)
            {
                logger.Error("GetCarProductsFloorConfigEntity", ex);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public List<CarProductsFloorConfig> GetCarProductsFloorConfigList(int floorId)
        {
            try
            {
                return DALCarProducts.GetCarProductsFloorConfigsList(floorId);
            }
            catch (Exception ex)
            {
                logger.Error("GetCarProductsFloorConfigList", ex);
            }
            return null;
        }

        /// <summary>
        /// 插入banner图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertCarProductsFloor(CarProductsFloor model)
        {
            try
            {
                return DALCarProducts.InsertCarProductsFloor(model);
            }
            catch (Exception ex)
            {
                logger.Error("InsertCarProductsFloor", ex);
            }
            return 0;
        }

        /// <summary>
        /// 更新banner图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarProductsFloor(CarProductsFloor model)
        {
            try
            {
                return DALCarProducts.UpdateCarProductsFloor(model);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateCarProductsFloor");
            }
            return false;
        }


        /// <summary>
        /// 插入banner图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertCarProductsFloorConfig(CarProductsFloorConfig model)
        {
            try
            {
                return DALCarProducts.InsertCarProductsFloorConfig(model);
            }
            catch (Exception ex)
            {
                logger.Error("InsertCarProductsFloorConfig", ex);
            }
            return false;
        }

        /// <summary>
        /// 更新banner图片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarProductsFloorConfig(CarProductsFloorConfig model)
        {
            try
            {
                return DALCarProducts.UpdateCarProductsFloorConfig(model);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateCarProductsFloorConfig", ex);
            }
            return false;
        }

        /// <summary>
        /// 删除楼层
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCarProductsFloor(int id)
        {
            try
            {
                return DALCarProducts.DeleteCarProductsFloor(id);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteCarProductsFloor", ex);
            }
            return false;
        }

        /// <summary>
        /// 删除楼层配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCarProductsFloorConfig(int id)
        {
            try
            {
                return DALCarProducts.DeleteCarProductsFloorConfig(id);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteCarProductsFloorConfig", ex);
            }
            return false;
        }

        /// <summary>
        /// 删除楼层配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCarProductsFloorConfigByFloorId(int floorId)
        {
            try
            {
                return DALCarProducts.DeleteCarProductsFloorConfigByFloorId(floorId);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteCarProductsFloorConfigByFloorId", ex);
            }
            return false;
        }

        /// <summary>
        /// 是否存在相同排序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistEqualFloorSort(CarProductsFloor model)
        {
            var result = true;
            try
            {
                result = DALCarProducts.IsExistEqualFloorSort(model);
            }
            catch (Exception ex)
            {
                logger.Error("IsExistEqualSort", ex);
            }
            return result;
        }

        /// <summary>
        /// 是否存在相同楼层
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistEqualFloorName(CarProductsFloor model)
        {
            var result = true;
            try
            {
                result = DALCarProducts.IsExistEqualFloorName(model);
            }
            catch (Exception ex)
            {
                logger.Error("IsExistEqualFloorName", ex);
            }
            return result;
        }
    }
}

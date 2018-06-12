using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    /// <summary>
    /// 具体的产品业务实现
    /// </summary>
    public class OrderAreaProductManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("OrderAreaManager");
        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddOrderAreaProduct(OrderAreaProduct entity)
        {
            try
            {
                return DALOrderAreaProduct.AddOrderAreaProduct(entity);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaProductException(1, "AddOrderAreaProduct", ex);
                Logger.Log(Level.Error, exception, "AddOrderAreaProduct");
                throw ex;
            }
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateOrderAreaProduct(OrderAreaProduct entity)
        {
            try
            {
                //没有对应的产品信息
                if (DALOrderAreaProduct.SelectSingleOrderAreaProduct(entity.Id) == null)
                    return 0;

                return DALOrderAreaProduct.UpdateOrderAreaProduct(entity);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaProductException(1, "UpdateOrderAreaProduct", ex);
                Logger.Log(Level.Error, exception, "UpdateOrderAreaProduct");
                throw ex;
            }


        }

        /// <summary>
        /// 获取单一的产品信息
        /// </summary>
        /// <param name="PrimaryKey"></param>
        /// <returns></returns>
        public OrderAreaProduct SelectSingleOrderAreaProduct(int PrimaryKey)
        {
            try
            {
                return DALOrderAreaProduct.SelectSingleOrderAreaProduct(PrimaryKey);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaProductException(1, "SelectSingleOrderAreaProduct", ex);
                Logger.Log(Level.Error, exception, "SelectSingleOrderAreaProduct");
                throw ex;
            }
        }

        /// <summary>
        /// 通过appSetDataId 获取产品列表
        /// </summary>
        /// <param name="appSetDataId"></param>
        /// <returns></returns>
        public IEnumerable<OrderAreaProduct> GetProductByOrderAreaId(int orderAreaId)
        {
            try
            {
                return DALOrderAreaProduct.GetProductByOrderAreaId(orderAreaId);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaProductException(1, "GetProductByOrderAreaId", ex);
                Logger.Log(Level.Error, exception, "GetProductByOrderAreaId");
                throw ex;
            }

        }

        /// <summary>
        /// 判断产品id是否唯一存在
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool HasSingleProduct(int productId)
        {
            try
            {
                if (DALOrderAreaProduct.GetOrderAreaProductCountById(productId) == 1)
                    return true;
                return false;
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaProductException(1, "GetOrderAreaProductCountById", ex);
                Logger.Log(Level.Error, exception, "GetOrderAreaProductCountById");
                throw ex;
            }
        }
    }
}

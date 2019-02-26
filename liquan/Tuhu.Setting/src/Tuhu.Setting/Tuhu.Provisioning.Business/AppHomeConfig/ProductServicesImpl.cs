using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.Interface;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.AppHomeConfig
{
    /// <summary>
    /// 具体的产品业务实现
    /// </summary>
    public class ProductServicesImpl : IProductServices
    {
        //日志对象
        //private static readonly ILog logger = LoggerFactory.GetLogger("AppHomeConfig");
        
        //Dao层对象
        private IProductDAL productDAL = ProductFactory.GetProductInstance();

        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddProduct(TblAdProductV2 entity)
        {
            return productDAL.Add(entity);
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateProduct(TblAdProductV2 entity)
        {
            //logger.Log(Level.Debug, entity.ToString());
            //没有对应的产品信息
            if (productDAL.SelectSingle(entity.Id) == null)
                return 0;
            //throw Bussiness Exception
            return productDAL.Update(entity);
           
        }

        /// <summary>
        /// 获取单一的产品信息
        /// </summary>
        /// <param name="PrimaryKey"></param>
        /// <returns></returns>
        public TblAdProductV2 SelectSingleProduct(int PrimaryKey)
        {
            
            return productDAL.SelectSingle(PrimaryKey);
        }

        /// <summary>
        /// 通过appSetDataId 获取产品列表
        /// </summary>
        /// <param name="appSetDataId"></param>
        /// <returns></returns>
        public IEnumerable<TblAdProductV2> GetProductByNewsAppSetDataId(int appSetDataId)
        {
                //logger.Log(Level.Debug, string.Format("获取产品列表的appSetDataId:{0}", appSetDataId));
            return productDAL.GetProductByNewsAppSetDataId(appSetDataId);
            
        }

        /// <summary>
        /// 判断产品id是否唯一存在
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool HasSingleProduct(int productId)
        {
            if (productDAL.GetProductCountById(productId) == 1)
                return true;
            return false;
        }
    }
}

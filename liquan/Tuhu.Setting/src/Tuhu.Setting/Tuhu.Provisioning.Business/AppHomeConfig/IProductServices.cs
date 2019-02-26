using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.AppHomeConfig
{
    /// <summary>
    /// app产品配置业务接口
    /// </summary>
    public interface IProductServices
    {
        /// <summary>
        /// 增加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int AddProduct(TblAdProductV2 entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int UpdateProduct(TblAdProductV2 entity);

        /// <summary>
        /// 获取单一实体对象
        /// </summary>
        /// <param name="PrimaryKey">主键ID，暂时不考虑主键ID是Guid或者其他类型</param>
        /// <returns></returns>
        TblAdProductV2 SelectSingleProduct(int PrimaryKey);

        /// <summary>
        /// 通过 appsetDatId 获取所有的产品信息
        /// </summary>
        /// <param name="appSetDataId"></param>
        /// <returns></returns>
        IEnumerable<TblAdProductV2> GetProductByNewsAppSetDataId(int appSetDataId);

        /// <summary>
        /// 判断产品是否只存在一条
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        bool HasSingleProduct(int productId);
    }
}

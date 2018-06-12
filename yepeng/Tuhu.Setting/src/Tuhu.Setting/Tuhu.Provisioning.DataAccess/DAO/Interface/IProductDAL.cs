using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.Interface
{
    /// <summary>
    /// 产品的接口（对应的可以写产品相关的DAO定义）
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IProductDAL : IBaseDAL<TblAdProductV2>
    {
        /// <summary>
        /// 通过 appsetDatId 获取所有的产品信息
        /// </summary>
        /// <param name="appSetDataId"></param>
        /// <returns></returns>
        IEnumerable<TblAdProductV2> GetProductByNewsAppSetDataId(int appSetDataId);

        /// <summary>
        /// 通过产品id获取产品数量
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        int GetProductCountById(int productId);
    }
}

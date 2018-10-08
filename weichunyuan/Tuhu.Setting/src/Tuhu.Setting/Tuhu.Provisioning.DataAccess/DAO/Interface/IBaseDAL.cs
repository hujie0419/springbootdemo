using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.DAO.Interface
{

    /// <summary>
    /// Dal 接口，包括（CRUD 操作）
    /// </summary>
    public interface IBaseDAL<TEntity>
    {
        /// <summary>
        /// 增加实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Add(TEntity entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Remove(TEntity entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(TEntity entity);

        /// <summary>
        /// 获取单一实体对象
        /// </summary>
        /// <param name="PrimaryKey">主键ID，暂时不考虑主键ID是Guid或者其他类型</param>
        /// <returns></returns>
        TEntity SelectSingle(int PrimaryKey);
    }
}

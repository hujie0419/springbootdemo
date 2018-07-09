using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Impl;
using Tuhu.Provisioning.DataAccess.DAO.Interface;

namespace Tuhu.Provisioning.Business.AppHomeConfig
{
    /// <summary>
    /// Product 实例工厂
    /// </summary>
    public static class ProductFactory
    {

        /// <summary>
        /// 2015年12月9日18:46:50
        /// 通过工厂获取具体的Product DAO层的实例，暂时先考虑在代码中写，以后如果需要更改，可以通过反射方式解决
        /// </summary>
        /// <returns></returns>
        public static IProductDAL GetProductInstance()
        {
            return new ProductDAL();
        }
    }
}

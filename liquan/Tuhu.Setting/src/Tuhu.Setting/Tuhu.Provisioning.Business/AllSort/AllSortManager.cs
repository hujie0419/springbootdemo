using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO.AllSort;
using Tuhu.Provisioning.DataAccess.Entity.AllSort;

namespace Tuhu.Provisioning.Business.AllSort
{
    public class AllSortManager
    {
        /// <summary>
        ///     获取所有全品类分类
        /// </summary>
        public IEnumerable<AllSortConfig> GetList()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalSort();
                return dal.GetList(connection);
            }
        }

        /// <summary>
        ///     获取选中的全品类分类详情
        /// </summary>

        public AllSortConfig GetEntity(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalSort();
                return dal.GetEntity(connection, id);
            }
        }
        
        /// <summary>
        ///     获取选中的全品类分类标签详情
        /// </summary>
        public AllSortTagConfig GetTagEntity(int id,string title)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalSort();
                return dal.GetTagEntity(connection, id,title);
            }
        }

        /// <summary>
        ///     获取选中的全品类内容详情
        /// </summary>
        public AllSortListConfig GetListEntity(int id,string title)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalSort();
                return dal.GetListEntity(connection, id,title);
            }
        }

        /// <summary>
        ///     逻辑删除选中的全品类分类
        /// </summary>
        public bool DeleteEntity(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var dal = new DalSort();
                return dal.DeleteEntity(connection, id);
            }
        }

        /// <summary>
        ///     逻辑删除选中的全品类分类标签
        /// </summary>
        public bool DeleteTagEntity(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var dal = new DalSort();
                return dal.DeleteTagEntity(connection, id);
            }
        }

        /// <summary>
        ///     逻辑删除选中的全品类分类内容
        /// </summary>
        public bool DeleteListEntity(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var dal = new DalSort();
                return dal.DeleteListEntity(connection, id);
            }
        }

        /// <summary>
        ///     新建全品类分类
        /// </summary>
        public bool AddConfig(AllSortConfig model)
        {
            var dal = new DalSort();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddConfig(connection, model);
            }
        }

        /// <summary>
        ///     新建全品类标签
        /// </summary>
        public bool AddTagConfig(AllSortTagConfig model)
        {
            var dal = new DalSort();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddTagConfig(connection, model);
            }
        }

        /// <summary>
        ///     新建全品类内容
        /// </summary>
        public bool AddListConfig(AllSortListConfig model)
        {
            var dal = new DalSort();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddListConfig(connection, model);
            }
        }

        /// <summary>
        ///     修改全品类分类
        /// </summary>

        public bool UpdateConfig(AllSortConfig model)
        {
            var dal = new DalSort();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateEntity(connection, model);
            }
        }

        /// <summary>
        ///     修改全品类标签
        /// </summary>
        public bool UpdateTagConfig(AllSortTagConfig model)
        {
            var dal = new DalSort();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateTagEntity(connection, model);
            }
        }

        /// <summary>
        ///     修改全品类内容
        /// </summary>
        public bool UpdateListConfig(AllSortListConfig model)
        {
            var dal = new DalSort();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateListEntity(connection, model);
            }
        }
    }
}
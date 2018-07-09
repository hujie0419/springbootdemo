using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO.Router;
using Tuhu.Provisioning.DataAccess.Entity.Router;

namespace Tuhu.Provisioning.Business.RouterConfig
{
    public class RouterConfigManager
    {
        public RouterList GetList()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetList(connection);
            }
        }

        public RouterList GetListForApp()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetListForApp(connection);
            }
        }

        public RouterList GetListForWeixin()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetListForWeixin(connection);
            }
        }

        public RouterMainLink GetMainLink(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetMainLink(connection, id);
            }
        }

        public RouterParameter GetParameter(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetParameter(connection, id);
            }
        }

        public IEnumerable<RouterParameter> GetParameterList(int mId)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetParameterList(connection, mId);
            }
        }

        public RouterLink GetLink(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetLink(connection, id);
            }
        }

        /// <summary>
        /// 获取指定id的主参数配对参数构造类
        /// </summary>
        public IEnumerable<RouterLink> GetLinkList(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetLinkList(connection,id);
            }
        }
        public bool DeleteMainLink(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var dal = new DalRouter();
                return dal.DeleteMainLink(connection, id);
            }
        }

        public bool DeleteParameter(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var dal = new DalRouter();
                return dal.DeleteParameter(connection, id);
            }
        }

        public bool DeleteLink(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var dal = new DalRouter();
                return dal.DeleteLink(connection, id);
            }
        }

        /// <summary>
        /// 新增主链接
        /// </summary>
        public bool AddMainLink(RouterMainLink model)
        {
            var dal = new DalRouter();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddMainLink(connection, model);
            }
        }
        
        /// <summary>
        /// 新增参数
        /// </summary>
        public bool AddParameter(RouterParameter model)
        {
            var dal = new DalRouter();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddParameter(connection, model);
            }
        }
        /// <summary>
        /// 新增链接关系
        /// </summary>
        public bool AddLink(RouterLink model)
        {
            var dal = new DalRouter();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddLink(connection, model);
            }
        }
        /// <summary>
        /// 修改主链接
        /// </summary>
        public bool UpdateMainLink(RouterMainLink model)
        {
            var dal = new DalRouter();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateMainLink(connection, model);
            }
        }

        /// <summary>
        /// 修改参数
        /// </summary>
        public bool UpdateParameter(RouterParameter model)
        {
            var dal = new DalRouter();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateParameter(connection, model);
            }
        }

        /// <summary>
        /// 修改完整链接
        /// </summary>
        public bool UpdateLink(RouterLink model)
        {
            var dal = new DalRouter();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateLink(connection, model);
            }
        }

       
    }
}

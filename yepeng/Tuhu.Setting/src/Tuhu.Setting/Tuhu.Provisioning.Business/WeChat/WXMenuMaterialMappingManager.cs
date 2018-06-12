using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class WXMenuMaterialMappingManager
    {
        private DALWXMenuMaterialMapping dal = null;
        public WXMenuMaterialMappingManager()
        {
            dal = new DALWXMenuMaterialMapping();
        }
        /// <summary>
        /// 新增微信菜单和素材关系信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddWXMenuMaterialMapping(WXMenuMaterialMappingModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddWXMenuMaterialMapping(conn, model);
            }
        }
        /// <summary>
        /// 获取微信菜单和素材关系列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<WXMenuMaterialMappingModel> GetWXMenuMaterialMappingList(long objectId, string objectType)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetWXMenuMaterialMappingList(conn, objectId, objectType);
            }
        }
        /// <summary>
        /// 获取微信菜单和素材关系详情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public WXMenuMaterialMappingModel GetWXMenuMaterialMappingInfo(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetWXMenuMaterialMappingInfo(conn, pkid);
            }
        }
        /// <summary>
        /// 删除微信菜单和素材关系详情
        /// </summary>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMenuMaterialMapping(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteWXMenuMaterialMapping(conn, pkid);
            }
        }
        /// <summary>
        /// 删除微信菜单和素材关系详情
        /// </summary>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMenuMaterialMapping(long objectId, string objectType)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteWXMenuMaterialMapping(conn, objectId, objectType);
            }
        }


    }
}

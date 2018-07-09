using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class WXMaterialTextManager
    {
        private DALWXMaterialText dal = null;
        public WXMaterialTextManager()
        {
            dal = new DALWXMaterialText();
        }
        /// <summary>
        /// 新增文字素材
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddWXMaterialText(WXMaterialTextModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddWXMaterialText(conn, model);
            }
        }
        /// <summary>
        /// 获取文字素材列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public List<WXMaterialTextModel> GetWXMaterialTextList(long objectId, string objectType)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetWXMaterialTextList(conn,objectId, objectType);
            }
        }
        /// <summary>
        /// 获取文字素材列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public WXMaterialTextModel GetWXMaterialTextInfo(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetWXMaterialTextInfo(conn, pkid);
            }
        }
        /// <summary>
        /// 删除文字素材
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMaterialText(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteWXMaterialText(conn, pkid);
            }
        }
        /// <summary>
        /// 删除文字素材
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMaterialText(long objectId, string objectType)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteWXMaterialText(conn, objectId, objectType);
            }
        }
    }
}

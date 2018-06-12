using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.MemberPage;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;

namespace Tuhu.Provisioning.Business.MemberPage
{
    public class MemberPageModuleContentManager
    {
        private DALMemberPageModuleContent contentDAL = null;
        private DALMemberPageChannel channelDAL = null;
        public MemberPageModuleContentManager()
        {
            contentDAL = new DALMemberPageModuleContent();
            channelDAL = new DALMemberPageChannel();
        }
        /// <summary>
        /// 获取模块内容详细信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public MemberPageModuleContentModel GetMemberPageModuleContentInfo(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                var contentInfo = contentDAL.GetMemberPageModuleContentInfo(conn, pkid);
                contentInfo.ChannelList = channelDAL.GetMemberPageChannelList(conn, pkid);
                return contentInfo;
            }
        }
        /// <summary>
        /// 新增模块内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMemberPageModuleContent(MemberPageModuleContentModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return contentDAL.AddMemberPageModuleContent(conn, model);
            }
        }
        /// <summary>
        /// 修改模块内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateMemberPageModuleContent(MemberPageModuleContentModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return contentDAL.UpdateMemberPageModuleContent(conn, model);
            }
        }
        /// <summary>
        /// 删除模块内容
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteMemberPageModuleContent(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return contentDAL.DeleteMemberPageModuleContent(conn, pkid);
            }
        }
        /// <summary>
        /// 根据模块标识删除模块内容
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageModuleID"></param>
        /// <returns></returns>
        public bool DeleteMemberPageModuleContentByModuleID(long memberPageModuleID)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return contentDAL.DeleteMemberPageModuleContentByModuleID(conn, memberPageModuleID);
            }
        }
    }
}

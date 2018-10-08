using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.MemberPage;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;

namespace Tuhu.Provisioning.Business.MemberPage
{
    public class MemberPageChannelManager
    {
        private DALMemberPageChannel dal = null;
        public MemberPageChannelManager()
        {
            dal = new DALMemberPageChannel();
        }
        /// <summary>
        /// 根据内容ID查询渠道列表
        /// </summary>
        /// <param name="memberPageModuleContentID"></param>
        /// <returns></returns>
        public List<MemberPageChannelModel> GetMemberPageChannelList(long memberPageModuleContentID)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetMemberPageChannelList(conn, memberPageModuleContentID);
            }
        }
        /// <summary>
        /// 新增个人中心页面配置渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMemberPageChannel(MemberPageChannelModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddMemberPageChannel(conn, model);
            }
        }
        /// <summary>
        /// 修改个人中心页面配置渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateMemberPageChannel(MemberPageChannelModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateMemberPageChannel(conn, model);
            }
        }
        /// <summary>
        /// 删除个人中心页面配置渠道
        /// </summary>
        /// <param name="memberPageModuleContentID"></param>
        /// <returns></returns>
        public bool DeleteMemberPageChannel(long memberPageModuleContentID)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteMemberPageChannel(conn, memberPageModuleContentID);
            }
        }
        /// <summary>
        /// 根据模块标识删除个人中心页面配置渠道
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="memberPageModuleID"></param>
        /// <returns></returns>
        public bool DeleteMemberPageChannelByModuleID(long memberPageModuleID)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteMemberPageChannelByModuleID(conn, memberPageModuleID);
            }
        }
    }
}

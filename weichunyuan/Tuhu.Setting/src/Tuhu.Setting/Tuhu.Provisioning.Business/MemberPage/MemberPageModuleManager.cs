using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Provisioning.DataAccess.DAO.MemberPage;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Business.MemberPage
{
    public class MemberPageModuleManager
    {
        private DALMemberPageModule moduleDAL = null;
        private DALMemberPageModuleContent contentDAL = null;
        public MemberPageModuleManager()
        {
            moduleDAL = new DALMemberPageModule();
            contentDAL = new DALMemberPageModuleContent();
        }
        /// <summary>
        /// 获取模块和模块内容列表
        /// </summary>
        /// <param name="pageCode">页面编码，member=个人中心，more=更多</param>
        /// <returns></returns>
        public List<MemberPageModuleListModel> GetMemberPageModuleList(string pageCode)
        {
            MemberPageModuleListModel model = null;
            List<MemberPageModuleListModel> list = new List<MemberPageModuleListModel>();
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                var moduleList = moduleDAL.GetMemberPageModuleList(conn, pageCode);
                if (moduleList.Any())
                {
                    var contentList = contentDAL.GetMemberPageModuleContentList(conn, moduleList.First().MemberPageID);
                    foreach (var item in moduleList)
                    {
                        //添加模块数据
                        model = new MemberPageModuleListModel();
                        model = ObjectMapper.ConvertTo<MemberPageModuleModel, MemberPageModuleListModel>(item);
                        model.MemberPageModuleID = item.PKID;
                        model.ModuleDisplayIndex = item.DisplayIndex;
                        list.Add(model);
                        //添加模块内容数据
                        list.AddRange(ObjectMapper.ConvertTo<MemberPageModuleContentModel, MemberPageModuleListModel>(contentList.Where(t => t.MemberPageModuleID == item.PKID)));
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取模块详细信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public MemberPageModuleModel GetMemberPageModuleInfo(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return moduleDAL.GetMemberPageModuleInfo(conn, pkid);
            }
        }
        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMemberPageModule(MemberPageModuleModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return moduleDAL.AddMemberPageModule(conn,model);
            }
        }
        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateMemberPageModule( MemberPageModuleModel model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return moduleDAL.UpdateMemberPageModule(conn, model);
            }
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteMemberPageModule(long pkid)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return moduleDAL.DeleteMemberPageModule(conn,pkid);
            }
        }
        /// <summary>
        /// 刷新个人中心页面配置缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshMemberPageCache()
        {
            using (var client = new ConfigClient())
            {
                return client.RefreshMemberPageCache().Result;
            }
        }
    }
}

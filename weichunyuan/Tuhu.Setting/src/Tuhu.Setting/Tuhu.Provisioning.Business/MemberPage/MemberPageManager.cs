using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.MemberPage;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;

namespace Tuhu.Provisioning.Business.MemberPage
{
    public class MemberPageManager
    {
        private DALMemberPage memberPageDAL = null;
        public MemberPageManager()
        {
            memberPageDAL = new DALMemberPage();
        }
        /// <summary>
        /// 根据页面编码获取页面配置信息
        /// </summary>
        /// <param name="pageCode">页面编码，member=个人中心，more=更多</param>
        /// <returns></returns>
        public MemberPageModel GetMemberPageByPageCode(string pageCode)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return memberPageDAL.GetMemberPageByPageCode(conn, pageCode);
            }
        }
    }
}

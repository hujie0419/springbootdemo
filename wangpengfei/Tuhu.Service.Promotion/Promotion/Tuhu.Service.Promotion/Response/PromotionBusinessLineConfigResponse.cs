using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 优惠券业务线
    /// </summary>
    public class PromotionBusinessLineConfigResponse
    {
        /// <summary>
        /// 主键自增列
        /// </summary>		
        private int _pkid;
        public int PKID
        {
            get { return _pkid; }
            set { _pkid = value; }
        }
        /// <summary>
        /// 业务线的名称
        /// </summary>		
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }
        /// <summary>
        /// 业务线的创建者
        /// </summary>		
        private string _creater;
        public string Creater
        {
            get { return _creater; }
            set { _creater = value; }
        }
        /// <summary>
        /// 业务线最后修改者
        /// </summary>		
        private string _lastupdater;
        public string LastUpdater
        {
            get { return _lastupdater; }
            set { _lastupdater = value; }
        }
        /// <summary>
        /// 审核人账号或者邮箱
        /// </summary>		
        private string _auditor;
        public string Auditor
        {
            get { return _auditor; }
            set { _auditor = value; }
        }
    }
}

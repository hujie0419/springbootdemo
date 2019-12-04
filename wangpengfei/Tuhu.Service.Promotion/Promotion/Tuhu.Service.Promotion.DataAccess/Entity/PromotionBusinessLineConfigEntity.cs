using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 成本归属 - 业务线
    /// </summary>
    public class PromotionBusinessLineConfigEntity
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
        /// 是否已删除
        /// </summary>		
        private int _isdel;
        public int IsDel
        {
            get { return _isdel; }
            set { _isdel = value; }
        }
        /// <summary>
        /// 更新时间
        /// </summary>		
        private DateTime _updatetime;
        public DateTime UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>		
        private DateTime _createtime;
        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 部门用途
    /// </summary>
    public class CouponDepartmentUseSettingEntity
    {

        /// <summary>
        /// PKID
        /// </summary>		
        private int _pkid;
        public int PKID
        {
            get { return _pkid; }
            set { _pkid = value; }
        }
        /// <summary>
        /// SettingId
        /// </summary>		
        private int _settingid;
        public int SettingId
        {
            get { return _settingid; }
            set { _settingid = value; }
        }
        /// <summary>
        /// DisplayName
        /// </summary>		
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }
        /// <summary>
        /// ParentSettingId
        /// </summary>		
        private int _parentsettingid;
        public int ParentSettingId
        {
            get { return _parentsettingid; }
            set { _parentsettingid = value; }
        }
        /// <summary>
        /// Type
        /// </summary>		
        private int _type;
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// IsDel
        /// </summary>		
        private bool _isdel;
        public bool IsDel
        {
            get { return _isdel; }
            set { _isdel = value; }
        }
        /// <summary>
        /// CreateTime
        /// </summary>		
        private DateTime _createtime;
        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        /// <summary>
        /// UpdateTime
        /// </summary>		
        private DateTime _updatetime;
        public DateTime UpdateTime
        {
            get { return _updatetime; }
            set { _updatetime = value; }
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

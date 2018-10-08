using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class ActionPower
    {
        public int PKID { get; set; }

        public int ParentID { get; set; }

        public string LinkName { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Remark { get; set; }

        public string ParametersName { get; set; }

        public string ParametersValue { get; set; }

        public int? ORDERBY { get; set; }

        public string Type { get; set; }

        public bool IsActive { get; set; }

        public string UserNos { get; set; }

        public string BtnKey { get; set; }

        public byte BtnType { get; set; }

        public byte CgKey { get; set; }

        public string Module { get; set; }

        /// <summary>
        /// 字段控制类别
        /// </summary>
        public int? FcType { get; set; }

        #region 权限用属性
        /// <summary>
        /// 视图KEY
        /// </summary>
        public string ActionKey { get; set; }

        /// <summary>
        /// 是否有实体CSHTML
        /// </summary>
        public bool? IsViewAction { get; set; }
        #endregion

        #region 分类菜单
        public int? CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string CategoryIcon { get; set; }
        public int Sort { get; set; }
        #endregion
    }
}

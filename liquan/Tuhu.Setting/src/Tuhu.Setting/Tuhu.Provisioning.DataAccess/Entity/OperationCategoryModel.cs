using System;
using System.ComponentModel.DataAnnotations;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary> 
    /// tbl_OperationCategory:实体类 
    /// </summary>  
    [Serializable]
    public class OperationCategoryModel
    {
        #region tbl_OperationCategory

        public int Id { get; set; }

        public int ParentId { get; set; }

        public string CategoryCode { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// APP处理值 IOS
        /// </summary>
        public string AppKeyForIOS { get; set; }

        /// <summary>
        /// APP通讯值 IOS
        /// </summary>
        public string AppValueForIOS { get; set; }

        /// <summary>
        /// APP处理值 Android
        /// </summary>
        public string AppKeyForAndroid { get; set; }

        /// <summary>
        /// APP通讯值 Android
        /// </summary>
        public string AppValueForAndroid { get; set; }

        /// <summary>
        /// H5 URL
        /// </summary>
        public string H5Url { get; set; }

        public int Sort { get; set; }

        public int IsShow { get; set; }

        public int Type { get; set; }

        public DateTime CreateTime { get; set; }

        public string WebsiteImage { get; set; }

        /// <summary>
        /// 是否需要车型
        /// </summary>
        public int IsNeedVehicle { get; set; }

        #endregion
    }
}

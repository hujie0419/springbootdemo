using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public partial class SE_MDBeautyBrandConfigModel
    {
        public SE_MDBeautyBrandConfigModel()
        { }
        #region Model
        private int _id = 0;
        private int _parentid = 0;
        private string _categoryids = "";
        private string _brandname = "";
        private bool _isdisable = false;
        private DateTime? _createtime = DateTime.Now;
        private int _level = 0;

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int ParentId
        {
            set { _parentid = value; }
            get { return _parentid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CategoryIds
        {
            set { _categoryids = value; }
            get { return _categoryids; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BrandName
        {
            set { _brandname = value; }
            get { return _brandname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDisable
        {
            set { _isdisable = value; }
            get { return _isdisable; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Level
        {
            set { _level = value; }
            get { return _level; }
        }
        #endregion Model

    }
}

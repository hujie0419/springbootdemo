using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public partial class SE_MDBeautyCategoryConfigModel
    {
        public SE_MDBeautyCategoryConfigModel()
        { }
        #region Model
        private int _id = 0;
        private int _parentid = 0;
        private string _categoryname = "";
        private string _showname = "";
        private string _describe = "";
        private bool _isdisable = false;
        private DateTime? _createtime = DateTime.Now;
        private int _level = 0;

        public string Childs { get; set; }
        public string Parents { get; set; }

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
        public string CategoryName
        {
            set { _categoryname = value; }
            get { return _categoryname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ShowName
        {
            set { _showname = value; }
            get { return _showname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Describe
        {
            set { _describe = value; }
            get { return _describe; }
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

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        #endregion Model
    }

    [Serializable]
    public partial class SE_MDBeautyCategoryConfigForPartModel
    {
        public SE_MDBeautyCategoryConfigForPartModel()
        { }
        #region Model
        private string _id ="";
        private int _parentid = 0;
        private string _categoryname = "";
        private string _showname = "";
        private string _describe = "";
        private bool _isdisable = false;
        private DateTime? _createtime = DateTime.Now;
        private int _level = 0;

        public string Childs { get; set; }
        public string Parents { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Id
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
        public string CategoryName
        {
            set { _categoryname = value; }
            get { return _categoryname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ShowName
        {
            set { _showname = value; }
            get { return _showname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Describe
        {
            set { _describe = value; }
            get { return _describe; }
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

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        #endregion Model
    }
}

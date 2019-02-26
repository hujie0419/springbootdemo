using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class ProductLibraryConfigModel
    {
        #region Model
        private int _id;
        private int? _oid;
        private string _pid;
        private string _couponids;
        private int? _state = 0;
        private DateTime? _createtime = DateTime.Now;
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
        public int? Oid
        {
            set { _oid = value; }
            get { return _oid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PID
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CouponIds
        {
            set { _couponids = value; }
            get { return _couponids; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? State
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        #endregion Model
    }
}

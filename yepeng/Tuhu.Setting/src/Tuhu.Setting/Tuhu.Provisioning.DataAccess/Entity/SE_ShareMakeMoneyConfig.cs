using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 分享赚钱配置
    /// </summary>
    public class SE_ShareMakeMoneyConfig
    {

        private int _id;
        private string _name;
        private string _firstsharenumber;
        private int? _rewarddatetime;
        private DateTime? _createdate;
        private DateTime? _updatedate;
        private string _createusername;
        private string _updateusername;
        /// <summary>
        /// 主键
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 分享赚钱名称
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 每天首次分享奖励额度
        /// </summary>
        public string FirstShareNumber
        {
            set { _firstsharenumber = value; }
            get { return _firstsharenumber; }
        }
        /// <summary>
        /// 分享后购买赢得奖励时间 24  48  72  96  120  144 168
        /// </summary>
        public int? RewardDateTime
        {
            set { _rewarddatetime = value; }
            get { return _rewarddatetime; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate
        {
            set { _createdate = value; }
            get { return _createdate; }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDate
        {
            set { _updatedate = value; }
            get { return _updatedate; }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName
        {
            set { _createusername = value; }
            get { return _createusername; }
        }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUserName
        {
            set { _updateusername = value; }
            get { return _updateusername; }
        }

        /// <summary>
        /// 分享赚钱活动规则
        /// </summary>
        public string RuleInfo { get; set; }




    }


    public class SE_ShareMakeImportProducts
    {
        private int _id;
        private int? _fkid;
        private string _batchguid;
        private DateTime? _createdate;
        private string _pid;
        private string _displayname;
        private string _times;
        private bool _ismakemoney;
        private bool _isshare = false;
        private int? _orderby;
        /// <summary>
        /// 主键
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 外键
        /// </summary>
        public int? FKID
        {
            set { _fkid = value; }
            get { return _fkid; }
        }
        /// <summary>
        /// 活动规则批次商品的GUID
        /// </summary>
        public string BatchGuid
        {
            set { _batchguid = value; }
            get { return _batchguid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDate
        {
            set { _createdate = value; }
            get { return _createdate; }
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
        /// 产品名称
        /// </summary>
        public string DisplayName
        {
            set { _displayname = value; }
            get { return _displayname; }
        }
        /// <summary>
        /// 倍数
        /// </summary>
        public string Times
        {
            set { _times = value; }
            get { return _times; }
        }
        /// <summary>
        /// 状态 1 启用 0禁用 启用时分享赚钱，禁用时分享不赚钱
        /// </summary>
        public bool IsMakeMoney
        {
            set { _ismakemoney = value; }
            get { return _ismakemoney; }
        }
        /// <summary>
        /// 1 显示在活动页 0不显示 默认不显示
        /// </summary>
        public bool IsShare
        {
            set { _isshare = value; }
            get { return _isshare; }
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Orderby
        {
            set { _orderby = value; }
            get { return _orderby; }
        }
    }

}

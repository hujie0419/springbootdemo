using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public partial class SE_MDBeautyCategoryProductConfigModel
    {
        public SE_MDBeautyCategoryProductConfigModel()
        { }
        #region Model
        private int _pid;
        private string _prodcutname;
        private string _categoryids;
        private string _describe;
        private decimal? _beginprice = 0M;
        private decimal? _endprice = 0M;
        private decimal? _beginpromotionprice = 0M;
        private decimal? _endpromotionprice = 0M;
        private decimal? _commission = 0M;
        private int? _everydaynum = 0;
        private string _brands;
        private int? _recommendcar = 0;
        private int? _adaptivecar = 0;
        private bool _isdisable = false;
        private bool _isnotshow = false;
        private DateTime? _createtime = DateTime.Now;

        public int RowNumber { get; set; }

        public int TotalCount { get; set; }

        public string ParentsName { get; set; }

        public string TreeItems { get; set; }

        /// <summary>
        /// 产品库父产品ID
        /// </summary>
        public string ProdcutId { get; set; }

        /// <summary>
        /// 产品库类目ID
        /// </summary>
        public string PrimaryParentCategory { get; set; }

        /// <summary>
        /// 产品库产品图片
        /// </summary>
        public string Image_filename { get; set; }

        /// <summary>
        /// 产品库顶级类目ID
        /// </summary>
        public string DefinitionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PId
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ProdcutName
        {
            set { _prodcutname = value; }
            get { return _prodcutname; }
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
        public string Describe
        {
            set { _describe = value; }
            get { return _describe; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? BeginPrice
        {
            set { _beginprice = value; }
            get { return _beginprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? EndPrice
        {
            set { _endprice = value; }
            get { return _endprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? BeginPromotionPrice
        {
            set { _beginpromotionprice = value; }
            get { return _beginpromotionprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? EndPromotionPrice
        {
            set { _endpromotionprice = value; }
            get { return _endpromotionprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Commission
        {
            set { _commission = value; }
            get { return _commission; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? EveryDayNum
        {
            set { _everydaynum = value; }
            get { return _everydaynum; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Brands
        {
            set { _brands = value; }
            get { return _brands; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? RecommendCar
        {
            set { _recommendcar = value; }
            get { return _recommendcar; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? AdaptiveCar
        {
            set { _adaptivecar = value; }
            get { return _adaptivecar; }
        }

        public string AdaptiveCarCheckBox { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisable
        {
            set { _isdisable = value; }
            get { return _isdisable; }
        }
        public bool IsNotShow
        {
            set { _isnotshow = value; }
            get { return _isnotshow; }
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

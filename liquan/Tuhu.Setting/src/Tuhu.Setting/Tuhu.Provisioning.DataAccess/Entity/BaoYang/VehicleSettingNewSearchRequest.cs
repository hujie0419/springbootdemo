using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYang
{
    public class VehicleSettingNewSearchRequest
    {

        private int _pageIndex;
        private int _pageSize;
        private bool _isConfig;
        private bool _isEnabled;

        public string PartName { get; set; }

        public string Brand { get; set; }

        public string VehicleId { get; set; }

        public string Viscosity { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public bool IsConfig
        {
            get
            {
                return _isEnabled || _isConfig;
            }
            set
            {
                _isConfig = value;
            }
        }

        public int PageIndex
        {
            get { return this._pageIndex > 0 ? this._pageIndex : 1; }
            set
            {
                this._pageIndex = value > 0 ? value : 1;
            }
        }

        public int PageSize
        {
            get { return this._pageSize > 0 ? this._pageSize : 10; }
            set
            {
                this._pageSize = value > 0 ? value : 10;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public string PriorityBrand { get; set; }

        public string PrioritySeries { get; set; }

        public string PriorityType { get; set; }

        public int Priority { get; set; }

        public string Grade { get; set; }
    }
}

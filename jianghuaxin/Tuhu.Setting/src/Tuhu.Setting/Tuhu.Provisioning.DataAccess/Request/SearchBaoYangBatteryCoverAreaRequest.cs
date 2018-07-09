using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Request
{
    public class SearchBaoYangBatteryCoverAreaRequest
    {
        private int _index;

        private int _size;

        public string Brand { get; set; }

        public int Province { get; set; }

        public int City { get; set; }

        public int PageIndex { get { return _index; } set { _index = value > 0 ? value : 1; } }

        public int PageSize { get { return _size; } set { _size = value > 0 ? value : 20; } }
    }
}

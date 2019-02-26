using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class SexAnnualVoteQueryRequest
    {
        public int ProvinceId{ get; set; }
        public int CityId { get; set; }
        public long ShopId { get; set; }
        public long EmployeeId { get; set; }
        public string Keywords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}

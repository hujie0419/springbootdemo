using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Models
{
    public class MatchGiftsResponse
    {
        public string Pid { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }
        /// <summary>是否必须</summary>
        public bool Require { get; set; }

        /// <summary>赠品类型（0：促销；1：赠品）</summary>
        public int GiftsType { get; set; }

        public string GiftDescription { get; set; }
    }
}

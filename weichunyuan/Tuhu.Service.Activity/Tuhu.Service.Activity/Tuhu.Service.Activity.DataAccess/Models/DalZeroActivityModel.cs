using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.DataAccess.Models
{
   public  class DalZeroActivityModel
    {        
        /// <summary>商品PID</summary>
        public string Pid { get; set; }

        /// <summary>期数</summary>
        public int Period { get; set; }

        /// <summary>获奖人数</summary>
        [Column("SucceedQuota")]
        public int NumOfWinners { get; set; }

        /// <summary>奖品总量</summary>
        public int Quantity { get; set; }
        /// <summary>活动开始日期</summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>活动结束日期</summary>
        public DateTime EndDateTime { get; set; }
    }
}

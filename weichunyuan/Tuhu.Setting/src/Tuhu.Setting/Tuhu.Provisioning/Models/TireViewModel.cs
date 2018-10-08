using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class TireViewModel
    {
        //车系(德系,日系)
        public IEnumerable<string> VehicleDepartment { get; set; }
        //品牌车系(车型一二级)
        public Dictionary<string,Dictionary<string,List<VehicleModel>>> VehicleOneTwoLevel { get; set; }
        //轮胎品牌
        public IEnumerable<string> Brands { get; set; }

        /// <summary>
        /// 车型类别(SUV,轿车)
        /// </summary>
        public IEnumerable<string> VehicleBodyTypes { get; set; }

        /// <summary>
        /// 所有轮胎规格信息
        /// </summary>
        public IEnumerable<TireSizeModel> TireSize { get; set; }
    }
}
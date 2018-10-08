using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class AntifreezeSettingModel : BaseModel
    {
        public int PKID { get; set; }
      
        public string FreezingPoint { get; set; }
        public string Brand { get; set; }

        public string ProvinceIds { get; set; }
        public string ProvinceNames { get; set; }
        //状态(0:启用，1：停止)
        public byte Status { get; set; }
         public AntifreezeSettingModel() : base() { }
         public AntifreezeSettingModel(DataRow row) : base(row) { }
     
    }
    public class ProvinceItem :BaseModel
    {
        public int PKID { get; set; }
        public string RegionName { get; set; }
        public ProvinceItem() : base() { }
        public ProvinceItem(DataRow row) : base(row) { }
    }
}

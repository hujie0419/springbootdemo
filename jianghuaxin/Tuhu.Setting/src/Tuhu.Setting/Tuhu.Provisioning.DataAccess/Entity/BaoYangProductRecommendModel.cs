using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangProductRecommendModel : BaseModel
    {
        public BaoYangProductRecommendModel()
        { }
        public BaoYangProductRecommendModel(DataRow row) : base(row)
        {
        }
        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }
        //PKID
        public int PKID { get; set; }
        //项目名称（机油）目前只有机油
        public string PartName { get; set; }
        //属性，（HX3,HX5，HX6,HX7,HX8,ULTR）这些都是属于机油的
        public string Property { get; set; }
        //优先级
        public string Type { get; set; }
        //状态(0:启用，1：停止)
        public int Enabled { get; set; }
        public string Priority { get; set; }
    }
}

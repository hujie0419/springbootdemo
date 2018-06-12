using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PrioritySettingModel : BaseModel
    {
        public PrioritySettingModel()
        { }
        public PrioritySettingModel(DataRow row): base(row)
        {
        }
        protected override void Parse(DataRow row)
        {
            base.Parse(row);
        }
        //ID
        public int ID { get; set; }
        //项目名称（机油，机油滤清器，空气滤清器）
        public string PartName { get; set; }
        //属性，（HX3,HX5，HX6,HX7,HX8,ULTR）这些都是属于机油的
        public string  PropertyType { get; set; }
        //(第一，第二，第三)优先级，是按什么来分的，Brand：1-3优先级是按品牌来分的
        public string PriorityField { get; set; }
        //状态(0:启用，1：停止)
        public int Enabled { get; set; }
        //第一优先级
        public string FirstPriority { get; set; }
        //第二优先级
        public string SecondPriority { get; set; }
        //第三优先级
        public string ThirdPriority { get; set; }
    }
}

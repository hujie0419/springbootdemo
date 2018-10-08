using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.SalePromotionActivity
{

    /// <summary>
    /// 渠道列表模型
    /// </summary>
    public class SelectChannelListModel
    {
        public List<ChannelVModel> OwnChannel { get; set; }

        public List<ChannelVModel> ThirdChannel { get; set; }
        public List<ChannelVModel> CooperationChannel { get; set; }
    }

    public class ChannelVModel 
    {
        public string ChannelValue { get; set; }

        public string ChannelKey { get; set; }

        public string ChannelType { get; set; }

    }

    /// <summary>
    /// 级联选择 递归数据
    /// </summary>
    public class RescueSelectModel
    {
        //public int Id { get; set; }
        //public int ParentId { get; set; }
        public string value { get; set; }
        public string label { get; set; }

        public List<RescueSelectModel> children { get; set; } 
        public RescueSelectModel()
        {
            children = new List<RescueSelectModel>();
        }
    }

    /// <summary>
    /// 前台结果数据封装
    /// </summary>
    public class VResultMode
    {
        public bool Status { get; set; }

        public string Msg { get; set; }

        public object Data { get; set; }

        public int Count { get; set; } 

    }

    public class ProductCostAndStock
    {
        
        public string Pid { get; set; } 
        public int TotalStock { get; set; }
        public decimal CostPrice { get; set; }
    }
}

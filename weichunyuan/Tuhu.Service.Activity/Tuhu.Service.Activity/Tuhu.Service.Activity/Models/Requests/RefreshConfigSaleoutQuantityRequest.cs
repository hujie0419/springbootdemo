using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class UpdateConfigSaleoutQuantityRequest
    {
        public RefreshType RefreshType { get; set; }
        public List<UpdateQuantityProductModel> ProductModels { get; set; }
    }

    public class UpdateQuantityProductModel
    {
        public Guid ActivityId { get; set; }

        public string Pid { get; set; }
    }
    public enum RefreshType
    {
        RefreshByPid = 1,

        RefreshByActivityId = 2,

        RefreshAll
    }
}

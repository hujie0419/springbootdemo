
using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class DiscountActivtyProductRequest
    {
        public string Pid { get; set; }
        public string ActivityId { get; set; }
        public Guid UserId { get; set; }
    }
}

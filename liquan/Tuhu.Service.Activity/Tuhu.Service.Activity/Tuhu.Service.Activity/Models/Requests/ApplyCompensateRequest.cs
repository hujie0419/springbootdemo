using System;
namespace Tuhu.Service.Activity.Models.Requests
{
  public   class ApplyCompensateRequest
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string OrderId { get; set; }

        public string ProductName { get; set; }

        public string Link { get; set; }

        public decimal DifferencePrice { get; set; }

        public string Images { get; set; }

        public string OrderChannel { get; set; }
    }
}

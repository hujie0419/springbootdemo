using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class ActivityTypeRequest:ActivityTypeModel
    {
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public int Status { get; set; }

    }
}

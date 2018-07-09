using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class UserRewardApplicationRequest
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplicationName { get; set; }

        public string Phone { get; set; }

        public string ImageUrl1 { get; set; }

        public string ImageUrl2 { get; set; }

        public string ImageUrl3 { get; set; }
    }
}
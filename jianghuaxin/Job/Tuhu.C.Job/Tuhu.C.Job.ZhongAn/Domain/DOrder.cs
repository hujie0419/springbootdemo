using System;

namespace K.Domain
{
    public class DOrder
    {
        public int PKID { get; set; }

        public DateTime OrderDate { get; set; }

        public string UserName { get; set; }

        public string UserTel { get; set; }

        public string CarPlate { get; set; }

        public string StoreAddress { get; set; }

        public string StoreName { get; set; }

        public string OrderNo { get; set; }
    }
}

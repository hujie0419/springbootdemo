using System;
using Tuhu.Models;

namespace Tuhu.C.Job.Models
{
    public  class OrderGroupBuyModel: BaseModel
    {
        /*
        [ID] [int] NOT NULL IDENTITY(1, 1),
[OrderID] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[PID] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[UserId] [uniqueidentifier] NULL,
[OrderStatus] [int] NULL,
[OrderType] [int] NULL,
[ContactOrderID] [varchar] (1000) COLLATE Chinese_PRC_CI_AS NULL,
[CreateDate] [datetime] NULL CONSTRAINT [DF__tbl_Order__Creat__4AADF94F] DEFAULT (getdate()),
[FalshSaleGuid] [uniqueidentifier] NULL,
[IsPush] [int] NULL
         */
         public int ID { get; set; }

        public string OrderID { get; set; }

        public string PID { get; set; }

        public Guid UserId { get; set; }

        public int OrderStatus { get; set; }

        public int OrderType { get; set; }

        public string ContactOrderID { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid FalshSaleGuid { get; set; }

        public int? IsPush { get; set; }


    }
}

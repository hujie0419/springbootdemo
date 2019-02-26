using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class BigBrandRealLogModel
    {
        /*
        PKID INT IDENTITY(1,1) PRIMARY KEY,
[CreateDateTime] [datetime] NOT NULL,
[LastUpdateDateTime] [datetime] NOT NULL,
UserId UNIQUEIDENTIFIER NOT NULL,
Prize VARCHAR(300) NOT NULL,--奖品
[Address] VARCHAR(500),
FKBigBrandPoolID INT NOT NULL,
Tip UNIQUEIDENTIFIER NOT NULL --标记后期更新
         */
        public int PKID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public Guid UserId { get; set; }

        public string Prize { get; set; }

        public string Address { get; set; }

        public int FKBigBrandPoolID { get; set; }

        public int FKBigBrandID { get; set; }

        /// <summary>
        /// 标记
        /// </summary>
        public Guid Tip { get; set; }

        public string UserName { get; set; }
    }


    public class BigBrandRealResponse
    {
        public int Code { get; set; }

        public string Msg { get; set; }
    }

}

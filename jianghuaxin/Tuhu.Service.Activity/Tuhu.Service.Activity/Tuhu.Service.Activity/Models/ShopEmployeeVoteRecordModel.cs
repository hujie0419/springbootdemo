using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Service.Activity.Models
{
    public class ShopEmployeeVoteRecordModel:ShopVoteRecordModel
    {
        public long EmployeeId { get; set; }
    }
}
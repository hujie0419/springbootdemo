using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BlackListConfigModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int State { get; set; }

        public DateTime CreateTime { get; set; }
    }
}

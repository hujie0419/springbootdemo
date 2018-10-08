using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class U_ChannelPayModel
    {
        public int Id { get; set; }

        public string Channel { get; set; }

        public string PayMethod { get; set; }

        public int Type { get; set; }

        public bool IsChecked { get; set; }
    }
}

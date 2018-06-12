using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ChannelDictionariesModel
    {

        public string ChannelKey { get; set; }

        public string ChannelValue { get; set; }

        public string ChannelType { get; set; }
        public bool IsChecked { get; set; }
    }
}

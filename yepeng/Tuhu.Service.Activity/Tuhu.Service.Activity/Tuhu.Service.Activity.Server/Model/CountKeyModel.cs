using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Server.Model
{
    public class CountKeyModel
    {
        public string Key { get; set; }
        public int Sort { get; set; }
        public KeyType Type { get; set; }
    }

    public enum KeyType
    {
        PlaceUserIdKey=1,
        PlaceDeviceIdKey=2,
        PlaceUserTelKey=3,
        PersonUserIdKey=4,
        PersonDeviceIdKey = 5,
        PersonUserTelKey = 6
    }
}

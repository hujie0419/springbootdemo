using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data;

namespace Tuhu.C.Job.UserAccountCombine.Model
{
    public class UserDailyInc
    {
        public int CountNum { get; set; }

        public string CreateTime { get; set; }

        public string Channel { get; set; }
    }

    public class UserGrowthSchedule
    {
        public DateTime DayString { get; set; }
        public string Group { get; set; }
        public string Channel { get; set; }
        public int IncNum { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }

    public class UserChannelGroupV1
    {
        public string Group { get; set; }
        public string Channel { get; set; }
    }
}

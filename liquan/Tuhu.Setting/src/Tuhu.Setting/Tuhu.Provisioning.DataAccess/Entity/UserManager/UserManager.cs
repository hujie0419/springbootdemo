using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.UserManager
{
    public class UserGrowthSchedule
    {
        public DateTime DayString { get; set; }
        public string Group { get; set; }
        public string Channel { get; set; }
        public int IncNum { get; set; }
    }

    public class DailyIncNum
    {
        public string Date { get; set; }
        public int Num { get; set; }
    }

    public class UserGrowthItem
    {
        public string Group { get; set; }
        public string Channel { get; set; }
        public List<DailyIncNum> IncNumList { get; set; }
    }

    public class PostUserDailyInc
    {
        public List<string> Groups { get; set; }
        public List<string> Columns { get; set; }
        public List<UserGrowthItem> Items { get; set; }
    }
}

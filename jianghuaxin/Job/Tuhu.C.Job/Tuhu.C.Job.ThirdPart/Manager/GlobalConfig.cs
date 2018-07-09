using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Manager
{
    public class GlobalConfig
    {
        public static readonly string Environment = ConfigurationManager.AppSettings["Env"];
    }
}

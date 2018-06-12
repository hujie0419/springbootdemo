using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Job;

namespace Tuhu.C.Job.ThirdPart.Manager
{
    public class JobLogger
    {
        /// <summary>
        /// 广发联名卡日志
        /// </summary>
        public static readonly ILog GFLogger = LogManager.GetLogger(typeof(GFBankCardJob));
    }
}

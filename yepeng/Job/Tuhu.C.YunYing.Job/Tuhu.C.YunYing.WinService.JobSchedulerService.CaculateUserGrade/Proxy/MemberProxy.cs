using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using System.IO;
using System.Threading;
using Common.Logging;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Proxy
{
    public class MemberProxy
    {
        public static bool CaculateUserGrade(Guid userId, ILog log)
        {
            using (var client = new UserIntegralClient())
            {
                bool result = false;
                try
                {
                    var temp = client.CaculateUserGrade(userId);
                    if (temp != null)
                    {
                        if (temp.Exception != null)
                        {
                            log.Info(temp.Exception);
                        }
                        else
                        {
                            result = temp.Result;
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }

        }

    }
}

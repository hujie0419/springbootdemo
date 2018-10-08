using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Utils
{
    public class EmailHelper
    {
        private static string _gFBankCardJobUsers = ConfigurationManager.AppSettings["GFBankCardJob:To"];
        public static void SendEmail(string subject, string msg)
        {
            TuhuMessage.SendEmail(subject, _gFBankCardJobUsers, msg);
        }
    }
}

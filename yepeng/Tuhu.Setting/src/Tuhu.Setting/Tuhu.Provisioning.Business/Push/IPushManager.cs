using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Push
{
    public interface IPushManager
    {
        IList<string> GetPushMsgPersonConfig();
    }
}

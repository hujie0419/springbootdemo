using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.Enum
{
    public class GFModelBll
    {
        public static string ConvertToTuhuBusinessType(string businessType)
        {
            return string.Equals("激活", businessType.Trim()) ? nameof(GFBusinessType.Activate)
                : string.Equals("注销", businessType.Trim()) ? nameof(GFBusinessType.Cancel) : businessType;
        }
    }
    public enum GFBusinessType
    {
        /// <summary>
        /// 激活
        /// </summary>
        Activate = 0,
        /// <summary>
        /// 注销
        /// </summary>
        Cancel = 1
    }
}

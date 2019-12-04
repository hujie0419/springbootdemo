using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 基础验证请求属性
    /// </summary>
    public abstract class BaseValidRequest
    {
        public bool IsPassed
        {
            get
            {
                string msg;
                bool result = IsValid(out msg);
                if (!result)
                    ErrorMsg = msg;
                return result;
            }
        }
        private string _ErrorMsg;
        public string ErrorMsg
        {
            get
            {
                return _ErrorMsg;
            }
            private set
            {
                _ErrorMsg = value;
            }
        }
        protected abstract bool IsValid(out string errorMsg);
    }
}

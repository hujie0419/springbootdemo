using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Models
{
    public class ResultModel<T>
    {
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public string Code { get; set; }

        public T Data { get; set; }
    }
}

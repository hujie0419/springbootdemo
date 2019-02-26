using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class ResultModel<T>
    {
        public bool IsSuccess { get; set; }

        public int Code { get; set; }

        public string Msg { get; set; }

        public T Result { get; set; }

    }
}

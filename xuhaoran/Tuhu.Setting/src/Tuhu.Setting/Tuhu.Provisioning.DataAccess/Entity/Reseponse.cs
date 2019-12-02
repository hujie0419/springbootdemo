using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class Reseponse<T>
    {
        /// <summary>状态</summary>
        public int status { get; set; }

        /// <summary>信息</summary>
        public string Message { get; set; }

        /// <summary>数据</summary>
        public T data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class PagedDataModel<T>
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalSize { get; set; }

        /// <summary>
        /// PageIndex
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public ICollection<T> Data { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }
    }

}
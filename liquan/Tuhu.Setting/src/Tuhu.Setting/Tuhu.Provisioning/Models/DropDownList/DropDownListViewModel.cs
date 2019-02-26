using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class DropDownListViewModel
    {
        /// <summary>
        /// 下拉类别值
        /// </summary>
        public string  Id { get; set; }

        /// <summary>
        /// 下拉列表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelect { get; set; }

    }
}
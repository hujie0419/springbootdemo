using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class tbl_UserTask:BaseModel
    {

        public tbl_UserTask() : base() { }
        public tbl_UserTask(DataRow row) : base(row) { }

        public int ID { get; set; }

        /// <summary>
        /// 升级任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 1 ：android 0 ：IOS
        /// </summary>
        public int APPType { get; set; }

        /// <summary>
        /// APP处理值
        /// </summary>
        public string APPHandler { get; set; }



        /// <summary>
        /// APP通信值
        /// </summary>
        public string APPConnect { get; set; }

    }
}

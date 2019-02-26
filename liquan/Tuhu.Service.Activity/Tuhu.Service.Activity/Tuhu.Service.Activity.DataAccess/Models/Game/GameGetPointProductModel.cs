using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{

    /// <summary>
    /// [Activity].[dbo].tbl_GameGetPointProduct 购买可获得积分的pidc  表实体
    /// </summary>
    public class GameGetPointProductModel
    {
        public int PKID { get; set; }

        /// <summary>
        /// 购买可获得积分的pid
        /// </summary>
        public string Pid { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
   public class GameRankModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 排名数
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 积分数
        /// </summary>
        public int Point { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 天天秒杀配置
    /// </summary>
    public class SE_EveryDaySeckill
    {

        public int? ID { get; set; }

        public Guid? ActivityGuid { get; set; }

        public string ActivityName { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public SE_EveryDaySeckillInfo EveryDaySeckillInfo { get; set; }

    }



    public class SE_EveryDaySeckillInfo
    {
        public int ID { get; set; }

        public Guid? FK_EveryDaySeckill { get; set; }


        public string SpecialUrl { get; set; }


        public string SpecialPicture { get; set; }

        public string SpecialStyle { get; set; }

        /// <summary>
        /// { Url BgUrl OrderBy Style 0:通知栏;1:三列 }
        /// </summary>
        public string ActivityContent { get; set; }


        public Guid FlashActivityGuid { get; set; }

        public DateTime CreateDate { get; set; }


        public DateTime UpdateDate { get; set; } 


    }



}

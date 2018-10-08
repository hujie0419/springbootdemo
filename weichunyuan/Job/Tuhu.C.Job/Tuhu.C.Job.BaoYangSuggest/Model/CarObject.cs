using System;

namespace Tuhu.C.Job.BaoYangSuggest.Model
{
    public class CarObject
    {
        public Guid UserId { get; set; }
        public string VehicleId { get; set; }
        public Guid CarId { get; set; }
        public int BuyYear { get; set; }//上路年
        public int BuyMonth { get; set; }//上路月
        public int Nian { get; set; }//车年份

        public int UsedMonth {
            get
            {
                DateTime now = DateTime.Now;
                var onRoadYear = this.BuyYear <= 0||this.BuyYear>2200 ? Nian : this.BuyYear;
                int year = now.Year - onRoadYear;
                var onRoadMonth = this.BuyMonth <= 0 || this.BuyMonth >12 ? 6 : this.BuyMonth;
                int month = now.Month - onRoadMonth;
                return year*12 + month;
            }
        }
        public DateTime OdometerTime { get; set; }//里程更新时间
        public int I_km_total { get; set; }//至历程更新时间的行驶里程

        public int BaoYangDistance { get; set; }

        public DateTime BaoyangDateTime { get; set; }
        public string BaoYangType { get; set; }
        public bool IsTuhuRecord { get; set; }

    }
}

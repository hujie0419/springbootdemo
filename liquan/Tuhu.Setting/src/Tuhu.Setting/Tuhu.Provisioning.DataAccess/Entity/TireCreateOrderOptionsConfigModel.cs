
namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class TireCreateOrderOptionsConfigModel
    {
        public int Id { get; set; }
        public int Type { get; set; }

        public int Status { get; set; }

        public string Pid { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public int IsAuto { get; set; }

        public int HasFreight { get; set; }

        public string ServicePid { get; set; }

        public int? ServicePrice { get; set; }

    }
    public class OrderOptionReferProductModel
    {
        public int OrderOptionId { get; set; }

        public string Pid { get; set; }

        public int Num { get; set; }

        public decimal Price { get; set; }

    }
}

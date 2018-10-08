
namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class LimitAreaSaleModel: SimpleLimitAreaSaleModel
    {


        public string ProductName { get; set; }



        public int TotalCount { get; set; }
    }
    public class SimpleLimitAreaSaleModel
    {
        public string Pid { get; set; }



        public int IsLimit { get; set; }


    }
}

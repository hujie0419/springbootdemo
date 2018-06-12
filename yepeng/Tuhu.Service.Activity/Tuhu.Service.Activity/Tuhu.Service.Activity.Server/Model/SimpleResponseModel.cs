
namespace Tuhu.Service.Activity.Server.Model
{
    public class SimpleResponseModel
    {
        public CheckFlashSaleStatus Code { get; set; }

        public string Mesage { get; set; }

        public int Record { get; set; }

        //public int DbQuantity { get; set; }

        public int DeviceCount { get; set; }

        public int TelCount { get; set; }
    }
}

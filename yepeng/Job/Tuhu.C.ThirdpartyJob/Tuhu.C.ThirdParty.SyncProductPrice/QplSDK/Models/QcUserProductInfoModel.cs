using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    public class QcUserProductInfoModel
    {
        public String UID { get; set; }
        public String ProductId { get; set; }
        public String UserId { get; set; }
        public String CurrentPrice { get; set; }
        public String LastPrice { get; set; }
        public String ProductNo { get; set; }
        public String ProductName { get; set; }
        public int Reserved { get; set; }
        public String Contact { get; set; }
        public double MarketPrice { get; set; }
        public double ListPrice { get; set; }
        public int Inventory { get; set; }

        private string userSKU;

        public string UserSKU { get { return userSKU; }
            set {
                userSKU = value;
                var topSplit = value.Split(new char[] { '^' });
                PID = topSplit[0];
                var splitAgain = topSplit[1].Split(new char[] { '.' });
                WarehouseID = int.Parse(splitAgain[0]);
                WarehouseName = splitAgain[1];
                BatchID = splitAgain[2];
                WeekYear = splitAgain[3];
            } }

        public string PID { get; private set; }

        public int? WarehouseID { get; private set; }

        public string WarehouseName { get; private set; }

        public string BatchID { get; private set; }

        public string WeekYear { get; private set; }

        /// <summary>
        /// 用户条码
        /// </summary>
        public String UserBarcode { get; set; }
        public String Gift { get; set; }
        public String Barcode { get; set; }
        public String Extension { get; set; }
        /// <summary>
        /// 0：上架；1：下架；2：禁售
        /// </summary>
        public int Availability { get; set; }
        public int IsRemove { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public String Createduser { get; set; }
        public String Updateduser { get; set; }
        public PriceInventoryModel priceInventory { get; set; }

        public ProductInfoModel ProductInfo { get; set; }
    }
}

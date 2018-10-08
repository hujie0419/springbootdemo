using System.Data;

namespace Tuhu.Provisioning.Business.Purchase
{
    public interface IPurchaseManager
    {
        void OrderDeQueue();

        void PurchaseVerify(string xmlStr, int invoiceId);

        DataTable GetOrderInvoiceByVendorId(int vendorId);
    }
}

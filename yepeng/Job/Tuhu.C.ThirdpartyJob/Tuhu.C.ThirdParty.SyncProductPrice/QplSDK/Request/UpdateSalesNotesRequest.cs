using System.Net.Http;
using System.Web;
using Qpl.Api.Response;

namespace Qpl.Api.Request
{
    public class UpdateSalesNotesRequest : IQplRequest<UpdateSalesNotesResponse>
    {
        public string OrderID { get; set; }
        public string SalesNotes { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

        #region IQplRequest<UpdateSalesNotesResponse> 成员

        public string ApiName { get { return "Qpl.CustomerOrder.UpdateSalesNotesById"; } }

        public HttpMethod Method { get { return HttpMethod.Put; } }

        public string Uri { get { return "CustomerOrder/UpdateSalesNotesById/" + HttpUtility.UrlEncode(OrderID); } }

        public object GetParam()
        {
            return SalesNotes;
        }

        public void Validate() { }

        #endregion
    }
}

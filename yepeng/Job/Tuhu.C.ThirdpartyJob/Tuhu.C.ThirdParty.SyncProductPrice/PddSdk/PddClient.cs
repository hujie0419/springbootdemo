using System;
using System.Threading.Tasks;
using PddSdk.Response;
using PddSdk.Request;
using Common.Logging;
using System.Net.Http;
using System.Net;

namespace PddSdk
{
    public class PddClient : IPddClient
    {
        private static readonly ILog _logger;
        static PddClient()
        {
            _logger = LogManager.GetLogger<PddClient>();
        }

        private readonly HttpClient httpClient;
        public PddClient(string baseAddress)
        {
            httpClient = new HttpClient {BaseAddress = new Uri(baseAddress)};
        }

        public async Task<T> ExecuteAsync<T>(IPddRequest<T> request) where T : PddResponse
        {
            Task<HttpResponseMessage> tResponse = null;
            if (request.Method == HttpMethod.Post)
                tResponse = httpClient.PostAsJsonAsync(request.Uri, request.GetParam());
            else if (request.Method == HttpMethod.Get)
                tResponse = httpClient.GetAsync(request.Uri + request.GetParam());

            if (tResponse == null)
                return null;

            var response = Activator.CreateInstance<T>();

            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await tResponse;

                if (httpResponse.IsSuccessStatusCode || httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                    response.Body = await httpResponse.Content.ReadAsStringAsync();
                else
                    httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                httpResponse?.Dispose();
            }

            return response;
        }
    }
}

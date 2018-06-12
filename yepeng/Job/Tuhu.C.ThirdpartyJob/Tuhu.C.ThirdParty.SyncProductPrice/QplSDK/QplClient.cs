using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Qpl.Api.Request;
using Qpl.Api.Response;
using Tuhu;

namespace Qpl.Api
{
    public class QplClient : IQplClient
    {
        private static readonly ILog _logger;

        static QplClient()
        {
            _logger = LogManager.GetLogger<QplClient>();
        }

        private readonly HttpClient httpClient;
        private readonly SymmetricAlgorithm algorithm;
        public QplClient(string baseAddress, string key, string iv)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);

            algorithm = SecurityHelper.GetSymmetricAlgorithm(SymmetricAlgorithmType.TripleDes, key, iv);
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;
        }

        #region IQplClient 成员

        public async Task<T> ExecuteAsync<T>(IQplRequest<T> request) where T : QplResponse
        {
            Task<HttpResponseMessage> tResponse = null;
            if (request.Method == HttpMethod.Post)
                tResponse = httpClient.PostAsJsonAsync(request.Uri, request.EncryptParam ? algorithm.Encrypt(JsonConvert.SerializeObject(request.GetParam())) : request.GetParam());
            else if (request.Method == HttpMethod.Put)
                tResponse = httpClient.PutAsJsonAsync(request.Uri, request.EncryptParam ? algorithm.Encrypt(JsonConvert.SerializeObject(request.GetParam())) : request.GetParam());
            else if (request.Method == HttpMethod.Get)
                tResponse = httpClient.GetAsync(request.Uri + request.GetParam());

            if (tResponse == null)
                return null;

            var response = Activator.CreateInstance<T>();

            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await tResponse;

                if (httpResponse.IsSuccessStatusCode)
                    response.Body = algorithm.Decrypt(await httpResponse.Content.ReadAsAsync<string>());
                else if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
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
                if (httpResponse != null)
                    httpResponse.Dispose();
            }

            return response;
        }
        #endregion
    }
}

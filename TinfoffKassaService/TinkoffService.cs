using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace TinfoffKassaService
{
    public class TinkoffService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public TinkoffService(IHttpContextAccessor httpContext, IHttpClientFactory httpClientFactory)
        {
            _httpContext = httpContext;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> SendInit(PaymentVm payment)
        {
            try
            {
                var dict = new Dictionary<string, object>()
                {
                    { "TerminalKey" , "findgiufig" },
                    {"OrderId" , payment.OrderId},
                    {"Amount" , payment.Amount},
                    {"SuccessURL" , $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}/OnSuccess?{payment.OrderId}"},
                };
                dict.Add("Token", GetToken(dict));
                using HttpClient client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using StringContent jsonContent = new(JsonSerializer.Serialize(dict));
                jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync("https://securepay.tinkoff.ru/v2/Init", jsonContent);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public async Task<string> SendGetState(string paymentId)
        {
            var dict = new Dictionary<string, object>()
            {
                { "TerminalKey" , "hgjghjghjkghkh" },
                {"Password" , "ixjhkhja9qq2ofgjghjghkzbb0o4ci"},
                {"PaymentId" , paymentId},
            };
            dict.Add("Token", GetToken(dict));
            using HttpClient client = _httpClientFactory.CreateClient();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using StringContent jsonContent = new(JsonSerializer.Serialize(dict));
                jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync("https://securepay.tinkoff.ru/v2/GetState", jsonContent);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        private string GetToken(Dictionary<string, object> content)
        {
            Dictionary<string, object> values = content;
            values.Add("Password", "ываываывавыа");
            values = values.OrderBy(x => x.Key).ToDictionary();
            string s = string.Join("", values.Values);
            byte[] messageBytes = Encoding.UTF8.GetBytes(s);
            byte[] hashValue = SHA256.HashData(messageBytes);
            content.Remove("Password");
            return Convert.ToHexString(hashValue).ToLower();
        }
        public async Task CancelPayment(string paymentId)
        {
            var dict = new Dictionary<string, object>()
        {
            { "TerminalKey" , "1706276729818DEMO" },
            {"PaymentId" , paymentId},
        };
            dict.Add("Token", GetToken(dict));
            using HttpClient client = _httpClientFactory.CreateClient();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using StringContent jsonContent = new(JsonSerializer.Serialize(dict));
                jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync("https://securepay.tinkoff.ru/v2/Cancel", jsonContent);
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}

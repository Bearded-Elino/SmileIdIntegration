using Microsoft.Extensions.Options;
using SmileIdVerify.Configurations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SmileIdVerify.Services
{
    public class SmileIdService
    {
        private readonly string _partnerId;
        private readonly string _secretKey;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SmileIdService(IOptions<SmileIdConfig> config, HttpClient httpClient)
        {
            _partnerId = config.Value.PartnerId;
            _secretKey = config.Value.SecretKey;
            _httpClient = httpClient;
            _baseUrl = config.Value.BaseUrl;
        }

        public string GenerateSignature(string timeStamp)
        {
            string dataToSign = $"{_partnerId}+{timeStamp}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                return Convert.ToBase64String(hash);
            }
        }


        public async Task<string> VerifyIdentityAsync(string endpointUrl, object requestData)
        {
            string timeStamp = DateTime.UtcNow.ToString("o");
            string signature = GenerateSignature(timeStamp);

            Console.WriteLine($"Timestamp: {timeStamp}");
            Console.WriteLine($"Signature: {signature}");
            Console.WriteLine($"Endpoint url: { endpointUrl}");

            Console.WriteLine($"input request data: {JsonSerializer.Serialize(requestData)}");

            var requestPayload = requestData.GetType()
                .GetProperties()
                .Where(p => p.GetValue(requestData) != null && p.Name.ToLower() != "signature") // Exclude "signature"
                .ToDictionary(
                    p => JsonNamingPolicy.CamelCase.ConvertName(p.Name),
                    p => p.GetValue(requestData)
                );

            requestPayload["signature"] = signature;
            requestPayload["timestamp"] = timeStamp;
            requestPayload["partner_id"] = _partnerId;

            // Serialize to JSON
            string jsonPayload = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"Request Payload: {jsonPayload}");

            // Create the HTTP request
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            Console.WriteLine("Headers:");
            Console.WriteLine($"  Content-Type: application/json");
            Console.WriteLine($"  smileid-partner-id: {_partnerId}");
            Console.WriteLine($"  smileid-request-signature: {signature}");
            Console.WriteLine($"  smileid-timestamp: {timeStamp}");

            // Send the request
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Error: {response.StatusCode}");
                Console.WriteLine($"Response Body: {responseBody}");
                throw new HttpRequestException($"Error: {response.StatusCode}, {responseBody}");
            }

            // Read and return the response body
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> VerifyPhoneNumberAsync(string callbackUrl, string country, string phoneNumber,
                                                 Dictionary<string, string> matchFields)
        {
            string timeStamp = DateTime.UtcNow.ToString("o");
            string signature = GenerateSignature(timeStamp);

            var sanitizedMatchFields = matchFields
                .Where(pair => !string.IsNullOrEmpty(pair.Value))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var requestPayload = new
            {
                callback_url = callbackUrl,
                country = country,
                phone_number = phoneNumber,
                match_fields = sanitizedMatchFields
            };

            string jsonPayload = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"Request Payload: {jsonPayload}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}verify-phone-number")
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("smileid-partner-id", _partnerId);
            requestMessage.Headers.Add("smileid-request-signature", signature);
            requestMessage.Headers.Add("smileid-timestamp", timeStamp);
            requestMessage.Headers.Add("smileid-source-sdk", "rest_api");
            requestMessage.Headers.Add("smileid-source-sdk-version", "2.0.0");

            Console.WriteLine($"Headers: PartnerID={_partnerId}, Signature={signature}, Timestamp={timeStamp}");


            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseBody}");
            return responseBody;
        }


    }

}

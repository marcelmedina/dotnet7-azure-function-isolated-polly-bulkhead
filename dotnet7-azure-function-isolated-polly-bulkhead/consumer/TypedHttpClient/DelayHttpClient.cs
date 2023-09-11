using Polly;

namespace consumer.TypedHttpClient
{
    public class DelayHttpClient
    {
        private readonly HttpClient _httpClient;

        public DelayHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetProducer(string endpoint)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(endpoint));

            // Operation key is used to identify the operation in the cache
            httpRequestMessage.SetPolicyExecutionContext(new Context("GetProducer"));

            var response = await _httpClient.SendAsync(httpRequestMessage);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
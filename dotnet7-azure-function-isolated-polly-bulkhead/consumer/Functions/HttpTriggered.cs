using System.Net;
using consumer.TypedHttpClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace consumer.Functions
{
    public class HttpTriggered
    {
        private readonly IConfiguration _configuration;
        private readonly DelayHttpClient _httpClient;
        private readonly ILogger _logger;

        public HttpTriggered(ILoggerFactory loggerFactory, IConfiguration configuration, DelayHttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = loggerFactory.CreateLogger<HttpTriggered>();
        }

        [Function("HttpTriggered")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation($"### New request");

            try
            {
                var endpoint = _configuration.GetValue<string>(Constants.ProducerEndpoint);

                var message = await _httpClient.GetProducer(endpoint);

                _logger.LogInformation($"### Delay Response = {message}");

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                await response.WriteStringAsync(message);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                await response.WriteStringAsync("Execution was rejected.");
                return response;
            }
        }
    }
}

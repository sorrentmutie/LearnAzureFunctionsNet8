using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace LearnFunctions
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IClock clock;

        public Function1(ILoggerFactory loggerFactory, IClock clock)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            this.clock = clock;
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            var myTime = clock.GetNow();
            response.WriteString($"{myTime}");

            return response;
        }
    }
}

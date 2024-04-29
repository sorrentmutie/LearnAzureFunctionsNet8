using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Azure.Data.Tables;

namespace LearnFunctions
{
    public class MyTimer
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IClock _clock;

        public MyTimer(ILoggerFactory loggerFactory, 
            IHttpClientFactory factory,
            IClock clock)
        {
            _logger = loggerFactory.CreateLogger<MyTimer>();
            _httpClient = factory.CreateClient("RandomUserApi");
            _clock = clock;
        }

        [Function("MyTimer")]
        [QueueOutput("secondqueue")]
        public async Task<string> Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadFromJsonAsync<RandomUserResponse>();

                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                TableClient client = new TableClient(connectionString, "mySecondTable");
                var newRecord = new MyTableData()
                {
                    PartitionKey = "example",
                    RowKey = Guid.NewGuid().ToString(),
                    FirstName = data.results[0].name.first,
                    LastName = data.results[0].name.last
                };
                await client.AddEntityAsync(newRecord);

                return
                    $"Fetched data from RandomUser API: {data.results[0].name.first} {data.results[0].name.last}";
               
            }
            catch (Exception)
            {

              return $"Failed to fetch data from RandomUser API at {_clock.GetNow()}";
            }


            
           
        }
    }
}

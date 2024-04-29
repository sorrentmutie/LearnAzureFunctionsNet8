using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LearnFunctions
{
    public class MyQueueFunction
    {
        private readonly ILogger<MyQueueFunction> _logger;
        private readonly IClock clock;

        public MyQueueFunction(ILogger<MyQueueFunction> logger, IClock clock)
        {
            _logger = logger;
            this.clock = clock;
        }

        [Function(nameof(MyQueueFunction))]
        [QueueOutput("secondqueue")]
       
        public string Run([QueueTrigger("firstqueue", Connection = "StorageQueueConnectionString")] string message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message}");
            var myTime = clock.GetNow();
            var messageOutput = $"{message}: The time is {myTime}";
            // Queue Output messages
            return messageOutput;
        }
    }
}

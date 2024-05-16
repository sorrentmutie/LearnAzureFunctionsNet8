using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LearnFunctions
{
    public static class MySecondDurable
    {
        [Function(nameof(MySecondDurable))]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(MySecondDurable));
            logger.LogInformation("Saying hello.");
            TimeSpan timeout = TimeSpan.FromSeconds(10);
            DateTime deadline = context.CurrentUtcDateTime.Add(timeout);

            using (var cts = new CancellationTokenSource())
            {
                Task activityTask = context.CallActivityAsync("SayHello2");
                Task timeoutTask = context.CreateTimer(deadline, cts.Token);

                Task winner = await Task.WhenAny(activityTask, timeoutTask);
                if (winner == activityTask)
                {
                    // success case
                    cts.Cancel();
                    return true;
                }
                else
                {
                    // timeout case
                    return false;
                }
            }
        }

        [Function(nameof(SayHello2))]
        public static async Task<string> SayHello2([ActivityTrigger] string name, FunctionContext executionContext)
        {
            await Task.Delay(6000);
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [Function("MySecondDurable_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("MySecondDurable_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(MySecondDurable));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
